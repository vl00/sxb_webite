using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProductManagement.Tool.HttpRequest.SecurityUtils
{
    public class AesUtils
    {
        public static string Encrypt(string text, string aesKey)
        {      
            byte[] aesKeyBytes = Encoding.UTF8.GetBytes(aesKey);

            byte[] aesKeySha256 = SHA256.Create().ComputeHash(aesKeyBytes);

            byte[] ivMd5 = MD5.Create().ComputeHash(aesKeyBytes);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(aesKeySha256, ivMd5))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(text);
                            csEncrypt.Write(bytesToBeEncrypted ,0, bytesToBeEncrypted.Length);
                        }

                        var decryptedContent = msEncrypt.ToArray();


                        return Convert.ToBase64String(decryptedContent);
                    }
                }
            }
        }
    }
}
