using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("LSRFLeveInfo")]
    public class LSRFLeveInfo
    {
        [Key]
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int City { get; set; }
        /// <summary>
        /// 留资类型 1：快捷，2：普通
        /// </summary>
        public LSRFLeveInfoType Type { get; set; }
        public int Area { get; set; }
        public int Stage { get; set; }
        public int CourseSetting { get; set; }
        public Guid? SchId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Creator { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Updator { get; set; }
    }
}
