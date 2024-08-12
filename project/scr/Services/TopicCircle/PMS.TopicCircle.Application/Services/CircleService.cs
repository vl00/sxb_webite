using AutoMapper;
using Org.BouncyCastle.Asn1.Crmf;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using static PMS.UserManage.Domain.Common.EnumSet;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace PMS.TopicCircle.Application.Services
{
    public class  CircleService: ApplicationService<Circle>, ICircleService
    {
        private readonly IMapper _mapper;


        ICircleRepository _repository;
        CircleCoverService _circleCoverService;
        CircleFollowerService _circleFollowerService;
        CircleAccessLogService _circleAccessLogService;
        ICollectionService _collectionService;
        ISysMessageService _sysMessageService;
        UserManage.Application.IServices.IUserService _userService;
        IEasyRedisClient _easyRedisClient;
        IWeChatAppClient _weChatAppClient;
        IWeChatQRCodeService _weChatQRCodeService;

        private readonly IDataSearch _dataSearch;

        public CircleService(
            ICircleRepository repository,
            ICircleCoverService circleCoverService,
            ICircleFollowerService circleFollowerService, IMapper mapper,
            ICircleAccessLogService circleAccessLogService, ICollectionService collectionService,
            ISysMessageService sysMessageService, IUserService userService, IDataSearch dataSearch
            , IEasyRedisClient easyRedisClient
            , IWeChatAppClient weChatAppClient
            , IWeChatQRCodeService weChatQRCodeService) : base(repository)
        {
            this._repository = repository;
            this._circleCoverService = circleCoverService as CircleCoverService;
            this._circleFollowerService = circleFollowerService as CircleFollowerService;
            _mapper = mapper;
            _circleAccessLogService = circleAccessLogService as CircleAccessLogService;
            _collectionService = collectionService;
            _sysMessageService = sysMessageService;
            _userService = userService;
            _dataSearch = dataSearch;
            _easyRedisClient = easyRedisClient;
            _weChatAppClient = weChatAppClient;
            _weChatQRCodeService = weChatQRCodeService;
        }

        public AppServiceResultDto CheckPermission(CircleCheckPermissionRequestDto input)
        {
            Circle circle = this._repository.Get(input.CircleId);
            if (circle == null)
            {
                return AppServiceResultDto.Failure($"该圈子不存在,目标ID:{input.CircleId}");
            }
            if (circle.UserId == input.UserId)
            {
                return AppServiceResultDto.Success();
            }
            else
            {
                return AppServiceResultDto.Failure("该圈子不属于这个圈主");
            }
        }

        public AppServiceResultDto<CircleCreateReturnDto> Create(CircleCreateRequestDto input)
        {
            Circle circle = new Circle()
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                UserId = input.UserId,
                Intro = input.Intro,
                ModifyTime = DateTime.Now,
                CreateTime = DateTime.Now,
                BGColor = input.BGColor
            };
            CircleCover cover = new CircleCover()
            {
                Id = Guid.NewGuid(),
                Url = input.CoverUrl,
                CircleId = circle.Id,
                Modifytime = DateTime.Now
            };
            bool isSuccess = this._repository.CreateTran(circle, cover);

            if (isSuccess)
            {
                return AppServiceResultDto.Success<CircleCreateReturnDto>(circle);
            }
            else
            {
                return AppServiceResultDto.Failure<CircleCreateReturnDto>("保存数据库失败");
            }
        }

        public AppServiceResultDto Edit(CircleEditRequestDto input)
        {
            Circle circle = this._repository.Get(input.CircleId);
            if (circle != null)
            {
                circle.Intro = input.Intro;
                circle.Name = input.Name;
                circle.ModifyTime = DateTime.Now;
                circle.BGColor = input.BGColor;
                circle = this._circleCoverService.GetCover(circle);
                if (circle.Cover == null)
                {
                    this._circleCoverService.Add(new CircleCover()
                    {
                        Id = Guid.NewGuid(),
                        CircleId = circle.Id,
                        Url = input.CoverUrl,
                        Modifytime = DateTime.Now


                    });
                }
                else
                {
                    circle.Cover.Url = input.CoverUrl;
                    circle.Cover.Modifytime = DateTime.Now;
                }
                var isSuccess = this._repository.EditTran(circle, circle.Cover);
                return AppServiceResultDto.Success();
            }
            else
            {
                return AppServiceResultDto.Failure("无该圈子");
            }

        }

        public AppServiceResultDto<IEnumerable<CircleItemDto>> GetCircles(Guid userId)
        {
            var circles = this._repository.GetCircles(userId);
            if (circles == null || !circles.Any())
            {
                return AppServiceResultDto.Failure<IEnumerable<CircleItemDto>>("该用户未创建任何话题圈。");
            }
            var userIDs = circles.Where(p => p.UserId.HasValue).Select(p => p.UserId.Value).Distinct();
            var userInfos = _userService.ListUserInfo(userIDs?.ToList());
            var dtos = circles.Select(c => new CircleItemDto()
            {
                Id = c.Id,
                Intro = c.Intro,
                Name = c.Name,
                BGColor = c.BGColor,
                CircleMasterName = userInfos.FirstOrDefault(p => p.Id == c.UserId)?.NickName,
                Cover = _circleCoverService.GetCover(new Circle() { Id = c.Id })?.Cover?.Url,
                FollowCount = _circleFollowerService.GetFollowerCount(new Circle { Id = c.Id })?.FollowerCount ?? 0
            });
            return AppServiceResultDto.Success(dtos);
        }

        public AppServiceResultDto<CircleDetailDto> GetTalentCircleDetail(Guid userID)
        {
            Circle circle = this._repository.GetCircles(userID).FirstOrDefault();
            if (circle == null)
            {
                return AppServiceResultDto.Failure<CircleDetailDto>("该用户未创建任何话题圈。");
            }
            if (circle.IsDisable)
            {
                return AppServiceResultDto.Failure<CircleDetailDto>("该用户已禁止话题圈对外开放。");
            }
            circle = this._circleCoverService.GetCover(circle);
            circle = this._circleFollowerService.GetFollowerCount(circle);
            return AppServiceResultDto.Success<CircleDetailDto>(circle);
        }

        public AppServiceResultDto<CircleDetailDto> GetDetail(Guid circleID, DateTime? time)
        {
            Circle circle = this._repository.Get(circleID);
            if (circle == null)
            {
                return AppServiceResultDto.Failure<CircleDetailDto>("找不到该话题圈");
            }
            circle = this._circleCoverService.GetCover(circle);
            circle = this._circleFollowerService.GetFollowerCount(circle);
            CircleDetailDto circleDetail = circle;
            circleDetail.NewFollower = this._circleFollowerService.GetNewFollowCounts(circleDetail.Id, time);
            circleDetail.TopicCount = this._repository.TopicCount(circleDetail.Id);
            circleDetail.YesterdayTopicCount = this._repository.YesterdayTopicCount(circleDetail.Id);
            circleDetail.YesterdayFollowerCount = this._repository.YesterdayFollowerCount(circleDetail.Id);
            return AppServiceResultDto.Success(circleDetail);
        }

        public AppServiceResultDto<IEnumerable<CircleDetailDto>> GetRecommends(int cityCode)
        {
            IEnumerable<Circle> circles = this._repository.ExcuteUSP_STATISTICNEWSCIRCLE(cityCode);
            circles = this._circleCoverService.GetCover(circles);
            circles = this._circleFollowerService.GetFollowerCount(circles);

            return AppServiceResultDto.Success(circles.Select<Circle, CircleDetailDto>(c => c));
        }

        public AppServiceResultDto JoinCircle(CircleJoinRequestDto input)
        {
            //检查加入的人是不是圈主自己，就是
            Circle circle = this._repository.Get(input.CircleId);
            if (circle == null)
            {
                return AppServiceResultDto.Failure("圈子不存在，加入失败");
            }
            else if (circle.UserId == input.UserId)
            {
                return AppServiceResultDto.Failure("圈主不需要加入自己圈子。");
            }
            if (circle.IsDisable)
            {
                //return AppServiceResultDto.Failure("当前圈子禁止用户加入。");
            }
            //检查是否已关注
            bool isFollow = this._circleFollowerService.CheckIsFollow(new CheckIsFollowRequestDto()
            {
                CircleId = input.CircleId,
                UserId = input.UserId
            });
            if (isFollow)
            {
                return AppServiceResultDto.Failure(null, CodeEnum.HasFollowCircle);
            }

            bool joinResult = this._circleFollowerService.Add(new CircleFollower()
            {
                Id = Guid.NewGuid(),
                CircleId = input.CircleId,
                UserId = input.UserId,
                Time = DateTime.Now,
                ModifyTime = DateTime.Now
            });
            if (joinResult)
            {
                var talentUser = this._userService.GetTalentDetail(circle.UserId.GetValueOrDefault());
                //帮助用户关注达人
                _collectionService.AddCollection(input.UserId, circle.UserId.Value, (byte)CollectionDataType.UserLector);
                _sysMessageService.AddSysMessage(new List<UserManage.Domain.Entities.SysMessage>() {
                 new UserManage.Domain.Entities.SysMessage(){
                      Id = Guid.NewGuid(),
                      SenderUserId =circle.UserId.GetValueOrDefault(),
                      UserId = input.UserId,
                      DataId = circle.Id,
                      DataType = MessageDataType.Circle,
                      PushTime = DateTime.Now,
                      Type=  UserManage.Domain.Common.SysMessageType.JoinCircleNotity,
                      Title = $"您已经成功加入{talentUser.Nickname}{circle.Name}话题圈，点击查看话题动态>>",
                      Content = $"您已经成功加入{talentUser.Nickname}{circle.Name}话题圈，点击查看话题动态>>"
                 }
                });
                return AppServiceResultDto.Success();
            }
            else
            {
                return AppServiceResultDto.Failure("加入失败");
            }

        }

        public AppServiceResultDto ExitCircle(CircleExitRequestDto input)
        {
            var res = this._circleFollowerService.Delete(input.UserId, input.CircleId);
            if (res)
            {
                return AppServiceResultDto.Success();
            }
            else
            {
                return AppServiceResultDto.Failure("退出操作失败.");
            }
        }



        public AppServiceResultDto<IEnumerable<MyCircleItemDto>> GetTalentCircles(Guid userId)
        {
            var circle = this._repository.GetByUserId(userId);
            if (circle == null)
            {
                return AppServiceResultDto.Failure<IEnumerable<MyCircleItemDto>>("当前达人未创建任何圈子");
            }
            circle = this._circleCoverService.GetCover(circle);
            //最后一次访问圈子时间来源于圈子访问日志表
            CircleAccessLog circleAccessLog = _circleAccessLogService.GetLatest(circle.Id, userId);
            var circleIncludeStaticInfo = _repository.ExcuteUSP_STATICCIRCLENEWSINFO(circleAccessLog?.CreateTime, new List<Guid>() { circle.Id });
            IEnumerable<MyCircleItemDto> myCircles = circleIncludeStaticInfo.Select<USPSTATICCIRCLENEWSINFO, MyCircleItemDto>(mycircle => mycircle);
            myCircles = myCircles.Select(mc =>
            {
                mc.IsCircleMaster = true;
                mc.Cover = circle.Cover?.Url;
                return mc;
            });
            AppServiceResultDto<IEnumerable<MyCircleItemDto>> dtos = AppServiceResultDto.Success(myCircles);
            return dtos;
        }

        public AppServiceResultDto<IEnumerable<MyCircleItemDto>> GetMyCircles(GetMyCirclesRequestDto input)
        {
            List<MyCircleItemDto> endresult = new List<MyCircleItemDto>();
            var circles = this._repository.GetByFollower(new CircleFollower() { UserId = input.UserId });
            circles = this._circleCoverService.GetCover(circles);
            var userInfos = _userService.ListUserInfo(circles.Where(p => p.UserId.HasValue).Select(p => p.UserId.Value).Distinct().ToList());
            foreach (var circle in circles)
            {
                CircleAccessLog circleAccessLog = _circleAccessLogService.GetLatest(circle.Id, input.UserId);
                var circleIncludeStaticInfo = _repository.ExcuteUSP_STATICCIRCLENEWSINFO(circleAccessLog?.CreateTime, new List<Guid>() { circle.Id });
                MyCircleItemDto myCircle = circleIncludeStaticInfo.Select<USPSTATICCIRCLENEWSINFO, MyCircleItemDto>(mycircle => mycircle).FirstOrDefault();
                myCircle.Cover = circle.Cover?.Url;
                myCircle.IsCircleMaster = false;
                myCircle.CircleMasterName = userInfos.FirstOrDefault(p => p.Id == circle.UserId)?.NickName;
                endresult.Add(myCircle);
            }
            endresult = endresult.OrderByDescending(m => m.NEWTOPICCOUNT + m.NEWREPLYCOUNT)
                .ThenByDescending(m => m.NEWTOPICCOUNT)
                .ThenByDescending(m => m.NEWREPLYCOUNT)
                .ThenByDescending(m => m.FOLLOWERCOUNT)
                .ThenBy(m => m.Name)
                .ToList();
            AppServiceResultDto<IEnumerable<MyCircleItemDto>> dtos = AppServiceResultDto.Success<IEnumerable<MyCircleItemDto>>(endresult);
            return dtos;
        }

        public async Task<AppServicePageResultDto<IEnumerable<MyCircleItemDto>>> GetMyCircles(Guid userID, int offset = 0, int limit = 10)
        {
            List<MyCircleItemDto> endresult = new List<MyCircleItemDto>();
            var result = await _repository.GetByFollower(userID, offset, limit);
            var circles = result.Item1;
            circles = _circleCoverService.GetCover(circles);
            var userInfos = _userService.ListUserInfo(circles.Where(p => p.UserId.HasValue).Select(p => p.UserId.Value).Distinct().ToList());
            foreach (var circle in circles)
            {
                CircleAccessLog circleAccessLog = _circleAccessLogService.GetLatest(circle.Id, userID);
                var circleIncludeStaticInfo = _repository.ExcuteUSP_STATICCIRCLENEWSINFO(circleAccessLog?.CreateTime, new List<Guid>() { circle.Id });
                MyCircleItemDto myCircle = circleIncludeStaticInfo.Select<USPSTATICCIRCLENEWSINFO, MyCircleItemDto>(mycircle => mycircle).FirstOrDefault();
                myCircle.Cover = circle.Cover?.Url;
                myCircle.IsCircleMaster = false;
                myCircle.CircleMasterName = userInfos.FirstOrDefault(p => p.Id == circle.UserId)?.NickName;
                endresult.Add(myCircle);
            }
            AppServicePageResultDto<IEnumerable<MyCircleItemDto>> dtos = AppServiceResultDto.Success<IEnumerable<MyCircleItemDto>>(endresult, result.Item2);
            return dtos;
        }

        public AppServiceResultDto<CircleToggleDisableStatuDto> ToggleDisableStatu(Guid circleId)
        {
            Circle circle = this._repository.Get(circleId);
            if (circle == null)
            {
                return AppServiceResultDto.Failure<CircleToggleDisableStatuDto>("找不到该话题圈");
            }
            else
            {
                circle.IsDisable = !circle.IsDisable; // 取反
                bool updateStatu = this._repository.Update(circle, fields: new[] { "IsDisable" });
                if (updateStatu)
                {
                    return AppServiceResultDto.Success(new CircleToggleDisableStatuDto()
                    {
                        CircleId = circle.Id,
                        IsDisable = circle.IsDisable
                    });
                }
                else
                {
                    return AppServiceResultDto.Failure<CircleToggleDisableStatuDto>("操作失败");
                }
            }

        }

        /// <summary>
        /// 检查用户是否已经创建了话题圈
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckIsHasCircle(Guid userId)
        {
            var circles = this._repository.GetCircles(userId);
            return circles != null && circles.Any();
        }

        /// <summary>
        /// 根据圈子id, 查询圈子信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SearchCircleDto> GetCircles(IEnumerable<Guid> ids, Guid? loginUserId)
        {
            var circles = _repository.GetList(ids);
            var dto = _mapper.Map<List<SearchCircleDto>>(circles);

            var covers = _circleCoverService.GetCover(ids);
            var followerCounts = _circleFollowerService.GetFollowerCount(ids);

            dto.ForEach(s =>
            {
                s.Cover = covers.FirstOrDefault(c => c.CircleId == s.Id)?.Url;
                s.FollowCount = followerCounts.FirstOrDefault(c => c.CircleId == s.Id)?.FollowerCount ?? 0;
            });

            //登录用户的关注情况
            if (loginUserId != null)
            {
                var followers = _circleFollowerService.GetFollowers(loginUserId.Value, ids);
                dto.ForEach(s =>
                {
                    s.LoginUserFollow = loginUserId == s.UserId ?
                    true :
                    followers.FirstOrDefault(c => c.CircleId == s.Id) != null;
                });
            }
            return dto;
        }

        /// <summary>
        /// 根据圈子id, 查询圈子信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public AppServicePageResultDto<List<SearchCircleDto>> SearchCircles(Guid? loginUserId, SearchBaseQueryModel queryModel)
        {
            var searchResult = _dataSearch.SearchCircle(out long total, queryModel);
            var ids = searchResult.Select(s => s.Id);

            var circles = SearchCircles(ids, loginUserId);
            circles.ForEach(circle =>
            {
                var searchCircle = searchResult.FirstOrDefault(s => s.Id == circle.Id);
                circle.Name = searchCircle?.Name;
                circle.UserName = searchCircle?.UserName;
            });

            return AppServiceResultDto.Success(circles, total: total);
        }

        public List<SearchCircleDto> SearchCircles(IEnumerable<Guid> ids, Guid? loginUserId)
        {
            if (ids.Any())
            {
                var circleDtos = GetCircles(ids, loginUserId);
                //使用原有序列, 原有圈粉数量, 避免排序差异
                return ids
                    .Where(q => circleDtos.Any(p => p.Id == q))
                    .Select(q =>
                    {
                        var circle = circleDtos.First(p => p.Id == q);
                        return circle;
                    })
                    .ToList();
            }
            return new List<SearchCircleDto>();
        }


        public IEnumerable<Circle> GetByIDs(IEnumerable<Guid> ids)
        {
            return _repository.GetBy("ID in @ids", new { ids });
        }

        public async Task<IEnumerable<CircleDetailDto>> GetCircles(int count = 8, int provinceCode = 0, int cityCode = 0, IEnumerable<Guid> notInIDs = null)
        {
            if (count < 0 || cityCode < 0) return null;
            var result = new List<CircleDetailDto>();
            var finds = await _repository.GetList(count, provinceCode, cityCode, notInIDs);
            if (finds?.Any() == true)
            {
                var covers = _circleCoverService.GetCover(finds.Select(p => p.Id));
                var statistics = await _repository.GetStatistics(finds.Select(p => p.Id));
                if (covers == null) covers = new List<CircleCover>();
                foreach (var item in finds)
                {
                    var staticInfo = statistics.FirstOrDefault(p => p.Id == item.Id) ?? new CircleStatisticsDto();
                    var entity = new CircleDetailDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        City = item.City ?? 0,
                        Cover = covers.FirstOrDefault(o => o.CircleId == item.Id)?.Url,
                        UserName = item.UserName,
                        FollowerCount = item.FollowerCount,
                        DynamicCount = staticInfo.DynamicCount,
                        TopicCount = staticInfo.TopicCount
                    };
                    result.Add(entity);
                }
            }
            return result;
        }

        public async Task<(IEnumerable<CircleDetailDto>, int)> GetAllCircles(Guid userID, int offset = 0, int limit = 14, int order = 1)
        {
            IEnumerable<SimpleCircleDto> finds = null;
            switch (order)
            {
                case 1:
                    finds = await _repository.GetSelfFirstCircles(userID, offset, limit);
                    break;
                case 2:
                    finds = await _repository.GetSelfFirstCircles(userID, offset, limit, "c.CreateTime desc");
                    break;
                case 3:
                    finds = await _repository.GetCircleOrderByYesterdayActive(userID, offset, limit);
                    break;
            }
            if (finds?.Any() == true)
            {
                var total = await _repository.Count();
                var statistics = await _repository.GetStatistics(finds.Select(p => p.Id));
                var result = new List<CircleDetailDto>();
                var covers = _circleCoverService.GetCover(finds);
                foreach (var item in finds)
                {
                    var staticInfo = statistics.FirstOrDefault(p => p.Id == item.Id) ?? new CircleStatisticsDto();
                    var entity = new CircleDetailDto()
                    {
                        Id = item.Id,
                        Cover = covers.FirstOrDefault(p => p.Id == item.Id)?.Cover?.Url,
                        City = item.City ?? 0,
                        FollowerCount = item.FollowerCount,
                        Intro = item.Intro,
                        IsCircleMaster = item.UserId == userID,
                        UserId = item.UserId ?? Guid.Empty,
                        IsDisable = item.IsDisable,
                        IsFollowed = item.IsFollowed,
                        Name = item.Name,
                        DynamicCount = staticInfo.DynamicCount,
                        TopicCount = staticInfo.TopicCount
                    };
                    result.Add(entity);
                }

                if (result?.Any() == true)
                {
                    if (order == 1)
                    {
                        var sorted_Result = new List<CircleDetailDto>();
                        if (result.Any(p => p.UserId == userID)) sorted_Result.Add(result.FirstOrDefault(p => p.UserId == userID));
                        sorted_Result.AddRange(CommonHelper.ListRandom(result.Where(p => p.IsFollowed && p.UserId != userID)));
                        sorted_Result.AddRange(CommonHelper.ListRandom(result.Where(p => !p.IsFollowed && p.UserId != userID)));
                        return (sorted_Result, total);
                    }
                    return (result, total);
                }
            }
            return (new List<CircleDetailDto>(), 0);
        }

        public Task<IEnumerable<KeyValuePair<Guid, bool>>> CheckIsFollowCircle(IEnumerable<Guid> circleIDs, Guid userID)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateCircleCountData()
        {
            return await _repository.UpdateCircleCountData();
        }

        public async Task<string> GenerateWeChatCode(Guid circleId, string callbackUrl, bool isMiniApp = false)
        {
            string sceneKey = $"joincircle:isMiniApp_{isMiniApp}:" + circleId.ToString("N");
            int expire_second = (int)TimeSpan.FromDays(30).TotalSeconds; //30天有效期的二维码
            var accessToken = await this._weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            string subscribeHandleUrl = callbackUrl; // $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={circleId}&type={(int)SubscribeCallBackType.joincircle}";
            _ = this._easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));
            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            Scene.SetScene(sceneKey);
            var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);
            return qrcodeResponse?.ImgUrl;


        }

        public Dictionary<Guid, bool> GetCirclesHasNews(IEnumerable<Guid> ids, Guid userID)
        {
            var result = new Dictionary<Guid, bool>();
            if (ids?.Any() == true && userID != Guid.Empty)
            {
                foreach (var id in ids)
                {
                    CircleAccessLog circleAccessLog = _circleAccessLogService.GetLatest(id, userID);
                    var circleIncludeStaticInfo = _repository.ExcuteUSP_STATICCIRCLENEWSINFO(circleAccessLog?.CreateTime, new List<Guid>() { id });
                    if (circleIncludeStaticInfo?.Any() == true)
                    {
                        var find = circleIncludeStaticInfo.FirstOrDefault();
                        result.Add(find.Id, find.HASNEWS);
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<dynamic>> ExportStaticData(DateTime btime, DateTime etime)
        {
            return await _repository.ExportStaticData(btime, etime);
        }
    }
}
