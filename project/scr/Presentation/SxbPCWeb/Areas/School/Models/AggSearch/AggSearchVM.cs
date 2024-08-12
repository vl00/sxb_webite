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

namespace Sxb.PCWeb.Areas.School.Models
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
        /// 查询学校
        /// </summary>
        public List<AggSearchSchoolVM> SearchSchools { get; set; }

        /// <summary>
        /// 推荐学校
        /// </summary>
        public List<AggSearchSchoolVM> RecommendSchools { get; set; }


        public List<AggSearchArticleVM> SearchArticles { get; set; }
        public List<AggSearchArticleVM> RecommendArticles { get; set; }

        public List<AggSearchCommentVM> SearchComments { get; set; }
        public List<AggSearchCommentVM> RecommendComments { get; set; }


        public List<AggSearchQuestionVM> SearchQuestions { get; set; }
        public List<AggSearchQuestionVM> RecommendQuestions { get; set; }


        public class WikiDto
        {
            public string Name { get; set; }
            public string Content { get; set; }
        }
    }
}
