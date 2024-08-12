using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models
{

    public class SearchSchoolVM
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


        /// <summary>
        /// <see cref="iSchool.SchFType0"/>
        /// <see cref="SchTypeUtils"/>
        /// </summary>
        public string SchFType0Desc { get; set; }
        public string SchFTypeCode { get; set; }

        public static SearchSchoolVM Convert(SchoolExtFilterDto s)
        {
            return new SearchSchoolVM()
            {
                ExtId = s.ExtId,
                ShortNo = s.ShortId,
                SchoolName = s.Name,
                Str_Score = string.IsNullOrWhiteSpace(s.Str_Score) ? "暂未评分" : s.Str_Score,
                LodgingTypeName = s.LodgingTypeName,
                CityName = s.City,
                AreaName = s.Area,
                TuitionPerYear = s.TuitionPerYearFee,
                Intro = s.Intro ?? "暂未收录",//s.ExtIntro ??
                Grade = s.Grade,
                SchFType0Desc = s.SchFType0.GetDesc(),
                TalentUser = null,
                SchFTypeCode = s.SchFType0.Code
            };
        }

        internal SearchSchoolVM SetTalentUser(IEnumerable<(int Grade, Guid UserId)> gradeUserIds)
        {
            //非广州不显示推荐达人
            if (!CityName.Contains("广州"))
            {
                return this;
            }

            var users = gradeUserIds.Where(s => s.Grade == Grade);
            if (users.Any())
            {
                TalentUser = users.FirstOrDefault().UserId;
            }
            return this;
        }
    }
}
