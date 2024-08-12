using PMS.School.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Entities
{
    /// <summary>
    /// 学校详情
    /// </summary>
    public class SchoolExtension
    {
        /// <summary>
        /// 学校分部id，每个学校必定对应一间分部
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学校总类型
        /// </summary>
        public string SchFtype { get; set; }
        /// <summary>
        /// 学校年级
        /// </summary>
        public SchoolGrade SchoolGrade { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool Sdextern { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        public decimal Tuition { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 城市code
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// 学校代码
        /// </summary>
        public int SchoolNo { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool HasSchoolBus { get; set; }
    }

    /// <summary>
    /// 学校分部点评统计
    /// </summary>
    public class SchoolExtensionTotal : SchoolExtension
    {
        /// <summary>
        /// 分部总点评
        /// </summary>
        public int SectionCommentTotal { get; set; }
        /// <summary>
        /// 分部总提问数
        /// </summary>
        public int SectionQuestionTotal { get; set; }
        /// <summary>
        /// 学校点评总数
        /// </summary>
        public int CommentTotal { get; set; }
        /// <summary>
        /// 学校平均分
        /// </summary>
        public decimal SchoolAvgScore { get; set; }
    }


    /// <summary>
    /// 获取学校状态信息
    /// </summary>
    public class SchoolInfoStatus : SchoolExtension
    {
        public bool IsValid { get; set; }

        public int Status { get; set; }
    }

}
