using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Settlement
{
    public class PartTimeJobDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 微信用户身份唯一标识
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public Guid JobId { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public int SettlementAmount { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 祝福语
        /// </summary>
        public string Blessings { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 操作者信息
        /// </summary>
        public string Sender { get; set; }
    }
}
