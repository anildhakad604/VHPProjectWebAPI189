using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectCommonUtility.Encryption
{
    public interface IEncryptionHelper
    {
        public string getIV();
        string doEncryption(string jsonstring, bool isOtp = false);
        string doDecryption(string encryptedString, bool isOtp = false);
    }
}
