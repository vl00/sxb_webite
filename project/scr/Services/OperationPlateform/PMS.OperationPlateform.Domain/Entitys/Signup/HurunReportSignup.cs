using Dapper.Contrib.Extensions;
using System;

namespace PMS.OperationPlateform.Domain.Entitys.Signup
{
    [Table("SignupHurunReport")]
    public class HurunReportSignup
    {
        /// <summary>
        /// 报名ID
        /// </summary>
        [ExplicitKey]
        public Guid ID { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工作单位
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 工作年限
        /// </summary>
        public int WorkingYears { get; set; }
        /// <summary>
        /// 问题三
        /// </summary>
        public string Question3 { get; set; }
        /// <summary>
        /// 问题四
        /// </summary>
        public string Question4 { get; set; }
        /// <summary>
        /// 华东地区学校
        /// </summary>
        public string EastSchools { get; set; }
        /// <summary>
        /// 华南地区学校
        /// </summary>
        public string SouthSchools { get; set; }
        /// <summary>
        /// 北方地域学校
        /// </summary>
        public string NorthernSchools { get; set; }
        /// <summary>
        /// 西部及其余地区学校
        /// </summary>
        public string WesternSchools { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
    }
}
