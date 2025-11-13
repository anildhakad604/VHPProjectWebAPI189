using System;
using System.Collections.Generic;
using System.Text;

namespace VHPProjectCommonUtility.Configuration
{
    public class AppsettingsConfig
    {
        public MasterProjData MasterProjData { get; set; } = new MasterProjData();
        public EmailSetting EmailSetting { get; set; } = new EmailSetting();
        public bool RequestResponseLoggingEnabled { get; set; }
    }
}