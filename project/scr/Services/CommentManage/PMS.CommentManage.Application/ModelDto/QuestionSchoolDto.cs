using PMS.CommentsManage.Domain.Entities;
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
    public class QuestionSchoolDto
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
        /// 分部所在学校的问题数
        /// </summary>
        public int QuestionTotal { get; set; }

        /// <summary>
        /// 是否为国际
        /// </summary>
        public bool IsInternactioner { get; set; }

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool IsLodging { get; set; }

        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth { get; set; }
    }
}
