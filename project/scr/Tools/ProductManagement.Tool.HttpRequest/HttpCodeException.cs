using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.HttpRequest
{
    public class HttpCodeException : Exception
    {
        public HttpCodeException(string message) : base(message)
        {
            
        }
    }
}
