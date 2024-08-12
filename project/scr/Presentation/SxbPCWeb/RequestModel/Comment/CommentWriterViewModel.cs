using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.RequestModel.Comment
{
    /// <summary>
    /// 点评提交
    /// </summary>
    public class CommentWriterViewModel
    {
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid Sid { get; set; }

        /// <summary>
        /// 分部id
        /// </summary>
        public Guid Eid { get; set; }
        /// <summary>
        /// 校方辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 总分
        /// </summary>
        /// <value>The school score.</value>
        public decimal AggScore { get; set; }
        /// <summary>
        /// 师资力量分
        /// </summary>
        /// <value>The school score.</value>
        public decimal TeachScore { get; set; }
        /// <summary>
        /// 硬件设施分
        /// </summary>
        /// <value>The school score.</value>
        public decimal HardScore { get; set; }
        /// <summary>
        /// 环境周边分
        /// </summary>
        /// <value>The school score.</value>
        public decimal EnvirScore { get; set; }
        /// <summary>
        /// 学风管理分
        /// </summary>
        /// <value>The school score.</value>
        public decimal ManageScore { get; set; }
        /// <summary>
        /// 校园生活分
        /// </summary>
        /// <value>The school score.</value>
        public decimal LifeScore { get; set; }
    }
}
