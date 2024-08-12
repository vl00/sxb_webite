using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Sxb.Web.Utils;
using ProductManagement.Framework.Foundation;
using PMS.TopicCircle.Application.Dtos;

namespace Sxb.Web.Response
{
    [Description("通用响应结果")]
    public class ResponseResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [Description("是否成功")]
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
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseResult<TData> TSuccess<TData>(TData data,string msg) where TData:class
        {
            return new ResponseResult<TData>()
            {
                Succeed = true,
                status = ResponseCode.Success,
                Msg = msg,
                Data = data
            };
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
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed( ResponseCode code,string msg, object data)
        {
            return new ResponseResult()
            {
                Succeed = false,
                status = code,
                Msg = msg,
                Data = data
            };
        }


        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(ResponseCode code, object data)
        {
            return new ResponseResult()
            {
                Succeed = false,
                status = code,
                Msg = code.Description(),
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

        public static ResponseResult Build(bool succeed)
        {
            return new ResponseResult()
            {
                Succeed = succeed,
                status = succeed ? ResponseCode.Success : ResponseCode.Failed,
            };
        }


        /// <summary>
        /// 将topic定义的业务code转为接口响应的code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static ResponseCode TopicCode2ResponseCode(CodeEnum  code,bool statu)
        {
            switch (code)
            {
                case CodeEnum.HasFollowCircle:
                    return ResponseCode.HasFollowCircle;
                default:
                    return statu ? ResponseCode.Success : ResponseCode.Failed;
            }
        }


        /// <summary>
        /// 对AppServiceResultDto对象隐式转换为ResponseResult
        /// </summary>
        /// <param name="apiResultDto"></param>
        public static implicit operator ResponseResult(AppServiceResultDto apiResultDto)
        {
            return new ResponseResult()
            {
                Succeed = apiResultDto.Status,
                status = TopicCode2ResponseCode(apiResultDto.StatuCode, apiResultDto.Status),
                Msg = apiResultDto.Msg,
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



    [Description("Data泛型Response")]
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
        public  static ResponseResult<T> Failed(ResponseCode code,string msg)
        {
            return new ResponseResult<T>()
            {
                Succeed = false,
                status = code,
                Msg = msg
            };
        }

        public static ResponseResult<T> Success(T data, string msg)
        {
            return new ResponseResult<T>()
            {
                Succeed = true,
                status = ResponseCode.Success,
                Msg = msg,
                Data = data
            };
        }


        public static implicit operator ResponseResult<T>(PMS.TopicCircle.Application.Dtos.AppServiceResultDto<T> appServiceResult)
        {
            return new ResponseResult<T>()
            {
                Succeed = appServiceResult.Status,
                status = appServiceResult.Status ? ResponseCode.Success : ResponseCode.Failed,
                Msg = appServiceResult.Msg,
                Data = appServiceResult.Data
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


    [Description("分页Response")]
    public class ResponsePageResult: ResponseResult
    {
        public long Total { get; set; }


        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponsePageResult Success(object data, long total, string message = null)
        {
            return new ResponsePageResult()
            {
                Succeed = true,
                Data = data,
                Msg = message,
                status = ResponseCode.Success,
                Total = total
            };
        }

        /// <summary>
        /// 返回一个失败的返回值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static ResponsePageResult Failed(string message = null)
        {
            return new ResponsePageResult()
            {
                Succeed = false,
                Msg = message,
                status = ResponseCode.Failed,
            };
        }


    }

    [Description("分页泛型Response")]
    public class ResponsePageResult<T> : ResponsePageResult where T : class
    {


        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponsePageResult<T> Success(T data,long total ,string message=null)
        {
            return new ResponsePageResult<T>()
            {
                Succeed = true,
                Data = data,
                Msg = message,
                status = ResponseCode.Success,
                Total = total
            };
        }

        /// <summary>
        /// 返回一个失败的返回值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public new static ResponsePageResult<T> Failed(string message = null)
        {
            return new ResponsePageResult<T>()
            {
                Succeed = false,
                Msg = message,
                status = ResponseCode.Failed,
            };
        }



        public static implicit operator ResponsePageResult<T>(AppServicePageResultDto<T> appServiceResult)
        {
            return new ResponsePageResult<T>()
            {
                Succeed = appServiceResult.Status,
                status = appServiceResult.Status ? ResponseCode.Success : ResponseCode.Failed,
                Msg = appServiceResult.Msg,
                Data = appServiceResult.Data,
                Total = appServiceResult.Total
            };
        }
    }


}