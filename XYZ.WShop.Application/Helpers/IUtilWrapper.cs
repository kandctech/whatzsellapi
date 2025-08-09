using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Helpers
{
    public interface IUtilWrapper
    {
        string GenerateOtp(out string secret, out long counter);
        bool VerifyOtp(string secret, long counter, string otpCode);
        string MaskMobileNumber(string mobile, int startIndex, string mask);
        string GetDaySuffix(int day);
        string ComputeSha256Hash(string rawData);
        string GenerateHtmlStringFromTemplate(string templatePath, object data);
        string FormatPhoneNumber(string phoneNumber);
    }
}
