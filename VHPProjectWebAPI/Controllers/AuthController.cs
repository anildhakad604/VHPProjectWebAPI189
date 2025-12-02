using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.Login;
using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.LoginDTO.request;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _service;

        public AuthController(ILoginService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            ResultWithDataDTO<string> response = await _service.LoginAsync(request);

            if (!response.IsSuccessful)
                return BadRequest(response); 

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            ResultWithDataDTO<string> response = await _service.RegisterAsync(request);

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response); 
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _service.ForgotPasswordAsync(request);

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            var response = await _service.VerifyOtpAsync(request);

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _service.ResetPasswordAsync(request);

            if (!response.IsSuccessful)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
