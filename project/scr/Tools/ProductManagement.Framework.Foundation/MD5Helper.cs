using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public class MD5Helper
    {
        private static string MD5Tail;
        public MD5Helper(string _MD5Tail)
        {
            MD5Tail = _MD5Tail;
        }
        public static string GetMD5(string str)
        {
            return GetSimpleMD5(str + MD5Tail);
        }
        public static string GetSimpleMD5(string str)
        {
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(str);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToUpper();
        }
        public static string GetSimpleMD5(byte[] data)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToUpper();
        }
        public static string GetSimpleMD5(Stream stream)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] OutBytes = md5.ComputeHash(stream);
            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToUpper();
        }



        public static string GetSHA256(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] hash = SHA256.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }
    }
}
