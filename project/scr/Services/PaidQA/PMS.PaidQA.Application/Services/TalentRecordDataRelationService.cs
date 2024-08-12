using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Repository;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class TalentRecordDataRelationService : ApplicationService<TalentRecordDataRelation>, ITalentRecordDataRelationService
    {
        IArticleService _articleService;
        ITalentRecordDataRelationRepository _repository;
        ITalentQACaseRepository _talentQACaseRepository;
        ILiveServiceClient _liveServiceClient;
        public TalentRecordDataRelationService(ITalentRecordDataRelationRepository talentRecordDataRelationRepository, IArticleService articleService, ITalentQACaseRepository talentQACaseRepository, ILiveServiceClient liveServiceClient) : base(talentRecordDataRelationRepository)
        {
            _repository = talentRecordDataRelationRepository;
            _articleService = articleService;
            _talentQACaseRepository = talentQACaseRepository;
            _liveServiceClient = liveServiceClient;
        }

        public async Task<(IEnumerable<ArticleDto> data, int total)> GetArticleDtos(Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            var pageResult = await _repository.GetTalentRecordDatas(userId, TalentRecordDataRelationDataType.article, pageIndex, pageSize);
            if (!pageResult.data.Any())
            {
                return (new List<ArticleDto>(), 0);
            }
            var ids = pageResult.data.Select(tr => tr.DataId);
            var articles = _articleService.GetArticles(ids, false);
            if (articles?.Any() == true)
            {
                var joinResult = articles.Join(pageResult.data, (t1) => t1.Id, (t2) => t2.DataId, (data, relation) =>
                {
                    return new { data, relation };
                });
                articles = joinResult.OrderBy(j => j.relation.Sort).ThenBy(j => j.data.Id).Select(j => j.data);
                return (articles, pageResult.total);

            }
            else
            {
                return (new List<ArticleDto>(), pageResult.total);
            }


        }

        public async Task<(IEnumerable<TalentQACase> data, int total)> GetQACases(Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            var pageResult = await _repository.GetTalentRecordDatas(userId, TalentRecordDataRelationDataType.qacase, pageIndex, pageSize);
            if (!pageResult.data.Any())
            {
                return (new List<TalentQACase>(), 0);
            }
            var ids = pageResult.data.Select(tr => tr.DataId);
            var cases = await _talentQACaseRepository.GetByAsync(" id in @ids ", new { ids });
            if (cases?.Any() == true)
            {
                var joinResult = cases.Join(pageResult.data, (t1) => t1.Id, (t2) => t2.DataId, (data, relation) =>
                {
                    return new { data, relation };
                });
                cases = joinResult.OrderBy(j => j.relation.Sort).ThenBy(j => j.data.Id).Select(j => j.data);
                return (cases, pageResult.total);
            }
            else
            {
                return (new List<TalentQACase>(), pageResult.total);
            }


        }

        public async Task<(IEnumerable<LectureItem> data, int total)> GetLives(Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            var pageResult = await _repository.GetTalentRecordDatas(userId, TalentRecordDataRelationDataType.live, pageIndex, pageSize);
            if (!pageResult.data.Any())
            {
                return (new List<LectureItem>(), 0);
            }
            var ids = pageResult.data.Select(tr => tr.DataId).ToList();
            var lectureResult = await _liveServiceClient.QueryLectures(ids, null);
            if (lectureResult?.Items?.Any() == true)
            {
                var joinResult = lectureResult.Items.Join(pageResult.data, (t1) => t1.Id, (t2) => t2.DataId, (data, relation) =>
                {
                    return new { data, relation };
                });
                var lives = joinResult.OrderBy(j => j.relation.Sort).ThenBy(j => j.data.Id).Select(j => j.data);
                return (lives, pageResult.total);

            }
            else
            {
                return (new List<LectureItem>(), pageResult.total);
            }
        }


    }
}
