using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VHPProjectBAL.Services.OTP;
using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.DataModel;
using VHPProjectDAL.Repository.MemberRepo;
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
        private readonly IHostEnvironment _env;
        private readonly ILoggerManager _loggerManager;
        private readonly MasterProjContext _context;
        public MemberService(IMemberRepository memberRepository,
            IOTPService otpService, IConfiguration config,
            IHostEnvironment env, IExcelService excelService,
            ILoggerManager loggerManager,
            MasterProjContext context
            
            )
        {
            _memberRepository = memberRepository;
            _excelService = excelService;
            _otpService = otpService;
            _config = config;
            _env = env;
            _loggerManager = loggerManager;
            _context = context;

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

                _loggerManager.LogInfo("Reading members from Excel...");

                var extension = Path.GetExtension(file.FileName);
                if (extension != ".xlsx")
                {
                    response.ValidationMessages.Add("Invalid file format. Please upload an Excel (.xlsx) file.");
                    return response;
                }

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
                    .Where((m, i) => !response.ValidationMessages.Any(vm => vm.StartsWith($"Record {i + 2}:")))
                    .ToList();

                if (validMembers.Any())
                {
                    _loggerManager.LogInfo($"Inserting {validMembers.Count} valid member records...");
                    await _memberRepository.AddMembers(validMembers);
                }

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
                return Array.Empty<byte>();

            return _excelService.GenerateMemberExcelFile(members);
        }

        public async Task<Member> GetMemberByIdAsync(int memberId)
        {
            return await _memberRepository.GetMemberByIdAsync(memberId);
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

                //var newAccessToken = GenerateJwtRefreshToken(memberRecords);
                //resultWithDataDTO.IsSuccessful = true;
                //resultWithDataDTO.Message = "New token generated succesfully.";
                //resultWithDataDTO.Data = new MemberResponseRefreshToken { AccessToken = newAccessToken, RefreshToken = refreshTokenDetails.Value };
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
                IsUsed = false,          // Should be false at creation (not used yet)
                IsRevoked = false,
                CreatedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),  // Token valid for 7 days
                
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

        //private string GenerateJwtRefreshToken(MemberRecord_DTO memberRecord_DTO)
        //{
        //    var jwtSettings = _config.GetSection("Jwt");
        //    var key = jwtSettings["Key"];
        //    var keyBytes = Encoding.UTF8.GetBytes(key);
        //    var signingKey = new SymmetricSecurityKey(keyBytes);

        //    var claims = new List<Claim>
        //    {
        //        new Claim("MemberId", memberRecord_DTO.MemberId.ToString()),
        //        new Claim("Is_Core", memberRecord_DTO.Is_Core.ToString()),
        //        new Claim("Is_Admin", memberRecord_DTO.Is_Admin.ToString()),
        //        new Claim("Village_Master_Id", memberRecord_DTO.Village_Master_Id.ToString()),
        //        new Claim("Taluka_Master_Id", memberRecord_DTO.Taluka_Master_Id.ToString()),
        //        new Claim("Designation_Id", memberRecord_DTO.Designation_Id.ToString())
        //    };

        //    var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: jwtSettings["Issuer"],
        //        audience: jwtSettings["Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(5),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}


        public MemberResponseDTO AddMember(AddMemberRequestDTO request)
        {
            if (!_memberRepository.IsMobileUnique(request.MobileNumber))
                return new MemberResponseDTO { Data = 0, Message = "MobileNumber must be unique" };

            var member = new Member
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MobileNumber = request.MobileNumber,
                DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth)
            };
            _memberRepository.AddMember(member);
            return new MemberResponseDTO { Data = 1, Message = "Member added successfully" };
        }

        public MemberResponseDTO UpdateMember(UpdateMemberRequestDTO request)
        {
            var member = _memberRepository.GetMember(request.MemberId);
            if (member == null) return new MemberResponseDTO { Data = 0, Message = "Member not found" };

            if (!_memberRepository.IsMobileUnique(request.MobileNumber, request.MemberId))
                return new MemberResponseDTO { Data = 0, Message = "MobileNumber must be unique" };

            member.FirstName = request.FirstName;
            member.LastName = request.LastName;
            member.MobileNumber = request.MobileNumber;
            //member.DateOfBirth = request.DateOfBirth;

            _memberRepository.UpdateMember(member);
            return new MemberResponseDTO { Data = 1, Message = "Member updated successfully" };
        }

        public MemberResponseDTO GetMember(int memberId)
        {
            var member = _memberRepository.GetMember(memberId);
            if (member == null) return new MemberResponseDTO { Data = 0, Message = "Member not found" };

            return new MemberResponseDTO { Data = 1, Message = "Member retrieved successfully" };
        }

        public MemberResponseDTO DeleteMember(int memberId)
        {
            var member = _memberRepository.GetMember(memberId);
            if (member == null) return new MemberResponseDTO { Data = 0, Message = "Member not found" };

            _memberRepository.DeleteMember(member);
            return new MemberResponseDTO { Data = 1, Message = "Member deleted successfully" };
        }

    }
}
