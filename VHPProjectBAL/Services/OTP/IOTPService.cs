using VHPProjectDTOModel.OTPDTO.response;

namespace VHPProjectBAL.Services.OTP
{
    public interface IOTPService
    {
        Task<OTPResponseDTO> SendOTPAsync(string mobileNumber);
        Task<OTPResponseDTO> VerifyOTPAsync(string mobileNumber, string otp);
        Task<OTPResponseDTO> ResendOTPAsync(string mobileNumber);
    }
}
