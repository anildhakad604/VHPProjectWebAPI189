using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.LoginRepo
{
    public interface ILoginRepository
    {
        Task<Login> GetByUserNameAsync(string username);
        Task<Login> GetByMobileAsync(string mobileNumber);
        Task AddAsync(Login user);

        Task SaveOtpAsync(string mobile, string otp, DateTime expireAt);
        Task<Userotp> GetOtpAsync(string mobile);
        Task MarkOtpVerifiedAsync(int otpId);
        Task UpdatePasswordAsync(string userName, string newPassword);
    }
}
