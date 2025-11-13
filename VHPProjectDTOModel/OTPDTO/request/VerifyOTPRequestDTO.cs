using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.OTPDTO.request
{
    public class VerifyOTPRequestDTO
    {
        public string MobileNumber { get; set; }
        public string OTP { get; set; }
    }
}
