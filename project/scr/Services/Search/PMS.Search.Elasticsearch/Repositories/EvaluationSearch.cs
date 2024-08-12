using Microsoft.Extensions.Options;
using Nest;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Framework.SearchAccessor;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.Search.Elasticsearch.Repositories
{
    public class EvaluationSearch : IEvaluationSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _evaluationIndex;
        public EvaluationSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _evaluationIndex = _esConfig.Indices.Evaluation;
            }
        }

        public PaginationModel<SearchEvaluation> SearchEvaluations(SearchEvaluationQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var orderBy = queryModel.OrderBy;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            var search = new SearchDescriptor<SearchEvaluation>()
              .Index(_evaluationIndex.SearchIndex)
              .From(from)
              .Size(size)
              .Sort(SearchPUV.GetSortDesc<SearchEvaluation>(orderBy, "modifyDateTime"))
              ;
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchEvaluation>, QueryContainer>>
            {
                q => q.Term(t => t.Field("title").Value(keyword).Boost(10))
            };

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchEvaluation>, QueryContainer>>
            {
                m => m.Term(t => t.Field(f => f.IsValid).Value(1)),
                m => m.Term(t => t.Field(f => f.Status).Value(1))
            };

            if (!queryModel.SearchTitleOnly)
            {
                if (queryModel.QueryEsKeywords.Any())
                {
                    var innerShoulds = NestHelper.KeywordsToNest<SearchEvaluation>(
                        queryModel.QueryEsKeywords,
                        queryModel.IsMatchAllWord,
                        Infer.Fields("title.pinyin", "title.keyword", "courseName.pinyin", "courseName.keyword", "orgName.pinyin", "orgName.keyword"),
                        Infer.Fields("title.cntext", "courseName.cntext", "orgName.cntext"));
                    mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
                }

                shouldQuerys.Add(q => q.Term(t => t.Field("courseName").Value(keyword).Boost(10)));
                shouldQuerys.Add(q => q.Term(t => t.Field("orgName").Value(keyword).Boost(10)));
                //shouldQuerys.Add(q => q.Match(t => t.Field("orgName.keyword").Query(keyword).Boost(5)));
            }
            else
            {
                if (queryModel.QueryEsKeywords.Any())
                {
                    var innerShoulds = NestHelper.KeywordsToNest<SearchEvaluation>(
                        queryModel.QueryEsKeywords,
                        queryModel.IsMatchAllWord,
                        Infer.Fields("title.pinyin", "title.keyword"),
                        Infer.Fields("title.cntext"));
                    mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
                }
            }



            search.Query(q =>
                q.Bool(b =>
                    b.Must(mustQuerys).Should(shouldQuerys)
                ));

            var result = _client.Search<SearchEvaluation>(search);

            var data = result.Hits.Select(s => s.Source).ToList();

            return PaginationModel.Build(data, result.Total);
        }
    }
}
