using PMS.Search.Application.ModelDto;
using PMS.Search.Application.ModelDto.Talent;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Application.IServices
{
    public interface IEvaluationSearchService
    {
        PaginationModel<SearchEvaluationDto> SearchEvaluations(SearchEvaluationQueryModel queryModel);
    }
}
