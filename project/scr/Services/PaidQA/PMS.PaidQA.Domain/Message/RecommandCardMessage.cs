using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public enum RecommandCardMessageStatu { 

        /// <summary>
        /// 默认
        /// </summary>
        Default= 0,

        /// <summary>
        /// 拒绝转单
        /// </summary>
        Refus = 1,
        
        /// <summary>
        /// 已转单
        /// </summary>
        HasTransity =2,
    }

    public class RecommandCardMessage:PaidQAMessage
    {
        public string Content { get; set; }

        public Guid TalentUserID { get; set; }

        public RecommandCardMessageStatu Status { get; set; }

        /// <summary>
        /// 转单后ID
        /// </summary>
        public Guid? TrantiyOrderID { get; set; }

        /// <summary>
        /// 差价
        /// </summary>
        public string PriceSpread { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.RecommandCard;


    }
}
