using System;
using System.ComponentModel.DataAnnotations;

namespace Sxb.Web.Areas.SignUp.Models
{
    public class HurunSignUpData
    {
        /// <summary>
        /// 名字
        /// </summary>
        [Display(Name = "名字")]
        [Required(ErrorMessage = "{0}不能为空")]
        [StringLength(200, ErrorMessage = "{0}长度错误")]
        public string Name { get; set; }
        /// <summary>
        /// 工作单位
        /// </summary>
        [Display(Name = "工作单位")]
        [Required(ErrorMessage = "{0}不能为空")]
        [StringLength(200, ErrorMessage = "{0}长度错误")]
        public string Organization { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [Display(Name = "职位")]
        [Required(ErrorMessage = "{0}不能为空")]
        [StringLength(200, ErrorMessage = "{0}长度错误")]
        public string Job { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        [Display(Name = "联系方式")]
        [Required(ErrorMessage = "{0}不能为空")]
        [StringLength(200, ErrorMessage = "{0}长度错误")]
        public string Contact { get; set; }
        /// <summary>
        /// 工作年限
        /// </summary>
        [Display(Name = "工作年限")]
        [Required(ErrorMessage = "{0}不能为空")]
        [Range(1, 100, ErrorMessage = "{0}错误")]
        public int WorkingYears { get; set; }
        /// <summary>
        /// 问题三
        /// </summary>
        [Display(Name = "问题三")]
        [StringLength(200, ErrorMessage = "{0}长度错误")]
        public string Question3 { get; set; }
        /// <summary>
        /// 问题四
        /// </summary>
        [Display(Name = "问题四")]
        [StringLength(200, ErrorMessage = "{0}长度错误")]
        public string Question4 { get; set; }
        /// <summary>
        /// 华东地区学校
        /// </summary>
        [Display(Name = "华东地区学校")]
        public string[] EastSchools { get; set; }
        /// <summary>
        /// 华南地区学校
        /// </summary>
        [Display(Name = "华南地区学校")]
        public string[] SouthSchools { get; set; }
        /// <summary>
        /// 北方地域学校
        /// </summary>
        [Display(Name = "北方地域学校")]
        public string[] NorthernSchools { get; set; }
        /// <summary>
        /// 西部及其余地区学校
        /// </summary>
        [Display(Name = "西部及其余地区学校")]
        public string[] WesternSchools { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        [Display(Name = "城市名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CityName { get; set; }
    }
}
