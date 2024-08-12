using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.ViewModels.Question;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.PCWeb.Utils.CommentHelper
{
    /// <summary>
    /// 获取热门点评数据、热门学校
    /// </summary>
    public class PullHottestCSHelper
    {
        private readonly IGiveLikeService _likeservice;
        private readonly IUserService _userService;
        private readonly ImageSetting _setting;
        private readonly ISchoolInfoService _schoolInfoService;
        readonly ISchService _SchService;

        public PullHottestCSHelper(
            IGiveLikeService likeservice,
            IUserService userService,
            ISchoolInfoService schoolInfoService,
            ImageSetting setting,
            ISchService schService = null)
        {
            _likeservice = likeservice;
            _userService = userService;
            _setting = setting;
            _schoolInfoService = schoolInfoService;
            _SchService = schService;
        }

        /// <summary>
        /// 学校热门点评【分地区】
        /// </summary>
        /// <param name="CommentQuery"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<CommentInfoViewModel> HotComment(List<SchoolCommentDto> HotComment, Guid UserId)
        {
            if (HotComment == null || HotComment.Count == 0)
            {
                return new List<CommentInfoViewModel>();
            }
            else
            {
                var LikeComment = _likeservice.CheckLike(HotComment.GroupBy(x => x.Id).Select(x => x.Key).ToList(), UserId);
                HotComment.ForEach(x =>
                {
                    if (LikeComment.Contains(x.Id))
                    {
                        x.IsLike = true;
                    }
                    else
                    {
                        x.IsLike = false;
                    }
                });
            }

            //获取点评写入用户信息
            var UserIds = new List<Guid>();
            UserIds.AddRange(HotComment.GroupBy(x => x.UserId).Select(x => x.Key));
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            //热门点评分数实体
            var HottestScore = ScoreToStarHelper.CommentScoreToStar(HotComment.Select(x => x.Score)?.ToList());

            //获取学校信息
            var schoolExt = _schoolInfoService.GetSchoolName(HotComment.Select(x => x.SchoolSectionId).ToList());

            return CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, HotComment, UserHelper.UserDtoToVo(Users), HottestScore, SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolExt));
        }

        /// <summary>
        /// 热问
        /// </summary>
        /// <param name="questionDtos"></param>
        /// <returns></returns>
        public List<QuestionInfoViewModel> HotQuestion(List<QuestionDto> questionDtos)
        {
            if (questionDtos == null || questionDtos.Count == 0)
            {
                return new List<QuestionInfoViewModel>();
            }

            //获取学校信息
            var schoolExt = _schoolInfoService.GetSchoolName(questionDtos.Select(x => x.SchoolSectionId).ToList());

            return QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, questionDtos, new List<Models.User.UserInfoVo>(), SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(schoolExt));
        }

        /// <summary>
        /// 热评学校
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryAll">true：全国，false：指定类型的学校</param>
        /// <returns></returns>
        public List<SchoolCommentCardViewModel> HottestSchool(List<HotCommentSchoolDto> HottestSchool)
        {
            if (HottestSchool?.Any() == true)
            {
                if (_SchService != null)
                {
                    var ids = HottestSchool.Where(p => string.IsNullOrWhiteSpace(p.ShortSchoolNo)).Select(p => p.SchoolSectionId).ToList();
                    if (ids?.Any() == true)
                    {
                        var nos = _SchService.GetSchoolextNo(ids.ToArray());
                        if (nos?.Length > 0)
                        {
                            foreach (var item in nos)
                            {
                                var find = HottestSchool.FirstOrDefault(p => p.SchoolSectionId == item.Item1);
                                if (find != null)
                                {
                                    find.ShortSchoolNo = UrlShortIdUtil.Long2Base32(item.Item2).ToLower();
                                }
                            }
                        }
                    }
                }
                return SchoolDtoToSchoolCardHelper.HottestSchoolItem(HottestSchool);
            }
            else
            {
                return new List<SchoolCommentCardViewModel>();
            }
        }

        /// <summary>
        /// /热问学校
        /// </summary>
        /// <param name="HottestSchool"></param>
        /// <returns></returns>
        public List<SchoolQuestionCardViewModel> HottestQuestionItem(List<HotQuestionSchoolDto> HottestSchool)
        {
            return SchoolDtoToSchoolCardHelper.HottestQuestionSchoolItem(HottestSchool);
        }

    }
}
