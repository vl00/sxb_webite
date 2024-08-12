using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.RabbitMQ.Message;
using PMS.School.Domain.IRepositories;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.Services.Answer
{
    public class QuestionsAnswersInfoService : AppService<QuestionsAnswersInfo>, IQuestionsAnswersInfoService
    {
        private readonly IEventBus _eventBus;

        private readonly IQuestionsAnswersInfoRepository _repo;
        private readonly IGiveLikeRepository _giveLike;
        private readonly IPartTimeJobAdminRepository _partTimeJobAdmin;
        private readonly IQuestionInfoRepository _question;
        private readonly IPartTimeJobAdminRolereRepository _JobRoleAdmin;
        private readonly ISchoolRepository _schoolRepository;


        public QuestionsAnswersInfoService(IQuestionInfoRepository question, IPartTimeJobAdminRepository partTimeJobAdmin, IQuestionsAnswersInfoRepository repo,
            IGiveLikeRepository giveLike, IPartTimeJobAdminRolereRepository JobRoleAdmin, IEventBus eventBus, ISchoolRepository schoolRepository)
        {
            _partTimeJobAdmin = partTimeJobAdmin;
            _repo = repo;
            _giveLike = giveLike;
            _question = question;
            _JobRoleAdmin = JobRoleAdmin;
            _eventBus = eventBus;
            _schoolRepository = schoolRepository;
        }

        public int Add(QuestionsAnswersInfo questionsAnswersInfo)
        {
            var ret = Insert(questionsAnswersInfo);
            if (ret > 0)
            {
                PublishAnswerAdd(questionsAnswersInfo);
                //39Task.Run(() => PublishAnswerAdd(questionsAnswersInfo));
            }

            return ret;
        }

        public void PublishAnswerAdd(QuestionsAnswersInfo questionsAnswersInfo)
        {
            var answer = questionsAnswersInfo;
            var question = _question.GetModelById(answer.QuestionInfoId);
            if (answer == null || question == null) return;

            var school = _schoolRepository.GetSchoolInfo(question.SchoolSectionId);
            if (school == null) return;

            _eventBus.Publish(new SyncAnswerAddMessage()
            {
                Id = questionsAnswersInfo.Id,
                UserId = questionsAnswersInfo.UserId,
                Content = questionsAnswersInfo.Content,
                QuestionContent = question.Content,
                QuestionUrl = $"/question/{UrlShortIdUtil.Long2Base32(question.No).ToLower()}.html",
                QuestionUserId = question.UserId,
                QuestionNo = question.No,
                ExtId = school.Id,
                SchoolName = school.SchoolName,
            });
        }

        public override int Delete(Guid Id)
        {
            return _repo.Delete(Id);
        }

        public override IEnumerable<QuestionsAnswersInfo> GetList(Expression<Func<QuestionsAnswersInfo, bool>> where = null)
        {
            return _repo.GetList(where);
        }

        public List<AnswerInfoDto> GetListDto(Expression<Func<QuestionsAnswersInfo, bool>> where = null)
        {
            var data = GetList(where);

            List<Guid> userIds = data.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());

            return data.Select(x => ConvertAnswerToDto(x, questions, null))?.ToList();
        }

        public override QuestionsAnswersInfo GetModelById(Guid Id)
        {
            return _repo.GetModelById(Id);
        }

        public List<QuestionsAnswersInfo> GetQuestionsAnswerByAdminId(Guid Id, int PageIndex, int PageSize, out int Total)
        {
            return _repo.GetQuestionsAnswerByAdminId(Id, PageIndex, PageSize, out Total);
        }

        public bool SetTop(Guid answerId)
        {

            var answer = _repo.GetModelById(answerId);

            if (_repo.SetNotTopByQuestionId(answer.QuestionInfoId))
            {
                return _repo.SetTop(answerId, true);
            }
            return false;
        }


        public override int Insert(QuestionsAnswersInfo model)
        {
            int rez = _repo.Insert(model);
            if (rez > 0)
            {
                if (model.ParentId == null)
                {
                    _question.UpdateQuestionLikeOrReplayCount(model.QuestionInfoId, 1, false);
                    //var a = _question.GetModelById(model.QuestionInfoId);
                    rez = _question.GetAnswerCount(model.QuestionInfoId);
                }
                else
                {
                    _repo.UpdateAnswerLikeorReplayCount((Guid)model.ParentId, 1, false);

                    rez = _repo.GetReplyCount((Guid)model.ParentId);
                }
            }
            return rez;
        }

        public override bool isExists(Expression<Func<QuestionsAnswersInfo, bool>> where)
        {
            return _repo.isExists(where);
        }

        public override int Update(QuestionsAnswersInfo model)
        {
            return _repo.Update(model);
        }

        public List<AnswerDto> PageQuestionsAnswerByExamineState(int page, int limit, int examineState, out int total)
        {
            var list = _repo.PageQuestionsAnswerByExamineState(page, limit, examineState, out total);

            List<Guid> userIds = list.GroupBy(q => q.UserId).Select(q => q.Key).ToList();

            return list.Select(q => ConvertAnswerDto(q.QuestionInfo, q, null)).ToList();
        }
        public List<AnswerDto> PageQuestionsAnswer(PageQuestionAnswerQuery query, out int total)
        {
            var list = _repo.PageQuestionsAnswer(query.PageNo, query.PageSize, query.StartTime, query.EndTime, out total, query.SchoolIds);

            List<Guid> userIds = list.GroupBy(q => q.UserId).Select(q => q.Key).ToList();

            return list.Select(q => ConvertAnswerDto(q.QuestionInfo, q, null)).ToList();
        }

        public AnswerDto QueryQuestionAnswer(Guid answerId)
        {
            var result = _repo.GetModelById(answerId);

            if (result == null)
                throw new Exception("没有此问题回答");

            return ConvertAnswerDto(result.QuestionInfo, result, null);
        }

        //private QuestionDto ConvertQuestionDto(QuestionInfo question, List<QuestionsAnswersInfo> answers)
        //{
        //    return new QuestionDto
        //    {
        //        QuestionId = question.Id,
        //        QuestionContent = question.Content,
        //        QuestionUserId = question.User.Id,
        //        QuestionUserName = question.User.NickName,
        //        Phone = question.User.Phone,
        //        Answers = answers.Select(q => ConvertQuestionAnswerDto(q)).ToList(),
        //        SchoolId = question.SchoolId,
        //        SchoolSectionId = question.SchoolSectionId,
        //        CreateTime = question.CreateTime
        //    };
        //}


        //private QuestionAnswerDto ConvertQuestionAnswerDto(QuestionsAnswersInfo answer)
        //{
        //    return new QuestionAnswerDto
        //    {
        //        AnswerId = answer.Id,
        //        AnswerContent = answer.Content,
        //        AnswerUserId = answer.User.Id,
        //        AnswerUserName = answer.User.NickName,
        //        Phone = answer.User.Phone,
        //        ParentId = answer.ParentId,
        //        IsTop = answer.IsTop,
        //        State = answer.State,
        //        CreateTime = answer.CreateTime
        //    };
        //}
        private AnswerDto ConvertAnswerDto(QuestionInfo question, QuestionsAnswersInfo answer, QuestionsAnswersInfo answerReply)
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
                State = answer == null ? ExamineStatus.Unknown : answer.State,
                CreateTime = question.CreateTime
            };
        }


        public List<AnswerInfoDto> GetNewAnswerInfoByQuestionId(Guid QuestionId, Guid UserId, int PageIndex, int PageSize, out int total)
        {
            var newAnswer = _repo.GetList(x => x.QuestionInfoId == QuestionId && x.ParentId == null && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize);
            total = _repo.GetList(x => x.QuestionInfoId == QuestionId && x.ParentId == null && (x.State == ExamineStatus.Unknown || x.State == ExamineStatus.Unread || x.State == ExamineStatus.Readed || x.State == ExamineStatus.Highlight)).Count();
            List<Guid> replyIds = newAnswer.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            var Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);

            var questions = _question.GetQuestionInfoByIds(newAnswer.Select(x => x.QuestionInfoId)?.ToList());

            return newAnswer.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }


        public List<AnswerInfoDto> GetAnswerInfoByQuestionId(Guid QuestionId, Guid UserId, int PageIndex, int PageSize)
        {
            List<GiveLike> Likes = new List<GiveLike>();
            List<UserInfo> Users = new List<UserInfo>();

            List<Guid> questionInfos = new List<Guid>() { QuestionId };

            var data = _repo.QuestionAnswersOrderByRole(questionInfos, PageSize);
            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());

            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());
            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();

            //int take = PageSize;
            //AnswerInfoDto rez = null;
            //List<AnswerInfoDto> answer = new List<AnswerInfoDto>();
            //List<GiveLike> Likes = new List<GiveLike>();
            //List<UserInfo> Users = new List<UserInfo>();

            //if (PageIndex == 1)
            //{
            //    var result = _repo.GetList(x => x.QuestionInfoId == QuestionId && x.IsTop == true).FirstOrDefault();
            //    if (result != null)
            //    {
            //        Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, new List<Guid> { result.Id });
            //        Users = _userRepository.ListUserInfo(new List<Guid> { result.UserId });
            //    }
            //    var questions = _question.GetModelById(result.QuestionInfoId);
            //    rez = ConvertAnswerToDto(result,new List<QuestionInfo>() { questions }, Likes, Users);

            //    //只需要获取最精选的
            //    if (PageIndex == 1 && PageSize == 1)
            //    {
            //        if (rez != null)
            //        {
            //            answer.Add(rez);
            //            return answer;
            //        }
            //        else
            //        {
            //            var answerResult = _repo.GetList(x => x.QuestionInfoId == QuestionId && x.PostUserRole == UserManage.Domain.Common.UserRole.School).FirstOrDefault();
            //            if (answerResult != null)
            //            {
            //                Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, new List<Guid> { answerResult.Id });
            //                Users = _userRepository.ListUserInfo(new List<Guid> { answerResult.UserId });
            //            }
            //            var questions1 = _question.GetModelById(answerResult.QuestionInfoId);
            //            var SchoolUserAnswer = ConvertAnswerToDto(answerResult,new List<QuestionInfo>() { questions1 }, Likes, Users);
            //            if (SchoolUserAnswer != null)
            //            {
            //                answer.Add(SchoolUserAnswer);
            //                return answer;
            //            }
            //            else
            //            {
            //                var questionAnswer = _repo.GetList(x => x.QuestionInfoId == QuestionId).OrderByDescending(x => x.LikeCount).FirstOrDefault();
            //                if (questionAnswer != null)
            //                {
            //                    Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, new List<Guid> { questionAnswer.Id });
            //                    Users = _userRepository.ListUserInfo(new List<Guid> { questionAnswer.UserId });
            //                    var questions2 = _question.GetModelById(questionAnswer.QuestionInfoId);
            //                    answer.Add(ConvertAnswerToDto(questionAnswer,new List<QuestionInfo> { questions2 }, Likes, Users));
            //                }
            //                return answer;
            //            }
            //        }
            //    }

            //    if (rez != null)
            //    {
            //        answer.Add(rez);
            //        take--;
            //    }
            //}

            ////获取校方回复
            //var schoolUserQuestion = _repo.GetList(x => x.QuestionInfoId == QuestionId && !answer.Select(s => s.Id).Contains(x.Id) && x.PostUserRole == UserManage.Domain.Common.UserRole.School).Take(take)?.ToList();
            //take = take - schoolUserQuestion.Count();

            //List<Guid> replyIds = schoolUserQuestion.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            //Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);
            //List<Guid> userIds = schoolUserQuestion.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            //Users = _userRepository.ListUserInfo(userIds);

            //var schoolUserAnswer = _question.GetQuestionInfoByIds(schoolUserQuestion.Select(x => x.QuestionInfoId)?.ToList());


            //answer.AddRange(schoolUserQuestion.Select(x => ConvertAnswerToDto(x, schoolUserAnswer, Likes, Users)).ToList());

            ////获取达人回复
            //var darenUserQuestion = _repo.GetList(x => x.QuestionInfoId == QuestionId && !answer.Select(s => s.Id).Contains(x.Id) && x.PostUserRole == UserManage.Domain.Common.UserRole.Daren).Take(take)?.ToList();
            //take = take - darenUserQuestion.Count();

            //replyIds = darenUserQuestion.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            //Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);
            //userIds = darenUserQuestion.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            //Users = _userRepository.ListUserInfo(userIds);

            //var darenUserAnswer = _question.GetQuestionInfoByIds(schoolUserQuestion.Select(x => x.QuestionInfoId)?.ToList());

            //answer.AddRange(darenUserQuestion.Select(x => ConvertAnswerToDto(x, darenUserAnswer, Likes, Users)).ToList());

            //if (take > 0)
            //{
            //    var answers = _repo.GetList(x => x.QuestionInfoId == QuestionId && !answer.Select(s => s.Id).Contains(x.Id))?.ToList().OrderByDescending(x => x.LikeCount).Skip((PageIndex - 1) * take).Take(take);
            //    replyIds = answers.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            //    Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, replyIds);
            //    userIds = answers.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            //    Users = _userRepository.ListUserInfo(userIds);

            //    var UserAnswer = _question.GetQuestionInfoByIds(schoolUserQuestion.Select(x => x.QuestionInfoId)?.ToList());

            //    answer.AddRange(answers.Select(x => ConvertAnswerToDto(x, UserAnswer, Likes, Users)).ToList());
            //}

            //return answer;
        }

        public AnswerInfoDto ConvertAnswerToDto(QuestionsAnswersInfo questionsAnswers, List<QuestionInfo> questions, List<GiveLike> Likes)
        {
            if (questionsAnswers == null)
            {
                return null;
            }
            var question = questions.Where(x => x.Id == questionsAnswers.QuestionInfoId).FirstOrDefault();
            return new AnswerInfoDto()
            {
                Id = questionsAnswers.Id,
                UserId = questionsAnswers.UserId,
                IsLike = Likes != null ? Likes.FirstOrDefault(q => q.SourceId == questionsAnswers.Id) != null : false,
                AnswerContent = questionsAnswers.Content,
                LikeCount = questionsAnswers.LikeCount,
                IsSelected = questionsAnswers.State == Domain.Common.ExamineStatus.Highlight,
                AddTime = questionsAnswers.CreateTime.ConciseTime(),
                ReplyCount = questionsAnswers.ReplyCount,
                IsAnony = questionsAnswers.IsAnony,
                IsAttend = questionsAnswers.IsAttend,
                IsSchoolPublish = questionsAnswers.IsSchoolPublish,
                ParentId = questionsAnswers.ParentId,
                QuestionId = questionsAnswers.QuestionInfoId,
                SchoolId = question != null ? question.SchoolId : default(Guid),
                SchoolSectionId = question != null ? question.SchoolSectionId : default(Guid),
                questionDto = question != null ? new QuestionDto() { Id = question.Id, QuestionContent = question.Content, SchoolSectionId = question.SchoolSectionId, No  = question.No } : new QuestionDto()
            };
        }

        public AnswerInfoDto ConvertAnswerExtToDto(QuestionsAnswersInfoExt questionsAnswers)
        {
            if (questionsAnswers == null)
            {
                return null;
            }

            return new AnswerInfoDto()
            {
                Id = questionsAnswers.Id,
                AnswerContent = questionsAnswers.Content,
                LikeCount = questionsAnswers.LikeCount,
                IsSelected = questionsAnswers.State == Domain.Common.ExamineStatus.Highlight,
                AddTime = questionsAnswers.CreateTime.ConciseTime(),
                ReplyCount = questionsAnswers.ReplyCount,
                IsAnony = questionsAnswers.IsAnony,
                IsAttend = questionsAnswers.IsAttend,
                IsSchoolPublish = questionsAnswers.IsSchoolPublish,
                ParentId = questionsAnswers.ParentId,
                QuestionId = questionsAnswers.QuestionInfoId,
                UserId = questionsAnswers.UserId,
                ParentUserId = questionsAnswers.ParentUserId,
                ParentUserIdIsAnony = questionsAnswers.ParentUserIdIsAnony
            };
        }

        public AnswerInfoDto QueryAnswerInfo(Guid answerId)
        {
            var answer = _repo.GetModelById(answerId);
            var question = answer == null ? null : _question.GetModelById(answer.QuestionInfoId);

            return answer == null ? null : ConvertAnswerToDto(answer, new List<QuestionInfo>() { question }, new List<GiveLike>());
        }

        public List<AnswerInfoDto> PageAnswerReply(Guid answerId, Guid userId, int ordertype, int pageIndex, int pageSize)
        {
            var list = _repo.PageReplyByAnswerId(answerId, ordertype, pageIndex, pageSize);

            return list.Select(q => ConvertAnswerExtToDto(q)).ToList();
        }
        public int GetAnswerReplyTotal(Guid answerId)
        {
            int total = _repo.AnswerReplyTotalById(answerId);
            return total;
        }
        public List<AnswerInfoDto> PageDialog(Guid replyId, Guid userId, int pageIndex, int pageSize)
        {
            //获取该条回复的详情
            var reply = _repo.GetModelById(replyId);
            if (reply == null)
                return new List<AnswerInfoDto>();
            var list = _repo.PageDialog(reply.Id, new List<Guid> { reply.UserId, (Guid)reply.ParentAnswerInfo.UserId }, pageIndex, pageSize);
            return list.Select(q => ConvertAnswerExtToDto(q)).ToList();
        }
        public ExaminerStatistics GetExaminerStatistics()
        {
            //int i = _partTimeJobAdmin.GetList().Count();

            //var JobIds = _partTimeJobAdmin.GetList(x => x.Role == AdminUserRole.JobMember).Select(x => x.Id).ToList();

            var JobIds = _JobRoleAdmin.GetList(x => x.Role == 1 && x.Shield == false).Select(x => x.AdminId).ToList();

            int examinerAnswer = _repo.GetList(x => x.State != ExamineStatus.Unread && JobIds.Contains(x.UserId) && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
            //未审核的点评
            int noExaminerAnswer = _repo.GetList(x => (int)x.State == (int)ExamineStatus.Unread && JobIds.Contains(x.UserId) && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();

            return new ExaminerStatistics
            {
                ExaminerTotal = examinerAnswer,
                NoExaminerTotal = noExaminerAnswer
            };
        }

        public int UpdateAnswerLikeorReplayCount(Guid ReplayId, int operaValue, bool Field)
        {
            return _repo.UpdateAnswerLikeorReplayCount(ReplayId, operaValue, Field);
        }

        public List<AnswerInfoDto> GetAnswerByQuestionId(Guid SchoolId, int PageIndex = 1, int PageSize = 20)
        {
            throw new NotImplementedException();
        }

        public List<AnswerInfoDto> QuestionAnswersOrderByRole(Guid QuestionInfoId, Guid UserId, int PageIndex, int PageSize)
        {

            List<GiveLike> Likes = new List<GiveLike>();
            var data = _repo.QuestionAnswersOrderByRole(new List<Guid>() { QuestionInfoId }, PageSize);

            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());
            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());

            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        public List<AnswerInfoDto> ReplyNewest(List<Guid> replyIds)
        {
            return _repo.ReplyNewest(replyIds).Select(x => ConvertAnswerToDto(x, new List<QuestionInfo>(), new List<GiveLike>()))?.ToList();
        }


        public List<AnswerInfoDto> QuestionAnswersOrderByQuestionIds(List<QuestionInfo> questions, Guid UserId, int Take)
        {
            if (questions == null && questions.Count() == 0)
                return null;

            List<Guid> QuestionIds = questions.Select(x => x.Id)?.ToList();

            if (QuestionIds.Count() == 0)
            {
                return null;
            }

            var data = _repo.QuestionAnswersOrderByRole(QuestionIds, Take);

            List<GiveLike> Likes = new List<GiveLike>();


            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());

            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        /// <summary>
        /// 批量获取问题回答\回复
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<AnswerInfoDto> GetAnswerInfoDtoByIds(List<Guid> Ids, Guid UserId)
        {
            List<GiveLike> Likes = new List<GiveLike>();
            List<UserInfo> Users = new List<UserInfo>();



            var data = _repo.GetList(x => Ids.Contains(x.Id));
            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());

            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());
            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        /// <summary>
        /// 批量获取问题回答\回复
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public List<AnswerInfoDto> GetAnswerInfoDtoByIds(List<Guid> Ids)
        {
            List<GiveLike> Likes = new List<GiveLike>();
            List<UserInfo> Users = new List<UserInfo>();
            var data = _repo.GetList(x => Ids.Contains(x.Id));
            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());
            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        public List<AnswerInfoDto> GetNewestAnswerInfoDtoByUserId(int PageIndex, int PageSize, Guid UserId)
        {
            List<GiveLike> Likes = new List<GiveLike>();
            List<UserInfo> Users = new List<UserInfo>();



            var data = _repo.GetList(x => x.UserId == UserId).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize);
            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());

            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());
            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        public List<AnswerInfoDto> GetCurrentUserNewestAnswer(int PageIndex, int PageSize, Guid UserId)
        {
            //GetCurrentUserNewestAnswer

            List<GiveLike> Likes = new List<GiveLike>();
            List<UserInfo> Users = new List<UserInfo>();



            var data = _repo.GetCurrentUserNewestAnswer(PageIndex, PageSize, UserId);
            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());

            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());
            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        public AnswerInfoDto GetFirstParent(Guid Id)
        {
            return ConvertAnswerToDto(_repo.GetFirstParent(Id), new List<QuestionInfo>(), new List<GiveLike>());
        }

        public int QuestionAnswer(Guid userId)
        {
            return _repo.QuestionAnswer(userId);
        }

        public int AnswerReplyTotal(Guid userId)
        {
            return _repo.AnswerReplyTotal(userId);
        }

        public List<QuestionAnswerAndReply> CurrentPublishQuestionAnswerAndReply(Guid UserId, int PageIndex, int PageSize)
        {
            return _repo.CurrentPublishQuestionAnswerAndReply(UserId, PageIndex, PageSize);
        }

        public List<QuestionAnswerAndReply> CurrentLikeQuestionAndAnswer(Guid UserId, int PageIndex, int PageSize)
        {
            return _repo.CurrentLikeQuestionAndAnswer(UserId, PageIndex, PageSize);
        }

        public bool CheckAnswerDistinct(string content)
        {
            return _repo.CheckAnswerDistinct(content);
        }

        public bool UpdateAnswerViewCount(Guid answerId)
        {
            return _repo.UpdateViewCount(answerId);
        }

        public List<AnswerInfoDto> GetAnswerHottestReplys(Guid answerReplyId)
        {
            var list = _repo.GetAnswerHottestReplys(answerReplyId);

            return list.Select(q => ConvertAnswerExtToDto(q)).ToList();
        }

        public List<AnswerInfoDto> PageAnswerByUserId(Guid UserId, Guid QueryUserId, bool IsSelf, int PageIndex, int PageSize, List<Guid> AnswerIds = null)
        {

            List<GiveLike> Likes = new List<GiveLike>();
            List<UserInfo> Users = new List<UserInfo>();

            var data = _repo.PageAnswerByUserId(UserId, IsSelf, PageIndex, PageSize, AnswerIds);
            Likes = _giveLike.CheckCurrentUserIsLikeBulk(UserId, data.Select(x => x.Id)?.ToList());

            var questions = _question.GetQuestionInfoByIds(data.Select(x => x.QuestionInfoId)?.ToList());
            return data.Select(x => ConvertAnswerToDto(x, questions, Likes))?.ToList();
        }

        public List<AnswerReply> GetQuestionAnswerReplyByIds(List<Guid> Ids)
        {
            return _repo.GetQuestionAnswerReplyByIds(Ids);
        }

        public async Task<Dictionary<Guid, int>> GetTopAnswerCountsByQuestionIDs(IEnumerable<Guid> questionIDs)
        {
            return await _repo.GetAnswerCountByQuestionIDs(questionIDs);
        }
    }
}
