using MongoDB.Driver.Core.WireProtocol.Messages.Encoders;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class AppServiceResultDto
    {
        public bool Status { get; set; }

        //public CodeEnum StatuCode;

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
                Data = data,
                Status = true,
                Msg = msg
            };
        }

        public static AppServiceResultDto Failure(string msg = "")
        {
            return new AppServiceResultDto()
            {
                Status = false,
                Msg = msg
            };
        }
    }

    public class AppServiceResultDto<T> : AppServiceResultDto where T : class
    {
        public T Data { get; set; }

        public static AppServiceResultDto<T> Success(T data, string msg = "") 
        {
            return new AppServiceResultDto<T>()
            {
                Status = true,
                Msg = msg,
                Data = data
            };
        }

        public static AppServiceResultDto<T> Failure(string msg = "") 
        {
            return new AppServiceResultDto<T>()
            {
                Status = false,
                Msg = msg,
            };
        }

        public static AppServiceResultDto<T> Failure(T data, string msg = "")
        {
            return new AppServiceResultDto<T>()
            {
                Data = data,
                Status = false,
                Msg = msg,
            };
        }
    }

    public class AppServicePageResultDto<T> : AppServiceResultDto where T : class
    {


        public static AppServicePageResultDto<T> Success(IEnumerable<T> data, int total, string msg = "") 
        {
            return new AppServicePageResultDto<T>()
            {
                Status = true,
                Msg = msg,
                Data = data,
                Total = total
            };
        }
        public  static AppServicePageResultDto<T> Failure(string msg = "") 
        {
            return new AppServicePageResultDto<T>()
            {
                Status = false,
                Msg = msg,
            };
        }


        public int Total { get; set; }

        public IEnumerable<T> Data { get; set; }


    }
}
