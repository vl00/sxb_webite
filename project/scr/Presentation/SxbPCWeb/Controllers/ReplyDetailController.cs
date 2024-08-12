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
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Response;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Models.User;
using PMS.UserManage.Application.IServices;
using Sxb.PCWeb.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Sxb.PCWeb.Controllers
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

        
    }
}