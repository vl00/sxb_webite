using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Ad;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.PaidQA;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle
{
    public enum SubscribeQRCodeType
    {
        //默认（无任何场景）
        [SubscribeCallBackHandler(typeof(DefaultHandler))]
        Default = 0,

        [Description("加入圈子场景")]
        [SubscribeCallBackHandler(typeof(JoinCricleHandler))]
        JoinCircle = 1,

        [Description("查看完整的帖子场景")]
        [SubscribeCallBackHandler(typeof(ViewTopicDetailHandler))]
        ViewTopicDetail = 2,

        [Description("绑定账户")]
        [SubscribeCallBackHandler(typeof(BindAccountHandler))]
        BindAccount = 3,

        [Description("欢迎达人关注")]
        [SubscribeCallBackHandler(typeof(WelcomTalentHandler))]
        WelcomTalent = 4,

        [Description("上学问立刻咨询场景")]
        [SubscribeCallBackHandler(typeof(PaidQAAskAtOnceHandle))]
        PaidQAAskAtOnce = 5,

        [Description("开启上学问")]
        [SubscribeCallBackHandler(typeof(EnablePaidQAHandler))]
        EnablePaidQA = 6,

        [Description("关注记数")]
        [SubscribeCallBackHandler(typeof(FollowCountHandler))]
        FollowCount = 7,

        [Description("广告推送资料包")]
        [SubscribeCallBackHandler(typeof(AdWxDataPacketHandler))]
        AdWxData = 8,

        [Description("学校对比页")]
        [SubscribeCallBackHandler(typeof(SchoolComparePageHandler))]
        SchoolComparePage = 9,

        [Description("学校详情页")]
        [SubscribeCallBackHandler(typeof(SchoolDetailPageHandler))]
        SchoolDetailPage = 10,

        [Description("付费问答立即咨询页")]
        [SubscribeCallBackHandler(typeof(PaidAskPageHandler))]
        PaidAskPage = 11,

        [Description("机构详情页")]
        [SubscribeCallBackHandler(typeof(OrgHandler))]
        OrgDetailPage = 12,

        [Description("课程详情页")]
        [SubscribeCallBackHandler(typeof(CourseDetailPageHandler))]
        CourseDetailPage = 13,

        [Description("小程序消息转发")]
        [SubscribeCallBackHandler(typeof(WXMPMsgForwardHandler))]
        WXMPMsgForward = 14,

        [Description("网课通拉新顾问海报")]
        [SubscribeCallBackHandler(typeof(WangKeTongGuWenHaiBaoHandler))]
        WangKeTongGuWenHaiBao = 16,


        [Description("课程订单详情页面")]
        [SubscribeCallBackHandler(typeof(CourseOrderDetailPageHandler))]
        OrgOrderDetail = 18,

        [Description("通用跳转卡片")]
        [SubscribeCallBackHandler(typeof(OrgCommonPageHandler))]
        OrgCommonPage = 19,


        [Description("预约提醒")]
        [SubscribeCallBackHandler(typeof(SubscribeRemindHandler))]
        SubscribeRemind = 20,

        [Description("下载文件")]
        [SubscribeCallBackHandler(typeof(FileDownloadHandler))]
        FileDownload = 21,

        [Description("学位分析")]
        [SubscribeCallBackHandler(typeof(SchoolAssessHandler))]
        SchoolAssess = 22,

        [Description("签到提醒关注回调")]
        [SubscribeCallBackHandler(typeof(PointsMallSignInNotifyHandler))]
        PointsMallSignInNotify = 23,
    }

    public static class SubscribeQRCodeTypeExtension
    {
        public static Type GetHandler(this SubscribeQRCodeType subscribeQRCodeType)
        {
            Type type = subscribeQRCodeType.GetType();
            FieldInfo field = type.GetField(subscribeQRCodeType.ToString());
            if (field == null)
                return null;
            var attrs = field.GetCustomAttribute<SubscribeCallBackHandlerAttribute>();
            if (attrs == null)
                return null;
            else
                return attrs.THandler;
        }

    }
}
