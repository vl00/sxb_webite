using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using Sxb.Web.Areas.Article.Models;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using ProductManagement.Framework.Foundation;
using PMS.OperationPlateform.Domain.DTOs;
using Sxb.Web.ViewModels.School;
using PMS.Search.Application.IServices;
using PMS.School.Application.IServices;
using PMS.CommentsManage.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using static PMS.Search.Domain.Common.GlobalEnum;
using PMS.Infrastructure.Domain.Enums;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Models;
using Microsoft.Extensions.Options;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.Areas.Article.Models.Article;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Domain.Entities;
using PMS.Infrastructure.Application.IService;
using PMS.OperationPlateform.Domain.Enums;
using Sxb.Web.Areas.Common.Models.Search;
using static Sxb.Web.Areas.Article.Models.Article.ArticleSearchTools;

namespace Sxb.Web.Areas.Article.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleController : ApiBaseController
    {

        IArticleService _articleService;
        ISearchService _searchService;
        ISchService _schService;
        ISchoolService _schoolService;
        ISchoolInfoService _schoolInfoService;
        ISchoolRankService _schoolRankService;
        ISchoolCommentService _schoolCommentService;
        ICollectionService _collectionService;
        IUserService _userService;
        IQuestionInfoService _questionInfoService;

        IImportService _importService;
        IGeneralTagService _generalTagService;
        ImageSetting _imageSetting;
        private readonly IQuestionsAnswersInfoService _questionsAnswers;
        private readonly ICityInfoService _cityInfoService;

        public ArticleController(IArticleService articleService, IArticleCoverService articleCoverService, ISearchService searchService, ISchoolService schoolService, ISchoolRankService schoolRankService, ISchoolCommentService schoolCommentService, IImportService importService, IGeneralTagService generalTagService, IUserService userService, ISchoolInfoService schoolInfoService, IOptions<ImageSetting> options, ICollectionService collectionService, ISchService schService, IQuestionInfoService questionInfoService, IQuestionsAnswersInfoService questionsAnswers, ICityInfoService cityInfoService)
        {
            _articleService = articleService;
            _searchService = searchService;
            _schoolService = schoolService;
            _schoolRankService = schoolRankService;
            _schoolCommentService = schoolCommentService;
            _importService = importService;
            _generalTagService = generalTagService;
            _userService = userService;
            _schoolInfoService = schoolInfoService;
            _imageSetting = options.Value;
            _collectionService = collectionService;
            _schService = schService;
            _questionInfoService = questionInfoService;
            _questionsAnswers = questionsAnswers;
            _cityInfoService = cityInfoService;
        }


        /// <summary>
        /// 设置历史记录
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="channel">原定分channel保存历史记录, 暂没用</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public void SetHistory(string keyword, ChannelIndex channel, Guid? userId, ClientType clientType)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            if (clientType == ClientType.UnKown)
            {
                clientType = Request.Headers.GetClientType();
            }

            userId = userId == Guid.Empty ? null : userId;

            var historyChannel = channel.GetDefaultValue<int>();

            //es history
            var cityCode = Request.GetLocalCity();
            var uuid = Request.GetDevice();

            //没有登录又没有uuid则不插入记录
            if (userId == null && string.IsNullOrWhiteSpace(uuid))
            {
                return;
            }
            _importService.SetHistory(clientType.GetDefaultValue<string>(), historyChannel, keyword, cityCode, userId, uuid);
        }

        /// <summary>
        /// 获取文章卡片信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseResult GetArticleCardInfo(Guid id)
        {
            var article = _articleService.Get(id);
            if (article == null)
            {
                return ResponseResult.Failed("未找到数据");
            }
            var cover = _articleService.ArticleCoverToLink(article.Covers.FirstOrDefault());
            return ResponseResult.Success(new ArticleCardViewModel()
            {
                No = UrlShortIdUtil.Long2Base32(article.No),
                Title = article.title,
                Author = article.author,
                Cover = !string.IsNullOrWhiteSpace(cover) ? cover : "https://cos.sxkid.com/v4source/pc/imgs/home/sxb.png"
            });
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="no"></param>
        /// <param name="sxbtoken"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult Get(string no, Guid? id, string sxbtoken = "")
        {
            ArticleDetailDto data = _articleService.GetArticleDetail(no, id, UserId, sxbtoken);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// get article no by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GetNo(Guid id)
        {
            var article = _articleService.Get(id);
            return ResponseResult.Success(new
            {
                No = UrlShortIdUtil.Long2Base32(article.No)
            });
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GetArticleSearchTools(int cityId)
        {

            var tools = new ArticleSearchTools();

            List<AreaDto> areas = new List<AreaDto>();
            List<GradeTypeDto> types = new List<GradeTypeDto>();

            var tasks = new List<Task>() {
                Task.Run(async () => areas = await _cityInfoService.GetAreaCode(cityId)),
                Task.Run(() => types = _schoolService.GetGradeTypeList())
            };
            await Task.WhenAll(tasks);

            tools.Areas = areas;
            tools.SchoolGrades = SchoolGradeDetailVM.ConvertTo(types);
            tools.ArticleTypes = ArticleType.Policy.GetAllItemsDesc()
                .Select(s => new ArticleTypeVM (){ Name = s.desc, Value = s.value })
                .ToList();
            return ResponseResult.Success(tools);
        }

        /// <summary>
        /// 获取文章列表
        /// 1. 默认严选文章,  keyword和tagId为空
        /// 2. 传入tagId, 标签查询文章
        /// 3. 传入keyword, es搜索查询文章
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityCode"></param>
        /// <param name="tagId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetPagination(string keyword, int cityCode, string tagName = null, int pageIndex = 1, int pageSize = 20)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                SetHistory(keyword, ChannelIndex.Article, UserId, ClientType.Mobile);
            }

            string uuid = Request.GetDevice();
            cityCode = cityCode == 0 ? Request.GetLocalCity() : cityCode;

            Guid? tagId = null;
            if (!string.IsNullOrWhiteSpace(tagName))
                tagId = await _generalTagService.GetTagIDByNameForArticle(tagName);

            var data = await _articleService.GetPagination(keyword, cityCode, tagId, UserId, uuid, pageIndex, pageSize);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取文章分页列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetPagination([FromBody] ArticleSearchRequest search)
        {
            #region null if
            search.AreaCodes = search.AreaCodes ?? new List<int>();
            search.ArticleTypes = search.ArticleTypes ?? new List<ArticleType>();
            search.SchoolGradeIds = search.SchoolGradeIds ?? new List<int>();
            search.SchoolTypes = search.SchoolTypes ?? new List<string>();
            #endregion

            var keyword = search.Keyword;
            SetHistory(keyword, ChannelIndex.Article, UserId, search.ClientType);

            int cityCode = search.CityCode;
            cityCode = cityCode == 0 ? Request.GetLocalCity() : cityCode;

            //对于一级选中, 添加其子级到查询条件
            if (search.SchoolGradeIds.Any())
            {
                var allGradeTypes = _schoolService.GetGradeTypeList();
                var gradeTypes = allGradeTypes
                    .Where(s => search.SchoolGradeIds.Contains(s.GradeId))
                    .SelectMany(s => s.Types.Select(t => t.TypeValue));

                search.SchoolTypes = search.SchoolTypes.Concat(gradeTypes).ToList();
            }

            var data = await _articleService.GetPaginationByEs(keyword, userId: null, cityCode
                , search.AreaCodes, search.ArticleTypes, search.SchoolTypes
                , search.OrderBy, search.PageIndex, search.PageSize);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取文章分页列表实时的查询总数量
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GetPaginationTotal([FromBody] ArticleSearchRequest search)
        {
            int cityCode = search.CityCode;
            cityCode = cityCode == 0 ? Request.GetLocalCity() : cityCode;

            var data = await _articleService.GetPaginationTotalByEs(search.Keyword, userId: null, cityCode
                , search.AreaCodes, search.ArticleTypes, search.SchoolTypes);

            return ResponseResult.Success(new
            {
                TotalCount = data
            });
        }

        /// <summary>
        /// 获取隐藏文章列表（seo用）
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GetHidePagination(int pageIndex = 1, int pageSize = 20)
        {
            var data = _articleService.GetHidePagination(pageIndex, pageSize);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 热门攻略
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetHotArticles()
        {
            var data = await _articleService.GetHotArticles(Request.GetLocalCity(), UserId, 5);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 文章列表 - 推荐文章 猜你喜欢
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetRecommendArticles(int pageIndex = 1, int pageSize = 10)
        {
            //var data = await _articleService.GetRecommendArticles(Request.GetLocalCity(), UserId, pageIndex, pageSize);
            var data = await _articleService.GetRecommendArticles(Request.GetLocalCity(), pageIndex, pageSize);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 和articleId相关的文章
        /// 文章详情 - 推荐文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetCorrelationArticles(Guid articleId)
        {
            //var articles = _articleService.GetCorrelationArticle(articleId);
            //var dto = _articleService.ConvertToArticleDto(articles);
            var dto = await _articleService.GetRecommendArticles(articleId, 1, 10);
            return ResponseResult.Success(dto);
        }

        /// <summary>
        /// 和articleId相关的学校点评
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GetCorrelationComments(Guid articleId)
        {
            List<SCMDto> scms = _articleService.GetCorrelationSCMS(articleId, UserId);
            if (scms != null && scms.Any())
            {
                //查找学校
                var schoolInfos = _schoolInfoService.GetSchoolSectionByIds(scms.Select(x => x.SchoolExtId)?.ToList());

                //查找点评的学校
                scms.ForEach(s =>
                {
                    //添加域名
                    s.Imgs = s.Imgs.Select(img => _imageSetting.QueryImager + img).ToList();

                    var info = schoolInfos.Where(x => s.SchoolExtId == x.SchoolSectionId).FirstOrDefault();
                    if (info != null)
                    {
                        s.SchoolName = info.SchoolName;
                        s.SchoolCommentCount = info.SectionCommentTotal;
                        s.SchoolScore = info.SchoolAvgScore;
                        s.SchoolStarts = info.SchoolStars;
                        s.SchoolAvgScore = Math.Round(info.SchoolAvgScore / 20, 1);
                        s.SchoolNo = info.SchoolNo;
                    }
                    else
                    {
                        s.SchoolName = "暂无该学校";
                        s.SchoolCommentCount = 0;
                        s.SchoolScore = 0;
                    }
                });
            }

            return ResponseResult.Success(scms);
        }

        /// <summary>
        /// 热门标签
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetHotTags()
        {
            var data = await _articleService.GetHotTags();
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 相关学校
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetArticleSchools(Guid articleId, int pageSize = 10)
        {
            //政策文章(以地区和关联的学校类型来作学校和文章的关联)
            double latitude = NumberParse.DoubleParse(Request.GetLatitude(), 0);
            double longitude = NumberParse.DoubleParse(Request.GetLongitude(), 0);

            List<Guid> sids = _articleService.GetArticleSchoolIds(articleId, latitude, longitude, pageSize).ToList();
            List<SchoolExtFilterDto> schools = sids.Count > 0 ?
                await _schoolService.ListExtSchoolByBranchIds(sids, latitude, longitude) : new List<SchoolExtFilterDto>();

            //请求点评数据,榜单数据
            var schoolIds = schools.Select(p => p.ExtId).ToList();
            var rank = _schoolRankService.GetH5SchoolRankInfoBy(schoolIds.ToArray());
            var comment = _schoolCommentService.GetSchoolCardComment(schoolIds, SchoolSectionOrIds.SchoolSectionIds, default);

            #region convert to viewmodel
            var result = schools.Select(s => new SchoolExtListItemViewModel()
            {
                SchoolId = s.Sid,
                BranchId = s.ExtId,
                SchoolName = s.Name,
                CityName = s.City,
                AreaName = s.Area,
                Distance = s.Distance,
                LodgingType = (int)s.LodgingType,
                LodgingReason = s.LodgingType.Description(),
                International = s.International,
                Cost = s.Tuition,
                Tags = s.Tags,
                Score = s.Score,
                CommentTotal = s.CommentCount,
                Type = s.Type,

                Ranks = rank.Where(p => p.SchoolId == s.ExtId).Select(p => new SchoolExtListItemViewModel.SchoolRankInfoViewModel
                {
                    RankId = p.RankId,
                    Cover = p.Cover,
                    Ranking = p.Ranking,
                    RankUrl = p.rank_url,
                    SchoolId = p.SchoolId,
                    Title = p.Title,
                    No = p.No
                }).ToList(),
                Comment = comment.FirstOrDefault(p => p.SchoolSectionId == s.ExtId)?.Content,
                CommontNo = UrlShortIdUtil.Long2Base32(comment.FirstOrDefault(p => p.SchoolSectionId == s.ExtId)?.No ?? 0),
                SchoolNo = s.SchoolNo
            });
            #endregion
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 选校相关关键词
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetCorrelationTags(Guid articleId)
        {
            var data = _articleService.GetCorrelationTags(articleId, true);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 获取附近的学校
        /// </summary>
        /// <returns></returns>
        [Description("获取附近的学校")]
        public async Task<ResponseResult> GetNearBySchools()
        {
            double latitude = NumberParse.DoubleParse(Request.Cookies["latitude"], 0);
            double longitude = NumberParse.DoubleParse(Request.Cookies["longitude"], 0);
            //附近学校(查询时间接近1s，在无高并发的情况下,决定采用异步请求的方式优化用户体验)
            return ResponseResult.Success(await _schService.GetNearSchoolsBySchType((longitude, latitude)));
        }



        /// <summary>
        /// 查询QACard的数据
        /// </summary>
        /// <param name="QID"></param>
        /// <returns></returns>
        [Description("获取问答卡片数据")]
        public ResponseResult GetQACardData(Guid qaid, Guid? answerId = null)
        {
            var (question, sectionCommentCount) = _questionInfoService.GetQuestionInfoById(qaid);
            if (question == null) return ResponseResult.Failed("无内容");

            if (answerId != null)
            {
                //指定显示answerId
                var answers = _questionsAnswers.GetAnswerInfoDtoByIds(new List<Guid>() { answerId.Value });
                if (answers != null && answers.Count != 0)
                {
                    question.answer = answers;
                }
            }
            question.answer = question.answer ?? new List<PMS.CommentsManage.Application.ModelDto.AnswerInfoDto>();

            var userids = question.answer.Select(q => q.UserId).ToList();
            var users = _userService.ListUserInfo(userids);

            var schoolInfo = _schoolInfoService.QuerySchoolInfo(question.SchoolSectionId).Result;

            var QACard = new QACardViewModel()
            {
                QID = question.Id,
                replys = question.answer.Select(a => new QACardViewModel.Reply()
                {
                    id = a.Id,
                    content = a.AnswerContent,
                    uname = users.FirstOrDefault(q => q.Id == a.UserId)?.NickName
                }).ToList(),
                title = question.QuestionContent,
                replyTotalCount = question.AnswerCount,
                sname = schoolInfo.SchoolName,
                sqacount = sectionCommentCount,
                LodgingType = schoolInfo.LodgingType,
                SchoolType = schoolInfo.SchoolType,
                ShortSchoolNo = schoolInfo.ShortSchoolNo,
                ShortNo = UrlShortIdUtil.Long2Base32(question.No).ToLower()
            };

            return ResponseResult.Success(QACard);
        }

        /// <summary>
        /// 获取最新文章
        /// </summary>
        public async Task<ResponseResult> GetNewest(int count = 9)
        {
            var result = ResponseResult.Failed();
            var finds = await _articleService.GetNewest(count);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds.Select(p => new
                {
                    p.Title,
                    p.Time,
                    Url = $"/article/{p.No}.html"
                }));
            }
            return result;
        }
    }
}
