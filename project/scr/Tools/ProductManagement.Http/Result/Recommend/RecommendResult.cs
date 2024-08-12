using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProductManagement.API.Http.Result.Org
{
    public class RecommendResult<TData> where TData:class
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [Description("是否成功")]
        public bool Succeed { get; set; }

        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime MsgTime { get; set; }

        /// <summary>
        /// 返回错误码
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回Model
        /// </summary>
        public TData Data { get; set; }

    }
}
