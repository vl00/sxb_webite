using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    using Domain.Entitys;
    using Domain.DTOs;
    using System.Threading.Tasks;
    using PMS.OperationPlateform.Application.Dtos;
    using PMS.OperationPlateform.Domain.Enums;
    using ProductManagement.Infrastructure.Models;
    using PMS.Search.Domain.Common;
using PMS.Search.Domain.QueryModel;

    public interface IArticleService
    {

        /// <summary>
        /// 获取精选文章列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AppServicePageResultDto<ArticleGetChoicenessResultDto> GetChoiceness(AppServicePageRequestDto<ArticleGetChoicenessRequestDto> request);

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AppServiceResultDto<ArticleGetDetailResultDto> GetDetail(ArticleGetDetailRequestDto request);


        /// <summary>
        /// 查询订阅的文章
        /// </summary>
        /// <param name="preference"></param>
        /// <param name="schoolIds"></param>
        /// <returns></returns>
        Task<IEnumerable<article>> GetSubscribeArticles(Article_SubscribePreference preference, string iSchoolAuth, int offset = 0, int limit = 20);

        /// <summary>
        /// 获取显示给用户的文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <param name="isShow">是否显示的文章</param>
        /// <returns></returns>
        article GetShowForUserArticle(Guid id, int no, bool isPreview = false);

        /// <summary>
        /// 获取上一篇和下一篇
        /// <para>返回 No,Title</para>
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        Dictionary<int, string> GetNearArticle(int no);

        /// <summary>
        /// 获取展示给自己人的文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        article GetShowForOnOfUsArticle(Guid id);

        /// <summary>
        /// 查询相关的二维码群组
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns></returns>
        List<GroupQRCode> GetCorrelationGQRCodes(Guid id);

        /// <summary>
        /// 查询相关的标签
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns></returns>
        List<TagDto> GetCorrelationTags(Guid id, bool ms);

        /// <summary>
        /// 查询相关文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns></returns>
        List<article> GetCorrelationArticle(Guid id);

        /// <summary>
        /// 查询相关学部评论
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <returns></returns>
        List<SCMDto> GetCorrelationSCMS(Guid id, Guid? userId);

        /// <summary>
        /// 查询相关联的学校类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Article_SchoolTypes> GetCorrelationSchoolTypes(Guid id);

        /// <summary>
        /// 查询对比型文章相关的学校
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<SchoolExtDto> GetCorrelationSchool(Guid id);

        /// <summary>
        /// 查询相关联的区域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Article_Areas> GetCorrelationAreas(Guid id);

        /// <summary>
        /// 查询严选文章
        /// </summary>
        (IEnumerable<article> articles, int total) GetStrictSlctArticles(Guid? userId, string uuID, int cityId, int offset = 0, int limit = 20);

        /// <summary>
        /// 添加访问量
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        bool AddAccessCount(article article);

        /// <summary>
        /// 根据学校ID查询学校分部
        /// </summary>
        /// <param name="sid">学校ID</param>
        /// <returns></returns>

        IEnumerable<OnlineSchoolExtension> GetSchoolExtensionByParent(Guid sid);

        /// <summary>
        /// 获取省地区
        /// </summary>
        /// <returns></returns>
        IEnumerable<local_v2> GetProvince();

        /// <summary>
        /// 获取子地区
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        IEnumerable<local_v2> GetSubLocal(int pid);

        /// <summary>
        /// 查询18个学部类型
        /// </summary>
        /// <returns></returns>
        IEnumerable<Article_SchoolTypes> GetSchoolTypes();

        /// <summary>
        /// 设置文章订阅配置信息
        /// </summary>
        /// <param name="article_SubscribePreference"></param>
        /// <returns></returns>
        bool SetArticleSubscribeInfo(Article_SubscribePreference article_SubscribePreference);

        /// <summary>
        /// 获取用户订阅的文章
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<article> GetUserSubscribeArticles(Guid userId, int currentPage, int pageSize);

        /// <summary>
        /// 获取文章订阅配置
        /// </summary>
        Article_SubscribePreference GetArticleSubscribeInfoByUserId(Guid userId);

        /// <summary>
        /// 获取文章订阅配置信息(详细信息)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        //Article_SubscribePreference GetArticleSubscribeDetailInfoByUserId(Guid userId);

        List<SchoolExtDto> SearchCorrelationSchool(string searchVal, article article);

        /// <summary>
        /// 根据学校类型和地区获取政策性文章
        /// </summary>
        /// <param name="schoolTypes">学校类型</param>
        /// <param name="locals">地区</param>
        /// <param name="top"></param>
        /// <returns></returns>
        IEnumerable<article> GetPolicyArticles(List<Domain.DTOs.SchoolType> schoolTypes, List<Local> locals, int top = 10);

        /// <summary>
        /// 根据学校类型和地区获取政策性文章
        /// </summary>
        /// <param name="schoolTypes">学校类型</param>
        /// <param name="locals">地区</param>
        /// <param name="top"></param>
        /// <returns></returns>
        (IEnumerable<article> articles, int total) GetPolicyArticles_PageVersion(List<Domain.DTOs.SchoolType> schoolTypes, List<Local> locals, int offset = 0, int limit = 20);

        /// <summary>
        /// 根据学部ID获取相关的对比性文章
        /// </summary>
        /// <param name="branchIds">分部ID列表</param>
        /// <param name="top"></param>
        /// <returns></returns>
        IEnumerable<article> GetComparisionArticles(List<Guid> branchIds, int? top = 10);

        /// <summary>
        /// 根据学部ID获取相关的对比性文章(分页版本)
        /// </summary>
        /// <param name="branchIds">分部ID列表</param>
        /// <param name="top"></param>
        /// <returns></returns>
        (IEnumerable<article> articles, int total) GetComparisionArticles_PageVersion(List<Guid> branchIds, int offset = 0, int limit = 20);

        /// <summary>
        /// 根据id获取文章列表
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="readCovers">是否返回covers</param>
        /// <returns></returns>
        IEnumerable<article> GetByIds(Guid[] ids, bool readCovers = false);

        /// <summary>
        ///Get Effective By id arrays or no arrays.
        /// </summary>
        /// <param name="ids">业务主键</param>
        /// <param name="nos">自增主键</param>
        /// <param name="nos">indicate the field is bring html field or not</param>
        /// <returns></returns>
        IEnumerable<article> GetEffectiveBy(Guid[] ids, int[] nos, bool compelete);

        IEnumerable<Article_ESData> GetArticleData(int pageNo, int pageSize, DateTime lastTime);

        /// <summary>
        /// PC上学帮搜索列表
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="aids"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        (IEnumerable<article> articles, int total) PCSearchList(
            int cityId,
            Guid[] aids,
            int offset,
            int limit,
            bool getCover = false
            );

        /// <summary>
        /// 查找热门攻略
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="userId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<(IEnumerable<view_articlejoinuv> articles, int total)> HotPointArticles(
             int cityId,
             Guid? userId,
             int offset,
             int limit);

        /// <summary>
        /// 查找猜你喜欢文章
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        (IEnumerable<article> articles, int total) IThinkYouWillLike(
           Guid? userId,
           int offset,
           int limit
           );

        /// <summary>
        /// 查询达人发步的文章列表
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IEnumerable<article> GetTalent(Guid talentId, int page, int size, List<Guid> articleIds = default);

        /// <summary>
        /// 获取达人文章数量
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        long GetArticleTotal(Guid talentId, DateTime? startTime, DateTime? endTime);

        /// <summary>
        /// 获取已删除的文章ID列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<Guid> GetDeletedArticleId(List<Guid> ids);

        ArticleDto GetArticle(Guid? id, long? no, bool includeHtml = true);

        /// <summary>
        /// 传入一批文章ID获取包含完整字段的文章
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<ArticleDto> GetArticles(IEnumerable<Guid> ids, bool includeHtml = true);
        article GetArticleByNo(long no);
        PaginationModel<article> SearchArticles(SearchArticleQueryModel queryModel);
        List<article> SearchArticles(IEnumerable<Guid> ids);

        /// <summary>
        /// 获取热门标签
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TagDto>> GetHotTags(int limit = 30);

        /// <summary>
        /// 检查TagId是否被绑定
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<Guid, bool>> CheckTagIDInbind(IEnumerable<Guid> tagIDs);

        /// <summary>
        /// 根据便签Id分页
        /// </summary>
        /// <param name="tagID">便签ID</param>
        /// <param name="offset">偏移量</param>
        /// <param name="limit">获取量</param>
        /// <returns></returns>
        Task<(IEnumerable<article>, int)> PageByTagID(Guid tagID, int offset = 0, int limit = 20);

        /// <summary>
        /// 获取列表隐藏类型的所有文章
        /// </summary>
        /// <returns></returns>
        List<article> GetIsHideInLists();

        /// <summary>
        /// 获取热门文章, 获取50个, 随机前pageSize个
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="userId"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<ArticleDto>> GetHotArticles(int cityCode, Guid? userId, int pageSize);

        /// <summary>
        /// 获取文章相关学校Id
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<Guid> GetArticleSchoolIds(Guid articleId, double latitude, double longitude, int pageSize);
        string ArticleCoverToLink(article_cover c);
        Task<PaginationModel<ArticleDto>> GetPaginationByEs(string keyword, Guid? userId, int? cityId, List<int> areaId, List<ArticleType> articleTypes, List<string> schTypes, ArticleOrderBy orderBy, int pageIndex = 1, int pageSize = 10);
        Task<PaginationModel<ArticleDto>> GetPagination(string keyword, int cityCode, Guid? tagId, Guid? userId, string uuid, int pageIndex, int pageSize);
        PaginationModel<ArticleDto> GetHidePagination(int pageIndex, int pageSize);


        ArticleDetailDto GetArticleDetail(string no, Guid? id, Guid? userId, string sxbtoken = "");
        Task<IEnumerable<ArticleDto>> GetRecommendArticles(int cityCode, Guid? userId, int pageIndex, int pageSize);
        List<ArticleDto> ConvertToArticleDto<T>(IEnumerable<T> articles) where T : article;
        article Get(Guid id);
        Task<IEnumerable<Guid>> GetLastestArticleIds();
        Task<IEnumerable<Guid>> GetRandomLastestArticleIds(int takeSize, IEnumerable<Guid> excludeIds);
        Task<IEnumerable<Guid>> GetLastestArticleIds(int takeSize);

        Task<IEnumerable<ArticleDto>> GetNewest(int count = 9);

        Task<Guid?> GetTalentUserId(Guid articleId);
        Task<long> GetPaginationTotalByEs(string keyword, Guid? userId, int? cityId, List<int> areaId, List<ArticleType> articleTypes, List<string> schTypes);
        Task<IEnumerable<ArticleDto>> GetRecommendArticles(int cityId, int pageIndex, int pageSize);
        Task<IEnumerable<ArticleDto>> GetRecommendArticles(Guid articleId, int pageIndex, int pageSize);
    }
}