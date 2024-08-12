using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using System.Threading.Tasks;

    public interface IArticleChoiceHotPointRepository : IBaseRepository<ArticleChoiceHotPoint>
    {
        Task<(IEnumerable<article> data, int total)> GetArticles(Guid id, int offset, int limit);
    }
}
