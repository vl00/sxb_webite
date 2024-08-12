using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Aliyun;
using Sxb.UserCenter.Models.CommentViewModel;
using Sxb.UserCenter.Models.CommonViewModel;
using Sxb.UserCenter.Models.SchoolViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.Utils.DtoToViewModel;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    public class ApiCommentController : Base
    {
        private readonly ISchoolInfoService _schoolInfoService;
        private ICollectionService _collectionService;
        private readonly ImageSetting _setting;
        private ISchoolCommentService _commentService;
        private readonly IUserService _userService;
        private readonly ISearchService _dataSearch;
        public ApiCommentController(
            IText text,
            IMessageService inviteStatus,
            IAccountService account,
            ISchoolCommentService commentService,
            ICollectionService collectionService,
            IOptions<ImageSetting> set,
            IUserService userService,
            ISchoolInfoService schoolInfoService,
            ISearchService dataSearch)
        {
             _commentService = commentService;
            _setting = set.Value;
            _collectionService = collectionService;
            _userService = userService;
            _schoolInfoService = schoolInfoService;
            _dataSearch = dataSearch;
        }

        /// <summary>
        /// 个人列表页 点评列表
        /// </summary>
        /// <param name="search"></param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ResponseResult HomeComment(string search = "", Guid userId = default(Guid), int page = 1, int size = 10)
        {
            List<CommentInfoViewModel> comments = new List<CommentInfoViewModel>();

            Guid loginUserId = userID;
            bool IsSelf = userId == default || userId == loginUserId;
            Guid searchUserId = IsSelf ? loginUserId : userId;
       
            List<SchoolCommentDto> commentDtos = new List<SchoolCommentDto>();

            if (String.IsNullOrEmpty(search))
            {
                commentDtos = _commentService.PageCommentByUserId(searchUserId, loginUserId, page, size, IsSelf);
                if (!commentDtos.Any())
                {
                    return ResponseResult.Success(comments);
                }
            }
            else
            {
                //调用es 返回点评id

                //ES 模糊查询
                List<Guid> commentIds = new List<Guid>();
                if (!commentIds.Any()) 
                {
                    return ResponseResult.Success(comments);
                }
                commentDtos = _commentService.PageCommentByCommentIds(loginUserId, commentIds, IsSelf);
            }

            List<Guid> CommentIds = commentDtos.Select(x => x.Id).ToList(); 
            List<Guid> ExtIds = commentDtos.Select(x => x.SchoolSectionId).ToList();
            var SchoolExts = _schoolInfoService.GetSchoolStatuse(ExtIds);
            var Users = _userService.ListUserInfo(commentDtos.Select(x => x.UserId).ToList());
            var CommentScore = commentDtos.Select(x => x.Score).ToList();
            List<Guid> checkCollection = _collectionService.GetCollection(CommentIds, searchUserId);

            if (IsSelf)
            {
                commentDtos.ForEach(q => q.IsAnony = false);
            }

            comments = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, commentDtos, UserHelper.UserDtoToVo(Users), ScoreToStarHelper.CommentScoreToStar(CommentScore), SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(SchoolExts), checkCollection);

            return ResponseResult.Success(comments);
        }

    }
}
