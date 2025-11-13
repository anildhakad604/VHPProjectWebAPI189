using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectDAL.EmailRepo
{
    public interface IEmailRepository
    {
        void SendEmail(string emailbody, string Subject);

    }
}
