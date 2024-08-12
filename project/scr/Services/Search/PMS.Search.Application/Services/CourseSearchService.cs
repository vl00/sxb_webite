using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto;
using PMS.Search.Application.ModelDto.Talent;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Application.Services
{
    public class CourseSearchService : ICourseSearchService
    {
        private readonly ICourseSearch _courseSearch;

        public CourseSearchService(ICourseSearch courseSearch)
        {
            _courseSearch = courseSearch;
        }

        public async Task<PaginationModel<SearchCourseDto>> SearchCourses(SearchCourseQueryModel queryModel)
        {
            var pagination = await _courseSearch.SearchCourses(queryModel);
            var data = CommonHelper.MapperProperty<SearchCourse, SearchCourseDto>(pagination.Data).ToList();
            return PaginationModel.Build(data, pagination.Total);
        }


        public async Task<List<SearchShortIdSuggestItem>> ListSuggest(string keyword, int pageSize)
        {
            var data = await _courseSearch.ListSuggest(keyword, pageSize);
            return data;
        }
    }
}
