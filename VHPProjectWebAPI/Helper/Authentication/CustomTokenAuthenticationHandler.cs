using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using VHPProjectCommonUtility.Logger;
using VHPProjectDAL.Authentication;

namespace VHPProjectWebAPI.Helper.Authentication
{
    public class CustomTokenAuthenticationHandler : AuthenticationHandler<CustomTokenAuthenticationOptions>
    {
        private readonly ILoggerManager _loggerManager;
        private readonly IConfiguration _configuration;
        public CustomTokenAuthenticationHandler(
            IOptionsMonitor<CustomTokenAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration, ILoggerManager loggerManager)
            : base(options, logger, encoder, clock)
        {
            _loggerManager = loggerManager;
            _configuration = configuration;
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _loggerManager.LogInfo("Entry CustomTokenAuthenticationHandler => HandleAuthenticateAsync");
            var authHeader = Request.Headers["CustomAuthorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            var token = authHeader.Trim();
            var expectedToken = _configuration["CustomAuth:StaticToken"];
            if (token.Equals(expectedToken, System.StringComparison.OrdinalIgnoreCase))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "CustomAuthorizedUser") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            _loggerManager.LogInfo("Exit CustomTokenAuthenticationHandler => HandleAuthenticateAsync");
            return Task.FromResult(AuthenticateResult.Fail("Invalid Token"));
        }
    }
}
