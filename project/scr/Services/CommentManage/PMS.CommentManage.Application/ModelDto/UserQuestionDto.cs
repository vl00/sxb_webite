using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Domain.Dtos;
using ProductManagement.Framework.Foundation;

namespace PMS.CommentsManage.Application.ModelDto
{
    /// <summary>
    /// 问题及回复详情展示
    /// </summary>
    public class UserQuestionDto
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
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 学校问题总数
        /// </summary>
        public int SchoolQuestionCount { get; set; }

        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public IEnumerable<string> Images { get; set; }

        /// <summary>
        /// 当前问题的回答详情
        /// 1、显示回复数最多的一条评论
        /// 2、若回复数同样多，则显示最近的一条
        /// </summary>
        public UserAnswerDto Answer { get; set; }

        public SchoolExtAggDto School { get; set; }

        /// <summary>
        /// 发表人
        /// </summary>
        public TalentUser User { get; set; }
    }
}
