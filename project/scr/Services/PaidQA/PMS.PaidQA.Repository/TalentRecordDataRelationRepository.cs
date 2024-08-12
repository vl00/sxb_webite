using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class TalentRecordDataRelationRepository : Repository<TalentRecordDataRelation, PaidQADBContext>, ITalentRecordDataRelationRepository
    {
        PaidQADBContext _paidQADBContext;
        public TalentRecordDataRelationRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

        public async Task<(IEnumerable<TalentRecordDataRelation> data,int total)> GetTalentRecordDatas(Guid userId,TalentRecordDataRelationDataType dataType, int pageIndex = 1, int pageSize = 10)
        {
            int offset = (pageIndex - 1) * pageSize;
            string sql = @"
SELECT * FROM TalentRecordDataRelation
WHERE DataType = @dataType and UserId = @userId
Order By Sort,ID
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;
SELECT count(1) FROM TalentRecordDataRelation
WHERE DataType = @dataType and UserId = @userId
";
            using (var multi = _paidQADBContext.QueryMultiple(sql, new { userId, dataType, offset, limit = pageSize }))
            {
               var datas = await multi.ReadAsync<TalentRecordDataRelation>();
               int total = await multi.ReadFirstAsync<int>();
                return (datas, total);
            }
        }



    }
}
