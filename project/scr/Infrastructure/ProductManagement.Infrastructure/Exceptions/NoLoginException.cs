using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProductManagement.Infrastructure.Exceptions
{
    /// <summary>
    /// 没登录异常
    /// </summary>
    [Serializable]
    public class NoLoginException : Exception
    {

        public NoLoginException() : base() { }

        public NoLoginException(string message) : base(message) { }

        public NoLoginException(string message, Exception innerException) : base(message, innerException) { }

        protected NoLoginException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
