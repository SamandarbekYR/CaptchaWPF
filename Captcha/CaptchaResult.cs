using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captcha
{
    public class CaptchaResult
    {
        public string CaptchaCode { get; set; } = string.Empty;
        public byte[] CaptchaByteData { get; set; } = new byte[0];
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }
    }
}
