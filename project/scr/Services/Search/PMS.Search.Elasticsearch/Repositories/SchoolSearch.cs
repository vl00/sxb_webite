using Microsoft.Extensions.Options;
using Nest;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
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

namespace PMS.Search.Elasticsearch.Repositories
{
    public class SchoolSearch : ISchoolSearch
    {
        private readonly ElasticClient _client;
        private readonly EsConfig _esConfig;

        private readonly Index _index;

        public SchoolSearch(ISearch clientProvider, IOptions<EsConfig> options)
        {
            _client = clientProvider.GetClient();
            _esConfig = options.Value ?? new EsConfig();

            if (_esConfig.Indices != null)
            {
                _index = _esConfig.Indices.School;
            }
        }

        public async Task<PaginationModel<Guid>> RecommenAsync(string keyword, int cityId
            , List<int> areaIds
            , List<int> grades, List<string> schoolTypeCodes //学校类型(学段+类型)
            , int? minTotal, int? maxTotal //学校评分
            , List<Guid> authIds
            , List<Guid> courseIds
            , List<Guid> characIds
            , int pageIndex = 1, int pageSize = 10)
        {
            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            //日期衰减
            var queryDesc = GetRecommendQueryDescriptor(keyword, cityId, areaIds, grades, schoolTypeCodes, minTotal, maxTotal, authIds, courseIds, characIds);
            var searchDesc = new SearchDescriptor<SearchSchool>()
                .Index(_index.SearchIndex)
                .From(from)
                .Size(size)
                .Source(s => s.Includes(i => i.Field("id")))
                .Query(queryDesc);

            var result = await _client.SearchAsync<SearchSchool>(searchDesc);
            var total = result.Total;
            var data = result.Hits.Select(q => q.Source.Id).ToList();

            return PaginationModel.Build(data, total);
        }


        public async Task<List<SearchSchoolExplanationQueryModel>> RecommenExplainAsync(string keyword, int cityId
            , List<int> areaIds
            , List<int> grades, List<string> schoolTypeCodes //学校类型(学段+类型)
            , int? minTotal, int? maxTotal //学校评分
            , List<Guid> authIds
            , List<Guid> courseIds
            , List<Guid> characIds
            , int pageIndex = 1, int pageSize = 10)
        {
            int from = (pageIndex - 1) * pageSize;
            int size = pageSize;

            //日期衰减
            var queryDesc = GetRecommendQueryDescriptor(keyword, cityId, areaIds, grades, schoolTypeCodes, minTotal, maxTotal, authIds, courseIds, characIds);
            var searchDesc = new SearchDescriptor<SearchSchool>()
                .Explain()
                .Index(_index.SearchIndex)
                .From(from)
                .Size(size)
                .Source(s => s.Includes(i => i.Fields("id", "name")))
                .Query(queryDesc);

            var result = await _client.SearchAsync<SearchSchool>(searchDesc);
            var total = result.Total;

            List<SearchSchoolExplanationQueryModel> data = new List<SearchSchoolExplanationQueryModel>();
            foreach (var hit in result.Hits)
            {
                List<object> explanations = new List<object>();
                var score = 0d;
                var expls = hit.Explanation.Details;
                foreach (var expl in expls)
                {
                    var desc = string.Empty;
                    if (expl.Description.Contains("areaCode"))
                    {
                        desc = expl.Description.Replace("areaCode", "城区");
                    }
                    else if (expl.Description.Contains("ConstantScore(schooltypeNewCode"))
                    {
                        desc = expl.Description.Replace("ConstantScore(schooltypeNewCode", "(学校类型");
                    }
                    else if (expl.Description.Contains("score.total"))
                    {
                        desc = expl.Description.Replace("score.total", "学校评分");
                    }
                    else if (expl.Description.Contains("ConstantScore(authentication"))
                    {
                        desc = expl.Description.Replace("ConstantScore(authentication", "(学校认证");
                    }
                    else if (expl.Description.Contains("ConstantScore(courses"))
                    {
                        desc = expl.Description.Replace("ConstantScore(courses", "(课程设置");
                    }
                    else if (expl.Description.Contains("ConstantScore(characteristic"))
                    {
                        desc = expl.Description.Replace("ConstantScore(characteristic", "(特色课程");
                    }

                    if (desc != string.Empty)
                    {
                        score += expl.Value;
                        explanations.Add(new { desc, score = expl.Value });
                    }
                }

                var item = new SearchSchoolExplanationQueryModel()
                {
                    Id = hit.Source.Id,
                    Name = hit.Source.Name,
                    Score = score,
                    Explanations = explanations
                };

                data.Add(item);
            }

            return data;
        }

        /// <summary>
        /// 同城推荐
        /// 1. 城区     6
        /// 2, 学校类型 5
        /// 3. 学校评分 4
        /// 4. 学校认证 3
        /// 5. 课程设置 2
        /// 6. 特色课程 1
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private Func<QueryContainerDescriptor<SearchSchool>, QueryContainer> GetRecommendQueryDescriptor(string keyword, int cityId
            , List<int> areaIds
            , List<int> grades, List<string> schoolTypeCodes //学校类型(学段+类型)
            , int? minTotal, int? maxTotal //学校评分
            , List<Guid> authIds
            , List<Guid> courseIds
            , List<Guid> characIds)
        {
            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false)),
                m => m.Term(t => t.Field(tf => tf.CityCode).Value(cityId))
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
            };

            //关键字查询
            if (string.IsNullOrEmpty(keyword))
            {
                var queryContainer = new QueryContainerDescriptor<SearchSchool>();

                queryContainer.Bool(b => b.Should(
                    s => s.Term("name.keyword", keyword),
                    s => s.Term("name.pinyin", keyword),
                    s => s.MultiMatch(q => new MultiMatchQuery()
                    {
                        Fields = Infer.Field("name.pinyin").And("name.keyword").And("tags.pinyin"),
                        Operator = Operator.Or,
                        Query = keyword
                    })
                ));

                var queryTerm = new TermQuery()
                {
                    Field = Infer.Field("name.cntext"),
                    Value = keyword
                };

                var queryTermTags = new TermQuery()
                {
                    Field = Infer.Field("tags.keyword"),
                    Value = keyword
                };

                mustQuerys.Add(q => q.Bool(b => b.Must(m => queryContainer).Should(s => queryTerm || queryTermTags)));
            }

            //1. 地区推荐
            if (areaIds.Any())
            {
                shouldQuerys.Add(q => q.Terms(t =>
                    t.Field(f => f.AreaCode)
                    .Terms(areaIds)
                    .Boost(RecommendConstant.SchoolAreaScore)));
            }

            //double rate1 = 0.1;
            //double rate2 = 0.9;
            //2. 学校类型推荐
            if (schoolTypeCodes.Any()) //grades.Any() && 
            {
                //shouldQuerys.Add(q => q.Terms(t =>
                //    t.Field(f => f.Grade)
                //    .Terms(grades)
                //    .Boost(RecommendConstant.SchoolTypeScore * rate1)));

                shouldQuerys.Add(q => q.Terms(t =>
                    t.Field(f => f.SchooltypeNewCode)
                    .Terms(schoolTypeCodes)
                    .Boost(RecommendConstant.SchoolTypeScore)));
            }

            //3. 学校评分推荐
            if (minTotal != null && maxTotal != null)
            {
                var query = new NumericRangeQuery
                {
                    Field = Infer.Field<SearchSchool>(p => p.Score.Total),
                    GreaterThanOrEqualTo = minTotal,
                    LessThan = maxTotal,
                    Boost = RecommendConstant.SchoolScoreScore
                };
                shouldQuerys.Add(mq => query);
            }

            //4. 学校认证推荐
            if (authIds.Any())
            {
                shouldQuerys.Add(q => q.ConstantScore(cs =>
                    cs.Filter(sf => sf.Terms(t => t.Field(f => f.Authentication).Terms(authIds)))
                     .Boost(RecommendConstant.SchoolAuthScore)
                 ));
            }

            //5. 学校课程设置
            if (courseIds.Any())
            {
                shouldQuerys.Add(q => q.ConstantScore(cs =>
                    cs.Filter(sf => sf.Terms(t => t.Field(f => f.Courses).Terms(courseIds)))
                     .Boost(RecommendConstant.SchoolCourseScore)
                 ));
            }
            //6. 特色课程推荐
            if (characIds.Any())
            {
                shouldQuerys.Add(q => q.ConstantScore(cs =>
                    cs.Filter(sf => sf.Terms(t => t.Field(f => f.Characteristic).Terms(characIds)))
                     .Boost(RecommendConstant.SchoolCharacteristicScore)
                 ));
            }
            return q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys));
        }


        public async Task<PaginationModel<Guid>> SearchSchools(SearchBaseQueryModel queryModel)
        {
            int from = (queryModel.PageIndex - 1) * queryModel.PageSize;
            int size = queryModel.PageSize;

            var mustQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.IsDeleted).Value(false)),
            };
            var shouldQuerys = new List<Func<QueryContainerDescriptor<SearchSchool>, QueryContainer>>
            {
                m => m.Term(t => t.Field(tf => tf.CityCode).Value(queryModel.CityCode))
            };

            if (queryModel.QueryEsKeywords.Any())
            {
                var innerShoulds = NestHelper.KeywordsToNest<SearchSchool>(
                    queryModel.QueryEsKeywords,
                    queryModel.IsMatchAllWord,
                    Infer.Fields("name.pinyin", "name.cntext", "name.keyword", "tags.cntext", "tags.keyword"),
                    Infer.Fields("name.cntext", "tags.cntext"));
                mustQuerys.Add(q => q.Bool(b => b.Should(innerShoulds)));
            }

            if (queryModel.MustCityCode)
            {
                mustQuerys.Add(m => m.Term(t => t.Field(tf => tf.CityCode).Value(queryModel.CityCode)));
            }

            var searchDesc = new SearchDescriptor<SearchSchool>()
                .Index(_index.SearchIndex)
                .From(from)
                .Size(size)
                .Source(s => s.Includes(i => i.Field("id")))
                .Query(q => q.Bool(b => b.Must(mustQuerys).Should(shouldQuerys)));

            var result = await _client.SearchAsync<SearchSchool>(searchDesc);
            var total = result.Total;
            var data = result.Hits.Select(q => q.Source.Id).ToList();
            return PaginationModel.Build(data, total);
        }
    }
}
