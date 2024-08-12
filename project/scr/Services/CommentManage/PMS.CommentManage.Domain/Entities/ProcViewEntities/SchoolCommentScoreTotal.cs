using System;
namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    public class SchoolCommentScoreTotal
    {
        public Guid SchoolId { get; set; }
        public Guid SchoolSectionId { get; set; }
        public Guid SchoolName { get; set; }

        public decimal AggScore { get; set; }

        /// <summary>
        /// 师资力量分
        /// </summary>
        public decimal TeachScore { get; set; }

        /// <summary>
        /// 硬件设施分
        /// </summary>
        public decimal HardScore { get; set; }

        /// <summary>
        /// 环境周边分
        /// </summary>
        public decimal EnvirScore { get; set; }

        /// <summary>
        /// 学风管理分
        /// </summary>
        public decimal ManageScore { get; set; }

        /// <summary>
        /// 校园生活分
        /// </summary>
        public decimal LifeScore { get; set; }
    }
}
