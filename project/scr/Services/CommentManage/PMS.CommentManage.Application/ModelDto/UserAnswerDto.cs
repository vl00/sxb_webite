using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;

namespace PMS.CommentsManage.Application.ModelDto
{
    /// <summary>
    /// 回复详情
    /// </summary>
    public class UserAnswerDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 回答内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        public string FormatterCreateTime { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// 发表人
        /// </summary>
        public TalentUser User { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
    }
}
