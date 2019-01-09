using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace PatientPortalApi.Global
{
    public static class Utility
    {
        private static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        public static string GetAppSettingKey(string key)
        {
            try
            {
                string result = ConfigurationManager.AppSettings.Get(key);
                return string.IsNullOrEmpty(result) ? string.Empty : result;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
    }

}