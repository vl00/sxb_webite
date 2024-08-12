using Sxb.Web.Models.Replay;
using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.Answer
{
    /// <summary>
    /// 问答
    /// </summary>
    public class AnswerInfoVo
    {
        public Guid Id { get; set; }
        public UserInfoVo UserInfo { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        public int Reply { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前回复是否为精华
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string AnswerContent { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public string AddTime { get; set; }

        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 问题id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 回复父级id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 父级写入id
        /// </summary>
        public Guid? ParentUserId { get; set; }
        public UserInfoVo ParentUserInfo { get; set; }
        public bool ParentUserIdIsAnony { get; set; }

        /// <summary>
        /// 最新回复
        /// </summary>
        public ReplyDetailViewModel NewestReply { get; set; }
    }
}
