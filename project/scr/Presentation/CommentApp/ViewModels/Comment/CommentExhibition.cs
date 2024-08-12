using CommentApp.Models.School;
using PMS.CommentsManage.Domain.Entities;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Entities;
using Sxb.Web.Models.User;
using Sxb.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.Comment
{
    /// <summary>
    /// 前端点评展示
    /// </summary>
    public class CommentExhibition
    {
        //public PMS.UserManage.Domain.Entities.UserInfo userInfo { get; set; }
        public UserInfoVo UserInfo { get; set; }
        public SchoolExtensionVo School { get; set; }
        public SchoolCommentViewModel SchoolComment { get; set; }
        public List<SchoolImage> SchoolImages { get; set; }
        public List<SchoolTag> Tags { get; set; }
        public List<SchoolCommentReplyVo> CommentReplies { get; set; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsLike { get; set; }
        /// <summary>
        /// 当前用户是否评论
        /// </summary>
        public bool IsComemt { get; set; }
        public int StartTotal { get; set; }
        public int SchoolScore { get; set; }
        /// <summary>
        /// 是否为精选
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
