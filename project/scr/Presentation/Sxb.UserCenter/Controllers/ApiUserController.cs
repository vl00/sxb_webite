using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.Info;
using PMS.UserManage.Application.ModelDto.Message;
using PMS.UserManage.Domain.Entities;
using Sxb.UserCenter.Models.MessageViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.Utils.DtoToViewModel;
using Sxb.UserCenter.Request;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.UserManage.Application.Services;
using ProductManagement.UserCenter.Wx.Model;
using ProductManagement.Framework.Cache.Redis;
using PMS.Infrastructure.Application.IService;
using ProductManagement.UserCenter.Wx;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    /// <summary>
    /// 达人系统【我的】页面
    /// </summary>
    [AllowAnonymous]
    public class ApiUserController : Base
    {
        private IKeyValueService _keyValueService;
        private IAccountService _account;
        private IUserService _userService;
        private ICollectionService _collectionService;
        private IMessageService _message;
        private ITalentService _talentService;
        private ISysMessageService _sysMessage;
        private IMapper _mapper;
        IEasyRedisClient _easyRedisClient;
        private WeixinUtil _weixinUtil;

        public ApiUserController(IAccountService account, IKeyValueService keyValueService,
            ICollectionService collectionService,
            IUserService userService,
            ISysMessageService sysMessage,
            IMessageService message, ITalentService talentService, IMapper mapper, IEasyRedisClient easyRedisClient)
        {
            _keyValueService = keyValueService;
            _easyRedisClient = easyRedisClient;
            _sysMessage = sysMessage;
            _message = message;
            _collectionService = collectionService;
            _userService = userService;
            _account = account;
            _talentService = talentService;
            _mapper = mapper;
            _weixinUtil = new WeixinUtil(new System.Net.Http.HttpClient());
        }

        [Description("获取当前用户信息")]
        [HttpGet]
        public ResponseResult GetUserInfo(Guid userId)
        {

            if (userId == Guid.Empty && !User.Identity.IsAuthenticated)
                return ResponseResult.Failed("请先登录", new UserInfoDetailModel());

            Guid UserId = userId;
            bool IsFollow = false;
            if (User.Identity.IsAuthenticated)
            {
                var userIdentity = User.Identity.GetUserInfo();
                if (userId != Guid.Empty)
                    IsFollow = _collectionService.IsCollected(userIdentity.UserId, UserId, CollectionDataType.User) ||
                        _collectionService.IsCollected(userIdentity.UserId, UserId, CollectionDataType.UserLector);
                else
                    UserId = userIdentity.UserId;
            }

            var total = _collectionService.GetUserFollowFans(UserId);

            //获取用户设置的关键词
            string uuid = Request.Cookies["uuid"];
            PMS.UserManage.Application.ModelDto.Info.Interest interest = _account.GetUserInterest(UserId, uuid);

            var user = _userService.GetTalentDetail(UserId);

            return ResponseResult.Success(UserInfoDtoToVoHelper.UserDtoToVo(user, total, interest, IsFollow));
        }

        [Description("获取当前用户的动态")]
        [HttpGet]
        public async Task<ResponseResult> GetMydynamic()
        {

            Guid UserId = Guid.Empty;
            bool IsAuth = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
                IsAuth = user.IsAuth;
            }
            else
            {
                return ResponseResult.Success(new MydynamicDetail() { IsLogin = false });
            }

            List<InviteMessageViewModel> invites = new List<InviteMessageViewModel>();
            List<FollowLive> live = new List<FollowLive>();

            var total = _collectionService.MydynamicTotal(UserId);
            var talent = _userService.GetTalentDetail(UserId);


            //已认证获取邀请（点评、回答、提问）
            if (IsAuth)
            {
                //邀请
                var types = new List<byte>() { (byte)MessageType.InviteAnswer, (byte)MessageType.InviteComment,
                    (byte)MessageType.InviteQuestion };//, (byte)MessageType.InviteReplyComment

                var message = _message.GetPrivateMessage(UserId, 1, 6, types, true);
                invites = MessageModelToViewModel.MessageModelToViewModels(message);
            }
            else
            {
                //查看是否有关注的达人正在直播的微课
                //string  key = $"User:LiveMessageTips:{UserId}"


                var liveTips = _sysMessage.GetLiveMessageTips(UserId, 1, 6);
                if (liveTips.Any())
                {
                    //调用 微课api，根据id反查得到微课列表
                    foreach (var item in liveTips)
                    {
                        live.Add(new FollowLive()
                        {
                            Id = item.Id,
                            DataId = item.DataId,
                            Title = item.Content,
                            LiveUser = new Models.UserInfoViewModel.UserViewModel()
                            {
                                Id = item.SenderUserId,
                                UserName = item.nickname,
                                HeadImage = item.HeadImgUrl
                            }
                        });
                    }
                }
            }

            var bind = _account.GetBindInfo(UserId);

            var model = UserInfoDtoToVoHelper.UserDynamicDtoToVo(talent, total, invites, live);
            model.IsBindPhone = !string.IsNullOrEmpty(bind.Mobile);

            model.IsLogin = true;
            model.HasSysMeesage = await _sysMessage.HasSysMessage(UserId);

            return ResponseResult.Success(model);
        }

        /// <summary>
        /// 是否绑定手机
        /// </summary>
        /// <returns></returns>
        public ResponseResult IsBindPhone()
        {
            var isBindPhone = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserInfo().UserId;
                isBindPhone = !string.IsNullOrWhiteSpace(_userService.GetUserInfo(userId)?.Mobile);
            }
            return ResponseResult.Success(isBindPhone);
        }

        /// <summary>
        /// 批量获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult GetUsers([FromBody] GetUsersData data)
        {
            var users = _userService.ListUserInfo(data.UserIds);
            return ResponseResult.Success(users.Select(q => new { q.Id, q.HeadImgUrl, q.NickName }));
        }


        /// <summary>
        /// 通过微信OpenId获取用户昵称等信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetWxUserInfoByOpenID(string openId)
        {
            string weixinAccessToken = await _keyValueService.GetValue("weixin_access_token_fwh");
            WXUserInfoResult WXuserinfo = await _weixinUtil.GetUserInfo(weixinAccessToken, openId);

            return ResponseResult.Success(new
            {
                WXuserinfo.nickname,
                WXuserinfo.headimgurl,
                WXuserinfo.sex
            });
        }


        [HttpPost]
        public async Task<ResponseResult> SetUserNicknameOrHeadImg([FromBody] SetUserNicknameOrHeadImgRequest request)
        {
            var result = ResponseResult.Success();

            if (!User.Identity.IsAuthenticated)
            {
                result.status = ResponseCode.NoAuth;
                result.Msg = "need login";
                return result;
            }

            var userInfo = _userService.GetUserInfo(User.Identity.GetId());
            var openid = request.OpenID;
            //if (string.IsNullOrWhiteSpace(openid))
            //{
            //    openid = await _userService.GetUserOpenID(userInfo.Id,"web");
            //}
            //if (string.IsNullOrWhiteSpace(openid))
            //{
            //    result.status = ResponseCode.Failed;
            //    result.Msg = "参数错误";
            //    return result;
            //}
            var nickName = request.NickName;
            var isTalent = _talentService.IsTalent(userInfo.Id.ToString());//达人不让改昵称
            WXUserInfoResult wxUserInfo = null;
            if (string.IsNullOrWhiteSpace(openid))
            {
                wxUserInfo = await _easyRedisClient.GetAsync<WXUserInfoResult>($"WXUserInfo:UserID_{userID}");
            }
            else
            {
                var accessToken = await _keyValueService.GetValue("weixin_access_token");
                wxUserInfo = await _weixinUtil.GetUserInfo(accessToken, openid);
            }

            if (!string.IsNullOrWhiteSpace(wxUserInfo.headimgurl) && HttpContext.Request.Headers.Any(p => p.Key == "Referer" && !string.IsNullOrWhiteSpace(p.Value) && p.Value.ToString().ToLower().Contains("https://")))
            {
                wxUserInfo.headimgurl = wxUserInfo.headimgurl.Replace("http://", "https://");
            }


            if (!isTalent)
            {
                if (string.IsNullOrWhiteSpace(nickName))
                {

                    if (wxUserInfo != null && !string.IsNullOrWhiteSpace(wxUserInfo.errmsg))
                    {
                        result.status = ResponseCode.Failed;
                        //result.Msg = "can not get wechat user info : " + wxUserInfo.errmsg;
                        result.Msg = "无法获取到微信信息";
                        return result;
                    }
                    nickName = wxUserInfo.nickname;
                }

                if (userInfo.NickName != nickName)
                {
                    if (await _userService.CheckNicknameExist(nickName))
                    {
                        result.status = ResponseCode.Failed;
                        result.Msg = "您的昵称已被占用";
                        return result;
                    }
                }
            }
            else
            {
                nickName = userInfo.NickName;
            }
            if (!_userService.SyncUserInfo(userInfo.Id, nickName.Trim(), userInfo.Mobile, string.IsNullOrWhiteSpace(wxUserInfo.headimgurl) ? userInfo.HeadImgUrl : wxUserInfo.headimgurl))
            {
                result.status = ResponseCode.Error;
                result.Msg = "update action error";
            }
            else
            {
                result.Msg = "Success";
            }

            return result;
        }

        /// <summary>
        /// 通过UserId获取Union 微信昵称
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GetUnionWeixinNickName(Guid userId)
        {
            string nickName = _userService.GetUnionWeixinNickName(userId);
            return ResponseResult.Success<string>(nickName);
        }

        /// <summary>
        /// 通过UserId获取openId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ResponseResult> CheckBindFwh(Guid? userId)
        {
            if (userId == null)
            {
                userId = userID;
            }
            if (userId == null)
            {
                return ResponseResult.Failed("无参数UserId");
            }

            string openId = await _userService.GetUserOpenID(userId.Value, "fwh");

            return ResponseResult.Success(new
            {
                OpenId = openId,
                IsFollowFwh = !string.IsNullOrWhiteSpace(openId)
            });
        }
    }
}
