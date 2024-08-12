using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.Common
{
    public class EnumSet
    {
        public enum DeviceType
        {
            iOS = 0,
            Android = 1,
            Windows = 2,
            Chrome = 3
        }
        public enum ReportType
        {
            [Description("文章")]
            Article = 0,
            [Description("学校")]
            School = 1,
            [Description("问题")]
            QA = 2,
            [Description("点评")]
            Comment = 3,
            [Description("回答")]
            Answer = 4,
            [Description("点评回复")]
            Reply = 5
        }
        public enum CollectionType
        {
            [Description("文章")]
            Article = 0,
            [Description("学校")]
            School = 1,
            [Description("问答")]
            QA = 2,
            [Description("点评")]
            Comment = 3
        }
        /// <summary>
        /// 学校类型
        /// </summary>
        public enum SchoolType : int
        {
            [Description("")]
            unknown = 0,

            /// <summary>
            /// 公办
            /// </summary>
            [Description("公办")]
            Public = 1,
            /// <summary>
            /// 民办
            /// </summary>
            [Description("民办")]
            Private = 2,
            /// <summary>
            /// 国际
            /// </summary>
            [Description("国际")]
            International = 3,
            /// <summary>
            /// 外籍
            /// </summary>
            [Description("外籍")]
            ForeignNationality = 4,
            /// <summary>
            /// 港澳台
            /// </summary>
            [Description("港澳台")]
            SAR = 80,
            /// <summary>
            /// 其它
            /// </summary>
            [Description("其它")]
            Other = 99

        }

        /// <summary>
        /// 达人认证方式 0:审核认证 certification_way
        /// </summary>
        public enum TalentCertificationWay
        {
            [Description("审核认证")]
            Check,
            [Description("邀请认证")]
            Invitation,
            [Description("机构邀请")]
            OrganizationInvitation
        }

        /// <summary>
        /// 达人认证状态 0未审核，1已通过，2已驳回
        /// </summary>
        public enum TalentCertificationStatus
        {
            [Description("未审核")]
            Checking,
            [Description("已通过")]
            Pass,
            [Description("已驳回")]
            Reject
        }
        /// <summary>
        /// 达人类型
        /// </summary>
        public enum TalentType
        {
            [Description("个人")]
            Personal,
            [Description("机构")]
            Organization,
        }
        /// <summary>
        /// 达人认证类型 0:个人身份认证，1 机构身份认证，99 上学帮内部认证
        /// </summary>
        public enum TalentCertificationType
        {
            [Description("个人身份认证")]
            Personal,
            [Description("机构身份认证")]
            Organization,
            [Description("上学帮内部认证")]
            Sxb = 99
        }

        /// <summary>
        /// 达人认证说明 0：认证称号，1前缀+认证称号
        /// </summary>
        public enum TalentCertificationExplanation
        {
            [Description("认证称号")]
            Title,
            [Description("前缀+认证称号")]
            PrefixAndTitle
        }


        /// <summary>
        /// 达人审核状态 0 不通过，1 通过
        /// </summary>
        public enum TalentAuditStatus
        {
            [Description("不通过")]
            Fail,
            [Description("通过")]
            Pass
        }

        /// <summary>
        /// 达人审核驳回原因类型 0 证明材料不足，1 不符合认证身份，2 其他
        /// </summary>
        public enum TalentAuditReasonType
        {
            [Description("证明材料不足")]
            Incomplete,
            [Description("不符合认证身份")]
            Unright,
            [Description("其他")]
            Other
        }

        /// <summary>
        /// 0：文章 1：学校学部 2：问答 3：点评 4：用户 5：讲师 6: 圈子 7:   话题
        /// </summary>
        public enum CollectionDataType
        {
            [Description("文章")]
            Article = 0,
            [Description("学校学部")]
            School = 1,
            [Description("问答")]
            QA = 2,
            [Description("点评")]
            Comment = 3,
            [Description("用户")]
            User = 4,
            [Description("讲师")]
            UserLector = 5,
            //TopicCircle -> CircleFollower
            //[Description("圈子")]
            //Circle = 6,
            [Description("话题")]
            Topic = 7
        }

        /// <summary>
        /// 消息数据类型
        /// </summary>
        public enum MessageDataType
        {
            [Description("文章")]
            Article = 0,
            [Description("学校")]
            School = 1,
            [Description("问题")]
            Question = 2,
            [Description("点评")]
            Comment = 3,
            [Description("问题回复")]
            Answer = 4,
            [Description("点评回复")]
            CommentReply = 5,
            [Description("用户")]
            User = 6,
            [Description("直播")]
            Lecture = 7,
            [Description("话题圈")]
            Circle = 8,
            [Description("话题")]
            Topic = 9,
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public enum MessageType
        {
            [Description("通知")]
            Notice = 0,

            /// <summary>
            /// 非邀请回复
            /// </summary>
            [Description("回复")]
            Reply = 1,

            [Description("邀请点评")]
            InviteComment = 2,
            [Description("邀请提问")]
            InviteQuestion = 3,
            [Description("邀请回答")]
            InviteAnswer = 4,
            [Description("邀请回复点评")]
            InviteReplyComment = 7,

            [Description("点赞")]
            Like = 11,

            [Description("关注")]
            Follow = 16,
        }
    }
}
