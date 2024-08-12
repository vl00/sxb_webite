using Castle.DynamicProxy.Contributors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.ModelVo;
using PMS.UserManage.Application.ModelDto.Verify;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Service;
using ProductManagement.Framework.Cache.Redis;
using Sxb.UserCenter.Common;
using Sxb.UserCenter.Controllers;
using Sxb.UserCenter.Models.TalentViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;
using Sxb.UserCenter.Request;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;
using ProductManagement.Framework.Foundation;

namespace Sxb.Web.Controllers
{
    public class TalentController : Base
    {
        private readonly ITalentService _talentService;

        private readonly IAccountService _account;
        private readonly IUserService _userService;
        private readonly ICollectionService _collectionService;
        private readonly IQuestionsAnswersInfoService _answersInfoService;
        private readonly IMessageService _messageService;
        private readonly IArticleService _articleService;
        private readonly ILoginService _loginService;

        private readonly IEasyRedisClient _redisClient;
        private readonly IFileServiceClient _fileServiceClient;

        public TalentController(ITalentService talentService, IAccountService account,
            IUserService userService, ICollectionService collectionService, IEasyRedisClient redisClient,
            IQuestionsAnswersInfoService answersInfoService, IMessageService messageService, IArticleService articleService, ILoginService loginService, IFileServiceClient fileServiceClient)
        {
            _talentService = talentService;
            _account = account;
            _userService = userService;
            _collectionService = collectionService;
            _answersInfoService = answersInfoService;
            _messageService = messageService;
            _articleService = articleService;
            _loginService = loginService;
            _redisClient = redisClient;
            _fileServiceClient = fileServiceClient;
        }

        #region 达人
        /// <summary>
        /// 获取机构或个人达人列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="certificationType"></param>
        /// <param name="begainDate"></param>
        /// <param name="endDate"></param>
        /// <param name="type"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ResponseResult GetTalentList(string userId, string userName, int? certificationType, DateTime? begainDate, DateTime? endDate, int isInvite = 0, int type = 0, int pageindex = 1, int pagesize = 10)
        {
            var total = 0;
            var Rows = _talentService.GetTalentList(GetUserId().ToString(), userName, userId, certificationType, begainDate, endDate, ref total, isInvite, type, pageindex, pagesize);
            var result = new { Rows = Rows, PageIndex = pageindex, PageCount = Rows.Count, PageSize = pagesize, TotalCount = total };
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 修改达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ResponseResult UpdateTalent(TalentEntity talent)
        {
            var resp = _talentService.UpdateTalent(talent);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 修改达人状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ResponseResult ChangTalentStatus(string id, int status)
        {
            if (!_talentService.ChangTalentStatus(id, status))
            {
                return ResponseResult.Failed("修改失败");
            }
            return ResponseResult.Success();
        }


        /// <summary>
        /// 审核达人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="userId"></param>
        public ResponseResult AuditTalent(string id, int status, string reason)
        {
            _talentService.AuditTalent(id, status, reason, GetUserId().ToString());
            return ResponseResult.Success();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public ResponseResult GetSenior()
        {
            try
            {
                //var countData = _account.GetCountData(userId, Request.Headers["Cookie"]);
                var userInfo = _userService.GetUserInfo(userID);
                //var collectionInfo = _collectionService.GetUserFollowFans(userId);
                //var answerCount = _answersInfoService.QuestionAnswer(userId);
                if (userInfo?.HeadImgUrl?.ToLower().Contains("head.png") == true) userInfo.HeadImgUrl = null; //如果是默认头像的话 , 设置为null

                return ResponseResult.Success(new
                {
                    isExsitHead = !string.IsNullOrWhiteSpace(userInfo?.HeadImgUrl),
                    isBindMobile = !string.IsNullOrWhiteSpace(userInfo?.Mobile)
                    //isAnswerCountReach = answerCount >= 5,
                    //isCollectionReach = collectionInfo?.Follow >= 10
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }


        /// <summary>
        /// 达人码认证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user_id"></param>
        public ResponseResult InviteTalentByCode(string code)
        {
            var resp = _talentService.InviteTalentByCode(code, GetUserId().ToString());

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 邀请/申请达人
        /// </summary>
        /// <param name="talent"></param>
        /// <returns></returns>
        //[Produces("application/json")]
        public async Task<ResponseResult> InviteTalent([FromBody] TalentInput talentInput)
        {
            if (talentInput == null)
            {
                return ResponseResult.Failed("参数不能为为空");
            }
            if (talentInput.InviteType == 0)
            {
                talentInput.user_id = GetUserId().ToString();
            }

            var resp = await _talentService.ApplyTalent(talentInput);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 申请达人
        /// </summary>
        /// <param name="talentInput"></param>
        /// <returns></returns>

        public async Task<ResponseResult> ApplyTalent([FromBody] TalentInput talentInput)
        {
            if (talentInput == null)
            {
                return ResponseResult.Failed("参数不能为为空");
            }
            talentInput.user_id = GetUserId().ToString();
            var resp = await _talentService.ApplyTalent(talentInput);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 取消达人认证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseResult DisableTalent(string id)
        {
            var resp = _talentService.DisableTalent(id);
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 删除达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public ResponseResult DeleteTalentImg([FromBody] List<TalentImg> imgs)
        {
            var resp = _talentService.DeleteTalentImg(imgs);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 新增达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public ResponseResult AddTalentImg([FromBody] List<TalentImg> imgs)
        {
            var resp = _talentService.AddTalentImg(imgs);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }
        #endregion

        #region 机构员工
        /// <summary>
        /// 员工列表
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ResponseResult GetStaffList(int pageindex = 1, int pagesize = 10)
        {
            var total = 0;
            var Rows = _talentService.GetStaffList(GetUserId().ToString(), ref total, pageindex, pagesize);
            var result = new { Rows = Rows, PageIndex = pageindex, PageCount = Rows.Count, PageSize = pagesize, TotalCount = total };
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 邀请员工
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="effectivedate"></param>
        /// <returns></returns>
        public ResponseResult InviteStaff([FromBody] TalentStaffInputDto talentStaffdto)
        {
            var resp = _talentService.InviteStaff(GetUserId().ToString(), talentStaffdto);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 根据邀请码获取员工电话
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResponseResult GetTalentStaffPhoneByInviteCode(string code)
        {
            var resp = _talentService.GetTalentStaffPhoneByInviteCode(code);
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 发送员工登录验证短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SendStaffConfirmCode(string phone)
        {
            var resp = await _talentService.SendStaffConfirmCode(phone);
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="type">1为机构员工，0为申请达人</param>
        /// <returns></returns>
        public async Task<ResponseResult> SendMessage(string phone, int type = 1)
        {
            var resp = await _talentService.SendCode(phone);
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }
        /// <summary>
        /// 确认邀请
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="talentCode">达人码</param>
        /// <param name="smsCode">手机验证码</param>
        /// <param name="reconfirm">再次确认邀请</param>
        /// <returns></returns>
        public async Task<ResponseResult> StaffConfirmInvitation(string phone, string inviteCode, string smsCode, bool reconfirm = false)
        {
            var resp = await _talentService.VerifyStaffConfirmCode(phone, smsCode);
            object loginUserInfo = null;
            if (resp.code == 200)
            {
                resp = _talentService.StaffConfirmInvitation(phone, inviteCode, reconfirm);

                //验证短信后， 不论达人是否确认成功， 都登录
                var userInfo = _userService.GetUserInfo(phone);
                if (userInfo != null)
                {
                    TalentEntity talent = _talentService.GetTalentDetail(userInfo.Id.ToString());

                    loginUserInfo = new PMS.UserManage.Application.ModelDto.Login.Post()
                    {
                        ID = userInfo.Id,
                        Nickname = userInfo.NickName,
                        HeadImgUrl = userInfo.HeadImgUrl,
                        ReturnUrl = "/"
                    };

                    var loginInfo = new UserInfo
                    {
                        Id = userInfo.Id,
                        NickName = userInfo.NickName,
                        NationCode = (short?)userInfo.NationCode,
                        Mobile = userInfo.Mobile
                    };
                    await JwtCookieHelper.SetSignInCookie(HttpContext, loginInfo, talent);
                    HttpContext.Response.SetUserId(loginInfo.Id.ToString());
                }
            }
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = new
                {
                    type = resp.data,
                    userInfo = loginUserInfo
                }
            };
        }

        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseResult DeleteTalentStaff(Guid id)
        {
            if (!_talentService.DeleteTalentStaff(id))
            {
                return ResponseResult.Failed("删除失败");
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="talentStaffdto"></param>
        /// <returns></returns>
        public ResponseResult EditTalentStaff([FromBody] TalentStaffInputDto talentStaffdto)
        {
            if (!_talentService.EditTalentStaff(talentStaffdto))
            {
                return ResponseResult.Failed("修改失败");
            }
            return ResponseResult.Success();
        }
        #endregion

        #region 认证审核
        /// <summary>
        /// 认证审核通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="certificationType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseResult CheckCondition(string id, int certificationType, string userId)
        {
            return ResponseResult.Success(_talentService.CheckCondition(GetUserId()));
        }
        #endregion

        #region 关注
        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public ResponseResult AttentionUser([FromBody] AttentionUserData data)
        {
            var resp = _talentService.AttentionUser(GetUserId(), data.attionUserId);

            if (resp.code == 200)
            {
                CleanFocuseCache(GetUserId(), data.attionUserId);
            }

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public ResponseResult CancleAttention([FromBody] AttentionUserData data)
        {
            var resp = _talentService.CancleAttention(GetUserId(), data.attionUserId);
            if (resp.code == 200)
            {
                CleanFocuseCache(GetUserId(), data.attionUserId);
            }
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }



        private void CleanFocuseCache(Guid UserId, Guid FocuseUserId)
        {
            _redisClient.FuzzyRemove("lectures_adcode_");//清除首页推荐课堂列表
            _redisClient.FuzzyRemove("lectors_adcode_");//清除首页推荐讲师列表
            string key = "iscollect_userid_{0}_lectorid_{1}"; //0替换家长userid  1替换达人userid
            _redisClient.RemoveAsync(string.Format(key, UserId, FocuseUserId));
        }

        /// <summary>
        /// 获取新增的粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.Authorize]
        public ResponseResult GetNewFans(int pageindex = 1, int pagesize = 10)
        {
            var total = 0;
            var Rows = _talentService.GetNewFans(GetUserId(), ref total, pageindex, pagesize);
            var result = new { Rows = Rows, PageIndex = pageindex, PageCount = Rows.Count, PageSize = pagesize, TotalCount = total };
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取新增粉丝数量
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetNewFansCount()
        {
            return ResponseResult.Success(_talentService.GetNewFansCount(GetUserId()));
        }

        /// <summary>
        /// 获取ta的粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseResult GetFans(Guid userId, int pageindex = 1, int pagesize = 10)
        {
            //登录人
            Guid loginUserId = GetUserId();
            //被查询人
            Guid searchUserId = userId == Guid.Empty ? loginUserId : userId;
            if (searchUserId == Guid.Empty)
            {
                return ResponseResult.Failed("请登录");
            }

            var pagination = _talentService.GetFans(loginUserId, searchUserId, pageindex, pagesize);
            var result = new { Rows = pagination.Data, PageIndex = pageindex, PageCount = pagination.Data.Count, PageSize = pagesize, TotalCount = pagination.Total };
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取ta的关注
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseResult GetAttention(Guid userId, int pageindex = 1, int pagesize = 10)
        {
            //登录人
            Guid loginUserId = GetUserId();
            //被查询人
            Guid searchUserId = userId == Guid.Empty ? loginUserId : userId;
            if (searchUserId == Guid.Empty)
            {
                return ResponseResult.Failed("请登录");
            }

            var pagination = _talentService.GetAttention(loginUserId, searchUserId, pageindex, pagesize);
            var result = new { Rows = pagination.Data, PageIndex = pageindex, PageCount = pagination.Data.Count, PageSize = pagesize, TotalCount = pagination.Total };
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 标记为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseResult NoticeUser(List<Guid> id)
        {
            if (!_talentService.NoticeUser(id))
            {
                return ResponseResult.Failed("修改失败");
            }
            return ResponseResult.Success();
        }
        #endregion

        #region 达人身份认证
        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ResponseResult GetTalentPageIdentityList(Guid? id, string identity, int type = 0, int pageindex = 1, int pagesize = 10)
        {
            var total = 0;
            var Rows = _talentService.GetTalentPageIdentityList(id, identity, ref total, type, pageindex, pagesize);
            var result = new { Rows = Rows, PageIndex = pageindex, PageCount = Rows.Count, PageSize = pagesize, TotalCount = total };
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ResponseResult GetTalentAllIdentityList(Guid? id, string identity, int type = 0)
        {
            var data = _talentService.GetTalentAllIdentityList(id, identity, type);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 修改或新增达人身份
        /// </summary>
        /// <param name="talentIdentity"></param>
        /// <returns></returns>
        public ResponseResult EditOrAddIdentity([FromBody] TalentIdentityInput talentIdentityInput)
        {
            var resp = _talentService.EditOrAddIdentity(talentIdentityInput);

            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }
        #endregion

        #region 个人资料
        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <param name="interestDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult EditUserData([FromBody] InterestDto interestDto)
        {
            //var interestDto = new InterestDto() { userID = GetUserId(),cityCode= cityCode, introduction = introduction, focus_grade = focus_grade, focus_type= focus_type, focus_lodging = focus_lodging };
            if (interestDto == null)
            {
                return ResponseResult.Failed("修改失败，无数据");
            }
            interestDto.userID = GetUserId();
            var resp = _talentService.EditUserData(interestDto);
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        [HttpPost]
        [Authorize]
        public async Task<ResponseResult> MpEditUserData([FromBody] MpEditUserVM dto)
        {
            if (dto == null)
            {
                return ResponseResult.Failed("修改失败，无数据");
            }

            dto.HeadImgUrl = await _fileServiceClient.ConvertToSxbImg(dto.HeadImgUrl);
            var resp = await _talentService.MpEditUserData(new MpUpdateUserDto()
            {
                UserId = GetUserId(),
                NickName = dto.NickName,
                HeadImgUrl = dto.HeadImgUrl
            });
            return new ResponseResult()
            {
                Succeed = resp.code == 200,
                status = resp.code == 200 ? ResponseCode.Success : ResponseCode.Failed,
                Msg = resp.message,
                Data = resp.data
            };
        }

        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResponseResult GetUserData()
        {
            var result = ResponseResult.Success();
            if (User.Identity.IsAuthenticated)
            {
                var data = _talentService.GetUserData(GetUserId());
                if (data != null) result.Data = data;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <returns></returns>
        public Guid GetUserId()
        {
            //return new Guid("e7be96eb-b31f-4857-ba34-f2f165c4793c");
            if (User.Identity.IsAuthenticated)
            {
                return User.Identity.GetUserInfo().UserId;
            }
            //return Request.GetDeviceToGuid();
            return Guid.Empty;
        }

        /// <summary>
        /// 获取达人动态
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ResponseResult GetTalentStaffStatus(Guid userId, int? agoDay)
        {
            TalentStatusViewModel model = new TalentStatusViewModel();

            var searchUserId = userId != Guid.Empty ? userId : GetUserId();
            if (searchUserId == Guid.Empty)
            {
                return ResponseResult.Success(model);
            }

            var parentTalent = _talentService.GetTalentByUserId(searchUserId.ToString());
            if (parentTalent == null) return ResponseResult.Success(model);

            //查询时间
            DateTime? startTime = null, endTime = null;
            if (agoDay != null)
            {
                startTime = DateTime.Now.AddDays(-agoDay.Value);
                endTime = DateTime.Now;
            }

            var user = _userService.GetUserInfo(parentTalent.user_id);
            var talents = _talentService.GetTalentChildren(parentTalent.id.ToString());
            if (talents != null && talents.Count > 0)
            {
                model.Children = new List<TalentStatusViewModel>();
                TalentStatusViewModel child;
                foreach (var talent in talents)
                {
                    var childUserId = talent.user_id;
                    child = new TalentStatusViewModel();
                    child.TalentId = talent.id.ToString();
                    child.NickName = talent.nickname;
                    child.StaffName = talent.staffname;

                    //接收的赞和关注
                    child.LikeTotal = _messageService.GetMessageTotal(null, childUserId, MessageType.Like, null, read: null, ignore: false, startTime, endTime);
                    child.FollowTotal = _collectionService.GetCollectionUserTotal(childUserId, startTime, endTime);

                    //发送的问答 -- 【规则】统计所有员工这个时间段内回答的问题数量
                    child.QATotal = _messageService.GetMessageTotal(childUserId, null, MessageType.Reply, MessageDataType.Answer, read: null, ignore: null, startTime, endTime);

                    //发送的文章数量
                    child.ArticleTotal = _articleService.GetArticleTotal(talent.id, startTime, endTime);
                    //讲师  开课中/已结束的课程数量
                    child.LectureTotal = _talentService.GetLectureTotalByUserId(childUserId, startTime, endTime);

                    model.Children.Add(child);
                }
                //按总数逆序
                model.Children = model.Children.OrderByDescending(child => child.QATotal + child.ArticleTotal + child.LectureTotal).ToList();

                model.LikeTotal = model.Children.Sum(s => s.LikeTotal);
                model.FollowTotal = model.Children.Sum(s => s.FollowTotal);
                model.QATotal = model.Children.Sum(s => s.QATotal);
                model.ArticleTotal = model.Children.Sum(s => s.ArticleTotal);
                model.LectureTotal = model.Children.Sum(s => s.LectureTotal);
            }

            model.TalentId = parentTalent.id.ToString();
            model.NickName = user?.NickName;

            return ResponseResult.Success(model);
        }
    }
}