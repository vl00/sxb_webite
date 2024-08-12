using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Enums
{
    public enum MsgMediaType
    {
        [Description("文字")]
        Text = 1,
        [Description("图片")]
        Image = 2,
        [Description("声音")]
        Voice = 3,
        [Description("转单卡片")]
        RecommandCard = 4,
        [Description("图文")]
        TXT = 5,
        [Description("达人卡片")]
        RandomTenlentCard = 6,
        [Description("系统状态消息")]
        SystemStatu = 7,
        [Description("客服消息")]
        Custom = 8,
        [Description("系统消息")]
        System = 9,
        [Description("附件消息")]
        Attachment = 10,
        [Description("学校卡片消息")]
        SchoolCard = 11,
        [Description("文章卡片消息")]
        ArticleCard = 12,
        [Description("机构测评卡片消息")]
        OrgEvaluationCard = 13,
        [Description("机构卡片消息")]
        OrgCard = 14,
        [Description("课程卡片消息消息")]
        CourseCard = 15,
        [Description("直播卡片消息")]
        LiveCard = 16,
        [Description("榜单卡片消息")]
        SchoolRankCard = 17,

    }
}
