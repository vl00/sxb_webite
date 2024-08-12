﻿using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Application.ModelDto
{
    /// <summary>
    /// 问题及回复详情展示
    /// </summary>
    public class CommentSchoolDto
    {
        public Guid ExtId { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 分部所在学校的评论数
        /// </summary>
        public int CommentTotal { get; set; }

        /// <summary>
        /// 点亮
        /// </summary>
        public int SchoolStars { get; set; }
    }
}
