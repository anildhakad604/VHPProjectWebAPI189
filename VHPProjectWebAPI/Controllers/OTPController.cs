using Microsoft.AspNetCore.Mvc;
using VHPProjectBAL.Services.OTP;
using VHPProjectDTOModel.OTPDTO.request;

namespace VHPProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
        private readonly IOTPService _otpService;

        public OTPController(IOTPService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPRequestDTO request)
        {
            var result = await _otpService.SendOTPAsync(request.MobileNumber);
            return Ok(result);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequestDTO request)
        {
            var result = await _otpService.VerifyOTPAsync(request.MobileNumber, request.OTP);
            return Ok(result);
        }

        [HttpPost("ResendOTP")]
        public async Task<IActionResult> ResendOTP([FromBody] SendOTPRequestDTO request)
        {
            var result = await _otpService.ResendOTPAsync(request.MobileNumber);
            return Ok(result);
        }

    }
}
