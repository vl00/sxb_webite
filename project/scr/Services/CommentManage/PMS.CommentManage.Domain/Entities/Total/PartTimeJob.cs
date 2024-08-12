using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.Total
{
    public class PartTimeJob
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 微信用户身份唯一标识
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public Guid JobId { get; set; }
        /// <summary>
        /// 精选点评
        /// </summary>
        public int TotalSchoolCommentsSelected { get; set; }
        /// <summary>
        /// 精选问答
        /// </summary>
        public int TotalAnswerSelected { get; set; }
    }
}
