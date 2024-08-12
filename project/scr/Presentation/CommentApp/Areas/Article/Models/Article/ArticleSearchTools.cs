using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Domain.Enums;
using Sxb.Web.Areas.Common.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Models.Article
{
    public class ArticleSearchTools
    {
        /// <summary>
        /// 区域 - 城区
        /// </summary>
        public List<AreaDto> Areas { get; set; }

        /// <summary>
        /// 学段
        /// </summary>
        public List<SchoolGradeDetailVM> SchoolGrades { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        //public List<(string name, int value)> ArticleTypes { get; set; }
        public List<ArticleTypeVM> ArticleTypes { get; set; }

        public class ArticleTypeVM
        {
            public string Name { get; set; }

            public int Value {  get; set; }
        }
    }
}
