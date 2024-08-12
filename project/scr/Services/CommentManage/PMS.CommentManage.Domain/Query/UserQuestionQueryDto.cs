using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using ProductManagement.Framework.Foundation;

namespace PMS.CommentsManage.Domain
{
    /// <summary>
    /// 问题及回复详情展示
    /// </summary>
    public class UserQuestionQueryDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 写入的用户id
        /// </summary>
        public Guid UserId { get; set; }
        public Guid ExtId { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public long No { get; set; }
        public string ShortNo => UrlShortIdUtil.Long2Base32(No)?.ToLower();

        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最高回复的答案id
        /// </summary>
        public Guid? TopReplyAnswerId { get; set; }
        
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 学校问题数
        /// </summary>
        public int SchoolQuestionCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

    }
}
