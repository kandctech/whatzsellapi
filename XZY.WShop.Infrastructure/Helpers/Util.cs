using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace XZY.WShop.Infrastructure.Helpers
{
    public class Util
    {
        public static string GenerateOtp(out string secret, out long counter)
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            secret = Base32.Encode(key);
            //
            Random rand = new Random();
            counter = rand.Next(100000000, 999999999);
            var bytes = Base32Encoding.ToBytes(secret);
            var hotp = new Hotp(bytes);
            var result = hotp.ComputeHOTP(counter);
            return result;
        }

        public static bool VerifyOtp(string secret, long counter, string otpCode)
        {
            var bytes = Base32Encoding.ToBytes(secret);
            var hotp = new Hotp(bytes);
            return hotp.VerifyHotp(otpCode, counter);
        }

        // Usage: MaskMobile("13456789889", 3, "****") => "134****9889"
        public static string MaskMobileNumber(string mobile, int startIndex, string mask)
        {
            if (string.IsNullOrEmpty(mobile))
                return string.Empty;
            string result = mobile;
            int starLengh = mask.Length;
            if (mobile.Length >= startIndex)
            {
                result = mobile.Insert(startIndex, mask);
                if (result.Length >= (startIndex + starLengh * 2))
                    result = result.Remove((startIndex + starLengh), starLengh);
                else
                    result = result.Remove((startIndex + starLengh), result.Length - (startIndex + starLengh));
            }
            return result;
        }

        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateHtmlStringFromTemplate(string templatePath, object data)
        {
            string body = null;
            if (data != null && !string.IsNullOrEmpty(templatePath))
            {
                string templateContent = System.IO.File.ReadAllText(templatePath);
                var dataDict = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(data, null));
                Regex re = new Regex(@"\{(\w+)\}", RegexOptions.Compiled);
                body = re.Replace(templateContent, match => dataDict[match.Groups[1]?.Value]?.ToString());
            }
            return body;
        }

        public static string RandomNumber(int startindex, int lenght)
        {
            try
            {
                var _unique = Guid.NewGuid().ToString().Replace("-", string.Empty);
                var _result = Regex.Replace(_unique, "[a-zA-Z]", string.Empty).Substring(startindex, lenght);

                return _result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
