using System;
using System.Collections.Generic;
using System.Text;

namespace VHPProjectCommonUtility.Configuration
{
    public class EmailSetting
    {
        public string Secret { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string Sender { get; set; }

    }

    public class RequestObject
    {
        public List<To> to { get; set; }
        public From from { get; set; }
        public List<Attachments> attachments { get; set; }
        public List<Bcc> bcc { get; set; }
        public Variables variables { get; set; }
        public string domain { get; set; }
        public string mail_type_id { get; set; }
        public string in_reply_to { get; set; }
        public string template_id { get; set; }
    }
    public class To
    {
        public string name { get; set; }
        public string email { get; set; }
    }

    public class From
    {
        public string name { get; set; }
        public string email { get; set; }
    }
    public class Bcc
    {
        public string email { get; set; }
    }
    public class Attachments
    {
        public string file { get; set; }
        public string fileName { get; set; }
    }

    public class Variables
    {
        public string Aggregator_Name { get; set; }
        public string Aggregator_Code { get; set; }
        public string user_name { get; set; }
        public string Contact_Person { get; set; }
    }
}
