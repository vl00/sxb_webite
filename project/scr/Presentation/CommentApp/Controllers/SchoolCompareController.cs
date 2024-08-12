using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using Sxb.Web.RequestModel.School;
using Sxb.Web.Response;
using Sxb.Web.ViewModels.School;
using PMS.School.Domain.Common;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using ProductManagement.Framework.Cache.Redis;
using PMS.UserManage.Domain.Common;
using Sxb.Web.Common;
using PMS.OperationPlateform.Domain.Entitys;
using System.Text;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Question;
using Sxb.Web.Utils;
using PMS.UserManage.Application.IServices;
using AutoMapper;
using PMS.OperationPlateform.Application.IServices;
using PMS.CommentsManage.Application.IServices;
using Sxb.Web.Models;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Interface;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using ProductManagement.Framework.Foundation;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using PMS.CommentsManage.Application.Common;
using System.ComponentModel;
using PMS.OperationPlateform.Domain.Enums;
using ProductManagement.API.Http.Model;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Application.ModelDto;
using MongoDB.Driver;
using MongoDB.Bson;
using CommentApp.Models.School;
using PMS.CommentsManage.Domain.Entities;
using Newtonsoft.Json.Linq;
using NSwag.Annotations;

namespace Sxb.Web.Controllers
{
    [OpenApiIgnore]
    public class SchoolCompareController : Controller
    {
        private readonly ISchoolService _schoolService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly IQuestionInfoService _questionInfoService;
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration _configuration;
        private IArticleService _articleService;
        private readonly IEasyRedisClient _easyRedisClient;
        readonly ICityInfoService _cityCodeService;
        private readonly IMapper _mapper;
        private readonly ImageSetting _setting;
        private readonly MongoSetting _mongosetting;
        private readonly IUserService _userService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ISchoolInfoService _schoolInfoService;
        private PKSchools pkSchools = new PKSchools();
        private List<SchoolExtDto> CollectSchoolList = new List<SchoolExtDto>();
        private readonly ISchoolRankService _schoolRankService;

        public SchoolCompareController(
            ICommentReportService commentReportService,
            IAnswerReportService reportService,
            ISchoolService schoolService,
            ISchoolCommentService schoolComment,
            ISchoolCommentScoreService commentScoreService,
            IQuestionInfoService questionInfoService,
            IUserService userService,
            ICityInfoService cityService,
            IHttpClientFactory clientFactory,
            IMapper mapper,
            IOptions<ImageSetting> set,
            IOptions<MongoSetting> mongoset,
            IConfiguration iConfig,
            IArticleService articleService,
            IEasyRedisClient easyRedisClient,
            IUserServiceClient userServiceClient,
            ISchoolInfoService schoolInfoService,
            ISchoolRankService schoolRankService)
        {
            _cityCodeService = cityService;
            _schoolService = schoolService;
            _commentService = schoolComment;
            _commentScoreService = commentScoreService;
            _questionInfoService = questionInfoService;
            _configuration = iConfig;
            _clientFactory = clientFactory;
            _articleService = articleService;
            _easyRedisClient = easyRedisClient;
            _userService = userService;
            _mapper = mapper;
            _setting = set.Value;
            _mongosetting = mongoset.Value;
            _userServiceClient = userServiceClient;
            _schoolInfoService = schoolInfoService;
            _schoolRankService = schoolRankService;
            pkSchools.PKSchoolList = new List<SchoolExtDto>();

            //if (PrePKSchools.prePKSchoolList != null && PrePKSchools.prePKSchoolList.Count > 0)
            //{
            //    foreach (var it in PrePKSchools.prePKSchoolList)
            //    {
            //        pkSchools.PKSchoolList.Add(it);
            //    }
            //}
        }

        public async Task<(List<SchoolExtScoreIndex> index, List<SchoolExtScore> score)> GetSchoolExtScoreAsync(Guid extId)
        {
            var index = await _schoolService.schoolExtScoreIndexsAsync();
            //临时变动(大项：课程18 学术屏蔽19  子项：以上子项及入学难度2)
            index = index.Where(p => p.Id != 18 && p.Id != 19 && p.ParentId != 18 && p.ParentId != 19 && p.Id != 2).ToList();
            if (index == null || index.Count == 0)
                return (null, null);
            else
            {
                var score = await _schoolService.GetSchoolExtScoreAsync(extId);
                return (index, score);
            }
        }

        #region 旧PK

        /// <summary>
        /// 学校PK
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderBy">orderby 1推荐 2口碑 3.附近</param>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <param name="size"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        [Description("学校PK")]
        public async Task<IActionResult> ExtPKList(Guid[] extId = default(Guid[]), int type = 0, int orderBy = 1, int index = 1, int distance = 0, int size = 10, int province = 0, int city = 0, int area = 0)
        {
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());

            string Key = "";
            Guid userId = default(Guid);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                Key = $"SchoolPKList-1:{userId}";
            }
            else
            {
                userId = Request.GetDeviceToGuid();
                Key = $"SchoolPKList-0:{userId}";
            }
         
            //var tmp = await _easyRedisClient.SortedSetRemoveRangeByRankAsync(Key, 0, 50, StackExchange.Redis.CommandFlags.FireAndForget);
            var PKSchools = await _easyRedisClient.SortedSetRangeByRankAsync<PKSchool>(Key, 0, 50, StackExchange.Redis.Order.Descending);
            var pkList = PKSchools.Any()?PKSchools.ToList(): new List<PKSchool>();
            foreach (var item in pkList)
            {
                var School = _schoolService.GetSchoolDetailById(item.ExtId, latitude, longitude);
                pkSchools.PKSchoolList.Add(School);
            }
            if (extId.Length == 0)
            {
                return View(pkSchools);
            }
            else
            {
                if (pkSchools.PKSchoolList != null && pkSchools.PKSchoolList.Count > 0)
                {
                    foreach (var itm in extId)
                    {   //过滤重复勾选的内容
                        bool isSelect = false;
                        foreach (var select in pkSchools.PKSchoolList)
                        {
                            isSelect |= select.ExtId == itm;
                        }
                        if (isSelect == false)
                        {
                            var data = _schoolService.GetSchoolDetailById(itm, latitude, longitude);
                            PKSchool sch = new PKSchool()
                            {
                                Sid = data.Sid,
                                ExtId = data.ExtId
                            };
                            await _easyRedisClient.SortedSetAddAsync(Key, sch, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                            pkSchools.PKSchoolList.Insert(0, data);
                        }
                    }
                }
                else
                {
                    foreach (var itm in extId)
                    {
                        var data = _schoolService.GetSchoolDetailById(itm, latitude, longitude);
                        PKSchool sch = new PKSchool()
                        {
                            Sid = data.Sid,
                            ExtId = data.ExtId
                        };
                        await _easyRedisClient.SortedSetAddAsync(Key, sch, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                        pkSchools.PKSchoolList.Insert(0, data);
                    }
                }

                return View(pkSchools);
            }

        }

        /// <summary>
        /// 学校PK
        /// </summary>
        /// <param name="extId">学部IDs</param>
        /// <returns></returns>
        [Description("删除学校PK列表数据")]
        public async Task<IActionResult> ExtPKListDele(Guid[] extId = default(Guid[]))
        {
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            string Key = "";
            Guid userId = default(Guid);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                Key = $"SchoolPKList-1:{userId}";
            }
            else
            {
                userId = Request.GetDeviceToGuid();
                Key = $"SchoolPKList-0:{userId}";
            }

            List<PKSchool> delData = new List<PKSchool>();
            foreach (var itm in extId)
            {
                SchoolExtDto data = _schoolService.GetSchoolDetailById(itm, latitude, longitude);
                PKSchool sch = new PKSchool()
                {
                    Sid = data.Sid,
                    ExtId = data.ExtId
                };
                delData.Add(sch);
            }
            if (delData != null && delData.Count > 0)
            {
                var index = await _easyRedisClient.SortedSetRemoveAsync(Key, delData, StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var PKSchools = await _easyRedisClient.SortedSetRangeByRankAsync<PKSchool>(Key, 0, 50, StackExchange.Redis.Order.Descending);
            var pkList = PKSchools.ToList();
            List<Guid> last = new List<Guid>();
            foreach (var itm in pkList)
            {
                last.Add(itm.ExtId);
                var shool = _schoolService.GetSchoolDetailById(itm.ExtId, latitude, longitude);
                pkSchools.PKSchoolList.Add(shool);
            }
            ViewBag.ResidueExtId = last.ToArray<Guid>();

            return Json(ResponseResult.Success(last.ToArray<Guid>())); //View(pkSchools);
        }

        /// <summary>
        /// 增加PK学校
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderBy"></param>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <param name="size"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExtAddSchool(int type = 0, int orderBy = 1, int index = 1, int distance = 0, int size = 10, int province = 0, int city = 0, int area = 0)
        {
            SchoolPKDto modeldto = new SchoolPKDto();
            modeldto.recommendSchools = new SchoolExtListDto();
            modeldto.recommendSchools.List = new List<SchoolExtItemDto>();

            modeldto.historySchools = new HistoryPKShoolsDto();
            modeldto.historySchools.historySchoolList = new List<SchoolExtSimpleDto>();

            modeldto.CollectSchools = new CollectPKSchoolsDto();
            modeldto.CollectSchools.CollectSchoolList = new List<SchoolExtDto>();

            //从cookie中  取出省市区  年级
            if (city == 0)
            {
                city = Request.GetLocalCity();
            }
            else
            {
                Response.SetLocalCity(city.ToString());
                if (province == 0 && area == 0)
                {
                    Response.SetProvince("0");
                    Response.SetArea("0");
                }
            }
            if (province == 0)
            {
                province = Request.GetProvince();
            }
            else
            {
                Response.SetProvince(province.ToString());
            }
            if (area == 0)
            {
                area = Request.GetArea();
            }
            else
            {
                Response.SetArea(area.ToString());
            }
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            //获取年级  类型  住宿
            var grades = new int[] { };
            var types = new int[] { };
            var Lodging = new int[] { };
            ApiInterest interest = await GetInterestAsync();
            if (interest != null)
            {
                grades = interest.interest.grade;
                types = interest.interest.nature;
                Lodging = interest.interest.lodging;
            }
            var list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), province, city, area, grades, orderBy, types, distance, Lodging, index, size);

            //将数据保存在前台
            ViewBag.Province = province;
            ViewBag.City = city;
            ViewBag.Area = area;
            ViewBag.Latitude = latitude;
            ViewBag.Longitude = longitude;
            modeldto.recommendSchools = list;//推荐
            modeldto.recommendSchools.PageSize = size;
            ViewBag.PageSize = (list.PageCount + size - 1) / size;

            List<SchoolExtSimpleDto> historySchool = new List<SchoolExtSimpleDto>();
            string Key = "";
            Guid userId = default(Guid);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                Key = $"SchoolPK-1:{userId}";
            }
            else
            {
                userId = Request.GetDeviceToGuid();
                Key = $"SchoolPK-0:{userId}";
            }

            var historyPKSchool = await _easyRedisClient.SortedSetRangeByRankAsync<SchoolExtSimpleDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            historySchool.AddRange(historyPKSchool.ToList());

            //历史记录
            modeldto.historySchools.PageSize = 10;
            modeldto.historySchools.PageCount = 10;
            modeldto.historySchools.historySchoolList = historySchool;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                List<Guid> collectIds = _userService.GetShlCollectionList(userId, 1);
                foreach (var itm in collectIds)
                {
                    CollectSchoolList.Add(_schoolService.GetSchoolDetailById(itm, Convert.ToDouble(latitude), Convert.ToDouble(longitude)));
                }
                //关注列表
                modeldto.CollectSchools.PageSize = 10;
                modeldto.CollectSchools.PageCount = 10;
                if (CollectSchoolList != null && CollectSchoolList.Count >= 10)
                    modeldto.CollectSchools.CollectSchoolList = CollectSchoolList.GetRange(0, 10);
                else
                    modeldto.CollectSchools.CollectSchoolList = CollectSchoolList;
            }

            return View(modeldto);
        }

        /// <summary>
        /// 学校PK添加学校列表动态分页数据
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="tabType">添加学校列表类型（1推荐、2历史、3关注）</param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校PK添加学校列表动态分页数据")]
        public async Task<IActionResult> PKSchoolsRecommendAddPartial(int index = 1, int orderBy = 1, int province = 0, int city = 0, int area = 0, int distance = 0, int size = 10)
        {
            var grades = new int[] { };
            var types = new int[] { };
            var Lodging = new int[] { };
            ApiInterest interest = await GetInterestAsync();
            if (interest != null)
            {
                grades = interest.interest.grade;
                types = interest.interest.nature;
                Lodging = interest.interest.lodging;
            }
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), province, city, area, grades, orderBy, types, distance, Lodging, index, size);
            list.PageSize = size;
            ViewBag.UA = UserAgentHelper.Check(Request);
            return PartialView(list);
        }

        /// <summary>
        /// 学校PK添加学校列表动态分页数据
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="tabType">添加学校列表类型（1推荐、2历史、3关注）</param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校PK添加学校列表动态分页数据")]
        public async Task<IActionResult> PKSchoolsHistoryAddPartial(int index = 1, int orderBy = 1, int province = 0, int city = 0, int area = 0, int distance = 0, int size = 10)
        {
            HistoryPKShoolsDto res = new HistoryPKShoolsDto();
            res.historySchoolList = new List<SchoolExtSimpleDto>();
            List<SchoolExtSimpleDto> historySchool = new List<SchoolExtSimpleDto>();
            string Key = "";
            Guid userId = default(Guid);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                Key = $"SchoolPK-1:{userId}";
            }
            else
            {
                userId = Request.GetDeviceToGuid();
                Key = $"SchoolPK-0:{userId}";
            }

            var historyPKSchool = await _easyRedisClient.SortedSetRangeByRankAsync<SchoolExtSimpleDto>(Key, index * 10, index * 10 + 10, StackExchange.Redis.Order.Descending);
            //var historyPKSchool = await _easyRedisClient.SortedSetRangeByRankAsync<SchoolExtSimpleDto>(Key, 0, 100, StackExchange.Redis.Order.Descending);
            historySchool.AddRange(historyPKSchool.ToList());
            res.historySchoolList = historySchool;
            res.PageSize = size;
            return PartialView(res);
        }

        /// <summary>
        /// 学校PK添加学校列表动态分页数据
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="tabType">添加学校列表类型（1推荐、2历史、3关注）</param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校PK添加学校列表动态分页数据")]
        public IActionResult PKSchoolsCollectAddPartial(int index = 1, int orderBy = 1, int province = 0, int city = 0, int area = 0, int distance = 0, int size = 10)
        {
            CollectPKSchoolsDto res = new CollectPKSchoolsDto();
            res.CollectSchoolList = new List<SchoolExtDto>();
            List<SchoolExtDto> CollectSchool = new List<SchoolExtDto>();
            if (CollectSchoolList.Count > index * 10)
                CollectSchool = CollectSchoolList.GetRange(index * 10, index * 10 + 10);
            res.CollectSchoolList = CollectSchool;
            res.PageSize = size;
            return PartialView(res);
        }


        /// <summary>
        /// 获取PK详情
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExtPKDetail(Guid fextId, Guid lextId, Guid fsid = default(Guid), Guid lsid = default(Guid), Guid fid = default(Guid), Guid lid = default(Guid))
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            var fdata = await _schoolService.GetSchoolExtDtoAsync(fextId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            //如果学校分部为null的话跳转到错误页面
            if (fdata == null)
                return new ExtNotFoundViewResult();

            var fresult = fdata;
            var ldata = await _schoolService.GetSchoolExtDtoAsync(lextId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            //如果学校分部为null的话跳转到错误页面
            if (ldata == null)
                return new ExtNotFoundViewResult();

            var lresult = ldata;

            ViewBag.fExtId = fresult.ExtId;
            ViewBag.fSid = fresult.Sid;
            ViewBag.lExtId = lresult.ExtId;
            ViewBag.lSid = lresult.Sid;
            //学部名称
            string fname = "-";
            if (fresult.SchoolName != null && fresult.ExtName != null)
            {
                fname = fresult.SchoolName + "-" + fresult.ExtName;
            }
            ViewBag.fName = fname;
            string lname = "-";
            if (lresult.SchoolName != null && lresult.ExtName != null)
            {
                lname = lresult.SchoolName + "-" + lresult.ExtName;
            }
            ViewBag.lName = lname;
            //学校类型
            ViewBag.fType = fresult.Type;
            ViewBag.lType = lresult.Type;
            ViewBag.fGrade = fresult.Grade;
            ViewBag.lGrade = lresult.Grade;
            //走读、寄宿
            ViewBag.fLodging = LodgingUtil.Reason(fresult.Lodging, fresult.Sdextern).Description();
            ViewBag.lLodging = LodgingUtil.Reason(lresult.Lodging, lresult.Sdextern).Description();
            //师生比例
            string fteaProportion = "-";
            if (fresult.Studentcount != null && fresult.Teachercount != null && fresult.Studentcount != 0 && fresult.Teachercount != 0)
            {
                var ftea = Math.Round(Convert.ToDecimal(fresult.Teachercount / fresult.Studentcount), 0);
                var fteaNumber = NumberHelper.ConvertToFraction(fresult.Teachercount.Value, fresult.Studentcount.Value);
                fteaProportion = $"{fteaNumber.Item1}:{fteaNumber.Item2}";
            }
            ViewBag.fTeaProportion = fteaProportion;
            ViewBag.fStudentcount = fresult.Studentcount;
            ViewBag.fTeachercount = fresult.Teachercount;
            string lteaProportion = "-";
            if (lresult.Studentcount != null && lresult.Teachercount != null && lresult.Studentcount != 0 && lresult.Teachercount != null)
            {
                var ltea = Math.Round(Convert.ToDecimal(lresult.Teachercount / lresult.Studentcount), 0);
                var lteaNumber = NumberHelper.ConvertToFraction(lresult.Teachercount.Value, lresult.Studentcount.Value);
                lteaProportion = $"{lteaNumber.Item1}:{lteaNumber.Item2}";
            }
            ViewBag.lTeaProportion = lteaProportion;
            ViewBag.lStudentcount = lresult.Studentcount;
            ViewBag.lTeachercount = lresult.Teachercount;
            //学校认证
            var fAuthentication = string.IsNullOrEmpty(fresult.Authentication) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(fresult.Authentication);
            ViewBag.fAuthentication = fAuthentication;
            var lAuthentication = string.IsNullOrEmpty(lresult.Authentication) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(lresult.Authentication);
            ViewBag.lAuthentication = lAuthentication;
            //学校特色课程
            var fCharacteristic = string.IsNullOrEmpty(fresult.Characteristic) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(fresult.Characteristic);
            ViewBag.fCharacteristic = fCharacteristic;
            var lCharacteristic = string.IsNullOrEmpty(lresult.Characteristic) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(lresult.Characteristic);
            ViewBag.lCharacteristic = lCharacteristic;
            //距离
            ViewBag.fDistance = fresult.Distance;
            ViewBag.lDistance = lresult.Distance;
            //有无饭堂
            ViewBag.fCanteen = fresult.Canteen;
            ViewBag.lCanteen = lresult.Canteen;
            //学费
            ViewBag.fTuition = fresult.Tuition;
            ViewBag.lTuition = lresult.Tuition;
            //学生人数
            ViewBag.fStudentcount = fresult.Studentcount;
            ViewBag.lStudentcount = lresult.Studentcount;
            //
            Guid userId = default(Guid);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }
            else
            {
                userId = Request.GetDeviceToGuid();
            }
            //学校点评
            var fschoolComment = _commentService.GetSchoolSelectedComment(fextId, userId);
            CommentList fcommentDetail = null;
            if (fschoolComment != null)
            {
                var fuserInfo = _userService.GetUserInfo(fschoolComment.UserId);
                var fuser = UserHelper.ToAnonyUserName(new Sxb.Web.Models.User.UserInfoVo() { HeadImager = fuserInfo.HeadImgUrl, Id = fuserInfo.Id, NickName = fuserInfo.NickName, Role = fuserInfo.VerifyTypes?.ToList() }, fschoolComment.IsAnony);
                fcommentDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto, CommentList>(fschoolComment);
                for (int i = 0; i < fcommentDetail.Images.Count(); i++)
                {
                    fcommentDetail.Images[i] = _setting.QueryImager + fcommentDetail.Images[i];
                }
                fcommentDetail.UserInfo = fuser;
            }
            ViewBag.fSelectedComment = fcommentDetail;

            var lschoolComment = _commentService.GetSchoolSelectedComment(lextId, userId);
            CommentList lcommentDetail = null;
            if (lschoolComment != null)
            {
                var luserInfo = _userService.GetUserInfo(lschoolComment.UserId);
                var luser = UserHelper.ToAnonyUserName(new Sxb.Web.Models.User.UserInfoVo() { HeadImager = luserInfo.HeadImgUrl, Id = luserInfo.Id, NickName = luserInfo.NickName, Role = luserInfo.VerifyTypes?.ToList() }, lschoolComment.IsAnony);
                lcommentDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto, CommentList>(lschoolComment);
                for (int i = 0; i < lcommentDetail.Images.Count(); i++)
                {
                    lcommentDetail.Images[i] = _setting.QueryImager + lcommentDetail.Images[i];
                }
                lcommentDetail.UserInfo = luser;
            }
            ViewBag.lSelectedComment = lcommentDetail;

            //学校分数统计
            //左边
            SchoolScore fscores = null;
            if(fscores == null)
            {
                fscores = _commentService.GetSchoolScoreBySchoolId(fextId);
            }
            ViewBag.fscores = fscores;
            
            //右边
            SchoolScore lscores = null;
            if (lscores == null)
            {
                lscores = _commentService.GetSchoolScoreBySchoolId(lextId);
            }
            ViewBag.lscores = lscores;


            //学校点评 =》点评数量
            List<SchoolSectionCommentOrQuestionTotal> fres = _commentService.GetTotalBySchoolSectionIds(new List<Guid> { fextId });
            List<SchoolSectionCommentOrQuestionTotal> lres = _commentService.GetTotalBySchoolSectionIds(new List<Guid> { lextId });
            ViewBag.fComments = fres.Count>0?fres.FirstOrDefault().Total:0;
            ViewBag.lComments = lres.Count>0?lres.FirstOrDefault().Total:0;

            List<Guid> fSchSectionIds = new List<Guid>();
            List<Guid> lSchSectionIds = new List<Guid>();
            foreach (var fitem in fres)
            {
                fSchSectionIds.Add(fitem.SchoolSectionId);
            }
            foreach (var litem in fres)
            {
                lSchSectionIds.Add(litem.SchoolSectionId);
            }
            List<SchoolCommentDto> fallComments = _commentService.AllSchoolSelectedComment(userId, fSchSectionIds, 1);
            List<SchoolCommentDto> lallComments = _commentService.AllSchoolSelectedComment(userId, lSchSectionIds, 1);
            //点评关注数
            int fcollCommentCount = 0; int lcollCommentCount = 0;
            foreach (var itm in fallComments)
            {
                if (_userService.IsCollection(itm.Id, 3))
                    fcollCommentCount++;
            }
            foreach (var itm in lallComments)
            {
                if (_userService.IsCollection(itm.Id, 3))
                    lcollCommentCount++;
            }
            ViewBag.fcollComments = fcollCommentCount;
            ViewBag.lcollComments = lcollCommentCount;

            //int flike = 0; int llike = 0;
            //int fStartTotal = 0; int lStartTotal = 0;
            //int fReplyCount = 0; int lReplyCount = 0;
            //foreach (var it in fallComments)
            //{
            //    flike += it.LikeCount;
            //    fStartTotal += it.StartTotal;
            //    fReplyCount += it.ReplyCount;
            //}
            //foreach (var it in lallComments)
            //{
            //    llike += it.LikeCount;
            //    lStartTotal += it.StartTotal;
            //    lReplyCount += it.ReplyCount;
            //}

            var fCommentData = _commentService.GetCommentDataByID(fextId);
            var lCommentData = _commentService.GetCommentDataByID(lextId);
            //点评点赞数
            ViewBag.fCommentLike = fCommentData==null ? 0 : fCommentData.LikeCount;
            ViewBag.lCommentLike = lCommentData == null ? 0 : lCommentData.LikeCount;
            //点评热度数（浏览数）暂时缺少统计数据
            ViewBag.fCommentViews = fCommentData == null ? 0 : fCommentData.CommentViewCount + fCommentData.CommentRepliyViewCount;
            ViewBag.lCommentViews = lCommentData == null ? 0 : lCommentData.CommentViewCount + lCommentData.CommentRepliyViewCount;
            //点评回复数
            ViewBag.fReplyCount = fCommentData == null ? 0 : fCommentData.ReplyCount;
            ViewBag.lReplyCount = lCommentData == null ? 0 : lCommentData.ReplyCount;

            //学校问答
            var fQuestionData = _questionInfoService.GetQuestionDataByID(fextId);
            var lQuestionData = _questionInfoService.GetQuestionDataByID(lextId);

            var fschoolQuestion = _questionInfoService.PushSchoolInfo(new List<Guid>() { fextId }, userId);
            
            ViewBag.fQuestionTotalCount = fQuestionData == null ? 0 : fQuestionData.QuestionCount; //总问题
            ViewBag.fQuestionLikeCount = fQuestionData == null ? 0 : fQuestionData.LikeCount; //点赞
            ViewBag.fQuestionAnswerCount = fQuestionData == null ? 0 : fQuestionData.ReplyCount; //问题回复
            ViewBag.fQuestionViews = fQuestionData == null ? 0 : fQuestionData.QuestionViewCount + fQuestionData.AnswerRepliyViewCount;  //学校问答热度（浏览数）

            var lschoolQuestion = _questionInfoService.PushSchoolInfo(new List<Guid>() { lextId }, userId);
            ViewBag.lQuestionTotalCount = lQuestionData == null ? 0 : lQuestionData.QuestionCount; //总问题
            ViewBag.lQuestionLikeCount = lQuestionData == null ? 0 : lQuestionData.LikeCount; //点赞
            ViewBag.lQuestionAnswerCount = lQuestionData == null ? 0 : lQuestionData.ReplyCount; //问题回复                                                     
            ViewBag.lQuestionViews = lQuestionData == null ? 0 : lQuestionData.QuestionViewCount + lQuestionData.AnswerRepliyViewCount; //学校问答热度（浏览数）

            int fcollQuestion = 0; int lcollQuestion = 0;
            foreach (var it in fschoolQuestion)
            {
                if (_userService.IsCollection(it.Id, 2))
                    fcollQuestion++;
            }
            foreach (var it in lschoolQuestion)
            {
                if (_userService.IsCollection(it.Id, 2))
                    lcollQuestion++;
            }
            //学校问答关注数
            ViewBag.fcollquestionCount = fcollQuestion;
            ViewBag.lcollquestionCount = lcollQuestion;

            //分数
            var fschoolScore = await GetSchoolExtScoreAsync(fextId);
            ViewBag.fScoreIndex = fschoolScore.index;
            ViewBag.fScore = fschoolScore.score;
            var lschoolScore = await GetSchoolExtScoreAsync(lextId);
            ViewBag.lScoreIndex = lschoolScore.index;
            ViewBag.lScore = lschoolScore.score;
            //学校排行
            var fRanks = _schoolRankService.GetRankInfoBySchID(fextId);
            var lRanks = _schoolRankService.GetRankInfoBySchID(lextId);

            var fpkdatas = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), fextId);
            //如果学校分部为null的话跳转到错误页面
            if (fpkdatas == null)
                return new ExtNotFoundViewResult();


            if (fRanks.ToList() != null && fRanks.ToList().Count > 3)
                fpkdatas.Ranks = fRanks.ToList().GetRange(0, 3);
            else
                fpkdatas.Ranks = fRanks.ToList();

            ViewBag.frank = fpkdatas;
            var lpkdatas = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), lextId);
            //如果学校分部为null的话跳转到错误页面
            if (lpkdatas == null)
                return new ExtNotFoundViewResult();


            if (lRanks.ToList() != null && lRanks.ToList().Count > 3)
                lpkdatas.Ranks = lRanks.ToList().GetRange(0, 3);
            else
                lpkdatas.Ranks = lRanks.ToList();

            ViewBag.lrank = lpkdatas;
            // 升学成绩内容
            object fAchData = null;
            if (fpkdatas.AchYear != null)
            {
                fAchData = _schoolService.GetAchData(fextId, fpkdatas.Grade, fpkdatas.Type, fpkdatas.AchYear.Value);
            }
            ViewBag.fAchData = fAchData;
            object lAchData = null;
            if (lpkdatas.AchYear != null)
            {
                lAchData = _schoolService.GetAchData(lextId, lpkdatas.Grade, lpkdatas.Type, lpkdatas.AchYear.Value);
            }
            ViewBag.lAchData = lAchData;
            object fAchievementData = null;
            if (fpkdatas.Achievement != null && fpkdatas.Achievement.Count > 0)
            {
                fAchievementData = fpkdatas.Achievement;
            }
            ViewBag.fAchievement = fAchievementData;
            object lAchievementData = null;
            if (fpkdatas.Achievement != null && fpkdatas.Achievement.Count > 0)
            {
                lAchievementData = lpkdatas.Achievement;
            }
            ViewBag.lAchievement = lAchievementData;
            var farticle = _articleService.GetComparisionArticles_PageVersion(new List<Guid>() { fextId });
            ViewBag.fTotal = farticle.total;

            ViewBag.fArticle = farticle.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                CommentCount = a.CommentCount,
                Covers = null,
                Digest = a.overview,
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount
            });

            var larticle = _articleService.GetComparisionArticles_PageVersion(new List<Guid>() { lextId });
            ViewBag.lTotal = larticle.total;

            ViewBag.lArticle = larticle.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                CommentCount = a.CommentCount,
                Covers = null,
                Digest = a.overview,
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount
            });

            //周边环境 
            var client = new MongoClient(_mongosetting.ConnectionString);
            //获取database
            var mydb = client.GetDatabase(_mongosetting.Database);
            //获取collection
            var collection = mydb.GetCollection<AmbientModel>("GDParams");


            AmbientModel fabModel = new AmbientModel();
            AmbientModel labModel = new AmbientModel();
            var collectiontest = mydb.GetCollection<BsonDocument>("GDParams");
            var filtertest  = Builders<BsonDocument>.Filter;
            var project = Builders<BsonDocument>.Projection;
            var fdoctest = collectiontest.Find(filtertest.Eq("eid", fextId.ToString().ToUpper())).Project(project.Exclude("poiinfos").Exclude("_id")).FirstOrDefault();
            var ldoctest = collectiontest.Find(filtertest.Eq("eid", lextId.ToString().ToUpper())).Project(project.Exclude("poiinfos").Exclude("_id")).FirstOrDefault();

            JObject fjsonObj = JObject.Parse(fdoctest.ToString());
            JObject ljsonObj = JObject.Parse(ldoctest.ToString());

            fabModel = fjsonObj.ToObject<AmbientModel>();
            labModel = ljsonObj.ToObject<AmbientModel>();

            //Fileter用于过滤
            ///var filter = Builders<AmbientModel>.Filter;
            //var fdoc = collection.Find(filter.Eq("eid", fextId.ToString().ToUpper())).FirstOrDefault();
            //var ldoc = collection.Find(filter.Eq("eid", lextId.ToString().ToUpper())).FirstOrDefault();
            AmbientScore fambient = new AmbientScore()
            {
                Eid = new Guid(fabModel.eid),
                Museum = (int)fabModel.museum,
                Metro = (int)fabModel.metro,
                Market = (int)fabModel.market,
                Library = (int)fabModel.library,
                ShoppingInfo = (int)fabModel.shoppinginfo,
                Police = (int)fabModel.police,
                BookMarket = (int)fabModel.bookmarket,
                River = (int)fabModel.river,
                Rubbish = (int)fabModel.rubbish,
                Hospital = (int)fabModel.hospital,
                Subway = (int)fabModel.subway,
                Buildingprice = (int)fabModel.buildingprice,
                Traininfo = (int)fabModel.traininfo,
                Poiinfo = (int)fabModel.poiinfo,
                Play = (int)fabModel.play
            };

            ViewBag.fAmbient = fambient;
            AmbientScore lambient = new AmbientScore()
            {
                Eid = new Guid(labModel.eid),
                Museum = (int)labModel.museum,
                Metro = (int)labModel.metro,
                Market = (int)labModel.market,
                Library = (int)labModel.library,
                ShoppingInfo = (int)labModel.shoppinginfo,
                Police = (int)labModel.police,
                BookMarket = (int)labModel.bookmarket,
                River = (int)labModel.river,
                Rubbish = (int)labModel.rubbish,
                Hospital = (int)labModel.hospital,
                Subway = (int)labModel.subway,
                Buildingprice = (int)labModel.buildingprice,
                Traininfo = (int)labModel.traininfo,
                Poiinfo = (int)labModel.poiinfo,
                Play = (int)labModel.play
            };
            ViewBag.lAmbient = lambient;

            //文章关注
            int fcollArticleCount = 0; int lcollArticleCount = 0;
            foreach (var itm in farticle.articles)
            {
                if (_userService.IsCollection(itm.id, 0))
                    fcollArticleCount++;
            }
            foreach (var itm in larticle.articles)
            {
                if (_userService.IsCollection(itm.id, 0))
                    lcollArticleCount++;
            }
            ViewBag.fcollArticle = fcollArticleCount;
            ViewBag.lcollArticle = lcollArticleCount;
            return View();
        }
        #endregion

        /// <summary>
        /// 学校总评PK
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExtScorePK(Guid fextId, Guid lextId, Guid fsid = default(Guid), Guid lsid = default(Guid))
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var fdata = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), fextId);
            //如果学校分部为null的话跳转到错误页面
            if (fdata == null)
                return new ExtNotFoundViewResult();
            var ldata = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), lextId);
            //如果学校分部为null的话跳转到错误页面
            if (ldata == null)
                return new ExtNotFoundViewResult();

            ViewBag.fExtId = fdata.ExtId;
            ViewBag.lExtId = ldata.ExtId;
            ViewBag.fSid = fdata.Sid;
            ViewBag.lSid = ldata.Sid;

            ViewBag.fSchoolName = fdata.SchoolName;
            ViewBag.lSchoolName = ldata.SchoolName;
            ViewBag.fExtName = fdata.ExtName;
            ViewBag.lExtName = ldata.ExtName;

            //分数
            var fschoolScore = await GetSchoolExtScoreAsync(fextId);
            ViewBag.fScoreIndex = fschoolScore.index;
            ViewBag.fScore = fschoolScore.score;
            var lschoolScore = await GetSchoolExtScoreAsync(lextId);
            ViewBag.lScoreIndex = lschoolScore.index;
            ViewBag.lScore = lschoolScore.score;

            return View();
        }

        /// <summary>
        /// 周边PK
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExtAmbientPK(Guid fextId, Guid lextId, Guid fsid = default(Guid), Guid lsid = default(Guid))
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var fdata = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), fextId);
            //如果学校分部为null的话跳转到错误页面
            if (fdata == null)
                return new ExtNotFoundViewResult();

            var ldata = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), lextId);
            //如果学校分部为null的话跳转到错误页面
            if (ldata == null)
                return new ExtNotFoundViewResult();

            ViewBag.fExtId = fdata.ExtId;
            ViewBag.lExtId = ldata.ExtId;
            ViewBag.fSid = fdata.Sid;
            ViewBag.lSid = ldata.Sid;

            ViewBag.fSchoolName = fdata.SchoolName;
            ViewBag.lSchoolName = ldata.SchoolName;
            ViewBag.fExtName = fdata.ExtName;
            ViewBag.lExtName = ldata.ExtName;

            //分数
            var fschoolScore = await GetSchoolExtScoreAsync(fextId);
            ViewBag.fScoreIndex = fschoolScore.index;
            ViewBag.fScore = fschoolScore.score;
            var lschoolScore = await GetSchoolExtScoreAsync(lextId);
            ViewBag.lScoreIndex = lschoolScore.index;
            ViewBag.lScore = lschoolScore.score;

            //var fambient = await _schoolService.GetAmbientScoreAsync(fextId);
            //ViewBag.fAmbient = fambient;
            //var lambient = await _schoolService.GetAmbientScoreAsync(lextId);
            //ViewBag.lAmbient = lambient;

            var client = new MongoClient(_mongosetting.ConnectionString);
            //获取database
            var mydb = client.GetDatabase(_mongosetting.Database);
            //获取collection
            var collection = mydb.GetCollection<BsonDocument>("GDParams");
            //Fileter用于过滤
            var filter = Builders<BsonDocument>.Filter;
            var project = Builders<BsonDocument>.Projection;
            var fdoc = collection.Find(filter.Eq("eid", fextId.ToString().ToUpper())).Project(project.Exclude("poiinfos").Exclude("_id")).FirstOrDefault();
            var ldoc = collection.Find(filter.Eq("eid", lextId.ToString().ToUpper())).Project(project.Exclude("poiinfos").Exclude("_id")).FirstOrDefault();

            JObject fjsonObj = JObject.Parse(fdoc.ToString());
            JObject ljsonObj = JObject.Parse(ldoc.ToString());

            AmbientModel fabModel = fjsonObj.ToObject<AmbientModel>();
            AmbientModel labModel = ljsonObj.ToObject<AmbientModel>();

            AmbientScore fambient = new AmbientScore()
            {
                Eid = new Guid(fabModel.eid),
                Museum = (int)fabModel.museum,
                Metro = (int)fabModel.metro,
                Market = (int)fabModel.market,
                Library = (int)fabModel.library,
                ShoppingInfo = (int)fabModel.shoppinginfo,
                Police = (int)fabModel.police,
                BookMarket = (int)fabModel.bookmarket,
                River = (int)fabModel.river,
                Rubbish = (int)fabModel.rubbish,
                Hospital = (int)fabModel.hospital,
                Subway = (int)fabModel.subway,
                Buildingprice = (int)fabModel.buildingprice,
                Traininfo = (int)fabModel.traininfo,
                Poiinfo = (int)fabModel.poiinfo,
                Play = (int)fabModel.play
            };
            ViewBag.fAmbient = fambient;
            AmbientScore lambient = new AmbientScore()
            {
                Eid = new Guid(labModel.eid),
                Museum = (int)labModel.museum,
                Metro = (int)labModel.metro,
                Market = (int)labModel.market,
                Library = (int)labModel.library,
                ShoppingInfo = (int)labModel.shoppinginfo,
                Police = (int)labModel.police,
                BookMarket = (int)labModel.bookmarket,
                River = (int)labModel.river,
                Rubbish = (int)labModel.rubbish,
                Hospital = (int)labModel.hospital,
                Subway = (int)labModel.subway,
                Buildingprice = (int)labModel.buildingprice,
                Traininfo = (int)labModel.traininfo,
                Poiinfo = (int)labModel.poiinfo,
                Play = (int)labModel.play
            };
            ViewBag.lAmbient = lambient;
            return View();
        }

        /// 获取用户的问卷
        /// 0走读  1寄宿
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [Description("获取用户的问卷")]
        private async Task<ApiInterest> GetInterestAsync()
        {
            ApiInterest interest = null;
            //http请求api
            var config = _configuration.GetSection("UserSystemConfig");
            var ip = config.GetSection("ServerUrl").Value;
            var uuid = Request.GetDevice(Guid.NewGuid().ToString());
            var url = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                interest = await _easyRedisClient.GetAsync<ApiInterest>($"interest-{user.UserId.ToString()}-{uuid}");
                url = $"{ip}/Info/GetUserInterest?userID={user.UserId.ToString()}&uuid={uuid}";
            }
            else
            {
                interest = await _easyRedisClient.GetAsync<ApiInterest>($"interest--{Request.GetDevice()}");
                url = $"{ip}/Info/GetUserInterest?uuid={uuid}";
            }
            if (interest == null)
            {
                //url请求
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var client = _clientFactory.CreateClient();
                try
                {
                    var response = await client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseResult = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonHelper.JSONToObject<ApiInterest>(responseResult);
                        if (jsonData.status == 0)
                        { return jsonData; }
                        else { return null; }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {

                    return null;
                }
            }
            return interest;
        }

        /// <summary>
        /// 文章列表项的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        string ArticleListItemTimeFormart(DateTime dateTime)
        {

            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");

            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }


        //public IActionResult Screeningschool()
        //{
        //    return View();
        //}

        //public IActionResult Schoolpkdetail()
        //{
        //    return View();
        //}

        //public IActionResult Comprehensive()
        //{
        //    return View();
        //}

    }
}