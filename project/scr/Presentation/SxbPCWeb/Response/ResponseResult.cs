using System;
using Newtonsoft.Json;
using Sxb.PCWeb.Utils;

namespace Sxb.PCWeb.Response
{
    public class ResponseResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeed { get; set; }

        /// <summary>
        /// 返回时间
        /// </summary>
        [JsonConverter(typeof(HmDateTimeConverter))]
        public DateTime MsgTime => DateTime.Now;

        /// <summary>
        /// 返回错误码
        /// </summary>
        public ResponseCode status { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        /// 返回Model
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Success()
        {
            return Success("操作成功");
        }

        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseResult Success(string message)
        {
            return Success(null, message);
        }

        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseResult Success<TData>(TData data)
        {
            return Success(data, "查询成功");
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed()
        {
            return Failed(null);
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(string msg)
        {
            return Failed(msg, null);
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseResult Failed<TData>(TData data)
        {
            return Failed("操作失败", data);
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(string msg, object data)
        {
            return new ResponseResult()
            {
                Succeed = false,
                status = ResponseCode.Failed,
                Msg = msg,
                Data = data
            };
        }

       
        /// <summary>
        /// 返回成功的返回值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ResponseResult Success(object data, string msg)
        {
            return new ResponseResult()
            {
                Succeed = true,
                status = ResponseCode.Success,
                Msg = msg,
                Data = data
            };
        }

        public static implicit operator ResponseResult(PMS.OperationPlateform.Application.Dtos.AppServiceResultDto appServiceResult)
        {
            return new ResponseResult()
            {
                Succeed = appServiceResult.Status,
                status = appServiceResult.Status ? ResponseCode.Success : ResponseCode.Failed,
                Msg = appServiceResult.Msg,
            };
        }
    }


    public class ResponseResult<T> : ResponseResult where T : class
    {
        public new static ResponseResult<T> Failed(string msg)
        {
            return new ResponseResult<T>()
            {
                Succeed = false,
                status = ResponseCode.Failed,
                Msg = msg
            };
        }


        public static implicit operator ResponseResult<T>(PMS.OperationPlateform.Application.Dtos.AppServiceResultDto<T> appServiceResult)
        {
            return new ResponseResult<T>()
            {
                Succeed = appServiceResult.Status,
                status = appServiceResult.Status ? ResponseCode.Success : ResponseCode.Failed,
                Msg = appServiceResult.Msg,
                Data = appServiceResult.Data
            };
        }

    }
}