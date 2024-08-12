using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using Sxb.Web.Areas.Home.Models;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Microsoft.Extensions.Configuration;
using Sxb.Web.Common;
using PMS.School.Domain.Entities;
using static PMS.Search.Domain.Common.GlobalEnum;
using System.ComponentModel;
using PMS.OperationPlateform.Application.IServices;
using PMS.TopicCircle.Application.Services;
using PMS.Live.Application.IServices;
using PMS.School.Domain.Dtos;
using ProductManagement.API.Http.Interface;
using PMS.CommentsManage.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.Info;
using PMS.UserManage.Application.ModelDto;

namespace Sxb.Web.Areas.Coupon.Controller
{
    [Route("Api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ApiBaseController
    {
        readonly string _pcNavigationRedisKey = "Navigation:PC";
        string _apiOrgTypeUrl;

        private readonly INavigationService _navigationService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IConfiguration _configuration;
        private readonly ISchoolScoreService _schoolScoreService;

        private readonly ISchoolService _schoolService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly IArticleService _articleService;
        private readonly ITopicService _topicService;
        private readonly ILectureService _lectureService;
        private readonly IUserRecommendClient _userRecommendClient;

        private readonly IAccountService _accountService;

        public HomeController(INavigationService navigationService, IEasyRedisClient easyRedisClient, IConfiguration configuration, ISchoolScoreService schoolScoreService,
            ISchoolService schoolService, ISchoolCommentService commentService, ISchoolRankService schoolRankService, IArticleService articleService, ITopicService topicService, ILectureService lectureService,
            IUserRecommendClient userRecommendClient, IAccountService accountService)
        {
            _schoolScoreService = schoolScoreService;
            _easyRedisClient = easyRedisClient;
            _navigationService = navigationService;
            _configuration = configuration;

            _schoolService = schoolService;
            _commentService = commentService;
            _schoolRankService = schoolRankService;
            _articleService = articleService;
            _topicService = topicService;
            _lectureService = lectureService;
            _accountService = accountService;

            _userRecommendClient = userRecommendClient;
            if (_configuration != null)
            {
                var baseUrl = _configuration.GetSection("ExternalInterface").GetValue<string>("OrganizationAddress");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    _apiOrgTypeUrl = $"{baseUrl.TrimEnd('/')}/api/OrgType";
                }
            }
        }

        /// <summary>
        /// PC首页导航
        /// </summary>
        public async Task<ResponseResult> GetNavigations(int type = 0)
        {
            var response = ResponseResult.Failed();
            var cityCode = Request.GetLocalCity();
            var result = await _easyRedisClient.GetAsync<List<GetNavigationsResponse>>(_pcNavigationRedisKey + $":{cityCode}");
            if (result?.Any() == true)
            {
                if (type > 0)
                {
                    response = ResponseResult.Success(result.Where(p => p.Type == type));
                }
                else
                {
                    response = ResponseResult.Success(result);
                }
            }
            else
            {
                var finds = await _navigationService.GetPCNavigations();
                if (finds?.Any() == true)
                {
                    result = new List<GetNavigationsResponse>();
                    foreach (var find in finds)
                    {
                        var item = new GetNavigationsResponse()
                        {
                            IconUrl = find.Navigation.IconUrl,
                            Index = find.Navigation.Index,
                            Name = find.Navigation.Name,
                            Type = (int)find.Navigation.Type
                        };
                        if (find.Recommends?.Any() == true)
                        {

                            if (find.Recommends.Any(p => p.City == cityCode))
                            {
                                find.Recommends = find.Recommends.Where(p => p.City == cityCode);
                            }
                            else
                            {
                                find.Recommends = find.Recommends.Where(p => p.City == 0);
                            }
                            if (find.Recommends?.Any() == true)
                            {
                                IEnumerable<KeyValuePair<Guid, int>> scores = null;
                                if (find.Recommends.First().Type == PMS.School.Domain.Enum.PCNavigationRecommendType.School)
                                {
                                    scores = await _schoolScoreService.GetSchoolAggScores(find.Recommends.Select(p => p.DataID).Distinct());
                                }
                                item.RecommendItems = find.Recommends.Where(p => p.PCNavigationID == find.Navigation.ID).OrderBy(p => p.Index).Select(p => new RecommendItem()
                                {
                                    ImgUrl = p.ImgUrl,
                                    Index = p.Index,
                                    Name = p.Content,
                                    Type = p.Type,
                                    Url = p.Url,
                                    Score = scores?.Any(x => x.Key == p.DataID) == true ? scores.FirstOrDefault(x => x.Key == p.DataID).Value : 0
                                });
                            }
                        }
                        if (find.Items?.Any() == true)
                        {
                            IEnumerable<PCNavigationItemInfo> cityCubjectTypes;
                            if (find.Items.Any(p => p.ParentID == Guid.Empty && p.PCNavigationID == find.Navigation.ID && p.City == cityCode))
                            {
                                cityCubjectTypes = find.Items.Where(p => p.ParentID == Guid.Empty && p.PCNavigationID == find.Navigation.ID && p.City == cityCode);
                            }
                            else {
                                cityCubjectTypes = find.Items.Where(p => p.ParentID == Guid.Empty && p.PCNavigationID == find.Navigation.ID && p.City == 0);
                            }
                            item.Items = cityCubjectTypes.OrderBy(p => p.Index).Select(p => new NavigationItem()
                                {
                                    Index = p.Index,
                                    Name = p.Name,
                                    Url = p.Url,
                                    SubItems = find.Items.Any(x => x.ParentID == p.ID) ? find.Items.Where(x => x.ParentID == p.ID).OrderBy(x => x.Index)
                                    .Select(x => new NavigationItem()
                                    {
                                        Index = x.Index,
                                        Name = x.Name,
                                        Url = x.Url
                                    }) : null
                                });
                        }
                        result.Add(item);
                    }
                    await _easyRedisClient.AddAsync(_pcNavigationRedisKey + $":{cityCode}", result, TimeSpan.FromHours(12));
                    if (type > 0)
                    {
                        response = ResponseResult.Success(result.Where(p => p.Type == type));
                    }
                    else
                    {
                        response = ResponseResult.Success(result);
                    }
                }
            }
            return response;


        }

        /// <summary>
        /// 获取机构课程导航
        /// </summary>
        public async Task<ResponseResult> GetOrgNavigations(int count = 5)
        {
            var result = ResponseResult.Failed();
            if (count < 5) return result;

            var orgTypes = await _easyRedisClient.GetOrAddAsync("Navigation:OrgTypes", () =>
            {
                try
                {
                    var webClient = new WebUtils();
                    var apiResponse = webClient.DoGet(_apiOrgTypeUrl);
                    if (!string.IsNullOrWhiteSpace(apiResponse))
                    {
                        var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResult>(apiResponse);
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<OrgTypeInfo>>(Newtonsoft.Json.JsonConvert.SerializeObject(object_Response.Data));
                    }
                }
                catch
                {
                }
                return null;
            }, TimeSpan.FromHours(3));
            if (orgTypes?.Any() == true)
            {
                result = ResponseResult.Success(orgTypes.OrderBy(p => p.Sort).Take(count).Append(new OrgTypeInfo()
                {
                    Key = 0,
                    Value = "更多",
                    Sort = count + 1
                }));
            }

            return result;
        }



        #region 移动端H5


        public async Task<ResponseResult> GetH5Navigations()
        {
            //从cookie中  取出省市区  年级
            int city = Request.GetLocalCity();
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            //导航
            var navigations = await _navigationService.GetNavigations();
            if (navigations?.Any() == true)
            {
                if (localCityCode != 440100)
                {
                    var nav = navigations.FirstOrDefault(q => q.Name == "上学问");
                    if (nav != null)
                    {
                        nav.Name = "问答广场";
                        nav.TargetUrl = "/comment?type=1";
                        nav.IconUrl = "https://cos.sxkid.com/images/navigationicon/cf90a312-3385-41d2-83c7-ea64d1d02ad5/6140b09d-78af-4e62-ba93-d5ed5e66d44f.png";
                    }
                }
            }
            return ResponseResult.Success(navigations);
        }

        public async Task<ResponseResult> GetH5SearchSuggest()
        {
            //从cookie中  取出省市区  年级
            int city = Request.GetLocalCity();
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            //搜索框轮询建议
            var searchSuggest = await _easyRedisClient.GetOrAddAsync("Home:SearchSuggest", async () =>
            {
                //获取最新文章
                var newestArticle = await _articleService.HotPointArticles(localCityCode, null, 0, 1);
                //获取最新直播
                var newestLive = await _lectureService.GetNewestLecture(1);
                //获取最新帖子
                var newestTopic = _topicService.GetHotList(null, localCityCode);
                //0 全部, 1 学校 ,2 点评 ,4 问答, 8 学校排名, 16 文章 ,32 直播, 64 话题圈 , 128 达人
                var result = new Dictionary<string, ChannelIndex>();
                if (newestTopic?.Any() == true)
                {
                    result.Add(newestTopic.OrderByDescending(p => p.CreateTime).First().CircleName, ChannelIndex.Circle);
                }
                if (newestLive?.Any() == true)
                {
                    result.Add(newestLive.First().Title, ChannelIndex.Live);
                }
                if (newestArticle.articles?.Any() == true)
                {
                    result.Add(newestArticle.articles.OrderBy(p => p.createTime).First().title, ChannelIndex.Article);
                }
                return result;
            }, TimeSpan.FromHours(1));

            return ResponseResult.Success(searchSuggest.Select(q=>new
            {
                keyword = q.Key,
                index = q.Value
            }));
        }
       
        /// <summary>
        /// 动态加载分页数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="orderBy">orderby 1推荐 2口碑 3.附近</param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="distance"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("首页学校列表数据")]
        public async Task<ResponseResult> GetH5IndexSchoolList(int pageIndex = 1, int pageSize = 10, int orderBy = 1, bool isrecommend = false)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }

            int city = Request.GetLocalCity();
            int province = Request.GetProvince();
            int area = Request.GetArea();
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;

            int distance = 0;


            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            var data = new SchoolExtListDto();
            isrecommend = (orderBy == 1 && pageIndex == 1);

            //如果orderby 1
            //推荐列表 先判断是否有推荐信息
            if (orderBy == 1 || isrecommend)
            {
                var recommend = await GetUserRecommendExt(Guid.Parse(id), pageIndex, pageSize);
                if (recommend != null && recommend.Items != null && recommend.Items.Count() > 0)
                {
                    
                    var extids = recommend.Items.Select(p => new Guid(p.ContentId)).ToArray();
                    var exts = await _schoolService.ListExtSchoolByBranchIds(extids.ToList(), Convert.ToDouble(latitude), Convert.ToDouble(longitude));

                    var extList = exts
                        .Where(p => p.SchoolValid == true && p.ExtValid == true)
                        .Select(p => new SchoolExtItemDto
                        {
                            Name = p.Name,
                            Grade = p.Grade,
                            Type = (byte)p.Type,
                            Area = p.Area,
                            AreaCode = p.AreaCode,
                            City = p.City,
                            CityCode = p.CityCode,
                            Province = p.Province,
                            ProvinceCode = p.ProvinceCode,
                            Score = p.Score,
                            Distance = p.Distance,
                            ExtId = p.ExtId,
                            Sid = p.Sid,
                            Lodging = p.Lodging,
                            Sdextern = p.Sdextern,
                            Tags = p.Tags,
                            Tuition = p.Tuition,
                            SchoolNo = p.SchoolNo
                        }).ToList();
                    //获取评论与排行榜
                    var rank = _schoolRankService.GetH5SchoolRankInfoBy(extids);
                    var comment = _commentService.GetSchoolCardComment(extids.ToList(), PMS.CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
                    extList.ForEach(p =>
                    {
                        p.Ranks = rank.Where(g => g.SchoolId == p.ExtId).ToList();
                        var commentData = comment.FirstOrDefault(g => g.SchoolSectionId == p.ExtId);
                        p.Comment = commentData?.Content;
                        p.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                        p.CommontId = commentData?.Id;
                        p.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0);
                    });

                    data.IsRecommend = true;
                    data.List = extList;
                    data.PageCount = recommend.PageInfo.TotalCount;
                    data.PageIndex = pageIndex;
                    data.PageSize = pageSize;
                }
            }

            if (data.List == null || !data.List.Any())
            {
                var grades = new int[] { };
                var types = new int[] { };
                var Lodging = new int[] { };
                var interest = await GetInterestAsync();
                if (interest != null)
                {
                    grades = interest.Interest.grade.ToArray();
                    types = interest.Interest.nature.ToArray();
                    Lodging = interest.Interest.lodging.ToArray();
                }

                data = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), province, city, area, grades, orderBy, types, distance, Lodging, pageIndex, pageSize);
                data.PageSize = pageSize;
                data.IsRecommend = false;
            }

            return ResponseResult.Success(data);
        }

        private async Task<RecommenderResponseDto> GetUserRecommendExt(Guid id ,int index, int size = 10)
        {
            try
            {
                var response = await _userRecommendClient.UserRecommendSchool(id, index, size);
                var result = new RecommenderResponseDto
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

                if (result.PageInfo.TotalPage < 1 || result.PageInfo.TotalCount < 10)
                {
                    return null;
                }

                return result;

            }
            catch
            {
            }
            return null;
        }
        private async Task<ApiInterestVo> GetInterestAsync()
        {
            //http请求api
            var config = _configuration.GetSection("UserSystemConfig");
            var ip = config.GetSection("ServerUrl").Value;
            var uuid = Request.GetDevice(Guid.NewGuid().ToString());
            Guid? userid = null;
            var url = string.Empty;

            string key = $"interest-{uuid}";
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userid = user.UserId;
                key = $"interest-{user.UserId}-{uuid}";
                
            }
            return await _easyRedisClient.GetOrAddAsync(key,()=> { return _accountService.GetApiInterest(userid, uuid); });
        }

        #endregion
    }
}
