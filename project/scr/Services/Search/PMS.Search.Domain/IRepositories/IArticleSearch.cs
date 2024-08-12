using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Domain.IRepositories
{
    public interface IArticleSearch
    {
        List<SearchArticle> SearchArticle(out long total, SearchArticleQueryModel queryModel);

        Task<PaginationModel<SearchArticle>> SearchAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaId, List<int> articleTypes, List<string> schTypes, bool? isTop
            , ArticleOrderBy orderBy, int pageIndex = 1, int pageSize = 10);

        Task<long> SearchTotalAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaId, List<int> articleTypes, List<string> schTypes, bool? isTop);
    }
}
