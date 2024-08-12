using ProductManagement.Framework.MSSQLAccessor.WebExample.Core;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Service
{
    public class OrderStatusLogService
    {
        private readonly OrderStatusLogRepository _orderStatusLogRepository;
        public OrderStatusLogService(OrderStatusLogRepository orderStatusLogRepository)
        {
            this._orderStatusLogRepository = orderStatusLogRepository;
        }

        public OrderStatusLogDomain GetOrderStatusLogById(int id)
        {
            return _orderStatusLogRepository.GetOrderStatusLogById(id);
        }
        //public int ReamrkOrderStatusLog(string reamrk, IDbTransaction transaction)
        //{
        //    return _orderStatusLogRepository.ReamrkOrderStatusLog(reamrk,transaction);
        //}
    }
}
