using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.RabbitMQ.Message;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class SchoolCommentScoreService : ISchoolCommentScoreService
    {
        private readonly ISchoolCommentScoreRepository _scoreRepository;


        public SchoolCommentScoreService(ISchoolCommentScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;
        }

        public int AddSchoolComment(SchoolCommentScore commentScore)
        {
            return _scoreRepository.AddSchoolComment(commentScore);
        }


        public decimal GetAvgScoreBybaraBranchSchool(Guid SchooId)
        {
            return _scoreRepository.GetAvgScoreBybaraBranchSchool(SchooId);
        }

        public bool UpdataSchoolQuestionTotal(Guid SchoolId, Guid SchoolSectionId,DateTime AddTime)
        {
            var score = _scoreRepository.GetSchoolScore(SchoolId, SchoolSectionId);
            if (score != null)
            {
                return _scoreRepository.UpdataQuestionTotalNewTime(SchoolSectionId, AddTime);
            }
            else
            {
                return _scoreRepository.AddSchoolScore(new SchoolScore()
                {
                    SchoolId = SchoolId,
                    SchoolSectionId = SchoolSectionId,
                    AggScore = 0,
                    TeachScore = 0,
                    ManageScore = 0,
                    LifeScore = 0,
                    HardScore = 0,
                    EnvirScore = 0,
                    CommentCount = 0,
                    AttendCommentCount = 0,
                    LastQuestionTime = AddTime,
                    QuestionCount = 1
                });
            }
        }

        public bool UpdataSchoolCommentTotal(Guid SchoolId, Guid SchoolSectionId,SchoolCommentScore commentScore,DateTime AddTime) 
        {
            var score = _scoreRepository.GetSchoolScore(SchoolId, SchoolSectionId);
            if (score != null)
            {
                int AttendCommentCount = commentScore.IsAttend ? 1 : 0;
               
                return _scoreRepository.UpdateSchoolScore(new SchoolScore()
                {
                    SchoolSectionId = SchoolSectionId,
                    SchoolId = SchoolId,
                    AttendCommentCount = AttendCommentCount,
                    CommentCount =1,
                    AggScore =  commentScore.AggScore,
                    EnvirScore = commentScore.EnvirScore,
                    HardScore = commentScore.HardScore,
                    LifeScore = commentScore.LifeScore,
                    ManageScore = commentScore.ManageScore,
                    TeachScore = commentScore.TeachScore,
                    LastCommentTime = AddTime
                });
            }
            else 
            {
                return _scoreRepository.AddSchoolScore(new SchoolScore()
                {
                    SchoolId = SchoolId,
                    SchoolSectionId = SchoolSectionId,
                    AggScore = commentScore.AggScore,
                    TeachScore = commentScore.TeachScore,
                    ManageScore = commentScore.ManageScore,
                    LifeScore = commentScore.LifeScore,
                    HardScore = commentScore.HardScore,
                    EnvirScore = commentScore.EnvirScore,
                    CommentCount = 1,
                    AttendCommentCount = commentScore.IsAttend ? 1 : 0,
                    LastQuestionTime = AddTime,
                    QuestionCount = 0
                });
            }
        }

        public SchoolCommentScoreDto GetSchoolCommentScore(Guid SchooId, Guid SchoolSectionId)
        {
            var result = _scoreRepository.GetSchoolScoreById(SchoolSectionId);

            return result==null?new SchoolCommentScoreDto(): new SchoolCommentScoreDto
            {
                SchoolId = result.SchoolId,
                SchoolSectionId = result.SchoolSectionId,
                AggScore = result.AggScore,
                TeachScore = result.TeachScore,
                ManageScore = result.ManageScore,
                LifeScore = result.LifeScore,
                HardScore = result.HardScore,
                EnvirScore = result.EnvirScore,
                UpdateTime = result.UpdateTime
            };
        }

        //评分统计列表
        public List<SchoolCommentScoreDto> PageSchoolCommentScore(PageCommentScoreQuery query)
        {
            var list  = _scoreRepository.GetSchoolScoreBySchool(query.SchoolIds);

            return list.Select(q => new SchoolCommentScoreDto {
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                AggScore = q.AggScore,
                //UpdateTime = DateTime.Now
                UpdateTime = q.UpdateTime,
                CommentCount = q.CommentCount,
                QuestionCount = q.QuestionCount
            }).ToList();
        }


        #region 学校分数统计
        public DateTime GetLastUpdateTime()
        {
            return _scoreRepository.GetLastUpdateTime();
        }

        public DateTime GetQuestionLastUpdateTime()
        {
            return _scoreRepository.GetQuestionLastUpdateTime();
        }

        public List<SchoolCommentScoreDto> PageSchoolCommentScoreByTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize)
        {
            List<SchoolCommentScore> list = _scoreRepository.PageSchoolCommentScoreByTime(startTime, endTime, pageNo, pageSize);

            return list.Select(q => new SchoolCommentScoreDto {
                SchoolId = q.SchoolComment.SchoolId,
                SchoolSectionId = q.SchoolComment.SchoolSectionId,
                AggScore = q.AggScore,
                TeachScore = q.TeachScore,
                ManageScore = q.ManageScore,
                LifeScore = q.LifeScore,
                HardScore = q.HardScore,
                EnvirScore = q.EnvirScore,
                IsAttend = q.IsAttend,
                UpdateTime = q.SchoolComment.AddTime,
                UserId = q.SchoolComment.CommentUserId
            }).ToList();
        }

        public bool UpdateSchoolScore(SchoolScoreDto schoolScore)
        {

            bool isExist = _scoreRepository.IsExistSchoolScore(schoolScore.SchoolSectionId);

            bool result;
            if (isExist)
            {
                result = _scoreRepository.UpdateSchoolScore(new SchoolScore
                {
                    SchoolId = schoolScore.SchoolId,
                    SchoolSectionId = schoolScore.SchoolSectionId,
                    AggScore = schoolScore.AggScore,
                    TeachScore = schoolScore.TeachScore,
                    ManageScore = schoolScore.ManageScore,
                    LifeScore = schoolScore.LifeScore,
                    HardScore = schoolScore.HardScore,
                    EnvirScore = schoolScore.EnvirScore,
                    CommentCount = schoolScore.CommentCount,
                    AttendCommentCount = schoolScore.AttendCommentCount,
                    LastCommentTime = schoolScore.LastCommentTime
                });
            }
            else
            {
                result = _scoreRepository.AddSchoolScore(new SchoolScore
                {
                    SchoolId = schoolScore.SchoolId,
                    SchoolSectionId = schoolScore.SchoolSectionId,
                    AggScore = schoolScore.CommentCount>0? schoolScore.AggScore/ schoolScore.CommentCount:0,
                    TeachScore = schoolScore.AttendCommentCount>0?schoolScore.TeachScore/ schoolScore.AttendCommentCount:0,
                    ManageScore = schoolScore.AttendCommentCount > 0 ? schoolScore.ManageScore / schoolScore.AttendCommentCount : 0,
                    LifeScore = schoolScore.AttendCommentCount > 0 ? schoolScore.LifeScore / schoolScore.AttendCommentCount : 0,
                    HardScore = schoolScore.AttendCommentCount > 0 ? schoolScore.HardScore / schoolScore.AttendCommentCount : 0,
                    EnvirScore = schoolScore.AttendCommentCount > 0 ? schoolScore.EnvirScore / schoolScore.AttendCommentCount : 0,
                    CommentCount = schoolScore.CommentCount,
                    AttendCommentCount = schoolScore.AttendCommentCount,
                    LastCommentTime = schoolScore.LastCommentTime
                });

            }
            return result;
        }

        public bool UpdateSchoolScore(QuestionTotalByTimeDto questionTotal)
        {
            bool isExist = _scoreRepository.IsExistSchoolScore(questionTotal.SchoolSectionId);
            bool result;
            if (isExist)
            {
                result = _scoreRepository.UpdateQuestionTotal(new SchoolScore
                {
                    SchoolId = questionTotal.School,
                    SchoolSectionId = questionTotal.SchoolSectionId,
                   QuestionCount = questionTotal.Total,
                   LastQuestionTime = questionTotal.CreateTime
                });
            }
            else
            {
                result = _scoreRepository.AddSchoolScore(new SchoolScore
                {
                    SchoolId = questionTotal.School,
                    SchoolSectionId = questionTotal.SchoolSectionId,
                    AggScore = 0,
                    TeachScore = 0,
                    ManageScore = 0,
                    LifeScore = 0,
                    HardScore = 0,
                    EnvirScore = 0,
                    CommentCount = 0,
                    AttendCommentCount = 0,
                    LastCommentTime = DateTime.Now,
                    QuestionCount = questionTotal.Total,
                    LastQuestionTime = questionTotal.CreateTime
                });
            }
            return result;
        }

        public int SchoolCommentScoreCountByTime(DateTime startTime, DateTime endTime)
        {
            return _scoreRepository.SchoolCommentScoreCountByTime(startTime,endTime);
        }

        public SchoolScoreDto GetSchoolScoreById(Guid SchoolSectionId)
        {
           var result =   _scoreRepository.GetSchoolScoreById(SchoolSectionId);
            return result == null ? new SchoolScoreDto() : new SchoolScoreDto
            {
                SchoolId = result.SchoolId,
                SchoolSectionId = result.SchoolSectionId,
                AggScore = result.AggScore,
                TeachScore = result.TeachScore,
                ManageScore = result.ManageScore,
                LifeScore = result.LifeScore,
                HardScore = result.HardScore,
                EnvirScore = result.EnvirScore,
                CommentCount = result.CommentCount,
                AttendCommentCount = result.AttendCommentCount,
                LastCommentTime = result.LastCommentTime,
                QuestionCount = result.QuestionCount,
                LastQuestionTime = result.LastQuestionTime
            };
        }
        public SchoolScoreDto GetSchoolScoreBySchoolId(Guid SchooId)
        {
            var result = _scoreRepository.GetSchoolScoreBySchoolId(SchooId);

            return result == null ? new SchoolScoreDto() : new SchoolScoreDto
            {
                SchoolId = result.SchoolId,
                AggScore = result.AggScore,
                TeachScore = result.TeachScore,
                ManageScore = result.ManageScore,
                LifeScore = result.LifeScore,
                HardScore = result.HardScore,
                EnvirScore = result.EnvirScore,
                CommentCount = result.CommentCount,
                AttendCommentCount = result.AttendCommentCount,
            };
        }


        public List<SchoolScoreDto> ListNewSchoolScores(DateTime oldTime)
        {
            var result = _scoreRepository.ListNewSchoolScores(oldTime);
            return result.Select(q => new SchoolScoreDto {
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                AggScore = q.AggScore,
                TeachScore = q.TeachScore,
                ManageScore = q.ManageScore,
                LifeScore = q.LifeScore,
                HardScore = q.HardScore,
                EnvirScore = q.EnvirScore,
                CommentCount = q.CommentCount,
                AttendCommentCount = q.AttendCommentCount,
                LastCommentTime = q.LastCommentTime
            }).ToList();
        }

        public List<SchoolScoreDto> SchoolScoreOrder(List<Guid> SchoolIds)
        {
            var result = _scoreRepository.SchoolScoreOrder(SchoolIds);
            return result.Select(q => new SchoolScoreDto
            {
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                AggScore = q.AggScore,
                TeachScore = q.TeachScore,
                ManageScore = q.ManageScore,
                LifeScore = q.LifeScore,
                HardScore = q.HardScore,
                EnvirScore = q.EnvirScore,
                CommentCount = q.CommentCount,
                AttendCommentCount = q.AttendCommentCount,
                LastCommentTime = q.LastCommentTime
            }).ToList(); 
        }

        public bool UpdateQuestionTotal(SchoolScore schoolScore)
        {
            return _scoreRepository.UpdateQuestionTotal(schoolScore);
        }

        public List<SchoolScoreDto> ListNewQuestion(DateTime oldTime)
        {
            var result = _scoreRepository.ListNewQuestion(oldTime);
            return result.Select(q => new SchoolScoreDto
            {
                SchoolId = q.SchoolId,
                SchoolSectionId = q.SchoolSectionId,
                AggScore = q.AggScore,
                TeachScore = q.TeachScore,
                ManageScore = q.ManageScore,
                LifeScore = q.LifeScore,
                HardScore = q.HardScore,
                EnvirScore = q.EnvirScore,
                CommentCount = q.CommentCount,
                AttendCommentCount = q.AttendCommentCount,
                LastCommentTime = q.LastCommentTime,
                LastQuestionTime = q.LastQuestionTime,
                QuestionCount = q.QuestionCount
            }).ToList(); ;
        }
        #endregion
    }
}
