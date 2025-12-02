using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDTOModel.LoginDTO.request
{
    public class RegisterRequest
    {
        public string UserName {  get; set; }

        public string Password { get; set; }

        public string MobileNumber {  get; set; }
    }
}
