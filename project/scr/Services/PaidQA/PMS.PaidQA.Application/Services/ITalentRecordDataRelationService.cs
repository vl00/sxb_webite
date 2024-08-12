using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PMS.PaidQA.Domain.Dtos;
using PMS.OperationPlateform.Domain.DTOs;
using ProductManagement.API.Http.Result;

namespace PMS.PaidQA.Application.Services
{
    public interface ITalentRecordDataRelationService : IApplicationService<TalentRecordDataRelation>
    {

        Task<(IEnumerable<ArticleDto> data, int total)> GetArticleDtos(Guid userId, int pageIndex=1, int pageSize=10);

        Task<(IEnumerable<TalentQACase> data, int total)> GetQACases(Guid userId, int pageIndex = 1, int pageSize = 10);

        Task<(IEnumerable<LectureItem> data, int total)> GetLives(Guid userId, int pageIndex = 1, int pageSize = 10);

    }
}