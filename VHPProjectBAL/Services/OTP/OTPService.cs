using VHPProjectBAL.Services.OTP;
using VHPProjectDAL.Repository.OTPRepo;
using VHPProjectDTOModel.OTPDTO.response;

namespace VHPProjectBAL.OTPService
{
    public class OTPService:IOTPService
    {
        private readonly IOTPRepository _otpRepository;

        public OTPService(IOTPRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task<OTPResponseDTO> SendOTPAsync(string mobileNumber)
        {
            var response = await _otpRepository.SendOTPAsync(mobileNumber);
            if (response.IsSuccessStatusCode)
            {
                return new OTPResponseDTO { Data = 1, Message = "OTP Sent Successfully" };
            }
            return new OTPResponseDTO { Data = 0, Message = "Failed to send OTP" };
        }

        public async Task<OTPResponseDTO> VerifyOTPAsync(string mobileNumber, string otp)
        {
            var response = await _otpRepository.VerifyOTPAsync(mobileNumber, otp);
            if (response.IsSuccessStatusCode)
            {
                return new OTPResponseDTO { Data = 1, Message = "OTP Verified Successfully" };
            }
            return new OTPResponseDTO { Data = 0, Message = "OTP Verification Failed" };
        }

        public async Task<OTPResponseDTO> ResendOTPAsync(string mobileNumber)
        {
            var response = await _otpRepository.ResendOTPAsync(mobileNumber);
            if (response.IsSuccessStatusCode)
            {
                return new OTPResponseDTO { Data = 1, Message = "OTP Resent Successfully" };
            }
            return new OTPResponseDTO { Data = 0, Message = "Failed to resend OTP" };
        }
    }
}
