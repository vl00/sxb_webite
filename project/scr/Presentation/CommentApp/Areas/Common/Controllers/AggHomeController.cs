using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nest;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Crypto;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Common;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Enums;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.Search.Application.IServices;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.Services;
using ProductManagement.API.Aliyun;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Controllers;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using static PMS.Search.Domain.Common.GlobalEnum;
using static PMS.UserManage.Domain.Common.EnumSet;
using static ProductManagement.API.Aliyun.Model.GarbageCheckResponse;

namespace Sxb.Web.Areas.Common.Models
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggHomeController : ApiBaseController
    {
        private readonly IText _text;

        private readonly ITalentSettingService _talentSettingService;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionInfoService;
        private readonly ISearchService _searchService;
        private readonly IImportService _importService;
        private readonly ISchoolService _schoolService;
        private readonly IArticleService _articleService;
        private readonly IWikiService _wikiService;
        private readonly IHistoryService _historyService;
        private readonly IEasyRedisClient _easyRedisClient;
        IHotTypeService _hotTypeService;

        private readonly School.SchoolController _schoolController;

        public AggHomeController(IText text, ITalentSettingService talentSettingService, ISchoolCommentService commentService, IQuestionInfoService questionInfoService, ISearchService searchService, ISchoolService schoolService, IArticleService articleService, IWikiService wikiService, IHistoryService historyService, IEasyRedisClient easyRedisClient, IImportService importService, School.SchoolController schoolController, IHotTypeService hotTypeService)
        {
            _text = text;
            _talentSettingService = talentSettingService;
            _commentService = commentService;
            _questionInfoService = questionInfoService;
            _searchService = searchService;
            _schoolService = schoolService;
            _articleService = articleService;
            _wikiService = wikiService;
            _historyService = historyService;
            _easyRedisClient = easyRedisClient;
            _importService = importService;
            _schoolController = schoolController;
            _hotTypeService = hotTypeService;
        }

        /// <summary>
        /// 设置历史记录
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="channel"></param>
        /// <param name="userId"></param>
        /// <param name="clientType"></param>
        [NonAction]
        public void SetHistory(string keyword, ChannelIndex channel, Guid? userId, ClientType clientType)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            if (clientType != ClientType.Mobile && clientType != ClientType.PC)
            {
                throw new ArgumentException("不支持的客户端类型");
            }
            var esKey = clientType == ClientType.PC ? "PC" : "H5";

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
            _importService.SetHistory(esKey, historyChannel, keyword, cityCode, userId, uuid);
        }

        private async Task<IEnumerable<Guid>> GetRandomRecommends(ChannelIndex? channelIndex, int size, IEnumerable<Guid> excludeIds)
        {
            switch (channelIndex)
            {
                case ChannelIndex.School:
                    return await _historyService.GetRandomTop(MessageDataType.School, size, excludeIds);
                case ChannelIndex.Comment:
                    return await _historyService.GetRandomTop(MessageDataType.Comment, size, excludeIds);
                case ChannelIndex.Question:
                    return await _historyService.GetRandomTop(MessageDataType.Question, size, excludeIds);
                case ChannelIndex.Article:
                    return await _articleService.GetRandomLastestArticleIds(size, excludeIds);
            }
            return Enumerable.Empty<Guid>();
        }

        private async Task<IEnumerable<Guid>> GetRecommends(ChannelIndex? channelIndex, int size)
        {
            switch (channelIndex)
            {
                case ChannelIndex.School:
                    return await _historyService.GetTop(MessageDataType.School, size);
                case ChannelIndex.Comment:
                    return await _historyService.GetTop(MessageDataType.Comment, size);
                case ChannelIndex.Question:
                    return await _historyService.GetTop(MessageDataType.Question, size);
                case ChannelIndex.Article:
                    return await _articleService.GetLastestArticleIds(size);
            }
            return Enumerable.Empty<Guid>();
        }

        /// <summary>
        /// 默认首页数据
        /// </summary>
        /// <param name="channels"></param>
        /// <returns></returns>
        private async Task<AggSearchVM> AggSearchDefault()
        {
            var channels = new ChannelIndex[] { ChannelIndex.Article, ChannelIndex.Question, ChannelIndex.Comment, ChannelIndex.School };
            var key = string.Format(RedisKeys.AggHomeSearchKey);
            var data = await _easyRedisClient.GetAsync<AggSearchVM>(key);
            if (data != null)
            {
                return data;
            }
            data = new AggSearchVM();

            var max = 3;
            foreach (var index in channels)
            {
                var allIds = await GetRecommends(index, max);
                switch (index)
                {
                    case ChannelIndex.School:
                        var schools = await _schoolService.SearchSchools(allIds, 0, 0, readIntro: true);
                        //var gradeUserIds = await _talentSettingService.GradeUserIds();
                        data.SearchSchools = schools.Select(s => SearchSchoolVM.Convert(s)).ToList();
                        break;
                    case ChannelIndex.Comment:
                        var comments = await _commentService.SearchUserComments(allIds, UserIdOrDefault);
                        data.SearchComments = comments.Select(s => SearchCommentVM.Convert(s)).ToList();
                        break;
                    case ChannelIndex.Question:
                        var questions = await _questionInfoService.SearchUserQuestions(allIds, UserIdOrDefault);
                        data.SearchQuestions = questions.Select(s => SearchQuestionVM.Convert(s)).ToList();
                        break;
                    case ChannelIndex.Article:
                        var articles = _articleService.SearchArticles(allIds);
                        data.SearchArticles = articles.Select(s => SearchArticleVM.Convert(s)).ToList();
                        break;
                }
            }

            await _easyRedisClient.AddAsync(key, data, TimeSpan.FromMilliseconds(60));
            return data;
        }

        private async Task<AggSearchVM> AggSearch(string keyword)
        {
            var channels = new ChannelIndex[] { ChannelIndex.Article, ChannelIndex.Question, ChannelIndex.Comment, ChannelIndex.School };
            var channel = ChannelIndex.SchoolCommentQuestionArticle;

            var data = new AggSearchVM();
            var max = 3;
            //2.查询三条数据
            var result = _searchService.ListSuggest(keyword, channel, Request.GetAvaliableCity(), max);
            data.TotalCount = result.Total < 0 ? 0 : result.Total;

            //3.不足三条, 推荐3-n条
            foreach (var index in channels)
            {
                var ids = result.Suggests.FirstOrDefault(s => s.Index == index)?.Items.Select(s => s.SourceId);
                ids = ids ?? Enumerable.Empty<Guid>();

                var suplus = max - ids.Count();
                var recommendIds = suplus > 0 ? await GetRandomRecommends(index, suplus, ids) : Enumerable.Empty<Guid>();
                var allIds = ids.Concat(recommendIds);
                switch (index)
                {
                    case ChannelIndex.School:
                        var schools = await _schoolService.SearchSchools(allIds, 0, 0, readIntro: true);
                        //var gradeUserIds = await _talentSettingService.GradeUserIds();

                        var schoolVms = schools.Select(s => SearchSchoolVM.Convert(s));
                        //搜索结果
                        data.SearchSchools = schoolVms.Where(s => ids.Contains(s.ExtId)).ToList();
                        //推荐结果
                        data.RecommendSchools = schoolVms.Where(s => recommendIds.Contains(s.ExtId)).ToList();
                        break;
                    case ChannelIndex.Comment:
                        var comments = await _commentService.SearchUserComments(allIds, UserIdOrDefault);

                        var commentVMs = comments.Select(s => SearchCommentVM.Convert(s));
                        data.SearchComments = commentVMs.Where(s => ids.Contains(s.Id)).ToList();
                        data.RecommendComments = commentVMs.Where(s => recommendIds.Contains(s.Id)).ToList();
                        break;
                    case ChannelIndex.Question:
                        var questions = await _questionInfoService.SearchUserQuestions(allIds, UserIdOrDefault);

                        var questionVMs = questions.Select(s => SearchQuestionVM.Convert(s));
                        data.SearchQuestions = questionVMs.Where(s => ids.Contains(s.Id)).ToList();
                        data.RecommendQuestions = questionVMs.Where(s => recommendIds.Contains(s.Id)).ToList();
                        break;
                    case ChannelIndex.Article:
                        var articles = _articleService.SearchArticles(allIds);

                        var articleVms = articles.Select(s => SearchArticleVM.Convert(s));
                        data.SearchArticles = articleVms.Where(s => ids.Contains(s.Id)).ToList();
                        data.RecommendArticles = articleVms.Where(s => recommendIds.Contains(s.Id)).ToList();
                        break;
                }
            }

            //百科数据
            var wikiContent = _wikiService.GetWikiContentByName(keyword);
            data.wiki = new AggSearchVM.WikiDto()
            {
                Name = keyword,
                Content = wikiContent
            };
            return data;
        }


        [HttpGet("AggSearch/{clientType}")]
        public async Task<ResponseResult> AggSearch2(string keyword, ClientType clientType)
        {
            return await AggSearch(keyword, clientType);
        }

        /// <summary>
        /// 聚合查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="clientType">1 H5 2 PC</param>
        /// <returns></returns>
        [HttpGet("AggSearch/{clientType}/{keyword?}")]
        public async Task<ResponseResult> AggSearch(string keyword, ClientType clientType)
        {
            var channel = ChannelIndex.SchoolCommentQuestionArticle;
            SetHistory(keyword, channel, UserId, clientType);

            AggSearchVM data = null;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                //默认首页
                data = await AggSearchDefault();
            }
            else
            {
                //1.文本垃圾检测
                var block = await _text.GarbageCheck(keyword);
                if (block)
                {
                    //返回默认首页
                    data = await AggSearchDefault();
                    data.IsGarbage = true;
                }
                else
                {
                    data = await AggSearch(keyword);
                }
            }

            //获取聚合页广告学校
            data.SearchSchools = data.SearchSchools ?? new List<SearchSchoolVM>();
            data.RecommendSchools = data.RecommendSchools ?? new List<SearchSchoolVM>();
            var extIds = data.SearchSchools
                .Select(s => s.ExtId)
                .Concat(data.RecommendSchools.Select(s => s.ExtId));
            extIds = _wikiService.GetAggSchools(extIds);
            data.BigBannerExtId = extIds.Any() ? extIds?.First() : null;


            foreach (var item in data.SearchSchools)
            {
                if (item.CityName.Contains("广州"))
                {
                    item.TalentUser = (await _hotTypeService.GetRecommendTalentBySchFTypeCode(item.SchFTypeCode, UserIdOrDefault))?.TalentUserID;
                    //item.TalentUser = (await _schoolController.GetRecommendTalentUserID(item.SchFType0Desc, UserIdOrDefault))?.TalentUserID;
                }
            }
            foreach (var item in data.RecommendSchools)
            {
                if (item.CityName.Contains("广州"))
                {
                    item.TalentUser = (await _hotTypeService.GetRecommendTalentBySchFTypeCode(item.SchFTypeCode, UserIdOrDefault))?.TalentUserID;
                    //item.TalentUser = (await _schoolController.GetRecommendTalentUserID(item.SchFType0Desc, UserIdOrDefault))?.TalentUserID;
                }
            }

            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 是否是聚合活动学校数据
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [HttpGet("IsAggSchool/{extId}")]
        public ResponseResult IsAggSchool(Guid extId)
        {
            var extIds = _wikiService.GetAggSchools(new List<Guid>() { extId });
            return ResponseResult.Success(new { 
                IsAggSchool = extIds.Any()
            });
        }
    }
}

