using System;
using System.Collections.Generic;
using System.Text;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class CommentReportDto 
    {
        public CommentReportDto()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        /// <summary>
        /// 举报人ID
        /// </summary>
        /// <value>The report user identifier.</value>
        public Guid ReportUserId { get; set; }

        /// <summary>
        /// 点评ID
        /// </summary>
        public Guid CommentId { get; set; }
        /// <summary>
        /// 回复ID
        /// </summary>
        public Guid? ReplayId { get; set; }
        public int ReportReasonType { get; set; }
        public string ReportReason { get; set; }
        /// <summary>
        /// 举报内容
        /// </summary>
        public string ReportDetail { get; set; }

        /// <summary>
        /// 举报内容 0、未回复 1、已回复
        /// </summary>
        public ReportStatus Status { get; set; }



        /// <summary>
        /// 举报时间
        /// </summary>
        public DateTime ReportTime { get; set; }
        public ReportDataType ReportDataType { get; set; }


        /// <summary>
        /// 点评内容
        /// </summary>
        public SchoolCommentDto SchoolComment { get; set; }

    }
}
