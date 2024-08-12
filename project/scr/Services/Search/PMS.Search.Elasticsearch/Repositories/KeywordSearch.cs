using Microsoft.Extensions.Options;
using Nest;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.SearchAccessor;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Elasticsearch.Repositories
{
    public class KeywordSearch : IKeywordSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _keywordIndex;

        public KeywordSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _keywordIndex = _esConfig.Indices.Keyword;
            }
        }

        public async Task<PaginationModel<SearchKeywordHighlight>> SearchAsync(SearchKeywordQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            int from = (queryModel.PageIndex - 1) * queryModel.PageSize;
            int size = queryModel.PageSize;

            var search = new SearchDescriptor<SearchKeyword>()
                .Index(new string[] { _keywordIndex.SearchIndex })
                .Highlight(h => h.Fields(f => f.Field("keyword")))
                .Sort(sd => sd.Descending(t => t.ResultNumber).Descending(SortSpecialField.Score))
                .From(from)
                .Size(size);

            var mustQuery = new List<Func<QueryContainerDescriptor<SearchKeyword>, QueryContainer>>()
            {
                q=> q.Term(t => t.Field(p => p.IsDeleted).Value(false))
            };
            var constantQuery = new ConstantScoreQuery();

            //模糊查询
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Fields("keyword^1.1", "keyword.pinyin"),
                    Query = keyword
                };

                mustQuery.Add(q => query);
            }

            search.Query(q => q.FunctionScore(csq => csq.Query(qc => qc.Bool(b => b.Must(mustQuery)))
                .Functions(sfd =>
                    sfd.FieldValueFactor(fff => fff.Field(f => f.ExtraPoint)
                    .Factor(1)
                    .Missing(0)
                    .Modifier(FieldValueFactorModifier.None)))
                .BoostMode(FunctionBoostMode.Sum)
            ));


            var result = await _client.SearchAsync<SearchKeyword>(search);
            var total = result.Total;
            var data = result.Hits.Select(s => new SearchKeywordHighlight()
            {
                Keyword = s.Source.Keyword
            }).ToList();
            var highs = result.Hits.Select(s => s.Highlight).ToList();
            HighlightHelper.SetHighlights(ref data, highs, "Keyword", "Highlight");
            return PaginationModel.Build(data, total);
        }
    }
}
