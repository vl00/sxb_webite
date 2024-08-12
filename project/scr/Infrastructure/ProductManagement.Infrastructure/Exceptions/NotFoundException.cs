using System;
using System.Runtime.Serialization;

namespace ProductManagement.Infrastructure.Exceptions
{
    /// <summary>
    /// 没找到的异常
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    }
}
