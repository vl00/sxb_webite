using AutoMapper;
using iSchool;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Org.BouncyCastle.Crypto.Tls;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.IRepositories;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.Services.Question
{
    public class QuestionInfoService : IQuestionInfoService
    {
        private string HottestSchoolKey = "Question:HottestSchool";
        private string HottestQuestionKey = "Question:Hottest";

        private readonly IQuestionInfoRepository _repo;
        private readonly IGiveLikeRepository _giveLike;
        private readonly IQuestionsAnswersInfoService _answersInfoService;
        private readonly ISchoolImageRepository _imageRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly School.Domain.IRepositories.ISchoolScoreRepository _schoolScore;

        private readonly ISearchService _searchService;
        private readonly ITalentRepository _talentRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ICollectionRepository _collectionRepository;

        public QuestionInfoService(IQuestionInfoRepository repo, IGiveLikeRepository giveLike,
            IQuestionsAnswersInfoService answersInfoService, ISchoolImageRepository imageRepository, IEasyRedisClient easyRedisClient, School.Domain.IRepositories.ISchoolScoreRepository schoolScore, ISearchService searchService, ITalentRepository talentRepository, ISchoolRepository schoolRepository, ICollectionRepository collectionRepository)
        {
            _repo = repo;
            _giveLike = giveLike;
            _answersInfoService = answersInfoService;
            _imageRepository = imageRepository;
            _easyRedisClient = easyRedisClient;

            _schoolScore = schoolScore;
            _searchService = searchService;
            _talentRepository = talentRepository;
            _schoolRepository = schoolRepository;
            _collectionRepository = collectionRepository;
        }

        private void GetQuestionCommon(List<Guid> questionIds, Guid UserId,
            out List<GiveLike> Likes, out List<SchoolImage> Images)
        {
            Likes = UserId == Guid.Empty ? new List<GiveLike>() : _giveLike.CheckCurrentUserIsLikeBulk(UserId, questionIds);
            Images = _imageRepository.GetImageByDataSourceId(questionIds, ImageType.Question);
        }

        public QuestionDto GetQuestionById(Guid Id, Guid UserId)
        {
            var result = _repo.GetModelById(Id);
            if (result == null)
                return null;

            var questionIds = new List<Guid> { result.Id };
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(new List<QuestionInfo>() { result }, UserId, 1);

            return CovertQuestionToDto(result, UserId, Likes, Images, QuestionHotAnswer);
        }

        public List<QuestionDto> GetQuestionByIds(List<Guid> Ids, Guid UserId)
        {
            var result = _repo.GetList(q => Ids.Contains(q.Id));

            var questionIds = result.Select(q => q.Id).ToList();
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(result.ToList(), UserId, 1);

            return result.Select(x => CovertQuestionToDto(x, UserId, Likes, Images, QuestionHotAnswer)).ToList();
        }

        public List<QuestionDto> GetHotQuestionInfoBySchoolId(Guid SchoolId, Guid UserId, int PageIndex, int PageSize, SelectedQuestionOrder Order)
        {
            //int Total = 3;
            List<QuestionDto> question = new List<QuestionDto>();

            var questions = _repo.GetHotQuestionInfoBySchoolId(SchoolId);
            if (questions.Count() == 0)
            {
                return null;
            }
            List<Guid> questionIds = questions.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(questions, UserId, 1);

            question.AddRange(questions.Select(x => CovertQuestionToDto(x, UserId, Likes, Images, QuestionHotAnswer))?.ToList());

            if (Order == SelectedQuestionOrder.None)
            {
                return question;
            }
            else if (Order == SelectedQuestionOrder.Intelligence)
            {
                return question.OrderByDescending(x => x.LikeCount + x.AnswerCount)?.ToList();
            }
            //else if (Order == SelectedQuestionOrder.CreateTime)
            //{
            //    return question.OrderByDescending(x => x.QuestionCreateTime)?.ToList();
            //}
            else if (Order == SelectedQuestionOrder.Answer)
            {
                return question.OrderByDescending(x => x.AnswerCount)?.ToList();
            }
            return question;
        }

        public QuestionDto CovertQuestionToDto(QuestionInfo questionInfo, Guid UserId, List<GiveLike> Likes, List<SchoolImage> Images, List<AnswerInfoDto> answers = null)
        {
            if (questionInfo == null)
                return null;

            //var user = _userRepository.GetUserInfo(questionInfo.UserId);
            return new QuestionDto()
            {
                Id = questionInfo.Id,
                No = questionInfo.No,
                UserId = questionInfo.UserId,
                AnswerCount = questionInfo.ReplyCount,
                LikeCount = questionInfo.LikeCount,
                isLike = Likes.FirstOrDefault(q => q.SourceId == questionInfo.Id) != null,
                QuestionCreateTime = questionInfo.CreateTime,
                QuestionContent = questionInfo.Content,
                //问题展示页面只显示一条精选数据
                answer = answers.Where(x => x.QuestionId == questionInfo.Id).ToList(),
                //answer = new List<AnswerInfoDto>(),
                Images = Images.Where(q => q.DataSourcetId == questionInfo.Id).Select(x => x.ImageUrl).ToList(),
                SchoolId = questionInfo.SchoolId,
                SchoolSectionId = questionInfo.SchoolSectionId,
                IsAnony = questionInfo.IsAnony
            };
        }

        public int TotalQuestion(Guid SchoolId)
        {
            return _repo.TotalQuestion(new List<Guid>() { SchoolId });
        }

        public async Task<List<QuestionDto>> AllSchoolSelectedQuestion(Guid UserId, List<Guid> schoolBranchIds, SelectedQuestionOrder Order)
        {
            if (schoolBranchIds.Count() <= 0)
            {
                return new List<QuestionDto>();
            }

            List<QuestionDto> QuestionDtos = new List<QuestionDto>();

            List<QuestionInfo> questionInfos = new List<QuestionInfo>();
            List<AnswerInfoDto> answerDtos = new List<AnswerInfoDto>();

            questionInfos.AddRange(_repo.GetSchoolSelectedQuestion(schoolBranchIds, Order));

            if (questionInfos.Any())
            {
                answerDtos.AddRange(_answersInfoService.QuestionAnswersOrderByQuestionIds(questionInfos, UserId, 1));
                if (Order == SelectedQuestionOrder.CreateTime)
                {
                    Dictionary<Guid, int> answerCount = await _answersInfoService.GetTopAnswerCountsByQuestionIDs(questionInfos.Select(p => p.Id).Distinct());
                    if (answerCount?.Any() == true)
                    {
                        foreach (var item in answerCount)
                        {
                            var find = questionInfos.FirstOrDefault(p => p.Id == item.Key);
                            if (find != null) find.ReplyCount = item.Value;
                        }
                    }
                }
            }

            foreach (var item in schoolBranchIds)
            {
                var question = questionInfos.Where(x => x.SchoolSectionId == item).FirstOrDefault();
                if (question == null)
                {
                    QuestionDtos.Add(CovertQuestionToDto(
                        new QuestionInfo() { Id = default(Guid), SchoolSectionId = item, QuestionsAnswersInfos = new List<QuestionsAnswersInfo>() },
                        UserId, new List<GiveLike>(), new List<SchoolImage>(), new List<AnswerInfoDto>()));
                }
                else
                {
                    var userIds = new List<Guid> { question.UserId };
                    var questionIds = new List<Guid> { question.Id };
                    var answers = answerDtos.Where(x => x.QuestionId == question.Id)?.ToList();
                    GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);
                    QuestionDtos.Add(CovertQuestionToDto(question, UserId, Likes, Images, answers));
                }
            }

            foreach (var item in QuestionDtos)
            {
                item.isExists = true;
                if (item.Id == default(Guid))
                {
                    item.isExists = false;
                }
            }

            return QuestionDtos;
        }

        public List<QuestionDto> PageQuestionByUserId(Guid UserId, Guid QueryUserId, bool IsSelf, int PageIndex, int PageSize)
        {
            List<QuestionInfo> questions = new List<QuestionInfo>();
            questions = _repo.PageQuestionByUserId(UserId, IsSelf, PageIndex, PageSize);
            if (!questions.Any())
                return new List<QuestionDto>();

            List<Guid> userIds = questions.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            List<Guid> questionIds = questions.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            GetQuestionCommon(questionIds, QueryUserId, out List<GiveLike> Likes, out List<SchoolImage> Images);


            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(questions, QueryUserId, 1);

            return questions.Select(x => CovertQuestionToDto(x, UserId, Likes, Images, QuestionHotAnswer))?.ToList();
        }

        public List<QuestionDto> GetNewQuestionInfoBySchoolId(Guid schoolId, Guid UserId, int PageIndex, int PageSize, QueryQuestion query, SelectedQuestionOrder Order, out int total)
        {
            List<QuestionInfo> questions = new List<QuestionInfo>();


            questions = _repo.PageSchoolCommentBySchoolSectionIds(schoolId, query, Order, PageIndex, PageSize, out total);
            if (questions.Count() == 0)
                return null;


            List<Guid> userIds = questions.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            List<Guid> questionIds = questions.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);


            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(questions, UserId, 1);

            return questions.Select(x => CovertQuestionToDto(x, UserId, Likes, Images, QuestionHotAnswer))?.ToList();
        }
        public async Task<QuestionDto> QuestionDetailByNo(long No, Guid UserId)
        {
            var qeustion = _repo.GetQuestionByNo(No);

            if (qeustion == null)
                throw new Exception("没有此问题回答");

            var answerCount = await _answersInfoService.GetTopAnswerCountsByQuestionIDs(new Guid[] { qeustion.Id });

            var userIds = new List<Guid> { qeustion.UserId };
            var questionIds = new List<Guid> { qeustion.Id };
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(new List<QuestionInfo>() { qeustion }, UserId, 1);

            if (answerCount?.Any() == true)
            {
                qeustion.ReplyCount = answerCount[qeustion.Id];
            }

            return CovertQuestionToDto(qeustion, UserId, Likes, Images, QuestionHotAnswer);
        }
        public QuestionDto QuestionDetail(Guid questionId, Guid UserId)
        {
            var qeustion = _repo.GetModelById(questionId);

            if (qeustion == null)
                throw new Exception("没有此问题回答");

            var userIds = new List<Guid> { qeustion.UserId };
            var questionIds = new List<Guid> { qeustion.Id };
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(new List<QuestionInfo>() { qeustion }, UserId, 1);

            return CovertQuestionToDto(qeustion, UserId, Likes, Images, QuestionHotAnswer);
        }

        public bool AddQuestion(QuestionInfo question)
        {
            bool rez = _repo.Insert(question) > 0;
            if (rez)
            {
                DateTime createTime = _repo.QueryQuestionTime(question.Id);
                _schoolScore.UpdateQuestionTotal(question.SchoolSectionId, createTime);
            }
            return rez;
        }

        public int UpdateQuestionLikeorReplayCount(Guid QuestionId, int operaValue, bool Field)
        {
            return _repo.UpdateQuestionLikeOrReplayCount(QuestionId, operaValue, Field);
        }

        public List<SchoolQuestionTotal> CurrentQuestionTotalBySchoolId(Guid SchoolId)
        {
            return _repo.CurrentQuestionTotalBySchoolId(SchoolId);
        }

        public List<QuestionDto> PushSchoolInfo(List<Guid> SchoolIds, Guid userId)
        {
            List<QuestionDto> questionDtos = new List<QuestionDto>();
            var SchoolSectionQuestions = _repo.GetSchoolSelectedQuestion(SchoolIds, SelectedQuestionOrder.None);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(SchoolSectionQuestions, userId, 1);

            foreach (var item in SchoolIds)
            {
                var question = SchoolSectionQuestions.Where(x => x.SchoolSectionId == item).FirstOrDefault();
                if (question == null)
                {
                    questionDtos.Add(CovertQuestionToDto(
                        new QuestionInfo() { Id = default(Guid), SchoolSectionId = item, QuestionsAnswersInfos = new List<QuestionsAnswersInfo>() },
                        userId, new List<GiveLike>(), new List<SchoolImage>(), new List<AnswerInfoDto>()));
                }
                else
                {
                    var userIds = new List<Guid> { question.UserId };
                    var questionIds = new List<Guid> { question.Id };
                    GetQuestionCommon(questionIds, userId, out List<GiveLike> Likes, out List<SchoolImage> Images);
                    questionDtos.Add(CovertQuestionToDto(question, userId, Likes, Images, QuestionHotAnswer));
                }
            }

            return questionDtos;
        }


        public List<SchoolTotalDto> SchoolTotalQuestion(List<Guid> SchoolIds)
        {
            var result = _repo.SchoolTotalQuestion(SchoolIds);

            return result.Select(q => new SchoolTotalDto
            {
                Id = q.Id,
                Total = q.Total
            }).ToList();
        }
        public List<SchoolTotalDto> SchoolSectionTotalQuestion(List<Guid> SchoolExtIds)
        {
            var result = _repo.SchoolSectionTotalQuestion(SchoolExtIds);

            return result.Select(q => new SchoolTotalDto
            {
                Id = q.Id,
                Total = q.Total
            }).ToList();
        }


        #region 喜哥
        public QuestionDto CovertQuestionToArticleDto(QuestionInfo question)
        {
            if (question == null)
                return null;

            return new QuestionDto()
            {
                Id = question.Id,
                QuestionContent = question.Content,
                QuestionCreateTime = question.CreateTime,
                AnswerCount = question.ReplyCount,
                SchoolSectionId = question.SchoolSectionId
            };
        }

        public List<QuestionDto> GetQuestionAnswerBySchoolIdOrConente(Guid SchoolId, string Content, int PageIndex, int PageSize, out int Total)
        {
            List<QuestionInfo> questions = new List<QuestionInfo>();
            Total = 0;

            if (Content == "" || Content == null)
            {
                Total = _repo.GetList(s => s.SchoolSectionId == SchoolId).Count();
                Total = Convert.ToInt32(Math.Ceiling(Total / (PageSize * 1.0)));
                return _repo.GetList(x => x.SchoolSectionId == SchoolId).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => CovertQuestionToArticleDto(x))?.ToList();
            }
            else
            {
                Total = _repo.GetList(s => s.SchoolSectionId == SchoolId && s.Content.Contains(Content)).Count();
                Total = Convert.ToInt32(Math.Ceiling(Total / (PageSize * 1.0)));
                return _repo.GetList(x => x.SchoolSectionId == SchoolId && x.Content.Contains(Content)).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => CovertQuestionToArticleDto(x))?.ToList();
            }
        }
        #endregion

        public List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds)
        {
            return _repo.GetTotalBySchoolSectionIds(SchoolSectionIds);
        }

        public (QuestionDto, int) GetQuestionInfoById(Guid QuestionId)
        {
            int QuestionTotal;
            var question = _repo.GetModelById(QuestionId);
            QuestionTotal = 0;
            if (question == null)
                return (null, QuestionTotal);

            QuestionTotal = _repo.TotalQuestion(new List<Guid>() { question.SchoolSectionId });
            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(new List<QuestionInfo>() { question }, default(Guid), 1);

            var rez = CovertQuestionToDto(question, default(Guid), new List<GiveLike>(), new List<SchoolImage>(), QuestionHotAnswer);
            rez.answer = _answersInfoService.GetListDto(x => x.QuestionInfoId == QuestionId).OrderByDescending(x => x.AddTime).Take(2)?.ToList();
            return (rez, QuestionTotal);
        }

        public List<Guid> GetHotSchoolSectionId()
        {
            return _repo.GetHotSchoolSectionId();
        }

        public List<QuestionDto> GetQuestionData(int pageNo, int pageSize, DateTime lastTime)
        {
            var result = _repo.GetQuestionData(pageNo, pageSize, lastTime);
            return result == null ? new List<QuestionDto>() :
                result.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    No = q.No,
                    QuestionContent = q.Content,
                    AnswerCount = q.ReplyCount,
                    QuestionCreateTime = q.CreateTime,
                    SchoolSectionId = q.SchoolSectionId,
                    State = q.State
                }).ToList();
        }
        public List<QuestionDto> GetNewestQuestion(int PageIndex, int PageSize, Guid UserID)
        {
            var result = _repo.GetList(q => q.UserId == UserID).ToList().OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize); ;

            var questionIds = result.Select(q => q.Id).ToList();
            GetQuestionCommon(questionIds, UserID, out List<GiveLike> Likes, out List<SchoolImage> Images);

            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(result.ToList(), UserID, 1);

            return result.Select(x => CovertQuestionToDto(x, UserID, Likes, Images, QuestionHotAnswer)).ToList();
        }

        public int SchoolCommentQuestionCountByTime(DateTime startTime, DateTime endTime)
        {
            return _repo.SchoolCommentQuestionCountByTime(startTime, endTime);
        }

        public List<QuestionTotalByTimeDto> PageQuestionTotalTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize)
        {
            var rez = _repo.PageQuestionTotalTime(startTime, endTime, pageNo, pageSize);
            return rez == null ? new List<QuestionTotalByTimeDto>() : rez.Select(q => new QuestionTotalByTimeDto()
            {
                School = q.School,
                SchoolSectionId = q.SchoolSectionId,
                Total = q.Total,
                CreateTime = q.CreateTime
            }).ToList();
        }

        public int QuestionTotal(Guid userId)
        {
            return _repo.QuestionTotal(userId);
        }

        public List<HotQuestionSchoolDto> GetHotQuestionSchools(DateTime starTime, DateTime endTime, int count = 6)
        {
            return _repo.GetHotQuestionSchools(starTime, endTime, count).Select(x => new HotQuestionSchoolDto()
            {
                City = x.City,
                SchoolName = x.SchoolName,
                SchoolSectionId = x.SchoolSectionId,
                Total = x.Total,
                SchoolId = x.SchoolId,
                SchoolNo = x.SchoolNo
            })?.ToList();
        }

        /// <summary>
        /// 获取官网 提问列表页
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="City"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<QuestionDto> QuestionList_Pc(Guid UserId, int City, int PageIndex, int PageSize, out int Total)
        {
            var result = _repo.QuestionList_Pc(City, PageIndex, PageSize);
            Total = _repo.QuestionListCount_Pc(City);
            var questionIds = result.Select(q => q.Id).ToList();
            GetQuestionCommon(questionIds, UserId, out List<GiveLike> Likes, out List<SchoolImage> Images);

            //var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(result.ToList(), UserId, 1);
            return result.Select(x => CovertQuestionToDto(x, UserId, Likes, Images, new List<AnswerInfoDto>())).ToList();
        }

        /// <summary>
        /// 热问学校
        /// </summary>
        /// <returns></returns>
        public async Task<List<HotQuestionSchoolDto>> HottestSchool()
        {
            var hottestSchools = await _easyRedisClient.GetAsync<List<HotQuestionSchoolDto>>(HottestSchoolKey);
            if (hottestSchools == null || hottestSchools.Count < 50)
            {
                var date_Now = DateTime.Now;
                var leftData = GetHotQuestionSchools(date_Now.AddMonths(-6), date_Now, 50);
                if (leftData?.Count > 0)
                {
                    if (hottestSchools == null) hottestSchools = new List<HotQuestionSchoolDto>();

                    hottestSchools.AddRange(leftData);

                    hottestSchools = hottestSchools.GroupBy(p => p.SchoolSectionId).Select(p => p.First()).ToList();

                    await _easyRedisClient.AddAsync(HottestSchoolKey, hottestSchools, TimeSpan.FromMinutes(30));
                }
            }
            if (hottestSchools?.Count > 0)
            {
                CommonHelper.ListRandom(hottestSchools);
                hottestSchools = hottestSchools.Take(6).ToList();
                var questionCounts = GetQuestionCountBySIDs(hottestSchools.Select(p => p.SchoolSectionId).Distinct().ToList());
                if (questionCounts?.Any() == true)
                {
                    questionCounts.RemoveAll(p => p.Total < 1);
                    foreach (var item in questionCounts)
                    {
                        var find = hottestSchools.FirstOrDefault(p => p.SchoolSectionId == item.Id);
                        if (find?.Total != item.Total)
                        {
                            find.Total = item.Total;
                        }
                    }
                }
            }
            return hottestSchools.OrderByDescending(p => p.Total).ToList();
        }

        public List<QuestionDto> GetHotQuestion(DateTime startTime, DateTime endTime)
        {
            var result = _repo.GetHotQuestion(startTime, endTime);
            return result.Select(x => CovertQuestionToDto(x, Guid.Empty, new List<GiveLike>(), new List<SchoolImage>(), new List<AnswerInfoDto>())).ToList();
        }
        public async Task<List<QuestionDto>> GetHotQuestion()
        {
            var datas = await _easyRedisClient.GetAsync<List<QuestionDto>>(HottestQuestionKey);

            if (datas == null || datas.Count < 50)
            {
                var result = _repo.GetHotQuestion(DateTime.Now.AddMonths(-1), DateTime.Now, 50);
                if (result?.Any() == true)
                {
                    if (datas == null) datas = new List<QuestionDto>();
                    datas.AddRange(result.Select(x => CovertQuestionToDto(x, Guid.Empty, new List<GiveLike>(), new List<SchoolImage>(), new List<AnswerInfoDto>())).ToList());
                }
                if (datas?.Any() == true)
                {
                    datas = datas.GroupBy(p => p.Id).Select(p => p.First()).ToList();
                    await _easyRedisClient.AddAsync(HottestQuestionKey, datas, TimeSpan.FromHours(3));
                }
            }

            if (datas?.Any() == true)
            {
                CommonHelper.ListRandom(datas);
                return datas.Take(6).OrderByDescending(p => p.AnswerCount).ToList();
            }
            else
            {
                return new List<QuestionDto>();
            }
        }

        public bool UpdateQuestionViewCount(Guid questionId)
        {
            return _repo.UpdateViewCount(questionId);
        }

        /// <summary>
        /// 通过学部ID统计问答数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        public SchQuestionData GetQuestionDataByID(Guid SchoolSectionId)
        {
            return _repo.GetSchoolQuestionDataByID(SchoolSectionId);
        }
        public List<SchQuestionDataEx> GetQuestionDataByIDs(List<Guid> schoolSectionIds)
        {
            return _repo.GetSchoolQuestionDataByIDs(schoolSectionIds);
        }

        List<SchoolTotal> GetQuestionCountBySIDs(List<Guid> guids, List<int> state = null)
        {
            return _repo.GetSchoolQuestionCountBuSchoolSectionIDs(guids, state);
        }

        public List<SchoolTotalDto> GetAnswerCount(List<Guid> questionIds)
        {
            var result = _repo.GetAnswerCount(questionIds);
            return result.Select(q => new SchoolTotalDto { Id = q.Id, Total = q.Total }).ToList();
        }

        public List<SchoolTotalDto> GetQuestionAnswerCount(int pageNo, int pageSize)
        {
            var result = _repo.GetQuestionAnswerCount(pageNo, pageSize);
            return result.Select(q => new SchoolTotalDto { Id = q.Id, Total = q.Total }).ToList();
        }

        public List<QuestionDto> PageQuestionByQuestionIds(Guid QueryUserId, List<Guid> questionIds, bool IsSelf)
        {
            List<QuestionInfo> questions = new List<QuestionInfo>();
            questions = _repo.PageQuestionByQuestionIds(questionIds, IsSelf);
            if (!questions.Any())
                return new List<QuestionDto>();

            List<Guid> userIds = questions.GroupBy(q => q.UserId).Select(q => q.Key).ToList();
            List<Guid> Ids = questions.GroupBy(q => q.Id).Select(q => q.Key).ToList();
            GetQuestionCommon(Ids, QueryUserId, out List<GiveLike> Likes, out List<SchoolImage> Images);


            var QuestionHotAnswer = _answersInfoService.QuestionAnswersOrderByQuestionIds(questions, QueryUserId, 1);

            return questions.Select(x => CovertQuestionToDto(x, QueryUserId, Likes, Images, QuestionHotAnswer))?.ToList();
        }

        public List<QuestionItem> GetQuestionItems(List<Guid> Ids)
            => _repo.GetQuestionItems(Ids);


        public List<QuestionInfo> GetQuestionsBySchoolUser(List<Guid> extIds, List<Guid> userIds)
        {
            var questions = _repo.GetList(question => extIds.Contains(question.SchoolSectionId) && userIds.Contains(question.UserId)).ToList();
            return questions;
        }

        public List<QuestionInfo> GetQuestionInfoByIds(List<Guid> Ids)
        {
            return _repo.GetQuestionInfoByIds(Ids);
        }


        public List<QuestionDto> GetHotQuestion(DateTime startTime, DateTime endTime, int count = 10)
        {
            var result = _repo.GetHotQuestion(startTime, endTime, count);
            return result.Select(x => CovertQuestionToDto(x, Guid.Empty, new List<GiveLike>(), new List<SchoolImage>(), new List<AnswerInfoDto>())).ToList();
        }

        public PaginationModel<QuestionDto> SearchQuestions(SearchQuestionQuery query, Guid loginUserId)
        {
            var searchResult = _searchService.SearchQuestion(query);
            var ids = searchResult.Questions.Select(q => q.Id);
            //return SearchQuestions(ids, loginUserId);
            return PaginationModel<QuestionDto>.Build(SearchQuestions(ids, loginUserId), searchResult.Total);
        }

        public List<QuestionDto> SearchQuestions(IEnumerable<Guid> ids, Guid loginUserId)
        {
            var qeustions = GetQuestionByIds(ids.ToList(), loginUserId);
            var talents = _talentRepository.GetTalentsByUser(qeustions.Select(s => s.UserId).ToArray());

            //使用原有序列, 避免排序差异
            var data = ids
                .Where(q => qeustions.Any(p => p.Id == q))
                .Select(q =>
                {
                    var question = qeustions.First(p => p.Id == q);

                    question.TalentType = talents.FirstOrDefault(s => s.user_id == question.UserId)?.type;
                    return question;
                })
                .ToList();
            return data;
        }

        public async Task<List<UserQuestionDto>> SearchUserQuestions(IEnumerable<Guid> ids, Guid loginUserId)
        {
            IEnumerable<UserQuestionQueryDto> questions = this._repo.GetUserQuestions(ids);

            var answerIds = questions.Where(s=>s.TopReplyAnswerId!= null).Select(s=>s.TopReplyAnswerId.Value).ToList();
            var answers = _answersInfoService.GetAnswerInfoDtoByIds(answerIds);
            var userIds = questions.Select(s=>s.UserId).Concat(answers.Select(s=>s.UserId));
            var users = await _talentRepository.GetTalentUsers(userIds);

            var schools = await _schoolRepository.GetSchoolExtAggs(questions.Select(s=>s.ExtId));
            List<Guid> collections = _collectionRepository.GetCollection(ids.ToList(), loginUserId);
            var images = _imageRepository.GetImageByDataSourceId(ids.ToList(), ImageType.Question);

            //使用原有序列, 避免排序差异
            var data = ids
                .Where(q => questions.Any(p => p.Id == q))
                .Select(q =>
                {
                    var question = questions.First(p => p.Id == q);

                    UserQuestionDto dto = new UserQuestionDto
                    {
                        Id = question.Id,
                        ExtId = question.ExtId,
                        UserId = question.UserId,
                        No = question.No,
                        ReplyCount = question.ReplyCount,
                        Content = question.Content,
                        CreateTime = question.CreateTime,
                        IsCollection = collections.Any(s => s == question.Id),
                        IsAnony = question.IsAnony,
                        SchoolQuestionCount = question.SchoolQuestionCount,
                        Images = images.Where(s => s.DataSourcetId == q).Select(s => s.ImageUrl)
                    };
                    if (question.TopReplyAnswerId.HasValue)
                    {
                        AnswerInfoDto answer = answers.FirstOrDefault(s => s.Id == question.TopReplyAnswerId.Value);
                        if (answer != null)
                        {
                            UserAnswerDto answerDto = new UserAnswerDto
                            {
                                Id = answer.Id,
                                Content = answer.AnswerContent,
                                FormatterCreateTime = answer.AddTime,
                                ReplyCount = answer.ReplyCount,
                                IsAnony = answer.IsAnony,
                                User = Enumerable.FirstOrDefault(users, delegate (TalentUser s) {
                                    return s.UserId == answer.UserId;
                                })
                            };
                            dto.Answer = answerDto;
                        }
                    }
                 
                    dto.User = users.FirstOrDefault(s => s.UserId == question.UserId);
                    dto.School = schools.FirstOrDefault(s => s.ExtId == question.ExtId);
                    return dto;
                })
                .ToList();
            return data;
        }
    }
}
