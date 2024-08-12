using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.ModelDto.ModelVo
{
    public class QuestionAnswerData
    {
        public List<QuestionVo> questionModels { get; set; }
        public List<AnswerInfoVo> answerModels { get; set; }
    }
    /// <summary>
    /// 问题
    /// </summary>
    public class QuestionVo
    {
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
        public UserInfoModel UserInfo { get; set; }
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
            /// 是否住校
            /// </summary>
            public bool IsLoding { get; set; }
            /// <summary>
            /// 是否认证
            /// </summary>
            public bool IsAuth { get; set; }
        }
    }
}