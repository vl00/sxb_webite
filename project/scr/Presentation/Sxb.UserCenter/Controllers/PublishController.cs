using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;

namespace Sxb.UserCenter.Controllers
{
    public class PublishController : Base
    {
        private IPublishService _service;

        public PublishController(IPublishService publishService)
        {
            _service = publishService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Comment(int page=1)
        {
            string cookieStr = Request.Headers["Cookie"];
            var ret = _service.GetCommentList(cookieStr, page);
            return View(ret);
        }
        public IActionResult QA(int page=1)
        {
            string cookieStr = Request.Headers["Cookie"];
            var ret = _service.GetQAList(cookieStr, page);
            return View(ret);
        }
    }
}