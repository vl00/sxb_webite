using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.Search.Application.ModelDto;
using PMS.Search.Application.ModelDto.Query;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Models;

namespace PMS.Search.Application.IServices
{
    public interface ISearchService
    {
        SearchSuggestResult ListSuggest(string keyWord,int channel, int cityCode); 

        [Obsolete()]
        SearchSuggestResult ListSuggest(string keyWord, GlobalEnum.ChannelIndex channel, int cityCode, int size = 2);
        SearchSuggestResult ListSuggest(SearchAllQueryModel queryModel);

        ResultSchoolDto SearchSchool(SearchSchoolQuery query);
        ResultCommentDto SearchComment(SearchCommentQuery query);
        ResultQuestionDto SearchQuestion(SearchQuestionQuery query);
        ResultSchoolRankDto SearchSchoolRank(SearchBaseQueryModel queryModel);
        ResultArticleDto SearchArticle(string keywords,Guid? userId, int pageNo, int pageSize);
        ResultLiveDto SearchLive(SearchLiveQueryModel queryModel);

        ResultAllDto SearchAll(string keyword, int searchType, Guid? userId, int pageNo, int pageSize);

        List<ResultSchoolStatsDto> SearchSchoolCostStats(SearchSchoolQuery query);

        ResultMapSchoolDto SearchSchoolMap(SearchMapSchoolQuery query);


        List<string> GetHistoryList(Guid? userId, string UUID,int channel, int size);
        List<string> GetHotHistoryList(int channel, int pageSize, Guid? userId = null, int? cityCode = null, int min = 10);

        List<SearchEESchool> SearchEESchool(string keyword);

        List<SearchSignSchool> SearchSingSchool(string keyword, out long total, int pageNo = 1, int pageSize = 10);

        BTSearchResult BTSchoolSearch(BDSchoolSearch search);

        (List<Guid>, string scrollid) GetArticleIds(string scrollid);
        List<SearchTopicDto> SearchTopic(out long total, string keyword, Guid? circleId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime, int pageNo = 1, int pageSize = 10);
        List<SearchCircleDto> SearchCircle(SearchBaseQueryModel queryModel);
        ResultIdDto SearchIds(string keyword, GlobalEnum.ChannelIndex channel, Guid? userId, int pageNo = 1, int pageSize = 10);
        List<string> GetDistinctHistorySearch(int channel, Guid? userId, Guid? uuid = null, int pageSize = 10);

        PageData SearchSvsSchool(SvsSchoolPageListQuery query);
        List<(int cityId, long totalCount)> SearchSchoolCity(SearchSchoolQuery queryData, bool isVague = false);
        Task<long> SearchSchoolTotalCountAsync(SearchSchoolQuery query);
        Task<List<Guid>> RecommenSchoolIdsAsync(string keyword, int cityId, List<int> areaIds, List<int> grades, List<string> schoolTypeCodes, int? minTotal, int? maxTotal, List<Guid> authIds, List<Guid> courseIds, List<Guid> characIds, int pageIndex = 1, int pageSize = 10);
        Task<List<SearchSchoolExplanationQueryModel>> RecommenSchoolExplainAsync(string keyword, int cityId, List<int> areaIds, List<int> grades, List<string> schoolTypeCodes, int? minTotal, int? maxTotal, List<Guid> authIds, List<Guid> courseIds, List<Guid> characIds, int pageIndex = 1, int pageSize = 10);
        ResultArticleDto SearchArticle(SearchArticleQueryModel queryModel);
        List<(string keyword, long total)> ListSuggestTotal(List<string> keywords, GlobalEnum.ChannelIndex channel, int cityCode);
        Task<PaginationModel<Guid>> SearchSchoolsFullMatch(SearchBaseQueryModel queryModel);
    }
}
