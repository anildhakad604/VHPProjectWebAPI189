using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectCommonUtility.Extensions
{
    public static class HttpClientExtensions
    {
        public static string ToCurl(this HttpRequestMessage request)
        {
            var method = request.Method.Method;
            var uri = request.RequestUri.ToString();
            var headers = request.Headers;
            var content = request.Content;

            var curl = new StringBuilder();

            curl.Append($"curl -X {method} \"{uri}\"");

            foreach (var header in headers)
            {
                foreach (var value in header.Value)
                {
                    curl.Append($" -H \"{header.Key}: {value}\"");
                }
            }

            if (content != null)
            {
                var contentHeaders = content.Headers;

                foreach (var header in contentHeaders)
                {
                    foreach (var value in header.Value)
                    {
                        curl.Append($" -H \"{header.Key}: {value}\"");
                    }
                }

                var contentString = content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(contentString))
                {
                    curl.Append($" -d \"{contentString}\"");
                }
            }

            return curl.ToString();
        }
    }
}
