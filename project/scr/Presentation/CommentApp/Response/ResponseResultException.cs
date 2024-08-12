using System;
namespace Sxb.Web.Response
{
    public class ResponseResultException : Exception
    {
        public int Code { get; }

        public ResponseResultException(string msg, int errorCode) : this(errorCode, msg) { }
        public ResponseResultException(Exception ex, int errorCode) : this(errorCode, ex) { }

        public ResponseResultException(int errorCode, string msg) : base(msg)
        {
            if (errorCode == (int)ResponseCode.Success) throw new ArgumentException(nameof(errorCode));
            this.Code = errorCode;
        }

        public ResponseResultException(int errorCode, Exception ex) : base(ex.Message, ex)
        {
            if (errorCode == 0) throw new ArgumentException(nameof(errorCode));
            this.Code = errorCode;
        }

        public ResponseResult ToResult()
        {
            var r = ResponseResult.Failed(this.Message);
            r.status = (ResponseCode)this.Code;
            return r;
        }
    }
}
