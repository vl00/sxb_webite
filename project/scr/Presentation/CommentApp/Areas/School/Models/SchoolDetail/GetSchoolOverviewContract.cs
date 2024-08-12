using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.WechatDemo;
using Sxb.Web.Models.Question;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.SchoolDetail
{
    public class GetSchoolOverviewResponse
    {
        public IEnumerable<string> AuthenticationList { get; set; }
        /// <summary>
        /// 开设的项目与课程
        /// </summary>
        public IEnumerable<OnlineSchoolProjectInfo> Projects { get; set; }
        /// <summary>
        /// 课程设置
        /// </summary>
        public string Courses { get; set; }
        public string CourseAuthentication { get; set; }
        public string CourseCharacteristic { get; set; }
        /// <summary>
        /// 学生数量
        /// </summary>
        public int StudentQuantity { get; set; }
        /// <summary>
        /// 师生比
        /// </summary>
        public string TeacherStudentScale { get; set; }
        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool? HasSchoolBus { get; set; }
        /// <summary>
        /// 是否有饭堂
        /// </summary>
        public bool? HasCanteen { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        public string Abroad { get; set; }
        /// <summary>
        /// 学校地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 官网
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Intro { get; set; }
    }
}
