using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.RabbitMQ;
using PMS.RabbitMQ.Message;
using PMS.Search.Application.ModelDto.Query;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Application.IServices;
using ProductManagement.Infrastructure.Models;
using PMS.UserManage.Domain.IRepositories;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Domain.Dtos;
using Microsoft.EntityFrameworkCore.Internal;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class SchoolCommentService : ISchoolCommentService
    {
        //热门点评【全国】
        private readonly string HotCommentkey = "Comment:Hottest";

        //热评学校【全国】
        private readonly string HotSchoolKey = "Comment:HottestSchool";

        private readonly IGiveLikeRepository _giveLike;
        private readonly ISchoolCommentRepository _repo;
        private readonly ISchoolImageRepository _repositoryImg;
        private readonly ISchoolTagRepository _repositoryTag;
        private readonly ISchoolCommentScoreRepository _scoreRepository;
        private readonly ISchoolCommentScoreService _scoreService;
        private readonly IPartTimeJobAdminRepository _partTimeJobAdmin;
        private readonly IPartTimeJobAdminRolereRepository _JobRoleAdmin;

        //private readonly ISchoolInfoService _schoolInfoService;

        private readonly IEventBus _eventBus;
        private readonly IEasyRedisClient _easyRedisClient;

        //private readonly ISchoolInfoService _schoolInfoService;

        private readonly ISchoolCommentReplyRepository _replyRepository;

        private readonly School.Domain.IRepositories.ISchoolScoreRepository _schoolScore;
        private readonly ISchoolRepository _schoolRepository;

        private readonly IDataSearch _dataSearch;
        private readonly ISearchService _searchService;
        private readonly ITalentRepository _talentRepository;
        private readonly ICollectionRepository _collectionRepository;

        public SchoolCommentService(IPartTimeJobAdminRepository partTimeJobAdmin,
            ISchoolCommentScoreService scoreService, ISchoolCommentReplyRepository replyRepository, ISchoolCommentScoreRepository scoreRepository,
            IGiveLikeRepository giveLike, ISchoolCommentRepository repo, ISchoolImageRepository repositoryImg, IEasyRedisClient easyRedisClient,
            ISchoolTagRepository schoolTag, IPartTimeJobAdminRolereRepository obRoleAdmin, School.Domain.IRepositories.ISchoolScoreRepository schoolScore,
            IEventBus eventBus, ISchoolRepository schoolRepository, IDataSearch dataSearch, ISearchService searchService, ITalentRepository talentRepository, ICollectionRepository collectionRepository)
        {
            _partTimeJobAdmin = partTimeJobAdmin;
            _scoreService = scoreService;
            _replyRepository = replyRepository;
            _scoreRepository = scoreRepository;
            _giveLike = giveLike;
            _repositoryImg = repositoryImg;
            _repo = repo;
            _repositoryTag = schoolTag;
            _easyRedisClient = easyRedisClient;
            //_schoolInfoService = schoolInfoService;
            _JobRoleAdmin = obRoleAdmin;
            _schoolScore = schoolScore;
            _eventBus = eventBus;
            _schoolRepository = schoolRepository;
            _dataSearch = dataSearch;
            _searchService = searchService;
            _talentRepository = talentRepository;
            _collectionRepository = collectionRepository;
            //_schoolInfoService = schoolInfoService;
        }

        private void GetCommentCommon(List<Guid> commentIds, Guid UserId,
            out List<GiveLike> Likes, out List<SchoolImage> Images, out List<SchoolTag> Tags)
        {
            Likes = UserId == default(Guid) ? new List<GiveLike>() : _giveLike.CheckCurrentUserIsLikeBulk(UserId, commentIds);
            Images = _repositoryImg.GetImageByDataSourceId(commentIds, ImageType.Comment);
            Tags = _repositoryTag.GetSchoolTagByCommentId(commentIds);
        }

        public IEnumerable<SchoolComment> GetList(Expression<Func<SchoolComment, bool>> where)
        {
            return _repo.GetList(where);
        }

        public bool UserAgreement(Guid AdminId)
        {
            return _repo.UserAgreement(AdminId);
        }

        //private async Task GetCommentCommonAsync(List<Guid> commentIds, List<Guid> userIds, Guid UserId)
        //{
        //    List<GiveLike> Likes = new List<GiveLike>();
        //    List<SchoolImage> Images = new List<SchoolImage>();
        //    List<SchoolTag> Tags = new List<SchoolTag>();
        //    List<UserInfo> Users = new List<UserInfo>();
        //    Task t1 = Task.Run(() => Likes = UserId == Guid.Empty ? new List<GiveLike>() : _giveLike.CheckCurrentUserIsLikeBulk(UserId, commentIds));
        //    Task t2 = Task.Run(() => Images = _repositoryImg.GetImageByDataSourceId(commentIds, ImageType.Comment));
        //    Task t3 = Task.Run(() => Tags = _repositoryTag.GetSchoolTagByCommentId(commentIds));
        //    Task t4 = Task.Run(() => Users = _userRepository.ListUserInfo(userIds));

        //    await Task.WhenAll(t1, t2, t3, t4);
        //}

        public SchoolComment QuerySchoolComment(Guid Id)
        {
            return _repo.GetCommentById(Id);
        }

        public SchoolComment QuerySchoolCommentNo(long No)
        {
            return _repo.GetCommentByNo(No);
        }

        public bool AddSchoolComment(SchoolComment schoolComment)
        {
            bool rez = _repo.Insert(schoolComment) > 0;
            if (rez)
            {
                PublishCommentAdd(schoolComment);
                //Task.Run(() => PublishCommentAdd(schoolComment));

                DateTime AddTime = _repo.QueryCommentTime(schoolComment.Id);
                _schoolScore.UpdateCommentTotal(schoolComment.SchoolSectionId, AddTime);
            }
            return rez;
        }

        /// <summary>
        /// 发布新增点评消息
        /// </summary>
        /// <param name="schoolComment"></param>
        public void PublishCommentAdd(SchoolComment schoolComment)
        {
            var schoolExt = _schoolRepository.GetSchoolInfo(schoolComment.SchoolSectionId);
            var comment = _repo.GetCommentById(schoolComment.Id);
            if (schoolExt == null || comment == null) return;

            _eventBus.Publish(new SyncCommentAddMessage()
            {
                Id = comment.Id,
                UserId = comment.CommentUserId,
                Content = comment.Content,
                Url = $"/comment/{UrlShortIdUtil.Long2Base32(comment.No).ToLower()}.html",
                No = comment.No,
                ExtId = schoolExt.Id,
                SchoolName = schoolExt.SchoolName
            });
        }

        public List<SchoolComment> GetSchoolCommentByAdminId(Guid AdminId, int PageIndex, int PageSize, out int Total)
        {
            return _repo.GetSchoolCommentByUserId(AdminId, PageIndex, PageSize, out Total);
        }

        public List<SchoolCommentDto> PageSchoolCommentByExamineState(int page, int limit, int examineState, out int total)
        {
            var list = _repo.PageSchoolCommentByExamineState(page, limit, examineState, out total);

            var commentIds = list.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(commentIds, Guid.Empty, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return list.Select(q => ConvertSchoolCommentDto(q, Likes, Images, Tags)).ToList();
        }

        public List<SchoolCommentDto> PageSchoolComment(PageSchoolCommentQuery query, out int total)
        {
            var list = _repo.PageSchoolComment(query.PageNo, query.PageSize, query.StartTime, query.EndTime, out total, query.SchoolIds);

            var commentIds = list.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(commentIds, Guid.Empty, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return list.Select(q => ConvertSchoolCommentDto(q, Likes, Images, Tags)).ToList();
        }

        public SchoolCommentDto QueryCommentByNo(long no)
        {
            var result = _repo.GetCommentByNo(no);
            if (result == null)
                return null;

            var commentIds = new List<Guid> { result.Id };
            this.GetCommentCommon(commentIds, Guid.Empty, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return ConvertSchoolCommentDto(result, Likes, Images, Tags);
        }

        public SchoolCommentDto QueryComment(Guid commentId)
        {
            var result = _repo.GetCommentById(commentId);
            if (result == null)
                return null;

            var commentIds = new List<Guid> { result.Id };
            this.GetCommentCommon(commentIds, Guid.Empty, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return ConvertSchoolCommentDto(result, Likes, Images, Tags);
        }

        public List<SchoolCommentDto> QueryCommentByIds(List<Guid> Ids, Guid UserId)
        {
            var result = _repo.GetCommentsByIds(Ids);

            var commentIds = result.Select(x => x.Id)?.ToList();
            this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
            out List<SchoolTag> Tags);

            return result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();
        }

        public List<SchoolCommentDto> PageSchoolCommentBySchoolId(Guid schoolId, Guid UserId, int pageIndex, int pageSize, out int total, QueryCondition query = QueryCondition.All, CommentListOrder commentListOrder = CommentListOrder.None)
        {
            //List<Guid> schoolSectionIds = new List<Guid>();

            //string schoolSectionKey = "SchoolSectionInfos:{0}";
            ////根据学校分部id得到学校id
            //var School = await _redisClient.GetOrAddAsync(
            //    string.Format(schoolSectionKey, schoolSectionId),
            //    () => { return _repositorySchool.GetSchoolExtension( schoolSectionId); },
            //    new TimeSpan(0, 10, 0));

            ////高中部
            //if (query == QueryCondition.Other)
            //{
            //    schoolSectionIds = _repositorySchool.GetSchoolAllHighSchool(School.SchoolId, true, SchoolGrade.SeniorMiddleSchool);
            //}
            //else
            //{
            //    string schoolBranchKey = "SchoolBranch:{0}";
            //    var sectionSchool = await _redisClient.GetOrAddAsync(
            //    string.Format(schoolBranchKey, School.SchoolId),
            //    () => { return _repositorySchool.GetAllSchoolBranch(School.SchoolId); },
            //    new TimeSpan(0, 10, 0));
            //    schoolSectionIds = sectionSchool.GroupBy(q => q.Id).Select(p => p.Key).ToList();
            //}
            List<SchoolComment> SelectedComment = _repo.PageSchoolCommentBySchoolSectionIds(schoolId, query, commentListOrder, pageIndex, pageSize, out total);

            var commentIds = SelectedComment.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return SelectedComment.Select(c => ConvertSchoolCommentDto(c, Likes, Images, Tags)).ToList();
        }

        public List<SchoolCommentDto> GetSchoolCommentByCommentId(List<Guid> commentIds, Guid userId = default)

        {
            List<SchoolComment> SelectedComment = _repo.GetSchoolCommentByCommentId(commentIds);
            var _commentIds = SelectedComment.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(_commentIds, userId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return SelectedComment.Select(c => ConvertSchoolCommentDto(c, Likes, Images, Tags)).ToList();
        }

        public List<SchoolCommentDto> PageSchoolCommentBySchoolSectionId(Guid schoolSectionId, int pageIndex, int pageSize, out int total)
        {
            var SelectedComment = _repo.PageSchoolCommentBySchoolSectionId(schoolSectionId, pageIndex, pageSize, out total);
            return SelectedComment.Select(q => new SchoolCommentDto
            {
                No = q.No,
                Id = q.Id,
                Content = q.Content,
                Score = q.SchoolCommentScore == null ? null : new CommentScoreDto
                {
                    CommentId = q.Id,
                    IsAttend = q.SchoolCommentScore.IsAttend,
                    AggScore = q.SchoolCommentScore.AggScore,
                    EnvirScore = q.SchoolCommentScore.EnvirScore,
                    HardScore = q.SchoolCommentScore.HardScore,
                    LifeScore = q.SchoolCommentScore.LifeScore,
                    ManageScore = q.SchoolCommentScore.ManageScore,
                    TeachScore = q.SchoolCommentScore.TeachScore,
                },
                State = q.State,
                CreateTime = q.AddTime,
                IsTop = q.IsTop
            }).ToList();
        }

        /// <summary>
        /// 点评展示实体
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        private SchoolCommentDto ConvertSchoolCommentDto(SchoolComment comment, List<GiveLike> Likes, List<SchoolImage> Images, List<SchoolTag> Tags)
        {
            if (comment == null)
                return null;

            return new SchoolCommentDto
            {
                No = comment.No,
                Id = comment.Id,
                UserId = comment.CommentUserId,
                SchoolId = comment.SchoolId,
                SchoolSectionId = comment.SchoolSectionId,
                Content = comment.Content,
                State = comment.State,
                IsTop = comment.IsTop,
                CreateTime = comment.AddTime,
                LikeCount = comment.LikeCount,
                ReplyCount = comment.ReplyCount,
                StartTotal = SchoolScoreToStart.GetCurrentSchoolstart(comment.SchoolCommentScore == null ? 0 : comment.SchoolCommentScore.AggScore),
                IsLike = Likes.FirstOrDefault(q => q.SourceId == comment.Id) != null,
                IsRumorRefuting = comment.RumorRefuting,
                IsAnony = comment.IsAnony,
                Score = comment.SchoolCommentScore == null ? null : new CommentScoreDto
                {
                    CommentId = comment.Id,
                    IsAttend = comment.SchoolCommentScore.IsAttend,
                    AggScore = comment.SchoolCommentScore.AggScore,
                    EnvirScore = comment.SchoolCommentScore.EnvirScore,
                    HardScore = comment.SchoolCommentScore.HardScore,
                    LifeScore = comment.SchoolCommentScore.LifeScore,
                    ManageScore = comment.SchoolCommentScore.ManageScore,
                    TeachScore = comment.SchoolCommentScore.TeachScore,
                },
                Images = Images.Where(q => q.DataSourcetId == comment.Id).Select(x => x.ImageUrl)?.ToList(),
                Tags = Tags.Where(q => q.SchoolCommentId == comment.Id && q.Tag != null).Select(x => x.Tag.Content)?.ToList()
            };
        }

        public bool SetTop(Guid schoolCommentId)
        {
            var comment = _repo.GetModelById(schoolCommentId);

            if (_repo.SetNotTopBySchoolId(comment.SchoolId, comment.SchoolSectionId))
            {
                return _repo.SetTop(schoolCommentId, true);
            }
            return false;
        }

        public int SchoolTotalComment(Guid SchoolId)
        {
            return _repo.SchoolTotalComment(SchoolId);
        }

        public SchoolComment SchoolTopComment(Guid SchoolId)
        {
            return _repo.SchoolTopComment(SchoolId);
        }

        public SchoolComment PraiseAndReplyTotalTop(Guid SchoolId)
        {
            return _repo.PraiseAndReplyTotalTop(SchoolId);
        }

        public List<SchoolComment> GetSchoolCommentsByIds(List<Guid> SchoolSectionIds)
        {
            return _repo.GetCommentsBySchoolExtIds(SchoolSectionIds);
        }

        public List<SchoolCommentTotal> CurrentCommentTotalBySchoolId(Guid SchoolId)
        {
            return _repo.CurrentCommentTotalBySchoolId(SchoolId);
        }

        public SchoolComment SelectedComment(Guid SchoolId)
        {
            return _repo.SelectedComment(SchoolId);
        }

        public void TranAdd(SchoolComment comment)
        {
            _repo.TranAdd(comment);
        }

        public void GetCommentsByCommentSearchType(Guid SchoolBranch, CommentSearchType commentSearchType)
        {
            switch (commentSearchType)
            {
                case CommentSearchType.All:

                    break;

                case CommentSearchType.Selected:
                    break;

                case CommentSearchType.Past:
                    break;

                case CommentSearchType.Rumor:
                    break;

                case CommentSearchType.Praise:
                    break;

                case CommentSearchType.Bad:
                    break;

                case CommentSearchType.Imagers:
                    break;

                case CommentSearchType.HighSchool:
                    break;

                case CommentSearchType.International:
                    break;

                default:
                    break;
            }
        }

        public List<SchoolCommentDto> SelectedThreeComment(Guid schoolSectionId, Guid UserId, QueryCondition query = QueryCondition.All, CommentListOrder commentListOrder = CommentListOrder.None)
        {
            //List<Guid> schoolSectionIds = new List<Guid>();

            //string schoolSectionKey = "SchoolSectionInfos:{0}";
            ////根据学校分部id得到学校id
            //var School = await _redisClient.GetOrAddAsync(
            //    string.Format(schoolSectionKey, schoolSectionId),
            //    () => { return _repositorySchool.GetSchoolExtension(schoolSectionId); },
            //    new TimeSpan(0, 10, 0));

            //其他分部
            //if (query == QueryCondition.Other)
            //{
            //    schoolSectionIds = _repositorySchool.GetSchoolAllHighSchool(School.SchoolId, true, SchoolGrade.SeniorMiddleSchool);
            //}
            //else
            //{
            //    string schoolBranchKey = "SchoolBranch:{0}";
            //    var sectionSchool = await _redisClient.GetOrAddAsync(
            //    string.Format(schoolBranchKey, School.SchoolId),
            //    () => { return _repositorySchool.GetAllSchoolBranch(School.SchoolId); },
            //    new TimeSpan(0, 10, 0));
            //    schoolSectionIds = sectionSchool.GroupBy(q => q.Id).Select(p => p.Key).ToList();
            //}

            var SelectedComment = _repo.SelectedThreeComment(schoolSectionId, query);

            var commentIds = SelectedComment.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            var list = SelectedComment.Select(c => ConvertSchoolCommentDto(c, Likes, Images, Tags)).ToList();
            return list;
            //if (commentListOrder == CommentListOrder.None)
            //{
            //    return list;
            //}
            //else if (commentListOrder == CommentListOrder.AddTime)
            //{
            //    return list.OrderByDescending(x => x.CreateTime)?.ToList();
            //}
            //else if (commentListOrder == CommentListOrder.Intelligence)
            //{
            //    return list.OrderByDescending(x => x.LikeCount + x.ReplyCount)?.ToList();
            //}
            //return null;
        }

        public int UpdateCommentLikeorReplayCount(Guid CommentId, int operaValue, bool Field)
        {
            return _repo.UpdateCommentLikeorReplayCount(CommentId, operaValue, Field);
        }

        /// <summary>
        /// 获取该点评下的图片与转换
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        public List<CommentExhibitionDto> CommentToDto(List<SchoolComment> comments)
        {
            if (comments != null && comments.Count > 0)
            {
                return null;
            }

            List<CommentExhibitionDto> exhibitionDto = new List<CommentExhibitionDto>();

            var commentIds = comments.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            var images = _repositoryImg.GetImageByDataSourceId(commentIds, ImageType.Comment);

            foreach (var item in comments)
            {
                CommentExhibitionDto entity = new CommentExhibitionDto
                {
                    CommentImages = images.Where(q => q.DataSourcetId == item.Id).ToList(),
                    schoolComment = item
                };
                exhibitionDto.Add(entity);
            }
            return exhibitionDto;
        }

        public List<CommentExhibitionDto> CurrentSchoolAllComment(Guid BranchId)
        {
            List<CommentExhibitionDto> exhibitionDtos = new List<CommentExhibitionDto>();

            //获取当前学校设置为置顶的精选点评
            var selected = CurrentSchoolSelectedComment(x => x.SchoolSectionId == BranchId && x.IsTop == true);
            exhibitionDtos.Add(selected);

            return exhibitionDtos;
        }

        /// <summary>
        /// 精选点评（后台手动设置为精选点评）
        /// </summary>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public CommentExhibitionDto CurrentSchoolSelectedComment(Expression<Func<SchoolComment, bool>> where)
        {
            List<SchoolComment> comments = new List<SchoolComment>();
            var SelectedComment = _repo.GetList(where).FirstOrDefault();
            if (SelectedComment != null)
            {
                comments.Add(SelectedComment);
            }
            return CommentToDto(comments).FirstOrDefault();
        }

        /// <summary>
        /// 获取当前学校点赞+回复数最高的点评
        /// </summary>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public CommentExhibitionDto LikeAnswerTotalTop(Guid BranchId)
        {
            CommentExhibitionDto exhibitionDto = new CommentExhibitionDto();
            exhibitionDto.schoolComment = _repo.GetList(x => x.SchoolSectionId == BranchId).OrderByDescending(x => x.LikeCount + x.ReplyCount).FirstOrDefault();
            exhibitionDto.CommentImages = _repositoryImg.GetSchoolImageList(x => x.DataSourcetId == exhibitionDto.schoolComment.Id && x.ImageType == ImageType.Comment)?.ToList();
            return exhibitionDto;
        }

        /// <summary>
        /// 获取该学校下分页点评数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        //public List<SchoolCommentDto> GetCurrentSchoolComment(Expression<Func<SchoolComment, bool>> where, int PageIndex, int PageSize)
        //{
        //    var comments = _repo.CurrentSchoolComment(where, PageIndex, PageSize).ToList();
        //    if (comments != null)
        //    {
        //        var schoolScore =  _scoreRepository.GetSchoolScoreBySchool(comments.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList());
        //        List<SchoolCommentDto> schoolComments = new List<SchoolCommentDto>();

        //        comments.ForEach(x => {
        //            var comment = ConvertSchoolCommentDto(x);
        //            comment.SchoolInfo.SchoolAvgScore = (int)Math.Ceiling(schoolScore.Where(s=>s.SchoolId==x.SchoolSectionId).FirstOrDefault().AggScore);
        //            comment.SchoolInfo.SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(comment.SchoolInfo.SchoolAvgScore);
        //            schoolComments.Add(comment);
        //        });
        //        return schoolComments;
        //    }
        //    return null;
        //}

        public List<SchoolCommentDto> GetNewSchoolComment(Guid SchoolId, Guid UserId, int PageIndex, int PageSize)
        {
            var NewSchoolComment = _repo.GetList(x => x.SchoolSectionId == SchoolId).Skip((PageIndex - 1) * PageSize).Take(PageSize);

            var commentIds = NewSchoolComment.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return NewSchoolComment.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();
        }

        public List<SchoolCommentDto> AllSchoolSelectedComment(Guid UserId, List<Guid> schoolBranchIds, int order)
        {
            List<SchoolCommentDto> commentDtos = new List<SchoolCommentDto>();

            if (!schoolBranchIds.Any())
            {
                return commentDtos;
            }
            //foreach (var item in schoolBranchIds)
            //{
            //    int isExiste = 0;

            //    List<Guid> commentIds = new List<Guid>();

            //    var comment = _repo.GetSchoolSelectedComment(item,order);
            //    if (comment != null)
            //    {
            //        isExiste = 1;
            //        commentIds = new List<Guid> { comment.Id };
            //        this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
            //            out List<SchoolTag> Tags);

            //        commentDtos.Add(ConvertSchoolCommentDto(comment, Likes, Images, Tags));
            //    }

            //    if (isExiste == 0)
            //    {
            //        commentDtos.Add(new SchoolCommentDto
            //        {
            //            SchoolSectionId = item
            //        });
            //    }
            //}
            var comment = _repo.GetSchoolSelectedComment(schoolBranchIds, order);
            if (comment != null && comment.Any())
            {
                //comment.ForEach(x => {
                //});
                GetCommentCommon(comment.Select(x => x.Id).ToList(), UserId, out List<GiveLike> Likes, out List<SchoolImage> Images, out List<SchoolTag> Tags);
                comment.ForEach(x =>
                {
                    commentDtos.Add(ConvertSchoolCommentDto(x, Likes, Images, Tags));
                });
            }
            return commentDtos;
        }

        public ExaminerStatistics GetExaminerStatistics()
        {
            //var JobIds = _partTimeJobAdmin.GetList(x => x.Role == AdminUserRole.JobMember).Select(x => x.Id).ToList();

            var JobIds = _JobRoleAdmin.GetList(x => x.Role == 1 && x.Shield == false).Select(x => x.AdminId).ToList();

            int examinerComment = _repo.GetList(x => x.State != ExamineStatus.Unread && JobIds.Contains(x.CommentUserId) && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
            //未审核的点评
            int noExaminerComment = _repo.GetList(x => (int)x.State == (int)ExamineStatus.Unread && JobIds.Contains(x.CommentUserId) && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();

            return new ExaminerStatistics
            {
                ExaminerTotal = examinerComment,
                NoExaminerTotal = noExaminerComment
            };
        }

        public List<SchoolComment> QueryAllExaminers(List<Guid> Ids, int page, int limit, out int total)
        {
            List<Guid> JobIds = new List<Guid>();
            if (!Ids.Any())
            {
                JobIds = _JobRoleAdmin.GetList(x => x.Role == 1 && x.Shield == false).Select(x => x.AdminId).ToList();
            }
            else
            {
                JobIds = Ids;
            }
            //if (!Ids.Any())
            //{
            //    //JobIds = _partTimeJobAdmin.GetList(x => x.Role == AdminUserRole.JobMember && x.PartTimeJobAdminRoles.Where(a=>a.Role == 1).Select(a=>a.Shield).FirstOrDefault() == false).Select(x => x.Id).ToList();

            //    JobIds = _JobRoleAdmin.GetList(x => x.Role == 1 && x.Shield == false).Select(x => x.AdminId).ToList();
            //}
            //else
            //{
            //    if (status == -1)
            //    {
            //        JobIds = _JobRoleAdmin.GetList(x => x.Role == 1 && Ids.Contains(x.AdminId) && x.Shield == false).Select(x => x.AdminId).ToList();
            //    }
            //    else
            //    {
            //        JobIds = _JobRoleAdmin.GetList(x => x.Role == 1 && Ids.Contains(x.AdminId) && x.Shield == false).Select(x => x.AdminId).ToList();
            //    }
            //    //JobIds = _partTimeJobAdmin.GetList(x => x.Role == AdminUserRole.JobMember && Ids.Contains(x.Id) && x.PartTimeJobAdminRoles.Where(a => a.Role == 1).Select(a => a.Shield).FirstOrDefault() == false).Select(x => x.Id).ToList();
            //}

            total = _repo.GetList(x => (int)x.State == 1 && JobIds.Contains(x.CommentUserId) && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
            var allExaminers = _repo.GetList(x => (int)x.State == 1 && JobIds.Contains(x.CommentUserId) && x.AddTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderBy(x => x.AddTime).Skip((page - 1) * limit).Take(limit).ToList();
            return allExaminers;
        }

        /// <summary>
        /// 点评成功后得到的学校信息以及下面最精选的点评数据
        /// </summary>
        /// <param name="SchoolIds">模糊筛选后得到的学校id</param>
        /// <returns></returns>
        public List<SchoolCommentDto> PushSchoolInfo(List<Guid> SchoolIds, Guid userId)
        {
            List<SchoolCommentDto> commentDtos = new List<SchoolCommentDto>();
            var comment = _repo.GetSchoolSelectedComment(SchoolIds, -1);
            if (comment != null && comment.Any())
            {
                GetCommentCommon(comment.Select(x => x.Id).ToList(), userId, out List<GiveLike> Likes, out List<SchoolImage> Images, out List<SchoolTag> Tags);
                comment.ForEach(x =>
                {
                    commentDtos.Add(ConvertSchoolCommentDto(x, Likes, Images, Tags));
                });
            }
            return commentDtos;
        }

        public List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds)
        {
            return _repo.GetTotalBySchoolSectionIds(SchoolSectionIds);
        }

        public SchoolScore GetSchoolScoreBySchoolId(Guid SchoolSectionId)
        {
            return _repo.GetSchoolScoreBySchoolId(SchoolSectionId);
        }

        public List<Guid> GetHotSchoolSectionId()
        {
            return _repo.GetHotSchoolSectionId();
        }

        public List<SchoolCommentDto> GetCommentData(int pageNo, int pageSize, DateTime lastTime)
        {
            var result = _repo.GetCommentData(pageNo, pageSize, lastTime);
            return result.Select(q => new SchoolCommentDto
            {
                Id = q.Id,
                Content = q.Content,
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                ReplyCount = q.ReplyCount,
                State = q.State,
                CreateTime = q.AddTime,
                Score = q.SchoolCommentScore == null ? new CommentScoreDto() :
                new CommentScoreDto
                {
                    CommentId = q.Id,
                    IsAttend = q.SchoolCommentScore.IsAttend,
                    AggScore = q.SchoolCommentScore.AggScore,
                    EnvirScore = q.SchoolCommentScore.EnvirScore,
                    HardScore = q.SchoolCommentScore.HardScore,
                    LifeScore = q.SchoolCommentScore.LifeScore,
                    ManageScore = q.SchoolCommentScore.ManageScore,
                    TeachScore = q.SchoolCommentScore.TeachScore
                }
            }).ToList();
        }

        /// <summary>
        /// 获取该用户最新点评
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<SchoolCommentDto> QueryNewestCommentByIds(int PageIndex, int PageSize, Guid UserId)
        {
            var result = _repo.GetList(x => x.CommentUserId == UserId)?.ToList().OrderByDescending(x => x.AddTime).Skip((PageIndex - 1) * PageSize).Take(PageSize); ;

            var commentIds = result.Select(x => x.Id)?.ToList();
            this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
            out List<SchoolTag> Tags);

            return result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();
        }

        public List<SchoolCommentDto> CommentList_Pc(int City, int PageIndex, int PageSize, Guid UserId, out int Total)
        {
            var result = _repo.CommentList_Pc(City, PageIndex, PageSize, out Total);
            var commentIds = result.Select(x => x.Id)?.ToList();
            this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
            out List<SchoolTag> Tags);
            return result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();
        }

        public async Task<List<SchoolCommentDto>> HotComment(HotCommentQuery CommentQuery, Guid UserId)
        {
            List<SchoolCommentDto> HotComment = new List<SchoolCommentDto>();

            //string ViewKey = $"ViewComment:Hottest_City:{CommentQuery.City}_Grade:{CommentQuery.Grade}_Type:{CommentQuery.Type}_Discount:{Convert.ToInt32(CommentQuery.Discount)}_Diglossia:{Convert.ToInt32(CommentQuery.Diglossia)}_Chinese:{Convert.ToInt32(CommentQuery.Chinese)}";
            string ViewKey = $"ViewComment:HottestCity_{CommentQuery.City}:Grade_{CommentQuery.Grade}:Type_{CommentQuery.Type}:Discount_{Convert.ToInt32(CommentQuery.Discount)}:Diglossia_{Convert.ToInt32(CommentQuery.Diglossia)}:Chinese_{Convert.ToInt32(CommentQuery.Chinese)}";

            if (CommentQuery.City > 0)
            {
                //HotComment = await _easyRedisClient.GetOrAddAsync(ViewKey, () =>
                //{
                //    var result = _repo.HotComment(CommentQuery);
                //    var commentIds = result.Select(x => x.Id)?.ToList();
                //    this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                //    out List<SchoolTag> Tags);
                //    return result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();
                //});

                var result = _repo.HotComment(CommentQuery);
                //var commentIds = result.Select(x => x.Id)?.ToList();
                //this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                //out List<SchoolTag> Tags);
                //HotComment = result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();

                //检测热评学校是否存在5条及以上
                if (result?.Count() < 50)
                {
                    var HottestC = await _easyRedisClient.GetAsync<List<SchoolCommentDto>>(HotCommentkey);
                    if (HottestC != null)
                    {
                        HotComment.AddRange(HottestC.ToList());
                    }
                    CommentQuery.City = 0;
                    CommentQuery.Condition = false;
                    var leftData = _repo.HotComment(CommentQuery);
                    if (leftData?.Any() == true)
                    {
                        result.AddRange(leftData);
                    }
                }
                if (result?.Any() == true)
                {
                    var commentIds = result.Select(x => x.Id)?.ToList();
                    this.GetCommentCommon(commentIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                    out List<SchoolTag> Tags);
                    HotComment.AddRange(result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList());
                }
            }
            else
            {
                HotComment = _easyRedisClient.GetAsync<List<SchoolCommentDto>>(HotCommentkey).Result ?? new List<SchoolCommentDto>();
            }


            if (HotComment?.Any() == true)
            {
                CommonHelper.ListRandom(HotComment);
                HotComment = HotComment.Take(6).ToList();
                var likeCounts = GetLikeCount(HotComment.Select(p => p.Id).Distinct().ToList());
                var replyCounts = GetReplyCount(HotComment.Select(p => p.Id).Distinct().ToList());
                if (replyCounts?.Any() == true)
                {
                    replyCounts.RemoveAll(p => p.Total < 1);
                    foreach (var item in replyCounts)
                    {
                        var find = HotComment.FirstOrDefault(p => p.Id == item.Id);
                        if (find?.ReplyCount != item.Total)
                        {
                            find.ReplyCount = item.Total;
                        }
                    }
                }
                if (likeCounts?.Any() == true)
                {
                    likeCounts.RemoveAll(p => p.Count < 1);
                    foreach (var item in likeCounts)
                    {
                        var find = HotComment.FirstOrDefault(p => p.Id == item.SourceId);
                        if (find?.LikeCount != item.Count)
                        {
                            find.LikeCount = item.Count;
                        }
                    }
                }

                return HotComment.OrderByDescending(p => p.LikeCount).ToList();
            }
            else
            {
                return new List<SchoolCommentDto>();
            }
        }

        public List<SchoolCommentDto> HottestComment(DateTime StartTime, DateTime EndTime)
        {
            var result = _repo.HottestComment(StartTime, EndTime);
            var commentIds = result.Select(x => x.Id)?.ToList();
            this.GetCommentCommon(commentIds, Guid.Empty, out List<GiveLike> Likes, out List<SchoolImage> Images,
            out List<SchoolTag> Tags);

            if (!result.Any())
            {
                return new List<SchoolCommentDto>();
            }
            //var schoolExt = _schoolInfoService.GetSchoolName(result.Select(x=>x.SchoolSectionId).ToList());
            var rez = result.Select(x => ConvertSchoolCommentDto(x, Likes, Images, Tags))?.ToList();
            //rez.ForEach(x => {
            //    x.SchoolInfo = new SchoolInfoDto() {
            //        SchoolSectionId = x.SchoolSectionId,
            //        SchoolId = x.SchoolId,
            //        SchoolName = schoolExt.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault().SchoolName
            //    };
            //});
            return rez;
        }


        public List<LikeCountDto> GetLikeCount(List<Guid> ids)
        {
            var result = _repo.GetLikeCount(ids);
            return result.Select(q => new LikeCountDto
            {
                SourceId = q.SourceId,
                LikeType = q.LikeType,
                Count = q.Count
            }).ToList();
        }

        #region 公用接口

        /// <summary>
        ///
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="query">true：点评 | false：分部id</param>
        /// <returns></returns>
        public List<SchoolCommentDto> GetSchoolCardComment(List<Guid> Ids, SchoolSectionOrIds query, Guid UserId)
        {
            if (!Ids.Any())
            {
                return new List<SchoolCommentDto>();
            }

            List<Guid> SchoolIds = new List<Guid>();
            List<SchoolCommentDto> commentDtos = new List<SchoolCommentDto>();
            List<SchoolComment> comment = new List<SchoolComment>();
            List<SchoolCommentScore> scores = new List<SchoolCommentScore>();
            if (query == SchoolSectionOrIds.IdS)
            {
                comment.AddRange(_repo.GetList(x => Ids.Contains(x.Id))?.ToList());
            }
            else
            {
                comment.AddRange(_repo.GetCommentsBySchoolExtIds(Ids)?.ToList());
            }

            List<Guid> commentIds = new List<Guid>();

            if (comment.Count() > 0)
            {
                commentIds = comment.Select(x => x.Id)?.ToList();
                SchoolIds = comment.Select(x => x.SchoolSectionId)?.ToList();
                scores = _scoreRepository.GetSchoolScoreByCommentIds(commentIds);
            }

            GetCommentCommon(commentIds, UserId, out List<GiveLike> give, out List<SchoolImage> images, out List<SchoolTag> tag);

            comment.ForEach(x =>
            {
                x.SchoolCommentScore = scores.FirstOrDefault(s => s.CommentId == x.Id);
                commentDtos.Add(ConvertSchoolCommentDto(x, give, images, tag));
            });

            //var schoolExtension =  _repositorySchool.GetSchoolByIds(string.Join(",",SchoolIds));

            //List<SchoolInfoDto> schools =
            //commentDtos.ForEach(x => { x.SchoolInfo = _schoolInfoService.QuerySchoolInfo(x.SchoolSectionId).Result; });

            return commentDtos;
        }

        public List<SchoolCommentDto> GetSchoolCommentBySchoolIdOrConente(Guid SchoolId, string Conent = "")
        {
            return _repo.GetSchoolCommentBySchoolIdOrConente(SchoolId, Conent).Select(x => ConvertSchoolCommentDto(x, new List<GiveLike>(), new List<SchoolImage>(), new List<SchoolTag>()))?.ToList();
        }

        public SchoolCommentDto GetSchoolSelectedComment(Guid SelectionId, Guid userId)
        {
            var comment = _repo.GetSchoolSelectedComment(new List<Guid>() { SelectionId }, 1).FirstOrDefault();
            if (comment != null)
            {
                GetCommentCommon(new List<Guid>() { comment.Id }, userId, out List<GiveLike> give, out List<SchoolImage> images, out List<SchoolTag> tag);
                return ConvertSchoolCommentDto(comment, give, images, tag);
            }
            else
            {
                return null;
            }
        }

        #endregion 公用接口

        public List<CommentAndReply> GetCommentAndReplies(Guid UserId, int PageIndex, int PageSize)
        {
            return _repo.GetCommentAndReplies(UserId, PageIndex, PageSize);
        }

        public int CommentTotal(Guid userId)
        {
            return _repo.CommentTotal(userId);
        }

        public List<HotCommentSchoolDto> GetHotCommentSchools(DateTime beginTime, DateTime endTime)
        {
            return _repo.GetHotCommentSchools(beginTime, endTime).Select(x => new HotCommentSchoolDto()
            {
                SchoolName = x.SchoolName,
                SchoolSectionId = x.SchoolSectionId,
                Total = x.Total,
                SchoolId = x.SchoolId
            })?.ToList();
        }

        public List<HotCommentDto> GetHotComments(DateTime date)
        {
            return _repo.GetHotComments(date).Select(x => new HotCommentDto()
            {
                City = x.City,
                SchoolName = x.SchoolName,
                SchoolSectionId = x.SchoolSectionId,
                Total = x.Total,
                SchoolId = x.SchoolId,
                Id = x.Id
            })?.ToList();
        }

        public bool CheckLogout(Guid userId)
        {
            return _repo.CheckLogout(userId);
        }

        public List<SchoolCommentDto> GetCommentLikeTotal(List<Guid> CommentId)
        {
            return _repo.GetCommentLikeTotal(CommentId).Select(x => ConvertSchoolCommentDto(x, new List<GiveLike>(), new List<SchoolImage>(), new List<SchoolTag>()))?.ToList();
        }

        public async Task<List<HotCommentSchoolDto>> HottestSchool(HotCommentQuery query, bool queryAll)
        {
            List<HotCommentSchoolDto> HottestSchool = new List<HotCommentSchoolDto>();
            if (query.City > 0)
            {
                string key = $"Comment:HottestSchool:City_{query.City}:Grade_{query.Grade}:Type_{query.Type}:Discount_{Convert.ToInt32(query.Discount)}:Diglossia_{Convert.ToInt32(query.Diglossia)}:Chinese_{Convert.ToInt32(query.Chinese)}";
                HottestSchool = await _easyRedisClient.GetAsync<List<HotCommentSchoolDto>>(key);

                if (HottestSchool == null || HottestSchool.Count < 1)
                {
                    HottestSchool = _repo.HottestSchool(query, queryAll).Select(x => new HotCommentSchoolDto()
                    {
                        SchoolName = x.SchoolName,
                        SchoolSectionId = x.SchoolSectionId,
                        Total = x.Total,
                        SchoolId = x.SchoolId
                    })?.ToList();
                }


                //检测热评学校是否存在6条及以上
                if (HottestSchool == null || HottestSchool.Count() < 50)
                {
                    var rez = await _easyRedisClient.GetAsync<List<HotCommentSchoolDto>>(HotSchoolKey);

                    if (rez == null || rez.Count < 1)
                    {
                        rez = _repo.GetHotCommentSchools(query.StartTime, query.EndTime, 50).Select(x => new HotCommentSchoolDto()
                        {
                            SchoolName = x.SchoolName,
                            SchoolSectionId = x.SchoolSectionId,
                            Total = x.Total,
                            SchoolId = x.SchoolId
                        })?.ToList();
                    }

                    if (rez?.Any() == true)
                    {
                        await _easyRedisClient.AddAsync(HotSchoolKey, rez, TimeSpan.FromDays(1));
                        if (HottestSchool == null) HottestSchool = new List<HotCommentSchoolDto>();
                        HottestSchool.AddRange(rez.ToList());
                    }
                    if (HottestSchool?.Any() == true) await _easyRedisClient.AddAsync(key, HottestSchool, TimeSpan.FromMinutes(30));
                }

            }
            else
            {
                HottestSchool = _repo.GetHotCommentSchools(query.StartTime, query.EndTime, 50).Select(x => new HotCommentSchoolDto()
                {
                    SchoolName = x.SchoolName,
                    SchoolSectionId = x.SchoolSectionId,
                    Total = x.Total,
                    SchoolId = x.SchoolId
                })?.ToList() ?? new List<HotCommentSchoolDto>();
            }

            if (HottestSchool?.Any() == true)
            {
                HottestSchool = HottestSchool.GroupBy(p => p.SchoolSectionId).Select(p => p.First()).ToList();
                CommonHelper.ListRandom(HottestSchool);
                HottestSchool = HottestSchool.Take(6).ToList();
                var commentCount = GetCommentCountBySchoolSectionIDs(HottestSchool.Select(p => p.SchoolSectionId).ToList());
                commentCount.RemoveAll(p => p.Total < 1);
                foreach (var item in commentCount)
                {
                    var find = HottestSchool.FirstOrDefault(p => p.SchoolSectionId == item.Id);
                    if (find?.Total != item.Total)
                    {
                        find.Total = item.Total;
                    }
                }

                return HottestSchool.OrderByDescending(p => p.Total).ToList();
            }
            else
            {
                return new List<HotCommentSchoolDto>();
            }

        }

        public bool Checkisdistinct(string content)
        {
            return _repo.Checkisdistinct(content);
        }

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <returns></returns>
        public bool UpdateCommentViewCount(Guid commentId)
        {
            return _repo.UpdateViewCount(commentId);
        }

        /// <summary>
        /// 通过学部ID统计点评数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        public SchCommentData GetCommentDataByID(Guid SchoolSectionId)
        {
            return _repo.GetSchoolCommentDataByID(SchoolSectionId);
        }

        public List<SchoolTotalDto> GetCommentCountBySchool(List<Guid> schoolSectionId)
        {
            var result = _repo.GetCommentCountBySchool(schoolSectionId);
            return result.Select(q => new SchoolTotalDto { Id = q.Id, Total = q.Total }).ToList();
        }

        public List<SchoolTotalDto> GetCommentCountBySchoolSectionIDs(List<Guid> schoolSectionIDs)
        {
            var result = _repo.GetCommentCountBySchoolSectionIDs(schoolSectionIDs);
            return result.Select(q => new SchoolTotalDto { Id = q.Id, Total = q.Total }).ToList();
        }

        public List<SchoolTotalDto> GetReplyCount(List<Guid> commentIds)
        {
            var result = _repo.GetReplyCount(commentIds);
            return result.Select(q => new SchoolTotalDto { Id = q.Id, Total = q.Total }).ToList();
        }

        public List<SchoolTotalDto> GetCommentReplyCount(int pageNo, int pageSize)
        {
            var result = _repo.GetCommentReplyCount(pageNo, pageSize);
            return result.Select(q => new SchoolTotalDto { Id = q.Id, Total = q.Total }).ToList();
        }

        public List<SchoolCommentDto> PageCommentByUserId(Guid userId, Guid queryUserId, int pageIndex, int pageSize, bool isSelf = true)
        {
            List<SchoolComment> SelectedComment = _repo.PageCommentByUserId(userId, pageIndex, pageSize, isSelf);

            var commentIds = SelectedComment.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(commentIds, queryUserId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return SelectedComment.Select(c => ConvertSchoolCommentDto(c, Likes, Images, Tags)).ToList();
        }

        public List<SchoolCommentDto> PageCommentByCommentIds(Guid userId, List<Guid> commentIds, bool isSelf = true)
        {
            List<SchoolComment> SelectedComment = _repo.PageCommentByCommentIds(commentIds, isSelf);

            var Ids = SelectedComment.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            this.GetCommentCommon(Ids, userId, out List<GiveLike> Likes, out List<SchoolImage> Images,
                out List<SchoolTag> Tags);

            return SelectedComment.Select(c => ConvertSchoolCommentDto(c, Likes, Images, Tags)).ToList();
        }

        public List<SchoolComment> GetSchoolCommentsBySchoolUser(List<Guid> extIds, List<Guid> userIds)
        {
            var data = _repo.GetList(s => extIds.Contains(s.SchoolSectionId) && userIds.Contains(s.CommentUserId)).ToList();
            return data;
        }

        public List<SchoolComment> GetCommentsByIds(List<Guid> commentIds)
        {
            var data = _repo.GetCommentsByIds(commentIds);
            return data;
        }

        public PaginationModel<SchoolCommentDto> SearchComments(SearchCommentQuery query, Guid loginUserId)
        {
            var searchResult = _searchService.SearchComment(query);
            var ids = searchResult.Comments.Select(q => q.Id);
            return PaginationModel<SchoolCommentDto>.Build(SearchComments(ids, loginUserId), searchResult.Total);
        }

        public List<SchoolCommentDto> SearchComments(IEnumerable<Guid> ids, Guid loginUserId)
        {
            var comments = QueryCommentByIds(ids.ToList(), loginUserId);
            var talents = _talentRepository.GetTalentsByUser(comments.Select(s => s.UserId).ToArray());

            //使用原有序列, 避免排序差异
            var data = ids
                .Where(q => comments.Any(p => p.Id == q))
                .Select(q =>
                {
                    var comment = comments.First(p => p.Id == q);

                    comment.TalentType = talents.FirstOrDefault(s => s.user_id == comment.UserId)?.type;
                    return comment;
                })
                .ToList();

            return data;
        }

        public IEnumerable<SchoolCommentScoreStatisticsDto> GetSchoolCommentScoreStatistics(Guid extID)
        {
            if (extID == Guid.Empty) return null;
            var finds = _repo.GetSimpleCommentScores(extID);
            if (finds?.Any() == true)
            {
                return finds.Select(p => new SchoolCommentScoreStatisticsDto()
                {
                    AddTime = p.Item1,
                    AggScore = p.Item3,
                    HeadImgUrl = p.Item2
                });
            }
            return null;
        }

        public async Task<List<UserCommentDto>> SearchUserComments(IEnumerable<Guid> ids, Guid loginUserId)
        {
            var comments = this._repo.GetUserComments(ids, new Guid?(loginUserId));

            var users = await this._talentRepository.GetTalentUsers(comments.Select(s => s.UserId));
            var schools = await this._schoolRepository.GetSchoolExtAggs(comments.Select(s => s.ExtId));
            List<Guid> collections = this._collectionRepository.GetCollection(ids.ToList(), loginUserId);

            var images = _repositoryImg.GetImageByDataSourceId(ids.ToList(), ImageType.Comment);

            //使用原有序列, 避免排序差异
            var data = ids
                .Where(q => comments.Any(p => p.Id == q))
                .Select(q =>
                {
                    var comment = comments.First(p => p.Id == q);
                    UserCommentDto dto = new UserCommentDto
                    {
                        Id = comment.Id,
                        ExtId = comment.ExtId,
                        UserId = comment.UserId,
                        No = comment.No,
                        ReplyCount = comment.ReplyCount,
                        LikeCount = comment.LikeCount,
                        Content = comment.Content,
                        CreateTime = comment.CreateTime,
                        Score = comment.AggScore,
                        IsLike = comment.IsLike,
                        IsCollection = collections.Any(s => s == comment.Id),
                        IsAnony = comment.IsAnony,
                        IsTop = comment.IsTop,
                        IsAttend = comment.IsAttend,
                        RumorRefuting = comment.RumorRefuting,
                        SchoolCommentCount = comment.SchoolCommentCount,
                        Images = images.Where(s => s.DataSourcetId == q).Select(s => s.ImageUrl)
                    };

                    dto.User = users.FirstOrDefault(s => s.UserId == comment.UserId);
                    dto.School = schools.FirstOrDefault(s => s.ExtId == comment.ExtId);
                    return dto;
                })
                .ToList();

            return data;
        }



    }
}