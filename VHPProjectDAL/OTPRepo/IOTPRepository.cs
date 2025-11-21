namespace VHPProjectDAL.OTPRepo
{
    public interface IOTPRepository
    {
        Task<HttpResponseMessage> SendOTPAsync(string mobileNumber);
        Task<HttpResponseMessage> VerifyOTPAsync(string mobileNumber, string otp);
        Task<HttpResponseMessage> ResendOTPAsync(string mobileNumber);
    }
}
