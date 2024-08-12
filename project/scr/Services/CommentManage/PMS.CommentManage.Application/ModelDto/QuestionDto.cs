using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Application.ModelDto
{
    /// <summary>
    /// 问题及回复详情展示
    /// </summary>
    public class QuestionDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public long No { get; set; }
        /// <summary>
        /// 问题id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }

        public Guid UserId { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string QuestionContent { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int AnswerCount { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool isLike { get; set; }
        /// <summary>
        /// 当前用户是否回复
        /// </summary>
        public bool isAnswer { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 问题图片
        /// </summary>
        public List<string> Images { get; set; }
        public DateTime QuestionCreateTime { get; set; }
        /// <summary>
        /// 当前问题的回答详情
        /// </summary>
        public List<AnswerInfoDto> answer { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool isSelected { get; set; }
        /// <summary>
        /// 是否存在问题
        /// </summary>
        public bool isExists { get; set; }

        public ExamineStatus State { get; set; }
        public int? TalentType { get; set; }

        public class SchoolInfo
        {
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
            public bool isInternactioner { get; set; }
            /// <summary>
            /// 是否住校
            /// </summary>
            public bool isLoding { get; set; }
            /// <summary>
            /// 是否认证
            /// </summary>
            public bool isAuth { get; set; }
            /// <summary>
            /// 分部名称
            /// </summary>
            public string SchoolBranch { get; set; }
        }
    }
}
