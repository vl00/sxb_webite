using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using iSchool.Library.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.Search.Application.IServices;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Models;
using Sxb.Web.Areas.TopicCircle.Filters;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Common;
using Sxb.Web.Response;
using Sxb.Web.ViewModels.Search;
using static PMS.Search.Domain.Common.GlobalEnum;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.TopicCircle.Controllers
{

    [ApiController]
    [Route("tpc/[controller]/[action]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IHtmlServiceClient _htmlServiceClient;

        ICircleFollowerService _circleFollowerService;
        ISearchService _searchService;

        private readonly ISchoolService _schoolService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly IArticleService _articleService;
        private readonly ILiveServiceClient _liveServiceClient;
        private readonly ISchoolCommentService _schoolCommentService;
        private readonly IQuestionInfoService _questionInfoService;

        public SearchController(ICircleFollowerService circleFollowerService, ISearchService searchService, ISchoolInfoService schoolInfoService, IArticleService articleService, ISchoolService schoolService, ILiveServiceClient liveServiceClient, ISchoolCommentService schoolCommentService, IQuestionInfoService questionInfoService, IHtmlServiceClient htmlServiceClient, ILogger<SearchController> logger)
        {
            this._circleFollowerService = circleFollowerService;
            _searchService = searchService;
            _schoolInfoService = schoolInfoService;
            _articleService = articleService;
            _schoolService = schoolService;
            _liveServiceClient = liveServiceClient;
            _schoolCommentService = schoolCommentService;
            _questionInfoService = questionInfoService;
            _htmlServiceClient = htmlServiceClient;
            _logger = logger;
        }

        [HttpPost]
        [ValidateCircleMaster]
        public ResponsePageResult<IEnumerable<CircleFollowerDetailDto>> SearchFollower([FromQuery] Guid circleId, [FromBody] SearchFollowerRequest request)
        {
            return this._circleFollowerService.Search(new AppServicePageRequestDto<CricleFollowerSearchRequestDto>()
            {
                limit = request.Limit,
                offset = request.Offset,
                input = new CricleFollowerSearchRequestDto()
                {
                    CircleId = circleId,
                    NickName = request.SearchContent
                }
            });
        }

        /// <summary>
        /// 联想词搜索
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="channel">1 学校 16 文章  32 直播</param>
        /// <param name="isRelated">是否查询达人相关的数据</param>
        /// <returns></returns>
        //public async Task<ResponseResult> GetSuggestList(string keyWords, ChannelIndex? channel, bool isRelated = false, int pageIndex = 1, int pageSize = 10)
        public async Task<ResponseResult> GetSuggestList([FromQuery] SearchSuggestFormModel model)
        {
            var keyWords = model.KeyWords;
            var channel = model.Channel;
            var isRelated = model.IsRelated;
            var pageIndex = model.PageIndex;
            var pageSize = model.PageSize;

            Guid? userId = null;
            //登录用户获取搜索历史记录
            if (User.Identity.IsAuthenticated)
            {
                var loginUser = User.Identity.GetUserInfo();
                userId = loginUser.UserId;
            }

            var topicChannelAll = ChannelIndex.School | ChannelIndex.Article | ChannelIndex.Live;
            channel = channel ?? topicChannelAll;

            //查询相关联的信息
            var searchUserId = isRelated ? userId : null;
            var pagination = _searchService.SearchIds(keyWords, channel.Value, searchUserId, pageIndex, pageSize);
            var data = pagination.List;
            var group = data.GroupBy(s => s.Channel);

            List<TopicSuggestDto> result = await GetSuggestDto(group);

            var total = pagination.Total;
            var paginationDto = PaginationModel<TopicSuggestDto>.Build(result, total);
            return ResponseResult.Success(paginationDto);
        }

        [NonAction]
        private async Task<List<TopicSuggestDto>> GetSuggestDto(IEnumerable<IGrouping<ChannelIndex, PMS.Search.Application.ModelDto.SearchIdDto>> group)
        {
            var cookies = Request.Cookies.ToDictionary(s => s.Key, v => v.Value);
            var result = new List<TopicSuggestDto>();
            foreach (var item in group)
            {
                var ids = item.Select(s => s.Id).ToList();
                var itemResult = new List<TopicSuggestDto>();
                switch (item.Key)
                {
                    case ChannelIndex.School:
                        var schools = _schoolInfoService.GetSchoolName(ids);
                        itemResult = schools.Select(s => new TopicSuggestDto()
                        {
                            Channel = item.Key,
                            Id = s.SchoolSectionId,
                            Name = s.SchoolName,
                            Url = $"/school-{UrlShortIdUtil.Long2Base32(s.SchoolNo)}"
                        }).ToList();
                        break;
                    case ChannelIndex.Article:
                        var articles = _articleService.GetByIds(ids.ToArray());
                        itemResult = articles.Select(s => new TopicSuggestDto()
                        {
                            Channel = item.Key,
                            Id = s.id,
                            Name = s.title,
                            Url = $"/article/{UrlShortIdUtil.Long2Base32(s.No)}.html"
                        }).ToList();
                        break;
                    //话题圈 粉丝数
                    case ChannelIndex.Live:
                        try
                        {
                            var lives = await _liveServiceClient.QueryLectures(ids, cookies);
                            itemResult = lives?.Items.Select(s => new TopicSuggestDto()
                            {
                                Channel = item.Key,
                                Id = s.Id,
                                Name = s.Subject,
                                Url = $"/live/client/livedetail.html?showtype=1&contentid={s.Id}"
                            }).ToList();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "搜索关联获取直播出错");
                        }
                        break;
                }

                //维持原有顺序
                itemResult = ids.Select(id => itemResult.FirstOrDefault(s => s.Id == id)).Where(s => s != null).ToList();
                result.AddRange(itemResult);
            }

            return result;
        }

        /// <summary>
        /// 分析链接
        /// </summary>
        /// <param name="url"></param>
        public async Task<ResponseResult> AnalysisUrl(string url)
        {
            url = HttpUtility.UrlDecode(url);
            var cookies = Request.Cookies.ToDictionary(s => s.Key, v => v.Value);
            var urlEndPoints = UrlEndPoints.Build(url);
            if (urlEndPoints != null)
            {
                var success = true;
                {
                    var no = urlEndPoints.No;
                    var id = urlEndPoints.Id;
                    //识别到Id, 根据ID查询
                    if (id != null && id.Value != Guid.Empty)
                    {
                        #region 根据ID查询
                        switch (urlEndPoints.ControllerName)
                        {
                            case "school":
                                var school = _schoolService.GetSchoolDetailById(id.Value);
                                urlEndPoints.Content = school?.SchoolName;
                                urlEndPoints.Type = TopicType.School;
                                break;
                            case "comment":
                                var comment = _schoolCommentService.GetList(s => s.Id == id.Value).FirstOrDefault();
                                urlEndPoints.Content = comment?.Content;
                                urlEndPoints.Type = TopicType.Comment;
                                break;
                            case "question":
                                var question = _questionInfoService.GetQuestionById(id.Value, Guid.Empty);
                                urlEndPoints.Content = question?.QuestionContent;
                                urlEndPoints.Type = TopicType.Question;
                                break;
                            case "article":
                                var article = _articleService.GetByIds(new[] { id.Value }).FirstOrDefault();
                                urlEndPoints.Content = article?.title;
                                urlEndPoints.Type = TopicType.Article;
                                break;
                            case "live":
                                var lives = await _liveServiceClient.QueryLectures(new List<Guid> { id.Value }, cookies);
                                var live = lives?.Items.FirstOrDefault();
                                urlEndPoints.Content = live?.Subject;
                                urlEndPoints.Type = TopicType.Live;
                                break;
                            default:
                                success = false;
                                break;
                        }
                        #endregion
                    }
                    //未识别到Id, 根据No查询
                    else if (urlEndPoints.No > 0)
                    {
                        #region 根据No查询
                        switch (urlEndPoints.ControllerName)
                        {
                            case "school":
                                var school = _schoolService.GetSchoolByNo(no);
                                urlEndPoints.Id = school?.Id;
                                urlEndPoints.Content = school?.SchoolName;
                                urlEndPoints.Type = TopicType.School;
                                break;
                            case "comment":
                                var comment = _schoolCommentService.QueryCommentByNo(no);
                                urlEndPoints.Id = comment?.Id;
                                urlEndPoints.Content = comment?.Content;
                                urlEndPoints.Type = TopicType.Comment;
                                break;
                            case "question":
                                var question = await _questionInfoService.QuestionDetailByNo(no, Guid.Empty);
                                urlEndPoints.Id = question?.Id;
                                urlEndPoints.Content = question?.QuestionContent;
                                urlEndPoints.Type = TopicType.Question;
                                break;
                            case "article":
                                var article = _articleService.GetArticleByNo(no);
                                urlEndPoints.Id = article?.id;
                                urlEndPoints.Content = article?.title;
                                urlEndPoints.Type = TopicType.Article;
                                break;
                            case "live":
                                //直播是Id页面
                                var lives = await _liveServiceClient.QueryLectures(new List<Guid> { id.Value }, cookies);
                                var live = lives?.Items.FirstOrDefault();
                                urlEndPoints.Content = live?.Subject;
                                urlEndPoints.Type = TopicType.Live;
                                break;
                            default:
                                success = false;
                                break;
                        }
                        #endregion
                    }
                }
                //外链, 或者上边没有获取到
                if (!urlEndPoints.IsSxkidUrl || string.IsNullOrWhiteSpace(urlEndPoints.Content))
                {
                    urlEndPoints.Type = TopicType.OuterUrl;
                    urlEndPoints.Content = await _htmlServiceClient.GetTitle(urlEndPoints.Url);
                    if (string.IsNullOrWhiteSpace(urlEndPoints.Content))
                    {
                        urlEndPoints.Content = "外链";
                    }
                }

                success = !string.IsNullOrWhiteSpace(urlEndPoints.Content);
                if (success)
                    return ResponseResult.Success(urlEndPoints);
            }
            return ResponseResult.Failed("此链接不支持生成对应卡片", urlEndPoints);
        }
    }
}
