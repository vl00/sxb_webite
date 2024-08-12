using System;
using System.Collections.Generic;
using System.Text;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class QuestionReportDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 举报人ID
        /// </summary>
        /// <value>The report user identifier.</value>
        public Guid ReportUserId { get; set; }
        /// <summary>
        /// 举报类型
        /// </summary>
        public ReportDataType ReportDataType { get; set; }


        public string ReportReason { get; set; }

        /// <summary>
        /// 举报内容
        /// </summary>
        public string ReportContent { get; set; }

        /// <summary>
        /// 举报内容 0、未回复 1、已回复
        /// </summary>
        public ReportStatus Status { get; set; }

        /// <summary>
        /// 举报时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        public AnswerDto Answer { get; set; }

    }
}
