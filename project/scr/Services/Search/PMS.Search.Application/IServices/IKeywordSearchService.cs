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
    public interface IKeywordSearchService
    {
        Task<PaginationModel<SearchKeywordHighlight>> SearchAsync(SearchKeywordQueryModel queryModel);
    }
}
