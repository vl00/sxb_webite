using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 审核日志记录表
    /// </summary>
    [Table("ExaminerRecords")]
    public class ExaminerRecord
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 审核类型，1：点评，2：问答，3：问题,4：结算
        /// </summary>
        public ExaminerRecordType ExaminerType { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid AdminId { get; set; }
        public bool IsPartTimeJob { get; set; }
        /// <summary>
        /// 被审核的数据
        /// </summary>
        public Guid TargetId { get; set; }
        /// <summary>
        /// 更改前状态
        /// </summary>
        public int ChangeFirstStatus { get; set; }
        /// <summary>
        /// 更改后的状态
        /// </summary>
        public int ChangeAfterStatus { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ExaminerTime { get; set; }
    }
}
