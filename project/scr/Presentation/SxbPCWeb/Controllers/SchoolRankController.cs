using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;

namespace Sxb.PCWeb.Controllers
{
    using AutoMapper;
    using iSchool.Internal.API.UserModule;
    using PMS.Infrastructure.Application.IService;
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.School.Application.IServices;
    using PMS.Search.Application.IServices;
    using Sxb.PCWeb.Common;
    using Sxb.PCWeb.Utils;
    using Sxb.PCWeb.ViewModels.SchoolRank;
    using System.ComponentModel;

    /// <summary>
    /// 学校排行榜
    /// </summary>
    public class SchoolRankController : BaseController
    {


        ISchoolRankService schoolRankService;

        ISchoolService schoolService;

        IArticle_SchoolTypeService article_SchoolTypeService;

        ISchoolRankBindsService schoolRankBindsService;

        ISearchService _dataSearch;

        UserApiServices userApiServices;

        ILocalV2Services _localV2Services;

        ICityInfoService _cityService;

        IMapper _mapper;

        //IEasyRedisClient _redisClient;

        public SchoolRankController(
            ISchoolRankService schoolRankService
            ,
             ISchoolService schoolService
            ,
             IArticle_SchoolTypeService article_SchoolTypeService
            ,
             ISchoolRankBindsService schoolRankBindsService
             ,
             ISearchService _dataSearch
            ,
             UserApiServices userApiServices
            ,
              ILocalV2Services localV2Services
            ,
            ICityInfoService cityService
            
            , IMapper mapper
            // IEasyRedisClient redisClient
            )
        {
            this.schoolRankService = schoolRankService;
            this.schoolService = schoolService;
            this.article_SchoolTypeService = article_SchoolTypeService;
            this.schoolRankBindsService = schoolRankBindsService;
            this._dataSearch = _dataSearch;
            this.userApiServices = userApiServices;
            this._localV2Services = localV2Services;
            this._cityService = cityService;
            _mapper = mapper;
            //this._redisClient = redisClient;
        }

        /// <summary>
        /// 列出所有的排行榜
        /// </summary>
        [Description("获取所有的排行榜")]
        [Route("/{controller}/{action}")]
        [Route("/{controller}/{grade}")]
        [Route("/{controller}/{grade}/city_{city}/{page}.html")]
        public async Task<IActionResult> ListAll(int page=1, SchoolGradePY grade = SchoolGradePY.Unknow, int city = 0)
        {
            int limit = 16;
            int offset = (page - 1) * limit;
            //获取当前城市code
            int locationCityCode = city == 0 ? Request.GetLocalCity() : city;
            ViewBag.LocalCity = await _cityService.GetCityName(locationCityCode);
            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;
            //是否为Ajax请求
            if (Request.IsAjax())
            {
                //根据学校年级(幼儿园，小学，中学，高中)查询学校排行榜
                var datas = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, grade == SchoolGradePY.Unknow ? null : (int?)grade, offset, limit);
                return AjaxSuccess("OK", datas.schoolRanks);
            }
            else
            {
                var local = this._localV2Services.GetById(locationCityCode);

                //根据学校年级(幼儿园，小学，中学，高中)查询学校排行榜
                var ranks  = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, grade == SchoolGradePY.Unknow ? null : (int?)grade, offset, limit);
                ViewBag.CityName = local.name;
                ViewBag.RankDatas = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(ranks.schoolRanks);
                ViewBag.InitGrade = (SchoolGrade)(int?)grade;
                ViewBag.InitOffset = offset;
                ViewBag.InitLimit = limit;
                ViewBag.CurrentPage = page;
                ViewBag.PageCount = limit;
                ViewBag.TotalPage = (int)Math.Ceiling((double)ranks.total / limit);
                ViewBag.CurrrentGrade = (int)grade;
                ViewBag.CurrentCity = locationCityCode;
                ViewBag.GradeString = grade.GetDescription();
                return View();
            }
        }


        /// <summary>
        /// 列出某个学校所在的排行榜
        /// </summary>
        [Description("获取有某个学校的排行榜")]
        public async Task<IActionResult> List(Guid? schExtId, int city = 0)
        {
            //获取当前城市code
            int locationCityCode = city == 0 ? Request.GetLocalCity() : city;
            ViewBag.LocalCity = await _cityService.GetCityName(locationCityCode);
            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;
            //查询推荐榜单
            //1.查询用户已经关联的学校类型
            var interestInfo = await this.userApiServices.GetUserInterestAsync(this.userId, Request.GetDevice());
            //问卷中记录了学校等级和学校类型
            var schoolTypes = this.article_SchoolTypeService.OrCombination(
                  interestInfo.grade
                 , interestInfo.nature).ToList();
            if (schExtId != null)
            {
                var targets = this.schoolRankService.GetSchoolRanks(schExtId.GetValueOrDefault(), 8);
                if (targets.Any())
                {
                    //批量拿到需要的学校信息，因为层级引用问题，只能在UI层作数据拼接
                    List<Guid> sids = new List<Guid>();
                    foreach (var sr in targets)
                    {
                        sids.AddRange(sr.SchoolRankBinds.Select(srb => srb.SchoolId));

                    }
                    var sInfos = this.schoolService.ListExtSchoolByBranchIds(sids).Result;  //调用学校基层查询数据。
                    var models = targets.Select(sr =>
                    {
                        var vm = new SchoolRankViewModel()
                        {
                            RankName = sr.Title,
                            RankCoverUrl = sr.Cover,
                            RankId = sr.Id,
                            No = sr.No

                        };
                        vm.Schools = sr.SchoolRankBinds.OrderBy(srb => srb.Sort).Select(srb =>
                        {

                            var sItem = new SchoolRankViewModel.SchoolInfo()
                            {
                                Id = srb.SchoolId,
                                Sort = (int)srb.Sort,
                                ExtId = srb.SchoolId,
                            };
                            sItem.Name = sInfos.Where(s => s.ExtId == sItem.Id).FirstOrDefault()?.Name;
                            return sItem;

                        }).ToList();
                        return vm;

                    });
                    ViewBag.Targets = models;
                    ViewBag.TargetId = schExtId;
                }
                else {
                    ViewBag.Targets = new List<SchoolRankViewModel>();
                }
            }



            //获取推荐榜单
            var correlationSchoolRanks =this.schoolRankService.GetRecommendSchoolRanks(schoolTypes, locationCityCode, 3);
            var schoolRankViewModels = correlationSchoolRanks.Select(sr =>
            {
                var schoolrankDetail = _mapper.Map<SchoolRankDetailViewModel>(sr);
                SchoolRankViewModel schoolRankViewModel = new SchoolRankViewModel();
                schoolRankViewModel.RankId = sr.Id;
                schoolRankViewModel.RankName = sr.Title;
                schoolRankViewModel.RankCoverUrl = sr.Cover;
                schoolRankViewModel.No = schoolrankDetail.No;
                var srbs = this.schoolRankBindsService.GetSchoolRankBinds_PageVersion(sr, 0, 9).Item1.ToList();
                var sInfos = this.schoolService.ListExtSchoolByBranchIds(srbs.Select(srb => srb.SchoolId).ToList()).Result;  //调用学校基层查询数据。
                schoolRankViewModel.Schools = sInfos.Select(sInfo =>
                {
                    var srb = srbs.Where(sb => sb.SchoolId == sInfo.ExtId).First();
                    return new SchoolRankViewModel.SchoolInfo()
                    {
                        Id = sInfo.Sid,
                        Name = sInfo.Name,
                        ExtId = sInfo.ExtId,
                        Sort = (int)srb.Sort,
                        Statu = sInfo.Status == PMS.School.Domain.Common.SchoolStatus.Success ? 1 : 0,
                        SchoolNo = sInfo.SchoolNo
                    };

                }).OrderBy(s => s.Sort).ToList();
                return schoolRankViewModel;
            });



            //获取更多榜单
            var moreRanks = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, null, 0, 4);




            ViewBag.Recommeds = schoolRankViewModels;
            ViewBag.MoreRanks = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(moreRanks.schoolRanks);
            return View();
        }

        [Description("获取排行榜详情")]
        [Route("/{controller}/{action}/{id:Guid}/{pageIndex=1}")]
        [Route("/{controller}/{action}/{base32No}.html/{pageIndex=1}")]
        public async Task<IActionResult> Detail(Guid id, string base32No, Guid? schExtId, int pageIndex = 1,int pageCount = 30)
        {
            int? no = null;
            int offset = 0, limit = pageCount;
            if (!string.IsNullOrEmpty(base32No))
            {
                //解析no
                no = (int)UrlShortIdUtil.Base322Long(base32No);
            }
            var schoolRank = this.schoolRankService.GetSchoolRank(id, no);
            if (schoolRank == null) { return new CustomErrorViewResult(404); }


            if (pageIndex > 0)
            {
                offset = (pageIndex - 1) * limit;
            }

            //由于业务原因，采用内存分页的方式：
            var all = this.schoolRankBindsService.GetSchoolRankBinds(schoolRank).ToList();
            if (schExtId != null)
            {
                int target_index = 0;   //找出目标的下标

                for (; target_index < all.Count(); target_index++)
                {
                    if (all[target_index].SchoolId == schExtId)
                        break;
                    if (target_index == all.Count - 1)
                    {
                        target_index = -1;
                        break;
                    }
                }
                if (target_index >= 0)
                {
                    int sortNum = target_index + 1;
                    int page = (int)Math.Ceiling(sortNum / (double)limit);
                    offset = (page - 1) * limit;    //调整offset
                }
            }

            var page_data = all.Skip(offset).Take(limit);
            var sInfos = await this.schoolService.ListExtSchoolByBranchIds(page_data.Select(srb => srb.SchoolId).ToList());  //调用学校基层查询数据。
            SchoolRankViewModel model = new SchoolRankViewModel()
            {
                RankId = schoolRank.Id,
                RankName = schoolRank.Title,
                RankCoverUrl = schoolRank.Cover,
                DTSource = schoolRank.DTSource,
                Intro = schoolRank.Intro,
                No = schoolRank.No,
                Schools = page_data.Select(srb =>
                {
                    var sinfo = sInfos.Where(info => info.ExtId == srb.SchoolId).FirstOrDefault();
                    return new SchoolRankViewModel.SchoolInfo()
                    {
                        Id = sinfo?.Sid ?? default,
                        Name = sinfo?.Name ?? "[暂无该学校信息]",
                        ExtId = srb.SchoolId,
                        Sort = (int)srb.Sort,
                        Statu = sinfo?.Status == PMS.School.Domain.Common.SchoolStatus.Success ? 1 : 0,
                        Remark = srb.Remark,
                        SchoolNo = sinfo.SchoolNo
                    };

                }).OrderBy(s => s.Sort).ToList(),
            };


            int locationCityCode = HttpContext.Request.GetLocalCity();
            var local = this._localV2Services.GetById(locationCityCode);
            ViewBag.CityName = local.name;

            var total = all.Count;
            ViewBag.SchExtId = schExtId;
            ViewBag.CurrentPage = pageIndex;
            ViewBag.PageCount = pageCount;
            ViewBag.TotalPage = (int)Math.Ceiling((double)total / limit);
            return View(model);
        }
    }
}