//using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.ModelVo.CommentVo
{
    /// <summary>
    /// 回复
    /// </summary>
    public class ReplayExhibition
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 写入者
        /// </summary>
        public Guid ReplyUserId { get; set; }
        /// <summary>
        /// 点评id
        /// </summary>
        public Guid SchoolCommentId { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplayCount { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 写入用户
        /// </summary>
        public UserInfoModel User { get; set; }
        /// <summary>
        /// 是否为学生
        /// </summary>
        public bool IsStudent { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnonymou { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
    }
}
