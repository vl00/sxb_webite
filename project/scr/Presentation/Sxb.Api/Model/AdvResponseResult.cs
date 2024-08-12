using PMS.OperationPlateform.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.Model
{
    /// <summary>
    /// 广告模块接口响应结果
    /// </summary>
    public class AdvResponseResult : Response.AppResponseResult
    {
        /// <summary>
        /// 广告列表
        /// </summary>
        public List<AdvOption> Advs { get; set; }

        /// <summary>
        /// 广告的参数
        /// </summary>
        public class AdvOption
        {
            /// <summary>
            /// 广告位置
            /// </summary>
            public int Place { get; set; }

            /// <summary>
            /// 广告位的广告项
            /// </summary>
            public List<AdvertisingBaseGetAdvertisingResultDto> Items { get; set; }
        }

        ///// <summary>
        ///// 广告的基本信息
        ///// </summary>
        //public class AdvBase
        //{
        //    /// <summary>
        //    /// 排序位置
        //    /// </summary>
        //    public int Sort { get; set; }

        //    /// <summary>
        //    /// 图片地址
        //    /// </summary>
        //    public string PicUrl { get; set; }

        //    /// <summary>
        //    /// 标题
        //    /// </summary>
        //    public string SloGan { get; set; }

        //    /// <summary>
        //    /// 跳转链接
        //    /// </summary>
        //    public string Url { get; set; }

        //    /// <summary>
        //    /// Rate参数
        //    /// </summary>
        //    public double Rate { get; set; }
        //}
    }
}