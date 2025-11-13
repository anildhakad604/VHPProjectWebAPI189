//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json.Linq;
//using MasterProjCommonUtility.Logger;
//using MasterProjDAL.DataModel;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace MasterProjDAL.JWTService
//{
//    public class JWTService : IJWTService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly ILoggerManager _loggerManager;
//        private readonly IMasterProjContext _opsHubContext;
//        public JWTService(IConfiguration configuration, IMasterProjContext opsHubContext,ILoggerManager loggerManager) 
//        {
//            _configuration = configuration;
//            _loggerManager = loggerManager;
//            _opsHubContext = opsHubContext;
//        }

//        public async Task<RefreshTokens> AddRefreshToken(RefreshTokens token)
//        {
//            _loggerManager.LogInfo("Entry JWTService => AddRefreshToken");
//            var dataresult = await _opsHubContext.RefreshTokens.AddAsync(token);
//            await _opsHubContext.SaveChangesAsync();
//            _loggerManager.LogInfo("Exit JWTService => AddRefreshToken");
//            return dataresult.Entity;
//        }

//        public string GenerateRefreshToken()
//        {
//            _loggerManager.LogInfo("Entry JWTService => GenerateRefreshToken");
//            var randomBytes = new byte[64];
//            using (var rng = RandomNumberGenerator.Create())
//            {
//                rng.GetBytes(randomBytes);
//            }
//            var tokenString = Convert.ToBase64String(randomBytes);
//            _loggerManager.LogInfo("Exit JWTService => GenerateRefreshToken");
//            return tokenString;
//        }

//        public string GenerateToken(string type,int UserId,int TenantId,int ModuleId, string role, List<string> permissions)
//        {
//            _loggerManager.LogInfo("Entry JWTService => GenerateToken");
//            var claims = new List<Claim>
//            {
//                new Claim("Type",type),
//                new Claim("UserId", UserId.ToString()),
//                new Claim("TenantId", TenantId.ToString()),
//                new Claim("ModuleId", ModuleId.ToString()),
//                new Claim("Role", role)
//            };

//            foreach (var perm in permissions)
//            {
//                claims.Add(new Claim("Permission", perm));
//            }

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddMinutes(15),
//                signingCredentials: creds
//            );
//            _loggerManager.LogInfo("Exit JWTService => GenerateToken");
//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        public async Task<RefreshTokens> GetRefreshToken(int UserId, string Type, string deviceId)
//        {
//            _loggerManager.LogInfo("Entry JWTService => GetRefreshToken");
//            var dataresult = await _opsHubContext.RefreshTokens.FirstOrDefaultAsync(x=>x.UserId == UserId && x.UserType == Type && x.DeviceId == deviceId && x.IsUsed==false);
//            _loggerManager.LogInfo("Exit JWTService => GetRefreshToken");
//            return dataresult;
//        }

//        public async Task<RefreshTokens> GetRefreshTokenStatus(int UserId, string Type, string deviceId, string refreshToken)
//        {
//            _loggerManager.LogInfo("Entry JWTService => GetRefreshTokenStatus");
//            var dataresult = await _opsHubContext.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == UserId && x.UserType == Type && x.DeviceId == deviceId && x.Token == refreshToken 
//            && !(x.IsUsed??false) &&
//        !(x.IsRevoked??false) &&
//        x.ExpiresAt > DateTime.UtcNow);
//            _loggerManager.LogInfo("Exit JWTService => GetRefreshTokenStatus");
//            return dataresult;
//        }

//        public async Task<RefreshTokens> UpdateRefreshToken(RefreshTokens token)
//        {
//            _loggerManager.LogInfo("Entry JWTService => UpdateRefreshToken");
//            var dataresult = _opsHubContext.RefreshTokens.Update(token);
//            await _opsHubContext.SaveChangesAsync();
//            _loggerManager.LogInfo("Exit JWTService => UpdateRefreshToken");
//            return dataresult.Entity;
//        }
//    }
//}
