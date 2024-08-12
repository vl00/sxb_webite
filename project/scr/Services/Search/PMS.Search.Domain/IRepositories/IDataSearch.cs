using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace PMS.Search.Domain.IRepositories
{
    public interface IDataSearch
    {
        SearchSuggestResult ListSuggest(string keyword,int channel, int cityCode);
        SearchSuggestResult ListSuggest(string keyword, ChannelIndex channel, int cityCode);
        SearchSuggestResult ListSuggest(SearchAllQueryModel queryModel);

        List<SchoolResult> SearchSchool(SearchSchoolQueryModel queryData,out long total,bool isVague = false);

        List<SchoolStatsResult> SearchSchoolCostStats(SearchSchoolQueryModel queryData, bool isVague = false);

        List<SearchComment> SearchComment(bool? lodging, string keyword, List<string> keywords, Guid? userId,
            List<int> cityIds, List<int> areaIds, List<int> gradeIds, List<int> typeIds,
            int orderBy, Guid? Eid, out long total, int pageNo = 1, int pageSize = 10);
        List<SearchQuestion> SearchQuestion(bool? lodging,string keyword, List<string> keywords, Guid? userId,
            List<int> cityIds, List<int> areaIds, List<int> gradeIds, List<int> typeIds,
            int orderBy,Guid? Eid, out long total, int pageNo = 1, int pageSize = 10);
        List<SearchLive> SearchLive(out long total, SearchLiveQueryModel queryModel);
        List<SearchSchoolRank> SearchSchoolRank(out long total, SearchBaseQueryModel queryModel);
        //List<SearchArticle> SearchArticle(out long total, SearchArticleQueryModel queryModel);

        SearchSchoolMap SearchMapSchool(int level, string keyword,
            double swLat, double swLng, double neLat, double neLng, int cityCode,
            List<Guid> metroLineIds, List<int> metroStationIds,
            List<int> grades, List<string> schoolTypeCodes,
            int minCost, int maxCost, int minStudentCount, int maxStudentCount,
             int minTeacherCount, int maxTeacherCount, bool? lodging, bool? canteen,
            List<Guid> authIds, List<Guid> characIds, List<Guid> abroadIds, List<Guid> courseIds);

        List<SearchHistory> GetHistorySearch(Guid? UserId, string UUID, int channel, int pageSize);
        List<string> GetHotSearch(int channel, int pageSize, Guid? userId = null, int? cityCode = null, int min = 10);

        List<SearchEESchool> SearchEESchool(string keyword);

        List<SearchSignSchool> SearchSingSchool(string keyword, out long total, int pageNo = 1, int pageSize = 10);

        BTSearchResult BTSchoolSearch(BDSchoolSearch search);

        List<SearchAll> SearchAll(string keyword, int searchType, Guid? userId, out long total, int pageNo = 1, int pageSize = 10);

        (List<Guid>, string scrollid) GetArticleIds(string scrollid);
        
        List<SearchTopic> SearchTopic(out long total, string keyword, Guid? circleId, Guid? circleUserId, Guid? loginUserId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime, int pageNo = 1, int pageSize = 10);

        List<int> SearchTopicTags(string keyword, Guid? circleId, Guid? circleUserId, Guid? loginUserId, Guid? creator, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime);

        List<SearchCircle> SearchCircle(out long total, SearchBaseQueryModel queryModel);

        List<SearchId> SearchIds(out long total, string keyword, ChannelIndex channel, Guid? userId, int pageNo = 1, int pageSize = 10);
        List<string> GetDistinctHistorySearch(int channel, Guid? userId, Guid? uuid, int pageSize);

        PageData SearchSvsSchool(SvsSchoolPageListQuery query);
        List<(int cityId, long totalCount)> SearchSchoolCity(SearchSchoolQueryModel queryData, bool isVague = false);
        Task<long> SearchSchoolTotalCountAsync(SearchSchoolQueryModel queryData, bool isVague = false);
        Task<long> ListSuggestTotalAsync(string keyword, ChannelIndex channel, int cityCode);
    }
}
