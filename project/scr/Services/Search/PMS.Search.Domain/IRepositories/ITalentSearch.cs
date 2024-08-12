using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.IRepositories
{
    public interface ITalentSearch
    {
        PaginationModel<SearchTalent> SearchPagination(SearchBaseQueryModel queryModel);
        List<SearchTalent> SearchTalents(SearchBaseQueryModel queryModel);
    }
}
