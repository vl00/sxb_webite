using PMS.Search.Application.ModelDto;
using PMS.Search.Application.ModelDto.Talent;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Application.IServices
{
    public interface ICourseSearchService
    {
        Task<List<SearchShortIdSuggestItem>> ListSuggest(string keyword, int pageSize);
        Task<PaginationModel<SearchCourseDto>> SearchCourses(SearchCourseQueryModel queryModel);
    }
}
