using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Utils.CommentHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewComponents
{
    public class HotQuestionViewComponent : ViewComponent
    {
        private readonly ISchoolCommentService _commentService;

        protected IQuestionInfoService _questionInfoService;
        //热门点评、学校组合方法
        private PullHottestCSHelper hot;
        public HotQuestionViewComponent(IGiveLikeService likeservice, IUserService _userService, ISchoolInfoService _schoolInfoService, IOptions<ImageSetting> set,
            ISchoolCommentService commentService, IQuestionInfoService questionInfoService,ISchService schService)
        {
            hot = new PullHottestCSHelper(likeservice, _userService, _schoolInfoService, set.Value, schService);
            _commentService = commentService;
            _questionInfoService = questionInfoService;
        }

        public IViewComponentResult Invoke()
        {
            var date_Now = DateTime.Now;
            var questions = _questionInfoService.GetHotQuestion(date_Now.AddMonths(-1), date_Now, 50);
            CommonHelper.ListRandom(questions);
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(questions.Take(6).ToList());
            return View();
        }

        async Task<List<QuestionDto>> GetHotQuestions()
        {
            return await _questionInfoService.GetHotQuestion();
        }
    }
}
