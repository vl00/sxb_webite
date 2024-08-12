using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Application.IServices;
using PMS.School.Domain.Entities;
using PMS.Search.Application.IServices;
using ProductManagement.Framework.Foundation;
using Sxb.Web;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels;
using Sxb.Web.ViewModels.Sign;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.PCWeb.Controllers
{
    [OpenApiIgnore]
    public class SignController : BaseController
    {
        private ILSRFLeveInfoService _leveInfoService;

        private readonly IMapper _mapper;

        private ICourseTypeService _courseTypeService;

        private ILSRFSchoolDetailService _schoolDetailService;

        private readonly ISearchService _dataSearch;

        private readonly IAdvertisingOptionService _advertisingOptionService;

        private readonly IAdvertisingBaseService _advertisingBaseService;
        IInternationalSchoolService _internationalSchoolService;

        public SignController(ILSRFLeveInfoService leveInfoService, ICourseTypeService courseTypeService,
            IMapper mapper, ILSRFSchoolDetailService schoolDetailService,
            ISearchService dataSearch, IAdvertisingOptionService advertisingOptionService, IAdvertisingBaseService advertisingBaseService, IInternationalSchoolService internationalSchoolService)
        {
            _internationalSchoolService = internationalSchoolService;
            _leveInfoService = leveInfoService;
            _mapper = mapper;
            _courseTypeService = courseTypeService;
            _schoolDetailService = schoolDetailService;
            _dataSearch = dataSearch;
            _advertisingOptionService = advertisingOptionService;
            _advertisingBaseService = advertisingBaseService;
        }


        // GET: /<controller>/
        public IActionResult Index(PMS.OperationPlateform.Domain.Enums.CourseType CourseType, int PageNo = 1, int PageSize = 6)
        {
            //获取去重数据
            var dis = _schoolDetailService.SchDistinct((int)CourseType);

            List<Guid> AdvSchIds = dis.Where(x => x.Type == PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Advertise).Select(x => x.Eid)?.ToList();

            var cityId = Request.GetAvaliableCity();
            var advers = this._advertisingBaseService.GetAdvertising(9, cityId).Select(s =>
            {
                Adver adv = new Adver();
                adv.PicUrl = s.PicUrl;
                adv.Title = s.SloGan;
                var clientType = Request.GetClientType();
                if (clientType == UA.Mobile)
                {
                    adv.Url = $"ischool://web?url={HttpUtility.UrlEncode(s.Url)}";
                }
                else
                {
                    adv.Url = s.Url;
                }
                return adv;
            }).ToList();
            ViewBag.Advs = advers;

            //获取当前广告位总条数
            int adveTotal = int.Parse(_schoolDetailService.GetCourseTypeAdveTotal(CourseType).Value);

            //获取总条数
            decimal Total = decimal.Parse(_schoolDetailService.GetCurrentCourseTotal(CourseType, new List<Guid>()).Value);
            Total += adveTotal;

            List<LSRFSchoolDetail> SchDetails = new List<LSRFSchoolDetail>();

            List<LSRFSchoolDetail> Recommend = new List<LSRFSchoolDetail>();

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

            List<LSRFSchoolDetail> temp = new List<LSRFSchoolDetail>();
            if (Take > 0)
            {
                //获取该课程类型下的学校数据
                temp = _schoolDetailService.GetLSRFSchools(CourseType, AdvSchIds, PageNo, Take);
            }

            if (temp.Any())
            {
                SchDetails.AddRange(temp);
            }

            //数据不足6条 获取推荐数据补充
            if (PageNo == 1 && SchDetails.Count() < 6)
            {
                int tempTake = (PageSize - SchDetails.Count());
                List<Guid> RecommSchIds = dis.Select(x => x.Eid)?.ToList();


                var recommend = _schoolDetailService.GetAdvertisementSchool(CourseType, PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Recommend, RecommSchIds, 1, tempTake);

                if (recommend.Any())
                {
                    Recommend.AddRange(recommend);
                }
            }

            //还原pageNo原有值
            PageNo = tempPageNo;
            //}

            ViewBag.TotalPage = Total;

            //获取课程类型标签
            var courseType = _courseTypeService.GetCourses();
            ViewBag.CourseType = _mapper.Map<List<CourseType>, List<CourseTypeVo>>(courseType);

            //领域实体To View层实体 搜索栏数据
            ViewBag.SchViewModel = LSRFSchoolDetailToVoHelper.LSRFSchoolDetailHelper(SchDetails);

            //推荐学校
            ViewBag.Recommend = LSRFSchoolDetailToVoHelper.LSRFSchoolDetailHelper(Recommend);

            //当前选中的课程设置类型
            ViewBag.ActiveCourseType = (int)CourseType;

            //搜索栏热词学校相关搜索
            ViewBag.HotSchoolWord = _dataSearch.GetHotHistoryList(0, 3);

            ViewBag.PageIndex = PageNo;
            ViewBag.PageSize = PageSize;
            return View();
        }

        [Route("/international/{id}.html")]
        public async Task<IActionResult> International(int id)
        {
            var cityId = Request.GetAvaliableCity();
            var advers = this._advertisingBaseService.GetAdvertising(9, cityId).Select(s =>
            {
                Adver adv = new Adver();
                adv.PicUrl = s.PicUrl;
                adv.Title = s.SloGan;
                var clientType = Request.GetClientType();
                if (clientType == UA.Mobile)
                {
                    adv.Url = $"ischool://web?url={HttpUtility.UrlEncode(s.Url)}";
                }
                else
                {
                    adv.Url = s.Url;
                }
                return adv;
            }).ToList();
            ViewBag.Advs = advers;

            var paging = await _internationalSchoolService.GetSelectedReading(id);
            ViewBag.SelectedReading = paging;
            var schoolInfos = await _internationalSchoolService.GetSchoolByPageID(paging.PageID);
            if (!string.IsNullOrWhiteSpace(paging?.Title)) ViewBag.ShortTitle = paging.Title.GetShortString(15);
            return View(schoolInfos);
        }

        /// <summary>
        /// 加载更多数据
        /// </summary>
        /// <param name="CourseType"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ResponseResult Load(PMS.OperationPlateform.Domain.Enums.CourseType CourseType, int PageNo = 1, int PageSize = 6)
        {
            //获取去重数据
            var dis = _schoolDetailService.SchDistinct((int)CourseType);

            int TempNo = PageNo;

            //获取总条数
            decimal Total = decimal.Parse(_schoolDetailService.GetCurrentCourseTotal(CourseType, new List<Guid>()).Value);

            int adveTotal = int.Parse(_schoolDetailService.GetCourseTypeAdveTotal(CourseType).Value);

            Total += adveTotal;


            //获取广告数据总分页数据

            int advePageTotal = (int)Math.Ceiling(adveTotal / (PageSize * 1.0));

            List<LSRFSchoolDetail> SchDetails = new List<LSRFSchoolDetail>();

            List<LSRFSchoolDetail> Recommend = new List<LSRFSchoolDetail>();

            if (advePageTotal >= PageNo)
            {
                SchDetails = _schoolDetailService.GetAdvertisementSchool((PMS.OperationPlateform.Domain.Enums.CourseType)CourseType, PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Advertise, new List<Guid>(), PageNo, PageSize);
            }

            //每页展示6条学校数据
            int Take = 0;

            int tempPageNo = PageNo;

            //减去广告位的条目数【目前制定一个课程类型只存在一条广告数据】
            Take = (PageSize - SchDetails.Count());

            //第一页有可能加上了 广告位的数据 导致分页会遗漏掉第一页的
            if (PageNo > advePageTotal || (PageNo == advePageTotal && Take > 0))
            {
                PageNo = ((PageNo - 1) * PageSize) - adveTotal;
                if (PageNo <= 0)
                {
                    PageNo = 1;
                }
            }

            List<LSRFSchoolDetail> temp = new List<LSRFSchoolDetail>();
            if (Take > 0)
            {
                List<Guid> AdvSchIds = dis.Where(x => x.Type == PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Advertise).Select(x => x.Eid)?.ToList();

                //获取该课程类型下的学校数据
                temp = _schoolDetailService.GetLSRFSchools(CourseType, AdvSchIds, PageNo, Take);
            }

            if (temp.Any())
            {
                SchDetails.AddRange(temp);
            }

            //数据不足6条 获取推荐数据补充
            if (PageNo == 1 && SchDetails.Count() < 6)
            {
                int tempTake = (PageSize - SchDetails.Count());
                List<Guid> RecommSchIds = dis.Select(x => x.Eid)?.ToList();

                var recommend = _schoolDetailService.GetAdvertisementSchool(CourseType, PMS.OperationPlateform.Domain.Enums.LSRFSchoolType.Recommend, RecommSchIds, 1, tempTake);

                if (recommend.Any())
                {
                    Recommend.AddRange(recommend);
                }
            }

            var data = LSRFSchoolDetailToVoHelper.LSRFSchoolDetailHelper(SchDetails);

            var redata = LSRFSchoolDetailToVoHelper.LSRFSchoolDetailHelper(Recommend);

            int PageTotal = (int)(Math.Ceiling(Total / PageSize));

            return ResponseResult.Success(new
            {
                rows = new { data, redata },
                isEnd = PageTotal == TempNo
            });
        }

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
