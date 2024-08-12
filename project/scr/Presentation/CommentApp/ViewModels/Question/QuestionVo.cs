using PMS.CommentsManage.Application.ModelDto;
using Sxb.Web.Models.Answer;
using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;

namespace Sxb.Web.Models.Question
{
    /// <summary>
    /// 问题
    /// </summary>
    public class QuestionVo
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        public string No { get; set; }
        public Guid Id { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 写入者
        /// </summary>
        public Guid UserId { get; set; }
        public int? TalentType { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string QuestionContent { get; set; }
        /// <summary>
        /// 点赞总数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回答总数
        /// </summary>
        public int AnswerCount { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前用户是否回复
        /// </summary>
        public bool IsAnswer { get; set; }
        /// <summary>
        /// 问题图片
        /// </summary>
        public List<string> Images { get; set; }
        /// <summary>
        /// 问题写入时间
        /// </summary>
        public string QuestionCreateTime { get; set; }
        /// <summary>
        /// 提问者用户信息
        /// </summary>
        public UserInfoVo UserInfo { get; set; }
        /// <summary>
        /// 当前问题的回答详情
        /// </summary>
        public List<AnswerInfoVo> Answer { get; set; }
        /// <summary>
        /// 学校总问题 / 回复数
        /// </summary>
        public SchoolInfo School { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 是否存在问题
        /// </summary>
        public bool IsExists { get; set; }
        public class SchoolInfo
        {
            //学校类型
            public int SchoolType { get; set; }
            public string TypeName => ((PMS.OperationPlateform.Domain.Enums.SchoolType)SchoolType).GetDescription();
            /// <summary>
            /// 当前学校下的总问题数
            /// </summary>
            public int SchoolQuestionTotal { get; set; }
            /// <summary>
            /// 学校总回复数
            /// </summary>
            public int SchoolReplyTotal { get; set; }
            /// <summary>
            /// 学校名称
            /// </summary>
            public string SchoolName { get; set; }
            /// <summary>
            /// 是否为国际
            /// </summary>
            public bool IsInternactioner { get; set; }

            /// <summary>
            /// 寄宿类型
            /// </summary>
            public int LodgingType { get; set; }
            public string LodgingTypeName => LodgingReason;
            /// <summary>
            /// 寄宿类型描述
            /// </summary>
            public string LodgingReason { get; set; }
            /// <summary>
            /// 是否认证
            /// </summary>
            public bool IsAuth { get; set; }
            /// <summary>
            /// 分部名称
            /// </summary>
            public string SchoolBranch { get; set; }
            public int SchoolNo { get; set; }
            public string ShortSchoolNo
            {
                get
                {
                    return ProductManagement.Framework.Foundation.UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
                }
            }
        }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }
}
