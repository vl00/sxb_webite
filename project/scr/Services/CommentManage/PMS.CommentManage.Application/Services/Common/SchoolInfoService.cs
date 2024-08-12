using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System.Linq;

namespace PMS.CommentsManage.Application.Services.Common
{
    public class SchoolInfoService : ISchoolInfoService
    {

        private readonly ISchoolRepository _repositorySchool;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly IEasyRedisClient _redisClient;
        public SchoolInfoService(ISchoolRepository repositorySchool,
            ISchoolCommentScoreService commentScoreService,
            IEasyRedisClient redisClient)
        {
            _repositorySchool = repositorySchool;
            _commentScoreService = commentScoreService;
            _redisClient = redisClient;
        }


        public List<SchoolInfoDto> GetCurrentSchoolAllExt(Guid sid)
        {

            var list = _repositorySchool.GetCurrentSchoolAllExt(sid);

            List<SchoolInfoDto> schoolInfoDtos = new List<SchoolInfoDto>();

            foreach (var item in list)
            {
                SchoolInfoDto schoolInfoDto = new SchoolInfoDto
                {
                    SchoolName = item.SchoolName,
                    SchoolSectionId = item.Id,
                    SchoolId = item.SchoolId
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }
            return schoolInfoDtos;
        }

        public List<SchoolInfoDto> GetSchoolName(List<Guid> eid)
        {
            if (!eid.Any())
            {
                return new List<SchoolInfoDto>();
            }

            var list = _repositorySchool.GetSchoolName(eid);
            List<SchoolInfoDto> schoolInfoDtos = new List<SchoolInfoDto>();

            foreach (var item in list)
            {
                SchoolInfoDto schoolInfoDto = new SchoolInfoDto
                {
                    SchoolName = item.SchoolName,
                    SchoolSectionId = item.Id,
                    SchoolId = item.SchoolId,
                    SectionCommentTotal = item.CommentTotal,
                    Lodging = item.Lodging,
                    Sdextern = item.Sdextern,
                    SchoolType = item.SchoolType,
                    SchoolAvgScore = (int)item.SchoolAvgScore,
                    SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(item.SchoolAvgScore),
                    SchoolNo = item.SchoolNo
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }
            return schoolInfoDtos;
        }

        public List<SchoolInfoDto> GetSchoolStatuse(List<Guid> eid)
        {
            if (!eid.Any())
            {
                return new List<SchoolInfoDto>();
            }

            var list = _repositorySchool.GetSchoolStatuse(eid);
            List<SchoolInfoDto> schoolInfoDtos = new List<SchoolInfoDto>();

            foreach (var item in list)
            {
                string schoolName = item.Status != 3 ? item.SchoolName + " 已下架" : !item.IsValid ? item.SchoolName + " 已下架" : item.SchoolName;

                SchoolInfoDto schoolInfoDto = new SchoolInfoDto
                {
                    SchoolName = schoolName,
                    SchoolSectionId = item.Id,
                    SchoolId = item.SchoolId
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }
            return schoolInfoDtos;
        }

        public async Task<SchoolInfoDto> QuerySchoolInfo(Guid schoolSectionId)
        {
            string schoolSectionKey = "SchoolSectionInfos:{0}";
            //根据学校分部id得到学校id
            var School = await _redisClient.GetOrAddAsync(
                string.Format(schoolSectionKey, schoolSectionId),
                () => { return _repositorySchool.GetSchoolExtension(schoolSectionId); },
                new TimeSpan(0, 10, 0));

            if (School == null)
            {
                return default;
            }

            string schoolSectionScoreKey = "SchoolSectionScores:{0}";
            var SchoolSectionScore = await _redisClient.GetOrAddAsync(
                string.Format(schoolSectionScoreKey, schoolSectionId),
                () => { return _commentScoreService.GetSchoolScoreById(schoolSectionId); },
                new TimeSpan(0, 1, 0));

            if (schoolSectionScoreKey == null)
            {
                return default;
            }

            string schoolScoreKey = "SchoolScores:{0}";
            var SchoolScore = await _redisClient.GetOrAddAsync(
                string.Format(schoolScoreKey, School.SchoolId),
                () => { return _commentScoreService.GetSchoolScoreBySchoolId(School.SchoolId); },
                new TimeSpan(0, 1, 0));


            return new SchoolInfoDto
            {
                SchoolId = School.SchoolId,
                SchoolName = School.SchoolName,
                CommentTotal = SchoolScore.CommentCount,
                SectionCommentTotal = SchoolSectionScore.CommentCount,
                SectionQuestionTotal = SchoolSectionScore.QuestionCount,
                SchoolAvgScore = (int)Math.Ceiling(SchoolSectionScore.AggScore),
                SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(SchoolSectionScore.AggScore),
                SchoolType = School.SchoolType,
                Lodging = School.Lodging,
                Sdextern = School.Sdextern,
                SchoolSectionId = School.Id,
                //上学帮认证，后续调用接口获取该学校是否已经成功认证
                IsAuth = false,
                SchoolNo = School.SchoolNo
            };
        }

        public List<SchoolInfoDto> GetSchoolSectionByIds(List<Guid> SchoolSectionIds)
        {
            if (!SchoolSectionIds.Any())
            {
                return new List<SchoolInfoDto>();
            }

            //批量获取学校分部数据
            var schoolExtension = _repositorySchool.GetSchoolExtListByBranchIds(SchoolSectionIds);
            //学校ids
            List<Guid> SchoolIds = schoolExtension.Select(x => x.Sid)?.ToList();
            if (!SchoolIds.Any())
            {
                return new List<SchoolInfoDto>();
            }
            //批量获取学校分部统计数据
            var SchoolSectionScore = _commentScoreService.PageSchoolCommentScore(new Model.Query.PageCommentScoreQuery() { SchoolIds = SchoolSectionIds });
            //根据批量学校id获取学校统计数据

            var SchoolScore = _commentScoreService.SchoolScoreOrder(SchoolIds);

            List<SchoolInfoDto> schoolInfoDtos = new List<SchoolInfoDto>();

            foreach (var item in schoolExtension)
            {
                var school = SchoolScore.Find(s => s.SchoolSectionId == item.ExtId);
                var schoolSections = SchoolSectionScore.Find(s => s.SchoolSectionId == item.ExtId);

                SchoolInfoDto schoolInfoDto = new SchoolInfoDto
                {
                    SchoolName = item.Name,
                    CommentTotal = school == null ? 0 : school.CommentCount,
                    SectionCommentTotal = schoolSections == null ? 0 : schoolSections.CommentCount,
                    SectionQuestionTotal = schoolSections == null ? 0 : schoolSections.QuestionCount,
                    SchoolAvgScore = schoolSections == null ? 0: Math.Round((decimal)(schoolSections.AggScore / 20),1),
                    SchoolStars = schoolSections == null ? 0 : SchoolScoreToStart.GetCurrentSchoolstart(schoolSections.AggScore),
                    SchoolType = (PMS.School.Domain.Common.SchoolType)item.Type,
                    Lodging = item.Lodging,
                    Sdextern = item.Sdextern,
                    SchoolSectionId = item.ExtId,
                    SchoolId = item.Sid,
                    //上学帮认证，后续调用接口获取该学校是否已经成功认证
                    IsAuth = false,
                    SchoolNo = item.SchoolNo
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }

            return schoolInfoDtos;
        }


        public List<SchoolInfoQaDto> GetSchoolSectionQaByIds(List<Guid> SchoolSectionIds)
        {
            //批量获取学校分部数据
            var schoolExtension = _repositorySchool.GetSchoolExtListByBranchIds(SchoolSectionIds);
            //学校ids
            List<Guid> SchoolIds = schoolExtension.Select(x => x.Sid)?.ToList();

            List<SchoolInfoQaDto> schoolInfoDtos = new List<SchoolInfoQaDto>();

            foreach (var item in schoolExtension)
            {
                SchoolInfoQaDto schoolInfoDto = new SchoolInfoQaDto
                {
                    SchoolName = item.Name,
                    SchoolType = (PMS.School.Domain.Common.SchoolType)item.Type,
                    IsInternactioner = item.Type == (int)PMS.School.Domain.Common.SchoolType.International,
                    Lodging = item.Lodging,
                    Sdextern = item.Sdextern,
                    SchoolSectionId = item.ExtId,
                    SchoolId = item.Sid,
                    //上学帮认证，后续调用接口获取该学校是否已经成功认证
                    IsAuth = false,
                    SchoolNo = item.SchoolNo
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }
            return schoolInfoDtos;
        }

        /// <summary>
        /// 学校点评、提问
        /// </summary>
        /// <param name="QueryComment">搜索类型 true：点评，false：提问</param>
        /// <param name="City"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<SchoolInfoDto> QuerySchoolCards(bool QueryComment, int City, int PageIndex, int PageSize)
        {
            List<SchoolInfoDto> schoolInfoDtos = new List<SchoolInfoDto>();
            var result = _repositorySchool.QuerySchoolCard(QueryComment, City, PageIndex, PageSize);

            foreach (var item in result)
            {
                SchoolInfoDto schoolInfoDto = new SchoolInfoDto
                {
                    SchoolName = item.SchoolName,
                    SchoolType = (PMS.School.Domain.Common.SchoolType)item.Type,
                    Lodging = item.Lodging,
                    Sdextern = item.Sdextern,
                    SchoolSectionId = item.SchoolSectionId,
                    SchoolId = item.SchoolId,
                    SectionCommentTotal = item.CommentCount,
                    SectionQuestionTotal = item.QuestionCount,
                    SchoolAvgScore = (int)Math.Ceiling(item.AggScore),
                    SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(item.AggScore),
                    //上学帮认证，后续调用接口获取该学校是否已经成功认证
                    IsAuth = false
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }
            return schoolInfoDtos;
        }

        public SchoolInfoDto QueryESchoolInfo(Guid eid)
        {
            var item = _repositorySchool.GetSchoolInfo(eid);
            if (item == null)
            {
                return null;
            }

            return new SchoolInfoDto()
            {
                SchoolName = item.SchoolName,
                SchoolType = (PMS.School.Domain.Common.SchoolType)item.SchoolType,
                Sdextern = item.Sdextern,
                Lodging = item.Lodging,
                SchoolSectionId = item.Id,
                SchoolId = item.SchoolId,
                SchoolAvgScore = Math.Round(item.SchoolAvgScore),
                SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(item.SchoolAvgScore),
                SchoolGrade = item.SchoolGrade,
                SchoolCommentAvgScore=item.SchoolAvgScore,
                //上学帮认证，后续调用接口获取该学校是否已经成功认证
                IsAuth = false,
                SchoolNo = item.SchoolNo
            };
        }
    }
}
