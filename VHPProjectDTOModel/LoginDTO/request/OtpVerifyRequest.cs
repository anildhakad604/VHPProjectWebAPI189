using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.LoginDTO.request
{
    public class OtpVerifyRequest
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }

}
