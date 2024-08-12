using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PMS.Live.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Domain.Enum;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Response;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    public class SpecialTopicController : BaseController
    {
        ISpecialTopicServices _specialTopicServices;
        ISpecialTopicItemServices _specialTopicItemServices;
        ILectureService _lectureService;
        IUserService _userService;

        public SpecialTopicController(ISpecialTopicServices specialTopicServices, ISpecialTopicItemServices specialTopicItemServices, ILectureService lectureService,
            IUserService userService)
        {
            _userService = userService;
            _lectureService = lectureService;
            _specialTopicServices = specialTopicServices;
            _specialTopicItemServices = specialTopicItemServices;
        }

        [Description("分页获取专题列表")]
        [HttpPost]
        public ResponseResult Page(int offset, int limit, string cityCode, SpecialTopicType type = SpecialTopicType.Unknow)
        {
            var finds = _specialTopicServices.Page(offset, limit, cityCode, type, "Index", "asc");
            if (finds.Item1?.Any() == true)
            {
                return ResponseResult.Success(finds.Item1);
            }
            return ResponseResult.Failed("no data found");
        }

        [Description("直播专题达人阵容")]
        [HttpPost]
        public ResponseResult GetLiveTopicUser(Guid specialTopicID, int limit = 10)
        {
            if (specialTopicID == default) return ResponseResult.Failed("param error");
            var finds = _specialTopicServices.GetLiveTopicUsers(specialTopicID, limit);
            if (finds?.Any() == true)
            {
                return ResponseResult.Success(finds);
            }
            return ResponseResult.Failed("no data found");
        }

        [Description("分页获取专题子项")]
        [HttpPost]
        public async Task<ResponseResult> ItemPage(Guid id, int offset = 0, int limit = 10)
        {
            var specialTopic = _specialTopicServices.Get(id);
            if (specialTopic == null) return ResponseResult.Failed("id error");
            var finds = _specialTopicItemServices.Page(offset, limit, id, "Index", "asc");
            if (finds?.Any() == true)
            {
                if (specialTopic.Type == SpecialTopicType.Live)
                {
                    var liveIDs = finds.Select(p => p.TargetID).Distinct();
                    var lives = await _lectureService.GetLectureDetails(liveIDs);

                    return ResponseResult.Success(new
                    {
                        topic = specialTopic,
                        items = lives.Select(p => new
                        {
                            finds.FirstOrDefault(x => x.TargetID == p.ID)?.ID,
                            finds.FirstOrDefault(x => x.TargetID == p.ID)?.TargetID,
                            finds.FirstOrDefault(x => x.TargetID == p.ID)?.TargetUserName,
                            finds.FirstOrDefault(x => x.TargetID == p.ID)?.Index,
                            finds.FirstOrDefault(x => x.TargetID == p.ID)?.Title,
                            LiveStatus = p.Status,
                            LiveCover = p.CoverUrl,
                            LiveViewCount = p.ViewCount
                        }).OrderBy(p => p.Index)
                        //items = finds.Select(p => new
                        //{
                        //    p.ID,
                        //    p.TargetID,
                        //    p.Title,
                        //    p.TargetUserName,
                        //    p.Index,
                        //    LiveStatus = lives.FirstOrDefault(x => x.ID == p.TargetID)?.Status,
                        //    LiveCover = lives.FirstOrDefault(x => x.ID == p.TargetID)?.CoverUrl,
                        //    LiveViewCount = lives.FirstOrDefault(x => x.ID == p.TargetID)?.ViewCount
                        //})
                    });
                }
                else
                {
                    return ResponseResult.Success(new
                    {
                        topic = specialTopic,
                        items = finds
                    });
                }
            }
            return ResponseResult.Failed("no data found");
        }
    }
}
