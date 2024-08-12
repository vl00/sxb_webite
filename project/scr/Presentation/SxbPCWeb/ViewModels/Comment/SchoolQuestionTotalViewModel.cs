using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Comment
{
    /// <summary>
    /// 学校问答标签
    /// </summary>
    public class SchoolQuestionTotalViewModel
    {
        /// <summary>
        /// 类型标签key值【给其他分部学校type加自定义的标签值】
        /// </summary>
        public int TotalTypeNumber { get; set; }
        /// <summary>
        /// 统计类型
        /// </summary>
        public string TotalType { get; set; }
        /// <summary>
        /// 统计总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string Name { get; set; }
    }
}
