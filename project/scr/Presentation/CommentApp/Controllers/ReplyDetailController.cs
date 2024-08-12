using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto.Reply;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Replay;
using Sxb.Web.Response;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models.User;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using PMS.UserManage.Domain.Common;
using System.ComponentModel;
using ProductManagement.Infrastructure.Exceptions;

namespace Sxb.Web.Controllers
{
    public class ReplyDetailController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ISchoolCommentReplyService _commentReplyService;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionsAnswersInfoService _answerService;
        private readonly IGiveLikeService _likeService;
        private readonly IMapper _mapper;
        private readonly ImageSetting _setting;
        public ReplyDetailController(IOptions<ImageSetting> set, IUserService userService,
             ISchoolCommentReplyService commentReplyService, ISchoolCommentService commentService,
              IQuestionsAnswersInfoService answerService,IGiveLikeService likeService, IMapper mapper)
        {
            _userService = userService;
            _commentService = commentService;
            _commentReplyService = commentReplyService;
            _answerService = answerService;
            _likeService = likeService;
            _mapper = mapper;
            _setting = set.Value;
        }

        /// <summary>
        /// 回复详情页
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="type">1：点评 2：问答</param>
        [Description("回复详情页")]
        public IActionResult Index(Guid DatascoureId, int type)
        {
            if (DatascoureId == null || DatascoureId == Guid.Empty)
            {
                throw new NotFoundException();
            }
            else
            {
                string redirectUrl = "";
                if (type == 1)
                {
                    redirectUrl = $"/comment/Reply/{DatascoureId}";
                }
                else 
                {
                    redirectUrl = $"/question/Reply/{DatascoureId}";
                }
                Response.Redirect(redirectUrl);
                Response.StatusCode = 301;
                return Json(new { });
            }
            #region
            //Guid userId = Guid.Empty;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    userId = user.UserId;
            //}


            //ViewBag.DatascoureId = DatascoureId;
            //ViewBag.Type = type;
            //var isLike = _likeService.CheckCurrentUserIsLike(userId, DatascoureId);


            ////检测当前用户身份 （true：校方用户 |false：普通用户）
            //ViewBag.isSchoolRole = false;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
            //}
            //ViewBag.Type = type;

            //if (type == 1)
            //{
            //    var commentReplyId = DatascoureId;
            //    var comment = _commentReplyService.GetCommentReply(commentReplyId);

            //    if(comment == null) 
            //    {
            //        throw new NotFoundException();
            //    }
            //    _commentReplyService.UpdateReplyViewCount(commentReplyId);

            //    var hottestCommentReply = _commentReplyService.GetSchoolCommentHottestReplies(commentReplyId);

            //    List<Guid> userIds = hottestCommentReply.Select(x => x.UserId)?.ToList();
            //    userIds.Add(comment.UserId);

            //    var users = _userService.ListUserInfo(userIds);

            //    var replyUser = users.FirstOrDefault(q => q.Id == comment.UserId);

            //    var reply = new ReplyDetailViewModel {
            //        Id = comment.Id,
            //        DatascoureId = comment.SchoolCommentId,
            //        SchoolSectionId = comment.SchoolComment.SchoolSectionId,
            //        Content = comment.Content,
            //        IsLike = isLike,
            //        IsAttend = comment.IsAttend,
            //        IsSchoolPublish = false,
            //        RumorRefuting = false,
            //        ReplyCount = comment.ReplyCount,
            //        LikeCount = comment.LikeCount,
            //        CreateTime = comment.CreateTime.ConciseTime(),
            //        UserInfo = UserHelper.ToAnonyUserName(new UserInfoVo
            //        {
            //            Id = comment.UserId,
            //            NickName = replyUser.NickName,
            //            HeadImager = replyUser.HeadImgUrl,
            //            Role = replyUser.VerifyTypes.ToList()
            //        }, comment.IsAnony)
            //    };


            //    List<Guid> replyIds = hottestCommentReply.GroupBy(q => q.Id).Select(s => s.Key).ToList();
            //    var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
            //    var hottestReplyViewModel = hottestCommentReply.Select(q => new ReplyViewModel
            //    {
            //        Id = q.Id,
            //        Content = q.Content,
            //        DatascoureId = q.SchoolCommentId,
            //        ReplyId = q.ReplyId,
            //        IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
            //        IsTop = q.IsTop,
            //        IsAttend = q.IsAttend,
            //        IsSchoolPublish = q.IsSchoolPublish,
            //        RumorRefuting = q.RumorRefuting,
            //        ReplyCount = q.ReplyCount,
            //        LikeCount = q.LikeCount,
            //        UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
            //        CreateTime = q.CreateTime.ConciseTime()
            //    }).ToList();

            //    ViewBag.replyHottest = hottestReplyViewModel;
            //    ViewBag.replyDetail = reply;
            //}
            //else
            //{
            //    var answerId = DatascoureId;
            //    var answer = _answerService.QueryAnswerInfo(answerId);

            //    if (answer == null)
            //    {
            //        throw new NotFoundException();
            //    }
            //    _answerService.UpdateAnswerViewCount(answerId);

            //    var hottestAnswerReply = _answerService.GetAnswerHottestReplys(answerId);

            //    List<Guid> userIds = hottestAnswerReply.Select(x => x.UserId)?.ToList();
            //    userIds.Add(answer.UserId);

            //    var users = _userService.ListUserInfo(userIds);

            //    var replyUser = users.FirstOrDefault(q => q.Id == answer.UserId);


            //    var reply = new ReplyDetailViewModel
            //    {
            //        Id = answer.Id,
            //        DatascoureId = answer.QuestionId,
            //        SchoolSectionId = answer.SchoolSectionId,
            //        Content = answer.AnswerContent,
            //        IsLike = isLike,
            //        IsAttend = answer.IsAttend,
            //        IsSchoolPublish = answer.IsSchoolPublish,
            //        RumorRefuting = answer.RumorRefuting,
            //        ReplyCount = answer.ReplyCount,
            //        LikeCount = answer.LikeCount,
            //        CreateTime = answer.AddTime,
            //        UserInfo = new UserInfoVo
            //        {
            //            Id = answer.UserId,
            //            NickName = replyUser.NickName,
            //            HeadImager = replyUser.HeadImgUrl,
            //            Role = replyUser.VerifyTypes.ToList()
            //        }.ToAnonyUserName(answer.IsAnony)
            //    };


            //    List<Guid> replyIds = hottestAnswerReply.GroupBy(q => q.Id).Select(s => s.Key).ToList();
            //    var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
            //    var hottestAnswerReplyViewModel =  hottestAnswerReply.Select(q => new ReplyViewModel
            //    {
            //        Id = q.Id,
            //        Content = q.AnswerContent,
            //        DatascoureId = q.QuestionId,
            //        ReplyId = q.ParentId,
            //        IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
            //        IsTop = false,
            //        IsAttend = q.IsAttend,
            //        IsSchoolPublish = q.IsSchoolPublish,
            //        RumorRefuting = q.RumorRefuting,
            //        ReplyCount = q.ReplyCount,
            //        LikeCount = q.LikeCount,
            //        UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
            //        CreateTime = q.AddTime
            //    }).ToList();

            //    ViewBag.replyHottest = hottestAnswerReplyViewModel;

            //    ViewBag.replyDetail = reply;
            //}
            //return View();
            #endregion
        }

        [HttpGet]
        [Description("回复列表")]
        public ResponseResult PageReply(Guid DatascoureId, int PageIndex, int PageSize, int type,int ordertype)
        {
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }

            if (type == 1)
            {
                var replyId = DatascoureId;

                var commentReplies =  _commentReplyService.PageCommentReplyById(replyId, ordertype, PageIndex, PageSize,out int total);
                if (commentReplies == null)
                {
                    throw new NotFoundException();
                }

                List<Guid> replyIds = commentReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();
                var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
                List<Guid> userIds = commentReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                var users = _userService.ListUserInfo(userIds);
                List<Guid> parentUserIds = commentReplies.Where(p => p.ReplyId != null).GroupBy(q => q.ReplyUserId).Select(s => (Guid)s.Key).ToList();
                var parentUsers = _userService.ListUserInfo(parentUserIds);



                return ResponseResult.Success(new
                {
                    rows = commentReplies.Select(q => new ReplyViewModel {
                        Id = q.Id,
                        Content = q.Content,
                        DatascoureId = q.SchoolCommentId,
                        ReplyId = q.ReplyId,
                        IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                        IsTop = q.IsTop,
                        IsAttend = q.IsAttend,
                        IsSchoolPublish = q.IsSchoolPublish,
                        RumorRefuting = q.RumorRefuting,
                        ReplyCount = q.ReplyCount,
                        LikeCount = q.LikeCount,
                        ReplyUserInfo = _mapper.Map<UserInfoVo>(parentUsers.FirstOrDefault(p => p.Id == q.ReplyUserId)).ToAnonyUserName(q.ReplyUserIdIsAnony),
                        UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
                        CreateTime = q.CreateTime.ConciseTime()
                    })
                });

            }
            else
            {
                var answerId = DatascoureId;

                var answerReplies = _answerService.PageAnswerReply(answerId, userId, ordertype, PageIndex, PageSize);
                if (answerReplies == null)
                {
                    throw new NotFoundException();
                }


                List<Guid> replyIds = answerReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();
                var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);

                List<Guid> userIds = answerReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                var users = _userService.ListUserInfo(userIds);
                List<Guid> parentUserIds =  answerReplies.Where(p => p.ParentId != null).GroupBy(q => q.ParentUserId).Select(s => (Guid)s.Key).ToList();
                var parentUsers = _userService.ListUserInfo(parentUserIds);

                return ResponseResult.Success(new
                {
                    rows = answerReplies.Select(q => new ReplyViewModel
                    {
                        Id = q.Id,
                        Content = q.AnswerContent,
                        DatascoureId = q.QuestionId,
                        ReplyId = q.ParentId,
                        IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                        IsTop =false,
                        IsAttend = q.IsAttend,
                        IsSchoolPublish = q.IsSchoolPublish,
                        RumorRefuting = q.RumorRefuting,
                        ReplyCount = q.ReplyCount,
                        LikeCount = q.LikeCount,
                        ReplyUserInfo = _mapper.Map<UserInfoVo>(parentUsers.FirstOrDefault(p => p.Id == q.ParentUserId)).ToAnonyUserName(q.ParentUserIdIsAnony),
                        UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
                        CreateTime = q.AddTime
                    })
                });
            }
        }



        /// <summary>
        /// 查看对话页面
        /// </summary>
        [Description("查看对话页面")]
        public IActionResult QueryDialog(Guid ReplyId, int type)
        {
            Guid userId = Guid.Empty;
            //bool isSchoolRole = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }
            ViewBag.ReplyId = ReplyId;
            ViewBag.Type = type;

            //检测是否为校方用户
            ViewBag.isSchoolRole = false;


            if (type == 1)
            {
                var commentReplies = _commentReplyService.PageDialog(ReplyId, 1, 10);
                List<Guid> replyIds = commentReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();
                var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
                List<Guid> userIds = commentReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                var users = _userService.ListUserInfo(userIds);
                List<Guid> parentUserIds = commentReplies.Where(p => p.ReplyId != null).GroupBy(q => q.ReplyUserId).Select(s => (Guid)s.Key).ToList();
                var parentUsers = _userService.ListUserInfo(parentUserIds);

                _commentReplyService.UpdateReplyViewCount(ReplyId);

                var reply = commentReplies.Select(q => new ReplyViewModel
                {
                    Id = q.Id,
                    Content = q.Content,
                    DatascoureId = q.SchoolCommentId,
                    ReplyId = q.ReplyId,
                    IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                    IsTop = q.IsTop,
                    IsAttend = q.IsAttend,
                    IsSchoolPublish = q.IsSchoolPublish,
                    RumorRefuting = q.RumorRefuting,
                    ReplyCount = q.ReplyCount,
                    LikeCount = q.LikeCount,
                    ReplyUserInfo = _mapper.Map<UserInfoVo>(parentUsers.FirstOrDefault(p => p.Id == q.ReplyUserId)).ToAnonyUserName(q.ReplyUserIdIsAnony),
                    UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
                    CreateTime = q.CreateTime.ConciseTime()
                });
                ViewBag.Replys = reply;
                ViewBag.LoadEnd = reply.Count() < 10;
            }
            else
            {
                var answerReplies =  _answerService.PageDialog(ReplyId, userId, 1, 10);
                List<Guid> replyIds = answerReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();
                var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
                List<Guid> userIds = answerReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                var users = _userService.ListUserInfo(userIds);
                List<Guid> parentUserIds = answerReplies.Where(p => p.ParentId != null).GroupBy(q => q.ParentUserId).Select(s => (Guid)s.Key).ToList();
                var parentUsers = _userService.ListUserInfo(parentUserIds);

                _answerService.UpdateAnswerViewCount(ReplyId);

                var reply = answerReplies.Select(q => new ReplyViewModel
                {
                    Id = q.Id,
                    Content = q.AnswerContent,
                    DatascoureId = q.QuestionId,
                    ReplyId = q.ParentId,
                    IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                    IsTop = false,
                    IsAttend = q.IsAttend,
                    IsSchoolPublish = q.IsSchoolPublish,
                    RumorRefuting = q.RumorRefuting,
                    ReplyCount = q.ReplyCount,
                    LikeCount = q.LikeCount,
                    ReplyUserInfo = _mapper.Map<UserInfoVo>(parentUsers.FirstOrDefault(p => p.Id == q.ParentUserId)).ToAnonyUserName(q.ParentUserIdIsAnony),
                    UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
                    CreateTime = q.AddTime
                });
                ViewBag.Replys = reply;
                ViewBag.LoadEnd = reply.Count() < 10;
            }
            return View();
        }

        /// <summary>
        /// 查看对话列表
        /// </summary>
        /// <param name="type">1：点评 2：问答</param>
        [Description("查看对话列表")]
        public ResponseResult PageDialog(Guid ReplyId, int PageIndex, int PageSize, int type)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }
                if (type == 1)
                {
                    var commentReplies = _commentReplyService.PageDialog(ReplyId, PageIndex, PageSize);
                    List<Guid> replyIds = commentReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();
                    var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
                    List<Guid> userIds = commentReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                    var users = _userService.ListUserInfo(userIds);
                    List<Guid> parentUserIds = commentReplies.Where(p => p.ReplyId != null).GroupBy(q => q.ReplyUserId).Select(s => (Guid)s.Key).ToList();
                    var parentUsers = _userService.ListUserInfo(parentUserIds);

                    var reply = commentReplies.Select(q => new ReplyViewModel
                    {
                        Id = q.Id,
                        Content = q.Content,
                        DatascoureId = q.SchoolCommentId,
                        ReplyId = q.ReplyId,
                        IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                        IsTop = q.IsTop,
                        IsAttend = q.IsAttend,
                        IsSchoolPublish = q.IsSchoolPublish,
                        RumorRefuting = q.RumorRefuting,
                        ReplyCount = q.ReplyCount,
                        LikeCount = q.LikeCount,
                        ReplyUserInfo = _mapper.Map<UserInfoVo>(parentUsers.FirstOrDefault(p => p.Id == q.ReplyUserId)).ToAnonyUserName(q.ReplyUserIdIsAnony),
                        UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
                        CreateTime = q.CreateTime.ConciseTime()
                    });
                    return ResponseResult.Success(new { 
                        rows = reply
                    });
                }
                else
                {
                    var answerReplies = _answerService.PageDialog(ReplyId, userId, PageIndex, PageSize);
                    List<Guid> replyIds = answerReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();
                    var isLikes = _likeService.CheckCurrentUserIsLikeBulk(userId, replyIds);
                    List<Guid> userIds = answerReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                    var users = _userService.ListUserInfo(userIds);
                    List<Guid> parentUserIds = answerReplies.Where(p => p.ParentId != null).GroupBy(q => q.ParentUserId).Select(s => (Guid)s.Key).ToList();
                    var parentUsers = _userService.ListUserInfo(parentUserIds);

                    var reply = answerReplies.Select(q => new ReplyViewModel
                    {
                        Id = q.Id,
                        Content = q.AnswerContent,
                        DatascoureId = q.QuestionId,
                        ReplyId = q.ParentId,
                        IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                        IsTop = false,
                        IsAttend = q.IsAttend,
                        IsSchoolPublish = q.IsSchoolPublish,
                        RumorRefuting = q.RumorRefuting,
                        ReplyCount = q.ReplyCount,
                        LikeCount = q.LikeCount,
                        ReplyUserInfo = _mapper.Map<UserInfoVo>(parentUsers.FirstOrDefault(p => p.Id == q.ParentUserId)).ToAnonyUserName(q.ParentUserIdIsAnony),
                        UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
                        CreateTime = q.AddTime
                    });
                    return ResponseResult.Success(new
                    {
                        rows = reply
                    });
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }
          
        
    }
}