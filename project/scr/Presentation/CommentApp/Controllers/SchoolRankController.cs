using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    using AutoMapper;
    using iSchool.Internal.API.UserModule;
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.School.Application.IServices;
    using PMS.Search.Application.IServices;
    using PMS.Search.Application.ModelDto.Query;
    using ProductManagement.Framework.Foundation;
    using Sxb.Web.Common;
    using Sxb.Web.Response;
    using Sxb.Web.Utils;
    using System.ComponentModel;
    using ViewModels.SchoolRank;

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

        IMapper _mapper;

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
            , IMapper mapper)
        {
            this.schoolRankService = schoolRankService;
            this.schoolService = schoolService;
            this.article_SchoolTypeService = article_SchoolTypeService;
            this.schoolRankBindsService = schoolRankBindsService;
            this._dataSearch = _dataSearch;
            this.userApiServices = userApiServices;
            _mapper = mapper;
        }


        /// <summary>
        /// 列出所有的排行榜
        /// </summary>
        [Description("获取所有的排行榜")]
        [Route("{controller}/{action}")]
        [Route("{controller}/{gradePY}")]
        public IActionResult ListAll(SchoolGrade grade = SchoolGrade.Kindergarten, int offset = 0, int limit = 20, SchoolGradePY gradePY = SchoolGradePY.Unknow)
        {
            if (gradePY != SchoolGradePY.Unknow)
            {
                switch (gradePY)
                {
                    case SchoolGradePY.YouErYuan:
                        grade = SchoolGrade.Kindergarten;
                        break;
                    case SchoolGradePY.XiaoXue:
                        grade = SchoolGrade.PrimarySchool;
                        break;
                    case SchoolGradePY.ChuZhong:
                        grade = SchoolGrade.JuniorMiddleSchool;
                        break;
                    case SchoolGradePY.GaoZhong:
                        grade = SchoolGrade.SeniorMiddleSchool;
                        break;
                }
            }


            int locationCityCode = HttpContext.Request.GetLocalCity();


            if (Request.IsAjax())
            {
                var datas = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, (int)grade, offset, limit);
                var schoolRankDetails = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(datas.schoolRanks);
                return AjaxSuccess("OK", schoolRankDetails);
            }
            else
            {

                //默认页面加载四个类别的数据
                var KindergartenDatas = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, (int)SchoolGrade.Kindergarten, offset, limit);
                ViewBag.KindergartenDatas = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(KindergartenDatas.schoolRanks);

                var PrimarySchoolDatas = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, (int)SchoolGrade.PrimarySchool, offset, limit);
                ViewBag.PrimarySchoolDatas = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(PrimarySchoolDatas.schoolRanks);

                var JuniorMiddleSchoolDatas = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, (int)SchoolGrade.JuniorMiddleSchool, offset, limit);
                ViewBag.JuniorMiddleSchoolDatas = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(JuniorMiddleSchoolDatas.schoolRanks);

                var SeniorMiddleSchoolDatas = this.schoolRankService.GetSchoolRanksByGrades(locationCityCode, (int)SchoolGrade.SeniorMiddleSchool, offset, limit);
                ViewBag.SeniorMiddleSchoolDatas = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(SeniorMiddleSchoolDatas.schoolRanks);

                ViewBag.InitGrade = grade;
                ViewBag.InitOffset = offset;
                ViewBag.InitLimit = limit;


                return View();


            }




        }


        /// <summary>
        /// 列出某个学校所在的排行榜
        /// </summary>
        [Description("获取有某个学校的排行榜")]
        public async Task<IActionResult> List(Guid? schoolId)
        {

            int locationCityCode = HttpContext.Request.GetLocalCity();
            //获取学校上榜的榜单
            List<SchoolRank> schoolranks = new List<SchoolRank>();
            if (schoolId != null)
            {
                schoolranks = this.schoolRankService.GetSchoolRanks(schoolId.GetValueOrDefault(), 2).ToList();
            }
            //批量拿到需要的学校信息，因为层级引用问题，只能在UI层作数据拼接
            List<Guid> sids = new List<Guid>();
            schoolranks.ForEach(sr =>
            {
                sids.AddRange(sr.SchoolRankBinds.Select(srb => srb.SchoolId));
            });
            var sInfos = this.schoolService.ListExtSchoolByBranchIds(sids).Result;  //调用学校基层查询数据。
            var models = schoolranks.Select(sr =>
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
                        Sort = (int)srb.Sort
                    };

                    sItem.Name = sInfos.Where(s => s.ExtId == sItem.Id).FirstOrDefault()?.Name;
                    return sItem;

                }).ToList();
                return vm;

            });


            //查询推荐榜单
            //1.查询用户已经关联的学校类型
            var interestInfo = await this.userApiServices.GetUserInterestAsync(this.userId, Request.GetDevice());
            //问卷中记录了学校等级和学校类型
            var schoolTypes = this.article_SchoolTypeService.OrCombination(
                  interestInfo.grade
                 , interestInfo.nature).ToList();
            ViewBag.CorrelationSchoolRanks = _mapper.Map<IEnumerable<SchoolRankDetailViewModel>>(this.schoolRankService.GetRecommendSchoolRanks(schoolTypes, locationCityCode, 6));



            ViewBag.SchoolId = schoolId;

            return View(models);
        }




        [Description("获取排行榜详情")]
        [Route("schoolrank/detail/{id:Guid}")]
        [Route("schoolrank/detail/{base32No}.html")]
        public IActionResult Detail(Guid id, string base32No)
        {
            int? no = null;
            if (!string.IsNullOrEmpty(base32No))
            {
                no = (int)UrlShortIdUtil.Base322Long(base32No);
            }
            var schoolRank = this.schoolRankService.GetSchoolRank(id, no);
            if (schoolRank == null) { return NotFound(); }
            var binds = this.schoolRankBindsService.GetSchoolRankBinds(schoolRank);
            var sInfos = this.schoolService.ListExtSchoolByBranchIds(binds.Select(srb => srb.SchoolId).ToList()).Result;  //调用学校基层查询数据。
            SchoolRankViewModel model = new SchoolRankViewModel()
            {
                RankId = schoolRank.Id,
                RankName = schoolRank.Title,
                RankCoverUrl = schoolRank.Cover,
                Intro = schoolRank.Intro,
                DTSource = schoolRank.DTSource,
                Schools = binds.Select(srb =>
                {
                    var info = sInfos.Where(s => s.ExtId == srb.SchoolId).FirstOrDefault();
                    var schoolNo = info?.SchoolNo ?? 0;
                    var schoolInfo = new SchoolRankViewModel.SchoolInfo()
                    {
                        Id = srb.Id,
                        ExtId = srb.SchoolId,
                        Name = info?.Name ?? "[暂无该学校信息]",
                        Sort = (int)srb.Sort,
                        SchoolNo = schoolNo,
                        ShortSchoolNo = UrlShortIdUtil.Long2Base32(schoolNo),
                        Remark = srb.Remark,
                        Statu = info?.Status == PMS.School.Domain.Common.SchoolStatus.Success ? 1 : 0
                    };
                    return schoolInfo;
                }).ToList()
            };

            return View(model);
        }

        public ResponseResult GetRankCardInfo(Guid id)
        {
            var schoolRank = schoolRankService.GetSchoolRank(id, no: null);
            if (schoolRank == null) return ResponseResult.Failed("未找到卡片信息");

            return ResponseResult.Success(new SchoolRankCardViewModel()
            {
                Id = schoolRank.Id,
                Title = schoolRank.Title,
                Cover = schoolRank.Cover,
                No = UrlShortIdUtil.Long2Base32(schoolRank.No)
            });
        }

        [Description("排行榜搜索学校")]
        public IActionResult SearchSchools(string keyword, int limit)
        {
            int locationCityCode = HttpContext.Request.GetLocalCity();
            double latitude = NumberParse.DoubleParse(Request.Cookies["latitude"], 0);
            double longitude = NumberParse.DoubleParse(Request.Cookies["longitude"], 0);
            var list = _dataSearch.SearchSchool(new SearchSchoolQuery
            {
                Keyword = keyword,
                Latitude = latitude,
                Longitude = longitude,
                CityCode = new List<int> { locationCityCode },
                PageNo = 1,
                PageSize = limit
            });

            return Json(new { statu = 1, data = list });
        }
    }
}