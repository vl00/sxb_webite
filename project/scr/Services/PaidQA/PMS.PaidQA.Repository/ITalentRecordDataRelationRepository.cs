using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public interface ITalentRecordDataRelationRepository : IRepository<TalentRecordDataRelation>
    {


        Task<(IEnumerable<TalentRecordDataRelation> data, int total)> GetTalentRecordDatas(Guid userId, TalentRecordDataRelationDataType dataType, int pageIndex = 1, int pageSize = 10);
    }
}
