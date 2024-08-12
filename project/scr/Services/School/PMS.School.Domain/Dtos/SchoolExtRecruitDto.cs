using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtRecruitDto
    {
        public string ExtName { get; set; }
        public string SchoolName { get; set; }
        public byte? Age { get; set; }
        public byte? MaxAge { get; set; }
        public string Target { get; set; }
        public int? Count { get; set; }
        /// <summary>
        /// 招生比例
        /// </summary>
        public float? Proportion { get; set; }
        public string Point { get; set; }
        /// <summary>
        /// 奖学金计划
        /// </summary>
        public string ScholarShip { get; set; }
        public string Date { get; set; }
        public string Contact { get; set; }
        /// <summary>
        /// 考试科目
        /// </summary>
        public string Subjects { get; set; }
        public string Data { get; set; }
        /// <summary>
        /// 往期入学考试内容
        /// </summary>
        public string Pastexam { get; set; }
        /// <summary>
        /// 学校荣誉
        /// </summary>
        public string Schoolhonor { get; set; }
        /// <summary>
        /// 学生荣誉
        /// </summary>
        public string Studenthonor { get; set; }
        /// <summary>
        /// 校长风采
        /// </summary>
        public string Principal { get; set; }
        /// <summary>
        /// 教师风采
        /// </summary>
        public string Teacher { get; set; }
        /// <summary>
        /// 硬件设施
        /// </summary>
        public string Hardware { get; set; }
        /// <summary>
        /// 社团活动
        /// </summary>
        public string Community { get; set; }
        /// <summary>
        /// 各个年级课程表
        /// </summary>
        public string TimeTables { get; set; }
        /// <summary>
        /// 作息时间表
        /// </summary>
        public string Schedule { get; set; }
        /// <summary>
        /// 校车路线
        /// </summary>
        public string Diagram { get; set; }
    }
}
