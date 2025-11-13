using System;
using System.Collections.Generic;
using System.Text;

namespace VHPProjectCommonUtility.Configuration
{

    public class BulkSms
    {
        public string flow_id { get; set; }
        public string sender { get; set; }
        public IList<Recipients> recipients { get; set; }
        public string route { get; set; }

    }
    public class Recipients
    {
        public string mobiles { get; set; }
        public string var1 { get; set; }
        public string var2 { get; set; }

    }
    
}
