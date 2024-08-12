using PMS.School.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Entities
{
    /// <summary>
    /// 分部
    /// </summary>
    public class SchoolBranch
    {
        /// <summary>
        /// 分部Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 分部名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 城市标识码
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool Sdextern { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }
    }

    /// <summary>
    /// 学校纠错 学校信息
    /// </summary>
    public class SchoolFeedback 
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string SchFType { get; set; }
        public int Grade { get; set; }
        public int Type { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
    }
}

