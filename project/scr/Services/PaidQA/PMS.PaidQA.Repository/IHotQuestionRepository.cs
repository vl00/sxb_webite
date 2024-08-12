using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace PMS.PaidQA.Repository
{
    public interface IHotQuestionRepository : IRepository<HotQuestion>
    {
        Task<int> Count(string str_Where, object param);
        Task<HotQuestion> GetByOrderID(Guid id);
        Task<HotQuestionExtend> GetWithTypeNameByOrderID(Guid id);
        Task<IEnumerable<(Guid, Guid)>> GetRandomOrderIDByGradeSort(int sort, int num = 3);
        Task<IEnumerable<(Guid, Guid)>> GetRandomOrderIDByGradeSorts(IEnumerable<int> sorts, int num = 3);
    }
}
