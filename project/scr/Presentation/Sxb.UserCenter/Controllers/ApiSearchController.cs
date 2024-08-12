using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class ApiSearchController : Controller
    {
        private readonly IUserService _userService;
        public ApiSearchController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 个人 | 他人动态搜索
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Description("动态搜索")]
        public ResponseResult SearchDynamic(Guid userId = default) 
        {
            Guid UserId = Guid.Empty, QueryUserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
                QueryUserId = user.UserId;
            }

            //用户实体
            var UserDto = _userService.GetUserInfo(UserId);
            var UserVo = UserHelper.UserDtoToVo(new List<PMS.UserManage.Application.ModelDto.UserInfoDto>() { UserDto }).FirstOrDefault();

            return ResponseResult.Success(new { user = UserVo, datas = new List<Sxb.UserCenter.Models.CommentViewModel.DynamicTmepItem>(), live = new Sxb.UserCenter.Models.CommentViewModel.Live(), isBindPhone = string.IsNullOrEmpty(UserDto.Mobile) });
        }


    }
}
