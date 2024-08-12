using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Models.User;
using Sxb.PCWeb.RequestModel.School;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using ProductManagement.Framework.Foundation;
using PMS.OperationPlateform.Domain.Enums;
using PMS.Search.Domain.Entities;
using ProductManagement.Framework.Cache.Redis;
using Sxb.PCWeb.Common;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.Text;
using Sxb.PCWeb.ViewModels.Search;
using Sxb.PCWeb.ViewModels.School;
using Sxb.PCWeb.ViewModels.Article;
using System.ComponentModel;

namespace Sxb.PCWeb.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly ImageSetting _imageSetting;
        private readonly ISearchService _dataSearch;
        private readonly IImportService _dataImport;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionService;
        private readonly ICityInfoService _cityService;
        private readonly ISchoolService _schoolService;
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IMapper _mapper;

        private readonly IArticleCoverService _articleCoverService;
        private readonly IArticleCommentService _articleCommentService;

        public SearchController(ISearchService dataSearch, IImportService dataImport, IMapper mapper,
            ISchoolCommentService commentService, IQuestionInfoService questionService,
            ISchoolInfoService schoolInfoService, IOptions<ImageSetting> set, IEasyRedisClient easyRedisClient,
            ISchoolService schoolService, ICityInfoService cityService, IUserService userService,
            IArticleService articleService, ISchoolRankService schoolRankService,
            IArticleCoverService articleCoverService, IArticleCommentService articleCommentService)
        {
            _dataSearch = dataSearch;
            _dataImport = dataImport;
            _schoolService = schoolService;
            _cityService = cityService;
            _commentService = commentService;
            _questionService = questionService;
            _mapper = mapper;
            _schoolInfoService = schoolInfoService;
            _imageSetting = set.Value;
            _userService = userService;
            _articleService = articleService;
            _schoolRankService = schoolRankService;
            _easyRedisClient = easyRedisClient;
            _articleCoverService = articleCoverService;
            _articleCommentService = articleCommentService;
        }

        public ResponseResult CreateHistoryIndex()
        {

            _dataImport.CreateHistoryIndex();
            return ResponseResult.Success();
        }
        public ResponseResult SetHistory()
        {
            _dataImport.SetHistory("PC", 3, "学校3", 440100, Guid.NewGuid(), null);
            return ResponseResult.Success();
        }

        /// <summary>
        /// 联想词搜索
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="channel">0学校 1点评 2问答 3排行榜 4.攻略 5.所有</param>
        /// <returns></returns>
        [Description("联想词搜索")]
        public ResponseResult GetSuggestList(string keyWords, int channel)
        {
            int cityCode = Request.GetLocalCity();
            //登录用户获取搜索历史记录
            if (User.Identity.IsAuthenticated)
            {
                var loginUser = User.Identity.GetUserInfo();
            }
            if(string.IsNullOrWhiteSpace(keyWords))
            {
                ResponseResult.Success(new
                {
                    keyWords,
                    total = 0,
                    list = new List<SuggestGroupViewModel>()
                });
            }
             var list = _dataSearch.ListSuggest(keyWords, channel, cityCode);


            var Group = new List<int> { 1, 2, 3, 4, 5 };
            var result = Group.Select(s => new SuggestGroupViewModel { GroupType = s }).ToList();


            for (int i = 0; i < result.Count(); i++)
            {
                var suggests = list.Suggests.FirstOrDefault(q => q.Type == result[i].GroupType)?.Items;

                if (suggests == null)
                {
                    result[i].Items = new List<SuggestViewModel>();
                }
                else
                {
                    var ids = suggests.Select(q => q.SourceId).ToList();
                    if (i == 1)
                    {
                        var comment = _commentService.QueryCommentByIds(ids, Guid.Empty);
                        result[i].Items =  suggests.Select(q => new SuggestViewModel
                        {
                            Channel = i,
                            Type = 1,
                            KeyWord = q.Name,
                            Value = new
                            {
                                Id = q.SourceId,
                                shortId = UrlShortIdUtil.Long2Base32(comment.FirstOrDefault(p => p.Id == q.SourceId)?.No ?? 0),
                                url = Url.Action("Detail","Comment",new {no= UrlShortIdUtil.Long2Base32(comment.FirstOrDefault(p => p.Id == q.SourceId)?.No ?? 0) })
                            }
                        }).ToList();
                    }
                    else if (i == 2)
                    {
                        var question = _questionService.GetQuestionByIds(ids, Guid.Empty);
                        result[i].Items = suggests.Select(q => new SuggestViewModel
                        {
                            Channel = i,
                            Type = 1,
                            KeyWord = q.Name,
                            Value = new
                            {
                                Id = q.SourceId,
                                shortId = UrlShortIdUtil.Long2Base32(question.FirstOrDefault(p => p.Id == q.SourceId)?.No ?? 0),
                                url = Url.Action("Detail", "Question", new { no = UrlShortIdUtil.Long2Base32(question.FirstOrDefault(p => p.Id == q.SourceId)?.No ?? 0) })
                            }
                        }).ToList();
                    }
                    else if(i == 0)
                    {
                        result[i].Items =  suggests.Select(q => new SuggestViewModel
                        {
                            Channel = i,
                            Type = 1,
                            KeyWord = q.Name,
                            Value = new {
                                Id = q.SourceId,
                                url = Url.Action("Detail", "School", new { Id = q.SourceId.ToString("N") })
                            }
                        }).ToList();
                    }
                    else if (i == 3)
                    {
                        result[i].Items = suggests.Select(q => new SuggestViewModel
                        {
                            Channel = i,
                            Type = 1,
                            KeyWord = q.Name,
                            Value = new { Id = q.SourceId , url = Url.Action("Detail", "Schoolrank", new { Id = q.SourceId.ToString("N") }) }
                        }).ToList();
                    }
                    else if (i == 4)
                    {
                        var article = _articleService.GetByIds(ids.ToArray());
                        result[i].Items = suggests.Select(q => new SuggestViewModel
                        {
                            Channel = i,
                            Type = 1,
                            KeyWord = q.Name,
                            Value = new { Id = q.SourceId,
                                shortId = UrlShortIdUtil.Long2Base32(article.FirstOrDefault(p => p.id == q.SourceId)?.No ?? 0),
                                url = Url.Action("Detail", "Article", new { no = UrlShortIdUtil.Long2Base32(article.FirstOrDefault(p => p.id == q.SourceId)?.No ?? 0) })
                                }
                        }).ToList();
                    }
                }
            }

            return ResponseResult.Success(new
            {
                keyWords,
                total = channel == 0 ? list.Total : list.Suggests.FirstOrDefault(q => q.Type == (channel + 1))?.Total ?? 0,
                list = result
            });
        }


        /// <summary>
        /// 搜索文章列表
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("搜索文章列表")]
        public ResponseResult SearchArticlePage(string keyWords, int PageNo = 1, int PageSize = 10)
        {
            int cityCode = Request.GetLocalCity();
            var UUID = Request.GetDevice();
            string UA = Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrWhiteSpace(keyWords) && !UA.ToLower().Contains("spider") && !UA.ToLower().Contains("bot") && Request.GetAvaliableCity() != 0)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    _dataImport.SetHistory("PC", 4, keyWords, Request.GetAvaliableCity(), user.UserId, UUID);
                }
                else
                {
                    _dataImport.SetHistory("PC", 4, keyWords, Request.GetAvaliableCity(), null, UUID);
                }
            }
            var result = _dataSearch.SearchArticle(keyWords,null, PageNo, PageSize);

            var articleIds = result.Articles.Select(q => q.Id).ToArray();
            var articleList = _articleService.GetByIds(articleIds).ToList();

            if (articleList != null && articleList.Count > 0)
            {
                //查询出目标背景图片
                var effactiveIds = articleList.Select(a => a.id).ToArray();
                var covers = _articleCoverService.GetCoversByIds(effactiveIds);
                var comments = _articleCommentService.Statistics_CommentsCount(articleIds);

                var _articles = new List<PMS.OperationPlateform.Domain.Entitys.article>();
                if (articleIds != null)
                {
                    foreach (var id in articleIds)
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
                var ogaData = articleList.Select(a => new ArticleViewModel()
                {
                    Id = a.id,
                    Time = a.time.GetValueOrDefault().ConciseTime(),
                    Title = a.title,
                    ViweCount = a.VirualViewCount,
                    CommentCount = a.CommentCount,
                    Digest = a.overview
                }).ToList();
                return ResponseResult.Success(result);
            }
            else
            {
                return ResponseResult.Success(new List<ArticleViewModel>());
            }
        }


        /// <summary>
        /// 历史搜索列表
        /// </summary>
        /// <returns></returns>
        [Description("历史搜索列表")]
        public ResponseResult GetHistoryList(int channel = -1, int pageSize = 10)
        {
            var result = new List<SearchWordViewModel>();
            var cityCode = Request.GetCity();
            var UUID = Request.GetDevice();

            var list = _dataSearch.GetHistoryList(null, UUID, channel, pageSize);

            if (!list.Any() && User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                list = _dataSearch.GetHistoryList(user.UserId, null, channel, pageSize);
            }
            result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 清除历史搜索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("清除历史搜索")]
        public ResponseResult RemoveHistoryList(int channel = -1)
        {
            if (User.Identity.IsAuthenticated)
            {

            }
            else
            {

            }
            return ResponseResult.Success();
        }


        /// <summary>
        /// 热门搜索词列表
        /// </summary>
        /// <returns></returns>
        [Description("热门搜索词列表")]
        public ResponseResult GetHotwordList(int channel = -1, int pageSize = 10)
        {
            int cityCode = Request.GetLocalCity();
            List<string> list = _dataSearch.GetHotHistoryList(channel, pageSize, null, cityCode);
            var result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();

            return ResponseResult.Success(result);
        }
    }
}
