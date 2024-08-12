using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Domain.Dtos;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Application.Common;

namespace PMS.CommentsManage.Application.ModelDto
{
    /// <summary>
    /// 问题及回复详情展示
    /// </summary>
    public class UserCommentDto
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
        /// 点评内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 学校点评数
        /// </summary>
        public int SchoolCommentCount { get; set; }

        /// <summary>
        /// 点亮
        /// </summary>
        public int Stars => SchoolScoreToStart.GetCurrentSchoolstart(Score);

        /// <summary>
        /// 点评分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }

        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }

        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public IEnumerable<string> Images { get; set; }

        public SchoolExtAggDto School { get; set; }

        /// <summary>
        /// 发表人
        /// </summary>
        public TalentUser User { get; set; }
    }
}
