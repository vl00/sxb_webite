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
    public class TalentSearch : ITalentSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _talentIndex;

        public TalentSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _talentIndex = _esConfig.Indices.Talent;
            }
        }

        public PaginationModel<SearchTalent> SearchPagination(SearchBaseQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var orderBy = queryModel.OrderBy;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            var sort = SearchPUV.GetSortDesc<SearchTalent>(orderBy, () =>
            {
                return s => s
                  .Descending(SortSpecialField.Score)
                  .Script(sc => sc
                      .Type("number")
                      .Script(ss => ss.Source(" return doc['fansTotal'].value + doc['answersTotal'].value "))
                  );
            });

            var search = new SearchDescriptor<SearchTalent>()
              .Index(_talentIndex.SearchIndex)
              .From(from)
              .Size(size)
              .Sort(sort)
              ;
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchTalent>, QueryContainer>>
            {
                q => q.Match(t => t.Field("nickName").Query(keyword).Boost(2)),
                q => q.Term(t => t.Field("nickName.pinyin").Value(keyword).Boost(2)),
            };
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchTalent>, QueryContainer>>
            {
                m => m.Term(t => t.Field(f => f.IsDeleted).Value(false)),
                m => m.Term(t => t.Field(f => f.IsEnable).Value(true)),
                //|| m.Match(ma => ma.Field("authTitle.pinyin").Query(keyword))
            };

            if (queryModel.QueryEsKeywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchTalent>(
                    queryModel.QueryEsKeywords,
                    queryModel.IsMatchAllWord,
                    Infer.Field("nickName.pinyin"),
                    Infer.Field("nickName.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
            }

            mustQuerys.Add(mm => mm.Bool(b =>
                b.Should(q =>
                    q.Bool(qs => qs.Must(m => m.Term("cityCode", queryModel.CityCode)))
                    || q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field("cityCode"))))
            )));

            search.Query(q =>
                q.Bool(b =>
                    b.Must(mustQuerys).Should(shouldQuerys)
                ));

            var result = _client.Search<SearchTalent>(search);

            var data = result.Hits.Select(s => s.Source).ToList();

            return PaginationModel.Build(data, result.Total);
        }

        public List<SearchTalent> SearchTalents(SearchBaseQueryModel queryModel)
        {
            return SearchPagination(queryModel).Data;
        }
    }
}
