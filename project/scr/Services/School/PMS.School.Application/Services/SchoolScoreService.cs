using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.CommentsManage.Application.Common;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Entities;
using PMS.School.Domain.Entities.Mongo;
using PMS.School.Domain.IMongo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;

namespace PMS.School.Application.Services
{
    public class SchoolScoreService : ISchoolScoreService
    {
        private readonly ISchoolScoreRepository _scoreRepo;
        IEasyRedisClient _easyRedisClient;
        ISchoolMongoRepository _schoolMongoRepository;

        public SchoolScoreService(ISchoolScoreRepository scoreRepo, IEasyRedisClient easyRedisClient, ISchoolMongoRepository schoolMongoRepository)
        {
            _schoolMongoRepository = schoolMongoRepository;
            _easyRedisClient = easyRedisClient;
            _scoreRepo = scoreRepo;
        }

        public void SyncSchoolScore(SchoolScoreDto schoolScore)
        {
            var score = _scoreRepo.GetSchoolScore(schoolScore.SchoolId, schoolScore.SchoolSectionId);

            if (score == null)
            {
                _scoreRepo.AddSchoolScore(new SchoolScore
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
            else if (score.LastCommentTime < schoolScore.LastCommentTime)
            {
                _scoreRepo.UpdateSchoolScore(new SchoolScore
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
        }

        public void SyncSchoolQuestionTotal(SchoolScoreDto schoolScore)
        {
            var score = _scoreRepo.GetSchoolScore(schoolScore.SchoolId, schoolScore.SchoolSectionId);

            if (score == null)
            {
                _scoreRepo.AddSchoolScore(new SchoolScore
                {
                    SchoolId = schoolScore.SchoolId,
                    SchoolSectionId = schoolScore.SchoolSectionId,
                    AggScore = 0,
                    TeachScore = 0,
                    ManageScore = 0,
                    LifeScore = 0,
                    HardScore = 0,
                    EnvirScore = 0,
                    CommentCount = 0,
                    AttendCommentCount = 0,
                    LastQuestionTime = schoolScore.LastQuestionTime,
                    QuestionCount = schoolScore.QuestionCount
                });
            }
            else if (score.LastQuestionTime < schoolScore.LastQuestionTime)
            {
                _scoreRepo.UpdateSchoolQuestionTotal(new SchoolScore
                {
                    LastQuestionTime = schoolScore.LastQuestionTime,
                    QuestionCount = schoolScore.QuestionCount,
                    SchoolId = schoolScore.SchoolId,
                    SchoolSectionId = schoolScore.SchoolSectionId
                });
            }
        }

        public SchoolScoreDto GetSchoolScore(Guid schoolId, Guid schoolSectionId)
        {
            var score = _scoreRepo.GetSchoolScore(schoolId, schoolSectionId);
            if (score != null)
            {
                return new SchoolScoreDto()
                {
                    AggScore = score.AggScore,
                    SchoolSectionId = score.SchoolSectionId,
                    SchoolId = score.SchoolId,
                    EnvirScore = score.EnvirScore,
                    HardScore = score.HardScore,
                    LifeScore = score.LifeScore,
                    ManageScore = score.ManageScore,
                    TeachScore = score.TeachScore
                };
            }
            else
            {
                return new SchoolScoreDto();
            }
        }

        public bool UpdateCommentTotal(Guid SchoolSectionId, DateTime lastCommentTime)
        {
            return _scoreRepo.UpdateCommentTotal(SchoolSectionId, lastCommentTime);
        }

        public bool UpdateQuestionTotal(Guid schoolSectionId, DateTime lastQuestionTime)
        {
            return _scoreRepo.UpdateQuestionTotal(schoolSectionId, lastQuestionTime);
        }

        public int GetSchCommentAggScore(Guid eid)
        {
            var score = _scoreRepo.GetSchCommentAggScore(eid);
            return SchoolScoreToStart.GetCurrentSchoolstart(score);
        }

        public async Task<Dictionary<int, double>> GetAvgScoreByAreaCode(int areaCode, string schFType = null)
        {
            if (areaCode < 1) return null;
            var redisKey = $"SchoolScore:AreaCode_{areaCode}";
            if (!string.IsNullOrWhiteSpace(schFType)) redisKey += $":SchoolType_{schFType}";
            return await _easyRedisClient.GetOrAddAsync(redisKey, async () =>
            {
                return await _scoreRepo.GetAvgByAreaCode(areaCode, schFType);
            }, TimeSpan.FromDays(1));
        }

        public async Task<Dictionary<int, double>> GetAvgScore(string schFType = null)
        {
            var redisKey = $"SchoolScore";
            if (!string.IsNullOrWhiteSpace(schFType)) redisKey += $":SchoolType_{schFType}";
            return await _easyRedisClient.GetOrAddAsync(redisKey, async () =>
            {
                return await _scoreRepo.GetAvgScore(schFType);
            }, TimeSpan.FromDays(1));
        }

        public async Task<Dictionary<int, double>> GetAvgScoreByCityCode(int cityCode, string schFType = null)
        {
            if (cityCode < 1) return null;
            var redisKey = $"SchoolScore:CityCode_{cityCode}";
            if (!string.IsNullOrWhiteSpace(schFType)) redisKey += $":SchoolType_{schFType}";
            return await _easyRedisClient.GetOrAddAsync(redisKey, async () =>
            {
                return await _scoreRepo.GetAvgByCityCode(cityCode, schFType);
            }, TimeSpan.FromDays(1));
        }

        public async Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType)
        {
            if (cityCode < 1 || string.IsNullOrWhiteSpace(schFType)) return 0;
            return (await _scoreRepo.GetSchoolRankingInCity(cityCode, score, schFType)).CutDoubleWithN(2);
        }

        public async Task<int> GetSchoolCountByCityAndSchFType(int cityCode, string schFType)
        {
            if (cityCode < 1 || string.IsNullOrWhiteSpace(schFType)) return 0;
            var str_RedisKey = $"Mongo:SchoolCount:CityCode_{cityCode}_SchFType_{schFType}";
            return await _easyRedisClient.GetOrAddAsync(str_RedisKey, () =>
            {
                return _schoolMongoRepository.GetSchoolCountViaSchFtypeCity(schFType, cityCode);
            }, TimeSpan.FromDays(1));
        }

        public async Task<int> GetSchoolCountByAreaAndSchFType(int areaCode, string schFType)
        {
            if (areaCode < 1 || string.IsNullOrWhiteSpace(schFType)) return 0;
            var str_RedisKey = $"Mongo:SchoolCount:AreaCode_{areaCode}_SchFType_{schFType}";
            return await _easyRedisClient.GetOrAddAsync(str_RedisKey, () =>
            {
                return _schoolMongoRepository.GetSchoolCountViaSchFtypeArea(schFType, areaCode);
            }, TimeSpan.FromDays(1));
        }

        public async Task<int> GetSchoolCount(string schFtype, int areaCode, int minScore, int maxScore)
        {
            if (string.IsNullOrWhiteSpace(schFtype) || areaCode < 1) return 0;
            return await _schoolMongoRepository.GetSchoolCount(schFtype, areaCode, minScore, maxScore);
        }

        public async Task<int> GetSchoolCount(string schFtype, int areaCode, int maxScore)
        {
            if (string.IsNullOrWhiteSpace(schFtype) || areaCode < 1) return 0;
            return await _schoolMongoRepository.GetSchoolCount(schFtype, areaCode, maxScore);
        }

        public async Task<GDParams> GetSurroundInfo(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            return await _schoolMongoRepository.GetGDParamsByEID(eid);
        }

        public async Task<GDParams> GetSurroundAvgInfo(string schFtype, int areaCode)
        {
            if (string.IsNullOrWhiteSpace(schFtype) || areaCode < 1) return null;
            var str_RedisKey = $"Mongo:GDParamsAvg:AreaCode_{areaCode}:SchFType_{schFtype}";
            return await _easyRedisClient.GetOrAddAsync(str_RedisKey, () =>
             {
                 return _schoolMongoRepository.GetGDParamsAvg(schFtype, areaCode);
             }, TimeSpan.FromDays(1));
        }

        public async Task<double> GetLowerPercent(string schFType, int areaCode, int indexID, double score)
        {
            if (string.IsNullOrWhiteSpace(schFType) || areaCode < 1 || indexID < 1 || score < 1) return 0;
            return (await _scoreRepo.GetLowerPercent(schFType, areaCode, indexID, score)).CutDoubleWithN(2);
        }

        public async Task<IEnumerable<KeyValuePair<Guid, int>>> GetSchoolAggScores(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                return await _scoreRepo.GetAggScoreByEIDs(eids);
            }
            return null;
        }
    }
}
