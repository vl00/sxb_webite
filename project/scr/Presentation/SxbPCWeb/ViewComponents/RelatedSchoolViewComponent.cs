using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Common;
using PMS.PaidQA.Application.Services;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Utils.CommentHelper;
using Sxb.PCWeb.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewComponents
{
    public class RelatedSchoolViewComponent : ViewComponent
    {
        IHotTypeService _hotTypeService;
        IHotPopularService _hotPopularService;
        ISchService _schService;
        ISchoolCommentService _commentService;
        IQuestionInfoService _questionInfoService;
        //热门点评、学校组合方法
        PullHottestCSHelper _hot;

        public RelatedSchoolViewComponent(IHotTypeService hotTypeService, IOrderService orderService, ITalentSettingService talentSettingService, ISchoolInfoService schoolInfoService,
              IHotPopularService hotPopularService, ISchService schService, IGiveLikeService likeservice, IUserService userService, IOptions<ImageSetting> set, ISchoolCommentService commentService,
              IQuestionInfoService questionInfoService)
        {
            _questionInfoService = questionInfoService;
            _commentService = commentService;
            _schService = schService;
            _hotPopularService = hotPopularService;
            _hotTypeService = hotTypeService;
            _hot = new PullHottestCSHelper(likeservice, userService, schoolInfoService, set.Value, schService);
        }

        public IViewComponentResult Invoke(Guid? eid = null)
        {
            var result = new RelatedSchoolViewModel();

            var date_Now = DateTime.Now;

            result.NearSchool = GetNearSchool(eid).Result;

            result.HotSchools = GetHotSchool(eid).Result;
            //热门学校点评|热问学校
            result.HotCommentSchools = _hot.HottestSchool(_commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, false).Result);

            //热问学校【侧边栏】
            result.HotQuestionSchools = _hot.HottestQuestionItem(_questionInfoService.HottestSchool().Result);

            return View(result);
        }

        /// <summary>
        /// 获取周边学校
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        async Task<IEnumerable<SchExtDto0>> GetNearSchool(Guid? eid = null)
        {
            if (eid.HasValue && eid.Value != Guid.Empty)
            {
                var finds = await _schService.GetNearSchoolsByEID(eid.Value, 16);
                if (finds?.Any() == true)
                {
                    finds = CommonHelper.ListRandom(finds);
                    return finds.Take(6);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取热门学校
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        async Task<IEnumerable<SimpleHotSchoolDto>> GetHotSchool(Guid? eid = null)
        {
            var userCitycode = Request.GetLocalCity();
            iSchool.SchFType0[] schtypes = null;

            var result = await _hotPopularService.GetHotVisitSchools(userCitycode, schtypes, 50);
            if (result?.Length > 0)
            {
                var list_Result = result.ToList();
                CommonHelper.ListRandom(list_Result);
                result = list_Result.Take(6).OrderByDescending(p => p.TotalScore).ToArray();

                var eids = result.Select(p => p.Eid).ToArray();
                var nos = _schService.GetSchoolextNo(eids);
                if (nos?.Length > 0)
                {
                    foreach (var no in nos)
                    {
                        if (result.Any(p => p.Eid == no.Item1)) result.FirstOrDefault(p => p.Eid == no.Item1).SchoolNo = no.Item2;
                    }
                    //ViewBag.SchoolNos = nos.Result.ToDictionary(k => k.Item1, v => UrlShortIdUtil.Long2Base32(v.Item2));
                }
            }
            return result;
        }
    }
}
