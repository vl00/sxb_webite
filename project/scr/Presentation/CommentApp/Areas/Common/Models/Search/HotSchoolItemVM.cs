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
    public class HotSchoolItemVM
    {
        public Guid ExtId { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }
        [Obsolete]
        public string ShortId => ShortNo;

        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string SchoolName { get; set; }
        [Obsolete]
        public string Name => SchoolName;

        /// <summary>
        /// 评级
        /// </summary>
        public string Str_Score { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchoolTypeName { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public string LodgingTypeName { get; set; }

        /// <summary>
        /// 格式化学费
        /// </summary>
        public string TuitionPerYearFee { get; set; }


        public static HotSchoolItemVM Convert(SchoolExtFilterDto s)
        {
            return new HotSchoolItemVM()
            {
                ExtId = s.ExtId,
                ShortNo = s.ShortId,
                SchoolName = s.Name,
                Str_Score = string.IsNullOrWhiteSpace(s.Str_Score) ? "暂未评分" : s.Str_Score,
                SchoolTypeName = s.SchoolTypeName,
                LodgingTypeName = s.LodgingTypeName,
                TuitionPerYearFee = s.TuitionPerYearFee,
            };
        }

    }
}
