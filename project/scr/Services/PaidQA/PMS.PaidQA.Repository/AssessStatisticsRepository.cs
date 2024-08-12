using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class AssessStatisticsRepository : Repository<AssessStatisticsInfo, PaidQADBContext>, IAssessStatisticsRepository
    {
        PaidQADBContext _paidQADBContext;
        public AssessStatisticsRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

        public async Task<bool> CheckExist(AssessType type, DateTime date)
        {
            return (await _paidQADBContext.QuerySingleAsync<int>("Select Count(1) From [AssessStatisticsInfo] Where Date = @date And AssessType = @type", new { type, date })) > 0;
        }

        public async Task<AssessStatisticsInfo> Get(AssessType assessType, DateTime date)
        {
            return await _paidQADBContext.QuerySingleAsync<AssessStatisticsInfo>("Select * From [AssessStatisticsInfo] Where [CountTime] = @date And AssessType = @assessType", new { assessType, date });
        }

        public async Task<IEnumerable<AssessStatisticsInfo>> GetUnCounteds(int count = 0)
        {
            var filed = "*";
            if (count > 0) filed = $" top {count} * ";
            return await _paidQADBContext.GetByAsync<AssessStatisticsInfo>("IsCounted = 0", fileds: new string[] { filed });
        }
    }
}
