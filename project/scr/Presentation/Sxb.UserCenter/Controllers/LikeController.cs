using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;

namespace Sxb.UserCenter.Controllers
{
    public class LikeController : Base
    {

        public IsHost IsHost { get; }
        private ILikeService _likeService { get; set; }
        public LikeController( ILikeService likeService,IOptions<IsHost> _isHost)
        {
            IsHost = _isHost.Value;
            _likeService = likeService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Comment(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            return View(_likeService.GetCommentLike(cookieStr, page));
        }
        public IActionResult QA(int page = 1)
        {
            string cookieStr = Request.Headers["Cookie"];
            return View(_likeService.GetQALike(cookieStr, page));
        }
    }
}