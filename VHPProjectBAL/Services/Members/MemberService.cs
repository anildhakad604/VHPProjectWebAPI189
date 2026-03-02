using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using VHPProjectBAL.Services.OTP;
using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.DataModel;
using VHPProjectDAL.MemberRepo;
using VHPProjectDTOModel.MemberDTO.request;
using VHPProjectDTOModel.MemberDTO.responses;

namespace VHPProjectBAL.Services.Members
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IExcelService _excelService;
        private readonly IOTPService _otpService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerManager _loggerManager;
        private readonly MasterProjContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FileStorageSettings _fileSettings;


        public MemberService(IMemberRepository memberRepository,
            IOTPService otpService,
            IConfiguration config,
            IWebHostEnvironment env, IExcelService excelService,
            ILoggerManager loggerManager,
            MasterProjContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<FileStorageSettings> fileSettings

            )
        {
            _memberRepository = memberRepository;
            _excelService = excelService;
            _otpService = otpService;
            _config = config;
            _env = env;
            _loggerManager = loggerManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _fileSettings = fileSettings.Value;

        }



        public async Task<MemberUploadResponseDTO> ProcessMemberExcelAsync(IFormFile file, string pathName)
        {
            var response = new MemberUploadResponseDTO
            {
                ValidationMessages = new List<string>(),
                TotalValidRecords = 0
            };

            try
            {
                _loggerManager.LogInfo("ProcessMemberExcelAsync started...");

                
                var extension = Path.GetExtension(file.FileName);
                if (extension != ".xlsx")
                {
                    response.ValidationMessages.Add("Invalid file format. Please upload an Excel (.xlsx) file.");
                    return response;
                }

                
                _loggerManager.LogInfo("Reading members from Excel...");
                var members = await _excelService.ReadMembersFromExcelAsync(file);

                int rowNumber = 2;

                foreach (var m in members)
                {
                    var errors = new List<string>();

                    if (string.IsNullOrWhiteSpace(m.FirstName))
                        errors.Add("Name is required");

                    if (string.IsNullOrWhiteSpace(m.MobileNumber))
                        errors.Add("Mobile number is required");

                    
                    if (errors.Any())
                        response.ValidationMessages.Add($"Record {rowNumber}: {string.Join(", ", errors)}");
                    else
                        response.TotalValidRecords++;

                    rowNumber++;
                }

                var validMembers = members
                    .Where((m, index) =>
                        !response.ValidationMessages.Any(msg => msg.StartsWith($"Record {index + 2}:")))
                    .ToList();

                if (validMembers.Any())
                {
                    _loggerManager.LogInfo($"Inserting {validMembers.Count} valid member records to database...");
                    await _memberRepository.AddMembers(validMembers);
                }

                _loggerManager.LogInfo("ProcessMemberExcelAsync completed successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError("Error processing member Excel file.", ex);
                response.ValidationMessages.Add("Error reading Excel file or inserting records.");
                return response;
            }
        }


        public async Task<byte[]> GenerateMemberExcelAsync(int? villageMasterId, int? talukaMasterId)
        {
            _loggerManager.LogInfo("Generating member Excel file...");

            var members = await _memberRepository.GetMembersByFilter(villageMasterId, talukaMasterId);

            if (members == null || !members.Any())
            {
                _loggerManager.LogWarn("No members found for the given filters.");
                return Array.Empty<byte>();
            }

            var excelBytes = _excelService.GenerateMemberExcelFile(members);

            _loggerManager.LogInfo("Excel file generated successfully.");

            return excelBytes;
        }


        public async Task<ResultWithDataDTO<MemberResponseDTO>> GetMemberAsync(int memberId)
        {
            var result = new ResultWithDataDTO<MemberResponseDTO>();

            try
            {
                var member = await _memberRepository.GetMemberByIdAsync(memberId);

                if (member == null)
                {
                    result.IsSuccessful = false;
                    result.IsBusinessError = true;
                    result.Message = "Member not found";
                    return result;
                }

                result.Data = new MemberResponseDTO
                {
                    MemberId = member.MemberId,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    MobileNumber = member.MobileNumber
                };

                result.IsSuccessful = true;
                result.Message = "Member retrieved successfully";
            }
            catch (Exception ex)
            {
                _loggerManager.LogError("Error while retrieving member", ex);

                result.IsSuccessful = false;
                result.IsSystemError = true;
                result.Message = "Error fetching member";
                result.SystemErrorMessage = ex.Message;
            }

            return result;
        }


        public async Task<ResultWithDataDTO<int>> VerifyLogout(MemberRequestRefreshToken_DTO refreshToken_DTO)
        {
            _loggerManager.LogInfo("Entry MemberService => RefreshToken");
            var resultWithDataDTO = new ResultWithDataDTO<int>()
            {
                IsSuccessful = false
            };

            var refreshTokenDetails = await _memberRepository.GetRefreshTokenDetails(refreshToken_DTO.RefreshToken);
            if (refreshTokenDetails == null)
            {
                return new ResultWithDataDTO<int>
                {
                    Data = 0,
                    Message = "Unauthorized",
                    IsSuccessful = false
                };
            }

            var updateRefreshToken = await _memberRepository.UpdateRefreshTokenAsync(refreshTokenDetails);
            if (refreshToken_DTO.RefreshToken == refreshTokenDetails.Value && refreshTokenDetails.IsUsed == true)
            {
                refreshTokenDetails.IsUsed = false;
                await _memberRepository.UpdateRefreshTokenAsync(refreshTokenDetails);
                await _context.SaveChangesAsync();

                return new ResultWithDataDTO<int>
                {
                    Data = 1,
                    Message = "Logout succesfully",
                    IsSuccessful = true
                };
            }

            _loggerManager.LogInfo("Exit MemberService => RefreshToken");
            return resultWithDataDTO;
        }

        public async Task<(MobileExistResponseDTO, string message)> MobileExistAsync(MobileExistRequestDTO request)
        {
            var member = await _memberRepository.GetMemberByMobileAsync(request.MobileNumber);

            if (member != null)
            {

                await _otpService.SendOTPAsync(request.MobileNumber);

                return (new MobileExistResponseDTO { Status = 1 }, "Mobile number exists");
            }

            return (new MobileExistResponseDTO { Status = 0 }, "Mobile number does not exist");
        }

        public async Task<ResultWithDataDTO<MemberResponseRefreshToken>> GetRefreshToken(MemberRequestRefreshToken_DTO refreshToken_DTO, HttpContext httpContext)
        {
            _loggerManager.LogInfo("Entry MemberService => RefreshToken");
            var resultWithDataDTO = new ResultWithDataDTO<MemberResponseRefreshToken>()
            {
                IsSuccessful = false
            };

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            string? expiredAccessToken = null;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                expiredAccessToken = authHeader.Substring("Bearer ".Length);
            else
            {
                resultWithDataDTO.Message = "Access token missing in header.";
                return resultWithDataDTO;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(expiredAccessToken);

            var memberIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "MemberId");
            if (memberIdClaim == null)
            {
                resultWithDataDTO.Message = "Invalid Access Token.";
                return resultWithDataDTO;
            }

            int memberId = int.Parse(memberIdClaim.Value);
            var member = await _memberRepository.GetPlainMemberById(memberId);
            if (member == null)
            {
                resultWithDataDTO.Message = "Member not found for the provided token.";
                return resultWithDataDTO;
            }
            var memberRecords = new MemberRecord_DTO
            {
                MemberId = member.MemberId,
                Village_Master_Id = member.VillageMasterId,
                Taluka_Master_Id = member.TalukaMasterId,

            };

            var refreshTokenDetails = await _memberRepository.GetRefreshTokenDetails(refreshToken_DTO.RefreshToken);

            if (refreshTokenDetails == null)
            {
                resultWithDataDTO.Message = "Invalid or expired refresh token.";
                return resultWithDataDTO;
            }



            if (refreshTokenDetails.ExpiryDate < DateTime.Now && refreshTokenDetails.IsUsed == true)
            {

                resultWithDataDTO.Message = "Refresh token expired or already used.";
                return resultWithDataDTO;

                
            }
            var newAccessToken = GenerateJwtRefreshToken(memberRecords);
            refreshTokenDetails.IsUsed = true;
            await _memberRepository.UpdateRefreshTokenAsync(refreshTokenDetails);
            await _context.SaveChangesAsync();

            resultWithDataDTO.IsSuccessful = true;
            resultWithDataDTO.Message = "New token generated successfully.";
            resultWithDataDTO.Data = new MemberResponseRefreshToken
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshTokenDetails.Value
            };


            _loggerManager.LogInfo("Exit MemberService => GetRefreshToken");
            return resultWithDataDTO;
        }

        private string GenerateJwtRefreshToken(MemberRecord_DTO memberRecord_DTO)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = jwtSettings["Key"];
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            var claims = new List<Claim>
            {
                new Claim("MemberId", memberRecord_DTO.MemberId.ToString()),
                new Claim("Is_Core", memberRecord_DTO.Is_Core.ToString()),
                new Claim("Is_Admin", memberRecord_DTO.Is_Admin.ToString()),
                new Claim("Village_Master_Id", memberRecord_DTO.Village_Master_Id.ToString()),
                new Claim("Taluka_Master_Id", memberRecord_DTO.Taluka_Master_Id.ToString()),
                new Claim("Designation_Id", memberRecord_DTO.Designation_Id.ToString())
            };

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(VerifyLoginResponseDTO, string message)> VerifyLoginAsync(VerifyLoginRequestDTO request)
        {


            var otpResponse = await _otpService.VerifyOTPAsync(request.MobileNumber, request.OTP);
            if (otpResponse.Data == 0)
            {
                return (new VerifyLoginResponseDTO(), "Invalid or expired OTP");
            }


            var member = await _memberRepository.GetMemberByMobileAsync(request.MobileNumber);
            if (member == null)
            {
                return (new VerifyLoginResponseDTO(), "Member not found");
            }



            var token = GenerateJwtToken(member);

            var refreshToken = Guid.NewGuid().ToString();


            await _memberRepository.AddRefreshTokenAsync(new Refreshtoken
            {
                Value = refreshToken,
                IsUsed = false,          
                IsRevoked = false,
                CreatedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),  

            });

            return (new VerifyLoginResponseDTO { AccessToken = token, RefreshToken = refreshToken }, "Login successful");
        }

        private string GenerateJwtToken(Member member)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("MemberId", member.MemberId.ToString()),

                new Claim("Village_Master_Id", member.VillageMasterId.ToString()),
                new Claim("Taluka_Master_Id", member.TalukaMasterId.ToString()),

            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResultWithDataDTO<List<string>>> UploadFilesAsync(IFormFile[] files, string pathName)
        {
            var result = new ResultWithDataDTO<List<string>> { Data = new List<string>() };

            try
            {
                string folderPath = Path.Combine(_fileSettings.BasePath, pathName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Get Host URL => https://localhost:44329
                var request = _httpContextAccessor.HttpContext.Request;
                string hostUrl = $"{request.Scheme}://{request.Host}";

                foreach (var file in files)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string ext = Path.GetExtension(file.FileName);
                    string fileName = $"image_{timestamp}{ext}";

                    string fullPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                   
                    string fileUrl = $"{hostUrl}/Files/{pathName}/{fileName}";

                    result.Data.Add(fileUrl);
                }

                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
               
                result.IsSuccessful = false;
                result.Message = "File upload failed.";
            }

            return result;
        }


        public async Task<ResultWithDataDTO<int>> DeleteFilesAsync(List<string> filePaths)
        {
            var result = new ResultWithDataDTO<int>();

            foreach (var relative in filePaths)
            {
                string fullPath = Path.Combine(_fileSettings.BasePath, relative);

                try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    else
                    {
                        result.ValidationMessages.Add($"{relative} not deleted.");
                    }
                }
                catch
                {
                    result.ValidationMessages.Add($"{relative} not deleted.");
                }
            }

            result.Data = 1;
            result.IsSuccessful = !result.ValidationMessages.Any();
            return result;
        }



        public async Task<ResultWithDataDTO<byte[]>> DownloadFilesAsZipAsync(List<string> filePaths)
        {
            var result = new ResultWithDataDTO<byte[]>();

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        foreach (var relativePath in filePaths)
                        {
                            string fullPath = Path.Combine(_fileSettings.BasePath, relativePath);

                            if (File.Exists(fullPath))
                            {
                                var zipEntry = zip.CreateEntry(Path.GetFileName(fullPath));

                                using var entryStream = zipEntry.Open();
                                using var fileStream = File.OpenRead(fullPath);

                                await fileStream.CopyToAsync(entryStream);
                            }
                            else
                            {
                                result.ValidationMessages.Add($"{relativePath} not found.");
                            }
                        }
                    }

                    if (result.ValidationMessages.Any())
                    {
                        result.IsSuccessful = false;
                        result.Message = "No valid files found to download.";
                        return result;
                    }

                    result.Data = ms.ToArray();
                    result.IsSuccessful = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                //_loggerManager.LogError(ex.ToString());
                result.IsSuccessful = false;
                result.Message = "Error while creating zip file.";
                return result;
            }
        }




        //public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string pathName)
        //{
        //    List<string> fileUrls = new List<string>();
        //    string folderPath = Path.Combine(_env.WebRootPath, "Files", pathName);

        //    if (!Directory.Exists(folderPath))
        //        Directory.CreateDirectory(folderPath);

        //    foreach (var file in files)
        //    {
        //        string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //        string fileExt = Path.GetExtension(file.FileName);
        //        string fileName = $"image_{timeStamp}{fileExt}";
        //        string filePath = Path.Combine(folderPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        string url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +
        //                     $"{_httpContextAccessor.HttpContext.Request.Host.Value}/Files/{pathName}/{fileName}";

        //        fileUrls.Add(url);
        //    }

        //    return fileUrls;
        //}



        //public object DeleteFiles(List<string> filePaths)
        //{
        //    List<string> errors = new();
        //    int deletedCount = 0;

        //    foreach (var path in filePaths)
        //    {
        //        try
        //        {
        //            var physicalPath = Path.Combine(_env.WebRootPath, path.Replace("/", "\\").TrimStart('\\'));

        //            if (File.Exists(physicalPath))
        //            {
        //                File.Delete(physicalPath);
        //                deletedCount++;
        //            }
        //            else
        //            {
        //                errors.Add($"{path} not deleted.");
        //            }
        //        }
        //        catch
        //        {
        //            errors.Add($"{path} not deleted.");
        //        }
        //    }

        //    if (errors.Any())
        //        return new { validationMessages = errors };

        //    return new { data = deletedCount };
        //}



        //public async Task<byte[]> DownloadFilesAsZipAsync(List<string> filePaths)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
        //        {
        //            foreach (var path in filePaths)
        //            {
        //                var physicalPath = Path.Combine(_env.WebRootPath, path.Replace("/", "\\").TrimStart('\\'));

        //                if (!File.Exists(physicalPath))
        //                    continue;

        //                var fileBytes = await File.ReadAllBytesAsync(physicalPath);

        //                var entry = archive.CreateEntry(Path.GetFileName(physicalPath));

        //                using (var entryStream = entry.Open())
        //                {
        //                    await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length);
        //                }
        //            }
        //        }

        //        return ms.ToArray();
        //    }




        //public async Task<ResultWithDataDTO<List<string>>> UploadFilesAsync(List<IFormFile> files, string pathName)
        //{
        //    var result = new ResultWithDataDTO<List<string>> { Data = new List<string>() };

        //    try
        //    {
        //        if (files == null || !files.Any())
        //        {
        //            result.IsSuccessful = false;
        //            result.ValidationMessages.Add("No files uploaded.");
        //            return result;
        //        }

        //        string folderPath = Path.Combine(_env.WebRootPath, "Files", pathName);
        //        if (!Directory.Exists(folderPath))
        //            Directory.CreateDirectory(folderPath);

        //        foreach (var file in files)
        //        {
        //            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //            string fileExt = Path.GetExtension(file.FileName);
        //            string fileName = $"image_{timeStamp}{fileExt}";
        //            string filePath = Path.Combine(folderPath, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            string url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}/Files/{pathName}/{fileName}";
        //            result.Data.Add(url);
        //        }

        //        _loggerManager.LogInfo($"{files.Count} files uploaded successfully to {pathName}");
        //    }
        //    catch (Exception ex)
        //    {
        //        //_loggerManager.LogError($"Error uploading files: {ex.Message}");
        //        result.IsSuccessful = false;
        //        result.SystemErrorMessage = ex.Message;
        //    }

        //    return result;
        //}

        //public ResultWithDataDTO<int> DeleteFiles(List<string> filePaths)
        //{
        //    var result = new ResultWithDataDTO<int> { Data = 0 };

        //    try
        //    {
        //        List<string> errors = new();

        //        foreach (var path in filePaths)
        //        {
        //            var physicalPath = Path.Combine(_env.WebRootPath, path.Replace("/", "\\").TrimStart('\\'));

        //            if (File.Exists(physicalPath))
        //            {
        //                File.Delete(physicalPath);
        //                result.Data++;
        //            }
        //            else
        //            {
        //                errors.Add($"{path} not deleted.");
        //                _loggerManager.LogWarn($"{path} not found for deletion.");
        //            }
        //        }

        //        if (errors.Any())
        //        {
        //            result.IsSuccessful = false;
        //            result.ValidationMessages = errors;
        //        }
        //        else
        //        {
        //            _loggerManager.LogInfo($"{result.Data} files deleted successfully.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //_loggerManager.LogError($"Error deleting files: {ex.Message}");
        //        result.IsSuccessful = false;
        //        result.SystemErrorMessage = ex.Message;
        //    }

        //    return result;
        //}

        //public async Task<ResultWithDataDTO<byte[]>> DownloadFilesAsZipAsync(List<string> filePaths)
        //{
        //    var response = new ResultWithDataDTO<byte[]>();

        //    // Base folder = wwwroot
        //    var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        //    var validFiles = new List<string>();

        //    foreach (var relativePath in filePaths)
        //    {
        //        var fullPath = Path.Combine(rootPath, relativePath);

        //        if (File.Exists(fullPath))
        //        {
        //            validFiles.Add(fullPath);
        //        }
        //    }

        //    if (!validFiles.Any())
        //    {
        //        response.Message = "No valid files found to download.";
        //        response.IsSuccessful = false;
        //        return response;
        //    }

        //    // Create ZIP in memory
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        using (var zip = new System.IO.Compression.ZipArchive(memoryStream,
        //                   System.IO.Compression.ZipArchiveMode.Create, true))
        //        {
        //            foreach (var file in validFiles)
        //            {
        //                var zipEntry = zip.CreateEntry(Path.GetFileName(file));

        //                using (var entryStream = zipEntry.Open())
        //                using (var fileStream = File.OpenRead(file))
        //                {
        //                    await fileStream.CopyToAsync(entryStream);
        //                }
        //            }
        //        }

        //        response.Data = memoryStream.ToArray();
        //    }

        //    response.IsSuccessful = true;
        //    response.Message = "Files downloaded successfully.";
        //    return response;
        //}

    }
}
