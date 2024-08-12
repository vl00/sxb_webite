using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IArticleChoiceHotPointService
    {
        Task<IEnumerable<ArticleChoiceHotPoint>> GetHotPoints(int city);
        Task<PaginationModel<ArticleDto>> GetArticles(Guid id, int offset, int limit);
    }
}
