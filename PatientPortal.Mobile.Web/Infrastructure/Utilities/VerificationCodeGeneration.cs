using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PatientPortal.Mobile.Web.Infrastructure.Utilities
{
    public class VerificationCodeGeneration : IVerificationCodeGeneration
    {
        const string ResetCode = "VerificationCode:CodeLength:ResetCode";
        const string VerificationCodeLength = "VerificationCode:CodeLength:Verification";
        const string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!$()*@[]^_`{|}~";
        const int LengthResetCode = 20;
        const int LengthofToken = 64;

        private IConfiguration Configuration { get; }

        public VerificationCodeGeneration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string GenerateResetCode()
        {
            if (!int.TryParse(Configuration[ResetCode], out var length))
            {
                length = LengthResetCode;
            }

            return GenerateStringUsingRNGCryptoService(length);
        }

        public string GenerateRandomString()
        {
            if (!int.TryParse(Configuration[VerificationCodeLength], out var length))
            {
                length = LengthofToken;
            }

            return GenerateStringUsingRNGCryptoService(length);
        }

        private string GenerateStringUsingRNGCryptoService(int length)
        {
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(Chars[(int)(num % (uint)Chars.Length)]);
                }
            }

            return res.ToString();
        }
    }
}
