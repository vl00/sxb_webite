using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Article
{
    public class ArticleViewModel
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
        /// 发表时间(中文)
        /// </summary>
        public string CNTime
        {
            get
            {
                if (DateTime.TryParse(Time, out DateTime convertResult))
                {
                    return convertResult.ToString("yyyy年MM月dd日");
                }
                return Time;
            }
        }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViweCount { get; set; }


        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }




        /// <summary>
        /// 文章内容 
        /// </summary>
        public string Html { get; set; }

        public string Author { get; set; }

        public string Digest { get; set; }

        /// <summary>
        /// 相关联的点评
        /// </summary>
        public List<CommentInfoViewModel> CorrelationSCMs { get; set; }

        /// <summary>
        /// 相关的群组二维码
        /// </summary>
        public List<GroupQRCode> CorrelationGQRCode { get; set; }

        /// <summary>
        /// 相关学校分部信息
        /// </summary>
        public IEnumerable<Sxb.PCWeb.ViewModels.School.SchoolExtListItemViewModel> CorrelationSchoolExt { get; set; }



        /// <summary>
        /// 关联的标签
        /// </summary>
        public List<TagDto> CorrelationTags { get; set; }



        /// <summary>
        /// 相关文章
        /// </summary>
        public IEnumerable<ArticleListItemViewModel> CorrelationArticle { get; set; }



        /// <summary>
        /// 文章关联的第一所学校分部的学校旗下的所有分部
        /// </summary>
        public List<OnlineSchoolExtension> FirstSchoolBranchs { get; set; }




    }
}
