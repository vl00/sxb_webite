using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
namespace PMS.PaidQA.Domain.Message
{
    public abstract class PaidQAMessage
    {

        protected abstract MsgMediaType MediaType { get; }

        protected readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


        public virtual Entities.Message CreateMessage(Guid senderId, Guid reciverId, Guid orderId, Enums.MsgType msgType)
        {
            return new Entities.Message()
            {
                ID = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                SenderID = senderId,
                ReceiveID = reciverId,
                MsgType = msgType,
                OrderID = orderId,
                IsValid = true,
                MediaType = MediaType,
                Content = JsonConvert.SerializeObject(this, jsonSerializerSettings)

            };
        }

        public static TMessage Create<TMessage>(Entities.Message message) where TMessage : PaidQAMessage
        {
            return JsonConvert.DeserializeObject<TMessage>(message.Content);
        }


        public static string GetContent(Entities.Message message)
        {
            switch (message.MediaType)
            {
                case MsgMediaType.Text:
                    TextMessage textMessage = PaidQAMessage.Create<TextMessage>(message);
                    return textMessage.Content;

                case MsgMediaType.TXT:
                    TXTMessage txtMessage = PaidQAMessage.Create<TXTMessage>(message);
                    return txtMessage.Content;
                case MsgMediaType.Image:
                    ImageMessage imageMessage = PaidQAMessage.Create<ImageMessage>(message);
                    return "[图片]";
                case MsgMediaType.Attachment:
                    AttachmentMessage attachmentMessage = PaidQAMessage.Create<AttachmentMessage>(message);
                    return attachmentMessage.Title;
                case MsgMediaType.SchoolCard:
                    SchoolCardMessage schoolCardMessage = PaidQAMessage.Create<SchoolCardMessage>(message);
                    return "[学校卡片]";
                case MsgMediaType.ArticleCard:
                    ArticleCardMessage articleCardMessage = PaidQAMessage.Create<ArticleCardMessage>(message);
                    return "[文章卡片]";
                case MsgMediaType.OrgEvaluationCard:
                    OrgEvaluationCardMessage orgEvaluationCardMessage = PaidQAMessage.Create<OrgEvaluationCardMessage>(message);
                    return orgEvaluationCardMessage.Title;
                case MsgMediaType.OrgCard:
                    OrgCardMessage orgCardMessage = PaidQAMessage.Create<OrgCardMessage>(message);
                    return orgCardMessage.Title;
                case MsgMediaType.CourseCard:
                    CourseCardMessage courseCardMessage = PaidQAMessage.Create<CourseCardMessage>(message);
                    return courseCardMessage.Title;
                case MsgMediaType.LiveCard:
                    LiveCardMessage liveCardMessage = PaidQAMessage.Create<LiveCardMessage>(message);
                    return "[直播卡片]";
                case MsgMediaType.SchoolRankCard:
                    SchoolRankCardMessage schoolRankCardMessage = PaidQAMessage.Create<SchoolRankCardMessage>(message);
                    return "[榜单卡片]";
                default:
                    return "[未知消息]";
            }
        }


        public virtual string Serialize()
        {
            string content = JsonConvert.SerializeObject(this);
            return content;
        }
    }
}
