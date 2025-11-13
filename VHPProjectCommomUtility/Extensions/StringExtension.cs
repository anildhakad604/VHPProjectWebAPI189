using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VHPProjectCommonUtility.Extensions
{
    public static class StringExtension
    {
        public static string ConvertObjectToBase64<T>(this T obj)
        {
            // Step 1: Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(obj);

            // Step 2: Convert the JSON string to a byte array
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);

            // Step 3: Convert the byte array to a Base64 encoded string
            string base64String = Convert.ToBase64String(byteArray);

            return base64String;
        }

        // Extension method to convert Base64 string back to object
        public static string ConvertBase64ToString(this string base64String)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64String);

            // Convert byte array to string
            string decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

            return decodedString;
        }
    }
}
