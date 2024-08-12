using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Application.IServices;
using PMS.School.Domain.Entities;
using PMS.Search.Application.IServices;
using PMS.Search.Domain.Entities;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.Utils.CommentHelper;
using Sxb.PCWeb.ViewModels.CourseType;
using Sxb.PCWeb.ViewModels.School;
using Sxb.PCWeb.ViewModels.Sign;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.PCWeb.Controllers
{
    public class SignController : Controller
    {
        private ILSRFLeveInfoService _leveInfoService;

        private readonly IMapper _mapper;

        private ICourseTypeService _courseTypeService;

        private ILSRFSchoolDetailService _schoolDetailService;
        private readonly ISchoolService _schoolService;
        private readonly ISearchService _dataSearch;

        private IImportService _importService;
        private ICityInfoService _cityService;
        IInternationalSchoolService _internationalSchoolService;

        public SignController(ILSRFLeveInfoService leveInfoService, ICourseTypeService courseTypeService,
            IMapper mapper, ILSRFSchoolDetailService schoolDetailService, IImportService importService,
            ICityInfoService cityService, ISearchService dataSearch, ISchoolService schoolService, IInternationalSchoolService internationalSchoolService)
        {
            _internationalSchoolService = internationalSchoolService;
            _leveInfoService = leveInfoService;
            _mapper = mapper;
            _courseTypeService = courseTypeService;
            _schoolDetailService = schoolDetailService;
            _dataSearch = dataSearch;
            _importService = importService;
            _cityService = cityService;
            _schoolService = schoolService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index(PMS.OperationPlateform.Domain.Enums.CourseType CourseType, Guid Id, string Name = "", int PageNo = 1, int PageSize = 6)
        {
            //_importService.ImportSingSchoolData(new List<PMS.Search.Domain.Entities.SearchSignSchool>() {
            //      new PMS.Search.Domain.Entities.SearchSignSchool(){Id = Guid.Parse("8DA49B3E-80D2-4E92-BBBC-2CABB778A713"),SchName="沈阳外国语学校A-level中心 - 国际高中部",Type = 3,IsDel = false},
            //      new PMS.Search.Domain.Entities.SearchSignSchool(){ Id = Guid.Parse("F45CAA42-44A6-4D1C-886E-0B5A71CFDC35"),SchName="北京中关村国际学校 - 国际高中部",Type = 1,IsDel = false},
            //      new PMS.Search.Domain.Entities.SearchSignSchool(){ Id = Guid.Parse("8B92F6C7-FBE7-44BA-8144-DD2A4F5D4300"),SchName="西安沣东中加学校 - 国际高中部",Type = 1,IsDel = false},
            //      new PMS.Search.Domain.Entities.SearchSignSchool(){ Id = Guid.Parse("937BE7E0-3D47-499A-B1E1-3BD1B791B94D"),SchName="天津英华国际学校 - 国际高中部",Type = 1,IsDel = false},
            //      new PMS.Search.Domain.Entities.SearchSignSchool(){ Id = Guid.Parse("B6199350-743F-45AC-8BF4-4AFD0935E67C"), SchName = "广外增城外国语学校 - 国际高中部",Type = 1,IsDel = false}
            //});

            //获取当前的城市
            var localCityCode = Request.GetLocalCity();
            if (localCityCode % 1000 == 0)//容错
            {
                var cityCodes = _cityService.GetCityCodes(localCityCode);
                if (cityCodes.Any())
                {
                    localCityCode = cityCodes[0].Id;
                }
                else
                {
                    localCityCode = 440100;
                }
                Response.SetLocalCity(localCityCode.ToString());
            }
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);


            //获取去重数据
            var dis = _schoolDetailService.SchDistinct((int)CourseType);


            //获取当前广告位总条数
            int adveTotal = int.Parse(_schoolDetailService.GetCourseTypeAdveTotal(CourseType).Value);

            //当存在文本检索时 则在全部中检索
            if (!string.IsNullOrEmpty(Name))
            {
                CourseType = PMS.OperationPlateform.Domain.Enums.CourseType.All;
            }

            List<Guid> AdvSchIds = dis.Where(x => x.Type == PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Advertise).Select(x => x.Eid)?.ToList();

            //获取总条数
            decimal Total = decimal.Parse(_schoolDetailService.GetCurrentCourseTotal(CourseType, new List<Guid>()).Value);
            Total += adveTotal;

            List<LSRFSchoolDetail> SchDetails = new List<LSRFSchoolDetail>();

            //if (!string.IsNullOrEmpty(Name))
            //{
            //根据关键字查询学校信息，定位至对应页

            //调用ES获取匹配到的学校ID
            //var searchSch = _dataSearch.SearchSingSchool(SName, out long total);

            //if (total != 0) 
            //{
            //    var SchExtId = searchSch.Select(x => x.Id).ToList();

            //    //反查得到该学校所在的对应行
            //    var Rank =  _schoolDetailService.GetSchoolRankByEid(SchExtId);

            //    //得到课程类型位全部的广告位总数值
            //    string Value = _schoolDetailService.GetCourseTypeAdveTotal(PMS.OperationPlateform.Domain.Enums.CourseType.All).Value;

            //    //得到该
            //    int AdveTotal = Value == "" ? 0 : int.Parse(Value);

            //    List<SignSearchSch> searchSches = new List<SignSearchSch>();

            //    //匹配对应的所在页码
            //    searchSch.ForEach(x => {
            //        SignSearchSch signSearch = new SignSearchSch();
            //        signSearch.SName = x.SchName;
            //        signSearch.ExtId = x.Id;

            //        //得到该页所在的行
            //        int currentRank = (int)Rank.Where(s => s.SchId == x.Id).FirstOrDefault().Rank;

            //        //得到该学校所在的页码
            //        signSearch.PageNo = currentRank % PageSize == 0 ? currentRank / PageSize : currentRank / PageSize + 1;


            //        searchSches.Add(signSearch);
            //    });

            //    ViewBag.SearchSches = searchSches;

            //    //带有学校搜索后自动切换至全部课程类型
            //    CourseType = 0;
            //}
            //}
            //else
            //{
            ViewBag.SearchSches = new List<SignSearchSch>();

            //获取广告数据总分页数据

            int advePageTotal = (int)Math.Ceiling(adveTotal / (PageSize * 1.0));

            if (advePageTotal >= PageNo)
            {
                //检测当前也是否还在拿广告数据
                SchDetails = _schoolDetailService.GetAdvertisementSchool((PMS.OperationPlateform.Domain.Enums.CourseType)CourseType, PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Advertise, new List<Guid>(), PageNo, PageSize);
            }

            //每页展示6条学校数据
            int Take = 0;

            //减去广告位的条目数【目前制定一个课程类型只存在一条广告数据】
            Take = (PageSize - SchDetails.Count());

            int tempPageNo = PageNo;

            //取广告之后的普通数据
            if (PageNo > advePageTotal || (PageNo == advePageTotal && Take > 0))
            {
                PageNo = ((PageNo - 1) * PageSize) - adveTotal;
                if (PageNo <= 0)
                {
                    PageNo = 1;
                }
                //PageNo = PageNo - advePageTotal;
            }

            //获取该课程类型下的学校数据
            var school = _schoolDetailService.GetLSRFSchools(CourseType, AdvSchIds, PageNo, Take);

            if (school.Any())
            {
                SchDetails.AddRange(school);
            }

            //数据不足6条 获取推荐数据补充
            if (SchDetails.Count() < 6)
            {
                int tempTake = (PageSize - SchDetails.Count());

                List<Guid> RecommSchIds = dis.Select(x => x.Eid)?.ToList();

                var recommend = _schoolDetailService.GetAdvertisementSchool(CourseType, PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Recommend, RecommSchIds, 1, tempTake);

                if (recommend.Any())
                {
                    SchDetails.AddRange(recommend);
                }
            }

            //还原pageNo原有值
            PageNo = tempPageNo;
            //}

            ViewBag.TotalPage = Total;

            //获取课程类型标签
            var courseType = _courseTypeService.GetCourses();
            ViewBag.CourseType = _mapper.Map<List<CourseType>, List<CourseTypeVo>>(courseType);

            //领域实体To View层实体
            ViewBag.SchViewModel = LSRFSchoolDetailToVoHelper.LSRFSchoolDetailHelper(SchDetails);

            //当前选中的课程设置类型
            ViewBag.ActiveCourseType = (int)CourseType;

            //搜索栏热词学校相关搜索
            ViewBag.HotSchoolWord = _dataSearch.GetHotHistoryList(0, 3, null, localCityCode);

            ViewBag.Name = Name;
            ViewBag.Id = Id;

            ViewBag.PageIndex = PageNo;
            ViewBag.PageSize = PageSize;
            return View();
        }

        [Route("/international/{id}.html")]
        public async Task<IActionResult> International(int id, string name = "")
        {
            var paging = await _internationalSchoolService.GetSelectedReading(id);
            ViewBag.SelectedReading = paging;
            var schoolInfos = await _internationalSchoolService.GetSchoolByPageID(paging.PageID);
            //获取当前的城市
            var localCityCode = Request.GetLocalCity();
            if (localCityCode % 1000 == 0)//容错
            {
                var cityCodes = _cityService.GetCityCodes(localCityCode);
                if (cityCodes.Any())
                {
                    localCityCode = cityCodes[0].Id;
                }
                else
                {
                    localCityCode = 440100;
                }
                Response.SetLocalCity(localCityCode.ToString());
            }
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);

            Guid? userID = null;
            if (User.Identity.IsAuthenticated) userID = User.Identity.GetId();

            //搜索栏热词学校相关搜索
            ViewBag.HotSchoolWord = _dataSearch.GetHistoryList(userID, null, 0, 10);

            ViewBag.Name = name;
            ViewBag.Id = id;
            return View(schoolInfos);
        }

        /// <summary>
        /// 国际学校留资页 学校数据导入es
        /// </summary>
        /// <param name="imports"></param>
        [HttpPost]
        public async Task<ResponseResult> ImportSingSchoolData([FromBody] List<ImportSchool> imports)
        {
            if (imports.Any())
            {
                var ids = imports.Select(q => q.Id).ToList();
                var extSchool = await _schoolService.ListExtSchoolByBranchIds(ids);

                var data = imports.Select(x => new PMS.Search.Domain.Entities.SearchSignSchool() { Id = x.Id, SchName = extSchool.FirstOrDefault(q => q.ExtId == x.Id)?.Name }).ToList();
                _importService.ImportSingSchoolData(data);
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 修改学校状态
        /// </summary>
        /// <param name="imports"></param>
        [HttpPost]
        public ResponseResult UpdateSignSchool([FromBody] List<ImportSchool> imports)
        {
            if (imports.Any())
            {
                var data = imports.Select(x => new PMS.Search.Domain.Entities.SearchSignSchool() { Id = x.Id, IsDel = x.IsDel }).ToList();
                _importService.UpdateSignSchoolData(data);
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 创建sing index的mapping
        /// </summary>
        public void CreateSignSchoolIndex()
        {
            _importService.CreateSignSchool();
        }

        /// <summary>
        /// 根据关键字查询学校信息，定位至对应页
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ResponseResult SearchSchool(string keyword)
        {
            List<SignSearchSch> searchSches = new List<SignSearchSch>();

            if (!string.IsNullOrEmpty(keyword))
            {
                //调用ES获取匹配到的学校ID
                var searchSch = _dataSearch.SearchSingSchool(keyword, out long total);
                if (total != 0)
                {
                    //获取当前广告位总条数
                    int adveTotal = int.Parse(_schoolDetailService.GetCourseTypeAdveTotal(0).Value);
                    //获取总条数
                    decimal Total = decimal.Parse(_schoolDetailService.GetCurrentCourseTotal(0, new List<Guid>()).Value);
                    Total += adveTotal;

                    int PageTotal = (int)Math.Ceiling(Total / 6);

                    var SchExtId = searchSch.Select(x => x.Id).ToList();

                    //反查得到该学校所在的对应行
                    var Rank = _schoolDetailService.GetSchoolRankByEid(SchExtId);

                    //得到课程类型位全部的广告位总数值
                    string Value = _schoolDetailService.GetCourseTypeAdveTotal(PMS.OperationPlateform.Domain.Enums.CourseType.All).Value;

                    //得到该
                    int AdveTotal = Value == "" ? 0 : int.Parse(Value);

                    //匹配对应的所在页码
                    searchSch.ForEach(x =>
                    {
                        SignSearchSch signSearch = new SignSearchSch();
                        signSearch.SName = x.SchName;
                        signSearch.ExtId = x.Id;

                        var randata = Rank.Where(s => s.SchId == x.Id).FirstOrDefault();
                        //得到该页所在的行
                        int currentRank = randata == null ? 0 : (int)randata.Rank;

                        //得到该学校所在的页码
                        int tempPage = currentRank % 6 == 0 ? currentRank / 6 : currentRank / 6 + 1;
                        signSearch.PageNo = tempPage < 1 ? 1 : tempPage > PageTotal ? PageTotal : tempPage;

                        searchSches.Add(signSearch);
                    });
                }
            }

            return ResponseResult.Success(new
            {
                rows = searchSches
            });
        }

        //public JsonResult test(int CourseType = 0, string SName = "", int PageNo = 1, int PageSize = 6) 
        //{
        //    List<LSRFSchoolDetail> SchDetails = new List<LSRFSchoolDetail>();


        //    //如果当前访问的是第一页则展示广告位数据
        //    if (PageNo == 1)
        //    {
        //        SchDetails = _schoolDetailService.GetAdvertisementSchool((PMS.OperationPlateform.Domain.Enums.CourseType)CourseType);
        //    }

        //    //每页展示6条学校数据
        //    int Take = 0;

        //    //减去广告位的条目数【目前制定一个课程类型只存在一条广告数据】
        //    Take = (PageSize - SchDetails.Count());




        //    if (PageNo > 1) 
        //    {

        //    }

        //    //获取该课程类型下的学校数据
        //    var school = _schoolDetailService.GetLSRFSchools((PMS.OperationPlateform.Domain.Enums.CourseType)CourseType, PageNo, Take);

        //    if (school.Any())
        //    {
        //        SchDetails.AddRange(school);
        //    }



        //    //获取课程类型标签
        //    var courseType = _courseTypeService.GetCourses();
        //    ViewBag.CourseType = _mapper.Map<List<CourseType>, List<CourseTypeVo>>(courseType);

        //    //领域实体To View层实体
        //    var SchViewModel = LSRFSchoolDetailToVoHelper.LSRFSchoolDetailHelper(SchDetails);

        //    return Json(new { rows = SchViewModel,pageTotal = ViewBag.PageTotal });
        //}

        /// <summary>
        /// 提交留资信息
        /// </summary>
        /// <param name="leveInfoVo"></param>
        /// <returns></returns>
        public ResponseResult SubmitSign(LSRFLeveInfoVo leveInfoVo)
        {
            bool isSuccess = false;
            string message = "";

            if (leveInfoVo.Phone != null && leveInfoVo.FullName != null)
            {
                if (Regex.IsMatch(leveInfoVo.Phone, @"^1[3456789]\d{9}$"))
                {
                    leveInfoVo.City = Request.GetLocalCity();
                    leveInfoVo.Area = Request.GetArea();

                    if (User.Identity.IsAuthenticated)
                    {
                        leveInfoVo.UserId = User.Identity.GetId();
                    }

                    var model = _mapper.Map<LSRFLeveInfoVo, LSRFLeveInfo>(leveInfoVo);
                    bool status = _leveInfoService.Insert(model);

                    if (status)
                    {
                        isSuccess = true;
                        message = "您已报名成功";
                    }
                    else
                    {
                        message = "抱歉，信息提交失败<br />请尝试再次提交";
                    }
                }
                else
                {
                    message = "联系方式不是一个有效的号码";
                }
            }
            else
            {
                message = "联系方式或称呼填写不允许为空";
            }

            return ResponseResult.Success(new
            {
                isSuccess,
                message
            });
        }


    }
}
