using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Utils
{
    public class EncryptHelper
    {

        public static string MD5Encrypt(string str)
        {
            using var md5 = MD5.Create();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string encrypt = BitConverter.ToString(result);
            return encrypt.Replace("-", "");
        }
    }
}
