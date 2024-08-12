using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class CommentReportService: ICommentReportService
    {
        private readonly ISchoolCommentReplyRepository _commentReplyRepo;
        private readonly ICommentReportRepository _reportRepo;
        private readonly ICommentReportReplyRepository _reportReplyRepo;
        private readonly ISchoolImageRepository _repositoryImg;
        private readonly ISchoolTagRepository _repositoryTag;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public CommentReportService(ICommentReportRepository repo, ICommentReportReplyRepository reportReplyRepo, ISchoolCommentReplyRepository commentReplyRepo,
            ISchoolImageRepository repositoryImg, ISchoolTagRepository schoolTag, IMapper mapper, IUserRepository userRepository)
        {
            _reportRepo = repo;
            _reportReplyRepo = reportReplyRepo;
            _repositoryImg = repositoryImg;
            _repositoryTag = schoolTag;
            _commentReplyRepo = commentReplyRepo;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public bool AddReport(CommentReportDto report)
        {
            var domainDate = _mapper.Map<CommentReportDto, SchoolCommentReport>(report);
            return _reportRepo.Insert(domainDate) > 0;
        }
        

        /// <summary>
        /// 分页获取点评举报信息
        /// </summary>
        /// <returns>The comment report.</returns>
        /// <param name="query">Query.</param>
        /// <param name="total">Total.</param>
        public List<CommentReportDto> PageCommentReport(PageCommentReportQuery query, out int total)
        {
            var list = _reportRepo.PageCommentReport(query.PageNo, query.PageSize, query.StartTime, query.EndTime,out total,query.SchoolIds);

            return list.Select(q=>new CommentReportDto {
                Id = q.Id,
                ReportUserId = q.ReportUserId,
                ReportDetail = q.ReportDetail,
                ReportReason = q.ReportType.TypeName,
                ReportReasonType = q.ReportType.Id,
                ReplayId = q.ReplayId,
                CommentId = q.CommentId,
                SchoolComment = ConvertSchoolCommentDto(q.SchoolComments),
                Status = q.Status,
                ReportDataType = q.ReportDataType,
                ReportTime = q.ReportTime
            }).ToList();
        }

        private SchoolCommentDto ConvertSchoolCommentDto(SchoolComment comment)
        {
            if (comment == null)
            {
                return null;
            }

            return new SchoolCommentDto
            {
                Id = comment.Id,
                UserId = comment.CommentUserId,
                Content = comment.Content,
                IsTop = comment.IsTop,
                SchoolId = comment.SchoolId,
                SchoolSectionId = comment.SchoolSectionId,
                State = comment.State,
                CreateTime = comment.AddTime,
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
                CommentReplies = null,
                Images = _repositoryImg.GetImageByDataSourceId(comment.Id).Select(x => x.ImageUrl).ToList(),
                Tags = _repositoryTag.GetSchoolTagByCommentId(comment.Id).Select(x => x.Tag.Content).ToList()
            };
        }


        public CommentReportDto GetCommentReport(CommentReportQuery query)
        {
            var result =  _reportRepo.GetModelById(query.Id);

            var UserIds = new List<Guid> {
                result.ReportUserId,result.SchoolComments.CommentUserId
            };
            var Users = _userRepository.ListUserInfo(UserIds);

            if (result == null)
                return null;

            var res =  new CommentReportDto
            {
                Id = result.Id,
                ReportUserId = result.ReportUserId,
                ReportDetail = result.ReportDetail,
                ReportReason = result.ReportType.TypeName,
                SchoolComment = ConvertSchoolCommentDto(result.SchoolComments),
                Status = result.Status,
                ReportTime = result.ReportTime
            };
            if (result.ReplayId != null)
            {
                //获取该条回复的详情
                var reply = _commentReplyRepo.QueryCommentReply((Guid)result.ReplayId);
                if (reply != null)
                {
                    var commentReplies = _commentReplyRepo.PageDialog(reply.Id, new List<Guid> { reply.UserId, (Guid)reply.ReplyUserId }, 0, 100);
                    List<Guid> userIds = commentReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList();
                    var users = _userRepository.ListUserInfo(userIds);
                    List<Guid> parentUserIds = commentReplies.Where(p => p.ReplyId != null).GroupBy(q => q.ReplyUserId).Select(s => (Guid)s.Key).ToList();
                    var parentUsers = _userRepository.ListUserInfo(parentUserIds);

                    res.SchoolComment.CommentReplies = commentReplies.Select(q => new CommentReplyDto
                    {
                        ReplyId = q.Id,
                        ReplyContent = q.Content,
                        ReplayUserId = q.UserId,
                        ReplyTime = q.CreateTime,
                        ParentId = q.ReplyId,
                        ParentUserId = q.ReplyUserId
                    }).ToList();
                }
            }
            return res;
        }

        public bool ReplyCommentReport(Guid commentReportId,Guid adminId ,string replyContent)
        {
            var data = _reportRepo.GetModelById(commentReportId);

            if(data==null)
            {
                return false;
            }

            if (data.Status == Domain.Common.ReportStatus.Answered || data.SchoolCommentReportReply != null)
            {
                return false;
            }

            if (_reportReplyRepo.AddCommentReportReply(commentReportId, adminId, replyContent))
            {
                data.Status = Domain.Common.ReportStatus.Answered;
                return _reportRepo.Update(data) > 0;
            }

            return false;
        }

    }
}
