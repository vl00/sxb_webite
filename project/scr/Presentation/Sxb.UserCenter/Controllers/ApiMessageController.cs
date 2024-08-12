using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using Sxb.UserCenter.Models.MessageViewModel;
using Sxb.UserCenter.Request;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.Utils.DtoToViewModel;
using PMS.UserManage.Domain.Common;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ProductManagement.Framework.Foundation;
using ProductManagement.API.Http.Result;
using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using PMS.CommentsManage.Domain.Common;
using PMS.Search.Application.IServices;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.OperationPlateform.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using Microsoft.Extensions.Logging;
using AutoMapper;
using PMS.UserManage.Application.ModelDto.ModelVo;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    public class ApiMessageController : Base
    {
        private IMessageService _message;
        private ISysMessageService _sysMessage;

        private readonly ISchoolCommentLikeMQService _commentLikeMQService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly IGiveLikeService _likeservice;
        private IQuestionInfoService _question;
        private IQuestionsAnswersInfoService _answersInfoService;
        private ISchoolCommentReplyService _replyService;
        private readonly IUserService _userService;

        private ITalentService _talentService;
        private IHistoryService _historyService;
        private ISchoolCommentService _commentService;
        private readonly ILiveServiceClient _liveServiceClient;
        private IAccountService _account;
        private IGiveLikeService _giveLikeService;
        private IArticleService _articleService;

        private ISearchService _searchService;

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ApiMessageController(
            IAccountService account,
            ISchoolCommentLikeMQService commentLikeMQService,
            ISchoolInfoService schoolInfoService,
            IMessageService message,
            IQuestionsAnswersInfoService answersInfoService,
            ISchoolCommentReplyService replyService,
            IUserService userService,
            ITalentService talentService,
            ISysMessageService sysMessage,
            IHistoryService historyService,
            IQuestionInfoService question,
            IGiveLikeService likeservice,
            ISchoolCommentService commentService, ISearchService searchService,
            ILiveServiceClient liveServiceClient, IGiveLikeService giveLikeService,
            IArticleService articleService, ILogger<ApiMessageController> logger, IMapper mapper)
        {

            _account = account;
            _commentLikeMQService = commentLikeMQService;
            _schoolInfoService = schoolInfoService;
            _likeservice = likeservice;
            _commentService = commentService;
            _historyService = historyService;
            _question = question;
            _talentService = talentService;
            _sysMessage = sysMessage;
            _message = message;
            _answersInfoService = answersInfoService;
            _replyService = replyService;
            _userService = userService;
            _liveServiceClient = liveServiceClient;
            _giveLikeService = giveLikeService;
            _searchService = searchService;
            _articleService = articleService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// 邀请我的【点评、提问、回答】
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public ResponseResult GetInviteMessages(int Page = 1, int Size = 10)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }
            //邀请
            var types = new List<byte>() { (byte)MessageType.InviteAnswer, (byte)MessageType.InviteComment,
                    (byte)MessageType.InviteQuestion }; //, (byte)MessageType.InviteReplyComment

            var message = _message.GetPrivateMessage(UserId, Page, Size, types, true);
            //更改消息状态 设置为已读
            _message.UpdateMessageState(types.Select(s => (int)s).ToList(), UserId);
            var res = MessageModelToViewModel.MessageModelToViewModels(message);

            var bind = _account.GetBindInfo(userID);

            return ResponseResult.Success(new { rows = res, isBindPhone = !string.IsNullOrEmpty(bind.Mobile) });
        }

        /// <summary>
        /// 我的邀请
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetMessageBySenderUser(int page = 1, int size = 10)
        {
            List<SelfInviteUserMessageViewModel> inviteViewModel = new List<SelfInviteUserMessageViewModel>();
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            var datas = _message.GetMessageBySenderUser(UserId, page, size);

            if (!datas.Any())
            {
                return ResponseResult.Success(inviteViewModel);
            }

            //update查阅状态
            //_message.UpdateReadState(false, default, UserId, datas.Select(x => x.DataId).ToList());
            //获取邀请的数据信息
            var users = _message.GetInviteUserInfos(datas.Select(x => x.DataId).ToList(), UserId);
            inviteViewModel = InviteSelfModelToViewModel.InviteSelfModelToViewModelHelper(datas, users);

            foreach (var item in inviteViewModel)
            {
                item.IsNewData = await _message.CheckMyInviteRedisData(UserId, item.DataId, (MessageType)item.Type);
            }
            return ResponseResult.Success(inviteViewModel);
        }

        /// <summary>
        /// 设置我的邀请列表为已读
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public ResponseResult SetReadMessageByDataId(Guid dataId, MessageType type)
        {
            Guid userId = userID;
            if (userId == Guid.Empty || dataId == Guid.Empty)
                return ResponseResult.Failed("请先登录");

            //update查阅状态
            _message.ClearMyInviteRedisData(userId, dataId, type);
            return ResponseResult.Success();
        }

        /// <summary>
        /// 获赞列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult GetLikeDataMsg(int page = 1, int size = 10)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            var data = _message.GetDataMsgs(UserId, MessageType.Like, null, ignore: false, page, size);
            if (!data.Any())
            {
                return ResponseResult.Success();
            }

            //更改消息状态 设置为已读
            _message.UpdateMessageState(new List<int>() { (int)MessageType.Like }, UserId);
            List<Guid> AnswerIds = data.Where(x => x.DataType == (int)MessageDataType.Answer).Select(x => x.DataId)?.ToList();
            List<AnswerReply> Answers = new List<AnswerReply>();
            List<ParentReply> Replys = new List<ParentReply>();
            List<Guid> UserIds = new List<Guid>();
            UserIds.AddRange(data.Where(x => x.DataType == (int)MessageDataType.Comment).Select(x => x.DataUserId)?.ToList());
            if (AnswerIds != null && AnswerIds.Any())
            {
                Answers = _answersInfoService.GetQuestionAnswerReplyByIds(AnswerIds);
                foreach (var item in Answers)
                {
                    if (item.AnswerUserId != Guid.Empty)
                    {
                        UserIds.Add(item.AnswerUserId);
                    }

                    if (item.QuestionUserId != Guid.Empty)
                    {
                        UserIds.Add(item.QuestionUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            List<Guid> ReplyIds = data.Where(x => x.DataType == (int)MessageDataType.CommentReply).Select(x => x.DataId)?.ToList();
            if (ReplyIds != null && ReplyIds.Any())
            {
                Replys = _replyService.GetParentReplyByIds(ReplyIds);
                foreach (var item in Replys)
                {
                    if (item.CommentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.CommentUserId);
                    }

                    if (item.ParentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ParentUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            var Users = _userService.ListUserInfo(UserIds);
            var res = LikeDataDtoToVoHelper.LikeDataDtosToVoHelper(data, UserHelper.UserDtoToVo(Users), Replys, Answers, new List<PMS.CommentsManage.Application.ModelDto.LikeCountDto>());
            return ResponseResult.Success(res);
        }

        /// <summary>
        /// 我的点赞列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult GetMyLike(int page = 1, int size = 10, int type = 0)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            var data = _message.GetMyLike(type, UserId, page, size);
            if (!data.Any())
            {
                return ResponseResult.Success();
            }

            var Ids = data.Select(x => x.DataId)?.ToList();
            var likeCounts = _commentService.GetLikeCount(Ids);


            List<Guid> AnswerIds = data.Where(x => x.DataType == (int)MessageDataType.Answer).Select(x => x.DataId)?.ToList();
            List<AnswerReply> Answers = new List<AnswerReply>();
            List<ParentReply> Replys = new List<ParentReply>();
            List<Guid> UserIds = new List<Guid>();

            UserIds.AddRange(data.Where(x => x.DataType == (int)MessageDataType.Comment).Select(x => x.DataUserId)?.ToList());

            if (AnswerIds != null && AnswerIds.Any())
            {
                Answers = _answersInfoService.GetQuestionAnswerReplyByIds(AnswerIds);
                foreach (var item in Answers)
                {
                    if (item.AnswerUserId != Guid.Empty)
                    {
                        UserIds.Add(item.AnswerUserId);
                    }

                    if (item.QuestionUserId != Guid.Empty)
                    {
                        UserIds.Add(item.QuestionUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            List<Guid> ReplyIds = data.Where(x => x.DataType == (int)MessageDataType.CommentReply).Select(x => x.DataId)?.ToList();
            if (ReplyIds != null && ReplyIds.Any())
            {
                Replys = _replyService.GetParentReplyByIds(ReplyIds);
                foreach (var item in Replys)
                {
                    if (item.CommentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.CommentUserId);
                    }

                    if (item.ParentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ParentUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            var Users = _userService.ListUserInfo(UserIds);
            var res = LikeDataDtoToVoHelper.LikeDataDtosToVoHelper(data, UserHelper.UserDtoToVo(Users), Replys, Answers, likeCounts);
            return ResponseResult.Success(res);
        }

        /// <summary>
        /// 回复我的
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult GetReplyMEDataMsg(int page = 1, int size = 10)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            var data = _message.GetReplyME(UserId, page, size);
            if (!data.Any())
            {
                return ResponseResult.Success();
            }

            //更改消息状态 设置为已读
            _message.UpdateMessageState(new List<int>() { (int)MessageType.Reply }, UserId);

            //回答
            List<Guid> AnswerIds = data.Where(x => x.DataType == (int)MessageDataType.Answer).Select(x => x.DataId)?.ToList();

            List<AnswerReply> Answers = new List<AnswerReply>();
            List<ParentReply> Replys = new List<ParentReply>();
            List<Guid> UserIds = new List<Guid>();
            if (AnswerIds != null && AnswerIds.Any())
            {
                Answers = _answersInfoService.GetQuestionAnswerReplyByIds(AnswerIds);
                foreach (var item in Answers)
                {
                    if (item.AnswerUserId != Guid.Empty)
                    {
                        UserIds.Add(item.AnswerUserId);
                    }

                    if (item.QuestionUserId != Guid.Empty)
                    {
                        UserIds.Add(item.QuestionUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            List<Guid> ReplyIds = data.Where(x => x.DataType == (int)MessageDataType.CommentReply).Select(x => x.DataId)?.ToList();
            if (ReplyIds != null && ReplyIds.Any())
            {
                Replys = _replyService.GetParentReplyByIds(ReplyIds);
                foreach (var item in Replys)
                {
                    if (item.CommentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.CommentUserId);
                    }

                    if (item.ParentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ParentUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            var Users = _userService.ListUserInfo(UserIds);
            var res = LikeDataDtoToVoHelper.LikeDataDtosToVoHelper(data, UserHelper.UserDtoToVo(Users), Replys, Answers, new List<PMS.CommentsManage.Application.ModelDto.LikeCountDto>());
            return ResponseResult.Success(res);
        }

        /// <summary>
        /// 我的回复
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult GetMyReply(int page = 1, int size = 10, int type = 0)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            var data = _message.GetMyReply(type, UserId, page, size);
            if (!data.Any())
            {
                return ResponseResult.Success();
            }

            //回答
            List<Guid> AnswerIds = data.Where(x => x.DataType == (int)MessageDataType.Answer).Select(x => x.DataId)?.ToList();

            List<AnswerReply> Answers = new List<AnswerReply>();
            List<ParentReply> Replys = new List<ParentReply>();
            List<Guid> UserIds = new List<Guid>();
            if (AnswerIds != null && AnswerIds.Any())
            {
                Answers = _answersInfoService.GetQuestionAnswerReplyByIds(AnswerIds);
                foreach (var item in Answers)
                {
                    if (item.AnswerUserId != Guid.Empty)
                    {
                        UserIds.Add(item.AnswerUserId);
                    }

                    if (item.QuestionUserId != Guid.Empty)
                    {
                        UserIds.Add(item.QuestionUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            List<Guid> ReplyIds = data.Where(x => x.DataType == (int)MessageDataType.CommentReply).Select(x => x.DataId)?.ToList();
            if (ReplyIds != null && ReplyIds.Any())
            {
                Replys = _replyService.GetParentReplyByIds(ReplyIds);
                foreach (var item in Replys)
                {
                    if (item.CommentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.CommentUserId);
                    }

                    if (item.ParentUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ParentUserId);
                    }

                    if (item.ReplyUserId != Guid.Empty)
                    {
                        UserIds.Add(item.ReplyUserId);
                    }
                }
            }

            var Users = _userService.ListUserInfo(UserIds);
            var res = LikeDataDtoToVoHelper.LikeDataDtosToVoHelper(data, UserHelper.UserDtoToVo(Users), Replys, Answers, new List<PMS.CommentsManage.Application.ModelDto.LikeCountDto>());
            return ResponseResult.Success(res);
        }


        /// <summary>
        /// 获取消息页 新消息统计
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetTipsTotal()
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }
            var total = _message.TotalTips(UserId);
            if (!total.Any())
            {
                return ResponseResult.Success(new TipsTotalViewModel());
            }

            var model = TipsToViewModelHelper.TipsToViewModel(total);
            return ResponseResult.Success(model);
        }

        /// <summary>
        /// 忽略邀请
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult IgnoreInvite([FromBody] InviteModel model)
        {
            if (model.IsAll == false && !model.Ids.Any())
            {
                return ResponseResult.Success();
            }

            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }
            _message.IgnoreInvite(model.Ids, UserId, model.IsAll);
            return ResponseResult.Success();
        }

        /// <summary>
        /// 添加系统消息
        /// </summary>
        /// <returns></returns>
        public ResponseResult AddSysMessage([FromBody] SysMessage message)
        {
            if (message == null)
            {
                return ResponseResult.Success(new
                {
                    success = false,
                    message = "并未接收到任何参数"
                });
            }

            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            message.SenderUserId = UserId;

            bool res = _sysMessage.AddSysMessage(new List<SysMessage>() { message });

            return ResponseResult.Success(new
            {
                success = res
            });
        }

        /// <summary>
        /// 消息列表页 消息提示
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetSysMessageTips(int Page = 1)
        {
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
            }

            var message = _sysMessage.GetSysMessageTips(UserId, Page);
            message = message.Where(s => s.Type != SysMessageType.Live || s.Content != "生成了直播课程回放").ToList();

            //获取消息后, 更新上次查询否有系统消息时间 为了接口ApiUser/GetMydynamic返回hasSysMeesage
            _sysMessage.RefreshSysMessageTime(userID);

            return ResponseResult.Success(MessageModelToViewModel.SysMessageTips(message));
        }

        /// <summary>
        /// 获取消息对话框消息内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetUserMessageDetail(Guid userId, int page = 1)
        {
            Guid thisUserID = userID;
            List<MessageDialogueViewModel> result = new List<MessageDialogueViewModel>();

            List<SysMessageDetail> messages = _sysMessage.GetSysMessages(senderId: userId, userId: thisUserID, page);
            //messages = messages.Where(s => s.Type != SysMessageType.Live || s.Content != "生成了直播课程回放").ToList();
            if (!messages.Any())
                return ResponseResult.Success(result);

            //普通消息已读【非文章,直播】
            _sysMessage.UpdateMessageRead(messages.Where(x => x.Type != SysMessageType.Article || x.Type == SysMessageType.Live).Select(x => x.DataId)?.ToList(), thisUserID);

            #region 文章/直播消息 已读
            List<Guid> MessageIds = messages.Where(x => x.Type == SysMessageType.Article || x.Type == SysMessageType.Live).Select(x => x.Id)?.ToList();
            if (MessageIds.Any())
            {
                //排除已读消息
                var temp = _sysMessage.CheckMessageIdIsExist(MessageIds, thisUserID);

                MessageIds = MessageIds.Where(x => !temp.Contains(x)).Select(x => x).ToList();

                //添加消息阅读
                _sysMessage.AddSysMessageState(MessageIds.Select(x => new SysMessageState() { MessageId = x, UserId = thisUserID })?.ToList());
            }
            #endregion

            SetUserMessageDetailViewModel(thisUserID, messages, ref result);
            return ResponseResult.Success(result);
        }

        [NonAction]
        private void SetUserMessageDetailViewModel(Guid searchUserId,List<SysMessageDetail> messages, ref List<MessageDialogueViewModel> result)
        {
            //问题回答内容
            List<AnswerInfoDto> answers = new List<AnswerInfoDto>();
            List<Guid> questionAnswersIds = messages.Where(x => x.Type == SysMessageType.InviteChange && x.DataType == MessageDataType.Answer).Select(x => x.DataId)?.ToList();
            if (questionAnswersIds != null && questionAnswersIds.Count > 0)
            {
                answers = _answersInfoService.GetAnswerInfoDtoByIds(questionAnswersIds, searchUserId);
            }

            //问题内容
            List<Guid> questionIds = messages.Where(x => x.Type == SysMessageType.InviteChange && x.DataType == MessageDataType.Question).Select(x => x.DataId)?.ToList();
            List<QuestionInfo> questions = _question.GetQuestionInfoByIds(questionIds);

            //点评
            List<Guid> commentIds = messages.Where(x => x.Type == SysMessageType.InviteChange && x.DataType == MessageDataType.Comment).Select(x => x.DataId)?.ToList();
            List<SchoolComment> comments = _commentService.GetCommentsByIds(commentIds);

            //文章
            List<Data> articleItems = new List<Data>();
            try
            {
                List<Guid> ArticleIds = messages.Where(x => x.Type == SysMessageType.Article).Select(x => x.DataId)?.ToList();
                articleItems = _historyService.ApiArticle(ArticleIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserMessageDetail-获取文章出错，ex：" + ex.Message);
            }
            //学校
            List<SchoolInfoDto> schools = _schoolInfoService.GetSchoolName(messages.Where(s => s.EId != null).Select(s => s.EId.Value).ToList());
            //var result = _mapper.Map<MessageDialogueViewModel>(messages);  
            result = MessageModelToViewModel.MessageDialogues(messages, articleItems, answers, questions, comments,  schools);
        }

        /// <summary>
        /// 获取个人动态列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [Description("个人动态列表")]
        [HttpGet]
        public async Task<ResponseResult> GetDynamic(string keywords, Guid userId = default, int page = 1, int size = 10)
        {
            //查询自己, 前端也可能传值userId
            bool IsSelf = userId == default || userId == userID;
            Guid searchUserId = IsSelf ? userID : userId;

            //动态列表id
            var items = new List<DynamicItem>();
            if (string.IsNullOrWhiteSpace(keywords))
            {
                items = _sysMessage.GetDynamicItems(searchUserId, page, size, IsSelf);
            }
            else
            {
                var searchResult = _searchService.SearchAll(keywords, 0, searchUserId, page, size);
                items = searchResult.List.Select(q => new DynamicItem
                {
                    Id = q.Id,
                    Type = (UserDynamicType)q.Type
                }).ToList();
            }

            List<Sxb.UserCenter.Models.CommentViewModel.DynamicTmepItem> tmepItems = new List<Sxb.UserCenter.Models.CommentViewModel.DynamicTmepItem>();

            //点评id
            List<Guid> commentIds = items.Where(x => x.Type == UserDynamicType.Comment).Select(x => x.Id).ToList();
            if (commentIds.Any())
            {
                var comments = _commentService.GetSchoolCommentByCommentId(commentIds, searchUserId);
                foreach (var item in comments)
                {
                    tmepItems.Add(new Models.CommentViewModel.DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.CreateTime.ConciseTime(),
                        Content = item.Content,
                        LikeCount = item.LikeCount,
                        ReplyCount = item.ReplyCount,
                        Type = (int)UserDynamicType.Comment,
                        Eid = item.SchoolSectionId,
                        No = UrlShortIdUtil.Long2Base32(item.No)
                    });
                }
            }

            //问题id
            List<Guid> questionIds = items.Where(x => x.Type == UserDynamicType.Question).Select(x => x.Id).ToList();
            if (questionIds.Any())
            {
                var questions = _question.GetQuestionByIds(questionIds, searchUserId);
                foreach (var item in questions)
                {
                    tmepItems.Add(new Models.CommentViewModel.DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.QuestionCreateTime.ConciseTime(),
                        Content = item.QuestionContent,
                        ReplyCount = item.AnswerCount,
                        Type = (int)UserDynamicType.Question,
                        Eid = item.SchoolSectionId,
                        No = UrlShortIdUtil.Long2Base32(item.No)
                    });
                }
            }

            //回答id
            List<Guid> answerIds = items.Where(x => x.Type == UserDynamicType.Answer).Select(x => x.Id).ToList();
            if (answerIds.Any())
            {
                var answers = _answersInfoService.GetAnswerInfoDtoByIds(answerIds, searchUserId);
                foreach (var item in answers)
                {
                    tmepItems.Add(new Models.CommentViewModel.DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.AddTime,
                        Content = item.AnswerContent,
                        LikeCount = item.LikeCount,
                        ReplyCount = item.ReplyCount,
                        Title = item.questionDto.QuestionContent,
                        Type = (int)UserDynamicType.Answer,
                        Eid = item.SchoolSectionId
                    });
                }
            }

            //文章id
            List<Guid> articleIds = items.Where(x => x.Type == UserDynamicType.Article).Select(x => x.Id).ToList();
            if (articleIds.Any())
            {

                var articles = this._articleService.GetArticles(articleIds); ;
                foreach (var item in articles)
                {
                    tmepItems.Add(new Models.CommentViewModel.DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.Time,
                        ViewCount = item.ViewCount,
                        Content = ArticleDataToVoHelper.RegexArticle(item.Html),
                        Title = item.Title,
                        Type = (int)UserDynamicType.Article
                    });
                }
            }

            //直播id
            List<Guid> liveIds = items.Where(x => x.Type == UserDynamicType.Live).Select(x => x.Id).ToList();
            if (liveIds.Any())
            {
                //调用api接口
                List<LiveResult> liveResults = new List<LiveResult>();

                var result = await _liveServiceClient.QueryLectures(liveIds, new Dictionary<string, string>());
                if (result?.Items != null && result.Items.Any())
                {
                    liveResults = result?.Items.Select(q => new LiveResult
                    {
                        Id = q.Id,
                        Title = q.Subject,
                        FrontCover = q.HomeCover,
                        AddTime = ((long)q.Time).I2D(),
                        ViewCount = q.Onlinecount
                    }).ToList();
                }

                foreach (var item in liveResults)
                {
                    tmepItems.Add(new Models.CommentViewModel.DynamicTmepItem()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        ViewCount = item.ViewCount,
                        AddTime = item.AddTime.ConciseTime(),
                        FrontCover = item.FrontCover,
                        Type = (int)UserDynamicType.Live
                    });
                }
            }

            //检测是否点赞
            var likes = _likeservice.CheckCurrentUserIsLikeBulk(searchUserId, tmepItems.Where(x => x.Type == 1 || x.Type == 2).Select(x => x.Id).ToList());

            //学校列表
            var schools = _schoolInfoService.GetSchoolName(tmepItems.GroupBy(q => q.Eid).Select(q => q.Key).ToList());

            tmepItems.ForEach(x =>
            {
                var temp = likes.Where(s => s.SourceId == x.Id).FirstOrDefault();
                if (temp != null)
                {
                    x.IsLike = temp.IsLike;
                }

                if (x.Eid != Guid.Empty)
                {
                    var school = schools.Where(s => s.SchoolSectionId == x.Eid).FirstOrDefault();
                    if (school != null)
                    {
                        x.SchoolName = school.SchoolName;
                    }
                }
            });

            //用户实体
            var UserDto = _userService.GetUserInfo(searchUserId);
            var UserVo = UserHelper.UserDtoToVo(new List<PMS.UserManage.Application.ModelDto.UserInfoDto>() { UserDto }).FirstOrDefault();

            tmepItems = items.Where(p => tmepItems.Any(q => q.Id == p.Id)).Select(q =>
            {
                return tmepItems.FirstOrDefault(p => p.Id == q.Id);
            }).ToList();

            var live = new Sxb.UserCenter.Models.CommentViewModel.Live();
            if (page == 1 && string.IsNullOrWhiteSpace(keywords))
            {
                try
                {
                    var livelist = await _liveServiceClient.LectorLectures(searchUserId, 4);
                    if (livelist.Items != null && livelist.Items.Any())
                    {
                        var res = livelist.Items.First();
                        live = new Models.CommentViewModel.Live
                        {
                            Id = res.Id,
                            Title = res.Subject,
                            ViewCount = res.Onlinecount
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("获取讲师直播接口出错，ex：", ex.Message);
                }
            }
            return ResponseResult.Success(new { user = UserVo, datas = tmepItems, live, isBindPhone = !string.IsNullOrEmpty(UserDto.Mobile) });
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="DataId"></param>
        /// <returns></returns>
        [Description("点赞")]
        public ResponseResult Like(Guid DataId, int LikeType)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                GiveLike giveLike = new GiveLike
                {
                    UserId = user.UserId,
                    LikeType = (PMS.CommentsManage.Domain.Common.LikeType)LikeType,
                    SourceId = DataId
                };
                _commentLikeMQService.SchoolCommentLike(giveLike);

                //_giveLikeService.AddLike(giveLike, out LikeStatus likeStatus);
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        //定时任务 定时推送消息
        public void PushInviteSysMessage()
        {
            //过去一天的消息邀请
            var startTime = DateTime.Now.AddDays(-1);
            //当前时间
            var endTime = DateTime.Now;
            PushInviteSysMessageByTime(startTime, endTime);
        }

        //定时任务 定时推送消息
        public void PushInviteSysMessageByTime(DateTime startTime, DateTime endTime)
        {
            //消息总数值
            int messageTotal = _message.GetMessageJobTotal(startTime, endTime);

            //每次取多少条，总分页
            int size = 100, pageTotal = (int)Math.Ceiling((messageTotal * 1.0) / 100);

            //导入提问回答消息
            for (int i = 1; i <= pageTotal; i++)
            {
                List<SysMessage> sysMessages = new List<SysMessage>();
                var messages = _message.GetMessageJob(startTime, endTime, i, size);

                var extIds = messages.Where(s => s.EID != null).Select(s => s.EID.Value).ToList();
                var userIds = messages.Select(s => s.userID).ToList();

                var questions = _question.GetQuestionsBySchoolUser(extIds, userIds).ToList();
                var comments = _commentService.GetSchoolCommentsBySchoolUser(extIds, userIds).ToList();
                foreach (var item in messages)
                {
                    var messageType = (MessageType)item.Type;
                    var msg = new SysMessage()
                    {
                        Id = Guid.NewGuid(),
                        SenderUserId = Guid.Parse("79AB946D-F6AE-4A31-9530-4B8C8EC7CA25"),
                        UserId = item.senderID,//发送给原来的邀请人
                        DataId = item.DataID,
                        OriContent = item.Content,
                        OriSenderUserId = item.senderID,
                        EId = item.EID,
                        Type = SysMessageType.InviteChange,
                        DataType = (MessageDataType?)item.DataType,
                        Content = "",
                        PushTime = item.ReadChangeTime ?? DateTime.Now,
                        OriType = messageType
                    };
                    msg.Content = "您的邀请有了新的反馈，点击查看>>";
                    if (messageType == MessageType.Reply || messageType == MessageType.InviteAnswer)
                    {
                        msg.Content = "您的回答邀请有了新的反馈，点击查看>>";
                    }
                    else if (messageType == MessageType.InviteQuestion)
                    {
                        msg.Content = "您的提问邀请有了新的反馈，点击查看>>";
                        var question = questions.Where(s => s.UserId == item.userID && s.SchoolSectionId == item.EID && s.CreateTime >= item.time).FirstOrDefault() ?? new QuestionInfo();
                        msg.DataId = question.Id;
                        msg.DataType = MessageDataType.Question;
                    }
                    else if(messageType == MessageType.InviteComment)
                    {
                        msg.Content = "您的点评邀请有了新的反馈，点击查看>>";
                        var comment = comments.Where(s => s.CommentUserId == item.userID && s.SchoolSectionId == item.EID && s.AddTime >= item.time).FirstOrDefault() ?? new SchoolComment();
                        msg.DataId = comment.Id;
                        msg.DataType = MessageDataType.Comment;
                    }
                    sysMessages.Add(msg);
                }
                _sysMessage.AddSysMessage(sysMessages);
            }
        }
    }

}
