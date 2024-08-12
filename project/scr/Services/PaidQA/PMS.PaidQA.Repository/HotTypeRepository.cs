using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class HotTypeRepository : Repository<HotType, PaidQADBContext>, IHotTypeRepository
    {
        PaidQADBContext _paidQADBContext;
        public HotTypeRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [HotType] Where {str_Where}";
            return await _paidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }

        public async Task<IEnumerable<Guid>> GetAllOrderIDs()
        {
            var str_SQL = $@"SELECT
                            hq.OrderID
                        FROM
                            HotQuestion AS hq
                            LEFT JOIN HotType AS ht ON ht.ID = hq.HotTypeID
                        WHERE
                            ht.IsValid = 1";
            return await _paidQADBContext.QueryAsync<Guid>(str_SQL, new { });
        }

        public async Task<IEnumerable<Guid>> GetOrderIDByHotTypeID(Guid hotTypeID, int pageIndex = 1, int pageSize = 10, int sortType = 1)
        {
            var offset = --pageIndex * pageSize;
            var str_Join = @"LEFT JOIN Evaluate as e on e.OrderID = hq.OrderID";
            var str_Order = "e.Score DESC";
            switch (sortType)
            {
                case 2:
                    str_Join = string.Empty;
                    str_Order = "hq.ViewCount Desc";
                    break;
                case 3:
                    str_Join = "Left Join [Order] as o on o.ID = hq.OrderID";
                    str_Order = "o.CreateTime Desc";
                    break;
            }

            var str_SQL = $@"SELECT
                            hq.OrderID
                        FROM
                            HotQuestion AS hq
                            LEFT JOIN HotType AS ht ON ht.ID = hq.HotTypeID
                            {str_Join}
                        WHERE
                            ht.IsValid = 1
                            AND ht.ID = @hotTypeID
                        Order By
                            {str_Order}
                        OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            return await _paidQADBContext.QueryAsync<Guid>(str_SQL, new { hotTypeID });
        }

        public async Task<IEnumerable<(Guid, int)>> GetViewCountByOrderIDs(IEnumerable<Guid> ids)
        {
            var str_SQL = $"Select OrderID,ViewCount from HotQuestion WHERE OrderID IN @ids";
            return await _paidQADBContext.QueryAsync<(Guid, int)>(str_SQL, new { ids });
        }
    }
}
