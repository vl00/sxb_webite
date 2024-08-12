using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class CircleEditRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string CoverUrl { get; set; }

        [Required]
        public string Intro { get; set; }

        [Description("背景颜色")]
        public string BGColor { get; set; }
    }
}
