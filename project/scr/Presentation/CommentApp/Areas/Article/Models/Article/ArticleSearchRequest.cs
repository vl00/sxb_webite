using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Domain.Enums;
using PMS.Search.Domain.Common;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Models.Article
{
    public class ArticleSearchRequest : PaginationReqDto
    {

        //(string keyword, Guid? userId, int? provinceId, int? cityId, List<int> areaId, List<int> articleTypes, List<string> schTypes, int pageIndex = 1, int pageSize = 10)
        /// <summary>
        /// 市区ID
        /// </summary>
        public int CityCode { get; set; }
        public string Keyword { get; set; }

        /// <summary>
        /// 城区
        /// </summary>
        public List<int> AreaCodes { get; set; } = new List<int>();

        /// <summary>
        /// 学段 1幼儿园 2小学 3初中 4高中
        /// </summary>
        public List<int> SchoolGradeIds { get; set; } = new List<int>();

        /// <summary>
        /// 学校类型 lx210 lx310
        /// </summary>
        public List<string> SchoolTypes { get; set; } = new List<string>();

        /// <summary>
        /// 文章类型
        /// </summary>
        public List<ArticleType> ArticleTypes { get; set; } = new List<ArticleType>();

        /// <summary>
        /// 排序
        /// </summary>
        public ArticleOrderBy OrderBy { get; set; }
        public ClientType ClientType { get; set; }
    }
}
