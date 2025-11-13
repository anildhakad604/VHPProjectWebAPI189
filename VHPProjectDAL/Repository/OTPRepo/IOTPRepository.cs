using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDAL.Repository.OTPRepo
{
    public interface IOTPRepository
    {
        Task<HttpResponseMessage> SendOTPAsync(string mobileNumber);
        Task<HttpResponseMessage> VerifyOTPAsync(string mobileNumber, string otp);
        Task<HttpResponseMessage> ResendOTPAsync(string mobileNumber);
    }
}
