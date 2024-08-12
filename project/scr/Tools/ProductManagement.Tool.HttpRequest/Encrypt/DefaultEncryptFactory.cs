using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.HttpRequest.Encrypt
{
    public class DefaultEncryptFactory : IEncryptFactory
    {
        public IEncrypt Instantiation(string key)
        {
            return new DefaultEncrypt();
        }
    }
}
