using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using Sxb.UserCenter.Models.LiveViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using ProductManagement.Framework.Foundation;
using Microsoft.Extensions.Logging;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{

    public class ApiLiveController : Base
    {
        private ISysMessageService _sysMessage;
        private readonly IUserService _userService;
        private readonly ILiveServiceClient _liveServiceClient;
        private readonly ILogger _logger;
        public ApiLiveController(ISysMessageService sysMessage, IUserService userService, ILiveServiceClient liveServiceClient, ILogger<ApiLiveController> logger)
        {
            _sysMessage = sysMessage;
            _userService = userService;
            _liveServiceClient = liveServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// 个人主页直播接口列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<ResponseResult> HomeLive(Guid userId = default(Guid), int page = 1, int size = 10) 
        {
            Guid UserId = Guid.Empty, QueryUserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
                QueryUserId = user.UserId;
            }

            if (userId != default)
            {
                //其他用户查看达人主页
                UserId = userId;
            }

            List<LiveViewModel> liveViews = new List<LiveViewModel>();
            try
            {
                var result = await _liveServiceClient.LectorLectures(UserId, 5, page);
                if (result.Items != null && result.Items.Any())
                {
                    liveViews = result.Items.Select(q => new LiveViewModel
                    {
                        Id = q.Id,
                        FrontCover = q.HomeCover,
                        Time = ((long)q.Time).I2D().ConciseTime(),
                        ViewCount = q.Onlinecount,

                        Title = q.Subject,
                        LectorId = q.lector?.Id ?? Guid.Empty,
                        LectorName = q.lector?.Name,
                        LectorHeadImg = q.lector?.Headimgurl
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("获取讲师直播接口出错，ex：", ex.Message);
            }
            //组装视图实体

            var UserDto = _userService.GetUserInfo(UserId);
            var UserVo = UserHelper.UserDtoToVo(new List<PMS.UserManage.Application.ModelDto.UserInfoDto>() { UserDto }).FirstOrDefault();

            return ResponseResult.Success(new { lives = liveViews, user = UserVo });
        }

    }
}
