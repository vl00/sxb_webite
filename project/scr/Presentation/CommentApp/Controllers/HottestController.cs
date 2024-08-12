using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.Live.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.IServices;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models;
using Sxb.Web.Response;
using Sxb.Web.ViewModels.Hottest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    public class HottestController : BaseController
    {
        ISchoolCommentReplyService _schoolCommentReplyService;
        IArticleCoverService _articleCoverService;
        IInviteStatusService _inviteStatusService;
        IGiveLikeService _giveLikeService;
        ISchoolInfoService _schoolInfoService;
        ICircleService _circleService;
        IQuestionInfoService _questionInfoService;
        ISchoolCommentService _schoolCommentService;
        IArticleService _articleService;
        ITopicService _topicService;
        ILiveServiceClient _liveServiceClient;
        IEasyRedisClient _easyRedisClient;
        ISchoolService _schoolService;
        IUserService _userService;
        ISchoolImageService _schoolImageService;
        ITalentService _talentService;
        ISysMessageService _sysMessageService;
        IQuestionsAnswersInfoService _questionsAnswersInfoService;
        ITopicReplyService _topicReplyService;
        ILectureService _lectureService;
        IMessageService _messageService;
        ImageSetting _imageSetting;

        public HottestController(IQuestionInfoService questionInfoService, ISchoolCommentService schoolCommentService, IArticleService articleService,
            ITopicService topicService, ILiveServiceClient liveServiceClient, IEasyRedisClient easyRedisClient, ISchoolService schoolService, IUserService userService
            , ICircleService circleService, ISchoolImageService schoolImageService, ITalentService talentService, ISysMessageService sysMessageService,
            IQuestionsAnswersInfoService questionsAnswersInfoService, ISchoolInfoService schoolInfoService, IGiveLikeService giveLikeService,
            IInviteStatusService inviteStatusService, IArticleCoverService articleCoverService, ITopicReplyService topicReplyService,
            ISchoolCommentReplyService schoolCommentReplyService, ILectureService lectureService, IMessageService messageService, IOptions<ImageSetting> set)
        {
            _messageService = messageService;
            _lectureService = lectureService;
            _schoolCommentReplyService = schoolCommentReplyService;
            _topicReplyService = topicReplyService;
            _articleCoverService = articleCoverService;
            _inviteStatusService = inviteStatusService;
            _giveLikeService = giveLikeService;
            _schoolInfoService = schoolInfoService;
            _questionsAnswersInfoService = questionsAnswersInfoService;
            _sysMessageService = sysMessageService;
            _talentService = talentService;
            _schoolImageService = schoolImageService;
            _circleService = circleService;
            _userService = userService;
            _schoolService = schoolService;
            _questionInfoService = questionInfoService;
            _schoolCommentService = schoolCommentService;
            _articleService = articleService;
            _topicService = topicService;
            _liveServiceClient = liveServiceClient;
            _easyRedisClient = easyRedisClient;
            _imageSetting = set.Value;
        }

        /// <summary>
        /// 分页获取站内热门
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("分页获取站内热门")]
        public async Task<ResponseResult> Page(int offset = 0, int limit = 10)
        {
            var finds = await _easyRedisClient.SortedSetRangeByRankAsync<SortSetItem>("SortedHottest:Union", offset, offset + limit - 1, StackExchange.Redis.Order.Descending);
            if (finds?.Any() == true)
            {
                var result = new List<HottestPageItem>();
                foreach (var find in finds)
                {
                    var item = new HottestPageItem();
                    switch (find.Type)
                    {
                        case "Question":
                            var question = _questionInfoService.GetQuestionInfoById(find.ID);
                            if (question.Item1 != null)
                            {
                                item.Type = HottestPageItemType.Question;
                                item.TargetID = question.Item1.Id;
                                item.Content = question.Item1.QuestionContent;
                                item.CreateTime = question.Item1.QuestionCreateTime;
                                item.Images = question.Item1.Images;
                                item.ShortID = UrlShortIdUtil.Long2Base32(question.Item1.No);
                                var images = _schoolImageService.GetImageByDataSourceId(question.Item1.Id).ToList();
                                if (images?.Any() == true)
                                {
                                    item.Images = images.Take(3).Select(p => $"{_imageSetting.QueryImager}{p.ImageUrl}").ToList();
                                    item.ImageCount = images.Count;
                                }

                                var school = _schoolService.GetSchoolExtensionById(question.Item1.SchoolId, question.Item1.SchoolSectionId);
                                if (school != null)
                                {
                                    item.SchoolName = school.SchoolName;
                                }
                                item.ReplyCount = question.Item1.AnswerCount;
                                //item.ImageCount = question.Item1.Images.Count;
                                result.Add(item);
                            }
                            break;
                        case "Topic":
                            var topic = _topicService.Get(find.ID);
                            if (topic != null)
                            {
                                item.Type = HottestPageItemType.Topic;
                                item.TargetID = topic.Id;
                                var user = _userService.GetUserInfoDetail(topic.Creator);
                                if (user != null)
                                {
                                    item.UserHeadImg = user.HeadImgUrl;
                                    item.UserName = user.NickName;
                                }
                                var talent = _userService.GetTalentDetail(topic.Creator);
                                if (talent != null && talent.Id != Guid.Empty && talent.Role.HasValue)
                                {
                                    item.TalentType = talent.Role ?? -1;
                                    item.IsTalent = true;
                                }
                                item.Content = topic.Content;
                                item.Images = topic.Images?.Select(p => p.Url).ToList();
                                item.CreateTime = topic.CreateTime;
                                var circle = _circleService.Get(topic.CircleId);
                                if (circle != null)
                                {
                                    item.CircleName = circle.Name;
                                }
                                item.ReplyCount = (int)topic.ReplyCount;
                                item.LikeCount = (int)topic.LikeCount;
                                result.Add(item);
                            }
                            break;
                        case "Comment":
                            var comment = _schoolCommentService.GetCommentsByIds(new List<Guid>() { find.ID })?.FirstOrDefault();
                            if (comment != null)
                            {
                                item.Type = HottestPageItemType.Comment;
                                item.TargetID = comment.Id;
                                item.ShortID = UrlShortIdUtil.Long2Base32(comment.No);
                                if (comment.IsAnony)
                                {
                                    item.UserName = "匿名用户";
                                    item.UserHeadImg = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png";
                                }
                                else
                                {
                                    var user = _userService.GetUserInfoDetail(comment.CommentUserId);
                                    if (user != null)
                                    {
                                        item.UserName = user.NickName;
                                        item.UserHeadImg = user.HeadImgUrl;
                                    }
                                }
                                var talent = _userService.GetTalentDetail(comment.CommentUserId);
                                if (talent != null && talent.Id != Guid.Empty && talent.Role.HasValue)
                                {
                                    item.TalentType = talent.Role ?? -1;
                                    item.IsTalent = true;
                                }
                                item.UserRoles = new List<string>()
                                {
                                    ((int)comment.PostUserRole).ToString()
                                };
                                item.Tags = new List<string>();
                                if (comment.SchoolCommentScore.IsAttend) item.UserTags.Add("过来人");
                                var school = _schoolService.GetSchoolExtensionById(comment.SchoolId, comment.SchoolSectionId);
                                if (school != null)
                                {
                                    item.SchoolName = school.SchoolName;
                                    int score = int.Parse(Math.Round(comment.SchoolCommentScore.AggScore).ToString());
                                    score = SchoolScoreToStart.GetCurrentSchoolstart(score);
                                    item.StarCount = score;
                                    var commentCounts = _schoolCommentService.GetCommentCountBySchool(new List<Guid>() { comment.SchoolSectionId });
                                    if (commentCounts?.Any() == true)
                                    {
                                        item.CommentCount = commentCounts.First().Total;
                                    }
                                }
                                item.Content = comment.Content;
                                item.ReplyCount = comment.ReplyCount;
                                item.LikeCount = comment.LikeCount;
                                if (comment.State == PMS.CommentsManage.Domain.Common.ExamineStatus.Highlight) item.IsHighLight = true;
                                item.IsRumor = comment.RumorRefuting;
                                var images = _schoolImageService.GetImageByDataSourceId(comment.Id).ToList();
                                if (images?.Any() == true)
                                {
                                    item.Images = images.Select(p => $"{_imageSetting.QueryImager}{p.ImageUrl}").Take(3).ToList();
                                    item.ImageCount = images.Count;
                                }
                                item.CreateTime = comment.AddTime;
                                result.Add(item);
                            }
                            break;
                        case "Article":
                            var articles = _articleService.GetArticles(new Guid[] { find.ID });
                            if (articles?.Any() == true)
                            {
                                var article = articles.First();
                                var covers = _articleCoverService.GetCoversByIds(new Guid[] { article.Id });
                                item.Type = HottestPageItemType.Article;
                                item.TargetID = article.Id;
                                item.ShortID = article.No;
                                item.CreateTime = article._Time;
                                item.ViewCount = article.ViewCount;
                                if (covers?.Any() == true)
                                {
                                    item.Images = covers.Where(c => c.articleID == article.Id).Select(c => $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToList();
                                }
                                item.ArticleLayout = article.Layout;
                                item.Title = article.Title;
                                if (item.ArticleLayout == 2)
                                {
                                    var articleDetail = _articleService.GetDetail(new PMS.OperationPlateform.Application.Dtos.ArticleGetDetailRequestDto()
                                    {
                                        Id = article.Id
                                    });
                                    if (articleDetail != null)
                                    {
                                        item.Content = articleDetail.Data.Html.GetHtmlHeaderString(100);
                                    }
                                }
                                result.Add(item);
                            }
                            break;
                        case "Live":
                            var live = await _lectureService.GetLectureDetailWithLectorUserID(find.ID);
                            if (live != null)
                            {
                                var talent = _userService.GetTalentDetail(live.LectorUserID);
                                item.Type = HottestPageItemType.Live;
                                item.TargetID = live.ID;
                                item.CreateTime = live.CreateTime;
                                item.ViewCount = (int)live.ViewCount;
                                item.Title = live.Title;
                                item.Images = new List<string>() { live.CoverUrl };
                                item.ImageCount = item.Images.Count;
                                if (talent != null && talent.Id != Guid.Empty && talent.Role.HasValue)
                                {
                                    //item.UserHeadImg = talent.HeadImgUrl;
                                    //item.UserName = talent.Organization_name;
                                    item.UserTags = new List<string>() { talent.Certification_title };
                                    item.TalentType = talent.Role ?? -1;
                                    item.IsTalent = true;
                                }
                                var user = _userService.GetUserInfoDetail(live.LectorUserID);
                                if (user != null)
                                {
                                    item.UserName = user.NickName;
                                    item.UserHeadImg = user.HeadImgUrl;
                                }
                                result.Add(item);
                            }
                            break;
                    }
                }

                if (User.Identity.IsAuthenticated)
                {
                    var userInfo = User.Identity.GetUserInfo();
                    if (userInfo.UserId != default)
                    {
                        var likes = _giveLikeService.CheckCurrentUserIsLikeBulk(userInfo.UserId, result.Where(p => p.Type == HottestPageItemType.Comment || p.Type == HottestPageItemType.Topic).Select(x => x.TargetID).ToList());

                        result.ForEach(x =>
                        {
                            var temp = likes.Where(s => s.SourceId == x.TargetID).FirstOrDefault();
                            if (temp != null)
                            {
                                x.IsLike = temp.IsLike;
                            }
                        });
                    }
                }

                return ResponseResult.Success(result);
            }
            return ResponseResult.Failed("No data found");
        }

        /// <summary>
        /// 生成站内热门
        /// </summary>
        [HttpPost]
        [Description("生成站内热门")]
        public async Task<ResponseResult> Crontab()
        {
            var questions = await Task.Run(() =>
            {
                return _questionInfoService.GetQuestionData(0, int.MaxValue, DateTime.Now.AddMonths(-6));
            });

            var comments = await Task.Run(() =>
            {
                return _schoolCommentService.GetCommentData(0, int.MaxValue, DateTime.Now.AddMonths(-6));
            });

            var articles = await Task.Run(() =>
            {
                return _articleService.GetArticleData(0, int.MaxValue, DateTime.Now.AddMonths(-6));
            });

            var topics = await Task.Run(() =>
            {
                return _topicService.GetPagination(null, null, null, null, null, null, null, DateTime.Now.AddMonths(-6), null, 0, int.MaxValue, false);
            });

            var lives = await Task.Run(() =>
            {
                return _lectureService.GetLectureByDate(DateTime.Now.AddMonths(-6), DateTime.Now);
            });

            var datetime_now = DateTime.Now;

            long questionResult = 0;
            long topicResult = 0;
            long articleResult = 0;
            long commentResult = 0;
            long liveResult = 0;

            await _easyRedisClient.RemoveAsync("SortedHottest:Topic",StackExchange.Redis.CommandFlags.FireAndForget);
            if (topics.Data?.Any() == true)
            {
                var sortset_Topic = new Dictionary<SortSetItem, double>();
                double topScore = topics.Data.Max(p => p.ReplyCount + p.LikeCount);
                if (topScore == 0) topScore = 1;
                foreach (var item in topics.Data)
                {
                    var score = (double)(item.ReplyCount + item.LikeCount) / topScore;
                    if (score < 0) score = 0;
                    sortset_Topic.Add(new SortSetItem
                    {
                        ID = item.Id,
                        Type = "Topic"
                    }, score);
                }
                topicResult = await _easyRedisClient.SortedSetAddAsync("SortedHottest:Topic", sortset_Topic.OrderByDescending(p => p.Value).Take(1000).ToDictionary(k => k.Key, v => v.Value));
            }

            _easyRedisClient.RemoveAsync("SortedHottest:Question");
            if (questions?.Any() == true)
            {
                var sortset_Question = new Dictionary<SortSetItem, double>();
                double topScore = questions.Max(p => p.AnswerCount);
                if (topScore == 0) topScore = 1;
                foreach (var item in questions)
                {
                    var score = (double)item.AnswerCount / topScore;
                    if (score < 0) score = 0;
                    sortset_Question.Add(new SortSetItem
                    {
                        ID = item.Id,
                        Type = "Question"
                    }, score);
                }
                questionResult = await _easyRedisClient.SortedSetAddAsync("SortedHottest:Question", sortset_Question.OrderByDescending(p => p.Value).Take(1000).ToDictionary(k => k.Key, v => v.Value));
            }

            await _easyRedisClient.RemoveAsync("SortedHottest:Article",StackExchange.Redis.CommandFlags.FireAndForget);
            if (articles?.Any() == true)
            {
                var sortset_Article = new Dictionary<SortSetItem, double>();
                double topScore = articles.Max(p => p.viewCount_r + p.viewCount).GetValueOrDefault(1);
                foreach (var item in articles)
                {
                    var score = ((double)item.viewCount_r.GetValueOrDefault(0) + (double)item.viewCount.GetValueOrDefault(0)) / topScore;
                    if (score < 0) score = 0;
                    sortset_Article.Add(new SortSetItem
                    {
                        ID = item.id,
                        Type = "Article"
                    }, score);
                }
                articleResult = await _easyRedisClient.SortedSetAddAsync("SortedHottest:Article", sortset_Article.OrderByDescending(p => p.Value).Take(1000).ToDictionary(k => k.Key, v => v.Value));
            }

            await _easyRedisClient.RemoveAsync("SortedHottest:Comment",StackExchange.Redis.CommandFlags.FireAndForget);
            if (comments?.Any() == true)
            {
                comments = comments.Where(p => p.State > PMS.CommentsManage.Domain.Common.ExamineStatus.Unknown &&
                p.State < PMS.CommentsManage.Domain.Common.ExamineStatus.Block).ToList() ?? new List<SchoolCommentDto>();//把状态为未知和屏蔽的去掉
                var sortset_Comment = new Dictionary<SortSetItem, double>();
                double topScore = comments.Max(p => p.ReplyCount + p.LikeCount);
                if (topScore == 0) topScore = 1;
                foreach (var item in comments)
                {
                    var score = (double)(item.ReplyCount + item.LikeCount) / topScore;
                    if (score < 0) score = 0;
                    sortset_Comment.Add(new SortSetItem
                    {
                        ID = item.Id,
                        Type = "Comment"
                    }, score);
                }
                commentResult = await _easyRedisClient.SortedSetAddAsync("SortedHottest:Comment", sortset_Comment.OrderByDescending(p => p.Value).Take(1000).ToDictionary(k => k.Key, v => v.Value));
            }

            await _easyRedisClient.RemoveAsync("SortedHottest:Live",StackExchange.Redis.CommandFlags.FireAndForget);
            if (lives?.Any() == true)
            {
                var sortset_Live = new Dictionary<SortSetItem, double>();
                double topScore = lives.Max(p => p.ViewCount);
                if (topScore == 0) topScore = 1;
                foreach (var item in lives)
                {
                    var score = (double)item.ViewCount / topScore;
                    if (score < 0) score = 0;
                    sortset_Live.Add(new SortSetItem()
                    {
                        ID = item.ID,
                        Type = "Live"
                    }, score);
                }
                liveResult = await _easyRedisClient.SortedSetAddAsync("SortedHottest:Live", sortset_Live.OrderByDescending(p => p.Value).Take(1000).ToDictionary(k => k.Key, v => v.Value));
            }

            var keys = new string[] {
                "SortedHottest:Article",
                "SortedHottest:Topic",
                "SortedHottest:Question",
                "SortedHottest:Comment",
                "SortedHottest:Live"
            };

            var totalResult = articleResult + topicResult + commentResult + questionResult + liveResult;
            await _easyRedisClient.RemoveAsync("SortedHottest:Union",StackExchange.Redis.CommandFlags.FireAndForget);
            await _easyRedisClient.SortedSetCombineAndStoreAsync(StackExchange.Redis.SetOperation.Union, "SortedHottest:Union", keys, new double[] { 1, 1, 1, 1, 1 });

            return ResponseResult.Success(new
            {
                questionCount = questions.Count,
                commentCount = comments.Count,
                articleCount = articles.Count(),
                topicCount = topics.Total
            });
        }

        [HttpPost]
        [Description("分页获取已关注的动态")]
        public ResponseResult GetAttentionDynamic(int pageIndex = 1, int pageSize = 10)
        {
            //1CE588C6-4C03-48F7-A598-B101F1ABA004
            if (!User.Identity.IsAuthenticated)
            {
                var result = ResponseResult.Failed("no login");
                result.status = ResponseCode.NoLogin;
                return result;
            }
            var userInfo = User.Identity.GetUserInfo();
            var attentionUsers = _talentService.GetAttention(userInfo.UserId, userInfo.UserId, 1, int.MaxValue);

            var items = _sysMessageService.GetDynamicItems(attentionUsers.Data.Select(p => p.DataId).ToList(), pageIndex, pageSize);

            List<DynamicTmepItem> tempItems = new List<DynamicTmepItem>();
            var allUserIDs = new List<Guid>();


            //点评id
            List<Guid> commentIds = items.Where(x => x.Type == UserDynamicType.Comment).Select(x => x.Id).ToList();
            if (commentIds.Any())
            {
                var comments = _schoolCommentService.GetSchoolCommentByCommentId(commentIds);
                allUserIDs.AddRange(comments.Select(p => p.UserId).Distinct());
                var userInfos = _userService.ListUserInfo(comments.Select(p => p.UserId).Distinct().ToList()) ?? new List<PMS.UserManage.Application.ModelDto.UserInfoDto>();
                foreach (var item in comments)
                {
                    var entity = new DynamicTmepItem()
                    {
                        Id = item.Id,
                        ShortID = UrlShortIdUtil.Long2Base32(item.No),
                        AddTime = item.CreateTime.ConciseTime(),
                        Content = item.Content,
                        LikeCount = item.LikeCount,
                        ReplyCount = item.ReplyCount,
                        Type = (int)UserDynamicType.Comment,
                        Eid = item.SchoolSectionId,
                        No = UrlShortIdUtil.Long2Base32(item.No),
                        UserName = userInfos.FirstOrDefault(p => p.Id == item.UserId)?.NickName,
                        UserHeadImage = userInfos.FirstOrDefault(p => p.Id == item.UserId)?.HeadImgUrl,
                        UserID = item.UserId
                    };

                    var imgs = _schoolImageService.GetImageByDataSourceId(item.Id);
                    if (imgs?.Any() == true)
                    {
                        entity.Images = imgs.Take(3)?.Select(p => _imageSetting.QueryImager + p.ImageUrl);
                        entity.ImageCount = imgs.Count();
                    }

                    //var inviteUsers = _inviteStatusService.GetInviteUserInfos(new List<Guid>() { item.Id }, item.UserId);
                    //if (inviteUsers?.Any() == true)
                    //{
                    //    entity.InviteCount = inviteUsers.Count();
                    //    entity.InviteUserHeadImages = inviteUsers.Select(p => p.HeadImgUrl).ToArray();
                    //}


                    tempItems.Add(entity);
                }
            }

            //点评回复
            List<Guid> commentReplyIds = items.Where(x => x.Type == UserDynamicType.CommentReply).Select(x => x.Id).ToList();
            if (commentReplyIds.Any())
            {
                var commentReplies = _schoolCommentReplyService.GetCommentReplyByIds(commentReplyIds, userInfo.UserId);
                var userIDs = commentReplies.Select(p => p.ReplyUserId).ToList();
                allUserIDs.AddRange(userIDs.Distinct());
                var comments = _schoolCommentService.GetSchoolCommentByCommentId(commentReplies.Select(p => p.SchoolCommentId).Distinct().ToList());
                var parentIDs = new List<Guid>();
                commentReplies.ForEach(item =>
                {
                    if (item.ParentID.HasValue) parentIDs.Add(item.ParentID.Value);
                });
                if (comments?.Any() == true)
                {
                    userIDs.AddRange(comments.Select(p => p.UserId).Distinct());
                }
                List<ReplyDto> parentReplies = new List<ReplyDto>();
                if (parentIDs?.Any() == true)
                {
                    parentReplies = _schoolCommentReplyService.GetCommentReplyByIds(parentIDs, userInfo.UserId);
                    if (parentReplies?.Any() == true)
                    {
                        userIDs.AddRange(parentReplies.Select(p => p.ReplyUserId));
                    }
                }
                var userInfos = _userService.ListUserInfo(userIDs.Distinct().ToList()) ?? new List<PMS.UserManage.Application.ModelDto.UserInfoDto>();
                foreach (var item in commentReplies)
                {
                    var entity = new DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.AddTime,
                        Content = item.Content,
                        LikeCount = item.LikeCount,
                        ReplyCount = item.ReplayCount,
                        Type = (int)UserDynamicType.CommentReply,
                        UserName = userInfos.FirstOrDefault(p => p.Id == item.ReplyUserId)?.NickName,
                        UserHeadImage = userInfos.FirstOrDefault(p => p.Id == item.ReplyUserId)?.HeadImgUrl,
                        Eid = comments.Any(p => p.Id == item.SchoolCommentId) ? comments.FirstOrDefault(p => p.Id == item.SchoolCommentId).SchoolSectionId : Guid.Empty,
                        UserID = item.ReplyUserId
                    };

                    if (item.ParentID.HasValue)
                    {
                        var parentReply = parentReplies.FirstOrDefault(p => p.Id == item.ParentID);
                        if (parentReply != null)
                        {
                            entity.OriginContent = parentReply.Content;
                            entity.OriginUserName = userInfos.FirstOrDefault(p => p.Id == parentReply.ReplyUserId)?.NickName;
                        }
                    }
                    else
                    {
                        var comment = comments.FirstOrDefault(p => p.Id == item.SchoolCommentId);
                        if (comment?.Id != Guid.Empty)
                        {
                            entity.OriginContent = comment.Content;
                            entity.OriginUserName = userInfos.FirstOrDefault(p => p.Id == comment.UserId)?.NickName;
                        }
                    }

                    tempItems.Add(entity);
                }
            }

            //问题id
            List<Guid> questionIds = items.Where(x => x.Type == UserDynamicType.Question).Select(x => x.Id).ToList();
            if (questionIds.Any())
            {
                var questions = _questionInfoService.GetQuestionInfoByIds(questionIds);
                allUserIDs.AddRange(questions.Select(p => p.UserId).Distinct());
                var userInfos = _userService.ListUserInfo(questions.Select(p => p.UserId).Distinct().ToList()) ?? new List<PMS.UserManage.Application.ModelDto.UserInfoDto>();
                foreach (var item in questions)
                {
                    var entity = new DynamicTmepItem()
                    {
                        Id = item.Id,
                        ShortID = UrlShortIdUtil.Long2Base32(item.No),
                        AddTime = item.CreateTime.ConciseTime(),
                        Content = item.Content,
                        ReplyCount = item.ReplyCount,
                        Type = (int)UserDynamicType.Question,
                        Eid = item.SchoolSectionId,
                        No = UrlShortIdUtil.Long2Base32(item.No),
                        UserName = userInfos.FirstOrDefault(p => p.Id == item.UserId)?.NickName,
                        UserHeadImage = userInfos.FirstOrDefault(p => p.Id == item.UserId)?.HeadImgUrl,
                        UserID = item.UserId
                    };
                    //if (item.IsHaveImagers)
                    //{
                    var imgs = _schoolImageService.GetImageByDataSourceId(item.Id);
                    if (imgs?.Any() == true)
                    {
                        entity.Images = imgs.Take(3)?.Select(p => _imageSetting.QueryImager + p.ImageUrl);
                        entity.ImageCount = imgs.Count();
                    }
                    //}

                    var inviteUsers = _messageService.GetInviteUserInfos(new List<Guid>() { item.Id }, item.UserId);

                    //var inviteUsers = _inviteStatusService.GetInviteUserInfos(new List<Guid>() { item.Id }, item.UserId);
                    if (inviteUsers?.Any() == true)
                    {
                        entity.InviteCount = inviteUsers.Count();
                        entity.InviteUserHeadImages = inviteUsers.Select(p => p.HeadImgUrl).ToArray();
                    }
                    tempItems.Add(entity);
                }
            }

            //回答id
            List<Guid> answerIds = items.Where(x => x.Type == UserDynamicType.Answer).Select(x => x.Id).ToList();
            if (answerIds.Any())
            {
                var answers = _questionsAnswersInfoService.GetAnswerInfoDtoByIds(answerIds);
                var userIDs = answers.Select(p => p.UserId).ToList();
                allUserIDs.AddRange(userIDs.Distinct());
                var parentIDs = new List<Guid>();
                answers.ForEach(item =>
                {
                    if (item.ParentId.HasValue) parentIDs.Add(item.ParentId.Value);
                });
                parentIDs = parentIDs.Distinct().ToList();
                List<AnswerInfoDto> parentAnwsers = new List<AnswerInfoDto>();
                if (parentIDs?.Any() == true)
                {
                    parentAnwsers = _questionsAnswersInfoService.GetAnswerInfoDtoByIds(parentIDs);
                    if (parentAnwsers?.Any() == true)
                    {
                        userIDs.AddRange(parentAnwsers.Select(p => p.UserId));
                    }
                }
                var questions = _questionInfoService.GetQuestionInfoByIds(answers.Where(p => !p.ParentId.HasValue).Select(p => p.QuestionId).Distinct().ToList());
                if (questions?.Any() == true)
                {
                    userIDs.AddRange(questions.Select(p => p.UserId));
                }
                var userInfos = _userService.ListUserInfo(userIDs.Distinct().ToList()) ?? new List<PMS.UserManage.Application.ModelDto.UserInfoDto>();
                foreach (var item in answers)
                {
                    var entity = new DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.AddTime,
                        Content = item.AnswerContent,
                        LikeCount = item.LikeCount,
                        ReplyCount = item.ReplyCount,
                        Title = item.questionDto.QuestionContent,
                        Type = (int)UserDynamicType.Answer,
                        Eid = item.SchoolSectionId,
                        UserName = userInfos.FirstOrDefault(p => p.Id == item.UserId)?.NickName,
                        UserHeadImage = userInfos.FirstOrDefault(p => p.Id == item.UserId)?.HeadImgUrl,
                        UserID = item.UserId
                    };

                    var parentAnwser = parentAnwsers.FirstOrDefault(p => p.Id == item.ParentId);

                    var imgs = _schoolImageService.GetImageByDataSourceId(item.Id);
                    if (imgs?.Any() == true)
                    {
                        entity.Images = imgs.Take(3)?.Select(p => p.ImageUrl);
                        entity.ImageCount = imgs.Count();
                    }

                    if (parentAnwser != null)
                    {
                        entity.OriginContent = parentAnwser.AnswerContent;
                        entity.OriginUserName = userInfos.FirstOrDefault(p => p.Id == parentAnwser.UserId)?.NickName;
                    }
                    else
                    {
                        var question = questions.FirstOrDefault(p => p.Id == item.QuestionId);
                        if (question != null)
                        {
                            entity.OriginContent = question.Content;
                            entity.OriginUserName = userInfos.FirstOrDefault(p => p.Id == question.UserId)?.NickName;
                            entity.IsReplyTop = true;
                            entity.ShortID = UrlShortIdUtil.Long2Base32(question.No);
                        }
                    }

                    tempItems.Add(entity);
                }
            }

            //话题
            List<Guid> topicIDs = items.Where(x => x.Type == UserDynamicType.Topic).Select(x => x.Id).ToList();
            if (topicIDs.Any())
            {
                var topics = _topicService.GetByIds(topicIDs);
                var circles = _circleService.GetByIDs(topics.Select(p => p.CircleId).Distinct());
                allUserIDs.AddRange(topics.Select(p => p.Creator).Distinct());
                var userInfos = _userService.ListUserInfo(topics.Select(p => p.Creator).Distinct().ToList()) ?? new List<PMS.UserManage.Application.ModelDto.UserInfoDto>();
                var topicReplyLikes = _topicReplyService.GetIsLike(userInfo.UserId, topicIDs);

                foreach (var item in topics)
                {
                    var entity = new DynamicTmepItem()
                    {
                        Id = item.Id,
                        AddTime = item.CreateTime.ConciseTime(),
                        Content = item.Content,
                        LikeCount = (int)item.LikeCount,
                        ReplyCount = (int)item.ReplyCount,
                        Title = circles.FirstOrDefault(p => p.Id == item.CircleId)?.Name,
                        Type = (int)UserDynamicType.Topic,
                        UserName = userInfos.FirstOrDefault(p => p.Id == item.Creator)?.NickName,
                        UserHeadImage = userInfos.FirstOrDefault(p => p.Id == item.Creator)?.HeadImgUrl,
                        IsLike = topicReplyLikes.Any(p => p.Key == item.Id && p.Value),
                        UserID = item.Creator
                    };
                    if (item.Images?.Any() == true)
                    {
                        entity.ImageCount = item.Images.Count();
                        entity.Images = item.Images.Select(p => p.Url);
                    }
                    tempItems.Add(entity);
                }
            }

            //学校列表
            var schools = _schoolInfoService.GetSchoolName(tempItems.GroupBy(q => q.Eid).Select(q => q.Key).ToList());
            var likes = _giveLikeService.CheckCurrentUserIsLikeBulk(userInfo.UserId, tempItems.Where(x => x.Type == 1 || x.Type == 2).Select(x => x.Id).ToList());
            var talents = _userService.GetTalentDetails(allUserIDs.Distinct()) ?? new List<Talent>();

            tempItems.ForEach(x =>
            {
                if (DateTime.TryParse(x.AddTime, out DateTime convertResult)) x.AddTime = convertResult.ToString("yyyy年MM月dd日");
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
                var talent = talents.FirstOrDefault(p => p.Id == x.UserID);
                if (talent != null)
                {
                    x.IsTalent = true;
                    x.TalentType = talent.Role ?? -1;
                }
            });

            tempItems = items.Where(p => tempItems.Any(q => q.Id == p.Id)).Select(q =>
            {
                return tempItems.FirstOrDefault(p => p.Id == q.Id);
            }).ToList();

            return ResponseResult.Success(tempItems);
        }
        class SortSetItem
        {
            public Guid ID { get; set; }
            public string Type { get; set; }
        }

        class DynamicTmepItem
        {
            public Guid Eid { get; set; }
            /// <summary>
            /// 数据id
            /// </summary>
            public Guid Id { get; set; }
            /// <summary>
            /// 短ID
            /// </summary>
            public string ShortID { get; set; }
            /// <summary>
            /// 数据类型
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 点赞数
            /// </summary>
            public int LikeCount { get; set; }
            /// <summary>
            /// 回复数
            /// </summary>
            public int ReplyCount { get; set; }
            /// <summary>
            /// 阅读数
            /// </summary>
            public int ViewCount { get; set; }
            /// <summary>
            /// 添加时间
            /// </summary>
            public string AddTime { get; set; }
            /// <summary>
            /// 学校名
            /// </summary>
            public string SchoolName { get; set; }
            /// <summary>
            /// 封面图
            /// </summary>
            public string FrontCover { get; set; }
            /// <summary>
            /// 是否点赞
            /// </summary>
            public bool IsLike { get; set; }

            public string No { get; set; }
            public IEnumerable<string> Images { get; set; }
            public int ImageCount { get; set; }
            public string UserName { get; set; }
            public string UserHeadImage { get; set; }
            public IEnumerable<string> InviteUserHeadImages { get; set; }
            public int InviteCount { get; set; }

            public string OriginUserName { get; set; }
            public string OriginContent { get; set; }
            public bool IsReplyTop { get; set; }
            public Guid? UserID { get; set; }
            public bool IsTalent { get; set; }
            public int TalentType { get; set; }
        }

    }
}
