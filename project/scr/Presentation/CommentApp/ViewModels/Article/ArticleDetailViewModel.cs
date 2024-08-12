using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Article
{
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.DTOs;
    using Sxb.Web.ViewModels.School;

    /// <summary>
    /// 文章详情的视图模型
    /// </summary>
    public class ArticleDetailViewModel : ArticleViewModel
    {
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Html { get; set; }

        public string Author { get; set; }

        public string Digest { get; set; }

        public int No { get; set; }

        /// <summary>
        /// 相关联的点评
        /// </summary>
        public List<SCMDto> CorrelationSCMs { get; set; }

        /// <summary>
        /// 相关的群组二维码
        /// </summary>
        public List<GroupQRCode> CorrelationGQRCode { get; set; }

        /// <summary>
        /// 相关学校分部信息
        /// </summary>
        public IEnumerable<SchoolExtListItemViewModel> CorrelationSchoolExt { get; set; }

        /// <summary>
        /// 关联的标签
        /// </summary>
        public List<TagDto> CorrelationTags { get; set; }

        /// <summary>
        /// 相关文章
        /// </summary>
        public List<ArticleListItemViewModel> CorrelationArticle { get; set; }

        /// <summary>
        /// 文章关联的第一所学校分部的学校旗下的所有分部
        /// </summary>
        public List<OnlineSchoolExtension> FirstSchoolBranchs { get; set; }
    }
}