using PMS.School.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models.Search
{
    /// <summary>
    /// 列表中的学校vm
    /// </summary>
    public class SchoolItemVM
    {
        public Guid ExtId { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 评级
        /// </summary>
        public string Str_Score { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public string LodgingTypeName { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }
        public int CityId{ get; set; }

        /// <summary>
        /// 城区名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 格式化学费
        /// </summary>
        public string TuitionPerYear { get; set; }

        /// <summary>
        /// 学生人数
        /// </summary>
        public int? StudentCount { get; set; }

        /// <summary>
        /// 教师人数
        /// </summary>
        public int? TeacherCount { get; set; }

        /// <summary>
        /// 建校年份
        /// </summary>
        public int? CreationYear { get; set; }

        /// <summary>
        /// taglist
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 学校类型(总)
        /// </summary>
        public string SchFTypeDesc { get; set; }
        /// <summary>
        /// 学校类型 lx120  lx123
        /// </summary>
        public string SchFType { get; set; }


        public static SchoolItemVM Convert(SchoolExtFilterDto s)
        {
            return new SchoolItemVM()
            {
                ExtId = s.ExtId,
                ShortNo = s.ShortId,
                SchoolName = s.Name,
                Str_Score = string.IsNullOrWhiteSpace(s.Str_Score) ? "暂未评分" : s.Str_Score,
                LodgingTypeName = s.LodgingTypeName,
                CityName = s.City,
                CityId = s.CityCode,
                AreaName = s.Area,
                TuitionPerYear = s.TuitionPerYearFee,
                StudentCount = s.StudentCount,
                TeacherCount = s.TeacherCount,
                CreationYear = s.CreationDate?.Year,
                Tags  = s.Tags,
                SchFType = s.SchFType0.ToString(),
                SchFTypeDesc = s.SchFType0.GetDesc()
            };
        }

    }
}
