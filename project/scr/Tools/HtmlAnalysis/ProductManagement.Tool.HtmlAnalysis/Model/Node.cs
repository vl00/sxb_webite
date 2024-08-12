using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.HtmlAnalysis.Model
{
    public class Node
    {
        //public List<Node> Children { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// html
        /// </summary>
        public string OuterHtml { get; set; }

        /// <summary>
        /// 自闭和标签(单标签/双标签)
        /// <img />
        /// </summary>
        public bool IsAutoClose { get; set; }

        /// <summary>
        /// 属性列表
        /// </summary>
        public List<Attr> Attrs { get; set; }
    }
}
