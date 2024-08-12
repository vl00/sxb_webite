using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class GoodsRankReposition: BaseRepository<GoodsRank>, IGoodsRankRepository
    {
        public GoodsRankReposition(OperationQueryDBContext db) : base(db)
        {
        }

        public IEnumerable<GoodsRank> GetGoodsByPage(int pageindex, int pagesize)
        {
            if(pageindex > 1 && pagesize > 0)
            {
                int LastPageIndex = pageindex - 1;
                int minrange = LastPageIndex * pagesize;

                string sql = $@"SELECT top {pagesize} * FROM GoodsRank where (id not in (select top {minrange} id from GoodsRank order by sortnum)) order by sortnum";
                return _db.Query<GoodsRank>(sql, new { });
            }
            else {
                string sql = $@"SELECT top {pagesize} * FROM GoodsRank order by sortnum";
                return _db.Query<GoodsRank>(sql, new { });
            }
    
            
        }

    }
}
