using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
    [Serializable]
    [Table("LSRFSchools")]
    public class LSRFSchool
    {
        [ExplicitKey]
        public int Id { get; set; }
        public Guid? SchId { get; set; }
        public int CourseSetting { get; set; }
        /// <summary>
        /// 推荐类型 1：普通 2：推荐 3：广告
        /// </summary>
        public LSRFSchoolType Type { get; set; }
        public bool IsDel { get; set; }
        public int Sort { get; set; }
        public int City { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Creator { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Updator { get; set; }
    }
}
