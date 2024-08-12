using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Data
{
    public class GroupOrderProgressRepository
    {
        private readonly DBContext _dbcontext;
        public GroupOrderProgressRepository(DBContext dbcontext)
        {
            this._dbcontext = dbcontext;
        }
        public GroupOrderProgressDomain GetGroupOrderProgressByOrderId(string orderid)
        {
            return _dbcontext.QuerySingle<GroupOrderProgressDomain>("SELECT grouporder_id,user_id,hm_go_paystatus FROM health_order.hm_grouporderprogress WHERE grouporder_id=@orderId", new { orderId = orderid });
        }
        //public GroupOrderProgressDomain GetGroupOrderProgressByOrderId(string orderid, IDbTransaction transaction)
        //{
        //    return _dbcontext.QuerySingle<GroupOrderProgressDomain>("SELECT grouporder_id,user_id,hm_go_paystatus FROM health_order.hm_grouporderprogress WHERE grouporder_id=@orderId", new { orderId = orderid },transaction);
        //}

        public int UpdatePayStatus(string orderid)
        {
            var reslut = _dbcontext.Execute("UPDATE health_order.hm_grouporderprogress SET hm_go_paystatus=hm_go_paystatus+1 WHERE grouporder_id=@orderId AND hm_go_paystatus<3", new { orderId = orderid });

            if (reslut<1)
            {
                throw new Exception("修改失败！");
            }

            return reslut;
        }
        //public int UpdatePayStatus(string orderid, IDbTransaction transaction)
        //{
        //    return _dbcontext.Execute("UPDATE health_order.hm_grouporderprogress SET hm_go_paystatus=hm_go_paystatus+1 WHERE grouporder_id=@orderId AND hm_go_paystatus<3", new { orderId = orderid }, transaction);
        //}
    }
}
