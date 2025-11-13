using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VHPProjectCommonUtility.Encryption
{
    public class EncryptionHelper : IEncryptionHelper
    {
         public string getIV()
         {
            string ivCHARS = "1234567890!@#$%^"; //Do not change this

            StringBuilder iv = new StringBuilder();

            Random rnd = new Random();

            while (iv.Length < 16)

            { // length of the random string.


                int index = (int)(rnd.NextDouble() * ivCHARS.Length);


                iv.Append(ivCHARS[index]);

            }

            Debug.WriteLine("MyIV", iv.ToString());


            return iv.ToString();

        }


        /**

        * Encryption Method: doEncryption

        * Params: jsonstring(string to be encrypted), token (shared by AuthBridge)

        * Return: Encrypted String

        * **/


        public string doEncryption(string jsonstring, bool isOtp = false)

        {
            if (string.IsNullOrEmpty(jsonstring))
            {
                return "";
            }


            string iv = getIV();
            string token = isOtp ? "India@2608" : "1219_VIBE_0975";


            using (Aes aes = Aes.Create())
            {
                SHA512 sha512 = SHA512.Create();

                byte[] bytes = Encoding.UTF8.GetBytes(token);


                byte[] hash = sha512.ComputeHash(bytes);

                string hex = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

                string shaKey = hex.Substring(0, 16);


                byte[] shaKeyby = Encoding.UTF8.GetBytes(shaKey);

                aes.Key = shaKeyby;

                byte[] initVectorBytes = Encoding.UTF8.GetBytes(iv);

                aes.IV = initVectorBytes;

                aes.Mode = CipherMode.CBC;


                ICryptoTransform encrypto = aes.CreateEncryptor();


                byte[] plainTextByte = ASCIIEncoding.UTF8.GetBytes(jsonstring);

                byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);


                string fiv = Convert.ToBase64String(Encoding.UTF8.GetBytes(iv));


                return Convert.ToBase64String(CipherText) + ":" + fiv;


            }

        }



        /**

        * Encryption Method: doDecryption

        * Params: encryptedString (string to be decrypted), token (shared by AuthBridge)

        * Return: Decrypted String

        * **/


        public string doDecryption(string encryptedString, bool isOtp = false)

        {
            if (string.IsNullOrEmpty(encryptedString))
            {
                return "";
            }

            string iv = getIV();
            string token = isOtp ? "India@2608" : "1219_VIBE_0975";
            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                {
                    SHA512 sha512 = SHA512.Create();
                    byte[] bytes = Encoding.UTF8.GetBytes(token);

                    byte[] hash = sha512.ComputeHash(bytes);

                    string hex = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

                    string shaKey = hex.Substring(0, 16);


                    /*** Initialization vector (or IV)****/


                    string[] enstring = encryptedString.Split(':');

                    byte[] IV = Convert.FromBase64String(enstring[1]);

                    /*** encrypted data ****/

                    string encrstring = enstring[0];


                    byte[] shaKeybyte = Encoding.UTF8.GetBytes(shaKey);


                   // RijndaelManaged aes = new RijndaelManaged();

                    aes.Key = shaKeybyte;

                    aes.IV = IV;

                    aes.Mode = CipherMode.CBC;

                    ICryptoTransform decrypto = aes.CreateDecryptor();

                    byte[] data = Convert.FromBase64String(encrstring);


                    byte[] CipherText = decrypto.TransformFinalBlock(data, 0, data.Length);


                    return System.Text.Encoding.UTF8.GetString(CipherText);


                }

            }


        }
    }
}
