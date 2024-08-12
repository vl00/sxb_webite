using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.UserManage.Domain.Entities
{
    [Table("talent_staff")]
    public class TalentStaff
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }

        /// <summary>
        /// 员工达人id
        /// </summary>
        public Guid talent_id { get; set; }

        /// <summary>
        /// 员工归属的机构达人id
        /// </summary>
        public Guid parentId { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 状态 0:已邀请 ，1:已接受
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 删除标记 0否，1是
        /// </summary>
        public int? isdelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [NotMapped]
        public Guid userId { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [NotMapped]
        public string headImgUrl { get; set; }
    }
}
