using System;

namespace Sxb.Web.Common
{
    public static class ApiResult
    {
        /// <summary>
        /// 返回操作成功
        /// </summary>
        /// <returns></returns>
        public static ResultDto Success()
            => new ResultDto() { };

        /// <summary>
        /// 返回操作成功
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResultDto Success(string message="", Object data =null)
            => new ResultDto() { message = message , data = data };

        /// <summary>
        /// 返回操作失败
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResultDto Fail(string message, Object data =null)
            => new ResultDto() { message = message,code=400, data = data };
    }

    public class ResultDto {
        /// <summary>
        /// 返回的信息
        /// </summary>
        public string message { get; set; } = "操作成功";
        /// <summary>
        /// 返回的状态
        /// </summary>
        public int code { get; set; } = 200;
        /// <summary>
        /// 返回的值
        /// </summary>
        public Object data { get; set; }
    }
}
