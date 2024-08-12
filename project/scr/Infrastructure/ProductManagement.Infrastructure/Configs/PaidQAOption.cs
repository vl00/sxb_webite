using ProductManagement.Infrastructure.Configs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductManagement.Infrastructure.Configs
{
    public class PaidQAOption
    {
        public const string PaidQA = "PaidQA";

        public WechatMessageTplSetting WechatMessageTplSetting { get; set; }

        public CustomMsgSetting CustomMsgSetting { get; set; }
        public MobileMsgTplSetting MobileMsgTplSetting { get; set; }

    }

    public class CustomMsgSetting
    {

        /// <summary>
        /// 转单提醒
        /// </summary>
        public TextCustomMsgTpl TranstingTips { get; set; }

        /// <summary>
        /// 达人不回复提醒
        /// </summary>
        public TextCustomMsgTpl TalentUnReplyTips { get; set; }

        /// <summary>
        /// 达人拒绝订单提醒
        /// </summary>
        public TextCustomMsgTpl TalentRefusOrderTips { get; set; }

        /// <summary>
        /// 提醒用户评价
        /// </summary>
        public TextCustomMsgTpl EvaluteTips { get; set; }


        /// <summary>
        /// 跳转到立即咨询的客服图文消息配置
        /// </summary>

        public TxtMsgTemplate AskAtOnceTips { get; set; }

        /// <summary>
        ///  高招会期间, 在AskAtOnceTips之前先发一次高招回消息
        /// </summary>
        public TxtMsgTemplate UniversityAskAtOnceTips { get; set; } = new TxtMsgTemplate
        {
            Title = "2021年全国高考招生咨询会火热进行中，快来了解一下吧！",
            Description = "点击进入活动页面，查看咨询会详情！",
            ImgUrl = "", //达人头像
            RedirectUrl =  "{mHost}/high-recruitment"//https://m3.sxkid.com
        };
    }

    /// <summary>
    /// 文字客服消息模板
    /// </summary>
    public class TextCustomMsgTpl
    {

        public string Content { get; set; }

    }

    public class WechatMessageTplSetting
    {

        /// <summary>
        /// 达人回复问题
        /// </summary>
        public TplMsgCofig DarenAnswerPayAsk { get; set; }

        /// <summary>
        /// 收到问题
        /// </summary>
        public TplMsgCofig PayAskRecieve { get; set; }
        /// <summary>
        /// 问答超过11小时未回复
        /// </summary>
        public TplMsgCofig TimeoutNotReply { get; set; }

        /// <summary>
        /// 订单已超时，用户已转单提醒
        /// </summary>
        public TplMsgCofig TimeoutOrderChange { get; set; }
        /// <summary>
        /// 用户发起追问
        /// </summary>
        public TplMsgCofig AddAsk { get; set; }
        /// <summary>
        /// 用户发起最后一条追问
        /// </summary>
        public TplMsgCofig LastAddAsk { get; set; }
        /// <summary>
        /// 用户已评价
        /// </summary>

        public TplMsgCofig UserEvaluate { get; set; }
        /// <summary>
        /// 转单
        /// </summary>
        public TplMsgCofig ChangeOrder { get; set; }

        public WXTemplateMsg CreateQuestion { get; set; }

        public WXTemplateMsg OrderTimeOut { get; set; }

        /// <summary>
        /// 为提问提醒
        /// </summary>
        public WXTemplateMsg UnAskTips { get; set; }

        /// <summary>
        /// 为提问提醒
        /// </summary>
        public WXTemplateMsg CouponExpireTips { get; set; }


    }
    public class MobileMsgTplSetting
    {
        public MobileMsgTplCofigBase TalentRefusOrderTipsNotify { get; set; }
        
        public MobileMsgTplCofigBase TalentUnReplyTipsNotify { get; set; }
        
        public MobileMsgTplCofigBase TranstingTipsNotify { get; set; }
        public MobileMsgTplCofigBase EvaluteTipsNotify { get; set; }
        
        public MobileMsgTplCofigBase RemainOnHoursNotify { get; set; }
        public MobileMsgTplCofigBase SendCreatorHasNewsNotify { get; set; }


        public MobileMsgTplCofigBase CreateQuestionNotify { get; set; }

        public MobileMsgTplCofigBase RefundNotify { get; set; }

        public MobileMsgTplCofigBase UnAskTips { get; set; }
        public MobileMsgTplCofigBase ReplyTips { get; set; }
        public MobileMsgTplCofigBase CouponExpireTips { get; set; }


    }
    public class TplMsgCofig
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string tplid { get; set; }
        /// <summary>
        /// 点击模板消息跳转的地址
        /// </summary>
        public string link { get; set; }

    }


    /// <summary>
    /// 短信模板基类
    /// </summary>
    public class MobileMsgTplCofigBase
    {

        /// <summary>
        /// 模板ID
        /// </summary>
        public string tplid { get; set; }

        public List<string> tplParams { get; set; }

    }

    public class RefundNotify : MobileMsgTplCofigBase
    {
        /// <summary>
        /// 点击模板消息跳转的地址
        /// </summary>
        public string link { get; set; }
    }

}
