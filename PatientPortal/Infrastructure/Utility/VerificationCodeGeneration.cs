using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PatientPortal.Infrastructure.Utility
{
    public static class VerificationCodeGeneration
    {
        private const string Chars = "1234567890";
        private const int LengthDeviceVerification = 6;
        private const int LengthSerialNumber = 8;
        private const int LengthResetCode = 64;
        private const string LongChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!$()*@[]_{|}";
        private static readonly Random srRandom = new Random();
        public static string GenerateDeviceVerificationCode()
        {
            return new string(Enumerable.Repeat(Chars, LengthDeviceVerification)
              .Select(s => s[srRandom.Next(s.Length)]).ToArray());
        }
        public static string GetSerialNumber()
        {
            string code = new string(Enumerable.Repeat(Chars, LengthSerialNumber)
              .Select(s => s[srRandom.Next(s.Length)]).ToArray());
            return code + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
        }

        public static string GetGeneratedResetCode()
        {
            return GenerateStringUsingRNGCryptoService(LengthResetCode);
        }

        private static string GenerateStringUsingRNGCryptoService(int length)
        {
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(LongChars[(int)(num % (uint)LongChars.Length)]);
                }
            }

            return res.ToString();
        }
    }
}