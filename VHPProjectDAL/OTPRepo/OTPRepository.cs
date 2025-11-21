using Microsoft.Extensions.Configuration;

namespace VHPProjectDAL.OTPRepo
{
    public class OTPRepository : IOTPRepository
    {

        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        private readonly string _authKey;
        private readonly string _templateId;

        public OTPRepository(IConfiguration config)
        {
            _config = config;
            _baseUrl = _config["MSG91:BaseUrl"];
            _authKey = _config["MSG91:AuthKey"];
            _templateId = _config["MSG91:TemplateId"];
        }

        public async Task<HttpResponseMessage> SendOTPAsync(string mobileNumber)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/?template_id={_templateId}&mobile=91{mobileNumber}&authkey={_authKey}");
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> VerifyOTPAsync(string mobileNumber, string otp)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/verify?mobile=91{mobileNumber}&otp={otp}&authkey={_authKey}");
            return await client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> ResendOTPAsync(string mobileNumber)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/retry?mobile=91{mobileNumber}&authkey={_authKey}&retrytype=text");
            return await client.SendAsync(request);
        }
    }
}
