using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.Entities.Mongo;
using PMS.School.Domain.Enum;
using PMS.School.Infrastructure.Common;
using PMS.Search.Application.ModelDto.Query;
using PMS.Search.Domain.QueryModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface ISchoolService
    {
        /// <summary>
        /// 获取学校列表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<SchoolDto> GetSchoolExtensions(int PageIndex, int PageSize, string SchoolName = null);
        List<SchoolDto> GetSchoolExtensions(int PageIndex, int PageSize, int provinceCode, int cityCode, int grade, int type);

        /// <summary>
        /// 获取该学校下所有分部信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<SchoolExtension> GetCurrentSchoolAllExt(Guid sid);

        int GetSchoolExtensionsCount(int provinceCode, int cityCode, int grade, int type);
        int GetSchoolExtensionsCount(string SchoolName = null);
        /// <summary>
        /// 根据学校id与分部id得到该学校分部实体
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="BranchSchoolId"></param>
        /// <returns></returns>
        SchoolDto GetSchoolExtensionById(Guid SchoolId, Guid BranchSchoolId);
        /// <summary>
        /// 根据分部id集合获取该集合学校数据
        /// </summary>
        /// <param name="Ids">ids：'guid','guid'.....</param>
        /// <returns></returns>
        List<SchoolDto> GetSchoolByIds(string Ids);
        SchoolDto GetSchoolExtension(Guid BranchId);
        /// <summary>
        /// 根据学校id获取所有的分部
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        List<SchoolBranch> GetAllSchoolBranch(Guid SchoolId);

        List<Guid> GetSchoolIdBySchoolName(string schoolName);

        /// <summary>
        /// 获取学校列表
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="grade"></param>
        /// <param name="orderBy"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<SchoolExtListDto> GetSchoolExtListAsync(double longitude, double latitude, int province, int city, int area, int[] grade, int orderBy, int[] type, int distance, int[] lodging, int index = 1, int size = 10);

        /// <summary>
        /// 获取学校分部列表
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="grade"></param>
        /// <param name="orderBy"> 1推荐 2口碑 3.附近</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<SchoolExtListDto> GetSchoolExtListsAsync(Guid extId, double longitude, double latitude, int province, int city, int area, int[] grade, int orderBy, int[] type, int distance, int[] lodging, string course = "", int? diglossia = null, int index = 1, int size = 10);

        /// <summary>
        /// 获取学校基本信息
        /// </summary>
        /// <returns></returns>
        System.Threading.Tasks.Task<SchoolExtSimpleDto> GetSchoolExtSimpleDtoAsync(double longitude, double latitude, Guid extId);

        /// <summary>
        /// 获取学部的详情
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<SchoolExtDto> GetSchoolExtDtoAsync(Guid extId, double latitude, double longitude);

        /// <summary>
        /// 获取升学去向
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<KeyValueDto<Guid, string, double, int, int>> GetschoolChoice(string extId, int grade, int type);

        /// <summary>
        /// 根据分部ID获取学校分部信息
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        Task<List<SchoolExtFilterDto>> ListExtSchoolByBranchIds(List<Guid> eids, double latitude = 0, double longitude = 0, bool readIntro = false, bool onlyAuthTag = false);

        /// <summary>
        /// 根据学校ID获取学校分部信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<List<SchoolExtFilterDto>> ListExtSchoolBySchoolId(Guid sid, double latitude = 0, double longitude = 0);

        /// <summary>
        /// 学校（学部）筛选列表
        /// </summary>
        /// <param name="query"></param>
        Task<List<SchoolExtFilterDto>> PageExtSchool(ListExtSchoolQuery query);

        /// <summary>
        /// 根据是否国际学校获取学校认证标签
        /// </summary>
        /// <param name="International"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, string>> GetAuthTagsBySchoolType(bool International);
        /// <summary>
        /// 根据是否国际学校获取特色课程标签
        /// </summary>
        /// <param name="International"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, string>> GetCharacTagsBySchoolType(bool International);


        //获取学校的地址
        List<KeyValueDto<Guid>> GetSchoolExtAdress(params Guid[] extId);
        /// <summary>
        /// 获取学校的地址
        /// <para>Key -> ExtID</para>
        /// <para>Value -> SchoolNo</para>
        /// <para>Message -> SchoolName - ExtName</para>
        /// <para>Data -> SchoolAddress</para>
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDto<Guid, int, string, string>>> GetExtAddresses(IEnumerable<Guid> eids);

        /// <summary>
        /// 获取所有Tag
        /// </summary>
        List<TagDto> GetTagList();

        /// <summary>
        /// 获取学校分类列表
        /// </summary>
        /// <returns></returns>
        List<SchoolTypeDto> GetSchoolTypeList();


        /// <summary>
        /// 获取学校年级分类列表
        /// </summary>
        /// <returns></returns>
        List<GradeTypeDto> GetGradeTypeList();

        /// <summary>
        /// 获取学校划片范围
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        string GetSchoolRange(Guid extId);

        /// <summary>
        /// 获取附近三公里学位房的信息
        /// </summary>
        /// <returns></returns>
        Task<List<KeyValueDto<double, double, string>>> GetBuildingDataAsync(Guid extId, double? latitude, double? longitude, float distance = 3000);
        Task<List<KeyValueDto<double, double, string, int>>> GetBuildingPriceDataAsync(Guid extId, double? latitude, double? longitude, float distance = 3000);

        /// <summary>
        /// 获取附近三公里学位房的信息
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="neLat"></param>
        /// <param name="swLat"></param>
        /// <param name="neLng"></param>
        /// <param name="swLng"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        Task<List<KeyValueDto<double, double, string>>> GetBuildingDataAsync(Guid extId, double? latitude, double? longitude, double neLat, double swLat, double neLng, double swLng, float distance = 3000);
        /// <summary>
        /// 获取学校划片范围的所有点的坐标
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        List<SmallLocation> GetSchoolExtRangePointsAsync(Guid extId);
        /// <summary>
        /// 获取学部的招生简章
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        SchoolExtRecruitDto GetSchoolExtRecruit(Guid extId);
        /// <summary>
        /// 获取学校升学成绩的内容
        /// </summary>
        /// <param name="extId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        object GetAchData(Guid extId, byte grade, byte type, int year);
        /// <summary>
        /// 获取学校升学成绩的所有年份
        /// </summary>
        /// <param name="extId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        List<int> GetAchYears(Guid extId, byte grade, byte type);

        /// <summary>
        /// 获得升学成绩的列表（year,name,count）
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <returns></returns>

        List<KeyValueDto<int, string, double, byte, Guid>> GetAchievementList(Guid extId, byte grade);
        /// <summary>
        /// 获取年份升学成绩的列表
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        List<KeyValueDto<Guid, string, double, int>> GetYearAchievementList(Guid extId, byte grade, int year);

        /// <summary>
        /// 获取学校的视频
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<KeyValueDto<DateTime, string, byte>> GetExtVideo(Guid extId, byte type = 0);

        /// <summary>
        /// 根据学校id获取分部姓名
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<KeyValueDto<Guid>> GetSchoolExtName(Guid sid);

        /// <summary>
        /// 根据分部id集合查询学校简介0
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<SchoolCollectionDto>> GetCollectionExtsAsync(Guid[] ids, (Guid Sid, Guid Eid)[] list = null, bool contain = false);

        /// <summary>
        /// 根据分部id获取相同类型的学校
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        Task<List<SchoolExtItemDto>> GetIdenticalSchool(Guid SchoolId, int PageIndex, int PageSize);

        /// <summary>
        ///获取学校分部特征信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<SchoolExtCharacterDto> GetSchoolCharacterAsync(Guid extId);
        /// <summary>
        ///获取学部分分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<List<SchoolExtScore>> GetSchoolExtScoreAsync(Guid extId);
        /// <summary>
        ///获取学部分分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolExtScore>> GetSchoolValidExtScoreAsync(Guid extId);

        /// <summary>
        ///获取周边分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        AmbientScore GetAmbientScore(Guid extId);

        /// <summary>
        /// 获取学部分数选项
        /// </summary>
        /// <returns></returns>
        Task<List<SchoolExtScoreIndex>> schoolExtScoreIndexsAsync();

        List<SchoolESDataDto> GetSchoolData(int pageNo, int pageSize, DateTime lastTime);
        List<SchoolESDataDto> GetSchoolDataBySchoolId(Guid SchoolId);
        bool InsertSchoolScore();
        List<SchoolScoreDataDto> GetSchoolScoreData(int pageNo, int pageSize);


        List<CommentsManage.Application.ModelDto.SchoolInfoDto> AllSchoolSelected(int cityCode, List<SchoolGrade> grade, List<SchoolType> type, List<int> isLodging, int pageIndex, int pageSize, QuestionAndCommentOrder order, List<int> schoolArea, bool queryType);
        /// <summary>
        /// 获取分部的推荐分部
        /// </summary>
        /// <param name="extid"></param>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        Task<List<KeyValueDto<Guid, Guid, string, string, int>>> RecommendSchoolAsync(Guid extid, byte type, byte grade, int city, int top);

        /// <summary>
        /// 获取pc首页热门学校
        /// </summary>
        /// <param name="city"></param>
        /// <param name="day"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<Dictionary<string, List<SchoolCollectionDto>>> GetPCHotSchool(int city, int day = 7, int count = 6);

        /// <summary>
        /// 获取PC首页数据
        /// </summary>
        /// <param name="city"></param>
        /// <param name="day"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<Dictionary<string, List<SchoolCollectionDto>>> GetPCIndexDataAsync(int city, int day = 7, int count = 6);


        bool InsertEESchoolData(bool isFirst);
        List<EESchool> GetEESchoolData(int pageIndex, int pageSize, DateTime time);

        SchoolExtDto GetSchoolDetailById(Guid extId, double latitude = 0, double longitude = 0);

        /// <summary>
        /// 获取学校与当前定位位置相差的距离
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        List<DisparityDto> GetSchoolDisparities(List<Guid> extIds, double longitude, double latitude);
        /// <summary>
        /// 人员管理平台 第一次学校数据导入ES
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        List<SchoolSearchImport> SchoolSearchImport(int PageIndex, DateTime CreateTime = default);
        List<SchoolSearchImport> BDSchDataSearch(List<Guid> SchId);
        /// <summary>
        /// 获取学校详情
        /// 进行缓存
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<SchoolExtDto> GetSchoolExtDetailsAsync(Guid extId);

        SchoolFeedback SchoolFeedback(Guid eid);
        SchoolDto GetSchoolByNo(long no);

        /// <summary>
        /// 获取分部考试年份
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        List<string> GetSchoolPastExamYears(string eid);

        /// <summary>
        /// 获取分部考试年份
        /// </summary>
        /// <param name="eid">分部ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        string GetSchoolPastExam(string eid, string year);
        /// <summary>
        /// 获取历史数据的年份
        /// </summary>
        /// <param name="eid">学部Id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        List<string> GetSchoolFieldYears(string eid, string field);
        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <param name="eid">学部Id</param>
        /// <param name="field">字段名</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        string GetSchoolFieldContent(string eid, string field, string year);

        /// <summary>
        /// 获取学校图片
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="type">图片类型</param>
        /// <returns></returns>
        Task<List<SchoolImageDto>> GetSchoolImages(Guid eid, SchoolImageType[] type);

        Task<List<KeyValueDto<DateTime, string, byte, string, string>>> GetSchoolVideos(Guid extId, byte type = 0);

        /// <summary>
        /// 获取学部详情(包含图片)
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<SchoolExtensionDto> GetSchoolExtensionDetails(Guid extId);


        /// <summary>
        /// GetSchoolExtensionDetails的副本，但是不做学校有效性判断
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<SchoolExtensionAnyDto> GetSchoolExtensionDetailsAny(Guid extId);

        /// <summary>
        /// 获取学部详情(包含图片)（ps:去掉了“暂未收录”的默认值。）
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<SchoolExtensionDto> GetSchoolExtensionDetailsNoZanWeiShouLu(Guid extId);

        Task<(List<SchoolExtFilterDto> data, long total)> SearchSchools(SearchSchoolQuery query, bool onlyAuthTag = false);
        Task<List<SchoolExtFilterDto>> SearchSchools(IEnumerable<Guid> ids, double latitude = 0, double longitude = 0, bool readIntro = false, bool onlyAuthTag = false);

        string GetCounterPartByYear(int? year = null);


        /// <summary>
        /// 从Mongo获取学校周边信息
        /// </summary>
        /// <returns></returns>
        Task<GDParams> GetSurroundInfo(Guid eid);
        /// <summary>
        /// 获取区域的周边平均信息
        /// </summary>
        /// <returns></returns>
        Task<GDParams> GetSurroundAvgInfo(string schFtype, int areaCode);
        /// <summary>
        /// 从Mongo获取Ai字段信息
        /// </summary>
        /// <returns></returns>
        Task<AiParams> GetAiParams(Guid eid);
        Task<AiParams> GetAiParamsAvg(string schFtype, int areaCode);

        /// <summary>
        /// 获取对口学校
        /// <para>
        /// Key -> 学名名称
        /// </para>
        /// <para>
        /// Value -> 学部ID
        /// </para>
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<string, Guid>>> GetCounterPartByEID(Guid eid);
        /// <summary>
        /// 获取学部学段
        /// <para>
        /// Key -> 学部ID
        /// </para>
        /// <para>
        /// Value -> 学段
        /// </para>
        /// </summary>
        /// <param name="eids">学部ID(复数)</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, SchoolGrade>>> GetGradesByEIDs(IEnumerable<Guid> eids);
        /// <summary>
        /// 获取学部的城市与区域
        /// <para>
        /// Key -> 学部ID
        /// </para>
        /// <para>
        /// Value -> (CityCode, AreaCode)
        /// </para>
        /// </summary>
        /// <param name="eids">学部ID(复数)</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, (int, int)>>> GetCityAndAreaByEIDs(IEnumerable<Guid> eids);

        Task<IEnumerable<WeChatSouYiSouSchool>> GetWeChatSouYiSouSchools(List<Guid> extIds);
        Task<IEnumerable<TagFlat>> GetTagFlats();

        Task<IEnumerable<string>> GetTagTypes(Guid eid);
        Task<IEnumerable<(Guid ExtId, string SchoolName)>> GetSchoolNameOnlyAsync(IEnumerable<Guid> extIds);
        Task<IEnumerable<CostSectionDto>> GetSearchSchoolCostAsync();

        /// <summary>
        /// 获取列表推荐学校
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityId"></param>
        /// <param name="areaIds"></param>
        /// <param name="grades"></param>
        /// <param name="schoolTypeCodes"></param>
        /// <param name="minTotal"></param>
        /// <param name="maxTotal"></param>
        /// <param name="authIds"></param>
        /// <param name="courseIds"></param>
        /// <param name="characIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<SchoolExtFilterDto>> GetRecommendSchools(string keyword, int cityId, List<int> areaIds, List<int> grades, List<string> schoolTypeCodes, int? minTotal, int? maxTotal, List<Guid> authIds, List<Guid> courseIds, List<Guid> characIds, int pageIndex = 1, int pageSize = 10);
        Task<IEnumerable<SchoolNameDto>> RecommendSchoolsAsync(Guid extId, int pageIndex, int pageSize);
        Task<(List<SchoolExtFilterDto> data, long total)> SearchSchoolsFullMatch(SearchBaseQueryModel queryModel, double latitude = 0, double longitude = 0);
        Task<IEnumerable<SchoolNameTypeArea>> GetSchoolNameTypeAreaAsync(IEnumerable<Guid> extIds);
        Task<IEnumerable<(long No, Guid ExtId)>> GetExtIdByNosAsync(IEnumerable<long> nos);
    }

}
