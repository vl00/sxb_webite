using NPOI.SS.Formula.PTG;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto;
using PMS.Search.Application.ModelDto.Query;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IDataSearch _dataSearch;
        private readonly ISchoolSearch _schoolSearch;
        private readonly IArticleSearch _articleSearch;
        public SearchService(IDataSearch dataSearch, ISchoolSearch schoolSearch, IArticleSearch articleSearch)
        {
            _dataSearch = dataSearch;
            _schoolSearch = schoolSearch;
            _articleSearch = articleSearch;
        }

        public void AggSuggestCityArea()
        {

        }

        public void AggSuggestSchoolType()
        {

        }

        public SearchSuggestResult ListSuggest(string keyWord, int channel, int cityCode)
        {
            var result = _dataSearch.ListSuggest(keyWord, channel, cityCode);

            return result;
        }

        public SearchSuggestResult ListSuggest(string keyword, ChannelIndex channel, int cityCode, int size = 2)
        {
            var result = _dataSearch.ListSuggest(new SearchAllQueryModel(keyword)
            {
                Channel = (int)channel,
                CityCode = cityCode,
                Size = size
            });
            return result;
        }
        public SearchSuggestResult ListSuggest(SearchAllQueryModel queryModel)
        {
            var result = _dataSearch.ListSuggest(queryModel);
            return result;
        }

        public List<(string keyword, long total)> ListSuggestTotal(List<string> keywords, ChannelIndex channel, int cityCode)
        {
            List<(string, long)> result = new List<(string, long)>();
            keywords.AsParallel().ForAll(keyword =>
            {
                var total = _dataSearch.ListSuggestTotalAsync(keyword, channel, cityCode).GetAwaiter().GetResult();
                result.Add((keyword, total));
            });
            return result;
        }


        public ResultAllDto SearchAll(string keyword, int searchType, Guid? userId, int pageNo = 1, int pageSize = 10)
        {
            var result = _dataSearch.SearchAll(keyword, searchType, userId, out long total, pageNo, pageSize);

            return new ResultAllDto
            {
                Total = total,
                List = result.Select(q => new SearchAllDto
                {
                    Id = q.Id,
                    Context = q.Context,
                    Title = q.Title,
                    UserId = q.UserId,
                    UpdateTime = q.UpdateTime,
                    Type = q.Type
                }).ToList()
            };
        }

        public ResultIdDto SearchIds(string keyword, ChannelIndex channel, Guid? userId, int pageNo = 1, int pageSize = 10)
        {
            var result = _dataSearch.SearchIds(out long total, keyword, channel, userId, pageNo, pageSize);

            return new ResultIdDto
            {
                Total = total,
                List = result.Select(q => new SearchIdDto
                {
                    Id = q.Id,
                    UserId = q.UserId,
                    Channel = q.Channel
                }).ToList()
            };
        }

        public Task<PaginationModel<Guid>> SearchSchoolsFullMatch(SearchBaseQueryModel queryModel)
        {
            var result = _schoolSearch.SearchSchools(queryModel);
            return result;
        }

        public async Task<List<Guid>> RecommenSchoolIdsAsync(string keyword, int cityId
            , List<int> areaIds
            , List<int> grades, List<string> schoolTypeCodes //学校类型(学段+类型)
            , int? minTotal, int? maxTotal //学校评分
            , List<Guid> authIds
            , List<Guid> courseIds
            , List<Guid> characIds
            , int pageIndex = 1, int pageSize = 10)
        {
            var result = await _schoolSearch.RecommenAsync(keyword, cityId, areaIds, grades, schoolTypeCodes, minTotal, maxTotal, authIds, courseIds, characIds, pageIndex, pageSize);
            return result?.Data;
        }

        public async Task<List<SearchSchoolExplanationQueryModel>> RecommenSchoolExplainAsync(string keyword, int cityId
            , List<int> areaIds
            , List<int> grades, List<string> schoolTypeCodes //学校类型(学段+类型)
            , int? minTotal, int? maxTotal //学校评分
            , List<Guid> authIds
            , List<Guid> courseIds
            , List<Guid> characIds
            , int pageIndex = 1, int pageSize = 10)
        {
            var result = await _schoolSearch.RecommenExplainAsync(keyword, cityId, areaIds, grades, schoolTypeCodes, minTotal, maxTotal, authIds, courseIds, characIds, pageIndex, pageSize);
            return result;
        }


        public ResultSchoolDto SearchSchool(SearchSchoolQuery query)
        {
            var queryData = query.ConvertTo();
            var result = _dataSearch.SearchSchool(queryData, out long total);

            if (total == 0)
            {
                //如果查询无数据，则用拼音模糊查询
                result = _dataSearch.SearchSchool(queryData, out total, true);
            }

            return new ResultSchoolDto
            {
                Total = total,
                Schools = result.Select(q => new SearchSchoolDto
                {
                    Id = q.Id,
                    SchoolId = q.SchoolId ?? Guid.Empty,
                    Name = q.Name,
                    Distance = q.Distance,
                    SearchScore = q.Score
                }).ToList()
            };
        }

        public async Task<long> SearchSchoolTotalCountAsync(SearchSchoolQuery query)
        {
            var queryData = query.ConvertTo();
            var totalCount = await _dataSearch.SearchSchoolTotalCountAsync(queryData);
            if (totalCount == 0)
            {
                //如果查询无数据，则用拼音模糊查询
                totalCount = await _dataSearch.SearchSchoolTotalCountAsync(queryData, true);
            }
            return totalCount;
        }

        public List<(int cityId, long totalCount)> SearchSchoolCity(SearchSchoolQuery query, bool isVague = false)
        {
            var queryData = query.ConvertTo();
            return _dataSearch.SearchSchoolCity(queryData, isVague);
        }

        /// <summary>
        /// 学校费用区间分布条形图📊统计
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<ResultSchoolStatsDto> SearchSchoolCostStats(SearchSchoolQuery query)
        {
            var queryData = query.ConvertTo();
            var costResult = _dataSearch.SearchSchoolCostStats(queryData);


            return costResult.Select(q => new ResultSchoolStatsDto
            {
                Name = q.Name,
                Hist = q.Hist.Select(p => new ResultSchoolHistDto
                {
                    Key = p.Key,
                    Count = p.Count
                }).ToList()
            }).ToList();
        }


        public ResultCommentDto SearchComment(SearchCommentQuery query)
        {
            var result = _dataSearch.SearchComment(query?.Lodging, query.Keyword, query.Keywords, query.UserId, query.CityIds, query.AreaIds, query.GradeIds, query.TypeIds,
                query.Orderby, query.Eid, out long total, query.PageNo, query.PageSize);
            return new ResultCommentDto
            {
                Total = total,
                Comments = result.Select(q => new SearchCommentDto
                {
                    Id = q.Id,
                    Context = q.Context,
                    PublishTime = q.PublishTime ?? new DateTime()
                }).ToList()
            };
        }

        public ResultQuestionDto SearchQuestion(SearchQuestionQuery query)
        {
            var result = _dataSearch.SearchQuestion(query.Lodging, query.Keyword, query.Keywords, query.UserId, query.CityIds, query.AreaIds, query.GradeIds, query.TypeIds,
                query.Orderby, query.Eid, out long total, query.PageNo, query.PageSize);
            return new ResultQuestionDto
            {
                Total = total,
                Questions = result.Select(q => new SearchQuestionDto
                {
                    Id = q.Id,
                    Context = q.Context,
                    PublishTime = q.PublishTime ?? new DateTime()
                }).ToList()
            };
        }
        public ResultLiveDto SearchLive(SearchLiveQueryModel queryModel)
        {
            var result = _dataSearch.SearchLive(out long total, queryModel);
            return new ResultLiveDto
            {
                Total = total,
                Lives = result.Select(q => new SearchLiveDto
                {
                    Id = q.Id
                }).ToList()
            };
        }

        public ResultSchoolRankDto SearchSchoolRank(SearchBaseQueryModel queryModel)
        {
            var result = _dataSearch.SearchSchoolRank(out long total, queryModel);
            return new ResultSchoolRankDto
            {
                Total = total,
                Ranks = result.Select(q => new SearchSchoolRankDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Schools = q.Schools.Select(p => p.Name).ToList()
                }).ToList()
            };
        }

        public ResultArticleDto SearchArticle(string keyword, Guid? userId, int pageNo = 1, int pageSize = 10)
        {
            return SearchArticle(new SearchArticleQueryModel(keyword, pageNo, pageSize) { UserId = userId });
        }

        public ResultArticleDto SearchArticle(SearchArticleQueryModel queryModel)
        {
            var result = _articleSearch.SearchArticle(out long total, queryModel);
            return new ResultArticleDto
            {
                Total = total,
                Articles = result.Select(q => new SearchArticleDto
                {
                    Id = q.Id,
                    Title = q.Title
                }).ToList()
            };
        }

        public ResultMapSchoolDto SearchSchoolMap(SearchMapSchoolQuery query)
        {
            var grades = new List<int>();
            var schoolTypeCodes = query.Type;
            if (query.Type.Count == 0)
            {
                //去掉学校类型所属的学段
                var g = query.Type.Where(q => q.Count() > 2)
                    .Select(q => q[2].ToString()).GroupBy(q => q).Select(q => Convert.ToInt32(q.Key)).ToList();

                grades = query.Grade.Where(q => !g.Contains(q)).ToList();
            }


            var result = _dataSearch.SearchMapSchool(query.Level, query.KeyWords,
                query.SWLat, query.SWLng, query.NELat, query.NELng, query.CityCode,
                query.MetroLineIds, query.MetroStationIds, grades, schoolTypeCodes, 0, 0, 0, 0, 0, 0,
                query.Lodging, query.Canteen, query.AuthIds, query.CharacIds, query.AbroadIds, query.CourseIds);
            return new ResultMapSchoolDto
            {
                Count = result.Count,
                ViewCount = result.ViewCount,
                List = result.List.Select(q => new SearchMapDto
                {
                    Id = q.Id,
                    SchoolId = q.SchoolId ?? Guid.Empty,
                    Name = q.Name,
                    Count = q.Count,
                    Latitude = q.Location?.Lat ?? 0,
                    Longitude = q.Location?.Lon ?? 0
                }).ToList()
            };
        }

        public List<string> GetHistoryList(Guid? userId, string UUID, int channel, int size)
        {
            var list = _dataSearch.GetHistorySearch(userId, UUID, channel, size);
            return list.Select(q => q.Word).ToList();
        }

        public List<string> GetDistinctHistorySearch(int channel, Guid? userId, Guid? uuid = null, int pageSize = 10)
        {
            return _dataSearch.GetDistinctHistorySearch(channel, userId, uuid, pageSize);
        }

        public List<string> GetHotHistoryList(int channel, int pageSize, Guid? userId = null, int? cityCode = null, int min = 10)
        {
            return _dataSearch.GetHotSearch(channel, pageSize, userId, cityCode, min);
        }

        public List<SearchEESchool> SearchEESchool(string keyword)
        {
            return _dataSearch.SearchEESchool(keyword);
        }


        public List<SearchSignSchool> SearchSingSchool(string keyword, out long total, int pageNo, int pageSize)
        {
            return _dataSearch.SearchSingSchool(keyword, out total);
        }

        public BTSearchResult BTSchoolSearch(BDSchoolSearch search)
        {
            return _dataSearch.BTSchoolSearch(search);
        }


        public (List<Guid>, string scrollid) GetArticleIds(string scrollid)
        {
            return _dataSearch.GetArticleIds(scrollid);
        }

        public List<SearchTopicDto> SearchTopic(out long total, string keyword, Guid? circleId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime, int pageNo = 1, int pageSize = 10)
        {
            //SearchTopicDto
            var data = _dataSearch.SearchTopic(out total, keyword, circleId, null, null, creator, type, isGood, isQA, tags, startTime, endTime, pageNo, pageSize);

            return data.Select(s => new SearchTopicDto()
            {
                Id = s.Id,
                CircleId = s.CircleId,
                Content = s.Content,
                LastReplyTime = s.LastReplyTime
            }).ToList();
        }


        public List<SearchCircleDto> SearchCircle(SearchBaseQueryModel queryModel)
        {
            var data = _dataSearch.SearchCircle(out long total, queryModel);

            return data.Select(s => new SearchCircleDto()
            {
                Id = s.Id,
                Name = s.Name,
                Intro = s.Intro,
                IsDisable = s.IsDisable,
                UserId = s.UserId,
                UserName = s.UserName,
                ModifyTime = s.ModifyTime,
                CreateTime = s.CreateTime,
                FollowCount = s.FollowCount
            }).ToList();
        }

        public PageData SearchSvsSchool(SvsSchoolPageListQuery query)
        {
            var data = _dataSearch.SearchSvsSchool(query);
            return data;
        }

    }
}
