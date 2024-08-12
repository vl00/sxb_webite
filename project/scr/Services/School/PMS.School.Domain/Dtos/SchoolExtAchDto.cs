using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtAchDto
    {

    }

    public class KindergartenAchievement
    {

        /// <summary>
        /// 
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid ExtId { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public int Year { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public string Link { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Createor { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime ModifyDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Modifier { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public bool IsValid { get; set; } = true;


    }

    public class MiddleSchoolAchievement
    {

        /// <summary>
        /// 
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid ExtId { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public int Year { get; set; }

        /// <summary>
        /// 重点率
        /// </summary> 
        public double? Keyrate { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public double? Average { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public double? Highest { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public double? Ratio { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Creator { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime ModifyDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Modifier { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public bool IsValid { get; set; } = true;


    }

    public class PrimarySchoolAchievement
    {

        /// <summary>
        /// 
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid ExtId { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public int Year { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public string Link { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Createor { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime ModifyDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Modifier { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public bool IsValid { get; set; } = true;


    }

    public class HighSchoolAchievement
    {

        /// <summary>
        /// 
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid ExtId { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public int Year { get; set; }

        /// <summary>
        /// 重本率
        /// </summary> 
        public double? Keyundergraduate { get; set; }

        /// <summary>
        /// 本科率
        /// </summary> 
        public double? Undergraduate { get; set; }

        /// <summary>
        /// 高优录取人数
        /// </summary> 
        public int? Count { get; set; }

        /// <summary>
        /// Name Time
        /// </summary> 
        public string Fractionaline { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Creator { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime ModifyDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Modifier { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public bool IsValid { get; set; } = true;

    }

}
