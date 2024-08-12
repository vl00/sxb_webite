using Microsoft.AspNetCore.Mvc;
using NLog;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web
{
    public class UserInformationApiController : BaseController
    {
        private readonly ISchoolService _schoolService;
        private readonly ICollectionService _collectionService;
        private readonly IUserService _userService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly string _ExtDataKey = "ext:chooseData:";


        public UserInformationApiController(
            ISchoolService schoolService,
            ICollectionService collectionService,
            IEasyRedisClient easyRedisClient,
            IUserService userService
            ) {
            _schoolService = schoolService;
            _collectionService = collectionService;
            _userService = userService;
            _easyRedisClient = easyRedisClient;
        }

        /// <summary>
        /// 默认视图
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 浏览历史
        /// </summary>
        /// <returns></returns>
        public async Task<ResultDto> GetHistorySchool(int page = 1, int pageSize = 10)
        {
            var historySchool = new List<SchoolExtSimpleDto>();
            var userId = GetUserId();
            string Key = User.Identity.IsAuthenticated==true ? $"SchoolPK-1:{userId}" : $"SchoolPK-0:{userId}";
            var start = (page - 1) * pageSize;
            var stop = page * pageSize;
            var historyPKSchool = await _easyRedisClient.SortedSetRangeByRankAsync<SchoolExtSimpleDto>(Key, start, stop, StackExchange.Redis.Order.Descending);
            historySchool.AddRange(historyPKSchool.ToList());
            var total = await _easyRedisClient.SortedSetLengthByValueAsync(Key, 0, 1000000);
            var result = new { Rows = historyPKSchool, PageIndex = page, PageCount = historySchool.Count(), PageSize = pageSize, TotalCount = total };
            return ApiResult.Success("操作成功", result);
        }

        /// <summary>
        /// 获取我的关注
        /// </summary>
        /// <returns></returns>
        public async Task<ResultDto> GetUserCollection(int page = 1, int pageSize = 10) {
            var Ids=new List<Guid>();
            var total = 0;
            var collection = _userService.GetShlCollectionList(GetUserId(), 1).Skip((page - 1) * 10).Take(pageSize);
            Ids.AddRange(collection);
            var data = await _schoolService.ListExtSchoolByBranchIds(Ids) ?? new List<SchoolExtFilterDto>();
            var collectionData = data.Select(d => {
                return new SchoolExtListItemViewModel()
                {
                    SchoolId = d.Sid,
                    BranchId = d.ExtId,
                    SchoolName = d.Name,
                    CityName = d.City,
                    AreaName = d.Area,
                    Distance = d.Distance,
                    LodgingType = (int)LodgingUtil.Reason(d.Lodging, d.Sdextern),
                    Type = d.Type,
                    International = d.International,
                    Cost = d.Tuition,
                    Tags = d.Tags,
                    Score = d.Score,
                    CommentTotal = d.CommentCount
                };
            }).ToList();
            var result = new { Rows= collectionData, PageIndex = page, PageCount = collectionData.Count(), PageSize = pageSize, TotalCount = total };
            return ApiResult.Success("操作成功", result);
        }

        /// <summary>
        /// 新增选择学校
        /// </summary>
        /// <param name="extId"></param>
        public async Task<ResultDto> AddChooseSchool(string extIdJson) {
            if (!string.IsNullOrEmpty(extIdJson)) {
                var userId = GetUserId().ToString();
                var extId = JsonHelper.JSONToObject<List<Guid>>(extIdJson);
                foreach (var e in extId)
                {
                    var schoolDetail = _schoolService.GetSchoolDetailById(e);
                    if (schoolDetail == null) continue;
                    await _easyRedisClient.HashSetAsync($"{_ExtDataKey}{userId}", e.ToString(), schoolDetail);
                }
            }
            
            return ApiResult.Success();
        }

        /// <summary>
        /// 删除选择学校
        /// </summary>
        /// <param name="extId"></param>
        public async Task<ResultDto> DeleteChooseSchool(string extIdJson) {
            if (!string.IsNullOrEmpty(extIdJson)) {
                var userId = GetUserId().ToString();
                LogManager.GetCurrentClassLogger().Info($"userId:{userId},extIdJson:{extIdJson}");
                var extId= JsonHelper.JSONToObject<List<Guid>>(extIdJson);
                for (int i = 0; i < extId.Count; i++) {
                    var id = extId[i].ToString();
                    await _easyRedisClient.HashDeleteAsync($"{_ExtDataKey}{userId}", id);
                }
            }
            return ApiResult.Success();
        }

        /// <summary>
        /// 获取已选学校
        /// </summary>
        /// <returns></returns>
        public async Task<ResultDto> GetChooseSchool()
        {
            var userId = GetUserId().ToString();
            var data = await _easyRedisClient.HashGetAllAsync<PMS.School.Domain.Dtos.SchoolExtDto>($"{_ExtDataKey}{userId}");
            return ApiResult.Success("操作成功", data.Values.ToList());
        }
        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <returns></returns>
        public Guid GetUserId() {
            if (User.Identity.IsAuthenticated)
            {
                return User.Identity.GetUserInfo().UserId;
            }
            return Request.GetDeviceToGuid();
        }


    }
}
