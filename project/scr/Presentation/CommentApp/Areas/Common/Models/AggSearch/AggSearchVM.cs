using DnsClient;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using PMS.UserManage.Application.ModelDto.ModelVo;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models
{
    public class AggSearchVM
    {
        /// <summary>
        /// 关键词百科内容
        /// </summary>
        public WikiDto wiki { get; set; }

        /// <summary>
        /// 匹配到的结果数量
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 是否是垃圾请求keyword
        /// </summary>
        public bool IsGarbage { get; set; }

        /// <summary>
        /// 大图广告的二维码ExtId
        /// </summary>
        public Guid? BigBannerExtId { get; set; }

        /// <summary>
        /// 查询学校
        /// </summary>
        public List<SearchSchoolVM> SearchSchools { get; set; }

        /// <summary>
        /// 推荐学校
        /// </summary>
        public List<SearchSchoolVM> RecommendSchools { get; set; }


        public List<SearchArticleVM> SearchArticles { get; set; }
        public List<SearchArticleVM> RecommendArticles { get; set; }

        public List<SearchCommentVM> SearchComments { get; set; }
        public List<SearchCommentVM> RecommendComments { get; set; }


        public List<SearchQuestionVM> SearchQuestions { get; set; }
        public List<SearchQuestionVM> RecommendQuestions { get; set; }


        public class WikiDto
        {
            public string Name { get; set; }
            public string Content { get; set; }
        }
    }
}
