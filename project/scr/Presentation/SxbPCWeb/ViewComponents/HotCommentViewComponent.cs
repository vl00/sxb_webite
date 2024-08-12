using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Utils.CommentHelper;

namespace Sxb.PCWeb.ViewComponents
{
    public class HotCommentViewComponent : ViewComponent
    {
        private readonly ISchoolCommentService _commentService;
        //热门点评、学校组合方法
        private PullHottestCSHelper hot;

        public HotCommentViewComponent(IGiveLikeService likeservice, IUserService _userService, ISchoolInfoService _schoolInfoService, IOptions<ImageSetting> set,
            ISchoolCommentService commentService, ISchService schService)
        {
            hot = new PullHottestCSHelper(likeservice, _userService, _schoolInfoService, set.Value, schService);
            _commentService = commentService;
        }

        public IViewComponentResult Invoke(int cityCode, Guid userId = default)
        {
            var date_Now = DateTime.Now;
            var query = new HotCommentQuery()
            {
                Condition = true,
                City = cityCode,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-2),
                EndTime = date_Now
            };

            var dtos = GetSchoolComments(query, userId);

            ViewBag.HottestComment = hot.HotComment(dtos.Result, userId);
            return View();
        }

        async Task<List<SchoolCommentDto>> GetSchoolComments(HotCommentQuery query, Guid userId)
        {
            var dtos = await _commentService.HotComment(query, userId);
            return dtos;
        }
    }
}
