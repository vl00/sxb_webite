using Sxb.Api.RequestModel.RequestEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.RequestModel.RequestOption
{
    /// <summary>
    /// 请求结构体
    /// </summary>
    public class RequestOption
    {
        /// <summary>
        /// 数据id
        /// </summary>
        public Guid dataId { get; set; }
        /// <summary>
        /// 数据id类型
        /// </summary>
        public dataIdType dataIdType { get; set; }
    }
}
