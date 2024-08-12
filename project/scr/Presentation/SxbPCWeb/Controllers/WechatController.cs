//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Sxb.Web.Common;
//using ProductManagement.Framework.Foundation;
//using PMS.CommentsManage.Application.IServices;
//using PMS.School.Application.IServices;
//using System.Text;
//using Microsoft.AspNetCore.Authorization;
//using Sxb.Web.Models.User;
//using PMS.UserManage.Application.IServices;
//using PMS.UserManage.Domain.Common;
//using AutoMapper;
//using PMS.UserManage.Application.ModelDto;
//using Sxb.Web.Response;
//using PMS.CommentsManage.Application.ModelDto;
//using ProductManagement.Framework.Cache.Redis;
//using ProductManagement.API.Http.RequestCommon;
//using System.Text.Encodings.Web;
//using Sxb.PCWeb.Common;
//using Sxb.PCWeb.Controllers;

//namespace Sxb.Web.Controllers
//{
//    [Authorize]
//    public class WechatController : BaseController
//    {
//        private ISchoolCommentService _commentService;
//        private ISchoolService _schoolService;
//        private IQuestionInfoService _questionInfo;
//        private ISchoolCommentReplyService _replyService;
//        private ISchoolService _schoolInfoService;
//        private IUserService _userService;
//        private IEasyRedisClient _easyRedisClient;

//        private IQuestionsAnswersInfoService _questionsAnswers;

//        private readonly IMapper _mapper;

//        public WechatController(ISchoolCommentService commentService,
//            ISchoolService schoolService,
//            IQuestionInfoService questionInfo,
//            ISchoolService schoolInfoService,
//            IUserService userService,
//            IEasyRedisClient easyRedisClient,
//            ISchoolCommentReplyService replyService,
//            IQuestionsAnswersInfoService questionsAnswers,
//            IMapper mapper)
//        {
//            _commentService = commentService;
//            _schoolService = schoolService;
//            _questionInfo = questionInfo;

//            _easyRedisClient = easyRedisClient;
//            _schoolInfoService = schoolInfoService;
//            _mapper = mapper;

//            _replyService = replyService;

//            _questionsAnswers = questionsAnswers;

//            _userService = userService;
//        }

//        /// <summary>
//        /// 根据角色类型获取浏览过相同学校的用户信息
//        /// </summary>
//        /// <param name="SchoolExtId"></param>
//        /// <param name="Role"></param>
//        /// <param name="PageIndex"></param>
//        /// <param name="PageSize"></param>
//        /// <returns></returns>
//        [Authorize]
//        public async Task<ResponseResult> GetPushUserInfo(Guid SchoolExtId, int Role, int type, Guid dataID, Guid eID, int PageIndex = 1, int PageSize = 10)
//        {
//            try
//            {
//                int dataType = 0;
//                int inviteType = 0;
//                if ((ShareType)type == ShareType.Comment || (ShareType)type == ShareType.SuccessComment || (ShareType)type == ShareType.Reply)
//                {
//                    if ((ShareType)type == ShareType.Comment)
//                    {
//                        inviteType = (int)MessageType.InviteComment;
//                    }
//                    else
//                    {
//                        inviteType = (int)MessageType.Reply;
//                    }

//                    dataType = (int)MessageDataType.Comment;
//                }
//                else if ((ShareType)type == ShareType.Question || (ShareType)type == ShareType.Answer || (ShareType)type == ShareType.SuccessQuestion)
//                {
//                    if ((ShareType)type == ShareType.Question)
//                    {
//                        inviteType = (int)MessageType.InviteQuesion;
//                    }
//                    else
//                    {
//                        inviteType = (int)MessageType.InviteAnswer;
//                    }

//                    dataType = (int)MessageDataType.Question;
//                }

//                Guid senderID = default(Guid);
//                if (User.Identity.IsAuthenticated)
//                {
//                    var user = User.Identity.GetUserInfo();
//                    senderID = user.UserId;
//                }

//                //获取相同类型学校，写入缓存
//                string key = $"schoolSectionIdentical:Section:{SchoolExtId}&PageIndex:{PageIndex}&PageSize:{PageSize}";
//                var SchoolExt = await _easyRedisClient.GetOrAddAsync(
//                   key, () => { return _schoolInfoService.GetIdenticalSchool(SchoolExtId, PageIndex, PageSize); },
//                    new TimeSpan(0, 30, 0));

//                List<Guid> ExtIds = SchoolExt.Select(x => x.ExtId)?.ToList();
//                List<InviteUserInfoVo> userInfoVos = new List<InviteUserInfoVo>();

//                //获取用户信息，根据学校类型
//                var UserDto = _userService.GetUserInfoByHistory(ExtIds, (UserRole)Role, PageIndex, PageSize, inviteType, dataType, senderID, dataID, SchoolExtId, senderID);
//                //var UserDto = _userService.GetUserInfoByHistory(ExtIds, (UserRole)Role, PageIndex, PageSize, type, dataType, senderID, dataID, eID);
//                userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(UserDto));
//                if (userInfoVos.Count() == 0)
//                {
//                    userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(_userService.GetUserInfoByHistory(Role, PageIndex, PageSize, inviteType, dataType, senderID, dataID, SchoolExtId, senderID)));
//                }

//                if (userInfoVos.Count() == 0 && Role == 0)
//                {
//                    userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(_userService.GetUserInfoByHistory(0, PageIndex, PageSize, inviteType, dataType, senderID, dataID, SchoolExtId, senderID)));
//                }
//                return ResponseResult.Success(userInfoVos);
//            }
//            catch (Exception ex)
//            {
//                return ResponseResult.Failed(ex.Message);
//            }
//        }

//        [Authorize]
//        public ResponseResult GetOrdinaryUser(int PageIndex, int PageSize)
//        {
//            try
//            {
//                Guid userId = default(Guid);
//                if (User.Identity.IsAuthenticated)
//                {
//                    var user = User.Identity.GetUserInfo();
//                    userId = user.UserId;
//                }

//                var userInfos = _userService.GetOrdinaryUserInfo(PageIndex, PageSize, userId);
//                return ResponseResult.Success(_mapper.Map<List<UserInfoDto>, List<UserInfoVo>>(userInfos));
//            }
//            catch (Exception ex)
//            {
//                return ResponseResult.Failed(ex.Message);
//            }
//        }

//        /// <summary>
//        /// true：点评 | false：问题
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        public async Task<IActionResult> Index(ShareType type, Guid SchoolId = default(Guid), Guid DataSource = default(Guid), bool isReplyDetail = false)
//        {
//            Guid userId = Guid.Empty;
//            var user = User.Identity.GetUserInfo();
//            userId = user.UserId;

//            ViewBag.isComment = type.Description();
//            ViewBag.Operation = (int)type;

//            //是否为校方用户
//            ViewBag.isSchoolRole = false;

//            //为提问、回答
//            bool isQuestionAnswer = false;

//            //消息推送内容
//            string PushMessage = "";
//            //Dictionary<string, string> keys = new Dictionary<string, string>();
//            string Title = "", shareContent = "", shareLink = "", questionContent = "";
//            int Total = 0;
//            if (type == ShareType.Comment || type == ShareType.SuccessComment || type == ShareType.Reply)
//            {
//                if (type != ShareType.Comment)
//                {
//                    if (type == ShareType.Reply && isReplyDetail)
//                    {
//                        PushMessage = _replyService.GetCommentReply(DataSource).Content.Replace(@"\r", "").Replace(@"\n", "");
//                    }
//                    else
//                    {
//                        PushMessage = _commentService.QuerySchoolComment(DataSource).Content.Replace(@"\r", "").Replace(@"\n", "");
//                    }
//                }
//                Total = _commentService.SchoolTotalComment(SchoolId);
//                ViewBag.isQuestion = false;
//            }
//            else if (type == ShareType.Question)
//            {

//                Total = _questionInfo.CurrentQuestionTotalBySchoolId(SchoolId)[0].Total;
//                ViewBag.isQuestion = true;
//            }
//            else if (type == ShareType.Answer)
//            {
//                var answer = _questionsAnswers.GetModelById(DataSource);
//                Total = answer.ReplyCount;
//                ViewBag.isQuestion = false;
//            }
//            else
//            {
//                var dto = await _questionInfo.GetQuestionById(DataSource, userId);
//                Total = dto.AnswerCount;
//                questionContent = dto.QuestionContent;
//                ViewBag.isQuestion = true;
//            }

//            string SchoolName = _schoolService.GetSchoolExtension(SchoolId).SchoolName;
//            shareContent = SchoolName;

//            shareLink = Request.Host.ToString();

//            if (type == ShareType.Comment)
//            {
//                Title = $"{user.Name} 邀请你为 {SchoolName}\\n进行点评";
//                shareContent = $"{Total}条评论\\n" + shareContent;
//                shareLink = "/SchoolComment/Index?SchoolId=" + SchoolId;
//            }
//            else if (type == ShareType.Reply)
//            {
//                Title = $"{user.Name} 邀请你为 {SchoolName}\\n的点评进行回复";
//                shareContent = $"{Total}条评论\\n" + shareContent;
//                shareLink = "/ReplyDetail/Index?type=1&DatascoureId=" + DataSource;
//            }
//            else if (type == ShareType.SuccessComment)
//            {
//                Title = $"{user.Name} 刚为 {SchoolName}\\n进行了点评，快来了解下吧!";
//                shareContent = $"{Total}条评论\\n" + shareContent;
//                shareLink = "/SchoolComment/CommentDetail?CommentId=" + DataSource;
//            }
//            else if (type == ShareType.Question)
//            {
//                Title = $"{user.Name} 邀请你提问 {SchoolName}";
//                shareContent = $"{Total}个回答\\n" + shareContent;
//                shareLink = "/Question/Index?SchoolId=" + SchoolId;
//            }
//            else if (type == ShareType.Answer)
//            {
//                Title = $"{user.Name} 邀请你回复：{questionContent}";
//                shareContent = $"{Total}个回复\\n" + shareContent;
//                shareLink = "/ReplyDetail/Index?type=2&DatascoureId=" + DataSource;
//                isQuestionAnswer = true;
//            }
//            else if (type == ShareType.SuccessQuestion)
//            {
//                Title = $"{user.Name} 邀请你回答：{questionContent}";
//                shareContent = $"{Total}个回答\\n" + shareContent;
//                shareLink = "/Question/QuestionDetail?QuestionId=" + DataSource;
//                isQuestionAnswer = true;
//                PushMessage = questionContent.Replace(@"\r", "").Replace(@"\n", "");
//            }
//            ViewBag.ShareTimeLineTitle = new Microsoft.AspNetCore.Html.HtmlString(Title);
//            ViewBag.ShareTitle = new Microsoft.AspNetCore.Html.HtmlString(Title);
//            ViewBag.ShareDesc = new Microsoft.AspNetCore.Html.HtmlString(shareContent);

//            var shareUrl = new UriBuilder
//            {
//                Scheme = HttpContext.Request.Scheme,
//                Host = HttpContext.Request.Host.Host,
//                Path = shareLink
//            };

//            ViewBag.ShareLink = shareLink;
//            ViewBag.SchoolId = SchoolId;
//            ViewBag.isQuestionAnswer = isQuestionAnswer;
//            ViewBag.DataSource = DataSource == default(Guid) ? SchoolId : DataSource;
//            ViewBag.type = (int)type;

//            int dataType = 0;
//            int inviteType = 0;
//            if (type == ShareType.Comment || type == ShareType.SuccessComment || type == ShareType.Reply)
//            {
//                if (type == ShareType.Comment)
//                {
//                    inviteType = (int)MessageType.InviteComment;
//                }
//                else
//                {
//                    inviteType = (int)MessageType.Reply;
//                }

//                dataType = (int)MessageDataType.Comment;
//            }
//            else if (type == ShareType.Question || type == ShareType.Answer || type == ShareType.SuccessQuestion)
//            {
//                if (type == ShareType.Question)
//                {
//                    inviteType = (int)MessageType.InviteQuesion;
//                }
//                else
//                {
//                    inviteType = (int)MessageType.InviteAnswer;
//                }

//                dataType = (int)MessageDataType.Question;
//            }

//            //当前已经成功邀请过的用户集合
//            ViewBag.InviteSuccess = _userService.CheckTodayInviteSuccessTotal(userId, SchoolId, DataSource == default(Guid) ? SchoolId : DataSource, inviteType, dataType, DateTime.Now);

//            ViewBag.PushMessage = PushMessage.Length > 20 ? PushMessage.Substring(0, 19) : PushMessage;
//            return View();
//        }
//    }
//}