using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Domain.Entities.Mongo;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.School
{
    public class GetUpgradeReportRequest
    {
        public Guid EID { get; set; }
    }

    public class GetUpgradeReportResponse
    {
        public string SchoolName { get; set; }
        public string ExtName { get; set; }
        public string SchoolImgUrl { get; set; }
        public List<string> Tags { get; set; }
        /// <summary>
        /// 学校总评分
        /// </summary>
        public double SchoolTotalScore { get; set; }
        /// <summary>
        /// 学校总评分区域排名
        /// </summary>
        public double SchoolTotalScoreRanking { get; set;    }
        double _teachingLevel;
        /// <summary>
        /// 教学实力
        /// <para>综合 + 师资 + 硬件 的Avg</para>
        /// </summary>
        public double TeachingLevel { get { return (_teachingLevel / 10).CutDoubleWithN(1); } set { _teachingLevel = value; } }
        /// <summary>
        /// 生源质量
        /// </summary>
        public double StudentSourceQuality { get; set; }
        /// <summary>
        /// 周边配套
        /// </summary>
        public double Peripheral { get; set; }
        /// <summary>
        /// 周边配套 , 区域均值
        /// </summary>
        public double PeripheralAreaAvg { get; set; }
        /// <summary>
        /// 推荐达人
        /// </summary>
        public TalentDetailExtend Talent { get; set; }
        /// <summary>
        /// 学校评级
        /// </summary>
        public string Str_Score { get; set; }
        /// <summary>
        /// 评级排名
        /// </summary>
        public int ScoreLevel { get; set; }
        /// <summary>
        /// 同区同级学校数量
        /// </summary>
        public double SameScoreSchoolCountPercent { get; set; }
        /// <summary>
        /// 同区更高级学校数量
        /// </summary>
        public double HigherScoreSchoolCountPercent { get; set; }
        /// <summary>
        /// 学校资质
        /// </summary>
        public int Qualification { get; set; } = 1;
        /// <summary>
        /// 区域资质
        /// </summary>
        public int QualificationArea { get; set; } = 1;
        /// <summary>
        /// 学校师资
        /// </summary>
        public int Teachers { get; set; } = 1;
        /// <summary>
        /// 区域师资
        /// </summary>
        public int TeachersArea { get; set; } = 1;
        /// <summary>
        /// 学校硬件
        /// </summary>
        public int Hardware { get; set; } = 1;
        /// <summary>
        /// 区域硬件
        /// </summary>
        public int HardwareArea { get; set; } = 1;
        public AiParams SchoolOtherInfo { get; set; }
        public AiParams SchoolOtherAvgInfo { get; set; }
        public GDParams SchoolSurroundInfo { get; set; }
        public GDParams SchoolSurroundAvgInfo { get; set; }
        /// <summary>
        /// 舒适度 , 优于区域百分比
        /// </summary>
        public double ComfortPercent { get; set; }
        /// <summary>
        /// 便利性 , 优于区域百分比
        /// </summary>
        public double ConveniencePercent { get; set; }
        /// <summary>
        /// 安全性 , 优于区域百分比
        /// </summary>
        public double SecurityPercent { get; set; }
        /// <summary>
        /// 文化建设 , 优于区域百分比
        /// </summary>
        public double CulturalPercent { get; set; }
        /// <summary>
        /// 热度口碑
        /// </summary>
        public double Heat { get; set; }
        /// <summary>
        /// 热度口碑 , 区域均值
        /// </summary>
        public double HeatArea { get; set; }
        /// <summary>
        /// 热度口碑 , 优于区域百分比
        /// </summary>
        public double HeatPercent { get; set; }
        /// <summary>
        /// 学校热度 , 区域均值
        /// </summary>
        public double SchoolHeatArea { get; set; }
        /// <summary>
        /// 学校热度 , 优于区域百分比
        /// </summary>
        public double SchoolHeatPercent { get; set; }
    }
    public class TalentInfo
    {
        public string HeadImgUrl { get; set; }
        public string Nickname { get; set; }
        public int OrgType { get; set; }
        public string AuthName { get; set; }
        public Guid TalentUserID { get; set; }
    }
}
