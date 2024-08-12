using NPOIHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{

    /// <summary>
    /// 资料包数据总表
    /// 每天数据统计
    /// </summary>
    public class DataPacketSummary
    {
        /// <summary>
        /// 统计日期
        /// </summary>
        [ColumnType(Name = "统计日期", Type = ColumnType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// 扫码位置（页面链接地址）
        /// </summary>
        [ColumnType(Name = "扫码位置（页面链接地址）")]
        public string ScanPage { get; set; }

        /// <summary>
        /// 扫码页面浏览数PV/日
        /// </summary>
        [ColumnType(Name = "扫码页面浏览数PV/日", Type = ColumnType.Number)]
        public long ScanPagePV { get; set; }

        /// <summary>
        /// 扫码页面独立访客数UV/日
        /// </summary>
        [ColumnType(Name = "扫码页面独立访客数UV/日", Type = ColumnType.Number)]
        public long ScanPageUV { get; set; }

        /// <summary>
        /// 二维码扫码人数UV/日
        /// =
        /// 中间页独立访客数UV/日
        /// </summary>
        [ColumnType(Name = "二维码扫码人数UV/日", Type = ColumnType.Number)]
        public long MiddlePageUV { get; set; }

        /// <summary>
        /// 服务号新增关注数
        /// </summary>
        [ColumnType(Name = "服务号新增关注数", Type = ColumnType.Number)]
        public long SubscribeCount { get; set; }
    }

    /// <summary>
    /// 资料包用户行为表
    /// </summary>
    public class DataPacketUserSummary
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        [ColumnType(Name = "用户昵称")]
        public string NickName { get; set; }

        [ColumnType(Hide = true)]
        public Guid UserId { get; set; }

        [ColumnType(Hide = true)]
        public string OpenId { get; set; }

        /// <summary>
        /// 关注公众号时间
        /// </summary>
        [ColumnType(Name = "关注公众号时间", Type = ColumnType.DateTime)]
        public DateTime SubscribeTime { get; set; }

        /// <summary>
        /// 关注公众号后回复关键词
        /// </summary>
        [ColumnType(Name = "关注公众号后回复关键词")]
        public string ReplyKeyBySubscribe { get; set; }

        /// <summary>
        /// 欢迎语点击链接地址
        /// </summary>
        [ColumnType(Name = "欢迎语点击链接地址")]
        public string ViewPageUrlBySubscribe { get; set; }

        /// <summary>
        /// 24小时点击链接地址
        /// </summary>
        [ColumnType(Name = "24小时点击链接地址")]
        public string ViewPageUrlBy24h { get; set; }

        /// <summary>
        /// 36小时点击链接地址
        /// </summary>
        [ColumnType(Name = "36小时点击链接地址")]
        public string ViewPageUrlBy36h { get; set; }

        /// <summary>
        /// 48小时回复关键词
        /// </summary>
        [ColumnType(Name = "48小时回复关键词")]
        public string ViewPageUrlBy48h { get; set; }

        /// <summary>
        /// 取关公众号时间
        /// </summary>
        [ColumnType(Name = "取关公众号时间", Type = ColumnType.DateTime)]
        public DateTime? UnSubscribeTime { get; set; }

        /// <summary>
        /// 当前关注状态
        /// </summary>
        [ColumnType(Hide = true)]
        public bool IsSubcribe { get; set; }
    }
}
