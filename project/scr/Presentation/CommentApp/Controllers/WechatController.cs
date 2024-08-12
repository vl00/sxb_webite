using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sxb.Web.Common;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Application.IServices;
using PMS.School.Application.IServices;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Sxb.Web.Models.User;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using AutoMapper;
using PMS.UserManage.Application.ModelDto;
using Sxb.Web.Response;
using PMS.CommentsManage.Application.ModelDto;
using ProductManagement.Framework.Cache.Redis;
using System.Text.Encodings.Web;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.UserManage.Application.ModelDto.Talent;

namespace Sxb.Web.Controllers
{
    [Authorize]
    public class WechatController : BaseController
    {
        private readonly IMapper _mapper;
        private IEasyRedisClient _easyRedisClient;

        private readonly IUserService _userService;
        private readonly ITalentService _talentService;
        private readonly IMessageService _messageService;

        private ISchoolCommentService _commentService;
        private ISchoolService _schoolService;
        private ISchoolService _schoolInfoService;
        private ISchoolInfoService _schoolInfo;
        private IQuestionInfoService _questionInfo;
        private ISchoolCommentReplyService _replyService;
        private IQuestionsAnswersInfoService _questionsAnswers;

        public WechatController(ISchoolCommentService commentService,
            ISchoolService schoolService,
            IQuestionInfoService questionInfo,
            ISchoolService schoolInfoService,
            IUserService userService,
            IEasyRedisClient easyRedisClient,
            ISchoolCommentReplyService replyService,
            IQuestionsAnswersInfoService questionsAnswers,
            ISchoolInfoService schoolInfo,
            IMapper mapper, ITalentService talentService, IMessageService messageService)
        {
            _commentService = commentService;
            _schoolService = schoolService;
            _questionInfo = questionInfo;

            _easyRedisClient = easyRedisClient;
            _schoolInfoService = schoolInfoService;
            _mapper = mapper;

            _replyService = replyService;

            _schoolInfo = schoolInfo;

            _questionsAnswers = questionsAnswers;

            _userService = userService;
            _talentService = talentService;
            _messageService = messageService;
        }

        private void SetRecommendTotal(TalentRecommend recommend)
        {
            var path = Request.Path.Value;
            var variable = string.Join('|', new string[] { recommend.TheCityTalentSize.ToString(), recommend.OtherCityTalentSize.ToString(), recommend.TheSchoolUserSize.ToString() });
            Response.SetPageVariable(variable, path);
        }
        private void LoadRecommendTotal(TalentRecommend recommend)
        {
            var variable = Request.GetPageVariable();
            if (string.IsNullOrWhiteSpace(variable) || recommend.GroupIndex == 1)
            {
                return;
            }
            var totals = variable.Split('|');
            if (totals.Length == 3)
            {
                recommend.TheCityTalentSize = totals[0].ToLong();
                recommend.OtherCityTalentSize = totals[1].ToLong();
                recommend.TheSchoolUserSize = totals[2].ToLong();
            }
        }

        [Authorize]
        public ResponseResult GetPushUserInfo2(Guid schoolExtId, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                Guid loginUserId = default(Guid);
                if (User.Identity.IsAuthenticated)
                    loginUserId = User.Identity.GetUserInfo().UserId;

                int offset = (pageIndex - 1) * pageSize;
                //int groupSize = pageSize * 5;
                //int groupIndex = offset / groupSize + 1;

                int groupIndex = pageIndex;
                int groupSize = pageSize;

                int city = Request.GetLocalCity();
                var recommend = new TalentRecommend()
                {
                    CityCode = city,
                    LoginUserId = loginUserId,
                    SchoolExtId = schoolExtId,
                    GroupIndex = groupIndex,
                    GroupSize = groupSize,
                };
                LoadRecommendTotal(recommend);
                recommend = _talentService.GetRecommendTalents(recommend);
                SetRecommendTotal(recommend);

                var vm = recommend.Talents;
                //var vm = recommend.Talents.Skip(offset).Take(pageSize);
                return ResponseResult.Success(vm);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        public ResponseResult GetPushMessageUsers(Guid schoolExtId, MessageType messageType, Guid dataId)
        {
            MessageDataType messageDataType = GetMessageDataType(messageType);

            Guid loginUserId = User.Identity.GetUserInfo().UserId;
            var userIds = _messageService.GetMessageUserIds(loginUserId, null, dataId, messageType, messageDataType, schoolExtId);
            return ResponseResult.Success(userIds);
        }

        private static MessageDataType GetMessageDataType(MessageType messageType)
        {
            MessageDataType messageDataType;
            if (messageType == MessageType.InviteComment || messageType == MessageType.InviteQuestion)
            {
                messageDataType = MessageDataType.School;
            }
            else if (messageType == MessageType.InviteReplyComment)
            {
                messageDataType = MessageDataType.Comment;
            }
            else if (messageType == MessageType.InviteAnswer)
            {
                messageDataType = MessageDataType.Question;
            }
            else
            {
                messageDataType = MessageDataType.School;
            }
            return messageDataType;
        }

        /// <summary>
        /// 根据角色类型获取浏览过相同学校的用户信息
        /// </summary>
        /// <param name="SchoolExtId"></param>
        /// <param name="Role"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResponseResult> GetPushUserInfo(Guid SchoolExtId, int? Role, int type, Guid dataID, Guid eID, int PageIndex = 1, int PageSize = 10, bool invite = false)
        {
            if (Role == null)
                Role = 0;
            try
            {
                int dataType = 0;
                int inviteType = 0;
                int city = Request.GetLocalCity();
                if ((ShareType)type == ShareType.Comment || (ShareType)type == ShareType.SuccessComment || (ShareType)type == ShareType.Reply)
                {
                    if ((ShareType)type == ShareType.Comment)
                    {
                        inviteType = (int)MessageType.InviteComment;
                    }
                    else
                    {
                        inviteType = (int)MessageType.Reply;
                    }

                    dataType = (int)MessageDataType.Comment;
                }
                else if ((ShareType)type == ShareType.Question || (ShareType)type == ShareType.Answer || (ShareType)type == ShareType.SuccessQuestion)
                {
                    if ((ShareType)type == ShareType.Question)
                    {
                        inviteType = (int)MessageType.InviteQuestion;
                    }
                    else
                    {
                        inviteType = (int)MessageType.InviteAnswer;
                    }

                    dataType = (int)MessageDataType.Question;
                }

                Guid senderID = default(Guid);
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    senderID = user.UserId;
                }

                //获取相同类型学校，写入缓存
                string key = $"schoolSectionIdentical:Section:{SchoolExtId}&PageIndex:{PageIndex}&PageSize:{PageSize}";
                var SchoolExt = await _easyRedisClient.GetOrAddAsync(
                   key, () => { return _schoolInfoService.GetIdenticalSchool(SchoolExtId, PageIndex, PageSize); },
                    new TimeSpan(0, 30, 0));

                List<Guid> ExtIds = SchoolExt.Select(x => x.ExtId)?.ToList();
                List<InviteUserInfoVo> userInfoVos = new List<InviteUserInfoVo>();


                //获取用户信息，根据学校类型
                var UserDto = _userService.GetUserInfoByHistory(ExtIds, (UserRole)Role, PageIndex, PageSize, inviteType, dataType, senderID, dataID, SchoolExtId, senderID, city);
                //var UserDto = _userService.GetUserInfoByHistory(ExtIds, (UserRole)Role, PageIndex, PageSize, type, dataType, senderID, dataID, eID);
                userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(UserDto));
                if (userInfoVos.Count() == 0)
                {
                    userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(_userService.GetUserInfoByHistory(Role.Value, PageIndex, PageSize, inviteType, dataType, senderID, dataID, SchoolExtId, senderID, city)));
                }

                if ((userInfoVos.Count() == 0 && Role == 0) || userInfoVos.Count() < 10)
                {
                    if (PageSize == 10)
                    {
                        int totle = userInfoVos.Count() == 0 ? 10 : userInfoVos.Count() - 10;
                        userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(_userService.GetUserInfoByHistory(0, PageIndex, PageSize, inviteType, dataType, senderID, dataID, SchoolExtId, senderID, city)));
                    }
                }

                //邀请页面首先展示3位随机达人信息
                if (invite && PageIndex == 1)
                {
                    userInfoVos.AddRange(_mapper.Map<List<InviteUserInfoDto>, List<InviteUserInfoVo>>(_userService.GetUserInfoByHistory(1, 1, 3, inviteType, dataType, senderID, dataID, SchoolExtId, senderID, city)));
                }

                userInfoVos.Reverse();

                return ResponseResult.Success(userInfoVos);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        public ResponseResult GetOrdinaryUser(int PageIndex, int PageSize)
        {
            try
            {
                Guid userId = default(Guid);
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                var userInfos = _userService.GetOrdinaryUserInfo(PageIndex, PageSize, userId);
                return ResponseResult.Success(_mapper.Map<List<UserInfoDto>, List<UserInfoVo>>(userInfos));
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// true：点评 | false：问题
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IActionResult Index(ShareType type, Guid SchoolId = default(Guid), Guid DataSource = default(Guid), bool isReplyDetail = false)
        {
            Guid userId = Guid.Empty;
            var user = User.Identity.GetUserInfo();
            userId = user.UserId;


            ViewBag.Operation = (int)type;

            //是否为校方用户
            ViewBag.isSchoolRole = false;

            //为提问、回答
            bool isQuestionAnswer = false;


            //消息推送内容
            string PushMessage = "";
            string Title = "", shareContent = "", shareLink = "";
            int Total = 0;
            bool IsAnony = false;

            string SchoolName = _schoolInfo.GetSchoolName(new List<Guid>() { SchoolId }).FirstOrDefault()?.SchoolName;
            shareContent = SchoolName;
            shareLink = Request.Host.ToString();

            int messagePushType = 0, messagePushDataType = 0;
            bool isAnswerReply = false;
            if (type == ShareType.Comment || type == ShareType.SuccessComment || type == ShareType.Reply)
            {
                Total = _commentService.SchoolTotalComment(SchoolId);

                //点评
                if (type == ShareType.Comment)
                {
                    //邀请点评
                    Title = $"{user.Name} 邀请你为 {SchoolName}\\n进行点评";
                    shareContent = $"{Total}条评论\\n" + shareContent;
                    shareLink = $"/comment/school/{SchoolId}";

                    messagePushType = (int)MessageType.InviteComment;
                    messagePushDataType = (int)MessageDataType.School;
                }
                else if ((type == ShareType.SuccessComment || type == ShareType.Reply) && isReplyDetail == false)
                {
                    var comment = _commentService.QuerySchoolComment(DataSource);
                    //邀请回复点评
                    PushMessage = comment.Content.Replace(@"\r", "").Replace(@"\n", "");

                    Title = $"{user.Name} 邀请你为 {SchoolName}\\n的点评进行回复";
                    shareContent = $"{Total}条评论\\n" + shareContent;
                    shareLink = $"/comment/{ UrlShortIdUtil.Long2Base32(comment.No).ToLower()}.html";



                    IsAnony = comment.IsAnony;

                    messagePushType = (int)MessageType.InviteReplyComment;
                    messagePushDataType = (int)MessageDataType.Comment;
                }
                else if (isReplyDetail == true)
                {
                    var reply = _replyService.GetCommentReply(DataSource);
                    //邀请回复点评的回复
                    PushMessage = reply.Content.Replace(@"\r", "").Replace(@"\n", "");

                    Title = $"{user.Name} 邀请你为 {SchoolName}\\n的点评进行回复";
                    shareContent = $"{Total}条评论\\n" + shareContent;
                    shareLink = $"/comment/Reply/{DataSource}";

                    IsAnony = reply.IsAnony;

                    messagePushType = (int)MessageType.InviteReplyComment;
                    messagePushDataType = (int)MessageDataType.Comment;
                }
                ViewBag.isQuestion = false;
            }
            else
            {
                ViewBag.isQuestion = false;
                //问答
                if (type == ShareType.Question)
                {
                    //邀请提问
                    ViewBag.isQuestion = true;
                    Total = _questionInfo.CurrentQuestionTotalBySchoolId(SchoolId)[0].Total;

                    Title = $"{user.Name} 邀请你提问 {SchoolName}";
                    shareContent = $"{Total}个回答\\n" + shareContent;
                    shareLink = $"/question/school/{SchoolId}";

                    messagePushType = (int)MessageType.InviteQuestion;
                    messagePushDataType = (int)MessageDataType.School;
                }
                else if ((type == ShareType.SuccessQuestion || type == ShareType.Answer) && isReplyDetail == false)
                {
                    //邀请回答
                    var answer = _questionInfo.GetQuestionById(DataSource, userId);
                    Total = answer.AnswerCount;
                    PushMessage = answer.QuestionContent;
                    ViewBag.isQuestion = false;

                    Title = $"{user.Name} 邀请你回答：{PushMessage}";
                    shareContent = $"{Total}个回答\\n" + shareContent;
                    shareLink = $"/question/{ UrlShortIdUtil.Long2Base32(answer.No).ToLower()}.html";
                    isQuestionAnswer = true;
                    PushMessage = PushMessage.Replace(@"\r", "").Replace(@"\n", "");

                    IsAnony = answer.IsAnony;
                    messagePushType = (int)MessageType.InviteAnswer;
                    messagePushDataType = (int)MessageDataType.Question;
                }
                else if (isReplyDetail == true)
                {
                    //邀请回复问题的回答
                    var answer = _questionsAnswers.GetModelById(DataSource);
                    Total = answer.ReplyCount;
                    PushMessage = answer.Content;
                    ViewBag.isQuestion = false;

                    Title = $"{user.Name} 邀请你回复：{PushMessage}";
                    shareContent = $"{Total}个回复\\n" + shareContent;
                    shareLink = $"/question/Reply/{DataSource}";
                    isQuestionAnswer = true;

                    isAnswerReply = true;
                    IsAnony = answer.IsAnony;

                    messagePushType = (int)MessageType.InviteAnswer;
                    messagePushDataType = (int)MessageDataType.Question;
                }
            }
            #region
            /*
            if (type == ShareType.Comment || type == ShareType.SuccessComment || type == ShareType.Reply)
            {
                if (type != ShareType.Comment)
                {
                    if (type == ShareType.Reply && isReplyDetail)
                    {
                    }
                    else
                    {
                    }
                }
                
                
            }
            else if (type == ShareType.Question)
            {

                
            }
            else if (type == ShareType.Answer)
            {
                
            }
            else
            {
                if (!isReplyDetail)
                {
                    var dto = await _questionInfo.GetQuestionById(DataSource, userId);
                    Total = dto.AnswerCount;
                    questionContent = dto.QuestionContent;
                }
                else 
                {
                    var answerDto = _questionsAnswers.GetModelById(DataSource);
                    Total = answerDto.ReplyCount;
                    questionContent = answerDto.Content;
                }
                ViewBag.isQuestion = true;
            }
            

            if (type == ShareType.Comment)
            {
                
            }
            else if (type == ShareType.Reply)
            {
                
            }
            else if (type == ShareType.SuccessComment)
            {
                
            }
            else if (type == ShareType.Question)
            {
                
            }
            else if (type == ShareType.Answer)
            {
                
            }
            else if (type == ShareType.SuccessQuestion)
            {
                Title = $"{user.Name} 邀请你回答：{questionContent}";
                shareContent = $"{Total}个回答\\n" + shareContent;
                shareLink = "/Question/QuestionDetail?QuestionId=" + DataSource;
                isQuestionAnswer = true;
                PushMessage = questionContent.Replace(@"\r", "").Replace(@"\n", "");
            }
            */
            #endregion

            ViewBag.ShareTimeLineTitle = new Microsoft.AspNetCore.Html.HtmlString(Title);
            ViewBag.ShareTitle = new Microsoft.AspNetCore.Html.HtmlString(Title);
            ViewBag.ShareDesc = new Microsoft.AspNetCore.Html.HtmlString(shareContent);

            var shareUrl = new UriBuilder
            {
                Scheme = HttpContext.Request.Scheme,
                Host = HttpContext.Request.Host.Host,
                Path = shareLink
            };

            ViewBag.ShareLink = shareLink;
            ViewBag.SchoolId = SchoolId;
            ViewBag.isQuestionAnswer = isQuestionAnswer;
            ViewBag.DataSource = DataSource == default(Guid) ? SchoolId : DataSource;
            ViewBag.type = messagePushType;
            ViewBag.DataType = messagePushDataType;

            //是否未匿名
            ViewBag.IsAnony = IsAnony;

            if (isAnswerReply)
            {
                ViewBag.isComment = "回复";
            }
            else
            {
                ViewBag.isComment = type != 0 ? type.Description() : "";
            }

            //当前已经成功邀请过的用户集合
            ViewBag.InviteSuccess = _userService.CheckTodayInviteSuccessTotal(userId, SchoolId, DataSource == default(Guid) ? SchoolId : DataSource, messagePushType, messagePushDataType, DateTime.Now);

            PushMessage = PushMessage.Length > 20 ? PushMessage.Substring(0, 19) : PushMessage;
            //兼容前端使用get传参
            ViewBag.PushMessage = PushMessage.Replace("&", "");
            return View();
        }

    }
}