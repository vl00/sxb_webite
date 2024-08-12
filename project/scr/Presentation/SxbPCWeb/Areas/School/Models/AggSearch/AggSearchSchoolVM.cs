using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Areas.School.Models
{

    public class AggSearchSchoolVM
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

        /// <summary>
        /// 城区名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 格式化学费
        /// </summary>
        public string TuitionPerYear { get; set; }

        /// <summary>
        /// 学校简介
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 推荐达人
        /// </summary>
        public Guid? TalentUser { get; set; }

        /// <summary>
        /// 年纪
        /// </summary>
        public int Grade { get; set; }

        public static AggSearchSchoolVM Convert(SchoolExtFilterDto s)
        {
            return new AggSearchSchoolVM()
            {
                ExtId = s.ExtId,
                ShortNo = s.ShortId,
                SchoolName = s.Name,
                Str_Score = s.Str_Score,
                LodgingTypeName = s.LodgingTypeName,
                CityName = s.City,
                AreaName = s.Area,
                TuitionPerYear = s.TuitionPerYearFee,
                Intro = s.Intro,//s.ExtIntro ??
                Grade = s.Grade,
                TalentUser = null
            };
        }

        internal AggSearchSchoolVM SetTalentUser(IEnumerable<(int Grade, Guid UserId)> gradeUserIds)
        {
            var users = gradeUserIds.Where(s => s.Grade == Grade);
            if (users.Any())
            {
                TalentUser = users.FirstOrDefault().UserId;
            }
            return this;
        }
    }
}
