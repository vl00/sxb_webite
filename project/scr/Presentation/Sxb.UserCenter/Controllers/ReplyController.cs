using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;

namespace Sxb.UserCenter.Controllers
{
    public class ReplyController : Base
    {
        private IReplyService _service;

        public ReplyController(IReplyService publishService)
        {
            _service = publishService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Comment(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            return View(_service.GetCommentList(cookieStr, page));
        }
        public IActionResult QA(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            return View(_service.GetQAList(cookieStr, page));
        }
    }
}