using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Comment
{
    /// <summary>
    /// 标签
    /// </summary>
    public class CommentTagViewModel
    {
        /// <summary>
        /// 标签Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 标签名
        /// </summary>
        public string TagName { get; set; }
    }
}
