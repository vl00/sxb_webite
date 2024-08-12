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
    public class ArticleSearch : IArticleSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _index;

        public ArticleSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _index = _esConfig.Indices.Article;
            }
        }

        public async Task<long> SearchTotalAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaId, List<int> articleTypes, List<string> schTypes, bool? isTop)
        {
            var queryDesc = GetQueryDescriptor(keyword, userId, provinceId, cityId, areaId, articleTypes, schTypes, isTop);
            var result = await _client.CountAsync<SearchArticle>(c =>
                                    c.Index(_index.SearchIndex).Query(queryDesc)
                                );
            return result.Count;
        }

        public async Task<PaginationModel<SearchArticle>> SearchAsync(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaIds, List<int> articleTypes, List<string> schTypes, bool? isTop
            , ArticleOrderBy orderBy, int pageIndex = 1, int pageSize = 10)
        {
            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            Func<SortDescriptor<SearchArticle>, IPromise<IList<ISort>>> sort;
            switch (orderBy)
            {
                case ArticleOrderBy.HotDesc:
                    sort = s => s.Descending(a => a.ViewCount);
                    break;
                case ArticleOrderBy.HotAsc:
                    sort = s => s.Ascending(a => a.ViewCount);
                    break;
                case ArticleOrderBy.TimeDesc:
                    sort = s => s.Descending(a => a.Time);
                    break;
                case ArticleOrderBy.TimeAsc:
                    sort = s => s.Ascending(a => a.Time);
                    break;
                case ArticleOrderBy.TopTimeDesc:
                    sort = s => s.Descending(a => a.TopTime);
                    break;
                case ArticleOrderBy.Default:
                default:
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        sort = s => s.Descending(SortSpecialField.Score);
                    }
                    //无搜索词, 时间逆序
                    else
                    {
                        sort = s => s.Descending(a => a.Time);
                    }
                    break;
            }

            //日期衰减
            var queryDesc = GetQueryDescriptor(keyword, userId, provinceId, cityId, areaIds, articleTypes, schTypes, isTop);
            var searchDesc = new SearchDescriptor<SearchArticle>()
                .Index(_index.SearchIndex)
                .From(from)
                .Size(size)
                .Sort(sort)
                .Query(q =>
                    q.FunctionScore(fs =>
                        fs.Query(queryDesc)
                        .BoostMode(FunctionBoostMode.Sum)
                        .Functions(fun =>
                            fun.GaussDate(gd =>
                                gd.Field(f => f.Time)
                                .Origin(DateTime.Now)
                                .Scale(TimeSpan.FromDays(1))
                                .Offset(TimeSpan.FromDays(30)
                            )))
                   ));

            var result = await _client.SearchAsync<SearchArticle>(searchDesc);
            var total = result.Total;
            var data = result.Hits.Select(q => q.Source).ToList();
            return PaginationModel.Build(data, total);
        }


        private Func<QueryContainerDescriptor<SearchArticle>, QueryContainer> GetQueryDescriptor(string keyword, Guid? userId, int? provinceId, int? cityId
            , List<int> areaIds, List<int> articleTypes, List<string> schTypes, bool? isTop)
        {
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchArticle>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchArticle>, QueryContainer>>
            {
            };

            //关键字查询
            if (!string.IsNullOrEmpty(keyword))
            {
                var query = new MultiMatchQuery()
                {
                    //Fields = Infer.Field<SearchArticle>(f => f.Title).And("tags^10"),
                    Fields = new string[] { "title.pinyin", "tags^10" },
                    Query = keyword,
                    Analyzer = "ik_search_analyzer",
                };
                mustQuerys.Add(q => query);
                shouldQuerys.Add(s => s.Match(m => m.Field("title").Query(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("title.pinyin").Value(keyword).Boost(2)));
            }

            if (isTop != null)
            {
                mustQuerys.Add(m => m.Term(t => t.Field(tf => tf.ToTop).Value(isTop.Value)));
            }

            //达人作者查询
            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchArticle>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }

            //文章类型查询
            if (articleTypes?.Any() == true)
            {
                mustQuerys.Add(mq => mq.Terms(t => t.Field(tf => tf.ArticleType).Terms(articleTypes)));
            }

            //学校类型给查询
            if (schTypes?.Any() == true)
            {
                mustQuerys.Add(mq => mq.Terms(t => t.Field(tf => tf.SchTypes).Terms(schTypes)));
            }

            //城市和城区查询
            if (cityId > 0)
            {
                if (!(provinceId > 0))
                {
                    throw new ArgumentNullException("请传入城市所在省份");
                }

                //不限制城市, 但同城,同省,全国优先
                mustQuerys.Add(mq => mq.Nested(n =>
                    n.Path(p => p.Areas)
                    .Query(nq =>
                        nq.Bool(nqb => nqb.Should(
                                //其他市
                                //s => s.Exists(e => e.Name("areas.cityId").Boost(1)),
                                //全市
                                s => s.Term("areas.cityId", cityId, 3),
                                //全省
                                s => s.Bool(b => b.Must(
                                      m => m.Term("areas.cityId", 0),
                                      m => m.Term("areas.provinceId", provinceId, 2)
                                    )),
                                //全国
                                s => s.Bool(b => b.Must(
                                      m => m.Term("areas.provinceId", 0, 1)
                                    ))
                            )
                    ))
                ));

                if (areaIds.Any())
                {
                    mustQuerys.Add(mq => mq.Nested(n =>
                        n.Path(p => p.Areas)
                        .Query(nq =>
                            nq.Bool(nqb => nqb.Must(m => m.Terms(t => t.Field("areas.areaId").Terms(areaIds)))
                        ))
                    ));
                }
            }
            return q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys));
        }

        private BoolQueryDescriptor<SearchArticle> CitySearch(BoolQueryDescriptor<SearchArticle> nqb, int? provinceId, int? cityId, List<int> areaId)
        {
            var boolQuery = nqb.Should(
                //全市
                s => s.Term("areas.cityId", cityId),
                //全省
                s => s.Bool(b => b.Must(
                      m => m.Term("areas.cityId", 0),
                      m => m.Term("areas.provinceId", provinceId)
                    )),
                //全国
                s => s.Bool(b => b.Must(
                      m => m.Term("areas.provinceId", 0)
                    ))
            );
            if (areaId.Any())
            {
                boolQuery.Must(m => m.Terms(t => t.Field("areas.areaId").Terms(areaId)));
            }
            return boolQuery;
        }


        /// <summary>
        /// 搜索文章列表
        /// </summary>
        public List<SearchArticle> SearchArticle(out long total, SearchArticleQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var userId = queryModel.UserId;
            var orderBy = queryModel.OrderBy;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchArticle>();
            searchDesc = searchDesc.Index(_index.SearchIndex)
                                   .Sort(SearchPUV.GetSortDesc<SearchArticle>(orderBy, "time"))
                                   .From(from)
                                   .Size(size);
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchArticle>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchArticle>, QueryContainer>>
            {
            };

            if (queryModel.QueryEsKeywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchArticle>(
                    queryModel.QueryEsKeywords,
                    queryModel.IsMatchAllWord,
                    Infer.Fields(new string[] { "title.pinyin", "tags^10" }),
                    Infer.Fields(new string[] { "title.cntext", "tags^10" }));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));


                shouldQuerys.Add(s => s.Match(m => m.Field("title").Query(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("title.pinyin").Value(keyword).Boost(2)));
            }

            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchArticle>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }
            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q =>
                    q.FunctionScore(fs => fs.Query(fsq => fsq.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)))
                    .BoostMode(FunctionBoostMode.Sum)
                    .Functions(fun =>
                        fun.GaussDate(gd =>
                            gd.Field(f => f.UpdateTime)
                            .Origin(DateTime.Now)
                            .Scale(TimeSpan.FromDays(1))
                            .Offset(TimeSpan.FromDays(30)
                    )))
                ));
            }

            var result = _client.Search<SearchArticle>(searchDesc);
            total = result.Total;
            return result.Hits.Select(q => q.Source).ToList();
        }
    }
}
