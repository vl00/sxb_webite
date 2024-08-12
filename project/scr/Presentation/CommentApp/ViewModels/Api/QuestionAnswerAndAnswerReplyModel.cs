using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Api
{
    /// <summary>
    /// 问题的回答，回答的回复
    /// </summary>
    public class QuestionAnswerAndAnswerReplyModel
    {
        /// <summary>
        /// 发表者用户信息
        /// </summary>
        public UserInfoVo userInfoVo { get; set; }

        /// <summary>
        /// 回复者信息用户信息
        /// </summary>
        public UserInfoVo answerUserInfoVo { get; set; }

        /// <summary>
        /// id 【问题id | 回答id】  
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 回答id
        /// </summary>
        public Guid AnswerId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string AnswerContent { get; set; }
        /// <summary>
        /// 提问回答数
        /// </summary>
        public int AnswerCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 点评写入时间 | 回复写入时间
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 回复写入时间
        /// </summary>
        public string AnswerAddTime { get; set; }
        /// <summary>
        /// 0：提问的回答，1：回答的回复
        /// </summary>
        public int Type { get; set; }
    }
}
