using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.CommentViewModel
{
    /// <summary>
    /// 点评回复
    /// </summary>
    public class CommentReplyViewModel
    {
        /// <summary>
        /// 回复Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 点评Id
        /// </summary>
        public Guid CommentId { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeTotal { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyTotal { get; set; }
        /// <summary>
        /// 是否为校方用户发布
        /// </summary>
        public bool IsStudent { get; set; }
        /// <summary>
        /// 是否匿名发布
        /// </summary>
        public bool IsAnonymou { get; set; }
        /// <summary>
        /// 是否为在读生 | 过来人
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 发表日期
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 写入者
        /// </summary>
        public UserViewModel UserInfoVo { get; set; }
    }
}
