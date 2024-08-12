using System;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using Sxb.Web.ViewModels.School;

using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.API.Http.Interface;
using PMS.School.Domain.Dtos;
using Sxb.Web.ViewModels.TopicCircle;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.Search.Domain.Common;
using Sxb.Web.Utils;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.Areas.Common.Controllers
{
    /// <summary>
    /// 猜你喜欢
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GuessController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly ISchoolService _schoolService;
        private readonly ICircleService _circleService;
        private readonly ITalentService _talentService;

        private readonly IUserRecommendClient _userRecommendClient;

        private readonly ChannelUrlHelper _channelUrlHelper;

        public GuessController(IMapper mapper, IEasyRedisClient easyRedisClient, ISchoolService schoolService, ICircleService circleService, IUserRecommendClient userRecommendClient, ITalentService talentService)
        {
            _channelUrlHelper = new ChannelUrlHelper(ConfigHelper.GetHost(), ConfigHelper.GetUserHost());

            _mapper = mapper;
            _easyRedisClient = easyRedisClient;
            _schoolService = schoolService;
            _circleService = circleService;
            _userRecommendClient = userRecommendClient;
            _talentService = talentService;
        }

        /// <summary>
        /// 话题圈猜你喜欢
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GuessCircles()
        {
            Guid? userId = null;
            if (User.Identity.IsAuthenticated)
                userId = User.Identity.GetUserInfo()?.UserId;

            int cityCode = Request.GetLocalCity();
            //根据城市推荐话题圈
            var result = this._circleService.GetRecommends(cityCode);
            var circles = result.Data;

            var viewModels = new List<CircleItemViewModel>();
            if (circles.Any())
            {
                var circleIds = circles.Select(s => s.Id);
                var circleDtos = _circleService.GetCircles(circleIds, userId);

                //使用原有序列
                var vm = _mapper.Map<List<CircleItemViewModel>>(circleDtos);
                viewModels = circleIds.Select(id => vm.Where(s => s.Id == id).FirstOrDefault()).ToList();
            }

            return ResponseResult.Success(viewModels);
        }

        /// <summary>
        /// 学校猜你喜欢
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GuessSchools(int index)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());//"df279433-217f-4098-a346-57b96789f4ea";
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }

            string key = $"UserRecommendExtApi:{id}:{index}";

            var list = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                try
                {
                    var response = await _userRecommendClient.UserRecommendSchool(Guid.Parse(id), index, 10);
                    return new RecommenderResponseDto
                    {
                        ErrorDescription = response.ErrorDescription,
                        Status = response.Status,
                        PageInfo = new PageInfo
                        {
                            CountPerpage = response.PageInfo?.CountPerpage ?? 0,
                            Curpage = response.PageInfo?.Curpage ?? 0,
                            TotalCount = response.PageInfo?.TotalCount ?? 0,
                            TotalPage = response.PageInfo?.TotalPage ?? 0,
                        },
                        Items = response.Items.Select(q => new RecommenderDto
                        {
                            BuType = q.BuType,
                            ContentId = q.ContentId
                        }).ToList()
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }, TimeSpan.FromMinutes(5));

             var data = new List<SchoolExtFilterDto>();
            if (list == null)
            {
                return ResponseResult.Success(data);
            }

            var ids = list.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid guid))
                    .Select(q => Guid.Parse(q.ContentId));

            data = await _schoolService.SearchSchools(ids);

            _channelUrlHelper.CompleteUrl(ref data, ChannelIndex.School, "SchoolNo");
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 点评猜你喜欢
        /// </summary>
        /// <returns></returns>
        public ResponseResult GuessComments(int index)
        {

            return ResponseResult.Success();
        }

        /// <summary>
        /// 问答猜你喜欢
        /// </summary>
        /// <returns></returns>
        public ResponseResult GuessQuestions(int index)
        {

            return ResponseResult.Success();
        }

        /// <summary>
        /// 文章猜你喜欢
        /// </summary>
        /// <returns></returns>
        public ResponseResult GuessArticles(int index)
        {

            return ResponseResult.Success();
        }

        /// <summary>
        /// 达人猜你喜欢
        /// </summary>
        /// <returns></returns>
        public ResponseResult GuessTalents()
        {
            return ResponseResult.Success();
        }

    }
}
