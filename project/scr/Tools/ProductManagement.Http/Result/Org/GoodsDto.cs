﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.Org
{
    public class GoodsDto
    {
        /// <summary>
        /// 课程id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 课程短id
        /// </summary>
        public string Id_s { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 副标题
        /// </summary>
        public string SubTitle { get; set; }
        
        /// <summary>
        /// 课程banner图片地址
        /// </summary>
        public string Banner { get; set; }
        /// <summary>
        /// 现在价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 原始价格
        /// </summary>
        public decimal? OrigPrice { get; set; }
        /// <summary>
        /// 是否认证（true：认证；false：未认证）
        /// </summary>
        public bool Authentication { get; set; }
        /// <summary>
        /// 销售数量
        /// </summary>
        public int SellCount { get; set; }
        /// <summary>
        /// 课程标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 是否爆款
        /// </summary>
        public bool IsExplosion { get; set; }
        /// <summary>
        /// 是否下架
        /// </summary>
        public bool IsOnline { get; set; }
        /// <summary>
        /// Pc的url
        /// </summary>
        public string PcUrl { get; set; }
        /// <summary>
        /// m站的url
        /// </summary>
        public string MUrl { get; set; }
        /// <summary>
        /// 微信小程序预留的url接收字段
        /// </summary>
        public string H5ToMpUrl { get; set; }

    }
    public class GoodsObject
    {
        public IEnumerable<GoodsDto> Courses { get; set; }
    }
}

