using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Search.Application.Services
{
    public class KeywordSearchService : IKeywordSearchService
    {
        private readonly IKeywordSearch _keywordSearch;

        public KeywordSearchService(IKeywordSearch keywordSearch)
        {
            _keywordSearch = keywordSearch;
        }

        public async Task<PaginationModel<SearchKeywordHighlight>> SearchAsync(SearchKeywordQueryModel queryModel)
        {
            var pagination = await _keywordSearch.SearchAsync(queryModel);
            return pagination;
        }
    }
}
