using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
//using PMS.Search.Elasticsearch.Common;
using ProductManagement.Framework.SearchAccessor;

namespace PMS.Search.Elasticsearch
{
    public class DataImport : IDataImport
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        public DataImport(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value;

            if (_esConfig.Indices != null)
            {
                _schoolIndexName = _esConfig.Indices.School.Name;
                _schoolIndexAliasName = _esConfig.Indices.School.Alias;
                _topicIndexName = _esConfig.Indices.Topic.Name;
                _circleIndexName = _esConfig.Indices.Circle.Name;
                _circleIndexAliasName = _esConfig.Indices.Circle.Alias;
            }
        }


        private readonly string _schoolIndexName = "schoolindex_09";
        private readonly string _schoolIndexAliasName = "school";
        private readonly string _schoolRankIndexName = "schoolrankindex";
        private readonly string _commentIndexName = "commentindex_03";
        private readonly string _questionIndexName = "questionindex_03";
        private readonly string _answerIndexName = "answerindex_02";
        private readonly string _articleIndexName = "articleindex_06";
        private readonly string _liveIndexName = "liveindex_01";
        private readonly string _singSchoolIndexName = "signschoolindex";
        private readonly string _BTschoolSearchIndexName = "bdschoolsearchindex";
        private readonly string _topicIndexName = "topicindex";
        private readonly string _circleIndexName = "circleindex";
        private readonly string _circleIndexAliasName = "circle";
        private readonly string _svsSchoolIndexName = "svsschoolindex01";
        private readonly string _courseIndexName = "courseindex";
        private readonly string _organizationIndexName = "organizationindex";
        private readonly string _evaluationIndexName = "evaluationindex";

        /// <summary>
        /// 获取最后更新时间
        /// </summary>
        public List<SearchLastUpdateTime> GetLastUpdate(string indexName = null)
        {
            List<SearchLastUpdateTime> lastUpdateTimes = new List<SearchLastUpdateTime> {
                new SearchLastUpdateTime{
                    Name = _schoolIndexName,UpdateTime = DateTime.MinValue,
                },
                new SearchLastUpdateTime{
                    Name = _commentIndexName,UpdateTime = DateTime.MinValue,
                },
                new SearchLastUpdateTime{
                    Name = _questionIndexName,UpdateTime = DateTime.MinValue,
                },
                new SearchLastUpdateTime{
                    Name = _schoolRankIndexName,UpdateTime = DateTime.MinValue,
                },
                new SearchLastUpdateTime{
                    Name = _articleIndexName,UpdateTime = DateTime.MinValue,
                },
                new SearchLastUpdateTime{
                    Name = _BTschoolSearchIndexName,UpdateTime = DateTime.MinValue,
                }
            };
            var channelIndex = _schoolIndexName + "," + _commentIndexName + "," + _questionIndexName + "," + _schoolRankIndexName + "," + _articleIndexName + "," + _BTschoolSearchIndexName;

            if (!string.IsNullOrWhiteSpace(indexName))
            {
                if (!lastUpdateTimes.Any(q => q.Name == indexName))
                {
                    lastUpdateTimes.Add(new SearchLastUpdateTime
                    {
                        Name = indexName,
                        UpdateTime = DateTime.MinValue,
                    });
                    channelIndex = indexName;
                }
            }

            var result = _client.Search<SearchSchool>(s => s
                   .Index(channelIndex)
                   .Size(0)
                   .Aggregations(aggs => aggs
                       .Terms("index_aggs", t => t.Field("_index").Size(5)
                           .Aggregations(indexaggs => indexaggs.TopHits("top_hits_aggs", th =>
                                th.Source(sf => sf.Includes(q => q.Field(p => p.UpdateTime)))
                                .Sort(so => so.Field(ss => ss.UpdateTime, SortOrder.Descending)).Size(1)))
                       )
                   )
            );
            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["index_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;
                    var source = ((TopHitsAggregate)bucket["top_hits_aggs"]).Hits<SearchLastUpdateTime>().Select(q => q.Source).ToList();
                    lastUpdateTimes.FirstOrDefault(q => q.Name == (string)bucket.Key).UpdateTime = source[0].UpdateTime;
                }
            }
            return lastUpdateTimes;
        }

        public void CreateSchoolIndex()
        {

            var createIndexResponse = _client.Indices.Create(_schoolIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                //取首字母缩写，拼音全连接，每个字拼音，保留原输入，小写的非中文字母，删除重复项
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_keyword_filter",
                                //取首字母缩写，拼音全连接，保留原输入，小写的非中文字母，删除重复项
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_only_filter",
                                //取首字母缩写，拼音全连接，不保留原输入，小写的非中文字母，删除重复项
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = false,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                //取首字母缩写，拼音全连接，保留原输入，小写的非中文字母，删除重复项
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepNoneChineseTogether = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .EdgeNGram("edge_ngram_filter",
                                etf => etf.MinGram(1).MaxGram(50)
                            )
                        )
                        .Tokenizers(t => t.EdgeNGram("ngram_tokenizer", nt => nt
                               .MinGram(1).MaxGram(6)
                               .TokenChars(TokenChar.Letter, TokenChar.Digit)
                            )
                        )
                        .Analyzers(aa => aa
                            //不分词，仅转小写
                            .Custom("string_lowercase", ca => ca
                                 .Tokenizer("keyword")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("edge_ngram_filter","lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("edge_ngram_filter", "pinyin_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_only_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_only_filter", "edge_ngram_filter",  "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                            .Custom("ngram_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ngram_tokenizer")
                                 .Filters("lowercase")
                             )
                            .Custom("ngram_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ngram_tokenizer")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                            //不分词，转拼音，EdgeNgram分割
                            .Custom("keyword_pinyin_ngram_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("keyword")
                                 .Filters("pinyin_filter", "edge_ngram_filter","lowercase")
                             )
                            //不分词，转小写，繁转中
                            .Custom("keyword_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("keyword")
                                 .Filters("lowercase")
                             )
                            //不分词，转拼音，拼音首字母
                            .Custom("keyword_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("keyword")
                                 .Filters("pinyin_keyword_filter", "edge_ngram_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchSchool>(mm => mm
                .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.SchoolId)
                        )
                        .Text(t => t
                            .Name(n => n.Name)
                            .Fields(f => f
                                .Text(ft
                                    => ft.Name("cntext")
                                    .Store(false)
                                    //.TermVector(TermVectorOption.WithPositionsOffsets)
                                    .Analyzer("ik_analyzer")
                                    .SearchAnalyzer("ik_search_analyzer"))
                                .Text(ft
                                    => ft.Name("pinyin")
                                    .Store(false)
                                    //.TermVector(TermVectorOption.WithPositionsOffsets)
                                    .Analyzer("ik_pinyin_only_analyzer")
                                    .SearchAnalyzer("ik_search_analyzer"))
                                .Text(ft
                                    => ft.Name("keyword")
                                    .Store(false)
                                    //.TermVector(TermVectorOption.WithPositionsOffsets)
                                    .Analyzer("keyword_pinyin_ngram_analyzer")
                                    .SearchAnalyzer("ik_search_analyzer"))
                            )
                        )
                        .GeoPoint(t => t
                            .Name(n => n.Location)
                        )
                        .Text(t => t
                            .Name(n => n.City)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Text(t => t
                            .Name(n => n.Area)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                        .Text(t => t
                            .Name(n => n.Cityarea)
                            //.Store(true)
                            .Fields(f => f
                                .Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_search_analyzer")
                                  )
                                  .Text(ft
                                  => ft.Name("cntext")
                                  .Store(false)
                                  .Analyzer("ik_analyzer")
                                  .SearchAnalyzer("ik_search_analyzer")
                                )
                            )
                        )
                        .Number(t => t.Name(n => n.Grade))
                        .Text(t => t
                            .Name(n => n.Schooltype)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_search_analyzer")
                                  )
                                  .Text(ft
                                  => ft.Name("cntext")
                                  .Store(false)
                                  .Analyzer("ik_analyzer")
                                  .SearchAnalyzer("ik_search_analyzer")
                                )
                            )
                        )
                        .Keyword(t => t
                            .Name(n => n.SchooltypeCode)
                        )
                        .Keyword(t => t
                            .Name(n => n.MetroLineId).Fields(ff => ff.Keyword(fk => fk.Name("keyword")))
                        )
                        .Keyword(t => t.Name(n => n.MetroStationId).Fields(ff => ff.Number(fk => fk.Name("keyword"))))
                        .Boolean(t => t.Name(n => n.Canteen))
                        .Boolean(t => t.Name(n => n.Lodging))
                        .Number(t => t.Name(n => n.Studentcount))
                        .Number(t => t.Name(n => n.Teachercount))
                        .Number(t => t.Name(n => n.Star))
                        .Number(t => t.Name(n => n.Commentcount))
                        .Number(t => t.Name(n => n.Questioncount))
                        .Object<SearchSchoolScore>(t => t.Name(n => n.Score)
                            .Properties(pp => pp.Number(fk => fk.Name(n => n.Total))
                                            .Number(fk => fk.Name(n => n.Composite))
                                            .Number(fk => fk.Name(n => n.Cost))
                                            .Number(fk => fk.Name(n => n.Course))
                                            .Number(fk => fk.Name(n => n.Hard))
                                            .Number(fk => fk.Name(n => n.Learn))
                                            .Number(fk => fk.Name(n => n.Teach))
                                            .Number(fk => fk.Name(n => n.Envir))
                            )
                        )
                        .Keyword(t => t.Name(n => n.Authentication).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Characteristic).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Abroad).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Courses).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Text(t => t.Name(n => n.Tags)
                            .Fields(ff => ff
                                .Text(fk => fk.Name("keyword").Analyzer("string_lowercase").SearchAnalyzer("keyword_search_analyzer"))
                                .Text(fk => fk.Name("pinyin").Analyzer("ik_pinyin_analyzer").SearchAnalyzer("ik_search_analyzer"))
                            )
                        )
                        .Number(t => t.Name(n => n.Tuition))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    ))
            );
        }
        public void CreateCommentIndex()
        {
            var createIndexResponse = _client.Indices.Create(_commentIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                        )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchComment>(mm => mm
                 .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.Eid)
                        )
                        .Keyword(t => t
                            .Name(n => n.UserId)
                        )
                        .Text(t => t
                            .Name(n => n.Context)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Number(t => t.Name(n => n.ReplyCount))
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                        .Number(t => t.Name(n => n.Grade))
                        .Number(t => t.Name(n => n.Type))
                        .Number(t => t.Name(n => n.Score))
                        .Boolean(t => t.Name(n => n.Lodging))
                        .Date(t => t.Name(n => n.PublishTime))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                )
            );
        }
        public void CreateQuestionIndex()
        {
            var createIndexResponse = _client.Indices.Create(_questionIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchQuestion>(mm => mm
               .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.Eid)
                        )
                        .Keyword(t => t
                            .Name(n => n.UserId)
                        )
                        .Text(t => t
                            .Name(n => n.Context)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Number(t => t.Name(n => n.ReplyCount))
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                        .Number(t => t.Name(n => n.Grade))
                        .Number(t => t.Name(n => n.Type))
                        .Boolean(t => t.Name(n => n.Lodging))
                        .Date(t => t.Name(n => n.PublishTime))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                )
            );

        }
        public void CreateAnwserIndex()
        {
            var createIndexResponse = _client.Indices.Create(_answerIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchAnswer>(mm => mm
               .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.QuestionId)
                        )
                        .Keyword(t => t
                            .Name(n => n.UserId)
                        )
                        .Text(t => t
                            .Name(n => n.Context)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Date(t => t.Name(n => n.PublishTime))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                )
            );

        }
        public void CreateSchoolRankIndex()
        {
            var createIndexResponse = _client.Indices.Create(_schoolRankIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchSchoolRank>(mm => mm
                 .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Text(t => t
                            .Name(n => n.Title)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Nested<SchoolRankItem>(t => t
                            .Name(n => n.Schools)
                            .Properties(np => np
                                .Number(tt => tt.Name(tn => tn.RankNo))
                                .Text(tt => tt
                                    .Name(tn => tn.Name)
                                    .Fields(f => f.Text(ft
                                          => ft.Name("pinyin")
                                          .Store(false)
                                          .TermVector(TermVectorOption.WithPositionsOffsets)
                                          .Analyzer("ik_pinyin_analyzer")
                                          .SearchAnalyzer("ik_analyzer")
                                          .Boost(5))
                            ))))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                )
            );

        }
        public void CreateArticleIndex()
        {
            var createIndexResponse = _client.Indices.Create(_articleIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("string_lowercase", ca => ca
                                 .Tokenizer("keyword")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchArticle>(mm => mm
               .Dynamic(false)
                   .Properties(p => p
                       .Keyword(t => t
                           .Name(n => n.Id)
                       )
                       .Keyword(t => t
                            .Name(n => n.UserId)
                        )
                       .Text(t => t
                           .Name(n => n.Title)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Text(t => t.Name(n => n.Tags).Fields(ff => ff.Text(fk => fk.Name("keyword").Analyzer("string_lowercase"))))
                       .Date(t => t.Name(n => n.UpdateTime))
                       .Boolean(t => t.Name(n => n.IsDeleted))
                   )
               )
            );

        }
        public void CreateLiveIndex()
        {
            var createIndexResponse = _client.Indices.Create(_liveIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchLive>(mm => mm
               .Dynamic(false)
                   .Properties(p => p
                       .Keyword(t => t
                           .Name(n => n.Id)
                       )
                       .Text(t => t
                           .Name(n => n.Title)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Keyword(t => t.Name(n => n.TypeIds).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                       .Text(t => t
                            .Name(n => n.City)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Text(t => t
                            .Name(n => n.Area)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Keyword(t => t
                           .Name(n => n.LectorId)
                        )
                        .Text(t => t
                            .Name(n => n.LectorName)
                            .Analyzer("ik_analyzer")
                            .SearchAnalyzer("ik_search_analyzer")
                        )
                        .Keyword(t => t
                           .Name(n => n.UserId)
                        )
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                       .Date(t => t.Name(n => n.UpdateTime))
                       .Boolean(t => t.Name(n => n.IsDeleted))
                   )
               )
            );
        }
        public void CreateSchoolCountIndex()
        {
            var createIndexResponse = _client.Indices.Create("schoolcountindex", c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "word_delimiter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("word_delimiter", "lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "word_delimiter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "word_delimiter", "lowercase")
                             )
                        )
                ))
            );
        }

        public void CreateTopicIndex()
        {
            var createIndexResponse = _client.Indices.Create(_topicIndexName, c => c
                //.Aliases(ad => ad.Alias(_topicIndexName))
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchTopic>(mm => mm
               .Dynamic(false)
                   .Properties(p => p
                       .Keyword(t => t.Name(n => n.Id))
                       .Keyword(t => t.Name(n => n.CircleId))
                       .Keyword(t => t.Name(n => n.OpenUserId))
                       .Number(t => t.Name(n => n.Type))
                       .Number(t => t.Name(n => n.Status))
                       .Number(t => t.Name(n => n.IsQA))
                       .Number(t => t.Name(n => n.TopType))
                       .Date(t => t.Name(n => n.TopTime))
                       .Number(t => t.Name(n => n.IsGood))
                       .Date(t => t.Name(n => n.GoodTime))
                       .Number(t => t.Name(n => n.LikeCount))
                       .Number(t => t.Name(n => n.FollowCount))
                       .Number(t => t.Name(n => n.ReplyCount))
                       .Nested<SearchTopic.SearchTag>(np =>
                            np.Name(n => n.Tags)
                            .Properties(tp =>
                                tp.Number(t => t.Name(n => n.Id))
                               .Text(t => t
                                    .Name(n => n.Name)
                                    .Fields(ff =>
                                        ff.Text(fk => fk
                                             .Name("keyword")
                                             .Analyzer("ik_pinyin_analyzer")
                                             .SearchAnalyzer("ik_pinyin_search_analyzer")
                           )))))
                       .Keyword(t => t.Name(n => n.Creator))
                       .Date(t => t.Name(n => n.CreateTime))
                       .Date(t => t.Name(n => n.LastReplyTime))
                       .Date(t => t.Name(n => n.LastEditTime))
                       .Date(t => t.Name(n => n.Time))
                       .Number(t => t.Name(n => n.IsDeleted))
                       .Text(t => t
                           .Name(n => n.Content)
                           .Fields(f => f.Text(ft
                                 => ft.Name("keyword")
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_pinyin_search_analyzer")
                                 .Boost(5))
                           )
                       )
                   )
               )
            );

        }

        public void CreateCircleIndex()
        {
            var createIndexResponse = _client.Indices.Create(_circleIndexName, c => c
                .Aliases(ad => ad.Alias(_circleIndexAliasName))
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                        )
                ))
                .Map<SearchCircle>(mm => mm
               .Dynamic(false)
                   .Properties(p => p
                       .Keyword(t => t.Name(n => n.Id))
                       .Keyword(t => t.Name(n => n.UserId))
                       .Date(t => t.Name(n => n.CreateTime))
                       .Date(t => t.Name(n => n.ModifyTime))
                       .Boolean(t => t.Name(n => n.IsDisable))
                       .Boolean(t => t.Name(n => n.IsDeleted))
                       .Text(t => t
                           .Name(n => n.Name)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Text(t => t
                           .Name(n => n.UserName)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Text(t => t
                           .Name(n => n.Intro)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                   )
               )
            );

        }
        
        public void CreateSvsSchoolIndex()
        {
            var createIndexResponse = _client.Indices.Create(_svsSchoolIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepFirstLetter = true,//分词首字母
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                        )
                ))
                .Map<SearchSvsSchool>(mm => mm
                .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.SId)
                        )
                        .Text(t => t
                            .Name(n => n.Name)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Number(t => t.Name(n => n.No))
                        .Text(t => t.Name(n => n.Logo))
                        .Text(t => t.Name(n => n.Tel))
                        .Number(t => t .Name(n => n.Province))
                        .Number(t => t.Name(n => n.City))
                        .Number(t => t.Name(n => n.Type))
                        .Number(t => t.Name(n => n.Level))
                        .Number(t => t.Name(n => n.Nature))
                        .Text(t => t.Name(n => n.Address))
                        .Text(t => t.Name(n => n.HotMajors))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                        .Keyword(t => t.Name(n => n.MajorSubNos).Fields(ff => ff.Number(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.MajorNos).Fields(ff => ff.Number(fk => fk.Name("keyword"))))
                    ))
            );
        }

        /// <summary>
        /// 课程
        /// </summary>
        public void CreateCourseIndex()
        {
            var createIndexResponse = _client.Indices.Create(_courseIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepFirstLetter = true,//分词首字母
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                        )
                ))
                .Map<SearchCourse>(mm => mm
                .Properties(p => p
                        .Keyword(t => t.Name(n => n.Id))
                        .Number(t => t.Name(n => n.No))
                    .Text(t => t.Name(n => n.Title)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Text(t => t.Name(n => n.Subtitle)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Number(t => t.Name(n => n.Status))
                    .Number(t => t.Name(n => n.Price))
                    .Keyword(t => t.Name(n => n.Mode).Fields(ff => ff.Number(fk => fk.Name("text"))))
                    .Number(t => t.Name(n => n.Subject))
                    .Number(t => t.Name(n => n.MinAge))
                    .Number(t => t.Name(n => n.MaxAge))
                    .Keyword(t => t.Name(n => n.OrgId))
                    .Number(t => t.Name(n => n.OrgNo))
                    .Text(t => t.Name(n => n.OrgName)
                        .Fields(f => f.Text(ft
                                    => ft.Name("pinyin")
                                    .Store(false)
                                    .TermVector(TermVectorOption.WithPositionsOffsets)
                                    .Analyzer("ik_pinyin_analyzer")
                                    .SearchAnalyzer("ik_analyzer")
                                    .Boost(5))
                        )
                    )
                    .Number(t => t.Name(n => n.Authentication))
                    .Text(t => t.Name(n => n.Desc))
                    .Text(t => t.Name(n => n.SubDesc))
                    .Number(t => t.Name(n => n.OrgStatus))
                    .Date(t => t.Name(n => n.ModifyDateTime))
                    .Number(t => t.Name(n => n.IsValid))
                    .Number(t => t.Name(n => n.OrgIsValid))
                    ))
            );
        }

        /// <summary>
        /// 机构
        /// </summary>
        public void CreateOrganizationIndex()
        {
            var createIndexResponse = _client.Indices.Create(_organizationIndexName, c => c
            #region
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepFirstLetter = true,//分词首字母
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                        )
                ))
            #endregion
                .Map<SearchOrganization>(mm => mm
                .Properties(p => p
                    .Keyword(t => t.Name(n => n.Id))
                    .Number(t => t.Name(n => n.No))
                    .Text(t => t.Name(n => n.Name)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Number(t => t.Name(n => n.Authentication))
                    .Number(t => t.Name(n => n.Status))
                    .Text(t => t.Name(n => n.Desc))
                    .Text(t => t.Name(n => n.SubDesc))
                    .Number(t => t.Name(n => n.MinAge))
                    .Number(t => t.Name(n => n.MaxAge))
                    .Keyword(t => t.Name(n => n.Types).Fields(ff => ff.Number(fk => fk.Name("number"))))
                    .Keyword(t => t.Name(n => n.Modes).Fields(ff => ff.Number(fk => fk.Name("number"))))
                    .Keyword(t => t.Name(n => n.Subjects).Fields(ff => ff.Number(fk => fk.Name("number"))))
                    .Date(t => t.Name(n => n.ModifyDateTime))
                    .Number(t => t.Name(n => n.IsValid))
                    ))
            );
        }

        /// <summary>
        /// 评测
        /// </summary>
        public void CreateEvaluationIndex()
        {
            var createIndexResponse = _client.Indices.Create(_evaluationIndexName, c => c
            #region
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepFirstLetter = true,//分词首字母
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                        )
                ))
            #endregion
                .Map<SearchEvaluation>(mm => mm
                .Properties(p => p
                    .Keyword(t => t.Name(n => n.Id))
                    .Number(t => t.Name(n => n.No))
                    .Text(t => t.Name(n => n.Title)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Keyword(t => t.Name(n => n.Mode))
                    .Number(t => t.Name(n => n.IsPlaintext))
                    .Number(t => t.Name(n => n.Stick))
                    .Number(t => t.Name(n => n.Status))
                    .Number(t => t.Name(n => n.IsOfficial))
                    .Keyword(t => t.Name(n => n.SpecialId))
                    .Number(t => t.Name(n => n.SpecialNo))
                    .Text(t => t.Name(n => n.SpecialName)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Number(t => t.Name(n => n.SpecialStatus))
                    .Keyword(t => t.Name(n => n.OrgId))
                    .Number(t => t.Name(n => n.OrgNo))
                    .Text(t => t.Name(n => n.OrgName)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                        )
                    .Number(t => t.Name(n => n.OrgIsAuthenticated))
                    .Text(t => t.Name(n => n.OrgDesc)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Text(t => t.Name(n => n.OrgSubdesc)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Number(t => t.Name(n => n.OrgStatus))
                    .Number(t => t.Name(n => n.CourseNo))
                    .Keyword(t => t.Name(n => n.CourseId))
                    .Number(t => t.Name(n => n.CourseNo))
                    .Text(t => t.Name(n => n.CourseName)
                        .Fields(f => f.Text(ft
                                        => ft.Name("pinyin")
                                        .Store(false)
                                        .TermVector(TermVectorOption.WithPositionsOffsets)
                                        .Analyzer("ik_pinyin_analyzer")
                                        .SearchAnalyzer("ik_analyzer")
                                        .Boost(5))
                            )
                    )
                    .Number(t => t.Name(n => n.Price))
                    .Keyword(t => t.Name(n => n.CourseMode).Fields(ff => ff.Number(fk => fk.Name("text"))))
                    .Number(t => t.Name(n => n.MinAge))
                    .Number(t => t.Name(n => n.MaxAge))
                    .Number(t => t.Name(n => n.Subject))
                    .Date(t => t.Name(n => n.ModifyDateTime))
                    .Number(t => t.Name(n => n.IsValid))
                    .Number(t => t.Name(n => n.SpecialIsValid))
                    .Number(t => t.Name(n => n.OrgIsValid))
                    .Number(t => t.Name(n => n.CourseIsValid))
                    ))
            );
        }
        #region PutMapping
        public void PutMappingSchool()
        {
            var mapping = _client.Indices.PutMapping<SearchSchool>(mm => mm.Index(_schoolIndexName)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.SchoolId)
                        )
                        .Text(t => t
                            .Name(n => n.Name)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .GeoPoint(t => t
                            .Name(n => n.Location)
                        )
                        .Text(t => t
                            .Name(n => n.City)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Text(t => t
                            .Name(n => n.Area)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                        .Text(t => t
                            .Name(n => n.Cityarea)
                            //.Store(true)
                            .Fields(f => f
                                .Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("keyword_pinyin_search_analyzer")
                                  )
                                  .Text(ft
                                  => ft.Name("cntext")
                                  .Store(false)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("keyword_search_analyzer")
                                )
                            )
                        )
                        .Number(t => t.Name(n => n.Grade))
                        .Text(t => t
                            .Name(n => n.Schooltype)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .Analyzer("ngram_pinyin_analyzer")
                                  .SearchAnalyzer("keyword_pinyin_search_analyzer")
                                  )
                                  .Text(ft
                                  => ft.Name("cntext")
                                  .Store(false)
                                  .Analyzer("ngram_analyzer")
                                  .SearchAnalyzer("keyword_search_analyzer")
                                )
                            )
                        )
                        .Keyword(t => t
                            .Name(n => n.SchooltypeCode)
                        )
                        .Keyword(t => t
                            .Name(n => n.SchooltypeNewCode)
                        )
                        .Keyword(t => t
                            .Name(n => n.MetroLineId).Fields(ff => ff.Keyword(fk => fk.Name("keyword")))
                        )
                        .Keyword(t => t.Name(n => n.MetroStationId).Fields(ff => ff.Number(fk => fk.Name("keyword"))))
                        .Boolean(t => t.Name(n => n.Canteen))
                        .Boolean(t => t.Name(n => n.Lodging))
                        .Number(t => t.Name(n => n.Studentcount))
                        .Number(t => t.Name(n => n.Teachercount))
                        .Number(t => t.Name(n => n.Star))
                        .Number(t => t.Name(n => n.Commentcount))
                        .Number(t => t.Name(n => n.Questioncount))
                        .Object<SearchSchoolScore>(t => t.Name(n => n.Score)
                            .Properties(pp => pp.Number(fk => fk.Name(n => n.Total))
                                            .Number(fk => fk.Name(n => n.Composite))
                                            .Number(fk => fk.Name(n => n.Cost))
                                            .Number(fk => fk.Name(n => n.Course))
                                            .Number(fk => fk.Name(n => n.Hard))
                                            .Number(fk => fk.Name(n => n.Learn))
                                            .Number(fk => fk.Name(n => n.Teach))
                            )
                        )
                        .Keyword(t => t.Name(n => n.Authentication).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Characteristic).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Abroad).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Courses).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Keyword(t => t.Name(n => n.Tags).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                        .Number(t => t.Name(n => n.Tuition))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                );
        }

        public void PutMappingSignSchool()
        {
            var createIndexResponse = _client.Indices.Create(_singSchoolIndexName, c => c
                .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                        )
                ))
                .Map<SearchSignSchool>(mm => mm
               .Dynamic(false)
                .Properties(p => p
                    .Keyword(t => t
                        .Name(n => n.Id)
                    )
                    .Text(t => t
                        .Name(n => n.SchName)
                        .Fields(f => f.Text(ft
                                  => ft.Name("ik")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                    .Boolean(x => x.Name(n => n.IsDel))
                    .Number(x => x.Name(n => n.Type))
                )));
        }

        public void PutMappingSchoolSearch()
        {
            var mapping = _client.Indices.Create(_BTschoolSearchIndexName, c => c
                 .Settings(s => s
                    .Setting("max_ngram_diff", 20)
                    .Setting("max_result_window", 500000)
                    .Analysis(a => a
                        .CharFilters(cf => cf
                           .UserDefined("tsconvert", new TSConvertCharFilter("t2s", "#"))
                         )
                        .TokenFilters(tf => tf
                            .UserDefined("pinyin_max_filter",
                                new PinyinTokenFilter
                                {
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = true,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_keyword_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                            .UserDefined("pinyin_filter",
                                new PinyinTokenFilter
                                {
                                    KeepFirstLetter = true,
                                    KeepSeparateFirstLetter = false,
                                    KeepJoinedFullPinyin = true,
                                    KeepFullPinyin = false,
                                    KeepOriginal = true,
                                    LimitFirstLetterLength = 16,
                                    Lowercase = true,
                                    RemoveDuplicatedTerm = true
                                })
                        )
                        .Tokenizers(t => t.NGram("ngram_tokenizer", nt => nt
                               .MinGram(1).MaxGram(10)
                               .TokenChars(TokenChar.Letter, TokenChar.Digit)
                            )
                        )
                        .Analyzers(aa => aa
                            .Custom("ik_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("lowercase")
                             )
                            .Custom("ik_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_max_word")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("ik_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ik_smart")
                                 .Filters("pinyin_filter", "lowercase")
                             )
                            .Custom("ngram_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ngram_tokenizer")
                                 .Filters("lowercase")
                             )
                            .Custom("ngram_pinyin_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("ngram_tokenizer")
                                 .Filters("pinyin_max_filter", "lowercase")
                             )
                            .Custom("keyword_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("keyword")
                                 .Filters("lowercase")
                             )
                            .Custom("keyword_pinyin_search_analyzer", ca => ca
                                 .CharFilters("tsconvert")
                                 .Tokenizer("keyword")
                                 .Filters("pinyin_keyword_filter", "lowercase")
                             )
                        )
                ))
                 .Map<BTSearchSchool>(mm => mm
                .Dynamic(false)
                 .Properties(p => p
                     .Keyword(t => t
                         .Name(n => n.Id)
                     )
                     .Text(t => t
                            .Name(n => n.Name)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                     .Date(x => x.Name(n => n.UpdateTime))
                     .Keyword(x => x.Name(n => n.Creator))
                     .Keyword(x => x.Name(n => n.Modifier))
                     .Number(x => x.Name(n => n.Status))
                     .Nested<SchoolExtDetail>(x => x.Name(n => n.SchoolExtDetails)
                        .Properties(pp => pp.Keyword(fk => fk.Name(n => n.Id).Fields(ff => ff.Keyword(ffk => ffk.Name("keyword"))))
                                            .Keyword(fk => fk.Name(n => n.Name))
                                            .Number(fk => fk.Name(n => n.province))
                                            .Number(fk => fk.Name(n => n.City))
                                            .Number(fk => fk.Name(n => n.Area))
                                            .Number(fk => fk.Name(n => n.Grade))
                                            .Number(fk => fk.Name(n => n.Type))))
                 ))
            );
        }

        public void PutMappingComment()
        {
            var mapping = _client.Indices.PutMapping<SearchComment>(mm => mm.Index(_commentIndexName)
                    .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.Eid)
                        )
                        .Text(t => t
                            .Name(n => n.Context)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Number(t => t.Name(n => n.ReplyCount))
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                        .Number(t => t.Name(n => n.Grade))
                        .Number(t => t.Name(n => n.Type))
                        .Number(t => t.Name(n => n.Score))
                        .Boolean(t => t.Name(n => n.Lodging))
                        .Date(t => t.Name(n => n.PublishTime))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                );
        }
        public void PutMappingQuestion()
        {
            var mapping = _client.Indices.PutMapping<SearchQuestion>(mm => mm.Index(_questionIndexName)
                    .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Keyword(t => t
                            .Name(n => n.Eid)
                        )
                        .Text(t => t
                            .Name(n => n.Context)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Number(t => t.Name(n => n.ReplyCount))
                        .Number(t => t.Name(n => n.CityCode))
                        .Number(t => t.Name(n => n.AreaCode))
                        .Number(t => t.Name(n => n.Grade))
                        .Number(t => t.Name(n => n.Type))
                        .Boolean(t => t.Name(n => n.Lodging))
                        .Date(t => t.Name(n => n.PublishTime))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                );
        }
        public void PutMappingSchoolRank()
        {
            var mapping = _client.Indices.PutMapping<SearchSchoolRank>(mm => mm.Index(_schoolRankIndexName)
                    .Dynamic(false)
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Text(t => t
                            .Name(n => n.Title)
                            .Fields(f => f.Text(ft
                                  => ft.Name("pinyin")
                                  .Store(false)
                                  .TermVector(TermVectorOption.WithPositionsOffsets)
                                  .Analyzer("ik_pinyin_analyzer")
                                  .SearchAnalyzer("ik_analyzer")
                                  .Boost(5))
                            )
                        )
                        .Nested<SchoolRankItem>(t => t
                            .Name(n => n.Schools)
                            .Properties(np => np
                                .Number(tt => tt.Name(tn => tn.RankNo))
                                .Text(tt => tt
                                    .Name(tn => tn.Name)
                                    .Fields(f => f.Text(ft
                                          => ft.Name("pinyin")
                                          .Store(false)
                                          .TermVector(TermVectorOption.WithPositionsOffsets)
                                          .Analyzer("ik_pinyin_analyzer")
                                          .SearchAnalyzer("ik_analyzer")
                                          .Boost(5))
                            ))))
                        .Date(t => t.Name(n => n.UpdateTime))
                        .Boolean(t => t.Name(n => n.IsDeleted))
                    )
                );
        }
        public void PutMappingArticle()
        {
            var mapping = _client.Indices.PutMapping<SearchArticle>(mm => mm.Index(_articleIndexName)
                    .Dynamic(false)
                   .Properties(p => p
                       .Keyword(t => t
                           .Name(n => n.Id)
                       )
                       .Text(t => t
                           .Name(n => n.Title)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Keyword(t => t.Name(n => n.Tags).Fields(ff => ff.Keyword(fk => fk.Name("keyword"))))
                       .Date(t => t.Name(n => n.UpdateTime))
                       .Boolean(t => t.Name(n => n.IsDeleted))
                   )
               );
        }
        #endregion


        public void CreateIndexHistory()
        {
            var indexName = "historyindex_" + DateTime.Now.Date.ToString("yyyyMMdd");
            if (!_client.Indices.Exists(indexName).Exists)
            {
                var mapping = _client.Indices.Create(indexName, m => m.Map<SearchHistory>(mm => mm
                    .Properties(p => p
                         .Text(t => t
                           .Name(n => n.SearchWord)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Keyword(t => t
                           .Name(n => n.Word)
                       )
                       .Number(t => t.Name(n => n.CityCode))
                       .Date(t => t.Name(n => n.DateTime))
                       .Keyword(t => t
                           .Name(n => n.UserId)
                       )
                       .Keyword(t => t
                           .Name(n => n.UUID)
                       )
                       .Keyword(t => t
                           .Name(n => n.Application)
                       )
                       .Keyword(t => t
                           .Name(n => n.Channel)
                       )
                    )
                ).Aliases(a => a.Alias("historyindex")));
            }
        }



        public void PutMappingHistory()
        {
            var mapping = _client.Indices.PutMapping<SearchHistory>(mm => mm.Index("historyindex")
                   .Properties(p => p
                       .Text(t => t
                           .Name(n => n.SearchWord)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Keyword(t => t
                           .Name(n => n.Word)
                       )
                       .Number(t => t.Name(n => n.CityCode))
                       .Date(t => t.Name(n => n.DateTime))
                       .Keyword(t => t
                           .Name(n => n.UserId)
                       )
                       .Keyword(t => t
                           .Name(n => n.UUID)
                       )
                       .Keyword(t => t
                           .Name(n => n.Application)
                       )
                       .Keyword(t => t
                           .Name(n => n.Channel)
                       )
                   )
               );
        }

        /// <summary>
        /// 学校统计
        /// </summary>
        public void PutMappingSchoolCount()
        {
            var mapping = _client.Indices.PutMapping<SearchArticle>(mm => mm.Index("schoolcountindex")
                   .Properties(p => p
                       .Keyword(t => t
                           .Name(n => n.Id)
                       )
                       .Number(t => t
                           .Name("type")
                       )
                       .Text(t => t
                           .Name(n => n.Title)
                           .Fields(f => f.Text(ft
                                 => ft.Name("pinyin")
                                 .Store(false)
                                 .TermVector(TermVectorOption.WithPositionsOffsets)
                                 .Analyzer("ik_pinyin_analyzer")
                                 .SearchAnalyzer("ik_analyzer")
                                 .Boost(5))
                           )
                       )
                       .Number(t => t
                           .Name("count")
                       )
                   )
               );
        }


        public void ImportSchoolData(List<SearchSchool> school)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(school,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_schoolIndexName).Document(record).Id(record.Id)));

        }
        public void ImportCommentData(List<SearchComment> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_commentIndexName).Document(record).Id(record.Id)));
        }

        public void ImportSingSchoolData(List<SearchSignSchool> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                        (bulkDescriptor, record) =>
                                            bulkDescriptor.Index(_singSchoolIndexName).Document(record).Id(record.Id)));
        }

        public bool ImportSearchSchoolData(List<BTSearchSchool> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                        (bulkDescriptor, record) =>
                                            bulkDescriptor.Index(_BTschoolSearchIndexName).Document(record).Id(record.Id)));
            return indexResponse.ApiCall.HttpStatusCode == 200;
        }

        public void ImportQuestionData(List<SearchQuestion> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_questionIndexName).Document(record).Id(record.Id)));
        }
        public void ImportSchoolRankData(List<SearchSchoolRank> list)
        {

            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_schoolRankIndexName).Document(record).Id(record.Id)));
        }
        public void ImportArticleData(List<SearchArticle> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_articleIndexName).Document(record).Id(record.Id)));
        }
        public void ImportLiveData(List<SearchLive> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_liveIndexName).Document(record).Id(record.Id)));
        }
        public void ImportTopicData(List<SearchTopic> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index(_topicIndexName).Document(record).Id(record.Id)));
        }
        public void UpdateTopicData(List<SearchTopic> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchTopic, object>(item.Id)
                {
                    Index = _topicIndexName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
        }
        public void ImportSchoolCountData()
        {
            var data = new List<SearchArticle>
            {

            };
            var indexResponse = _client.Bulk(s => s.IndexMany(data,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index("schoolcountindex").Document(record).Id(record.Id)));
        }



        public bool SetHistory(string Application, int channel, string keyWord, int cityCode, Guid? UserId, string UUID)
        {
            var indexResponse = _client.Index(new SearchHistory
            {
                UserId = UserId,
                UUID = UUID,
                CityCode = cityCode,
                Application = Application,
                Channel = channel,
                SearchWord = keyWord,
                Word = keyWord,
                DateTime = DateTime.Now
            }, i => i.Index("historyindex"));
            return indexResponse.IsValid;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list"></param>
        public bool RemoveUserHistory(Guid userId)
        {
            var update = new UpdateByQueryRequest("historyindex");

            var query = new QueryContainerDescriptor<SearchHistory>();
            query.Term(t => t.Field(f => f.UserId).Value(userId));
            update.Query = query;
            update.SourceIncludes = new Field[] { "UserId" };
            update.Script = new InlineScript("ctx._source[uv]=0");

            var indexResponse = _client.UpdateByQuery(update);
            return indexResponse.IsValid;
        }

        public void PutMappingEESchool()
        {
            var mapping = _client.Indices.Create("allschool", m => m.Map<SearchEESchool>(mm => mm
                    .Properties(p => p
                        .Keyword(t => t
                            .Name(n => n.Id)
                        )
                        .Text(t => t
                            .Name(n => n.Name)
                            .Analyzer("ik_max_word")
                            .SearchAnalyzer("ik_smart")
                        )
                        .Number(t => t.Name(n => n.Grade))
                        .Number(t => t.Name(n => n.Type))
                        .Text(t => t
                            .Name(n => n.Province)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Text(t => t
                            .Name(n => n.City)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Text(t => t
                            .Name(n => n.Area)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Text(t => t
                            .Name(n => n.Cityarea)
                            .Analyzer("ik_max_word")
                            .SearchAnalyzer("ik_smart")
                        )
                        .Text(t => t
                            .Name(n => n.Status)
                        //.CopyTo(ct => ct.Field(ff => ff.Cityarea))
                        )
                        .Date(t => t.Name(n => n.UpdateTime))
                    )
                ));
        }

        public void ImportEESchoolData(List<SearchEESchool> list)
        {
            var indexResponse = _client.Bulk(s => s.IndexMany(list,
                                       (bulkDescriptor, record) =>
                                         bulkDescriptor.Index("allschool").Document(record).Id(record.Id)));
        }

        /// <summary>
        /// 更新学校数据
        /// </summary>
        /// <param name="list"></param>
        public void UpdateSchoolData(List<SearchSchool> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchSchool, object>(item.Id)
                {
                    Index = _schoolIndexName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
        }

        /// <summary>
        /// 更新学校数据 - 浏览量, 热度
        /// </summary>
        /// <param name="list"></param>
        public bool UpdateSchoolViewCount(List<SearchSchoolViewCount> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchSchoolViewCount, object>(item.Id)
                {
                    Index = _schoolIndexAliasName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
            return indexResponse.ApiCall.HttpStatusCode == 200;
        }

        /// <summary>
        /// 更新学校数据 - 浏览量, 热度
        /// </summary>
        /// <param name="list"></param>
        public async Task<bool> UpdateUVAsync(string index, IEnumerable<SearchUV> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchUV, object>(item.Id)
                {
                    Index = index,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = await _client.BulkAsync(bulkUpdate);
            return indexResponse.ApiCall.HttpStatusCode == 200;
        }

        public async Task<bool> ResetUVAsync(string index)
        {
            var update = new UpdateByQueryRequest(index);

            var query = new QueryContainerDescriptor<SearchHistory>();
            query.MatchAll();
            update.Query = query;
            update.SourceIncludes = new Field[] {};
            update.Script = new InlineScript("ctx._source['uv']=0");

            var indexResponse = await _client.UpdateByQueryAsync(update);
            return indexResponse.IsValid;
        }

        /// <summary>
        /// 更新点评数据
        /// </summary>
        /// <param name="list"></param>
        public void UpdateCommentData(List<SearchComment> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchComment, object>(item.Id)
                {
                    Index = _commentIndexName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
        }
        /// <summary>
        /// 更新问题数据
        /// </summary>
        /// <param name="list"></param>
        public void UpdateQuestionData(List<SearchQuestion> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchQuestion, object>(item.Id)
                {
                    Index = _questionIndexName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
        }

        public void UpdateSignSchoolData(List<SearchSignSchool> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<SearchSignSchool, object>(item.Id)
                {
                    Index = _singSchoolIndexName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
        }

        public bool DelSchoolData(List<string> Ids)
        {
            //var DeleteIndexResponse = _client.DeleteMany<BTSearchSchool>(List, _schoolSearchIndexName);
            var DeleteIndexResponse = _client.DeleteByQuery<BTSearchSchool>(d => d
            .Index(_BTschoolSearchIndexName)
            .Query(q => q
                .Terms(qt => qt.Field(f => f.Id).Terms(Ids))));
            return DeleteIndexResponse.ApiCall.HttpStatusCode == 200;
        }

        public bool UpdateBDSchooData(List<BTSearchSchool> list)
        {
            var bulkUpdate = new BulkRequest() { Operations = new List<IBulkOperation>() };
            foreach (var item in list)
            {
                var operation = new BulkUpdateOperation<BTSearchSchool, object>(item.Id)
                {
                    Index = _BTschoolSearchIndexName,
                    Doc = item
                };
                bulkUpdate.Operations.Add(operation);
            }
            var indexResponse = _client.Bulk(bulkUpdate);
            return indexResponse.ApiCall.HttpStatusCode == 200;
        }

        public SearchLastUpdateTime GetBDLastCreateTime()
        {
            SearchLastUpdateTime lastUpdateTimes = new SearchLastUpdateTime();

            var result = _client.Search<BTSearchSchool>(s => s
                       .Index(_BTschoolSearchIndexName)
                       .Size(0)
                       .Aggregations(aggs => aggs
                           .Terms("index_aggs", t => t.Field("_index").Size(5)
                               .Aggregations(indexaggs => indexaggs.TopHits("top_hits_aggs", th =>
                                    th.Source(sf => sf.Includes(q => q.Field(p => p.UpdateTime)))
                                    .Sort(so => so.Field(ss => ss.UpdateTime, SortOrder.Descending)).Size(1)))
                           )
                       )
                );
            if (result.Aggregations.Count > 0)
            {
                var indexAggs = (BucketAggregate)result.Aggregations["index_aggs"];
                foreach (var item in indexAggs.Items)
                {
                    var bucket = (KeyedBucket<object>)item;
                    var source = ((TopHitsAggregate)bucket["top_hits_aggs"]).Hits<SearchBDlastCreateTime>().Select(q => q.Source).ToList();
                    //lastUpdateTimes.FirstOrDefault(q => q.Name == (string)bucket.Key).UpdateTime = source[0].UpdateTime;
                    lastUpdateTimes.UpdateTime = source[0].CreateTime;
                }
            }
            return lastUpdateTimes;
        }

        public bool DelArticleData(List<Guid> deleteIds)
        {
            var DeleteIndexResponse = _client.DeleteByQuery<SearchArticle>(d => d
            .Index(_articleIndexName)
            .Query(q => q
                .Terms(qt => qt.Field(f => f.Id).Terms(deleteIds))));
            return DeleteIndexResponse.ApiCall.HttpStatusCode == 200;
        }



        //public void DelExtSchoolData(List<string> Ids) 
        //{
        //    var DeleteIndexResponse = _client.DeleteByQuery<BTSearchSchool>(d => d
        //    .Index(_BTschoolSearchIndexName)
        //    .Query(q => q
        //        .Bool(x => 
        //            x.Must(m => 
        //                m.Nested(n =>
        //                    n.Path(p => p.SchoolExtDetails)
        //                    .Query(qq => 
        //                        qq.Terms(qt => qt.Name("schoolExtDetails.id.keyword").Terms(Ids)))))))));
        //}


        /// <summary>
        /// 中职批量更新
        /// </summary>
        /// <param name="province"></param>
        /// <param name="isDeleted"></param>
        public void UpdateSvsSchool(long province, string isDeleted)
        {
            ////批量修改
            var updateByQueryResponse = _client.UpdateByQuery<SearchSvsSchool>(u => u
                                        .Index(_svsSchoolIndexName)
                                        .Query(q => q.Term(t => t.Field(f => f.Province).Value(province)))
                                        .Script(s => s.Source($"ctx._source.isDeleted = {isDeleted};")));

        }
    }
}
