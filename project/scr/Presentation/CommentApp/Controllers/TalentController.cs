using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Nest;
using Newtonsoft.Json;
using PMS.Live.Application.IServices;
using PMS.Live.Domain.Dtos;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using Sxb.Web.Response;
using Sxb.Web.ResponseModel.ViewModels.User.Talent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    public class TalentController : BaseController
    {
        ILectureService _lectureService;
        ITalentService _TalentService;
        IUserService _UserService;
        ICollectionService _collectionService;
        public TalentController(ITalentService talentService, IUserService userService, ILectureService lectureService, ICollectionService collectionService)
        {
            _collectionService = collectionService;
            _lectureService = lectureService;
            _TalentService = talentService;
            _UserService = userService;
        }
        /// <summary>
        /// 获取达人榜
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("获取达人榜")]
        public async Task<ResponseResult> GetTalentRank(int offset = 0, int limit = 10)
        {
            var result = ResponseResult.Failed("No data found");
            var data = await _TalentService.GetTodayTalentRanking();
            if (data?.ID != default)
            {
                try
                {
                    var dataItems = JsonConvert.DeserializeObject<List<TalentRankingItem>>(data.DataJson);
                    if (dataItems?.Any() == true) dataItems = dataItems.Skip(offset).Take(limit).ToList();
                    var userIDs = dataItems.Select(p => p.UserID).Distinct();

                    var talentDetails = _UserService.GetTalentDetails(userIDs);
                    if (talentDetails == null) talentDetails = new List<Talent>();

                    List<LectorLiveStatusDto> liveStatus = new List<LectorLiveStatusDto>();
                    Dictionary<Guid, string> userHeadImgs = new Dictionary<Guid, string>();
                    if (userIDs?.Any() == true)
                    {
                        var livestatus = await _lectureService.GetLectorLiveStatus(userIDs);
                        if (livestatus?.Any() == true) liveStatus = livestatus.ToList();
                        userHeadImgs = _UserService.ListUserInfo(userIDs.ToList())?.ToDictionary(k => k.Id, v => v.HeadImgUrl);
                    }

                    //是否关注
                    var attendedUserIDs = new List<Guid>();
                    if (User.Identity.IsAuthenticated)
                    {
                        var userID = User.Identity.GetUserInfo().UserId;
                        _collectionService.GetCollection(userIDs.ToList(), userID)?.ForEach(item =>
                        {
                            attendedUserIDs.Add(item);
                        });
                    }

                    return ResponseResult.Success(new PageTalentItem()
                    {
                        Items = dataItems.Select(p => new TalentItem()
                        {
                            Distance = p.Distence,
                            HeadImgUrl = userHeadImgs.GetValueOrDefault(p.UserID),
                            Index = p.Index,
                            IsUp = p.IsUp,
                            TalentName = p.TalentName,
                            TalentUserID = p.UserID,
                            LiveID = liveStatus.FirstOrDefault(o => o.UserID == p.UserID && o.Status == PMS.Live.Domain.Enums.LectureStatus.Living)?.LectureID,
                            TalentTitle = p.TalentTitle,
                            IsAttend = attendedUserIDs.Contains(p.UserID),
                            Type = talentDetails.FirstOrDefault(o => o.Id == p.UserID)?.Role ?? 0
                        }),
                        ListDate = data.RankDate,
                        NextUpdateTime = DateTime.Now.AddDays(1).Date.AddMinutes(30)
                    }) ;
                }
                catch (Exception ex)
                {
#if DEBUG
                    result.Msg = ex.Message;
#else
                    result.Msg = "Get data fail";
#endif
                }
            }
            return result;
        }

        /// <summary>
        /// 调度任务-统计当日达人榜
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("调度任务-统计当日达人榜")]
        public ResponseResult CountingSchedule()
        {
            var result = ResponseResult.Failed();
            var nowDate = DateTime.Now.Date;
            var ranking = _TalentService.GetTalentRankingByDay(nowDate);
            if (ranking?.ID != default)
            {
                result.Msg = "Data already existed";
                return result;
            }
            var todayFollowRank = _TalentService.GetTalentFollowRank(0);
            try
            {
                if (_TalentService.AddTalentRanking(todayFollowRank))
                {
                    return ResponseResult.Success();
                }
            }
            catch
            {
                result.Msg = "Insert to DB error";
            }
            return result;
        }
    }
}
