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

namespace PMS.Search.Application.Services
{
    public class EvaluationSearchService : IEvaluationSearchService
    {
        private readonly IEvaluationSearch _evaluationSearch;

        public EvaluationSearchService(IEvaluationSearch evaluationSearch)
        {
            _evaluationSearch = evaluationSearch;
        }

        public PaginationModel<SearchEvaluationDto> SearchEvaluations(SearchEvaluationQueryModel queryModel)
        {
            var pagination = _evaluationSearch.SearchEvaluations(queryModel);
            var data = CommonHelper.MapperProperty<SearchEvaluation, SearchEvaluationDto>(pagination.Data).ToList();
            return PaginationModel.Build(data, pagination.Total);
        }
    }
}
