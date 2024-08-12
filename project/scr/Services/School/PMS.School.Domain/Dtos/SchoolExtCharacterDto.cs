using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtCharacterDto
    {
        /// <summary>
        /// 学校资质
        /// </summary>
        public double? SchoolCredentials { get; set; }
        /// <summary>
        /// 师资力量
        /// </summary>
        public double? TeacherPower { get; set; }
        /// <summary>
        /// 课程种类
        /// </summary>
        public double? CoureType { get; set; }

        /// <summary>
        /// 周边舒适度
        /// </summary>
        public double? Comfort { get; set; }


        /// <summary>
        /// 交通便利性
        /// </summary>
        public double? TrafficEasy { get; set; }

        /// <summary>
        /// 周边安全性
        /// </summary>
        public double? Safety { get; set; }

        /// <summary>
        /// 家长教育投入
        /// </summary>
        public double? RegionaleConomic { get; set; }

        /// <summary>
        /// 学位价值预测
        /// </summary>
        public double? PopulationDensity { get; set; }

        /// <summary>
        /// 区域教育投入
        /// </summary>
        public double? Edufund { get; set; }
        /// <summary>
        /// 硬件设施
        /// </summary>
        public double? Hardware { get; set; }
        /// <summary>
        /// 升学情况
        /// </summary>
        public double? UpgradeEasy { get; set; }
        /// <summary>
        /// 入学难度
        /// </summary>
        public double? EnterEasy { get; set; }
        /// <summary>
        /// 费用负担
        /// </summary>
        public double? Cost { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        public double? TotalScore { get; set; }
    }
}
