using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using ProductManagement.Framework.Foundation;
using System.Linq;
using Newtonsoft.Json;
using Sxb.Web.Utils;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    /// <summary>
    /// 问答分页请求参数
    /// </summary>
    /// <modify author="qzy" time="2021-01-11 17:13:09"></modify>
    public class PageRequest
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 问答状态
        /// </summary>
        public IEnumerable<int> _Status
        {
            get
            {
                var result = new List<int>();
                if (!string.IsNullOrWhiteSpace(Status))
                {
                    var chars = Status.ToCharArray();
                    foreach (var item in chars)
                    {
                        if (int.TryParse(item.ToString(), out int convertResult)) result.Add(convertResult);
                    }
                }
                return result;
            }
        }

        public string Status { get; set; }
        /// <summary>
        /// false:收到的提问 | true:发起的提问
        /// </summary>
        public bool IsAsk { get; set; } = true;
    }
    public class PageResult
    {
        public PageResult()
        {
            Items = new List<ResultItem>();
        }
        public IEnumerable<ResultItem> Items { get; set; }
    }
    public class ResultItem
    {
        /// <summary>
        /// 问答状态
        /// </summary>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// 头像URL
        /// </summary>
        public string HeadImageUrl { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonConverter(typeof(HmDateTimeConverter))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonConverter(typeof(HmDateTimeConverter))]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 中文时间
        /// </summary>
        public string FormatCreateTime
        {
            get
            {
                return CreateTime.ConciseTime("yyyy年MM月dd日");
            }
        }
        /// <summary>
        /// 问答编号
        /// </summary>
        public string NO { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否有新消息
        /// </summary>
        public bool HasNew { get; set; }
        /// <summary>
        /// 是否有评价
        /// </summary>
        public bool HasEvaluate { get; set; }
        /// <summary>
        /// 评价分数
        /// </summary>
        public int EvaluateScore { get; set; }
        /// <summary>
        /// 首次回复时间间隔
        /// <para>单位 : 秒</para>
        /// </summary>
        public int FirstReplyTimespan { get; set; }
        /// <summary>
        /// 评价ID
        /// </summary>
        public Guid EvaluateID { get; set; }
        /// <summary>
        /// 评价是否为自动评价
        /// </summary>
        public bool IsAutoEvaluate { get; set; }
        /// <summary>
        /// 提问内容
        /// </summary>
        public string QuestionContent { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderID { get; set; }
        /// <summary>
        /// 回答者UserID
        /// </summary>
        public Guid AnwserUserID { get; set; }
        /// <summary>
        /// 转单后的新订单ID
        /// </summary>
        public Guid NewOrderID { get; set; }
        /// <summary>
        /// 从待提问超时
        /// </summary>
        public bool IsTimeOutFromWaitAsk { get; set; }
    }
}
