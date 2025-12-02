using VHPProjectCommonUtility.Response;
using VHPProjectDTOModel.LoginDTO.request;

namespace VHPProjectBAL.Services.Login
{
    public interface ILoginService
    {
        Task<ResultWithDataDTO<string>> LoginAsync(LoginRequest request);
        Task<ResultWithDataDTO<string>> RegisterAsync(RegisterRequest request);

        Task<ResultWithDataDTO<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ResultWithDataDTO<string>> VerifyOtpAsync(OtpVerifyRequest request);
        Task<ResultWithDataDTO<string>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
