using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class ArticleBaseDetailDto
    {
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViweCount { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 背景图
        /// </summary>
        public List<string> Covers { get; set; }

        /// <summary>
        /// 标识序号的base32编码
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Digest { get; set; }

        public int Layout { get; set; }

        public static implicit operator ArticleBaseDetailDto(article article)
        {
            if (article == null) return null;
            return new ArticleBaseDetailDto()
            {
                Id = article.id,
                Time = article.time.GetValueOrDefault().ToArticleFormattString(),
                Title = article.title,
                ViweCount = article.VirualViewCount,
                Covers = article.Covers?.Select(c => $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToList(),
                No = UrlShortIdUtil.Long2Base32(article.No),
                Layout = article.layout,
                Author = article.author,

            };
        }


    }
}
