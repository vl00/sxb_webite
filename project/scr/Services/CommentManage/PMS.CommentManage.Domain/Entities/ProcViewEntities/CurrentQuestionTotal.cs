using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    /// <summary>
    /// 学校问题列表统计
    /// </summary>
    public class CurrentQuestionTotal
    {
        /// <summary>
        /// 总问题数
        /// </summary>
        public int QuestionTotal { get; set; }
        /// <summary>
        /// 辟谣问题
        /// </summary>
        public int RumorreFuting { get; set; }
        /// <summary>
        /// 师资力量问题数
        /// </summary>
        /// <value>The school score.</value>
        public int TeachTotal { get; set; }
        /// <summary>
        /// 硬件设施问题数
        /// </summary>
        /// <value>The school score.</value>
        public int HardTotal { get; set; }
        /// <summary>
        /// 环境周边问题数
        /// </summary>
        /// <value>The school score.</value>
        public int EnvirTotal { get; set; }
        /// <summary>
        /// 学风管理问题数
        /// </summary>
        /// <value>The school score.</value>
        public int ManageTotal { get; set; }
        /// <summary>
        /// 校园生活问题数
        /// </summary>
        /// <value>The school score.</value>
        public int LifeTotal { get; set; }
        /// <summary>
        /// 国际部问题
        /// </summary>
        public int InternationalTotal { get; set; }
        /// <summary>
        /// 高中部问题数
        /// </summary>
        public int HighSchoolTotal { get; set; }
    }
}
