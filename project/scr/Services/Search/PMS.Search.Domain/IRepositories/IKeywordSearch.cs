using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Domain.IRepositories
{
    public interface IKeywordSearch
    {
        Task<PaginationModel<SearchKeywordHighlight>> SearchAsync(SearchKeywordQueryModel queryModel);
    }
}
