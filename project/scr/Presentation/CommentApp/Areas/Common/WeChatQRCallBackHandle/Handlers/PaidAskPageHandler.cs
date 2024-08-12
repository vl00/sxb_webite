using PMS.Infrastructure.Application.IService;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class PaidAskPageHandler :  ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        IUserService _userService;
        public PaidAskPageHandler(IWeChatService weChatService, IUserService userService)
        {
            _weChatService = weChatService;
            _userService = userService;
        }

        public  async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var userInfo = _userService.GetUserInfo(qrequest.DataId.GetValueOrDefault());
            if (userInfo == null)
            {
                return ResponseResult.Failed("找不到该达人。");
            }
            await _weChatService.SendNews(frequest.OpenId
             , "点击开始咨询"
             , $"感谢关注上学帮！点击向{userInfo.NickName}开始咨询"
             , "https://cdn.sxkid.com/images/logo_share_v4.png"
             , qrequest.DataUrl);
            return ResponseResult.Success("OK");
        }
    }
}
