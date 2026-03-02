using Microsoft.EntityFrameworkCore;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.DataModel;

namespace VHPProjectDAL.LoginRepo
{
    public class LoginRepository : ILoginRepository
    {
        private readonly MasterProjContext _context;
        private readonly ILoggerManager _logger;

        public LoginRepository(MasterProjContext context, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Login user)
        {
            try
            {
                await _context.Login.AddAsync(user);
                await _context.SaveChangesAsync();
                _logger.LogInfo($"User {user.UserName} added to database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Login> GetByMobileAsync(string mobileNumber)
        {
            try
            {
                mobileNumber = mobileNumber.Trim();
                return await _context.Login
                    .FirstOrDefaultAsync(u => u.MobileNumber.Trim() == mobileNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetByMobileAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Login> GetByUserNameAsync(string username)
        {
            try
            {
                return await _context.Login.FirstOrDefaultAsync(u => u.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetByUserNameAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Userotp?> GetOtpAsync(string mobile)
        {

            if (string.IsNullOrWhiteSpace(mobile))
                return null;

            try
            {
                mobile = mobile.Trim();

                return await _context.Userotp
                    .AsNoTracking()
                    .Where(x => x.MobileNumber.Trim() == mobile)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOtpAsync: {ex.Message}");
                throw;
            }
        }

        public async Task MarkOtpVerifiedAsync(int otpId)
        {
            try
            {
                var otp = await _context.Userotp.FirstOrDefaultAsync(x => x.Id == otpId);
                if (otp != null)
                {
                    otp.IsVerified = true;
                    await _context.SaveChangesAsync();
                }

                _logger.LogInfo($"OTP id={otpId} marked as verified");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MarkOtpVerifiedAsync: {ex.Message}");
                throw;
            }
        }

        public async Task SaveOtpAsync(string mobile, string otp, DateTime expireAt)
        {
            try
            {
                mobile = mobile.Trim();

                var userOtp = new Userotp
                {
                    MobileNumber = mobile,
                    OtpCode = otp,
                    ExpireAt = expireAt,
                    IsVerified = false
                };

                await _context.Userotp.AddAsync(userOtp);
                await _context.SaveChangesAsync();

                _logger.LogInfo($"OTP saved for Mobile {mobile}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SaveOtpAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdatePasswordAsync(string userName, string newPassword)
        {
            try
            {
                var user = await _context.Login
                    .FirstOrDefaultAsync(x => x.UserName == userName);

                if (user != null)
                {
                    user.Password = newPassword;
                    await _context.SaveChangesAsync();
                    _logger.LogInfo($"Password updated for User {userName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdatePasswordAsync: {ex.Message}");
                throw;
            }
        }
    }
}
