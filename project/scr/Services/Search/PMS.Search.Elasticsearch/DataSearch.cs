using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.SearchAccessor;
using Remotion.Linq;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Elasticsearch
{
    public class DataSearch : IDataSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        //index名称使用别名
        private readonly string _schoolIndexName = "school";
        private readonly string _schoolRankIndexName = "schoolrank";
        private readonly string _commentIndexName = "comment";
        private readonly string _questionIndexName = "question";
        private readonly string _answerIndexName = "answer";
        private readonly string _liveIndexName = "live";
        private readonly string _articleIndexName = "article";
        private readonly string _singSchoolIndexName = "sign";
        private readonly string _BTschoolSearchIndexName = "schsearch";
        private readonly string _topicIndexName = "topic";
        private readonly string _circleIndexName = "circle";
        private readonly string _talentIndexName = "talent";
        private readonly string _svsSchoolIndexName = "svsschool";
        private readonly string _organizationIndexName = "organization";
        private readonly string _courseIndexName = "course";
        private readonly string _evaluationIndexName = "evaluation";


        public DataSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value;

            if (_esConfig.Indices != null)
            {
                _schoolIndexName = _esConfig.Indices.School.SearchIndex;
                _topicIndexName = _esConfig.Indices.Topic.SearchIndex;
                _circleIndexName = _esConfig.Indices.Circle.SearchIndex;
                _talentIndexName = _esConfig.Indices.Talent.SearchIndex;

                _commentIndexName = _esConfig.Indices.Comment.SearchIndex;
                _questionIndexName = _esConfig.Indices.Question.SearchIndex;
            }
        }

        public void SuggestCount()
        {

        }

        /// <summary>
        /// 联想搜索列表
        /// </summary>
        public SearchSuggestResult ListSuggest(string keyword, int channel, int cityCode)
        {
            ChannelIndex channelIndex;
            switch (channel)
            {
                case 0:
                    channelIndex = ChannelIndex.School;
                    break;
                case 1:
                    channelIndex = ChannelIndex.Comment;
                    break;
                case 2:
                    channelIndex = ChannelIndex.Question;
                    break;
                case 3:
                    channelIndex = ChannelIndex.SchoolRank;
                    break;
                case 4:
                    channelIndex = ChannelIndex.Article;
                    break;
                case 5:
                    channelIndex = ChannelIndex.Live;
                    break;
                case 7:
                    channelIndex = ChannelIndex.Circle;
                    break;
                default:
                    channelIndex = ChannelIndex.All;
                    break;
            }
            return ListSuggest(keyword, channelIndex, cityCode);
        }

        public SearchSuggestResult ListSuggest(string keyword, ChannelIndex channel, int cityCode)
        {
            var channelIndex = GetIndex(channel);
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                q => q.Term(t => t.Field("name.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field("context.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field("title.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field(tf => tf.CityCode).Value(cityCode).Boost(20)),
                q => q.Term(t => t.Field("userName.pinyin").Value(keyword).Boost(2)),
            };

            var result = _client.Search<SearchSchool>(s => s
                   .Index(channelIndex)
                   .Size(0)
                   .Query(q =>
                       (
                        q.Bool(qs => qs.Must(m => m.Term(tf => tf.Field(f => f.IsDeleted).Value(false))))
                        ||
                        q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field(f => f.IsDeleted))))
                       ) &&
                       q.Bool(b => b.Must(m => m.MultiMatch(mm => mm
                              .Fields(mf => mf
                                  .Field("name.pinyin^1.1").Field("cityarea").Field("schooltype")
                                  .Field("context.pinyin^1.1")
                                  .Field("title.pinyin^1.1")
                                  .Field("userName.pinyin")
                              )
                              .Query(keyword).Analyzer("ik_search_analyzer")
                           )).Should(shouldQuerys)
                       )
                   //&&q.Bool(b => b
                   //        .Must(m =>
                   //            m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
                   //            &&
                   //            (
                   //            m.Bool(mb=> mb.MustNot(mbn=>mbn.Exists(t=>t.Field(tf => tf.CityCode))))||
                   //            m.Bool(mb => mb.Must(mbn => mbn.Term(t => t.Field(tf => tf.CityCode).Value(cityCode)))
                   //            )
                   //        )
                   //    )
                   )
                   .Aggregations(aggs => aggs
                       .Terms("index_aggs", t => t.Field("_index").Size(7)
                           .Aggregations(indexaggs => indexaggs.TopHits("top_hits_aggs", th => th.Size(5)))
                       )
                   )
            );

            var total = result.Total;

            List<SearchSuggest> suggest = new List<SearchSuggest>();

            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["index_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;

                    if (((string)bucket.Key).Contains(_schoolRankIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchSchoolRank>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 4,
                            Index = ChannelIndex.SchoolRank,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = q.Title,
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                    else if (((string)bucket.Key).Contains(_schoolIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchSchool>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 1,
                            Index = ChannelIndex.School,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = q.Name,
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                    else if (((string)bucket.Key).Contains(_commentIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchComment>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 2,
                            Index = ChannelIndex.Comment,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = q.Context,
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                    else if (((string)bucket.Key).Contains(_questionIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchQuestion>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 3,
                            Index = ChannelIndex.Question,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = q.Context,
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                    else if (((string)bucket.Key).Contains(_articleIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchSchoolRank>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 5,
                            Index = ChannelIndex.Article,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = q.Title,
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                    else if (((string)bucket.Key).Contains(_liveIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchLive>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 6,
                            Index = ChannelIndex.Live,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = q.Title,
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                    else if (((string)bucket.Key).Contains(_circleIndexName))
                    {
                        var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                        var source = TopHitsAgg.Hits<SearchCircle>().Select(q => q.Source).ToList();
                        suggest.Add(new SearchSuggest
                        {
                            Type = 7,
                            Index = ChannelIndex.Circle,
                            Total = TopHitsAgg.Total.Value,
                            Items = source.Select(q => new SearchSuggestItem
                            {
                                Name = $"{q.Name}-{q.UserName}",
                                SourceId = q.Id,
                            }).ToList()
                        });
                    }
                }

            }
            return new SearchSuggestResult
            {
                Total = total,
                Suggests = suggest
            };
        }

        /// <summary>
        /// 使用es highlight
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        //public SearchSuggestResult ListSuggestBak(SearchAllQueryModel queryModel)
        //{
        //    string keyword = queryModel.Keyword;
        //    ChannelIndex channel = queryModel.ChannelIndex;
        //    int cityCode = queryModel.CityCode;
        //    int size = queryModel.Size;

        //    var channelIndex = GetIndex(channel);
        //    var queryDesc = GetSuggestQueryDescriptor(queryModel.Keyword, new List<string>() { keyword }, cityCode);
        //    Func<HighlightDescriptor<SearchSchool>, IHighlight> hignlight =
        //        h => h.Fields(
        //            f => f.Field("name.pinyin"),
        //            f => f.Field("cityarea"),
        //            f => f.Field("schooltype"),
        //            f => f.Field("context.pinyin"),
        //            f => f.Field("title.pinyin"),
        //            f => f.Field("talentUserName.pinyin"),
        //            //f => f.Field("authTitle.pinyin"),
        //            f => f.Field("nickName.pinyin")
        //        );

        //    var result = _client.Search<SearchSchool>(s => s
        //           .Index(channelIndex)
        //           .Size(0)
        //           .Query(queryDesc)
        //           .Aggregations(aggs => aggs
        //               .Terms("index_aggs", t => t.Field("_index").Size(channelIndex.Length)
        //                   .Aggregations(indexaggs => indexaggs.TopHits("top_hits_aggs", th => th.Size(size).Highlight(hignlight)))
        //               )
        //           )
        //    );

        //    var total = result.Total;
        //    List<SearchSuggest> suggest = new List<SearchSuggest>();

        //    if (result.Aggregations.Count > 0)
        //    {
        //        var indexAggs = (BucketAggregate)result.Aggregations["index_aggs"];
        //        foreach (var item in indexAggs.Items)
        //        {
        //            var bucket = (KeyedBucket<object>)item;
        //            var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
        //            var hits = TopHitsAgg.Hits<SearchSuggestOriginItem>();

        //            var highs = hits.Select(s => s.Highlight).ToList();
        //            foreach (var high in highs)
        //            {
        //            }

        //            var source = hits.Select(q => q.Source).ToList();

        //            var suggestItem = new SearchSuggest
        //            {
        //                Total = TopHitsAgg.Total.Value
        //            };

        //            var bucketKey = (string)bucket.Key;
        //            if (bucketKey.Contains(_schoolIndexName) && !bucketKey.Contains("rank"))
        //            {
        //                suggestItem.Index = ChannelIndex.School;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Name");
        //            }
        //            else if (bucketKey.Contains(_commentIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Comment;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Context");
        //            }
        //            else if (bucketKey.Contains(_questionIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Question;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Context");
        //            }
        //            else if (bucketKey.Contains(_schoolRankIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.SchoolRank;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Title");
        //            }
        //            else if (bucketKey.Contains(_articleIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Article;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Title");
        //            }
        //            else if (bucketKey.Contains(_liveIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Live;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Title");
        //            }
        //            else if (bucketKey.Contains(_circleIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Circle;
        //                suggestItem.Items = source.Select((q, position) =>
        //                {
        //                    var high = highs.ElementAtOrDefault(position);
        //                    var name = HighlightHelper.GetHighLightValue(high, "Name", q.Name);
        //                    var userName = HighlightHelper.GetHighLightValue(high, "UserName", q.UserName);
        //                    return new SearchSuggestItem
        //                    {
        //                        Name = name,
        //                        Name2 = userName,
        //                        SourceId = q.Id,
        //                    };
        //                }).ToList();
        //            }
        //            else if (bucketKey.Contains(_talentIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Talent;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "NickName");
        //            }
        //            else if (bucketKey.Contains(_courseIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.Course;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Title");
        //            }
        //            else if (bucketKey.Contains(_evaluationIndexName))
        //            {
        //                suggestItem.Index = ChannelIndex.OrgEvaluation;
        //                suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, highs, "Title");
        //            }
        //            else
        //            {
        //                continue;
        //            }

        //            //兼容旧
        //            suggestItem.Type = suggestItem.Index.Value.GetDefaultValue<int>();
        //            suggest.Add(suggestItem);
        //        }

        //    }
        //    return new SearchSuggestResult
        //    {
        //        Total = total,
        //        Suggests = suggest
        //    };
        //}

        /// <summary>
        /// 自定义 highlight
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public SearchSuggestResult ListSuggest(SearchAllQueryModel queryModel)
        {
            string keyword = queryModel.Keyword;
            ChannelIndex channel = queryModel.ChannelIndex;
            int cityCode = queryModel.CityCode;
            int size = queryModel.Size;

            var channelIndex = GetIndex(channel);
            var queryDesc = GetSuggestQueryDescriptor(queryModel.Keyword, queryModel.GetQueryEsKeywords(queryModel.Keyword), cityCode, queryModel.MustCityCode, queryModel.IsMatchAllWord);

            var result = _client.Search<SearchSchool>(s => s
                   .Index(channelIndex)
                   .Size(0)
                   .Query(queryDesc)
                   .Aggregations(aggs => aggs
                       .Terms("index_aggs", t => t.Field("_index").Size(channelIndex.Length)
                           .Aggregations(indexaggs => indexaggs.TopHits("top_hits_aggs", th => th.Size(size)))
                       )
                   )
            );

            var total = result.Total;
            List<SearchSuggest> suggest = new List<SearchSuggest>();

            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["index_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;
                    var TopHitsAgg = ((TopHitsAggregate)bucket["top_hits_aggs"]);
                    var hits = TopHitsAgg.Hits<SearchSuggestOriginItem>();

                    var source = hits.Select(q => q.Source).ToList();
                    var suggestItem = new SearchSuggest
                    {
                        Total = TopHitsAgg.Total.Value
                    };

                    var bucketKey = (string)bucket.Key;
                    if (bucketKey.Contains(_schoolIndexName) && !bucketKey.Contains("rank"))
                    {
                        suggestItem.Index = ChannelIndex.School;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Name");
                    }
                    else if (bucketKey.Contains(_commentIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Comment;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Context");
                    }
                    else if (bucketKey.Contains(_questionIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Question;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Context");
                    }
                    else if (bucketKey.Contains(_schoolRankIndexName))
                    {
                        suggestItem.Index = ChannelIndex.SchoolRank;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Title");
                    }
                    else if (bucketKey.Contains(_articleIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Article;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Title");
                    }
                    else if (bucketKey.Contains(_liveIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Live;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Title");
                    }
                    else if (bucketKey.Contains(_circleIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Circle;
                        suggestItem.Items = source.Select((q, position) =>
                        {
                            var name = HighlightHelper.GetHighLightValue(keyword, q.Name);
                            var userName = HighlightHelper.GetHighLightValue(keyword, q.Name);
                            return new SearchSuggestItem
                            {
                                Name = name,
                                Name2 = userName,
                                SourceId = q.Id,
                            };
                        }).ToList();
                    }
                    else if (bucketKey.Contains(_talentIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Talent;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "NickName");
                    }
                    else if (bucketKey.Contains(_courseIndexName))
                    {
                        suggestItem.Index = ChannelIndex.Course;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Title");
                    }
                    else if (bucketKey.Contains(_evaluationIndexName))
                    {
                        suggestItem.Index = ChannelIndex.OrgEvaluation;
                        suggestItem.Items = HighlightHelper.GetSimpleSuggestItems(source, keyword, "Title");
                    }
                    else
                    {
                        continue;
                    }

                    //兼容旧
                    suggestItem.Type = suggestItem.Index.Value.GetDefaultValue<int>();
                    suggest.Add(suggestItem);
                }

            }
            return new SearchSuggestResult
            {
                Total = total,
                Suggests = suggest
            };
        }

        public async Task<long> ListSuggestTotalAsync(string keyword, ChannelIndex channel, int cityCode)
        {
            var channelIndex = GetIndex(channel);
            var queryDesc = GetSuggestQueryDescriptor(keyword, new List<string>() { keyword }, cityCode, mustCityCode: true, isMatchAllWord: true);

            var result = await _client.SearchAsync<SearchSchool>(s =>
                s.Index(channelIndex)
                .TrackTotalHits()
                .Size(0)
                .Query(queryDesc)
            );

            return result.Total;
        }

        private Func<QueryContainerDescriptor<SearchSchool>, QueryContainer> GetSuggestQueryDescriptor(string keyword, List<string> keywords, int cityCode, bool mustCityCode = false, bool isMatchAllWord = false)
        {
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                q => q.Match(t => t.Field("name").Query(keyword).Boost(2)),
                q => q.Match(t => t.Field("context").Query(keyword).Boost(2)),
                q => q.Match(t => t.Field("title").Query(keyword).Boost(2)),
                q => q.Match(t => t.Field("talentUserName").Query(keyword).Boost(2)),
                //q => q.Match(t => t.Field("authTitle").Query(keyword).Boost(2)),
                q => q.Match(t => t.Field("nickName").Query(keyword).Boost(2)),

                q => q.Term(t => t.Field("name.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field("context.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field("title.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field("talentUserName.pinyin").Value(keyword).Boost(2)),
                //q => q.Term(t => t.Field("authTitle.pinyin").Value(keyword).Boost(2)),
                q => q.Term(t => t.Field("nickName.pinyin").Value(keyword).Boost(2)),
            };
            if (cityCode != 0)
            {
                shouldQuerys.Add(q => q.Term(t => t.Field(tf => tf.CityCode).Value(cityCode).Boost(20)));
            }

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                //m => m.MultiMatch(mm =>
                //        mm.Fields(mf => mf
                //            .Field("name.cntext").Field("name.keyword").Field("tags.pinyin")
                //            .Field("context.pinyin^1.1")
                //            .Field("title.pinyin^1.1").Field("title.keyword^1.1")
                //            .Field("talentUserName.pinyin")
                //            //.Field("authTitle.pinyin^1.1")
                //            .Field("nickName.pinyin")
                //        ).Query(keyword))
            };

            if (keywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchSchool>(
                    keywords,
                    isMatchAllWord,
                    Infer.Fields("name.cntext", "name.keyword", "tags.pinyin",
                            "context.pinyin^1.1", "title.pinyin^1.1", "title.keyword^1.1",
                            "talentUserName.pinyin", "nickName.pinyin"),
                    Infer.Fields("name.cntext", "tags.cntext", "tags.keyword",
                            "context.cntext", "title.cntext",
                            "talentUserName.cntext", "nickName.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
            }

            //仅限同城
            if (mustCityCode)
            {
                mustQuerys.Add(mm => mm.Bool(bb =>
                      bb.Should(q =>
                        (
                            q.Bool(qs => qs.Must(m => m.Term(tf => tf.Field(f => f.CityCode).Value(cityCode))))
                            || q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field(f => f.CityCode))))
                        )
                        && (
                            q.Bool(qs => qs.Must(m => m.Term("cityCodes", cityCode)))
                            || q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field("cityCodes"))))
                        )
                     )
                ));
            }

            Func<QueryContainerDescriptor<SearchSchool>, QueryContainer> queryDesc = q =>
            (
                q.Bool(qs => qs.Must(m => m.Term(tf => tf.Field(f => f.IsDeleted).Value(false))))
                ||
                q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field(f => f.IsDeleted))))
             )
             && (
                q.Bool(qs => qs.Must(m => m.Term(tf => tf.Field("isEnable").Value(true))))
                ||
                q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field("isEnable"))))
             )
            && q.Bool(b =>
                b.Must(mustQuerys)
                .Should(shouldQuerys)
            );
            return queryDesc;
        }


        /// <summary>
        /// es index to channel
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ChannelIndex GetChannel(string index)
        {
            ChannelIndex channel;
            string aliasName = index.Substring(0, index.IndexOf("index"));
            if (aliasName.Equals(_courseIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                return ChannelIndex.Course;
            }
            else if (aliasName.Equals(_organizationIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                return ChannelIndex.Org;
            }
            else if (aliasName.Equals(_evaluationIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                return ChannelIndex.OrgEvaluation;
            }

            if (aliasName.Equals(_schoolIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.School;
            }
            else if (aliasName.Equals(_commentIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.Comment;
            }
            else if (aliasName.Equals(_questionIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.Question;
            }
            else if (aliasName.Equals(_schoolRankIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.SchoolRank;
            }
            else if (aliasName.Equals(_articleIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.Article;
            }
            else if (aliasName.Equals(_liveIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.Live;
            }
            else if (aliasName.Equals(_circleIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.Circle;
            }
            else if (aliasName.Equals(_talentIndexName, StringComparison.CurrentCultureIgnoreCase))
            {
                channel = ChannelIndex.Talent;
            }
            else
            {
                channel = ChannelIndex.All;
            }
            return channel;
        }

        /// <summary>
        /// channel to es index array
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string[] GetIndex(ChannelIndex channel)
        {
            List<string> indexNames = new List<string>();
            if (channel.HasFlag(ChannelIndex.School))
            {
                indexNames.Add(_schoolIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Comment))
            {
                indexNames.Add(_commentIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Question))
            {
                indexNames.Add(_questionIndexName);
            }
            if (channel.HasFlag(ChannelIndex.SchoolRank))
            {
                indexNames.Add(_schoolRankIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Article))
            {
                indexNames.Add(_articleIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Live))
            {
                indexNames.Add(_liveIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Circle))
            {
                indexNames.Add(_circleIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Talent))
            {
                indexNames.Add(_talentIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Course))
            {
                indexNames.Add(_courseIndexName);
            }
            if (channel.HasFlag(ChannelIndex.Org))
            {
                indexNames.Add(_organizationIndexName);
            }
            if (channel.HasFlag(ChannelIndex.OrgEvaluation))
            {
                indexNames.Add(_evaluationIndexName);
            }
            return indexNames.ToArray();
        }


        /// <summary>
        /// 判断字符串全部是否是中文。返回true 则表示含有非中文
        /// </summary>
        /// <param name="str_chinese"></param>
        /// <returns></returns>
        private bool IsNotAllChinese(string str_chinese)
        {
            bool b = false;
            for (int i = 0; i < str_chinese.Length; i++)
            {
                Regex reg = new Regex(@"[\u4e00-\u9fa5]");
                if (!reg.IsMatch(str_chinese[i].ToString()))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        private SearchDescriptor<SearchSchool> GetSearchSchoolDescriptor(SearchSchoolQueryModel queryData, bool isVague = false)
        {
            var searchDesc = new SearchDescriptor<SearchSchool>();

            searchDesc = searchDesc.Index(_schoolIndexName)
                                   .StoredFields(sf => sf.Field("_source"));
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            if (!string.IsNullOrEmpty(queryData.Keyword))
            {
                var queryContainer = new QueryContainerDescriptor<SearchSchool>();

                if (IsNotAllChinese(queryData.Keyword) || isVague)
                {
                    queryContainer.Bool(b => b.Should(
                        s => s.Term("name.keyword", queryData.Keyword),
                        s => s.Term("name.pinyin", queryData.Keyword),
                        s => s.MultiMatch(q => new MultiMatchQuery()
                        {
                            Fields = Infer.Field("name.pinyin").And("name.keyword").And("tags.pinyin"),
                            Operator = Operator.Or,
                            Query = queryData.Keyword
                        })
                     ));
                }
                else
                {
                    queryContainer.MultiMatch(q => new MultiMatchQuery()
                    {
                        Fields = Infer.Field("name.cntext").And("tags.pinyin"),
                        Operator = Operator.Or,
                        Query = queryData.Keyword
                    });
                }

                var queryTerm = new TermQuery()
                {
                    Field = Infer.Field("name.cntext"),
                    Value = queryData.Keyword
                    //Boost = 5
                };
                var queryTermTags = new TermQuery()
                {
                    Field = Infer.Field("tags.keyword"),
                    Value = queryData.Keyword
                    //Boost = 5
                };

                mustQuerys.Add(q => q.Bool(b => b.Must(m => queryContainer).Should(s => queryTerm || queryTermTags)));
            }

            if (queryData.Grades.Count > 0 || queryData.SchoolTypeCodes.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();

                foreach (var grade in queryData.Grades)
                {
                    shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Grade).Value(grade)));
                }

                if (queryData.SchoolTypeCodes.Count > 0)
                {
                    var query = new TermsQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.SchooltypeNewCode),
                        Terms = queryData.SchoolTypeCodes
                    };
                    shouldQuerys.Add(mq => query);
                }

                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }


            //筛选省份
            //if (provinces.Count > 0)
            //{
            //    var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();

            //    foreach (var province in provinces)
            //    {
            //        shouldQuerys.Add(q => q.Term(t => t.Field("provinces").Value(province)));
            //    }
            //    mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            //}

            if (false)//queryData.Citys.Count > 0
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();
                foreach (var city in queryData.Citys)
                {
                    var mustCityQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
                    {
                        q => q.Term(t => t.Field(f => f.CityCode).Value(city))
                    };
                    var a = queryData.Areas.Where(q => q / 100 * 100 == city);
                    if (a.Any())
                    {
                        var shouldAreaQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();
                        foreach (var aa in a)
                        {
                            shouldAreaQuerys.Add(q => q.Term(t => t.Field(f => f.AreaCode).Value(aa)));
                        }
                        mustCityQuerys.Add(mq => mq.Bool(b => b.Should(shouldAreaQuerys)));
                    }
                    shouldQuerys.Add(sq => sq.Bool(b => b.Must(mustCityQuerys)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (queryData.Citys.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();
                foreach (var city in queryData.Citys)
                {
                    shouldQuerys.Add(sq => sq.Term(t => t.Field(f => f.CityCode).Value(city)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (queryData.Areas.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();
                foreach (var area in queryData.Areas)
                {
                    shouldQuerys.Add(sq => sq.Term(t => t.Field(f => f.AreaCode).Value(area)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            //if (areas.Count > 0)
            //{
            //    var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();

            //    foreach (var area in areas)
            //    {
            //        shouldQuerys.Add(q => q.Term(t => t.Field(f => f.AreaCode).Value(area)));
            //    }
            //    mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            //}

            if (queryData.MetroIds.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();
                foreach (var metroId in queryData.MetroIds)
                {
                    var mustMetroQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
                    {
                        m => m.Term(t => t.Field(tf => tf.MetroLineId).Value(metroId.LineId)) //.ToString().ToUpper()
                    };

                    if (metroId.StationIds.Any())
                    {
                        var shouldStationQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();
                        foreach (var StationId in metroId.StationIds)
                        {
                            shouldStationQuerys.Add(q => q.Term(t => t.Field(f => f.MetroStationId).Value(StationId)));
                        }
                        mustMetroQuerys.Add(mq => mq.Bool(b => b.Should(shouldStationQuerys)));
                    }
                    shouldQuerys.Add(sq => sq.Bool(b => b.Must(mustMetroQuerys)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (queryData.Distance > 0)
            {
                var query = new GeoDistanceQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Location),
                    DistanceType = GeoDistanceType.Arc,
                    Location = new GeoLocation(queryData.Latitude, queryData.Longitude),
                    Distance = queryData.Distance.ToString() + "m"
                };
                mustQuerys.Add(mq => query);
            }

            if (!(queryData.MinCost == null && queryData.MaxCost == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Tuition),
                    GreaterThanOrEqualTo = queryData.MinCost,
                    LessThan = queryData.MaxCost,
                };
                mustQuerys.Add(mq => query);
            }

            if (!(queryData.MinStudentCount == null && queryData.MaxStudentCount == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Studentcount),
                    GreaterThanOrEqualTo = queryData.MinStudentCount,
                    LessThan = queryData.MaxStudentCount,
                };
                mustQuerys.Add(mq => query);
            }

            if (!(queryData.MinTeacherCount == null && queryData.MaxTeacherCount == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Teachercount),
                    GreaterThanOrEqualTo = queryData.MinTeacherCount,
                    LessThan = queryData.MaxTeacherCount,
                };
                mustQuerys.Add(mq => query);
            }

            if (!(queryData.MinTeach == null && queryData.MaxTeach == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Teach),
                    GreaterThanOrEqualTo = queryData.MinTeach,
                    LessThan = queryData.MaxTeach,
                };
                mustQuerys.Add(mq => query);
            }
            if (!(queryData.MinComposite == null && queryData.MaxComposite == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Composite),
                    GreaterThanOrEqualTo = queryData.MinComposite,
                    LessThan = queryData.MaxComposite,
                };
                mustQuerys.Add(mq => query);
            }
            if (!(queryData.MinHard == null && queryData.MaxHard == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Hard),
                    GreaterThanOrEqualTo = queryData.MinHard,
                    LessThan = queryData.MaxHard,
                };
                mustQuerys.Add(mq => query);
            }
            if (!(queryData.MinLearn == null && queryData.MaxLearn == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Learn),
                    GreaterThanOrEqualTo = queryData.MinLearn,
                    LessThan = queryData.MaxLearn,
                };
                mustQuerys.Add(mq => query);
            }
            if (!(queryData.MinCourse == null && queryData.MaxCourse == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Course),
                    GreaterThanOrEqualTo = queryData.MinCourse,
                    LessThan = queryData.MaxCourse,
                };
                mustQuerys.Add(mq => query);
            }
            if (!(queryData.MinCostScore == null && queryData.MaxCostScore == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Cost),
                    GreaterThanOrEqualTo = queryData.MinCostScore,
                    LessThan = queryData.MaxCostScore,
                };
                mustQuerys.Add(mq => query);
            }
            if (!(queryData.MinTotal == null && queryData.MaxTotal == null))
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Total),
                    GreaterThanOrEqualTo = queryData.MinTotal,
                    LessThan = queryData.MaxTotal,
                };
                mustQuerys.Add(mq => query);
            }

            if (queryData.Lodging != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Lodging),
                    Value = queryData.Lodging
                };
                mustQuerys.Add(mq => query);
            }
            if (queryData.Sdextern != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Sdextern),
                    Value = queryData.Sdextern
                };
                mustQuerys.Add(mq => query);
            }

            if (queryData.Canteen != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Canteen),
                    Value = queryData.Canteen
                };
                mustQuerys.Add(mq => query);
            }

            if (queryData.AuthIds.Count > 0 || queryData.CharacIds.Count > 0 || queryData.AbroadIds.Count > 0)
            {
                if (queryData.AuthIds.Count > 0)
                {
                    var query = new TermsQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.Authentication),
                        Terms = queryData.AuthIds.Select(q => q.ToString())
                    };
                    mustQuerys.Add(mq => query);
                }
                if (queryData.CharacIds.Count > 0)
                {
                    var query = new TermsQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.Characteristic),
                        Terms = queryData.CharacIds.Select(q => q.ToString())
                    };
                    mustQuerys.Add(mq => query);
                }
                if (queryData.AbroadIds.Count > 0)
                {
                    var query = new TermsQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.Abroad),
                        Terms = queryData.AbroadIds.Select(q => q.ToString())
                    };
                    mustQuerys.Add(mq => query);
                }
            }

            if (queryData.CourseIds.Count > 0)
            {
                var query = new TermsQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Courses),
                    Terms = queryData.CourseIds.Select(q => q.ToString())
                };
                mustQuerys.Add(mq => query);
            }

            if (queryData.MustCurrentCity != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.CityCode),
                    Value = queryData.MustCurrentCity
                };
                mustQuerys.Add(mq => query);
            }
            if (mustQuerys.Count > 0)
            {
                Func<QueryContainerDescriptor<SearchSchool>, QueryContainer> query = q => q.Bool(b => b.Must(mustQuerys));
                if (queryData.CurrentCity != null)
                {
                    var queryCurrentCity = new TermQuery()
                    {
                        Field = Infer.Field<SearchSchool>(p => p.CityCode),
                        Value = queryData.CurrentCity,
                        Boost = 5
                    };
                    query = q => q.Bool(b => b.Must(mustQuerys).Should(queryCurrentCity));
                }

                //默认同城优先, 高分优先
                if (queryData.IsDefault && queryData.CurrentCity != null)
                {
                    searchDesc.Query(q => q
                            .FunctionScore(fs => fs
                                .Query(query)
                                .BoostMode(FunctionBoostMode.Sum)
                                .Functions(fun => fun.ScriptScore(
                                    ss => ss.Script(sss => sss
                                        //当前城市优先排序  当前城市的D评分 > 其他城市的A评分
                                        .Source($@" 
                                            double score = doc['score.total'].size() != 0 ? doc['score.total'].value : 0;
                                            if(doc['cityCode'].size() != 0 && doc['cityCode'].value == params.currentCity) return params.baseScore + score; else return score; ")
                                        .Params(p => p.Add("baseScore", 100).Add("currentCity", queryData.CurrentCity ?? 0))
                                    )
                                ))
                            ));
                }
                else
                {
                    searchDesc.Query(query);
                }
            }

            if (queryData.Longitude > 0 && queryData.Latitude > 0)
            {
                searchDesc = searchDesc.ScriptFields(q => q
                    .ScriptField("distance", sf => sf
                         .Source("doc['location'].arcDistance(params.lat,params.lon)")
                         .Params(p => p.Add("lat", queryData.Latitude).Add("lon", queryData.Longitude))
                         .Lang("painless")
                    )
                );
            }
            return searchDesc;
        }

        /// <summary>
        /// 搜索学校列表
        /// </summary>
        /// <param name="queryData">查询条件</param>
        /// <param name="total">返回总数</param>
        /// <param name="isVague">是否模糊或者拼音查询</param>
        /// <returns></returns>
        public List<SchoolResult> SearchSchool(SearchSchoolQueryModel queryData, out long total, bool isVague = false)
        {
            //无关键字, 并且未指定排序, 则使用学校分数排序
            if (queryData.OrderBy == 0 && string.IsNullOrWhiteSpace(queryData.Keyword))
            {
                //queryData.OrderBy = 5;
                queryData.IsDefault = true;
            }

            int from = (queryData.PageNo - 1) * queryData.PageSize;
            int size = queryData.PageSize;
            var searchDesc = GetSearchSchoolDescriptor(queryData, isVague);

            //排序
            Func<SortDescriptor<SearchSchool>, IPromise<IList<ISort>>> sortDesc = sd =>
            {
                //排序
                switch (queryData.OrderBy)
                {
                    case 1:
                        sd.GeoDistance(d => d.Field(f => f.Location).Points(new GeoLocation(queryData.Latitude, queryData.Longitude)).Descending());
                        break;
                    case 2:
                        sd.GeoDistance(d => d.Field(f => f.Location).Points(new GeoLocation(queryData.Latitude, queryData.Longitude)).Ascending());
                        break;
                    case 3:
                        sd.Descending(d => d.Tuition);
                        break;
                    case 4:
                        //sd.Ascending(d => d.Tuition);
                        sd.Field(d => d.Field(p => p.Tuition).Order(SortOrder.Ascending).MissingFirst());
                        break;
                    case 5:
                        sd.Descending(d => d.Score.Total);
                        break;
                    case 6:
                        sd.Descending(d => d.Score.Teach);
                        break;
                    case 7:
                        sd.Descending(d => d.Score.Hard);
                        break;
                    case 8:
                        sd.Descending(d => d.Score.Cost);
                        break;
                    case 9:
                        sd.Descending(d => d.Score.Envir);
                        break;
                    //case 5:
                    //    sd.Descending(d => d.Questioncount);
                    //    break;
                    //10  升序评分
                    case 10:
                        sd.Field(d => d.Field(p => p.Score.Total).Order(SortOrder.Ascending).MissingFirst());
                        break;
                    //11  降序热度
                    case 11:
                        sd.Descending(d => d.HotValue);
                        break;
                    //12  升序热度
                    case 12:
                        //sd.Ascending(d => d.HotValue);
                        sd.Field(d => d.Field(p => p.HotValue).Order(SortOrder.Ascending).MissingFirst());
                        break;
                    default:
                        //根据分值排序
                        sd.Descending(SortSpecialField.Score);
                        break;
                }
                return sd;
            };

            int minScore = 0;
            //if (!string.IsNullOrWhiteSpace(queryData.Keyword))
            //{
            //    minScore = 30;
            //}

            var result = _client.Search<SearchSchool>(searchDesc
                                   .MinScore(minScore)
                                   .TrackTotalHits(true)
                                   .Sort(sortDesc)
                                   .From(from)
                                   .Size(size)
                                   );
            total = result.Total;

            return result.Hits.Where(q => q.Source != null).Select(q => new SchoolResult
            {
                Id = q.Source.Id,
                SchoolId = q.Source.SchoolId,
                Name = q.Source.Name,
                Distance = (queryData.Longitude > 0 && queryData.Latitude > 0) ? q.Fields.Value<double>("distance") : 0,
                Score = q.Score
            }).ToList();
        }


        public async Task<long> SearchSchoolTotalCountAsync(SearchSchoolQueryModel queryData, bool isVague = false)
        {
            var searchDesc = GetSearchSchoolDescriptor(queryData, isVague);
            int minScore = 0;
            var result = await _client.SearchAsync<SearchSchool>(searchDesc
                                   .MinScore(minScore)
                                   //.TrackTotalHits(true) 最多1万
                                   .Size(0)
                           );
            return result.Total;
        }

        public List<(int cityId, long totalCount)> SearchSchoolCity(SearchSchoolQueryModel queryData, bool isVague = false)
        {
            var searchDesc = GetSearchSchoolDescriptor(queryData, isVague);

            int minScore = 0;
            var result = _client.Search<SearchSchool>(searchDesc
                                   .MinScore(minScore)
                                   .TrackTotalHits(true)
                                   .Size(0)
                                   .Aggregations(aggs => aggs.Terms("city",
                                        //agg => agg.Field(f => f.City).Size(3)
                                        agg => agg.Field(f => f.CityCode).Size(3)
                                       )
                                   )
                           );
            foreach (var agg in result.Aggregations)
            {
                var bucket = (BucketAggregate)agg.Value;
                //foreach (var item in bucket.Items)
                //{
                //    var itemBucket = (KeyedBucket<object>)item;
                //}

                return bucket.Items.Select(s =>
                    {
                        var b = (KeyedBucket<object>)s;
                        return ((int)(double)b.Key, b.DocCount ?? 0L);
                    })
                    .OrderByDescending(s => s.Item2)
                    .ToList();
            }
            return new List<(int cityId, long totalCount)>();
        }

        /// <summary>
        /// 学校费用区间分布条形图统计
        /// </summary>
        public List<SchoolStatsResult> SearchSchoolCostStats(SearchSchoolQueryModel queryData, bool isVague = false)
        {
            var searchDesc = GetSearchSchoolDescriptor(queryData, isVague);


            var costRanges = new List<Func<AggregationRangeDescriptor, IAggregationRange>>()
            {
                r => r.From(0).To(5000),
                r => r.From(5000).To(10000),
                r => r.From(10000).To(15000),
                r => r.From(15000).To(20000),
                r => r.From(20000).To(25000),
                r => r.From(25000).To(30000),
                r => r.From(30000).To(35000),
                r => r.From(35000).To(40000),
                r => r.From(40000).To(45000),
                r => r.From(45000).To(50000),
                r => r.From(50000).To(55000),
                r => r.From(55000).To(60000),
                r => r.From(60000).To(65000),
                r => r.From(65000).To(70000),
                r => r.From(70000).To(75000),
                r => r.From(75000).To(80000),
                r => r.From(80000).To(85000),
                r => r.From(85000).To(90000),
                r => r.From(90000).To(95000),
                r => r.From(95000).To(100000),
                r => r.From(100000)
            };


            var scoreRanges = new List<Func<AggregationRangeDescriptor, IAggregationRange>>()
            {
                r => r.From(0).To(5),
                r => r.From(5).To(10),
                r => r.From(10).To(15),
                r => r.From(15).To(20),
                r => r.From(20).To(25),
                r => r.From(25).To(30),
                r => r.From(30).To(35),
                r => r.From(35).To(40),
                r => r.From(40).To(45),
                r => r.From(45).To(50),
                r => r.From(50).To(55),
                r => r.From(55).To(60),
                r => r.From(60).To(65),
                r => r.From(65).To(70),
                r => r.From(70).To(75),
                r => r.From(75).To(80),
                r => r.From(80).To(85),
                r => r.From(85).To(90),
                r => r.From(90).To(95),
                r => r.From(95).To(100)
            };

            var result = _client.Search<SearchSchool>(searchDesc
                                   .TrackTotalHits(true)
                                   .StoredFields(sf => sf.Field("_source"))
                                   .Size(0)
                                   .Aggregations(aggs => aggs
                                    .Range("cost", ra => ra
                                        .Field(p => p.Tuition)
                                        .Ranges(costRanges.ToArray())
                                    ).Range("compositescore", ra => ra
                                        .Field(p => p.Score.Composite)
                                        .Ranges(scoreRanges.ToArray())
                                    ).Range("teachscore", ra => ra
                                        .Field(p => p.Score.Teach)
                                        .Ranges(scoreRanges.ToArray())
                                    ).Range("hardscore", ra => ra
                                        .Field(p => p.Score.Hard)
                                        .Ranges(scoreRanges.ToArray())
                                    ).Range("coursescore", ra => ra
                                        .Field(p => p.Score.Course)
                                        .Ranges(scoreRanges.ToArray())
                                    ).Range("learnscore", ra => ra
                                        .Field(p => p.Score.Learn)
                                        .Ranges(scoreRanges.ToArray())
                                    ).Range("costscore", ra => ra
                                        .Field(p => p.Score.Cost)
                                        .Ranges(scoreRanges.ToArray())
                                    ).Range("totalscore", ra => ra
                                        .Field(p => p.Score.Total)
                                        .Ranges(scoreRanges.ToArray())
                                    )));
            List<SchoolStatsResult> statsResult = new List<SchoolStatsResult>();

            foreach (var arr in result.Aggregations)
            {
                string arrName = arr.Key;
                List<SchoolHistogram> hist = new List<SchoolHistogram>();
                var indexAggs = (BucketAggregate)arr.Value;
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (RangeBucket)item;
                    hist.Add(new SchoolHistogram
                    {
                        Key = bucket.Key.ToString(),
                        Count = bucket.DocCount,
                        From = bucket.From,
                        To = bucket.To
                    });
                }
                statsResult.Add(new SchoolStatsResult
                {
                    Name = arrName,
                    Hist = hist
                });
            }

            return statsResult;
        }

        public List<SearchSignSchool> SearchSingSchool(string keyword, out long total, int pageNo = 1, int pageSize = 10)
        {
            int from = (pageNo - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchSignSchool>();
            searchDesc = searchDesc.Index(_singSchoolIndexName)
                                    .From(from)
                                    .Size(size);

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSignSchool>, QueryContainer>>();

            if (!string.IsNullOrEmpty(keyword))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Field<SearchSignSchool>(f => f.SchName),
                    //Type = TextQueryType.Phrase,
                    Query = keyword
                };
                mustQuerys.Add(q => query);

                //排除已被删除的数据
                var IsDel = new TermQuery
                {
                    Field = Infer.Field<SearchSignSchool>(p => p.IsDel),
                    Value = false
                };

                mustQuerys.Add(mq => query);
                mustQuerys.Add(mq => IsDel);

                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }



            var result = _client.Search<SearchSignSchool>(searchDesc
            //.Highlight(h => h.Fields(fs => fs.Field(p => p.Title).ForceSource()))
            );
            total = result.Total;

            return result.Hits.Select(q => q.Source).ToList();

        }

        /// <summary>
        /// 搜索点评列表
        /// </summary>
        public List<SearchComment> SearchComment(bool? lodging, string keyword, List<string> keywords, Guid? userId,
            List<int> cityIds, List<int> areaIds, List<int> gradeIds, List<int> typeIds,
            int orderBy, Guid? Eid, out long total, int pageNo = 1, int pageSize = 10)
        {
            int from = (pageNo - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchComment>();
            searchDesc = searchDesc.Index(_commentIndexName)
                                   .From(from)
                                   .Size(size);
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchComment>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchComment>, QueryContainer>>
            {
            };

            keywords = keywords ?? new List<string>();
            if (!keywords.Any() && !string.IsNullOrWhiteSpace(keyword))
            {
                keywords.Add(keyword);
            }
            if (keywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchComment>(
                    keywords,
                    isMatchAllWord: true,
                    Infer.Field("context.pinyin"),
                    Infer.Field("context.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));

                shouldQuerys.Add(s => s.Match(m => m.Field("context").Query(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("context.pinyin").Value(keyword).Boost(2)));
            }
            if (lodging != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchComment>(p => p.Lodging),
                    Value = lodging
                };
                mustQuerys.Add(mq => query);
            }
            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchComment>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }
            if (Eid != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchComment>(p => p.Eid),
                    Value = Eid
                };
                mustQuerys.Add(mq => query);
            }

            if (cityIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchComment>, QueryContainer>>();
                foreach (var city in cityIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.CityCode).Value(city)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }
            if (areaIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchComment>, QueryContainer>>();

                foreach (var area in areaIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.AreaCode).Value(area)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }

            if (gradeIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchComment>, QueryContainer>>();

                foreach (var grade in gradeIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Grade).Value(grade)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }

            if (typeIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchComment>, QueryContainer>>();

                foreach (var type in typeIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Type).Value(type)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }

            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)));
            }

            //排序
            Func<SortDescriptor<SearchComment>, IPromise<IList<ISort>>> sortDesc = sd =>
            {
                //排序
                switch (orderBy)
                {
                    case 1:
                        sd.Descending(d => d.PublishTime);
                        break;
                    case 2:
                        sd.Descending(d => d.Score);
                        break;
                    case 3:
                        sd.Descending(d => d.ReplyCount);
                        break;
                    default:
                        //根据分值排序
                        sd.Descending(SortSpecialField.Score);
                        break;
                }
                return sd;
            };
            searchDesc = searchDesc.Sort(sortDesc);

            var result = _client.Search<SearchComment>(searchDesc
            //.Highlight(h => h.Fields(fs => fs.Field(p => p.Title).ForceSource()))
            );
            total = result.Total;

            return result.Hits.Select(q => q.Source).ToList();
        }
        /// <summary>
        /// 搜索问答列表
        /// </summary>
        public List<SearchQuestion> SearchQuestion(bool? lodging, string keyword, List<string> keywords, Guid? userId,
            List<int> cityIds, List<int> areaIds, List<int> gradeIds, List<int> typeIds,
            int orderBy, Guid? Eid, out long total, int pageNo = 1, int pageSize = 10)
        {
            int from = (pageNo - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchQuestion>();
            searchDesc = searchDesc.Index(_questionIndexName)
                                   .From(from)
                                   .Size(size);
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>
            {
            };

            keywords = keywords ?? new List<string>();
            if (!keywords.Any() && !string.IsNullOrWhiteSpace(keyword))
            {
                keywords.Add(keyword);
            }
            if (keywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchQuestion>(
                    keywords,
                    isMatchAllWord: true,
                    Infer.Field("context.pinyin"),
                    Infer.Field("context.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));

                shouldQuerys.Add(s => s.Match(m => m.Field("context").Query(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("context.pinyin").Value(keyword).Boost(2)));
            }
            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchQuestion>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }
            if (lodging != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchQuestion>(p => p.Lodging),
                    Value = lodging
                };
                mustQuerys.Add(mq => query);
            }
            if (Eid != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchQuestion>(p => p.Eid),
                    Value = Eid
                };
                mustQuerys.Add(mq => query);
            }
            if (cityIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>();
                foreach (var city in cityIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.CityCode).Value(city)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }
            if (areaIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>();

                foreach (var area in areaIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.AreaCode).Value(area)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }

            if (gradeIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>();

                foreach (var grade in gradeIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Grade).Value(grade)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }

            if (typeIds.Count > 0)
            {
                var _shouldQuerys = new List<Func<QueryContainerDescriptor<SearchQuestion>, QueryContainer>>();

                foreach (var type in typeIds)
                {
                    _shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Type).Value(type)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(_shouldQuerys)));
            }

            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)));
            }

            //排序
            Func<SortDescriptor<SearchQuestion>, IPromise<IList<ISort>>> sortDesc = sd =>
            {
                //排序
                switch (orderBy)
                {
                    case 1:
                        sd.Descending(d => d.PublishTime);
                        break;
                    case 3:
                        sd.Descending(d => d.ReplyCount);
                        break;
                    default:
                        //根据分值排序
                        sd.Descending(SortSpecialField.Score);
                        break;
                }
                return sd;
            };
            searchDesc = searchDesc.Sort(sortDesc);

            var result = _client.Search<SearchQuestion>(searchDesc
            //.Query(q => q.Match(mq => mq.Field(p=>p.Title).Query("上海")))
            //.Highlight(h => h.Fields(fs => fs.Field(p => p.Title).ForceSource()))
            );
            total = result.Total;
            return result.Hits.Select(q => q.Source).ToList();
        }
        public List<SearchLive> SearchLive(out long total, SearchLiveQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var userId = queryModel.UserId;
            var orderBy = queryModel.OrderBy;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchLive>();
            searchDesc = searchDesc.Index(_liveIndexName)
                                   .Sort(SearchPUV.GetSortDesc<SearchLive>(orderBy))
                                   .From(from)
                                   .Size(size);
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchLive>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchLive>, QueryContainer>>
            {
            };
            if (queryModel.MustCityCode)
            {
                //mustQuerys.Add(q => q.Term(t => t.Field(f => f.CityCode).Value(queryModel.CityCode)));
                mustQuerys.Add(mm => mm.Bool(b =>
                    b.Should(q =>
                        q.Bool(qs => qs.Must(m => m.Term("cityCode", queryModel.CityCode)))
                        || q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field("cityCode"))))
                )));
            }

            if (queryModel.QueryEsKeywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchLive>(
                    queryModel.QueryEsKeywords,
                    queryModel.IsMatchAllWord,
                    Infer.Field("title.pinyin"),
                    Infer.Field("title.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));

                shouldQuerys.Add(s => s.Match(m => m.Field("title").Query(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("title.pinyin").Value(keyword).Boost(2)));
            }
            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchLive>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }
            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)));
            }

            var result = _client.Search<SearchLive>(searchDesc);

            total = result.Total;
            return result.Hits.Select(q => q.Source).ToList();
        }
        /// <summary>
        /// 搜索排行榜列表
        /// </summary>
        public List<SearchSchoolRank> SearchSchoolRank(out long total, SearchBaseQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var orderBy = queryModel.OrderBy;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSchoolRank>, QueryContainer>>
            {
                q => q.Term(t => t.Field(tf => tf.IsDeleted).Value(false)),
            };


            if (queryModel.MustCityCode)
            {
                //mustQuerys.Add(m => m.Term(t => t.Field(f => f.CityCodes).Value(queryModel.CityCode)));
                mustQuerys.Add(mm => mm.Bool(b =>
                    b.Should(q =>
                        q.Bool(qs => qs.Must(m => m.Term("cityCodes", queryModel.CityCode)))
                        || q.Bool(qs => qs.MustNot(m => m.Exists(tf => tf.Field("cityCodes"))))
                )));
            }

            //模糊查询
            if (queryModel.QueryEsKeywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchSchoolRank>(
                    queryModel.QueryEsKeywords,
                    queryModel.IsMatchAllWord,
                    (input, q) =>
                        q.MultiMatch(m => m.Fields(fd => fd
                            .Field("title.cntext"))
                            .Query(input))
                        || q.Nested(n => n
                            .Path("schools")
                            .Query(nq =>
                                nq.Bool(b => b
                                    .Should(m => m.MultiMatch(mm => mm.Fields(new string[] { "schools.name.cntext", "schools.name" }).Query(input)))
                                    ))
                    ));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
            }

            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchoolRank>, QueryContainer>>
            {
            };
            shouldQuerys.Add(s => s.Match(m => m.Field("title").Query(keyword).Boost(2)));
            shouldQuerys.Add(s => s.Nested(n => n
                        .Path("schools")
                        .Query(nq =>
                            nq.Bool(b => b
                                .Must(m => m.Match(mm => mm.Field("schools.name").Query(keyword)))
                                ))));
            shouldQuerys.Add(s => s.Term(m => m.Field("title.pinyin").Value(keyword).Boost(2)));


            var result = _client.Search<SearchSchoolRank>(s => s
                .Index(_schoolRankIndexName)
                .Source(so => so.IncludeAll().Excludes(se => se.Field(sf => sf.Schools)))
                .Query(q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)))
                .Sort(SearchPUV.GetSortDesc<SearchSchoolRank>(orderBy))
                .From(from)
                .Size(size)
            //.Highlight(h => h.Fields(fs => fs.Field(p => p.Title).ForceSource()))
            );
            total = result.Total;
            return result.Hits.Select(q => new SearchSchoolRank
            {
                Id = q.Source.Id,
                Title = q.Source.Title,
                UpdateTime = q.Source.UpdateTime,
                IsDeleted = q.Source.IsDeleted,
                Schools = q.InnerHits.Any(s => s.Key == "schools")
                    ? q.InnerHits["schools"].Documents<SchoolRankItem>().ToList()
                    : new List<SchoolRankItem>()
            }).ToList();
        }
        /// <summary>
        /// 搜索文章列表
        /// </summary>
        //public List<SearchArticle> SearchArticle(out long total, SearchArticleQueryModel queryModel)
        //{
        //    var keyword = queryModel.Keyword;
        //    var userId = queryModel.UserId;
        //    var orderBy = queryModel.OrderBy;
        //    var pageIndex = queryModel.PageIndex;
        //    var pageSize = queryModel.PageSize;

        //    int from = (pageIndex - 1) * pageSize;
        //    int size = pageSize;

        //    var searchDesc = new SearchDescriptor<SearchArticle>();
        //    searchDesc = searchDesc.Index(_articleIndexName)
        //                           .Sort(SearchPUV.GetSortDesc<SearchArticle>(orderBy))
        //                           .From(from)
        //                           .Size(size);
        //    var mustQuerys = new List<Func<QueryContainerDescriptor<SearchArticle>, QueryContainer>>
        //    {
        //        m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
        //    };
        //    var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchArticle>, QueryContainer>>
        //    {
        //    };

        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        var query = new MultiMatchQuery()
        //        {
        //            //Fields = Infer.Field<SearchArticle>(f => f.Title).And("tags^10"),
        //            Fields = new string[] { "title.pinyin", "tags^10" },
        //            Query = keyword,
        //            Analyzer = "ik_search_analyzer",
        //        };
        //        mustQuerys.Add(q => query);
        //        shouldQuerys.Add(s => s.Match(m => m.Field("title").Query(keyword).Boost(2)));
        //        shouldQuerys.Add(s => s.Term(m => m.Field("title.pinyin").Value(keyword).Boost(2)));
        //    }
        //    if (userId != null)
        //    {
        //        var query = new TermQuery
        //        {
        //            Field = Infer.Field<SearchArticle>(p => p.UserId),
        //            Value = userId
        //        };
        //        mustQuerys.Add(mq => query);
        //    }
        //    if (mustQuerys.Count > 0)
        //    {
        //        searchDesc.Query(q =>
        //            q.FunctionScore(fs => fs.Query(fsq => fsq.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)))
        //            .BoostMode(FunctionBoostMode.Sum)
        //            .Functions(fun =>
        //                fun.GaussDate(gd =>
        //                    gd.Field(f => f.UpdateTime)
        //                    .Origin(DateTime.Now)
        //                    .Scale(TimeSpan.FromDays(1))
        //                    .Offset(TimeSpan.FromDays(30)
        //            )))
        //        ));
        //    }

        //    var result = _client.Search<SearchArticle>(searchDesc);
        //    total = result.Total;
        //    return result.Hits.Select(q => q.Source).ToList();
        //}

        public List<SearchCircle> SearchCircle(out long total, SearchBaseQueryModel queryModel)
        {
            var keyword = queryModel.Keyword;
            var orderBy = queryModel.OrderBy;
            var pageIndex = queryModel.PageIndex;
            var pageSize = queryModel.PageSize;

            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;
            var search = new SearchDescriptor<SearchCircle>()
                .Index(new string[] { _circleIndexName })
                //.Sort(sd => sd.Descending(SortSpecialField.Score).Descending(f => f.FollowCount))
                .Sort(SearchPUV.GetSortDesc<SearchCircle>(orderBy, "modifyTime"))
                .From(from)
                .Size(size)
                .Highlight(h => h
                    .Fields(fs =>
                        fs.Field("userName.pinyin").FragmentSize(9999),
                        fs => fs.Field("name.pinyin").FragmentSize(9999)
                    )
                 );

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchCircle>, QueryContainer>>()
            {
                q=> q.Term(t => t.Field(p => p.IsDeleted).Value(false))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchCircle>, QueryContainer>>
            {
            };

            #region mustQuery

            //模糊查询
            if (queryModel.QueryEsKeywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchCircle>(
                    queryModel.QueryEsKeywords,
                    queryModel.IsMatchAllWord,
                    Infer.Fields("name.cntext", "userName.pinyin"),
                    Infer.Fields("name.cntext", "userName.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));

                shouldQuerys.Add(s => s.Match(m => m.Field("name").Query(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Match(m => m.Field("userName").Query(keyword)));
                shouldQuerys.Add(s => s.Term(m => m.Field("name.pinyin").Value(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("userName.pinyin").Value(keyword).Boost(2)));
            }

            #endregion

            if (mustQuerys.Count != 0)
                //search.Query(q => q.Bool(b => b.Must(mustQuery)));
                search.Query(q => q.FunctionScore(csq => csq.Query(qc => qc.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)))
                    .Functions(sfd =>
                        sfd.FieldValueFactor(fff => fff.Field(f => f.FollowCount)
                        .Factor(1)
                        .Modifier(FieldValueFactorModifier.Log2P)))
                    //.ScoreMode(FunctionScoreMode.Sum)
                    .BoostMode(FunctionBoostMode.Sum)
                ));

            var result = _client.Search<SearchCircle>(search);
            total = result.Total;
            var data = result.Hits.Select(s => s.Source).ToList();
            var highs = result.Hits.Select(s => s.Highlight).ToList();

            HighlightHelper.SetHighlights(ref data, keyword, "Name");
            HighlightHelper.SetHighlights(ref data, keyword, "UserName");

            return data;
        }



        public List<Func<QueryContainerDescriptor<SearchTopic>, QueryContainer>> GetSearchTopicQuery(string keyword, Guid? circleId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime)
        {
            var mustQuery = new List<Func<QueryContainerDescriptor<SearchTopic>, QueryContainer>>() {
                q => q.Term(tq => tq.Field(s => s.IsDeleted).Value(0)),
                q => q.Term(tq => tq.Field(s => s.Status).Value(0))
            };

            #region mustQuery

            if (circleId != null)
                mustQuery.Add(q => q.Term(tq => tq.Field(s => s.CircleId).Value(circleId.Value)));

            if (creator != null)
                mustQuery.Add(q => q.Term(tq => tq.Field(s => s.Creator).Value(creator.Value)));

            if (type != null)
                mustQuery.Add(q => q.Term(tq => tq.Field(s => s.Type).Value(type.Value)));

            if (true == isGood)
                mustQuery.Add(q => q.Term(tq => tq.Field(s => s.IsGood).Value(1)));

            if (true == isQA)
                mustQuery.Add(q => q.Term(tq => tq.Field(s => s.IsQA).Value(1)));

            //标签查询
            if (tags != null && tags.Count != 0)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    var tagId = tags[i];
                    mustQuery.Add(q => q.Nested(nq =>
                        nq.Path(s => s.Tags).Query(qq => qq.Term(s => s.Field("tags.id").Value(tagId))
                    )));
                }

                //多对多,  会搜不出来
                //mustQuery.Add(q => q.Nested(nq =>
                //            nq.Path(s => s.Tags)
                //            .Query(qq => qq.TermsSet(s => s.Field("tags.id").Terms(tags).MinimumShouldMatchScript(ms=>ms.Source("2"))))
                //        ));
            }

            //模糊查询
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Field("content.pinyin"),
                    Query = keyword
                };

                mustQuery.Add(q => query);

                //mustQuery.Add(q => query ||
                //    q.Nested(nq =>
                //        nq.Path(s => s.Tags)
                //        .Query(qq =>
                //            qq.Match(s => s.Field("tags.name.pinyin").Query(keyword))
                //        )));
            }

            //时间查询
            if (startTime != null)
            {
                var query = new DateRangeQuery()
                {
                    Field = Infer.Field<SearchTopic>(f => f.Time),
                    GreaterThanOrEqualTo = startTime
                };
                mustQuery.Add(q => query);
            }
            if (endTime != null)
            {
                var query = new DateRangeQuery()
                {
                    Field = Infer.Field<SearchTopic>(f => f.Time),
                    LessThan = endTime
                };
                mustQuery.Add(q => query);
            }
            #endregion
            return mustQuery;
        }

        public Func<QueryContainerDescriptor<SearchTopic>, QueryContainer> GetSearchTopicFilter(Guid? circleUserId, Guid? loginUserId)
        {
            #region filter
            //未登录只能查公开的话题
            if (loginUserId == null || loginUserId == Guid.Empty)
            {
                return q => q.Bool(b => b.MustNot(qq => qq.Exists(tq => tq.Field(s => s.OpenUserId))));
            }
            //登录后, 可以查看公开的 和 自己的话题
            else if (loginUserId != circleUserId)
            {
                return q =>
                     q.Bool(b =>
                         b.MustNot(qq => qq.Exists(tq => tq.Field(s => s.OpenUserId)))
                         )
                     || q.Bool(b =>
                         b.Must(qq => qq.Term(qt => qt.Field(s => s.Creator).Value(loginUserId.Value)))
                         )
                     ;
            }
            #endregion

            //圈主可以查看自己圈子的所有的话题
            //circleId
            return null;
        }

        public List<SearchTopic> SearchTopic(out long total, string keyword, Guid? circleId, Guid? circleUserId, Guid? loginUserId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime, int pageNo = 1, int pageSize = 10)
        {
            int from = (pageNo - 1) * pageSize;
            int size = pageSize;
            var search = new SearchDescriptor<SearchTopic>()
                .Index(_topicIndexName)
                //加精的, 根据加精时间逆序
                .Sort(sd => isGood == true ? sd.Descending(SortSpecialField.Score).Descending(f => f.GoodTime) : sd.Descending(SortSpecialField.Score).Descending(f => f.Time))
                .From(from)
                .Size(size)
                .Highlight(h => h.Fields(fs => fs.Field("content.pinyin").FragmentSize(9999)))
                ;


            var filter = GetSearchTopicFilter(circleUserId, loginUserId);
            var mustQuery = GetSearchTopicQuery(keyword, circleId, creator, type, isGood, isQA, tags, startTime, endTime);

            Func<QueryContainerDescriptor<SearchTopic>, QueryContainer> query = null;
            if (filter != null)
                query = q => q.Bool(b => b.Must(mustQuery)) && q.Bool(b => b.Filter(filter));
            else
                query = q => q.Bool(b => b.Must(mustQuery));

            //关键词模糊查询时, 额外时间加权排序
            if (!string.IsNullOrWhiteSpace(keyword))
                search.Query(q => q.FunctionScore(f =>
                    f.Query(query)
                    .BoostMode(FunctionBoostMode.Sum)
                    .Functions(fun =>
                        fun.GaussDate(gauss =>
                            gauss.Field(s => s.Time)
                                .Origin(DateTime.Now)
                                .Scale(TimeSpan.FromDays(30))
                                .Decay(0.1)
                                .Weight(10))
                    )));
            else
                search.Query(query);

            var result = _client.Search<SearchTopic>(search);


            total = result.Total;
            var data = result.Hits.Select(s => s.Source).ToList();
            var highs = result.Hits.Select(s => s.Highlight).ToList();

            HighlightHelper.SetHighlights(ref data, highs, "Content");

            return data;
        }

        public List<int> SearchTopicTags(string keyword, Guid? circleId, Guid? circleUserId, Guid? loginUserId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime)
        {
            var search = new SearchDescriptor<SearchTopic>()
                .Index(_topicIndexName)
                .Size(0)
                .Aggregations(aggs => aggs.Nested("aggs_tags", naggs => naggs.Path(topic => topic.Tags).Aggregations(
                    caggs => caggs.Terms("aggs_tags_id", s => s.Field("tags.id").Size(999))
                  )))
                ;

            var filter = GetSearchTopicFilter(circleUserId, loginUserId);
            var mustQuery = GetSearchTopicQuery(keyword, circleId, creator, type, isGood, isQA, tags, startTime, endTime);

            if (mustQuery.Count != 0 && filter != null)
                search.Query(q => q.Bool(b => b.Must(mustQuery)) && q.Bool(b => b.Filter(filter)));
            else if (mustQuery.Count != 0)
                search.Query(q => q.Bool(b => b.Must(mustQuery)));
            else if (filter != null)
                search.Query(q => q.Bool(b => b.Filter(filter)));

            var result = _client.Search<SearchTopic>(search);

            List<int> aggTags = new List<int>();
            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (SingleBucketAggregate)result.Aggregations["aggs_tags"];
                var tagAggs = (BucketAggregate)indexAggs["aggs_tags_id"];
                foreach (var item in tagAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;
                    if (int.TryParse(bucket.Key + "", out int tagId))
                    {
                        aggTags.Add(tagId);
                    }
                }

            }
            return aggTags;
        }


        /// <summary>
        /// 搜索历史
        /// </summary>
        public List<SearchHistory> GetHistorySearch(Guid? UserId, string UUID, int channel, int pageSize)
        {
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchHistory>, QueryContainer>>();

            if (UserId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.UserId),
                    Value = UserId
                };
                mustQuerys.Add(mq => query);
            }
            if (UUID != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.UUID),
                    Value = UUID
                };
                mustQuerys.Add(mq => query);
            }
            //if (channel != 0)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.Channel),
                    Value = channel
                };
                mustQuerys.Add(mq => query);
            }

            var searchDesc = new SearchDescriptor<SearchHistory>();
            searchDesc = searchDesc.Index("historyindex")
                                   .Size(pageSize);
            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }


            var result = _client.Search<SearchHistory>(searchDesc);
            return result.Hits.Select(q => q.Source).ToList();
        }

        /// <summary>
        /// 历史搜索
        /// </summary>
        public List<string> GetDistinctHistorySearch(int channel, Guid? userId, Guid? uuid, int pageSize)
        {
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchHistory>, QueryContainer>>();
            mustQuerys.Add(mq => new TermQuery
            {
                Field = Infer.Field<SearchHistory>(p => p.Channel),
                Value = channel
            });

            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }
            if (uuid != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.UUID),
                    Value = uuid
                };
                mustQuerys.Add(mq => query);
            }

            var searchDesc = new SearchDescriptor<SearchHistory>();
            searchDesc = searchDesc.Index("historyindex")
                                   .StoredFields(sf => sf.Field("_source"))
                                   .Size(0)
                                   .Sort(s => s.Descending(d => d.DateTime))
                                   .Aggregations(aggs => aggs
                                       .Terms("area_aggs", t => t.Field(f => f.Word).Size(pageSize)
                                       )
                                   );

            searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            var result = _client.Search<SearchHistory>(searchDesc);
            List<string> words = new List<string>();

            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["area_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;
                    words.Add(bucket.Key.ToString());
                }
            }
            return words;
        }

        /// <summary>
        /// 热门搜索
        /// </summary>
        public List<string> GetHotSearch(int channel, int pageSize, Guid? userId = null, int? cityCode = null, int min = 10)
        {
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchHistory>, QueryContainer>>();
            if (userId != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.UserId),
                    Value = userId
                };
                mustQuerys.Add(mq => query);
            }
            if (cityCode != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.CityCode),
                    Value = cityCode
                };
                mustQuerys.Add(mq => query);
            }

            if (channel != -1)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchHistory>(p => p.Channel),
                    Value = channel
                };
                mustQuerys.Add(mq => query);
            }
            mustQuerys.Add(mq => new DateRangeQuery
            {
                Boost = 1.1,
                Field = Infer.Field<SearchHistory>(p => p.DateTime),
                GreaterThanOrEqualTo = DateTime.Now.AddDays(-7)
            });

            var searchDesc = new SearchDescriptor<SearchHistory>();
            searchDesc = searchDesc.Index("historyindex")
                                   .StoredFields(sf => sf.Field("_source"))
                                   .Size(0)

                                   .Aggregations(aggs => aggs
                                       .Terms("area_aggs", t => t.Field(f => f.Word).Size(pageSize)
                                       )
                                   );
            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }
            var result = _client.Search<SearchHistory>(searchDesc);

            List<string> words = new List<string>();

            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["area_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;

                    var Count = bucket.DocCount ?? 0;
                    if (Count < min)
                        break;

                    words.Add(bucket.Key.ToString());
                }
            }
            return words;
        }


        public SearchSchoolMap SearchMapSchool(int level, string keyword,
            double swLat, double swLng, double neLat, double neLng, int cityCode,
            List<Guid> metroLineIds, List<int> metroStationIds,
            List<int> grades, List<string> schoolTypeCodes,
            int minCost, int maxCost, int minStudentCount, int maxStudentCount,
             int minTeacherCount, int maxTeacherCount, bool? lodging, bool? canteen,
            List<Guid> authIds, List<Guid> characIds, List<Guid> abroadIds, List<Guid> courseIds)
        {
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };

            if (!string.IsNullOrEmpty(keyword))
            {
                var queryContainer = new QueryContainerDescriptor<SearchSchool>();

                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Field("name.pinyin^5").And("cityarea^2").And("schooltype"),
                    Type = TextQueryType.Phrase,
                    Operator = Operator.Or,
                    Query = keyword
                };
                queryContainer.MultiMatch(q => query);


                var queryTerm = new TermQuery()
                {
                    Field = Infer.Field<SearchSchool>(p => p.Name),
                    Value = keyword,
                    Boost = 10
                };

                mustQuerys.Add(q => q.Bool(b => b.Must(m => queryContainer).Should(s => queryTerm)));
            }

            if (cityCode != 0)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.CityCode),
                    Value = cityCode
                };
                mustQuerys.Add(mq => query);
            }

            if (grades.Count > 0 || schoolTypeCodes.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();

                foreach (var grade in grades)
                {
                    shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Grade).Value(grade)));
                }

                if (schoolTypeCodes.Count > 0)
                {
                    var query = new TermsQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.SchooltypeNewCode),
                        Terms = schoolTypeCodes
                    };
                    shouldQuerys.Add(mq => query);
                }

                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (metroLineIds.Count > 0)
            {
                var query = new TermsQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.MetroLineId),
                    Terms = metroLineIds.Select(q => q.ToString())
                };
                mustQuerys.Add(mq => query);
                if (metroStationIds.Count > 0)
                {
                    var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>();

                    foreach (var metroStationId in metroStationIds)
                    {
                        shouldQuerys.Add(q => q.Term(t => t.Field(f => f.MetroStationId).Value(metroStationId)));
                    }
                    mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
                }
            }

            if (minCost + maxCost > 0)
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Tuition),
                    GreaterThanOrEqualTo = minCost,
                    LessThan = maxCost,
                };
                mustQuerys.Add(mq => query);
            }

            if (minStudentCount + maxStudentCount > 0)
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Studentcount),
                    GreaterThanOrEqualTo = minStudentCount,
                    LessThan = maxStudentCount,
                };
                mustQuerys.Add(mq => query);
            }

            if (minTeacherCount + maxTeacherCount > 0)
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Teachercount),
                    GreaterThanOrEqualTo = minTeacherCount,
                    LessThan = maxTeacherCount,
                };
                mustQuerys.Add(mq => query);
            }

            if (lodging != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Lodging),
                    Value = lodging
                };
                mustQuerys.Add(mq => query);
            }

            if (canteen != null)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Canteen),
                    Value = canteen
                };
                mustQuerys.Add(mq => query);
            }

            if (authIds.Count > 0 || characIds.Count > 0 || abroadIds.Count > 0)
            {
                if (authIds.Count > 0)
                {
                    var query = new TermsQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.Authentication),
                        Terms = authIds.Select(q => q.ToString())
                    };
                    mustQuerys.Add(mq => query);
                }
                if (characIds.Count > 0)
                {
                    var query = new TermQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.Characteristic),
                        Value = characIds.Select(q => q.ToString())
                    };
                    mustQuerys.Add(mq => query);
                }
                if (abroadIds.Count > 0)
                {
                    var query = new TermQuery
                    {
                        Field = Infer.Field<SearchSchool>(p => p.Abroad),
                        Value = abroadIds.Select(q => q.ToString())
                    };
                    mustQuerys.Add(mq => query);
                }
            }

            if (courseIds.Count > 0)
            {
                var query = new TermQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Courses),
                    Value = courseIds.Select(q => q.ToString())
                };
                mustQuerys.Add(mq => query);
            }

            var searchCountDesc = new SearchDescriptor<SearchSchool>();
            if (mustQuerys.Count > 0)
            {
                searchCountDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }
            searchCountDesc = searchCountDesc.Index(_schoolIndexName)
                                   .StoredFields(sf => sf.Field("_source"))
                                   .Size(0);
            var resultCount = _client.Search<SearchSchool>(searchCountDesc).Total;


            //可视范围
            if (level != 1 && Math.Abs(swLat - neLat) > 0 && Math.Abs(swLng - neLng) > 0)
            {
                var query = new GeoBoundingBoxQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Location),
                    BoundingBox = new Nest.BoundingBox
                    {
                        TopLeft = new GeoLocation(neLat, swLng),
                        BottomRight = new GeoLocation(swLat, neLng),
                    },
                    Type = GeoExecution.Indexed,
                    ValidationMethod = GeoValidationMethod.Strict
                };
                mustQuerys.Add(mq => query);
            }

            var searchDesc = new SearchDescriptor<SearchSchool>();
            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }

            if (level == 1)
            {
                searchDesc = searchDesc.Index(_schoolIndexName)
                                   .StoredFields(sf => sf.Field("_source"))
                                   .Size(0)
                                   .Aggregations(aggs => aggs
                                       .Terms("area_aggs", t => t.Field(f => f.AreaCode).Size(20)
                                       )
                                   );

                var result = _client.Search<SearchSchool>(searchDesc);

                List<SearchMap> maps = new List<SearchMap>();

                if (result.Aggregations.Count > 0)
                {
                    var indexAggs = (BucketAggregate)result.Aggregations["area_aggs"];
                    foreach (var item in indexAggs.Items)
                    {
                        var bucket = (KeyedBucket<object>)item;
                        maps.Add(new SearchMap
                        {
                            Name = bucket.Key.ToString(),
                            Count = bucket.DocCount ?? 0
                        });
                    }
                }
                return new SearchSchoolMap
                {
                    Count = result.Total,
                    List = maps
                };
            }
            else
            {
                int size = 1000;
                if (level == 2)
                {
                    size = 2000;
                }

                searchDesc = searchDesc.Index(_schoolIndexName)
                                    .Size(size)
                                    .StoredFields(sf => sf.Field("_source"))
                                    .Source(sf => sf.Includes(q =>
                                        q.Field(p => p.Id).Field(p => p.SchoolId).Field(p => p.Name).Field(p => p.Location))
                                    );
                var result = _client.Search<SearchSchool>(searchDesc);

                return new SearchSchoolMap
                {
                    Count = resultCount,
                    ViewCount = result.Total,
                    List = result.Hits.Select(q => new SearchMap
                    {
                        Id = q.Source.Id,
                        SchoolId = q.Source.SchoolId,
                        Location = q.Source.Location,
                        Name = q.Source.Name
                    }).ToList()
                };
            }
        }



        public List<SearchEESchool> SearchEESchool(string keyword)
        {
            string channelIndex = "allschool";


            var result = _client.Search<SearchEESchool>(s => s
                   .Index(channelIndex)
                   .Size(10)
                   .Query(q =>
                   q.MultiMatch(mm => mm
                          .Fields(mf => mf
                              .Field("name^2").Field("cityarea")
                          )
                          .Query(keyword)
                       )
                   //&&
                   // q.Bool(b => b
                   //         .MustNot(m =>
                   //             m.Term(t => t.Field(tf => tf.Status).Value("已下线"))
                   //         )
                   //     )

                   )
            );
            return result.Hits.Select(q => q.Source).ToList();
        }

        public BTSearchResult BTSchoolSearch(BDSchoolSearch search)
        {
            var searchDesc = new SearchDescriptor<BTSearchSchool>();

            searchDesc = searchDesc.Index(_BTschoolSearchIndexName);
            var mustQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();

            if (!string.IsNullOrEmpty(search.Name))
            {
                var queryContainer = new QueryContainerDescriptor<BTSearchSchool>();

                queryContainer.Match(q => new MatchQuery()
                {
                    Field = Infer.Field("name.pinyin"),
                    Query = search.Name
                });

                var queryTerm = new MatchQuery()
                {
                    Field = Infer.Field<BTSearchSchool>(p => p.Name),
                    Query = search.Name,
                    Boost = 10
                };

                mustQuerys.Add(q => q.Bool(b => b.Must(m => queryContainer).Should(s => queryTerm)));
            }

            if (!string.IsNullOrEmpty(search.Sid))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Field<BTSearchSchool>(f => f.Id),
                    Query = search.Sid
                };
                mustQuerys.Add(q => query);

                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }

            if (search.AuditStatus?.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();
                foreach (var status in search.AuditStatus)
                {
                    shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Status).Value(status)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.CreateTime1 != null && search.CreateTime2 != null)
            {
                mustQuerys.Add(mq => new DateRangeQuery
                {
                    Boost = 1.1,
                    Field = Infer.Field<BTSearchSchool>(p => p.UpdateTime),
                    GreaterThanOrEqualTo = search.CreateTime1,
                    LessThanOrEqualTo = search.CreateTime2
                });
            }

            if (search.EidtorIds?.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();
                foreach (var eidtor in search.EidtorIds)
                {
                    shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Modifier).Value(eidtor)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.AuditorIds?.Count > 0)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();
                foreach (var auditor in search.AuditorIds)
                {
                    shouldQuerys.Add(q => q.Term(t => t.Field(f => f.Creator).Value(auditor)));
                }
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.Grade != null)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();

                shouldQuerys.Add(q => q.Bool(b =>
                    b.Must(m =>
                        m.Nested(n =>
                            n.Path(p => p.SchoolExtDetails)
                               .Query(qq =>
                                   qq.Term("schoolExtDetails.grade", search.Grade))))));
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.Type != null)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();

                shouldQuerys.Add(q => q.Bool(b =>
                    b.Must(m =>
                        m.Nested(n =>
                            n.Path(p => p.SchoolExtDetails)
                               .Query(qq =>
                                   qq.Term("schoolExtDetails.type", search.Type))))));
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.Province != null)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();

                shouldQuerys.Add(q => q.Bool(b =>
                    b.Must(m =>
                        m.Nested(n =>
                            n.Path(p => p.SchoolExtDetails)
                               .Query(qq =>
                                   qq.Term("schoolExtDetails.province", search.Province))))));
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.City != null)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();

                shouldQuerys.Add(q => q.Bool(b =>
                    b.Must(m =>
                        m.Nested(n =>
                            n.Path(p => p.SchoolExtDetails)
                               .Query(qq =>
                                   qq.Term("schoolExtDetails.city", search.City))))));
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (search.Area != null)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<BTSearchSchool>, QueryContainer>>();

                shouldQuerys.Add(q => q.Bool(b =>
                    b.Must(m =>
                        m.Nested(n =>
                            n.Path(p => p.SchoolExtDetails)
                               .Query(qq =>
                                   qq.Term("schoolExtDetails.area", search.Area))))));
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }

            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }

            int from = (search.PageIndex - 1) * search.PageSize;
            int size = search.PageSize;

            BTSearchResult BTresult = new BTSearchResult();

            var result = _client.Search<BTSearchSchool>(searchDesc
                                   .From(from)
                                   .Size(size)
                                   );
            BTresult.Total = result.Total;

            if (BTresult.Total > 0)
            {
                BTresult.Sid = result.Hits.Where(q => q.Score != null).Select(x => x.Source.Id).ToList();
            }

            return BTresult;
        }

        public List<SearchId> SearchIds(out long total, string keyword, ChannelIndex channel, Guid? userId, int pageNo = 1, int pageSize = 10)
        {
            var indices = GetIndex(channel);
            int from = (pageNo - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchId>();
            searchDesc = searchDesc.Index(indices).IndicesBoost(fd =>
            {
                fd.Add(_schoolIndexName, 10);
                fd.Add(_liveIndexName, 5);
                return fd;
            })
            .From(from)
            .Size(size);

            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchId>, QueryContainer>>();
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchId>, QueryContainer>>
            {
                m => m.Bool(b=>b.Should(
                      bs=>bs.Term(t => t.Field("isDeleted").Value(false) )
                     ,
                      bs=>bs.Term(t => t.Field("IsValid").Value(1) )
                      ,
                      bs=>bs.Term(t => t.Field("isValid").Value(1) )
                     ))
            };
            if (!string.IsNullOrEmpty(keyword))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Field("title.pinyin")
                    .And("content.pinyin")
                    .And("context.pinyin")
                    .And("name.pinyin")
                    .And("name.cntext")
                    .And("name.keyword")
                    .And("userName.pinyin"),
                    Query = keyword
                };
                mustQuerys.Add(q => query);

                shouldQuerys.Add(s => s.Match(m => m.Field("title").Query(keyword)));
                shouldQuerys.Add(s => s.Match(m => m.Field("content").Query(keyword)));
                shouldQuerys.Add(s => s.Match(m => m.Field("name").Query(keyword)));
                shouldQuerys.Add(s => s.Match(m => m.Field("userName").Query(keyword)));
                shouldQuerys.Add(s => s.Term(m => m.Field("title").Value(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("content").Value(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("name").Value(keyword).Boost(2)));
                shouldQuerys.Add(s => s.Term(m => m.Field("userName").Value(keyword).Boost(2)));
            }

            if (userId != null)
            {

                shouldQuerys.Add(q => q.Term(t => t.Field(f => f.UserId).Value(userId)));
                shouldQuerys.Add(q => q.Term(t => t.Field(f => f.UserId).Value(userId.ToString().ToUpper())));
            }

            if (mustQuerys.Any())
            {
                if (shouldQuerys.Any())
                {
                    mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
                }
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }

            var result = _client.Search<SearchId>(searchDesc);

            total = result.Total;
            return result.Hits.Select(q => new SearchId
            {
                Channel = GetChannel(q.Index),
                Id = q.Source.Id,
                UserId = q.Source.UserId,
            }).ToList();
        }

        public List<SearchAll> SearchAll(string keyword, int searchType, Guid? userId, out long total, int pageNo = 1, int pageSize = 10)
        {

            string channelIndex = "";
            switch (searchType)
            {
                case 1:
                    channelIndex = _commentIndexName;
                    break;
                case 2:
                    channelIndex = _questionIndexName;
                    break;
                case 3:
                    channelIndex = _articleIndexName;
                    break;
                case 4:
                    channelIndex = _liveIndexName;
                    break;
                case 5:
                    channelIndex = _answerIndexName;
                    break;
                default:
                    channelIndex = _liveIndexName + "," + _commentIndexName + "," + _questionIndexName + "," + _articleIndexName + "," + _answerIndexName;
                    break;
            }


            int from = (pageNo - 1) * pageSize;
            int size = pageSize;

            var searchDesc = new SearchDescriptor<SearchAll>();
            searchDesc = searchDesc.Index(channelIndex)
                                   .From(from)
                                   .Size(size);
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchAll>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false))
            };
            if (!string.IsNullOrEmpty(keyword))
            {
                var query = new MultiMatchQuery()
                {
                    Fields = Infer.Field<SearchAll>(f => f.Title).And("context"),
                    Query = keyword
                };
                mustQuerys.Add(q => query);
            }
            if (userId != null)
            {
                var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchAll>, QueryContainer>>();
                shouldQuerys.Add(q => q.Term(t => t.Field(f => f.UserId).Value(userId)));
                shouldQuerys.Add(q => q.Term(t => t.Field(f => f.UserId).Value(userId.ToString().ToUpper())));
                mustQuerys.Add(mq => mq.Bool(b => b.Should(shouldQuerys)));
            }
            if (mustQuerys.Count > 0)
            {
                searchDesc.Query(q => q.Bool(b => b.Must(mustQuerys)));
            }

            var result = _client.Search<SearchAll>(searchDesc);

            total = result.Total;
            return result.Hits.Select(q => new SearchAll
            {
                Type = GetSearchType(q.Index),
                Id = q.Source.Id,
                UserId = q.Source.UserId,
                Context = q.Source.Context,
                Title = q.Source.Title,
                UpdateTime = q.Source.UpdateTime,
                IsDeleted = q.Source.IsDeleted
            }).ToList();
        }

        public int GetSearchType(string indexName)
        {
            if (indexName.StartsWith("commentindex"))
            {
                return 1;
            }
            else if (indexName.StartsWith("questionindex"))
            {
                return 3;
            }
            else if (indexName.StartsWith("articleindex"))
            {
                return 4;
            }
            else if (indexName.StartsWith("answerindex"))
            {
                return 2;
            }
            else if (indexName.StartsWith("liveindex"))
            {
                return 5;
            }
            return 0;
        }

        //批量获取ES里的文章ID
        public (List<Guid>, string scrollid) GetArticleIds(string scrollid)
        {
            List<Guid> ids = new List<Guid>();
            if (string.IsNullOrWhiteSpace(scrollid))
            {//todo:5.x 多了Slice设置 移除SearchType.Scan
                var result = _client.Search<SearchArticle>(s => s.Index(_articleIndexName).Query(q => q.MatchAll())
                    .Size(1000)
                    .Sort(st => st.Descending(ds => ds.Id))
                    .Scroll("20s")
                    //id从0开始 0,1,2...
                    //length=max
                    //例：max=3 id=0,id=1,id=2
                    //.Slice(sl => sl.Id(0).Max(3))
                    );
                var total = result.Total;
                ids = result.Documents.Select(q => q.Id).ToList();
                scrollid = result.ScrollId;
            }
            else
            {
                var result = _client.Scroll<SearchArticle>("20s", scrollid);
                ids = result.Documents.Select(q => q.Id).ToList();
                scrollid = result.ScrollId;
            }
            return (ids, scrollid);
        }

        /// <summary>
        /// 中职学校
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public PageData SearchSvsSchool(SvsSchoolPageListQuery query)
        {
            var mustQuery = new List<Func<QueryContainerDescriptor<SearchSvsSchool>, QueryContainer>>() {
                q => q.Term(t => t.Field("name.pinyin").Value(query.SearchTxt).Boost(2)),
                q => q.Term(tq => tq.Field(s => s.IsDeleted).Value(false))
            };

            #region mustQuery
            //省份
            if (query.Pr != null)
            {
                foreach (var item in query.Pr)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.Province).Value(item)));
                }
            }
            //城市
            if (query.Ci != null)
            {
                foreach (var item in query.Ci)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.City).Value(item)));
                }
            }
            //专业
            if (query.Ma != null)
            {
                foreach (var item in query.Ma)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.MajorNos).Value(item)));
                }
            }
            //2级专业
            if (query.Mas != null)
            {
                foreach (var item in query.Mas)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.MajorSubNos).Value(item)));
                }
            }
            //类型
            if (query.Ty != null)
            {
                foreach (var item in query.Ty)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.Type).Value(item)));
                }
            }
            //性质
            if (query.Na != null)
            {
                foreach (var item in query.Na)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.Nature).Value(item)));
                }
            }
            //等级
            if (query.Lv != null)
            {
                foreach (var item in query.Lv)
                {
                    mustQuery.Add(q => q.Term(tq => tq.Field(s => s.Level).Value(item)));
                }
            }
            #endregion
            var result = _client.Search<SearchSvsSchool>(s => s
                .Index(_svsSchoolIndexName)
                .From((query.PageIndex - 1) * query.PageSize)
                .Size(query.PageSize)
                .Query(q =>
                q.Bool(b => b.Must(m => m.MultiMatch(mm => mm
                            .Fields(mf => mf
                                .Field("name.pinyin^1.1")
                            )
                            .Query(query.SearchTxt)
                        )).Must(mustQuery)
                    )
                )
            );
            var res = new PageData();
            res.Total = result.Total;
            res.PageList = result.Hits.Select(q => q.Source).ToList();
            return res;
        }

        public List<SchoolResult> SearchSchoolVagueKey(SearchSchoolQueryModel queryData, out long total)
        {
            throw new NotImplementedException();
        }
    }
}
