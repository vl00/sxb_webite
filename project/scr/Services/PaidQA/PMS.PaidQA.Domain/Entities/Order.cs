using System;
using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("[Order]")]
    public partial class Order
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }

        /// <summary> 
        /// 订单编号 
        /// </summary> 
        public string NO { get; set; }

        /// <summary> 
        /// 订单状态：1待提问 2待回答 3进行中 4已结束 5已转单  6已超时 7已拒绝 
        /// </summary> 
        public OrderStatus Status { get; set; }

        /// <summary> 
        /// 提问剩余次数 
        /// </summary> 
        public int AskRemainCount { get; set; }

        /// <summary> 
        /// 订单价格 
        /// </summary> 
        public decimal Amount { get; set; }

        /// <summary> 
        /// 是否评价 
        /// </summary> 
        public bool IsEvaluate { get; set; }

        /// <summary> 
        /// 创建者ID 
        /// </summary> 
        public Guid CreatorID { get; set; }

        /// <summary> 
        /// 被提问者ID 
        /// </summary> 
        public Guid AnswerID { get; set; }

        /// <summary> 
        /// 原问题订单ID 
        /// </summary> 
        public Guid? OrginAskID { get; set; }

        /// <summary> 
        /// </summary> 
        public DateTime CreateTime { get; set; }

        /// <summary> 
        /// </summary> 
        public DateTime UpdateTime { get; set; }

        /// <summary> 
        /// 第一次回答的时间间隔，秒 
        /// </summary> 
        public int? FirstReplyTimespan { get; set; }

        /// <summary> 
        /// 系统屏蔽 
        /// </summary> 
        public bool? IsBlocked { get; set; }

        /// <summary> 
        /// 结束时间 
        /// </summary> 
        public DateTime? FinishTime { get; set; }

        /// <summary> 
        /// 是否匿名 
        /// </summary> 
        public bool? IsAnonymous { get; set; } = false;

        /// <summary> 
        /// 匿名名称 
        /// </summary> 
        public string AnonyName { get; set; }


        /// <summary>
        /// 是否已经有回复了
        /// </summary>
        public bool IsReply { get; set; }




        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsPublic { get; set; }



        public string Remark { get; set; }

        public decimal? PayAmount { get; set; }

        public string OriginType { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 是否开启转单
        /// </summary>
        public bool EnableTransiting { get; set; }

        /// <summary>
        /// 是否已拒绝转单
        /// </summary>
        [Obsolete]
        public bool IsRefusTransiting { get; set; }

        /// <summary>
        /// 是否已经发送了转单提醒
        /// </summary>
        [Obsolete]
        public bool IsSendTransitingMsg { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        [Obsolete]
        public Guid? QuestionID { get; set; }

    }
}