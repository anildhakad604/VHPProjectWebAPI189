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

        public async Task<ResultWithDataDTO<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _repo.GetByMobileAsync(request.MobileNumber);
            if (user == null)
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = false,
                    Message = "Mobile number not found"
                };

           
            string mobileWithCountryCode = request.MobileNumber.StartsWith("+")
                ? request.MobileNumber
                : "+91" + request.MobileNumber;

            string otp = new Random().Next(100000, 999999).ToString();
            DateTime expireAt = DateTime.Now.AddMinutes(int.Parse(_config["MSG91:OtpExpiry"]));

            await _repo.SaveOtpAsync(mobileWithCountryCode, otp, expireAt);

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("authkey", _config["MSG91:AuthKey"]);

                var payload = new
                {
                    template_id = _config["MSG91:TemplateId"],
                    mobile = mobileWithCountryCode,
                    otp = otp
                };

                var response = await client.PostAsJsonAsync(_config["MSG91:BaseUrl"], payload);
                response.EnsureSuccessStatusCode();

                _logger.LogInfo($"OTP for {mobileWithCountryCode} generated and sent via MSG91.");
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = true,
                    Message = "OTP has been sent to your mobile number"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending OTP via MSG91: {ex.Message}");
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
                    _logger.LogWarn($"Login failed. Username: {request.UserName} not found.");
                    return result;
                }

                bool valid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!valid)
                {
                    result.IsSuccessful = false;
                    result.Message = "Invalid password";
                    _logger.LogWarn($"Login failed. Invalid password for user: {request.UserName}");
                    return result;
                }

                string token = GenerateJwtToken(user);

                result.IsSuccessful = true;
                result.Message = "Login successful";
                result.Data = token;

                _logger.LogInfo($"User {request.UserName} logged in successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in LoginAsync: {ex.Message}");
                result.IsSuccessful = false;
                result.Message = "An error occurred during login.";
                return result;
            }
        }

        public async Task<ResultWithDataDTO<string>> RegisterAsync(RegisterRequest request)
        {
            var result = new ResultWithDataDTO<string>();

            try
            {
                var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new VHPProjectDAL.DataModel.Login
                {
                    UserName = request.UserName,
                    Password = hashed,
                    MobileNumber = request.MobileNumber,
                    CreatedAt = DateTime.Now
                };

                await _repo.AddAsync(user);

                result.IsSuccessful = true;
                result.Message = "User registered successfully";
                result.Data = user.UserName;

                _logger.LogInfo($"User {request.UserName} registered successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in RegisterAsync: {ex.Message}");
                result.IsSuccessful = false;
                result.Message = "An error occurred during registration.";
                return result;
            }
        }

        public async Task<ResultWithDataDTO<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _repo.GetByUserNameAsync(request.UserName);

            if (user == null)
                return new ResultWithDataDTO<string>()
                {
                    IsSuccessful = false,
                    Message = "User not found"
                };

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
            string mobileWithCountryCode = request.MobileNumber.StartsWith("+")
                ? request.MobileNumber
                : "+91" + request.MobileNumber;

            var otpData = await _repo.GetOtpAsync(mobileWithCountryCode);

            if (otpData == null || otpData.OtpCode != request.Otp)
                return new ResultWithDataDTO<string>() { IsSuccessful = false, Message = "Invalid OTP" };

            if (otpData.ExpireAt < DateTime.Now)
                return new ResultWithDataDTO<string>() { IsSuccessful = false, Message = "OTP expired" };

            await _repo.MarkOtpVerifiedAsync(otpData.Id);

            return new ResultWithDataDTO<string>() { IsSuccessful = true, Message = "OTP verified" };
        }

        private string GenerateJwtToken(VHPProjectDAL.DataModel.Login user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId", user.IdLogin.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

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
