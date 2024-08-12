using PMS.PaidQA.Domain.Entities;
using System;

namespace PMS.PaidQA.Domain.EntityExtend
{
    public class OrderPageExtend : Order
    {
        public string QuestionContent { get; set; } = string.Empty;
        public string ReplyContent { get; set; } = string.Empty;
        public bool HasNew { get; set; }
        public string HeadImgUrl { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string AuthName { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public Guid TalentUserID { get; set; }
        public int TalentRole { get; set; }
        public Evaluate Comment { get; set; }
        public int ViewCount { get; set; }
        public string TextContent
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrWhiteSpace(QuestionContent))
                {
                    try
                    {
                        var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<Message.TXTMessage>(QuestionContent);
                        if (messageContent != null) return messageContent.Content;
                    }
                    catch
                    {
                        return QuestionContent;
                    }
                }
                return result;
            }
        }

        public string TextContentReply
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrWhiteSpace(ReplyContent))
                {
                    try
                    {
                        var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<Message.TXTMessage>(ReplyContent);
                        if (messageContent != null) return messageContent.Content;
                    }
                    catch
                    {
                        return ReplyContent;
                    }
                }
                return result;
            }
        }
        /// <summary>
        /// 已转单的新订单ID
        /// </summary>
        public Guid? NewOrderID { get; set; }
        /// <summary>
        /// 是否从待提问超时
        /// </summary>
        public bool IsTimeOutFromWaitAsk { get; set; }
    }
}
