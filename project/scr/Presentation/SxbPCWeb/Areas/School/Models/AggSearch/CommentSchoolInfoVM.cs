using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Areas.School.Models
{

    public class CommentSchoolInfoVM
    {
        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 分部所在学校的评论数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 点亮
        /// </summary>
        public int SchoolStars { get; set; }
    }
}
