using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProductManagement.Tool.HttpRequest.SecurityUtils
{
    public static class SecurityEx
    {
        public static string Md5(this string dataString)
        {
            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(dataString);

            var md5Bytes = md5.ComputeHash(bytes);

            return BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();
        }

        public static string Des(this string sourceString,string key = "demo1238",string iv = "demo1238")
        {

            byte[] btKey = Encoding.UTF8.GetBytes(key);

            byte[] btIV = Encoding.UTF8.GetBytes(iv);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] inData = Encoding.UTF8.GetBytes(sourceString);

                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(inData, 0, inData.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
