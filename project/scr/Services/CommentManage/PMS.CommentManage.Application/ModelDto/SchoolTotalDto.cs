using System;
namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolTotalDto
    {
        /// <summary>
        /// 统计总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }

        public string No { get; set; }
    }
}
