using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.School
{
    public class CommentSchoolScore
    {
        /// <summary>
        /// 总分
        /// </summary>
        /// <value>The school score.</value>
        public decimal AggScore { get; set; }
        public int StarScore { get; set; }
        /// <summary>
        /// 师资力量分
        /// </summary>
        /// <value>The school score.</value>
        public int TeachScore { get; set; }
        /// <summary>
        /// 硬件设施分
        /// </summary>
        /// <value>The school score.</value>
        public int HardScore { get; set; }
        /// <summary>
        /// 环境周边分
        /// </summary>
        /// <value>The school score.</value>
        public int EnvirScore { get; set; }
        /// <summary>
        /// 学风管理分
        /// </summary>
        /// <value>The school score.</value>
        public int ManageScore { get; set; }
        /// <summary>
        /// 校园生活分
        /// </summary>
        /// <value>The school score.</value>
        public int LifeScore { get; set; }
    }
}
