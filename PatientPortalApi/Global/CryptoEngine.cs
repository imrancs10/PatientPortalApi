using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PatientPortalApi.Global
{
    public class CryptoEngine
    {
        public static string Encrypt(string input)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(input));
        }
        public static string Decrypt(string input)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(input));
        }
    }
}