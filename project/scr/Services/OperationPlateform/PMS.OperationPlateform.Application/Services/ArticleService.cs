using System;
using System.Collections.Generic;
using System.Linq;

namespace PMS.OperationPlateform.Application.Services
{
    using AutoMapper;
    using Dapper;
    using iSchool;
    using iSchool.Data.API;
    using iSchool.Internal.API.UserModule;
    using IServices;
    using Microsoft.Extensions.Logging;
    using NPOI.SS.Formula.PTG;
using Org.BouncyCastle.Crypto;
    using PMS.CommentsManage.Application.IServices;
    using PMS.Infrastructure.Application.IService;
    using PMS.OperationPlateform.Application.Dtos;
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.OperationPlateform.Domain.IRespositories;
    using PMS.School.Infrastructure;
    using PMS.Search.Application.IServices;
    using PMS.Search.Application.ModelDto;
    using PMS.Search.Application.ModelDto.Query;
    using PMS.Search.Domain.Common;
using PMS.Search.Domain.QueryModel;
    using PMS.TopicCircle.Domain.Repositories;
    using PMS.UserManage.Application.IServices;
    using PMS.UserManage.Application.ModelDto.Talent;
    using PMS.UserManage.Domain.Common;
    using ProductManagement.API.Http.Interface;
    using ProductManagement.Framework.Cache.Redis;
    using ProductManagement.Framework.Foundation;
    using ProductManagement.Infrastructure.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Threading.Tasks;
    using static PMS.UserManage.Domain.Common.EnumSet;

    public class ArticleService : IArticleService
    {
        /// <summary>
        /// 通用的文章查询字段，除非详情，否则不带html内容
        /// </summary>
        private string[] gm_fileds = new[] { " [id]",
                      "[title]",
                      "[author]",
                      "[author_origin]",
                      "[time]",
                      "[url]",
                      "[type]",
                      "[url_origin]",
                      "[layout]",
                      "[img]",
                      "[overview]",
                      "[toTop]",
                      "[show]",
                      "[linkOnly]",
                      "[viewCount]",
                      "[viewCount_r]",
                      "[assistant]",
                      "[city]",
                      "[createTime]",
                      "[creator]",
                      "[updateTime]",
                      "[updator]" ,
                      "[No]"};

        private string[] all_fileds = new[] { " [id]",
                      "[title]",
                      "[author]",
                      "[author_origin]",
                      "[time]",
                      "[url]",
                      "[type]",
                      "[url_origin]",
                      "[layout]",
                      "[img]",
                      "[overview]",
                      "[html]",
                      "[toTop]",
                      "[show]",
                      "[linkOnly]",
                      "[viewCount]",
                      "[viewCount_r]",
                      "[assistant]",
                      "[city]",
                      "[createTime]",
                      "[creator]",
                      "[updateTime]",
                      "[updator]" ,
                      "[No]"};

        //访问学部点评
        private ISchoolCommentService schoolCommentService;

        private ISchoolRepository schoolRepository;

        //访问用户信息
        private IUserService userService;
        private ITalentService _talentService;
        private ICircleRepository _circleRepository;

        private IArticleRepository articleRepository;
        private IArticleCommandRepository _articleCommandRepository;
        private ILocalV2Repository localV2Repository;
        private IArticleCoverRepository articleCoverRepository;
        private IArticle_SchoolBindRepository article_SchoolBindRepository;
        private UserApiServices userApiServices;
        private IKeyValueRespository keyValueRespository;
        private IGroupQRCodeRespository groupQRCodeRespository;

        private ILogger logger;
        private ISchoolDataClient schoolDataClient;
        private IEasyRedisClient _easyRedisClient;

        private IArticleCoverService _articleCoverService;
        private readonly ISearchService _searchService;
        private readonly IArticleSearchService _articleSearchService;
        private readonly IArticleCommentService _articleCommentService;
        private readonly IHistoryService _historyService;
        private readonly ICollectionService _collectionService;
        private readonly IMapper _mapper;

        private readonly ICityInfoService _cityInfoService;
        private readonly IRecommendClient _recommendClient;

        public ArticleService(IArticleRepository articleRepository
            , ILogger<ArticleService> logger
            , ISchoolDataClient schoolDataClient
            , ISchoolCommentService schoolCommentService
            , IUserService userService
            , ISchoolRepository schoolRepository
            , ILocalV2Repository localV2Repositor
            , UserApiServices userApiServices
            , IArticleCoverRepository articleCoverRepository
            , IEasyRedisClient _easyRedisClient
            , IArticle_SchoolBindRepository article_SchoolBindRepository
            , IKeyValueRespository keyValueRespository
            , IGroupQRCodeRespository groupQRCodeRespository
            , IArticleCoverService articleCoverService
            , ISearchService searchService
            , IArticleCommentService articleCommentService
            , IHistoryService historyService
            , ITalentService talentService
            , ICircleRepository circleRepository
            , ICollectionService collectionService
            , IArticleCommandRepository articleCommandRepository
            , IMapper mapper, IArticleSearchService articleSearchService, ICityInfoService cityInfoService
            , IRecommendClient recommendClient)
        {
            this.articleRepository = articleRepository;
            this.logger = logger;
            this.schoolDataClient = schoolDataClient;
            this.schoolCommentService = schoolCommentService;
            this.userService = userService;
            this.schoolRepository = schoolRepository;
            this.localV2Repository = localV2Repositor;
            this.userApiServices = userApiServices;
            this.articleCoverRepository = articleCoverRepository;
            this._easyRedisClient = _easyRedisClient;
            this.article_SchoolBindRepository = article_SchoolBindRepository;
            this.keyValueRespository = keyValueRespository;
            this.groupQRCodeRespository = groupQRCodeRespository;
            _articleCoverService = articleCoverService;
            _searchService = searchService;
            _articleCommentService = articleCommentService;
            _historyService = historyService;
            _talentService = talentService;
            _circleRepository = circleRepository;
            _collectionService = collectionService;
            _articleCommandRepository = articleCommandRepository;
            _mapper = mapper;
            _articleSearchService = articleSearchService;
            _cityInfoService = cityInfoService;
            _recommendClient = recommendClient;
        }

        public async Task<IEnumerable<article>> GetSubscribeArticles(Article_SubscribePreference preference, string iSchoolAuth, int offset = 0, int limit = 20)
        {
            var grades = preference.SchoolGradeEnums()?.ToArray();
            var schoolTypes = preference.SchoolTypeEnums()?.ToArray();

            Guid[] schoolIds = null;
            if (!string.IsNullOrEmpty(iSchoolAuth) && preference.IsPushSubscibeSchool.GetValueOrDefault())
            {
                //客户端没登录就不用去获取用户的收藏ID 了
                schoolIds = await this.userApiServices.GetSchoolCollectionID(iSchoolAuth);
            }

            var articles = this.articleRepository.SelectSubScribeArticles(preference, grades, schoolTypes, schoolIds, offset, limit);
            if (articles.Any())
            {
                //查询背景图片
                var covers = this.articleCoverRepository.GetCoversByIds(articles.Select(a => a.id).ToArray());
                articles = articles.Select(a =>
                {
                    a.Covers = covers.Where(c => a.id == c.articleID).ToList();
                    return a;
                });
            }

            return articles;
        }

        public bool AddAccessCount(article article)
        {
            //article.viewCount = article.viewCount.GetValueOrDefault() + 1;
            bool addReult = _articleCommandRepository.AddViewCount(article.id, article.viewCount.GetValueOrDefault());
            if (!addReult)
            {
                //article.viewCount = article.viewCount.GetValueOrDefault() - 1;
            }
            return addReult;
        }

        public List<Article_Areas> GetCorrelationAreas(Guid id)
        {
            return this.articleRepository.GetCorrelationAreas(id).ToList();
        }

        public List<article> GetCorrelationArticle(Guid id)
        {
            var articles = this.articleRepository.GetCorrelationArticle(id);
            var covers = this.articleCoverRepository.GetCoversByIds(articles.Select(a => a.id).ToArray());
            articles = articles.Select(a =>
            {
                a.Covers = covers.Where(c => c.articleID == a.id).OrderBy(c => c.sortID).ToList();
                return a;
            });
            return articles?.ToList();
        }

        public List<GroupQRCode> GetCorrelationGQRCodes(Guid id)
        {
            //先查看绑定的
            var binds = this.articleRepository.GetCorrelationGQRCodes(id).ToList();
            if (binds != null && binds.Any())
            {
                //如果绑定里有，取绑定的
                return binds;
            }
            else
            {
                //如果绑定里没有，取默认值
                var kv = this.keyValueRespository.GetKey("defaultQrCodes");
                var jarr = Newtonsoft.Json.Linq.JArray.Parse(kv.Value);
                var qrIds = jarr.Select(item => int.Parse(item.ToString()));
                var qrcodes = this.groupQRCodeRespository.GetGroupQRCodesBy(qrIds);
                return qrcodes.ToList();
            }
        }

        public List<SchoolExtDto> GetCorrelationSchool(Guid id)
        {
            return this.articleRepository.GetCorrelationSchool(id).Select(s =>
            {
                return new SchoolExtDto()
                {
                    SchoolId = s.SchoolId.GetValueOrDefault()
                };
            }).ToList();
        }

        public List<Article_SchoolTypes> GetCorrelationSchoolTypes(Guid id)
        {
            return this.articleRepository.GetCorrelationSchoolTypes(id).ToList();
        }

        public List<SCMDto> GetCorrelationSCMS(Guid id, Guid? userId)
        {
            var scm_binds = this.articleRepository.GetCorrelationSCMs(id);

            var scmDtos = this.schoolCommentService.GetSchoolCardComment(
                  scm_binds.Select(s => Guid.Parse(s.SCMId)).ToList(),
                   CommentsManage.Domain.Common.SchoolSectionOrIds.IdS,
                     userId != null ? userId.GetValueOrDefault() : default(Guid)
                  );



            var scms = scmDtos.Select(_scm =>
                  {
                      var _userInfo = this.userService.GetUserInfo(_scm.UserId) ?? new UserManage.Application.ModelDto.UserInfoDto();
                      if (_scm.IsAnony)
                      {
                          _userInfo.NickName = "匿名账户";
                          _userInfo.HeadImgUrl = string.Empty.ToHeadImgUrl();
                      }

                      var talent = _talentService.GetTalentByUserId(_scm.UserId.ToString());
                      SCMDto scm = new SCMDto();
                      scm.No = UrlShortIdUtil.Long2Base32(_scm.No).ToLower();
                      scm.HeadImgUrl = _userInfo.HeadImgUrl;
                      scm.Name = _userInfo.NickName;
                      scm.TalentType = talent?.type;
                      scm.Role = Application.Transfer.EnumsTransfer.UserRoleEnumToSCMRoleEnum(_userInfo.VerifyTypes.Length == 0 ? UserRole.Member : (UserRole)_userInfo.VerifyTypes[0]);
                      scm.Content = _scm.Content;
                      scm.Date = _scm.CreateTime.ToString("yyyy年MM月dd日");
                      scm.IsAuth = _scm.IsRumorRefuting;
                      scm.IsEssence = _scm.IsSelected;
                      scm.IsAnony = _scm.IsAnony;
                      scm.Id = _scm.Id;
                      scm.ReplyCount = _scm.ReplyCount;
                      scm.LikeCount = _scm.LikeCount;
                      scm.Imgs = _scm.Images;
                      scm.IsExprience = (_scm.Score?.IsAttend).GetValueOrDefault();
                      scm.SchoolStarts = _scm.StartTotal;
                      scm.IsLike = _scm.IsLike;
                      scm.SchoolId = _scm.SchoolId;
                      scm.SchoolExtId = _scm.SchoolSectionId;

                      scm.Score = CommonHelper.MapperProperty<CommentsManage.Application.ModelDto.CommentScoreDto, SCMDto.CommentScoreDto>(_scm.Score);
                      return scm;
                  });

            return scms.ToList();
        }

        public List<TagDto> GetCorrelationTags(Guid id, bool ms)
        {
            var result = new List<TagDto>();
            var tbs = this.articleRepository.GetCorrelationTags(id, ms);
            if (tbs == null || !tbs.Any())
            {
                return result;
            }
            try
            {
                var res = this.schoolDataClient.GetByIds(tbs.Select(t => t.tagID.ToString()).ToArray()).Result;
                if (res.isOk)
                {
                    return res.data.Select(t => Transfer.APIModelTransferLocalModel.iSchoolDataTagToLocalTag(t)).ToList();
                }
                else
                {
                    throw new Exception("iSchoolData响应 Not OK");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, null);
                return new List<TagDto>();
            }
        }

        public (IEnumerable<article> articles, int total) GetStrictSlctArticles(Guid? userId, string uuID, int cityId, int offset = 0, int limit = 20)
        {
            DynamicParameters parameters = new DynamicParameters();
            //根据cityId找出省份位置
            var local = this.localV2Repository.GetById(cityId);
            StringBuilder where = new StringBuilder(@" 
                    IsDeleted = 0 and
                    show = 1
                    AND IsHideInList = 0
                    AND time<= GETDATE()
                    AND EXISTS(SELECT 1 FROM Article_Areas
                         WHERE  (
                         (ProvinceId IS NULL and CityId IS  NULL and areaId IS  NULL)
                         OR
                         (ProvinceId = @pid and CityId is null and areaId is null)
                         OR
                         (ProvinceId = @pid and CityId = @cid  ))
                         AND 
                        Article_Areas.ArticleId = article.id) ");
            var interst = this.userApiServices.GetUserInterestAsync(userId, uuID.ToString()).GetAwaiter().GetResult();
            if (interst != null && ((interst.grade != null && interst.grade.Any()) || (interst.nature != null && interst.nature.Any())))
            {
                List<string> intrstFilterOr = new List<string>();
                string interstWhere = string.Empty;
                //加入用户画像筛选条件
                var itrstAnd = new List<string>() { " ArticleId = ARTICLE.Id " };
                if (interst.nature != null && interst.nature.Any())
                {
                    var natureOr = new List<string>();
                    foreach (var nture in interst.nature)
                    {
                        natureOr.Add($" SchoolType={nture} ");
                    }
                    if (natureOr.Any())
                    {
                        itrstAnd.Add(string.Join(" OR ", natureOr));
                    }
                }
                if (interst.grade != null && interst.grade.Any())
                {
                    var gradeOr = new List<string>();
                    foreach (var grde in interst.grade)
                    {
                        gradeOr.Add($" SchoolType={grde} ");
                    }
                    if (gradeOr.Any())
                    {
                        itrstAnd.Add(string.Join(" OR ", gradeOr));
                    }
                    intrstFilterOr.Add("(EXISTS(select 1 FROM iSchoolArticle.dbo.Article_PriodBinds WHERE ArticleId = ARTICLE.Id AND PriodId IN @priods ))");
                    parameters.Add("priods", interst.grade);
                }
                if (itrstAnd.Any())
                {
                    itrstAnd = itrstAnd.Select(s => $"({s})").ToList();
                    interstWhere = string.Format(" WHERE {0} ", string.Join(" AND ", itrstAnd));
                }
                intrstFilterOr.Add($@"(EXISTS(SELECT 1 FROM iSchoolArticle.dbo.Article_SchoolTypeBinds A1
                    JOIN iSchoolArticle.dbo.Article_SchoolTypes A2 ON A1.SchoolTypeId = A2.Id
                    {interstWhere} ))");
                where.AppendFormat(@" AND ({0}) ", string.Join(" OR ", intrstFilterOr));

            }
            parameters.AddDynamicParams(new { pid = local?.parent ?? -1, cid = local?.id ?? -1 });
            return this.articleRepository.SelectPage(
                 where: where.ToString(),
                 param: parameters,
                 order: "toTop desc, time desc",
                 fileds: gm_fileds,
                 isPage: true,
                 offset: offset,
                 limit: limit

                 );
        }

        public IEnumerable<OnlineSchoolExtension> GetSchoolExtensionByParent(Guid sid)
        {
            return this.schoolRepository.GetSchoolExtensionByParent(sid);
        }

        public IEnumerable<local_v2> GetProvince()
        {
            return this.localV2Repository.GetByParent(0);
        }

        public IEnumerable<local_v2> GetSubLocal(int pid)
        {
            return this.localV2Repository.GetByParent(pid);
        }

        public IEnumerable<Article_SchoolTypes> GetSchoolTypes()
        {
            return this.articleRepository.GetArticleSchoolTypes();
        }

        public bool SetArticleSubscribeInfo(Article_SubscribePreference article_SubscribePreference)
        {
            var info = this.articleRepository.GetArticleSubscribeInfoByUserId(article_SubscribePreference.UserId.GetValueOrDefault());
            if (info == null)
            {
                return _articleCommandRepository.AddArticleSubscribeInfo(article_SubscribePreference);
            }
            else
            {
                article_SubscribePreference.Id = info.Id;
                return _articleCommandRepository.UpdateArticleSubscribeInfo(article_SubscribePreference);
            }
        }

        public IEnumerable<article> GetUserSubscribeArticles(Guid userId, int currentPage, int pageSize)
        {
            var articles = this.articleRepository.GetUserSubscribeArticles(userId, currentPage, pageSize);
            var covers = this.articleCoverRepository.GetCoversByIds(articles.Select(a => a.id).ToArray());
            articles = articles.Select(a =>
            {
                a.Covers = covers.Where(c => c.articleID == a.id).OrderBy(c => c.sortID).ToList();
                return a;
            });

            return articles;
        }

        public Article_SubscribePreference GetArticleSubscribeInfoByUserId(Guid userId)
        {
            return this.articleRepository.GetArticleSubscribeInfoByUserId(userId);
        }

        //public Article_SubscribePreference GetArticleSubscribeDetailInfoByUserId(Guid userId)
        //{
        //    var article_subscribe =
        //          this.articleRepository.GetArticleSubscribeDetailInfoByUserId(userId)
        //          ?? new Article_SubscribePreference();
        //    //if (article_subscribe.AreaId != null)
        //    //{
        //    // article_subscribe.Area =   this.localV2Repository.GetById(article_subscribe.AreaId.Value);
        //    //}
        //    //else if (article_subscribe.CityId != null)
        //    //{
        //    //    article_subscribe.City = this.localV2Repository.GetById(article_subscribe.CityId.Value);
        //    //}
        //    //else if (article_subscribe.ProvinceId != null)
        //    //{
        //    //    article_subscribe.Province = this.localV2Repository.GetById(article_subscribe.ProvinceId.Value);
        //    //}

        //    //if (article_subscribe.SchoolStageId != null) {
        //    //    article_subscribe.SchoolStage = (SchoolGrade)article_subscribe.SchoolStageId.Value;
        //    //}

        //    return article_subscribe;

        //}

        public IEnumerable<article> GetPolicyArticles(List<Domain.DTOs.SchoolType> schoolTypes, List<Local> locals, int top = 10)
        {
            var result = this.articleRepository.SelectPolicyArticles(schoolTypes, locals, true, 0, top);
            return result.articles;
        }

        public (IEnumerable<article> articles, int total) GetPolicyArticles_PageVersion(List<Domain.DTOs.SchoolType> schoolTypes, List<Local> locals, int offset = 0, int limit = 20)
        {
            var result = this.articleRepository.SelectPolicyArticles(schoolTypes, locals, true, offset, limit);

            return result;
        }

        public IEnumerable<article> GetComparisionArticles(List<Guid> branchIds, int? top = 10)
        {
            var aids = this.article_SchoolBindRepository.GetArticleIds(branchIds.ToArray());
            //var result = this.articleRepository.SelectComparisionArticles(branchIds, true, 0, top.GetValueOrDefault());
            var result = this.articleRepository.SelectPage(
                 where: @" IsDeleted = 0 and show=1 and time<GETDATE() and id in @aids",
                 param: new { aids },
                 order: "toTop desc,time desc",
                 gm_fileds,
                 true,
                 0,
                 top.GetValueOrDefault());

            return result.Item1;
        }

        public (IEnumerable<article> articles, int total) GetComparisionArticles_PageVersion(List<Guid> branchIds, int offset = 0, int limit = 20)
        {
            var aids = this.article_SchoolBindRepository.GetArticleIds(branchIds.ToArray());
            var result = this.articleRepository.SelectPage(
                 where: @" IsDeleted = 0 and show=1 and time<GETDATE() and id in @aids",
                 param: new { aids },
                 order: "toTop desc,time desc",
                  gm_fileds,
                 true,
                 offset,
                 limit);

            return result;
        }

        public List<SchoolExtDto> SearchCorrelationSchool(string searchVal, article article)
        {
            throw new NotImplementedException();
        }

        public article Get(Guid id)
        {
            IEnumerable<article> articles = this.articleRepository.SelectByIds(new Guid[] { id });
            var article = articles.FirstOrDefault();
            if (article != null)
            {
                article.Covers = _articleCoverService.GetCoversByIds(new[] { article.id }).ToList();
            }
            return article;
        }

        public IEnumerable<article> GetByIds(Guid[] ids, bool readCovers = false)
        {
            IEnumerable<article> articles = this.articleRepository.SelectByIds(ids);
            if (readCovers)
            {
                AssembleArticleCovers(articles);
            }
            return articles;
        }

        public IEnumerable<Article_ESData> GetArticleData(int pageNo, int pageSize, DateTime lastTime)
        {
            return this.articleRepository.GetArticleData(pageNo, pageSize, lastTime);
        }

        public article GetShowForUserArticle(Guid id, int no, bool isPreview = false)
        {
            //给前端用户展示的文章必须是当前时间之前的并且是展示状态的
            string where = string.Empty;
            if (!isPreview)
            {
                where = " IsDeleted = 0 and show=1 and time<GETDATE() ";
                if (id != Guid.Empty)
                {
                    where += " and id = @id ";
                }
                else
                {
                    where += " and No = @no ";
                }
            }
            else
            {
                if (id != Guid.Empty)
                {
                    where = " IsDeleted = 0 and id = @id ";
                }
                else
                {
                    where = " IsDeleted = 0 and No = @no ";
                }
            }


            var article = this.articleRepository.Select(
               where: where,
               param: new { id = id, no = no },
               order: null,
               fileds: null
               ).FirstOrDefault();

            return article;
        }

        public article GetShowForOnOfUsArticle(Guid id)
        {
            var article = this.articleRepository.Select(
                where: " IsDeleted = 0 and id = @id",
                param: new { id = id },
                order: null,
                fileds: null
                ).FirstOrDefault();
            return article;
        }

        public (IEnumerable<article> articles, int total) PCSearchList(
            int cityId,
            Guid[] aids,
            int offset,
            int limit,
            bool getCover = false
            )
        {
            DynamicParameters parameters = new DynamicParameters();
            List<string> andWhere = new List<string>() {
               "IsDeleted=0", "show=1","time<=GETDATE()"
            };
            if (cityId > 0)
            {
                andWhere.Add("EXISTS(SELECT 1 FROM Article_Areas WHERE Article_Areas.ArticleId = article.id AND Article_Areas.CityId=@cityId )");
                parameters.Add("cityId", cityId);
            }
            if (getCover)
            {
                andWhere.Add("exists(select 1 FROM article_cover WHERE article_cover.articleID = article.id)");
            }
            if (aids != null && aids.Any())
            {
                andWhere.Add("article.id IN @aids");
                parameters.Add("aids", aids);
            }

            var result = this.articleRepository.SelectPage(
                  where: string.Join(" AND ", andWhere),
                  param: parameters,
                  order: " toTop DESC, time DESC",
                  fileds: gm_fileds
                  , isPage: true
                  , offset: offset
                  , limit: limit
                  );

            return result;
        }

        /// <summary>
        /// 获取热门文章, 获取50个, 随机前五个
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ArticleDto>> GetHotArticles(int cityCode, Guid? userId, int pageSize)
        {
            //热门攻略
            var randArticals = await _easyRedisClient.GetOrAddAsync($"HotPointArticles:{cityCode}:{userId}", async () =>
            {
                var (articles, total) = await HotPointArticles(cityCode, userId, 0, 50);

                var randomArticals = articles.ToList();
                if (articles.Count() > 0 && articles.Count() < 50)
                {
                    var leftArticals = await HotPointArticles(0, userId, articles.Count(), 50 - articles.Count());
                    if (leftArticals.articles.Count() > 0)
                    {
                        randomArticals.AddRange(leftArticals.articles.ToList());
                    }
                }
                randomArticals = randomArticals.Where(p => p.time > DateTime.Now.AddDays(-90)).ToList();//排除90天外的
                return randomArticals;
            }, DateTime.Now.AddMinutes(60 * 12));
            CommonHelper.ListRandom(randArticals);
            return ConvertToArticleDto(randArticals.Take(pageSize));
        }

        public async Task<IEnumerable<ArticleDto>> GetRecommendArticles(int cityCode, Guid? userId, int pageIndex, int pageSize)
        {
            //热门攻略
            var (articles, total) = await HotPointArticles(cityCode, userId, pageIndex, pageSize);

            var randomArticals = articles.ToList();
            CommonHelper.ListRandom(randomArticals);

            return ConvertToArticleDto(randomArticals);
        }

        /// <summary>
        /// 以最近一个月内，浏览量最高的排序列表替代
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ArticleDto>> GetRecommendArticles(int cityId, int pageIndex, int pageSize)
        {
            DateTime startTime = DateTime.Now.AddDays(-30);
            DateTime endTime = DateTime.Now;
            //暂不缓存
            var articles = await articleRepository.GetTopViewCounts(cityId, startTime, endTime, pageIndex, pageSize);

            //数据不足, 向前取
            if (pageIndex == 1 && articles.Count() < 10)
            {
                startTime = DateTime.Now.AddDays(-365);
                articles = await articleRepository.GetTopViewCounts(cityId, startTime, endTime, pageIndex, pageSize);
            }
            return ConvertToArticleDto(articles);
        }

        public async Task<IEnumerable<ArticleDto>> GetRecommendArticles(Guid articleId, int pageIndex, int pageSize)
        {
            var articleIds = await _recommendClient.RecommendArticleIds(articleId, pageIndex, pageSize);
            var articles = GetByIds(articleIds.ToArray(), readCovers: true);
            return ConvertToArticleDto(articles);
        }

        public async Task<(IEnumerable<view_articlejoinuv> articles, int total)> HotPointArticles(
            int cityId,
            Guid? userId,
            int offset,
            int limit)
        {
            int cacheSaveMinute = 60 * 12;
            string listKey = "hotpointarticles:" + cityId + userId + offset + limit;
            string countKey = "hotpointtotal:" + cityId + userId + offset + limit;
            var list = await this._easyRedisClient.GetAsync<IEnumerable<view_articlejoinuv>>(listKey);
            if (list?.Any() == true)
            {
                //有缓存，拿缓存，从缓存拿出来的文章需要更新一下阅读量
                var aids = list.Select(l => l.id);
                var aritcles = this.articleRepository.GetArticleViewCounts(aids);
                if (aritcles?.Any() == true)
                {
                    list = list.Select(l =>
                    {
                        var article = aritcles.FirstOrDefault(a => a.id == l.id) ?? new Domain.Entitys.article();
                        l.viewCount = article.viewCount;
                        l.viewCount_r = article.viewCount_r;
                        return l;
                    });
                }

                var count = await this._easyRedisClient.GetAsync<int>(countKey);
                return (list, count);
            }
            else
            {
                //没缓存，拿库，并且写入缓存
                DynamicParameters parameters = new DynamicParameters();
                List<string> andWhere = new List<string>();
                andWhere.Add("IsDeleted = 0");
                andWhere.Add("SHOW = 1");
                andWhere.Add("TIME < GETDATE()");
                andWhere.Add("TIME > DATEADD(day, -90, GETDATE())");
                if (cityId > 0)
                {
                    andWhere.Add("(EXISTS(SELECT 1 FROM Article_Areas WHERE Article_Areas.ArticleId= view_articlejoinuv.id AND  CityId=@cityId) OR NOT EXISTS(SELECT 1 FROM Article_Areas WHERE view_articlejoinuv.id = Article_Areas.ArticleId) )");
                    parameters.Add("cityId", cityId);
                }
                if (userId != null)
                {
                    andWhere.Add(@"EXISTS(SELECT 1 FROM Article_SchoolTypeBinds WHERE Article_SchoolTypeBinds.ArticleId = view_articlejoinuv.id AND
		                            EXISTS(
				                            SELECT 1 FROM Article_SchoolTypes WHERE Article_SchoolTypeBinds.SchoolTypeId = Article_SchoolTypes.Id --...
			                            )
		                            )
                            ");
                }

                var queryResult = this.articleRepository.SelectView_ArticleJoinUVPage(
                where: string.Join(" AND ", andWhere),
                param: parameters,
                order: "UV desc,c desc, ToTop desc, time DESC",
                fileds: new[] {
                      " [id]",
                      "[title]",
                      "[author]",
                      "[author_origin]",
                      "[time]",
                      "[url]",
                      "[type]",
                      "[url_origin]",
                      "[layout]",
                      "[img]",
                      "[overview]",
                      "[toTop]",
                      "[show]",
                      "[linkOnly]",
                      "[viewCount]",
                      "[viewCount_r]",
                      "[assistant]",
                      "[city]",
                      "[createTime]",
                      "[creator]",
                      "[updateTime]",
                      "[updator]",
                      "[No]",
                      " (select count(1) from Article_Areas where Article_Areas.ArticleId = view_articlejoinuv.id) c "
                },

                isPage: true,
                offset: offset,
                limit: limit
                   );
                await this._easyRedisClient.AddAsync(listKey, queryResult.Item1, DateTime.Now.AddMinutes(cacheSaveMinute), StackExchange.Redis.CommandFlags.FireAndForget);
                await this._easyRedisClient.AddAsync(countKey, queryResult.total, DateTime.Now.AddMinutes(cacheSaveMinute), StackExchange.Redis.CommandFlags.FireAndForget);

                return queryResult;
            }
        }

        public (IEnumerable<article> articles, int total) IThinkYouWillLike(
            Guid? userId,
            int offset,
            int limit
            )
        {
            DynamicParameters parameters = new DynamicParameters();
            List<string> andWhere = new List<string>();
            andWhere.Add("IsDeleted = 0");
            andWhere.Add("SHOW = 1");
            andWhere.Add("TIME < GETDATE()");
            if (userId != null)
            {
                andWhere.Add(@"EXISTS(SELECT 1 FROM Article_SchoolTypeBinds WHERE Article_SchoolTypeBinds.ArticleId = view_articlejoinuv.id AND
		                            EXISTS(
				                            SELECT 1 FROM Article_SchoolTypes WHERE Article_SchoolTypeBinds.SchoolTypeId = Article_SchoolTypes.Id --...
			                            )
		                            )
                            ");
            }
            return this.articleRepository.SelectPage(
           where: string.Join(" AND ", andWhere),
           param: parameters,
           order: "time DESC",
           fileds: gm_fileds,
           isPage: true,
           offset: offset,
           limit: limit
              );
        }

        public IEnumerable<article> GetEffectiveBy(Guid[] ids, int[] nos, bool compelete)
        {
            List<string> primaryConditionOrContact = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            if ((ids?.Any()).GetValueOrDefault())
            {
                primaryConditionOrContact.Add("id in @ids");
                parameters.AddDynamicParams(new { ids });
            }
            if ((nos?.Any()).GetValueOrDefault())
            {
                primaryConditionOrContact.Add("no in @nos");
                parameters.AddDynamicParams(new { nos });
            }
            if (!primaryConditionOrContact.Any())
            {
                return null;
            }

            return this.articleRepository.Select($"({string.Join(" or ", primaryConditionOrContact)}) AND time<=GETDATE()  AND IsDeleted = 0 AND SHOW=1 ", new { ids, nos }, "time desc", compelete ? all_fileds : gm_fileds);
        }

        public IEnumerable<article> GetTalent(Guid talentId, int page, int size, List<Guid> articleIds = default)
        {
            IEnumerable<article> articles = this.articleRepository.GetTalent(talentId, page, size, articleIds);
            AssembleArticleCovers(articles);
            return articles;
        }

        private void AssembleArticleCovers(IEnumerable<article> articles)
        {
            if (articles == null || !articles.Any())
                return;

            AssembleArticleCovers(articles, articles.Select(s => s.id).ToArray());
        }

        private void AssembleArticleCovers(IEnumerable<article> articles, Guid[] articleIds)
        {
            IEnumerable<article_cover> article_Covers = articleCoverRepository.GetCoversByIds(articleIds);
            if (article_Covers == null || !article_Covers.Any())
                return;

            foreach (var article in articles)
            {
                article.Covers = article_Covers.Where(s => s.articleID == article.id).ToList();
            }
        }

        /// <summary>
        /// 获取达人文章数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public long GetArticleTotal(Guid talentId, DateTime? startTime, DateTime? endTime)
            => this.articleRepository.GetArticleTotal(talentId, startTime, endTime);

        public List<Guid> GetDeletedArticleId(List<Guid> ids)
        {
            var articleIds = articleRepository.GetArticleId(ids);
            var delIds = ids.Where(q => !articleIds.Contains(q)).ToList();
            return delIds;
        }

        public ArticleDto GetArticle(Guid? id, long? no, bool includeHtml = true)
        {
            //给前端用户展示的文章必须是当前时间之前的并且是展示状态的
            string where = " IsDeleted = 0 and show=1 and time<GETDATE() And (id=@id or No=@no) ";
            var article = this.articleRepository.Select(
               where: where,
               param: new { id = id, no = no },
               order: null,
               fileds: includeHtml ? all_fileds : gm_fileds)
               .FirstOrDefault();
            if (article != null)
            {
                var covers = _articleCoverService.GetCoversByIds(new Guid[] { article.id });
                article.Covers = covers?.ToList();
            }
            return _mapper.Map<ArticleDto>(article);
        }
        public IEnumerable<ArticleDto> GetArticles(IEnumerable<Guid> ids, bool includeHtml = true)
        {
            var articles = articleRepository.SelectByIds(ids.ToArray(), includeHtml ? all_fileds : gm_fileds);
            if (articles?.Any() == true)
            {
                var covers = _articleCoverService.GetCoversByIds(articles.Select(a => a.id).ToArray());
                foreach (var article in articles)
                {
                    article.Covers = covers.Where(c => c.articleID == article.id)?.ToList();
                }
            }
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public Dictionary<int, string> GetNearArticle(int no)
        {
            return articleRepository.GetNearArticle(no);
        }

        public article GetArticleByNo(long no)
        {
            return articleRepository.GetArticleByNo(no);
        }

        public AppServicePageResultDto<ArticleGetChoicenessResultDto> GetChoiceness(AppServicePageRequestDto<ArticleGetChoicenessRequestDto> request)
        {
            try
            {
                var result = this.GetStrictSlctArticles(request.input.UserId, request.input.UUID, request.input.CityCode, request.offset, request.limit);
                var covers = this._articleCoverService.GetCoversByIds(result.articles.Select(a => a.id).ToArray());
                var datas = result.articles.Select(ac => new ArticleGetChoicenessResultDto()
                {
                    Id = ac.id,
                    Time = ac.time.GetValueOrDefault().ToArticleFormattString(),
                    Title = ac.title,
                    ViweCount = ac.VirualViewCount,
                    Covers = covers.Where(c => c.articleID == ac.id).Select(c => $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{(FileExtension)c.ext}").ToList(),
                    Layout = ac.layout,
                    Digest = ac.html.GetHtmlHeaderString(150),
                    No = UrlShortIdUtil.Long2Base32(ac.No)
                });
                return AppServicePageResultDto<ArticleGetChoicenessResultDto>.Success(datas, result.total);
            }
            catch (Exception ex)
            {
                return AppServicePageResultDto<ArticleGetChoicenessResultDto>.Failure(ex.Message);
            }
        }

        public AppServiceResultDto<ArticleGetDetailResultDto> GetDetail(ArticleGetDetailRequestDto request)
        {

            string where = " IsDeleted = 0 and show=1 and time<GETDATE() ";
            if (request.Id != Guid.Empty)
            {
                where += " and id = @id";
            }
            else
            {
                where += " and No=@no ";
            }


            ArticleGetDetailResultDto detail = this.articleRepository.Select(
               where: where,
               param: new { id = request.Id, no = request.No },
               order: null,
               fileds: null
               ).FirstOrDefault();
            if (detail == null)
            {
                return AppServiceResultDto<ArticleGetDetailResultDto>.Failure("找不到该文章");
            }
            //获取底部二维码
            detail.QRCodes = this.GetCorrelationGQRCodes(detail.Id)?.Select(code => code.Url).ToList();
            //获取主标签
            detail.MainTags = this.GetCorrelationTags(detail.Id, true)?.Select(code => code.TagName).ToList();
            //获取相关文章
            detail.CorrelationArticles = this.GetCorrelationArticle(detail.Id)?.Select<article, ArticleBaseDetailDto>(a => a).ToList();

            return AppServiceResultDto<ArticleGetDetailResultDto>.Success(detail);

        }


        public PaginationModel<article> SearchArticles(SearchArticleQueryModel queryModel)
        {
            var result = _searchService.SearchArticle(queryModel);

            var ids = result.Articles.Select(q => q.Id);
            List<article> _articles = SearchArticles(ids);
            return PaginationModel.Build(_articles, result.Total);
        }

        public List<article> SearchArticles(IEnumerable<Guid> ids)
        {
            var idArr = ids.ToArray();
            var articleList = GetByIds(idArr)?.ToList();

            var _articles = new List<article>();
            if (articleList != null && articleList.Count > 0)
            {
                //查询出目标背景图片
                var effactiveIds = articleList.Select(a => a.id).ToArray();
                var covers = _articleCoverService.GetCoversByIds(effactiveIds);
                var comments = _articleCommentService.Statistics_CommentsCount(idArr);

                if (ids != null)
                {
                    foreach (var id in ids)
                    {
                        var article = articleList.Find(a => a.id == id);
                        if (article != null)
                        {
                            article.Covers = covers.Where(c => c.articleID == article.id).ToList();
                            article.CommentCount = comments.Where(c => c.Id == article.id).FirstOrDefault()?.Count ?? 0;
                            _articles.Add(article);
                        }
                    }
                }
            }

            return _articles;
        }

        public async Task<IEnumerable<TagDto>> GetHotTags(int limit = 30)
        {
            var result = new List<TagDto>();

            var hotTagsRedisKey = "HotTags";

            var redisResult = await _easyRedisClient.GetAsync<IEnumerable<TagDto>>(hotTagsRedisKey);

            if (redisResult?.Any() == true)
            {
                return redisResult;
            }
            else
            {
                await _easyRedisClient.RemoveAsync(hotTagsRedisKey);
                var tbs = await articleRepository.GetHotTags();
                if (tbs == null || !tbs.Any())
                {
                    return result;
                }
                try
                {
                    var res = this.schoolDataClient.GetByIds(tbs.Select(t => t.tagID.ToString()).ToArray()).Result;
                    if (res.isOk)
                    {
                        var apiResult = res.data.Select(t => Transfer.APIModelTransferLocalModel.iSchoolDataTagToLocalTag(t));

                        foreach (var item in tbs)
                        {
                            if (apiResult.Any(p => Guid.TryParse(p.Id, out Guid convertID) && convertID == item.tagID)) result.Add(apiResult.FirstOrDefault(p => new Guid(p.Id) == item.tagID));
                        }
                    }
                    else
                    {
                        throw new Exception("iSchoolData响应 Not OK");
                    }
                    await _easyRedisClient.AddAsync(hotTagsRedisKey, result, TimeSpan.FromDays(1));
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, null);
                }
            }
            return result;
        }

        public async Task<Dictionary<Guid, bool>> CheckTagIDInbind(IEnumerable<Guid> tagIDs)
        {
            if (tagIDs == null || !tagIDs.Any()) return new Dictionary<Guid, bool>();
            return await articleRepository.CheckTagIDExist(tagIDs.Distinct());
        }

        public async Task<(IEnumerable<article>, int)> PageByTagID(Guid tagID, int offset = 0, int limit = 20)
        {
            if (tagID == Guid.Empty) return (new article[0], 0);
            return await articleRepository.PageByTagID(tagID, offset, limit);
        }

        public List<article> GetIsHideInLists()
        {
            return articleRepository.Select("IsDeleted = 0 and IsHideInList = 1").ToList();
        }
        private (IEnumerable<article> articles, int total) GetHideArticles(int offset, int limit)
        {
            DynamicParameters parameters = new DynamicParameters();
            return this.articleRepository.SelectPage(
                where: "IsDeleted = 0 and IsHideInList = 1",
                order: "toTop desc, time desc",
                param: parameters,
                fileds: gm_fileds,
                isPage: true,
                offset: offset,
                limit: limit
                );
        }

        /// <summary>
        /// 获取文章相关学校Id
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetArticleSchoolIds(Guid articleId, double latitude, double longitude, int pageSize)
        {
            var article = articleRepository.GetArticle(articleId, true);
            if (article?.ArticleType == ArticleType.Policy)
            {
                // 政策文章(以地区和关联的学校类型来作学校和文章的关联)
                return GetPolicyArticleSchools(articleId, latitude, longitude, pageSize).Select(s => s.Id);
            }
            else
            {
                // 对比文章（直接由编辑人员作关联）
                return GetCorrelationSchool(articleId).Select(s => s.SchoolId);
            }
        }

        /// <summary>
        /// 文章相关学校 - 政策文章(以地区和关联的学校类型来作学校和文章的关联)
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private List<SearchSchoolDto> GetPolicyArticleSchools(Guid articleId, double latitude, double longitude, int pageSize)
        {
            var cra = GetCorrelationAreas(articleId).Select(a =>
            {
                if (a.AreaId != null)
                    return int.Parse(a.AreaId);
                else if (a.CityId != null)
                    return int.Parse(a.CityId);
                else if (a.ProvinceId != null)
                    return int.Parse(a.ProvinceId);
                else
                    return 0;
            }).ToList();

            var crst = GetCorrelationSchoolTypes(articleId).Select(a =>
            {
                var schFT = new SchFType0((byte)a.SchoolGrade, (byte)a.SchoolType, a.Discount, a.Diglossia, a.Chinese);
                return schFT.ToString();
            }).ToList();

            //调用搜索接口，搜寻关联学校
            var list = _searchService.SearchSchool(new SearchSchoolQuery
            {
                Latitude = latitude,
                Longitude = longitude,
                CityCode = cra,
                Type = crst,
                Orderby = 5,
                PageSize = pageSize
            });
            return list.Schools;
        }

        public async Task<PaginationModel<ArticleDto>> GetPaginationByTag(Guid? tagId, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var limit = pageSize;
            var offset = (pageIndex - 1) * pageSize;
            //标签查询
            if (tagId == null)
            {
                return PaginationModel<ArticleDto>.Build(default, default);
            }

            var result = await PageByTagID(tagId.Value, offset, limit);
            var articleDtos = ConvertToArticleDto(result.Item1);
            return PaginationModel<ArticleDto>.Build(articleDtos, result.Item2);
        }

        private PaginationModel<ArticleDto> GetPaginationByStrict(int cityCode, Guid? userId, string uuid, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var limit = pageSize;
            var offset = (pageIndex - 1) * pageSize;
            var result = GetStrictSlctArticles(userId, uuid, cityCode, offset, limit);
            var articleDtos = ConvertToArticleDto(result.articles);
            return PaginationModel<ArticleDto>.Build(articleDtos, result.total);
        }

        public PaginationModel<ArticleDto> GetHidePaginationByStrict(int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            var limit = pageSize;
            var offset = (pageIndex - 1) * pageSize;
            var result = GetHideArticles(offset, limit);
            var articleDtos = ConvertToArticleDto(result.articles);
            return PaginationModel<ArticleDto>.Build(articleDtos, result.total);
        }

        public PaginationModel<ArticleDto> GetPaginationByEs(string keyword, int pageIndex, int pageSize)
        {
            var result = _searchService.SearchArticle(keyword, null, pageIndex, pageSize);
            var articleIds = result.Articles.Select(q => q.Id).ToArray();

            List<ArticleDto> articleDtos = new List<ArticleDto>();
            if (articleIds.Length != 0)
            {
                var articles = GetByIds(articleIds).OrderByDescending(a => a.time);
                articleDtos = ConvertToArticleDto(articles);
            }

            // es可能会返回-1
            var total = result.Total > 0 ? result.Total : 0;
            return PaginationModel<ArticleDto>.Build(articleDtos, total);
        }

        public async Task<long> GetPaginationTotalByEs(string keyword, Guid? userId, int? cityId
            , List<int> areaId, List<ArticleType> articleTypes, List<string> schTypes)
        {
            int? provinceId = null;
            if (cityId != null)
            {
                var province = await _cityInfoService.GetProvinceInfoByCity(cityId.Value);
                if (province == null)
                {
                    //无效城市值
                    cityId = null;
                }
                provinceId = province?.ProvinceId;
            }
            var types = articleTypes.Select(s => (int)s).ToList();

            bool? isTop = null;
            var result = await _articleSearchService.SearchTotalAsync(keyword, userId, provinceId, cityId, areaId, types, schTypes, isTop);
            return result;
        }

        public async Task<PaginationModel<ArticleDto>> GetPaginationByEs(string keyword, Guid? userId, int? cityId
            , List<int> areaId, List<ArticleType> articleTypes, List<string> schTypes
            , ArticleOrderBy orderBy, int pageIndex = 1, int pageSize = 10)
        {
            int? provinceId = null;
            if (cityId != null)
            {
                var province = await _cityInfoService.GetProvinceInfoByCity(cityId.Value);
                if (province == null)
                {
                    //无效城市值
                    cityId = null;
                }
                provinceId = province?.ProvinceId;
            }
            var types = articleTypes.Select(s => (int)s).ToList();

            PaginationModel<SearchArticleDto> result = null;
            if (string.IsNullOrEmpty(keyword))
            {
                //1.有搜索词, 不管置顶
                //2.无搜索词, 优先置顶
                bool? isTop = false;
                result = await _articleSearchService.SearchAsync(keyword, userId, provinceId, cityId, areaId, types, schTypes, isTop, orderBy, pageIndex, pageSize);

                //仅在首页添加置顶数据
                if (pageIndex == 1)
                {
                    bool? isTop2 = true;
                    var resultTop = await _articleSearchService.SearchAsync(keyword, userId, provinceId, cityId, areaId, types, schTypes, isTop2, ArticleOrderBy.TopTimeDesc, 1, 1000);
                    result.Total += resultTop.Total;
                    result.Data = resultTop.Data.Concat(result.Data).ToList();
                }
            }
            else
            {
                bool? isTop = null;
                result = await _articleSearchService.SearchAsync(keyword, userId, provinceId, cityId, areaId, types, schTypes, isTop, orderBy, pageIndex, pageSize);
            }

            var articleIds = result.Data.Select(q => q.Id).ToArray();

            List<ArticleDto> articleDtos = new List<ArticleDto>();
            if (articleIds.Length != 0)
            {
                var articles = GetByIds(articleIds);
                //维持排序
                var sortArticles = articleIds.Select(id => articles.FirstOrDefault(a => a.id == id))
                    .Where(s => s != null);
                articleDtos = ConvertToArticleDto(sortArticles);
            }

            // es可能会返回-1
            var total = result.Total > 0 ? result.Total : 0;
            return PaginationModel<ArticleDto>.Build(articleDtos, total);
        }

        /// <summary>
        /// 文章背景图转string 链接
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public string ArticleCoverToLink(article_cover c)
        {
            if (c == null)
            {
                return string.Empty;
            }
            return !string.IsNullOrWhiteSpace(c.ImgUrl) ? c.ImgUrl : $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";
        }

        public List<ArticleDto> ConvertToArticleDto<T>(IEnumerable<T> articles) where T : article
        {
            if (articles == null || !articles.Any())
            {
                return new List<ArticleDto>();
            }

            var ids = articles.Select(s => s.id);
            var covers = _articleCoverService.GetCoversByIds(ids.ToArray());
            return articles.Select(a => new ArticleDto
            {
                Id = a.id,
                No = UrlShortIdUtil.Long2Base32(a.No),
                Layout = a.layout,
                Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                Title = a.title,
                ViewCount = a.VirualViewCount,
                Time = a.time.GetValueOrDefault().ConciseTime("yyyy年MM月dd日"),
                Covers = covers.Where(c => c.articleID == a.id).Select(c => ArticleCoverToLink(c)).ToList()
            }).ToList();
        }

        public async Task<PaginationModel<ArticleDto>> GetPagination(string keyword, int cityCode, Guid? tagId, Guid? userId, string uuid, int pageIndex, int pageSize)
        {
            if (tagId != null)
            {
                return await GetPaginationByTag(tagId, pageIndex, pageSize);
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return GetPaginationByEs(keyword, pageIndex, pageSize);
            }
            return GetPaginationByStrict(cityCode, userId, uuid, pageIndex, pageSize);
        }

        public PaginationModel<ArticleDto> GetHidePagination(int pageIndex, int pageSize)
        {
            return GetHidePaginationByStrict(pageIndex, pageSize);
        }


        public ArticleDetailDto GetArticleDetail(string no, Guid? id, Guid? userId, string sxbtoken = "")
        {
            int no_int32 = (int)UrlShortIdUtil.Base322Long(no);
            var article = GetShowForUserArticle(id ?? Guid.Empty, no_int32, sxbtoken.Equals("sxb007"));
            return GetArticleDetailDto(userId, article);
        }

        private ArticleDetailDto GetArticleDetailDto(Guid? userId, article article)
        {
            if (article == null) { return default; }

            //添加访问记录
            AddAccessCount(article);


            bool isFollow = false;
            if (userId != null && userId != Guid.Empty)
            {
                _historyService.AddHistory(userId ?? Guid.Empty, article.id, (byte)MessageDataType.Article);
                isFollow = _collectionService.IsCollected(userId.Value, article.id);
            }

            //1.查绑定的群组图片
            var groupQRCodes = GetCorrelationGQRCodes(article.id);

            var dto = new ArticleDetailDto()
            {
                Id = article.id,
                Html = article.html,
                Title = article.title,
                ViewCount = article.VirualViewCount,
                Author = article.author,
                Digest = string.IsNullOrWhiteSpace(article.overview) ? article.html.GetHtmlHeaderString(150) : article.overview,
                GroupQRCodes = groupQRCodes.Select(s => s.Url).ToList(),
                Time = article.time.GetValueOrDefault().ToString("yyyy年MM月dd日"),
                IsFollow = isFollow
            };

            SetArticleSiblings(article.No, dto);

            dto.CorrelationTags = GetCorrelationTags(article.id, true).Select(s => s.TagName).ToList();
            if (article.AuthorType == (int)ArticleAuthorType.Talent)
            {
                dto.AuthorUserInfo = GetArticleDetailUserDto(article.id, userId);
            }
            return dto;
        }

        /// <summary>
        /// 获取设置该文章的上一篇和下一篇
        /// </summary>
        /// <param name="no_int32"></param>
        /// <param name="dto"></param>
        private void SetArticleSiblings(int no_int32, ArticleDetailDto dto)
        {
            var siblings = GetNearArticle(no_int32);
            //上一篇
            dto.PrevArticle = siblings
                .Where(s => s.Key > no_int32)
                .Select(s => new ArticleDetailDto.UrlArticle() { No = UrlShortIdUtil.Long2Base32(s.Key), Title = s.Value })
                .FirstOrDefault();

            //下一篇
            dto.NextArticle = siblings
                .Where(s => s.Key < no_int32)
                .Select(s => new ArticleDetailDto.UrlArticle() { No = UrlShortIdUtil.Long2Base32(s.Key), Title = s.Value })
                .FirstOrDefault();
        }

        /// <summary>
        /// 获取文章的达人作者信息, 话题圈信息
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="loginUserId"></param>
        /// <returns></returns>
        private ArticleDetailDto.ArticleDetailUserDto GetArticleDetailUserDto(Guid articleId, Guid? loginUserId)
        {
            var authorTalentId = articleRepository.GetAuthorTalentId(articleId);
            if (authorTalentId == null)
            {
                return null;
            }

            TalentFollowDto talent = _talentService.GetTalent(authorTalentId.Value, loginUserId);
            var articleUserInfo = CommonHelper.MapperProperty<TalentFollowDto, ArticleDetailDto.ArticleDetailUserDto>(talent);
            if (articleUserInfo?.UserId != null)
            {
                var circle = _circleRepository.GetByUserId(articleUserInfo.UserId.Value);
                articleUserInfo.CircleId = circle?.Id;
                articleUserInfo.CircleName = circle?.Name;
            }
            return articleUserInfo;
        }

        public async Task<IEnumerable<Guid>> GetLastestArticleIds()
        {
            int size = 100;
            var key = string.Format(RedisKeys.NewArticlesKey, MessageDataType.Article.ToString().ToLower());

            //优先取缓存
            var data = await _easyRedisClient.GetAsync<IEnumerable<Guid>>(key);
            if (data == null || !data.Any())
            {
                //缓存无, 取库
                data = await articleRepository.GetLastestArticleIds(size);
                //存入缓存
                await _easyRedisClient.AddAsync(key, data, TimeSpan.FromMinutes(70));
            }
            return data ?? Enumerable.Empty<Guid>();
        }

        public async Task<IEnumerable<Guid>> GetLastestArticleIds(int takeSize)
        {
            var data = await GetLastestArticleIds();
            return data.Take(takeSize);
        }

        public async Task<IEnumerable<Guid>> GetRandomLastestArticleIds(int takeSize, IEnumerable<Guid> excludeIds)
        {
            var data = await GetLastestArticleIds();
            var result = CommonHelper.ListRandom(data, excludeIds);
            return result.Take(takeSize);
        }


        public async Task<IEnumerable<ArticleDto>> GetNewest(int count = 9)
        {
            var finds = await articleRepository.GetNewest(count);
            if (finds?.Any() == true)
            {
                return finds.Select(p => new ArticleDto()
                {
                    No = UrlShortIdUtil.Long2Base32(p.No),
                    Title = p.title,
                    Time = p.time.HasValue ? p.time.Value.ArticleListItemTimeFormart("yyyy-MM-dd") : null,
                    Id = p.id
                });
            }
            return null;
        }

        public async Task<Guid?> GetTalentUserId(Guid articleId)
        {
            return await articleRepository.GetTalentUserId(articleId);
        }
    }
}