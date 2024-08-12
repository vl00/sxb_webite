using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IArticleRepository : IBaseQueryRepository<article>
    {
        IEnumerable<article> SelectSubScribeArticles(
             Article_SubscribePreference preference, SchoolGrade[] grades, Domain.Enums.SchoolType[] schoolTypes, Guid[] schoolIds, int offset = 0, int limit = 20);

        /// <summary>
        /// 查询文章
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <param name="isShow">是否显示</param>
        /// <returns></returns>
        article GetArticle(Guid id, bool isShow);

        /// <summary>
        /// 查询相关文章
        /// </summary>
        /// <param name="articleID">文章ID</param>
        /// <param name="top">取top几条</param>
        /// <returns></returns>
        IEnumerable<article> GetCorrelationArticle(Guid id, int top = 10);

        /// <summary>
        /// /查询关联的群组二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<GroupQRCode> GetCorrelationGQRCodes(Guid id);

        /// <summary>
        /// 查询关联绑定的标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<tag_bind> GetCorrelationTags(Guid id, bool ms);

        /// <summary>
        /// 查询相关的学部点评
        /// 后台编辑关联关系
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Article_SCMBinds> GetCorrelationSCMs(Guid id);

        ///// <summary>
        ///// 查询原创文章
        ///// </summary>
        ///// <param name="total"></param>
        ///// <param name="pageIdx"></param>
        ///// <param name="pageCount"></param>
        ///// <returns></returns>
        //IEnumerable<article> GetOgArticles(out int total, int pageIdx, int pageCount = 20);

        /// <summary>
        /// 查询文章除去html字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        article GetArticleNoHtml(Guid id);

        /// <summary>
        /// 查询相关联的学校类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Article_SchoolTypes> GetCorrelationSchoolTypes(Guid id);

        /// <summary>
        /// 查询相关联的地区
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Article_Areas> GetCorrelationAreas(Guid id);

        /// <summary>
        /// 查询相关联的学校
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Article_SchoolBind> GetCorrelationSchool(Guid id);

        /// <summary>
        // 获取18个学部类型枚举
        /// </summary>
        /// <returns></returns>
        IEnumerable<Article_SchoolTypes> GetArticleSchoolTypes();

        /// <summary>
        /// 查询文章订阅配置信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Article_SubscribePreference GetArticleSubscribeInfoByUserId(Guid userId);

        ///// <summary>
        ///// 查询文章订阅配置信息(详细连表)
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>

        //Article_SubscribePreference GetArticleSubscribeDetailInfoByUserId(Guid userId);

        /// <summary>
        /// 查询用户订阅的文章
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<article> GetUserSubscribeArticles(Guid userId, int currentPage, int pageSize);

        /// 通过学校类型和地区查询关联的政策性文章
        /// </summary>
        /// <param name="schoolTypes"></param>
        /// <param name="locals"></param>
        /// <returns></returns>
        (IEnumerable<article> articles, int total) SelectPolicyArticles(List<Domain.DTOs.SchoolType> schoolTypes, List<Local> locals, bool isPage, int offset = 0, int limit = 20);

        /// <summary>
        /// 查询学部关联的对比性文章
        /// </summary>
        /// <param name="branchIds"></param>
        /// <returns></returns>
        (IEnumerable<article> articles, int total) SelectComparisionArticles(List<Guid> branchIds, bool isPage, int offset = 0, int limit = 20);

        /// <summary>
        ///传入一批ID查询出文章
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<article> SelectByIds(Guid[] ids, string[] fileds = null);

        IEnumerable<Article_ESData> GetArticleData(int pageNo, int pageSize, DateTime lastTime);

        /// <summary>
        /// 分页查询 带有UV的视图view_articlejoinuv
        /// </summary>
        /// <returns></returns>
        (IEnumerable<view_articlejoinuv>, int total) SelectView_ArticleJoinUVPage(string where, object param = null, string order = null, string[] fileds = null, bool isPage = false, int offset = 0, int limit = 20);

        /// <summary>
        /// 分页查询 达人发表的文章列表
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
        /// 获取文章ID
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<Guid> GetArticleId(List<Guid> ids);

        /// <summary>
        /// 获取文章ViewCounts
        /// </summary>
        /// <param name="aids"></param>
        /// <returns></returns>
        IEnumerable<article> GetArticleViewCounts(IEnumerable<Guid> aids);

        Dictionary<int, string> GetNearArticle(int no);
        article GetArticleByNo(long no);

        /// <summary>
        /// 获取热门标签
        /// </summary>
        /// <param name="limit">获取数量</param>
        /// <returns></returns>
        Task<IEnumerable<tag_bind>> GetHotTags(int limit = 30);

        /// <summary>
        /// 检查便签ID是否被绑定
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, bool>> CheckTagIDExist(IEnumerable<Guid> ids);

        /// <summary>
        /// 根据便签ID分页
        /// </summary>
        /// <returns></returns>
        Task<(IEnumerable<article>, int)> PageByTagID(Guid tagID, int offset = 0, int limit = 20);
        Guid? GetAuthorTalentId(Guid articleId);
        Task<IEnumerable<Guid>> GetLastestArticleIds(int size);

        Task<IEnumerable<article>> GetNewest(int count = 9);

        Task<Guid?> GetTalentUserId(Guid articleId);
        Task<IEnumerable<article>> GetTopViewCounts(int cityId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize);
    }
}