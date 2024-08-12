using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface ISchoolRepository
    {
        /// <summary>
        /// 获取学校分部基本信息
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        SchoolExtensionTotal GetSchoolInfo(Guid eid);

        /// <summary>
        /// 获取学校分部名
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        List<SchoolExtensionTotal> GetSchoolName(List<Guid> eid);

        /// <summary>
        /// 用户中心获取学校信息接口
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        List<SchoolInfoStatus> GetSchoolStatuse(List<Guid> eids);

        /// <summary>
        /// 获取该学校下所有分部信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<SchoolExtension> GetCurrentSchoolAllExt(Guid sid);

        /// <summary>
        /// 获取学校详情
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<SchoolExtension> GetSchoolExtensions(int PageIndex, int PageSize, string SchoolName = null);
        List<SchoolExtension> GetSchoolExtensions(int pageIndex, int pageSize, int provinceCode, int cityCode, int grade, int type);
        int GetSchoolExtensionsCount(string schoolName = null);
        int GetSchoolExtensionsCount(int provinceCode, int cityCode, int grade, int type);
        /// <summary>
        /// 根据学校id与分部id得到该学校分部实体
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="BranchSchoolId"></param>
        /// <returns></returns>
        SchoolExtension GetSchoolExtensionById(Guid SchoolId, Guid BranchSchoolId);

        /// <summary>
        /// 根据分部id集合获取该集合学校数据
        /// </summary>
        /// <param name="Ids">ids：'guid','guid'.....</param>
        /// <returns></returns>
        List<SchoolExtension> GetSchoolByIds(string Ids);
        /// <summary>
        /// 根据学部id得到学校id
        /// </summary>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        SchoolExtension GetSchoolExtension(Guid BranchId);
        List<Guid> GetSchoolIdBySchoolName(string schoolName);

        /// <summary>
        /// 根据学校id获取所有的分部
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        List<SchoolBranch> GetAllSchoolBranch(Guid SchoolId);
        SchoolBranch SchoolBranchById(Guid Id);
        List<SchoolExtensionTotal> AllSchoolSelected(int CityCode, List<SchoolGrade> grade, List<SchoolType> type, List<int> isLodging, int PageIndex, int PageSize, QuestionAndCommentOrder order, List<int> schoolArea = null, bool queryType = true);


        /// <summary>
        /// 获取该学校下指定的招生年级，以及学校类型
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="queryType">true：招生年级，false：学校类型</param>
        /// <param name="grade">招生年级</param>
        /// <param name="schoolType">学校类型</param>
        /// <returns></returns>
        List<Guid> GetSchoolAllHighSchool(Guid SchoolId, bool queryType, SchoolGrade grade = SchoolGrade.Defalut, SchoolType schoolType = SchoolType.unknown);

        /// <summary>
        /// 获取学校分部列表
        /// </summary>
        /// <param name="city"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<SchoolExtListDto> GetSchoolExtList(double longitude, double latitude, int province, int city, int area,
            int[] grade, int orderBy, int distance, int[] type, int[] lodging, int index = 1, int size = 10);

        /// <summary>
        /// 获取学校分部的列表
        /// </summary>
        /// <param name="city"></param>
        /// <param name="grades"></param>
        /// <param name="orderBy"> 1推荐 2口碑 3.附近</param>
        /// <param name="count"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<SchoolExtListDto> GetSchoolExts(Guid extId, double longitude, double latitude, int province, int city, int area,
            int[] grades, int orderBy, int distance, int[] types, int[] lodging, string course, int? diglossia = null, int index = 1, int size = 10);

        /// <summary>
        /// 获取学校详情
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        SchoolExtDto GetSchoolExtDetails(Guid extId);


        /// <summary>
        /// 获取学校详情的copy，去除了学校的有效性判断
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        SchoolExtAnyDto GetSchoolExtDetailsAny(Guid extId);

        IEnumerable<KeyValueDto<Guid, string, double, int, int>> GetschoolChoice(string extId, int grade, int type);

        /// <summary>
        /// 获取学校划片范围
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        string GetSchoolExtRange(Guid extId);

        /// <summary>
        /// 批量获取学校分部的地址
        /// </summary>
        /// <returns></returns>
        List<KeyValueDto<Guid>> GetSchoolExtAddress(params Guid[] extId);
        Task<IEnumerable<KeyValueDto<Guid, int, string, string>>> GetSchoolExtAddress(IEnumerable<Guid> eids);


        //根据学部id获取距离
        KeyValueDto<double?> GetDistance(Guid extId, double longitude, double latitude);

        /// <summary>
        /// 获取所有Tag列表
        /// </summary>
        /// <returns></returns>
        List<Tag> GetTagList();

        /// <summary>
        /// 获取学校id
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        List<KeyValueDto<Guid, Guid?, string>> GetSchoolId(params Guid[] extId);

        /// <summary>
        /// 获取学校的videos
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        List<KeyValueDto<DateTime, string, byte>> GetExtViedeos(Guid extId, byte type = 0);

        /// <summary>
        /// 获取学校分部的招生计划更多信息
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        SchoolExtRecruitDto GetSchoolExtRecruit(Guid extId);

        SchoolExtRecruitDto GetSchoolExtRecruitAny(Guid extId);
        /// <summary>
        /// 获取学部的升学成绩
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        object GetAchievementData(Guid extId, byte type, byte grade, int? year);

        /// <summary>
        /// 获取学部的升学成绩年份
        /// </summary>
        /// <param name="extId">学部ID</param>
        /// <param name="type">学校类型</param>
        /// <param name="grade">招生年级</param>
        /// <returns></returns>
        List<int> GetAchievementYears(Guid extId, byte type, byte grade);


        /// <summary>
        /// 根据分部ID获取学校分部信息
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        List<SchoolExtItemDto> GetSchoolExtListByBranchIds(List<Guid> eids, double latitude = 0, double longitude = 0, bool readIntro = false);

        /// <summary>
        /// 根据学校ID获取学校分部信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        List<SchoolExtItemDto> GetSchoolExtListBySchoolId(Guid sid, double latitude = 0, double longitude = 0);

        /// <summary>
        /// 获取学校（学部）筛选列表
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="orderBy"></param>
        /// <param name="distance"></param>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        /// <param name="lodging"></param>
        /// <param name="authIds"></param>
        /// <param name="characIds"></param>
        /// <param name="abroadIds"></param>
        /// <param name="courseIds"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        List<SchoolExtItemDto> GetSchoolExtFilterList(double longitude, double latitude, List<int> province,
            List<int> city, List<int> area, List<Guid> MetroLineIds, List<int> MetroStationIds,
           List<int> gradeIds, List<int> typeIds, List<bool> discount, List<bool> diglossia, List<bool> chinese,
           int orderBy, decimal distance, int minCost, int maxCost, bool? lodging,
            List<Guid> authIds, List<Guid> characIds, List<Guid> abroadIds, List<Guid> courseIds, int index = 1, int size = 10);
        /// <summary>
        /// 根据是否国际学校获取学校认证标签
        /// </summary>
        /// <param name="International"></param>
        /// <returns></returns>
        List<Tags> GetAuthTagsBySchoolType(bool International);
        /// <summary>
        /// 根据是否国际学校获取特色课程标签
        /// </summary>
        /// <param name="International"></param>
        /// <returns></returns>
        List<Tags> GetCharacTagsBySchoolType(bool International);

        /// <summary>
        /// 获取升学成绩列表
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<KeyValueDto<int, string, double, byte, Guid>> GetAchievementList(Guid extId, byte grade);

        /// <summary>
        ///获取年份升学成绩
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        List<KeyValueDto<Guid, string, double, int>> GetYearAchievementList(Guid extId, byte grade, int year);

        /// <summary>
        /// 根据学校id获取其他分部的名字
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>

        List<KeyValueDto<Guid>> GetSchoolExtName(Guid sid);

        /// <summary>
        /// 获取附近的学位房
        /// </summary>
        /// <param name="extId">分部id</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">精度</param>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        List<KeyValueDto<double, double, string>> GetBuildingData(Guid extId, double? latitude, double? longitude, float distance);
        List<KeyValueDto<double, double, string, int>> GetBuildingPriceData(Guid extId, double? latitude, double? longitude, float distance);

        /// <summary>
        /// 获取学校划片范围的所有点的坐标
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        List<SmallLocation> GetSchoolExtRangePoints(Guid extId);
        /// <summary>
        /// 获取学校的特征
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        SchoolExtCharacterDto GetExtCharacter(Guid extId);


        /// <summary>
        /// 获取学校分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        List<SchoolExtScore> GetExtScore(Guid extId);
        /// <summary>
        /// 获取有效学校分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolExtScore>> GetValidExtScore(Guid extId);

        /// <summary>
        /// 获取周边分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        AmbientScore GetAmbientScore(Guid extId);

        /// <summary>
        /// 获取学校分数细项名称
        /// </summary>
        /// <returns></returns>
        List<SchoolExtScoreIndex> GetExtScoreIndex();

        /// <summary>
        /// 根据分部id获取学校的分部地址详情信息
        /// </summary>
        /// <param name="ExtId"></param>
        /// <returns></returns>
        List<SchoolExtInfoDto> GetSchoolExtInfoDto(List<Guid> ExtId);

        List<SchoolDataES> GetSchoolDataBySchoolId(Guid schoolId);
        List<SchoolDataES> GetSchoolData(int pageNo, int pageSize, DateTime lastTime);

        /// <summary>
        /// 学校推荐
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        List<KeyValueDto<Guid, Guid, string, string, int>> RecommendSchool(byte type, byte grade, int city, Guid extid, int top);

        /// <summary>
        /// 获取评分学部信息
        /// </summary>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <param name="scoreType">1.评论 2.学校</param>
        /// <returns></returns>
        List<KeyValueDto<Guid, byte, long, Guid>> GetScoreExtData(int city, int top, byte scoreType);



        bool InsertEESchoolData(bool isFirst);
        List<EESchool> GetEESchoolData(int pageIndex, int pageSize, DateTime time);

        bool InsertSchoolScore();
        List<SchoolScoreData> GetSchoolScoreData(int pageNo, int pageSize);

        /// <summary>
        /// 获取点评、提问 学校卡片
        /// </summary>
        /// <param name="City"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<SchoolCQDto> QuerySchoolCard(bool QueryComment, int City, int PageIndex, int PageSize);


        /// <summary>
        /// 获取学校与当前定位位置相差的距离
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        List<DisparityDto> GetSchoolDisparities(List<Guid> extIds, double longitude, double latitude);
        ///// <summary>
        ///// 获取学校基本信息
        ///// </summary>
        ///// <param name="eid"></param>
        ///// <returns></returns>
        //SchoolExtension GetSchoolPartial(Guid eid);
        /// <summary>
        /// 人员管理平台 第一次学校数据导入ES
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        List<SchoolSearchImport> SchoolSearchImport(int PageIndex, DateTime CreateTime);
        /// <summary>
        /// 后台管理学校查询
        /// </summary>
        /// <param name="SchId"></param>
        /// <returns></returns>
        List<SchoolSearchImport> BDSchDataSearch(List<Guid> SchId);

        SchoolFeedback SchoolFeedback(Guid eid);
        SchoolExtension GetSchoolByNo(long no);

        System.Threading.Tasks.Task<List<SchoolImageDto>> GetSchoolImages(Guid eid, int[] type);

        System.Threading.Tasks.Task<List<KeyValueDto<DateTime, string, byte, string, string>>> GetSchoolVideos(Guid extId, byte type = 0);

        /// <summary>
        /// 获取学部对口学校的JSON
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<string> GetCounterPartByEID(Guid eid);
        /// <summary>
        /// 获取学部学段
        /// </summary>
        /// <param name="eids">学部ID(复数)</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, SchoolGrade>>> GetGrades(IEnumerable<Guid> eids);
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
        Task<IEnumerable<SchoolExtAggDto>> GetSchoolExtAggs(IEnumerable<Guid> extIds);
        Task<IEnumerable<Guid>> GetAvailableExtIds(IEnumerable<Guid> extIds);
        Task<IEnumerable<TagFlat>> GetTagFlats();

        Task<IEnumerable<string>> GetTagTypes(Guid eid);
        Task<IEnumerable<(Guid ExtId, string SchoolName)>> GetSchoolNameOnlyAsync(IEnumerable<Guid> extIds);
        Task<IEnumerable<SchoolNameTypeArea>> GetSchoolNameTypeAreaAsync(IEnumerable<Guid> extIds);
        Task<IEnumerable<(long No, Guid ExtId)>> GetExtIdByNosAsync(IEnumerable<long> nos);
    }
}
