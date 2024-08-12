using PMS.UserManage.Application.ModelDto.ModelVo;
using Sxb.UserCenter.Models.ArticleViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using PMS.OperationPlateform.Domain.Entitys;
using System.Text.RegularExpressions;
using PMS.OperationPlateform.Domain.Enums;

namespace Sxb.UserCenter.Utils.DtoToViewModel
{
    public static class ArticleDataToVoHelper
    {
        public static List<DataViewModel> ArticleDataToViewModelHelper(List<Data> datas) 
        {
            List<DataViewModel> dataViews = new List<DataViewModel>();

            if (!datas.Any()) 
            {
                return dataViews;
            }

            foreach (var item in datas)
            {
                dataViews.Add(new DataViewModel() {
                    id = item.id,
                    time = item.time.ConciseTime(),
                    isShow = item.isShow,
                    covers = item.covers,
                    digest = item.digest,
                    layout = item.layout,
                    title =item.title,
                    viewCount = item.viewCount
                });
            }

            return dataViews;
        }


        public static List<DataViewModel> ArticleToViewModelHelper(List<article> datas)
        {
            List<DataViewModel> dataViews = new List<DataViewModel>();

            if (!datas.Any())
            {
                return dataViews;
            }

            foreach (var item in datas)
            {
                dataViews.Add(new DataViewModel()
                {
                    id = item.id,
                    time = item.time == null ? default : DateTime.Parse(item.time.ToString()).ConciseTime(),
                    isShow = item.show,
                    layout = item.layout,
                    covers = (item.Covers ?? new List<article_cover>()).Select(s => ArticleCoverLink(s.articleID?.ToString(), s.photoID?.ToString(),
                                        ((FileExtension)s.ext).ToString())).ToArray(),
                    title = item.title,
                    viewCount = (item.viewCount ?? 0) + (item.viewCount_r ?? 0),
                    content = RegexArticle(item.html)
                }); 
            }

            return dataViews;
        }

        public static string RegexArticle(string html)
        {
            if (html == null)
                return "";

            Regex regex = new Regex("<p.+?</p>");
            string content = "";
            foreach (var item in regex.Matches(html))
            {
                content+= Regex.Replace(item.ToString(), "<.+?>", "");
            }
            byte contentMaxLength = 60;
            content = content?.Length > contentMaxLength ? content.Substring(0, 60) : content;
            return content;
        }

        /// <summary>
        /// 文章背景图片链接生成
        /// </summary>
        public static string ArticleCoverLink(string articleId, string photoId, string fileExtention)
        {
            return $"https://cos.sxkid.com/images/article/{articleId}/{photoId}.{fileExtention}";

        }
    }
}
