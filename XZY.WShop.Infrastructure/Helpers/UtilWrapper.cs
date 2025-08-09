using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Helpers;

namespace XZY.WShop.Infrastructure.Helpers
{
    public class UtilWrapper : IUtilWrapper
    {
        public string GenerateOtp(out string secret, out long counter)
        {
            return Util.GenerateOtp(out secret, out counter);
        }

        public bool VerifyOtp(string secret, long counter, string otpCode)
        {
            return Util.VerifyOtp(secret, counter, otpCode);
        }

        public string MaskMobileNumber(string mobile, int startIndex, string mask)
        {
            return Util.MaskMobileNumber(mobile, startIndex, mask);
        }

        public string GetDaySuffix(int day)
        {
            return Util.GetDaySuffix(day);
        }

        public string ComputeSha256Hash(string rawData)
        {
            return Util.ComputeSha256Hash(rawData);
        }

        public string GenerateHtmlStringFromTemplate(string templatePath, object data)
        {
            return Util.GenerateHtmlStringFromTemplate(templatePath, data);
        }

        public string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null) return null;
            if (phoneNumber.StartsWith("+234"))
            {
                return phoneNumber.Replace("+234", "0");
            }
            return phoneNumber;
        }
    }
}
