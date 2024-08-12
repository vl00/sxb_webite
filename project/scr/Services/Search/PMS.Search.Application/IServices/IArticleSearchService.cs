using PMS.Search.Application.ModelDto;
using PMS.Search.Application.ModelDto.Talent;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Application.IServices
{
    public interface IArticleSearchService
    {
        Task<PaginationModel<SearchArticleDto>> SearchAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaId, List<int> articleTypes, List<string> schTypes, bool? isTop
            , ArticleOrderBy orderBy, int pageIndex = 1, int pageSize = 10);
        Task<long> SearchTotalAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaId, List<int> articleTypes, List<string> schTypes, bool? isTop);
    }
}
