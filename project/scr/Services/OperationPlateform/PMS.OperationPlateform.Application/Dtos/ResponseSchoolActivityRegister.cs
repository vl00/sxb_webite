using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    /// <summary>
    /// 报名结果
    /// </summary>
    public class ResponseSchoolActivityRegister
    {
        public ResponseSchoolActivityRegister()
        {
        }

        public ResponseSchoolActivityRegister(Code errorCode, string errorMsg)
        {
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public static AppServiceResultDto<ResponseSchoolActivityRegister> ToFailure(Code errorCode, string errorMsg)
        {
            var result = new ResponseSchoolActivityRegister(errorCode, errorMsg);
            return AppServiceResultDto<ResponseSchoolActivityRegister>.Failure(result, result.ErrorMsg);
        }

        public enum Code
        {
            OK = 200,
            /// <summary>
            /// 活动未开始
            /// </summary>
            NoTime = 1001,
            /// <summary>
            /// 活动已过期
            /// </summary>
            Expired = 1002,
            /// <summary>
            /// 字段填写不全
            /// </summary>
            NoField = 1003,
            /// <summary>
            /// 短信验证码错误
            /// </summary>
            SmsCodeError = 1004,
            /// <summary>
            /// 渠道x电话重复
            /// </summary>
            ExistChannelXPhone = 1005
        }


        public Code ErrorCode { get; set; } = Code.OK;
        public string ErrorMsg { get; set; }

    }
}
