using NPOI.SS.Formula.PTG;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Application.Services
{
    public class ArticleSearchService : IArticleSearchService
    {
        private readonly IArticleSearch _articleSearch;

        public ArticleSearchService(IArticleSearch articleSearch)
        {
            _articleSearch = articleSearch;
        }

        public async Task<PaginationModel<SearchArticleDto>> SearchAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaIds, List<int> articleTypes, List<string> schTypes, bool? isTop
            , ArticleOrderBy orderBy, int pageIndex = 1, int pageSize = 10)
        {
            var pagination = await _articleSearch.SearchAsync(keyword, userId, provinceId, cityId, areaIds, articleTypes, schTypes, isTop, orderBy, pageIndex, pageSize);

            return PaginationModel.MapperTo<SearchArticle, SearchArticleDto>(pagination);
        }

        public async Task<long> SearchTotalAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaIds, List<int> articleTypes, List<string> schTypes, bool? isTop)
        {
            return await _articleSearch.SearchTotalAsync(keyword, userId, provinceId, cityId, areaIds, articleTypes, schTypes, isTop);
        }
    }
}
