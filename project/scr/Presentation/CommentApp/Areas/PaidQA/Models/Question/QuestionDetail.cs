using Sxb.Web.Areas.PaidQA.Models.Evaluate;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Question
{
    public class QuestionDetailRequest
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }

    }
    public class QuestionDetailResult
    {
        [Description("对方用户信息")]
        public UserInfoResult Other { get; set; }

        [Description("专家信息")]
        public TalentInfoResult Talent { get; set; }

        [Description("订单信息")]
        public OrderInfoResult OrderInfo { get; set; }

        [Description("评价信息")]
        public EvaluateResult Evaluate { get; set; }

        [Description("聊天记录")]
        public List<MessageResult> ChatRecords { get; set; }

        [Description("是否已经有了回复")]
        public bool IsReply { get; set; }

        [Description("是否回答者")]
        public bool IsAnswer { get; set; }





    }


    [Description("消息")]
    public class MessageResult
    {
        [Description("消息ID")]
        public Guid ID { get; set; }

        [Description("发送者ID")]
        public Guid? SenderID { get; set; }

        [Description("接收者ID")]
        public Guid? ReceiveID { get; set; }

        [Description("内容")]
        public string Content { get; set; }

        [Description("媒体类型：1文字 2图片 3语音 4推荐卡片 5图文")]
        public int MediaType { get; set; }

        [Description("消息类型：1系统消息 2客服消息 3提问 4回答 ")]
        public int? MsgType { get; set; }

        [Description("发送时间")]
        public DateTime? CreateTime { get; set; }

        [Description("阅读时间")]
        public DateTime? ReadTime { get; set; }

        [Description("是否是自己的消息")]
        public bool IsBelongSelf { get; set; }

        [Description("发送者信息")]
        public UserInfoResult SenderInfo { get; set; }

    }


    [Description("用户信息")]
    public class UserInfoResult {

        [Description("用户ID")]
        public Guid ID { get; set; }

        [Description("用户昵称")]
        public string NickName { get; set; }

        [Description("用户头像")]
        public string HeadImgUrl { get; set; }
    }

}
