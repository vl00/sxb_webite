using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    /// <summary>
	/// DES加密/解密类。
    /// Copyright (C) Maticsoft
	/// </summary>
	public class DesTool
    {


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static string Md5(string dataString)
        {
            if (string.IsNullOrEmpty(dataString))
            {
                return dataString;
            }

            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(dataString);

            var md5Bytes = md5.ComputeHash(bytes);

            return BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();
        }

        public static string LieJiaMd5(string dataString)
        {
    
            if (string.IsNullOrEmpty(dataString))
            {
                return dataString;
            }
            string liejiaSrecret = ",;]Sd&@Hhib!$f#^vdv^82%%7(Q&*)#E";
            dataString = dataString + liejiaSrecret;
            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(dataString);

            var md5Bytes = md5.ComputeHash(bytes);

            return BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();


        }

        public static string DesDecryptPassword(string pToDecrypt)
        {
            if (string.IsNullOrEmpty(pToDecrypt))
            {
                return pToDecrypt;
            }
           
            var str = pToDecrypt.Replace("-", "+").Replace("_", "/").Replace("~", "=");

            var key = Encoding.ASCII.GetBytes("demo1238");

            var iv = Encoding.ASCII.GetBytes("demo1238");

            return DesDecrypt(str, key, iv);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pToDecrypt">要解密的以Base64</param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns>已解密的字符串</returns>
        public static string DesDecrypt(string pToDecrypt, byte[] key, byte[] iv)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

            using (DES des = DES.Create())
            {
                var crypto = des.CreateDecryptor(key, iv);

                var block = crypto.TransformFinalBlock(inputByteArray, 0, inputByteArray.Length);

                string str = Encoding.UTF8.GetString(block);

                return str;
            }
        }

    }
}
