using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Tool.HtmlAnalysis.Helper
{
    [Flags]
    public enum AnalysisType
    {
        None,
        /// <summary>
        /// 标题
        /// </summary>
        Title = 1,
        /// <summary>
        /// 标题图片
        /// </summary>
        TitleImage = 2,
        /// <summary>
        /// 正文图片
        /// </summary>
        Images = 4,
        All = Title | TitleImage | Images
    }
}
