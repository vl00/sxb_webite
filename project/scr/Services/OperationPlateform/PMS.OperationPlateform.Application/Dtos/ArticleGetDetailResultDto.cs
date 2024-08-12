using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class ArticleGetDetailResultDto:ArticleBaseDetailDto
    {
        /// <summary>
        /// 关联的主标签
        /// </summary>
        public List<string> MainTags { get; set; }

        /// <summary>
        /// 底部二维码
        /// </summary>
        public List<string> QRCodes { get; set; }


        /// <summary>
        /// 关联的文章
        /// </summary>
        public List<ArticleBaseDetailDto> CorrelationArticles { get; set; }

        public static implicit operator ArticleGetDetailResultDto(article article)
        {
            if (article == null) { return null; }
            return new ArticleGetDetailResultDto()
            {
                Id = article.id,
                Time = article.time.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                Html = article.html,
                Title = article.title,
                ViweCount = article.VirualViewCount,
                Author = article.author.Contains("上学帮") ? "上学帮" : article.author,
                Digest = article.html.GetHtmlHeaderString(150),
                No = UrlShortIdUtil.Long2Base32(article.No),
                Layout = article.layout
            };
        }
    }
}
