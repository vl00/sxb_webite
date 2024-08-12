using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class CircleCreateRequest
    {
        [Required(ErrorMessage = "请输入圈子名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请上传封面图")]
        public string CoverUrl { get; set; }

        [Required(ErrorMessage = "请输入圈子简介")]
        public string Intro { get; set; }

        [Description("背景颜色")]
        public string BGColor { get; set; }
    }
}
