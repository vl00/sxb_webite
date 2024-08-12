using System;
using System.Linq;
using System.Text;

namespace ProductManagement.Tool.HttpRequest.SecurityUtils
{
    public class Sha1Utils
    {
        public static string Encrypt(params string[] inputs)
        {
             var list = inputs.ToList();
             list.Sort(StringComparer.Ordinal);

            var input =  string.Join("", list);

            return Encrypt(input);
        }

        public static string Encrypt(string input)
        {
            var encryptByteArray = Encoding.UTF8.GetBytes(input);

            var sha1 = System.Security.Cryptography.SHA1.Create();

            var hash = sha1.ComputeHash(encryptByteArray);

            var enctryptString = BitConverter.ToString(hash).Replace("-", string.Empty);

            return enctryptString;
        }
    }
}
