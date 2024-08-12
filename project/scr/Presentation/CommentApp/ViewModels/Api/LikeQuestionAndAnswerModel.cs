using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Api
{
    /// <summary>
    /// 点赞-回答：当前用户点赞的“问题”+当前用户点赞的“回答”+当前用户点赞的“回答”的回复
    /// </summary>
    public class LikeQuestionAndAnswerModel
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
        /// 问题id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 回答id
        /// </summary>
        public Guid ParentAnswerId { get; set; }
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
        /// 回答的回复内容
        /// </summary>
        public string AnswerContent { get; set; }
        /// <summary>
        /// 回答数
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
        /// 写入时间
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 回答写入时间
        /// </summary>
        public string AnswerAddTime { get; set; }
        /// <summary>
        /// 0：回答，1：回复
        /// </summary>
        public int Type { get; set; }
    }
}
