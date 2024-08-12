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
    public interface ISchoolSearch
    {
        Task<PaginationModel<Guid>> RecommenAsync(string keyword, int cityId, List<int> areaIds, List<int> grades, List<string> schoolTypeCodes, int? minTotal, int? maxTotal, List<Guid> authIds, List<Guid> courseIds, List<Guid> characIds, int pageIndex = 1, int pageSize = 10);
        Task<List<SearchSchoolExplanationQueryModel>> RecommenExplainAsync(string keyword, int cityId, List<int> areaIds, List<int> grades, List<string> schoolTypeCodes, int? minTotal, int? maxTotal, List<Guid> authIds, List<Guid> courseIds, List<Guid> characIds, int pageIndex = 1, int pageSize = 10);
        Task<PaginationModel<Guid>> SearchSchools(SearchBaseQueryModel queryModel);
    }
}
