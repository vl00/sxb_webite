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
    public class CourseSearch : ICourseSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _courseIndex;

        public CourseSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _courseIndex = _esConfig.Indices.Course;
            }
        }

        public async Task<PaginationModel<SearchCourse>> SearchCourses(SearchCourseQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var orderBy = queryModel.OrderBy;
            var type = queryModel.Type;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            var search = new SearchDescriptor<SearchCourse>()
              .Index(_courseIndex.SearchIndex)
              .From(from)
              .Size(size)
              .Sort(SearchPUV.GetSortDesc<SearchCourse>(orderBy, "modifyDateTime"))
              ;

            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchCourse>, QueryContainer>>
            {
                q => q.Term(t => t.Field("title").Value(keyword).Boost(10))
            };

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchCourse>, QueryContainer>>
            {
                m => m.Term(t => t.Field(f => f.IsDeleted).Value(false)),
                //m => m.Term(t => t.Field(f => f.IsValid).Value(1)),
                //m => m.Term(t => t.Field(f => f.Status).Value(1)),
            };

            if (!queryModel.SearchTitleOnly)
            {
                if (queryModel.QueryEsKeywords.Any())
                { 
                    var innerShoulds = NestHelper.KeywordsToNest<SearchCourse>(
                        queryModel.QueryEsKeywords,
                        queryModel.IsMatchAllWord,
                        Infer.Fields("title.pinyin", "title.keyword", "orgName.pinyin", "orgName.keyword"),
                        Infer.Fields("title.cntext", "orgName.cntext"));
                    mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));

                    shouldQuerys.Add(q => q.Term(t => t.Field("orgName").Value(keyword).Boost(10)));
                }
            }
            else
            {
                if (queryModel.QueryEsKeywords.Any())
                {
                    var innerShoulds = NestHelper.KeywordsToNest<SearchCourse>(
                        queryModel.QueryEsKeywords,
                        queryModel.IsMatchAllWord,
                        Infer.Fields("title.pinyin", "title.keyword"),
                        Infer.Fields("title.cntext"));
                    mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
                }
            }


            if (type != null)
            {
                mustQuerys.Add(m => m.Term(t => t.Field(f => f.Type).Value(type)));
            }


            search.Query(q =>
                q.Bool(b =>
                    b.Must(mustQuerys).Should(shouldQuerys)
                ));

            var result = await _client.SearchAsync<SearchCourse>(search);
            var data = result.Hits.Select(s => s.Source).ToList();
            return PaginationModel.Build(data, result.Total);
        }


        public async Task<List<SearchShortIdSuggestItem>> ListSuggest(string keyword, int pageSize)
        {
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchCourse>, QueryContainer>>
            {
                q => q.Term(t => t.Field("title").Value(keyword).Boost(2))
            };
            Func<HighlightDescriptor<SearchCourse>, IHighlight> hignlight =
                h => h.Fields(
                    f => f.Field("title")
                );

            var result = await _client.SearchAsync<SearchCourse>(s => s
                   .Index(_courseIndex.SearchIndex)
                   .Size(0)
                   .Query(q =>
                        q.Term(t => t.Field(f => f.IsValid).Value(1))
                        && q.Term(t => t.Field(f => f.Status).Value(1))
                        && q.Bool(b => b.Must(m => m.MultiMatch(mm => mm
                              .Fields(mf => mf
                                  .Fields("title.pinyin", "title.keyword")
                              )
                              .Query(keyword)
                           ))
                       )
                   )
                   .Aggregations(aggs => aggs
                       .Terms("index_aggs", t => t.Field("_index")
                           .Aggregations(indexaggs => indexaggs.TopHits("top_hits_aggs", th => th.Size(pageSize).Highlight(hignlight)))
                       )
                   )
            );

            var total = result.Total;
            List<SearchShortIdSuggestItem> suggest = new List<SearchShortIdSuggestItem>();

            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["index_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;
                    var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                    var hits = TopHitsAgg.Hits<SearchSuggestOriginItem>();

                    var highs = hits.Select(s => s.Highlight).ToList();
                    var source = hits.Select(q => q.Source).ToList();
                    suggest = HighlightHelper.GetSimpleSuggestItems(source, highs, "Title", (originItem, highlightValue) =>
                    {
                        return new SearchShortIdSuggestItem()
                        {
                            No = originItem.No,
                            Name = highlightValue
                        };
                    });
                }

            }
            return suggest;
        }

    }
}
