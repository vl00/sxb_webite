using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Data
{
    public class OrderStatusLogRepository
    {
        private readonly DBContext _dbcontext;
        public OrderStatusLogRepository(DBContext dbcontext)
        {
            this._dbcontext = dbcontext;
        }
        public OrderStatusLogDomain GetOrderStatusLogById(int id)
        {
            var result = _dbcontext.QuerySingle<OrderStatusLogDomain>("SELECT * FROM health_order.hm_orderstatuslog2 WHERE hm_reflog_id=@id", new { id = id });
            
            return result;
        }
        public int ReamrkOrderStatusLog(string reamrk)
        {
            var result = _dbcontext.Execute("UPDATE health_order.hm_orderstatuslog2 SET hm_remark=@reamrk WHERE hm_reflog_id=5", new { reamrk = reamrk });
            if (result<1)
            {
                throw new Exception("修改失败！");
            }
            return result;
        }
        //public int ReamrkOrderStatusLog(string reamrk, IDbTransaction transaction)
        //{
        //    return _dbcontext.Execute("UPDATE health_order.hm_orderstatuslog2 SET hm_remark=@reamrk WHERE hm_reflog_id=5", new { reamrk = reamrk }, transaction);
        //}
    }
}
