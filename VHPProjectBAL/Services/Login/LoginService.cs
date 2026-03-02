using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using VHPProjectCommonUtility.Logger;
using VHPProjectCommonUtility.Response;
using VHPProjectDAL.LoginRepo;
using VHPProjectDTOModel.LoginDTO.request;

namespace VHPProjectBAL.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repo;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;

        public LoginService(ILoginRepository repo, IConfiguration config, ILoggerManager logger)
        {
            _repo = repo;
            _config = config;
            _logger = logger;
        }

        private string CleanMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return string.Empty;

            return mobile.Replace("+91", "").Replace(" ", "").Trim();
        }

        public async Task<ResultWithDataDTO<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            string cleanMobile = CleanMobile(request.MobileNumber);
            var user = await _repo.GetByMobileAsync(cleanMobile);

            if (user == null)
            {
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = false,
                    Message = "Mobile number not found"
                };
            }

            string mobileForSms = "+91" + cleanMobile;
            string otp = new Random().Next(100000, 999999).ToString();
            DateTime expireAt = DateTime.Now.AddMinutes(int.Parse(_config["MSG91:OtpExpiry"]));

            await _repo.SaveOtpAsync(cleanMobile, otp, expireAt);

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("authkey", _config["MSG91:AuthKey"]);

                var payload = new
                {
                    template_id = _config["MSG91:TemplateId"],
                    mobile = mobileForSms,
                    otp = otp
                };

                var response = await client.PostAsJsonAsync(_config["MSG91:BaseUrl"], payload);
                response.EnsureSuccessStatusCode();

                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = true,
                    Message = "OTP has been sent to your mobile number"
                };
            }
            catch
            {
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = false,
                    Message = "Failed to send OTP. Please try again."
                };
            }
        }

        public async Task<ResultWithDataDTO<string>> LoginAsync(LoginRequest request)
        {
            var result = new ResultWithDataDTO<string>();

            try
            {
                var user = await _repo.GetByUserNameAsync(request.UserName);

                if (user == null)
                {
                    result.IsSuccessful = false;
                    result.Message = "User not found";
                    return result;
                }

                bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

                if (!valid)
                {
                    result.IsSuccessful = false;
                    result.Message = "Invalid password";
                    return result;
                }

                string token = GenerateJwtToken(user);

                result.IsSuccessful = true;
                result.Message = "Login successful";
                result.Data = token;

                return result;
            }
            catch
            {
                result.IsSuccessful = false;
                result.Message = "An error occurred during login.";
                return result;
            }
        }

        public async Task<ResultWithDataDTO<string>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                string cleanMobile = CleanMobile(request.MobileNumber);
                var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new VHPProjectDAL.DataModel.Login
                {
                    UserName = request.UserName,
                    Password = hashed,
                    MobileNumber = cleanMobile,
                    CreatedAt = DateTime.Now
                };

                await _repo.AddAsync(user);

                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = true,
                    Message = "User registered successfully",
                    Data = user.UserName
                };
            }
            catch
            {
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = false,
                    Message = "An error occurred during registration."
                };
            }
        }

        public async Task<ResultWithDataDTO<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _repo.GetByUserNameAsync(request.UserName);

            if (user == null)
            {
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = false,
                    Message = "User not found"
                };
            }

            string hashed = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _repo.UpdatePasswordAsync(user.UserName, hashed);

            return new ResultWithDataDTO<string>()
            {
                IsSuccessful = true,
                Message = "Password updated successfully"
            };
        }

        public async Task<ResultWithDataDTO<string>> VerifyOtpAsync(OtpVerifyRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MobileNumber) || string.IsNullOrWhiteSpace(request.OtpCode))
            {
                return new ResultWithDataDTO<string>
                {
                    IsSuccessful = false,
                    Message = "Invalid request"
                };
            }

            try
            {
        string cleanMobile = CleanMobile(request.MobileNumber);

        var otpData = await _repo.GetOtpAsync(cleanMobile);

        if (otpData == null)
        {
            return new ResultWithDataDTO<string>
            {
                IsSuccessful = false,
                Message = "OTP not found or expired"
            };
        }
        if (otpData.IsVerified)
        {
            return new ResultWithDataDTO<string>
            {
                IsSuccessful = false,
                Message = "OTP already used"
            };
        }
        if (otpData.ExpireAt <= DateTime.UtcNow)
        {
            return new ResultWithDataDTO<string>
            {
                IsSuccessful = false,
                Message = "OTP expired"
            };
        }
        if (!string.Equals(otpData.OtpCode?.Trim(), request.OtpCode.Trim(), StringComparison.Ordinal))
        {
            return new ResultWithDataDTO<string>
            {
                IsSuccessful = false,
                Message = "Invalid OTP"
            };
        }
        await _repo.MarkOtpVerifiedAsync(otpData.Id);

        return new ResultWithDataDTO<string>
        {
            IsSuccessful = true,
            Message = "OTP verified successfully",
            Data = cleanMobile
        };
    }

            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error verifying OTP for mobile {request?.MobileNumber}",
                    ex
                );

                return new ResultWithDataDTO<string>
                {
                    IsSuccessful = false,
                    Message = "An internal error occurred"
                };
            }



        }



        private string GenerateJwtToken(VHPProjectDAL.DataModel.Login user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId", user.IdLogin.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
