using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;

namespace PMS.CommentsManage.Application.Services
{
    public class AnswerReportService: IAnswerReportService
    {
        private readonly IQuestionInfoRepository _questionRepo;
        private readonly IQuestionsAnswersInfoRepository _answerRepo;
        private readonly IAnswerReportRepository _reportRepo;
        private readonly IAnswerReportReplyRepository _reportReplyRepo;
        private readonly IMapper _mapper;

        public AnswerReportService(IMapper mapper,
            IQuestionInfoRepository questionRepo ,IAnswerReportRepository reportRepo,
             IAnswerReportReplyRepository reportReplyRepo, IQuestionsAnswersInfoRepository answerRepo)
        {
            _reportRepo = reportRepo;
            _questionRepo = questionRepo;
            _reportReplyRepo = reportReplyRepo;
            _answerRepo = answerRepo;
            _mapper = mapper;
        }

        public bool AddReport(QuestionReportDto report)
        {
           var domainReport =  _mapper.Map<QuestionReportDto, QuestionsAnswersReport>(report);
           return  _reportRepo.Add(domainReport) > 0;
        }

        public List<QuestionReportDto> PageAnswerReport(PageAnswerReportQuery query, out int total)
        {
            var list = _reportRepo.GetAnswerReportList(query.PageNo, query.PageSize, query.StartTime, query.EndTime, out total, query.SchoolIds);

            var answerIds = list.Where(q=>q.QuestionsAnswersInfoId!=null).GroupBy(p => p.QuestionsAnswersInfoId).Select(q => (Guid)q.Key).ToList();
            List<QuestionsAnswersInfo> answers = _answerRepo.ListAnswersById(answerIds);

            var replyIds = list.Where(q => q.AnswersReplyId != null).GroupBy(p => p.AnswersReplyId).Select(q => (Guid)q.Key).ToList();
            List<QuestionsAnswersInfo> answerReplys = _answerRepo.ListAnswersById(replyIds);


            return list.Select(q => new QuestionReportDto
            {
                Id = q.Id,
                ReportUserId = q.ReportUserId,
                ReportContent = q.ReportDetail,
                ReportReason = q.ReportType.TypeName,
                ReportDataType = q.ReportDataType,
                Answer = ConvertAnswerDto(q.QuestionInfos,
                                          q.QuestionsAnswersInfoId==null?null:answers.FirstOrDefault(p => p.Id == q.QuestionsAnswersInfoId),
                                          q.AnswersReplyId==null?null:answerReplys.FirstOrDefault(p => p.Id == q.AnswersReplyId)
                                          ),
                Status = q.Status,
                ReportTime = q.ReportTime
            }).ToList();
        }
        private AnswerDto ConvertAnswerDto(QuestionInfo question,QuestionsAnswersInfo answer, QuestionsAnswersInfo answerReply)
        {
            return new AnswerDto
            {
                QuestionId = question.Id,
                QuestionContent = question.Content,
                AnswerId = answer.Id,
                AnswerContent = answer?.Content,
                AnswerUserId = answer.UserId,
                ReplyId = answerReply?.Id,
                ReplyContent = answerReply?.Content,
                SchoolId = question.SchoolId,
                SchoolSectionId = question.SchoolSectionId,
                IsTop = answer != null && answer.IsTop,
                State = answer ==null? ExamineStatus.Unknown : answer.State,
                CreateTime = question.CreateTime
            };
        }


        public QuestionReportDto GetAnswerReport(AnswerReportQuery query)
        {
            var result = _reportRepo.GetModelById(query.Id);

            QuestionsAnswersInfo answer = null;
            QuestionsAnswersInfo reply = null;

            var UserIds = new List<Guid>
            {
                result.ReportUserId,
                result.QuestionInfos.UserId
            };
            if (result.QuestionsAnswersInfoId != null)
            {
                answer = _answerRepo.GetModelById((Guid)result.QuestionsAnswersInfoId);
                UserIds.Add(answer.UserId);
            }
            if (result.AnswersReplyId != null)
            {
                reply = _answerRepo.GetModelById((Guid)result.AnswersReplyId);
                UserIds.Add(reply.UserId);
            }
            return result == null ? null : new QuestionReportDto
            {
                Id = result.Id,
                ReportUserId = result.ReportUserId,
                ReportContent = result.ReportDetail,
                ReportReason = result.ReportType.TypeName,
                Answer = ConvertAnswerDto(result.QuestionInfos, answer, reply),
                Status = result.Status,
                ReportTime = result.ReportTime
            };
        }


        public bool ReplyAnswerReport(Guid answerReportId, Guid adminId, string replyContent)
        {
            var data = _reportRepo.GetModelById(answerReportId);

            if (data == null)
            {
                return false;
            }

            if (data.Status == Domain.Common.ReportStatus.Answered || data.QuestionsAnswersReportReply != null)
            {
                return false;
            }

            if (_reportReplyRepo.AddAnswerReportReply(answerReportId, adminId, replyContent))
            {
                data.Status = Domain.Common.ReportStatus.Answered;
                return _reportRepo.Update(data) > 0;
            }

            return false;
        }
    }
}
