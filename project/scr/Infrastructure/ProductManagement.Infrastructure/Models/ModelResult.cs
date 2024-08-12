using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.Toolibrary
{
    /// <summary>
    /// 单实体数据放回接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelResult<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
