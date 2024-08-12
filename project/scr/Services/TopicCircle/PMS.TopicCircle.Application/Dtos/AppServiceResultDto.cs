using PMS.TopicCircle.Domain.Enum;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class AppServiceResultDto
    {
        public bool Status { get; set; }

        public CodeEnum StatuCode;

        public string Msg { get; set; }


        public static AppServiceResultDto Success(string msg = "")
        {
            return new AppServiceResultDto()
            {
                Status = true,
                Msg = msg
            };
        }

        public static AppServiceResultDto<T> Success<T>(T data, string msg = "") where T : class
        {
            return new AppServiceResultDto<T>()
            {
                Status = true,
                Msg = msg,
                Data = data
            };
        }


        public static AppServicePageResultDto<T> Success<T>(T data, long total, string msg = "") where T : class
        {
            return new AppServicePageResultDto<T>()
            {
                Status = true,
                Msg = msg,
                Data = data,
                Total = total
            };
        }



        public static AppServiceResultDto<T> Failure<T>(string msg = "") where T : class
        {
            return new AppServiceResultDto<T>()
            {
                Status = false,
                Msg = msg,
            };
        }

        public static AppServiceResultDto Failure(string msg, CodeEnum code = 0)
        {
            return new AppServiceResultDto()
            {
                Status = false,
                Msg = string.IsNullOrEmpty(msg) ? code.GetDescription() : msg,
                StatuCode = code
            };
        }


    }

    public class AppServiceResultDto<T> : AppServiceResultDto where T : class
    {
        public T Data { get; set; }

    }

    public class AppServicePageResultDto<T> : AppServiceResultDto where T : class
    {

        public long Total { get; set; }

        public T Data { get; set; }


    }
}
