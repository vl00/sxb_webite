using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Framework.MSSQLAccessor.WebExample.ViewModel;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Service;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Data;
using ProductManagement.Framework.MSSQLAccessor.WebExample.Core;
using System.Transactions;
using HealthMall.Framework.PGAccessor;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Framework.MSSQLAccessor.WebExample.Controllers
{
    [Produces("application/json")]
    [Route("api/Orderstatuslog")]
    public class OrderstatuslogController : Controller
    {
        private readonly DBContext _dbcontext;

        private readonly GroupOrderProgressService _groupOrderProgressService;
        private readonly OrderStatusLogService _orderStatusLogService;

        //private readonly ILogger<OrderstatuslogController> _logger;

        //public OrderstatuslogController(DBContext dbcontext, GroupOrderProgressService groupOrderProgressService, OrderStatusLogService orderStatusLogService, ILogger<OrderstatuslogController> logger)
        //{
        //    //db = new DataRetriever(ConnectionManager.GetConntionString("U_ConnectionString"));
        //    _groupOrderProgressService = groupOrderProgressService;
        //    _orderStatusLogService = orderStatusLogService;
        //    _dbcontext = dbcontext;
        //    _logger = logger;
        //}
        public OrderstatuslogController(DBContext dbcontext, GroupOrderProgressService groupOrderProgressService, OrderStatusLogService orderStatusLogService)
        {
            //db = new DataRetriever(ConnectionManager.GetConntionString("U_ConnectionString"));
            _groupOrderProgressService = groupOrderProgressService;
            _orderStatusLogService = orderStatusLogService;
            _dbcontext = dbcontext;
        }
        // GET: api/Orderstatuslog
        [HttpGet]
        public IEnumerable<OrderStatusLog> Get()
        {
            //var db = new DataRetriever(ConnectionManager.GetConntionString("U_ConnectionString"));
            //_logger.LogError("≤‚ ‘£°");
            var list = new int[] { 1, 2, 3 };
            var intlist = new List<int> { 1, 2, 3 };
            var result = _dbcontext.Query<OrderStatusLog>("SELECT * FROM health_order.hm_orderstatuslog2 WHERE hm_reflog_id=ANY(@ids)", new { ids = intlist });
            //var result =await _dbcontext.QuerySingleAsync<OrderStatusLog>("SELECT hm_orderid FROM health_order.hm_orderstatuslog2 WHERE hm_reflog_id=ANY(@ids)", new { ids = intlist });
            //var result2 = _dbcontext.Query<OrderStatusLog>("SELECT * FROM health_order.hm_orderstatuslog2 WHERE hm_reflog_id=ANY(@ids)", new { ids = new[] { "1", "2", "3" } });

            //var orderidList = new string[] { "go170515175858243986", "go170516170213450413", "go170516170213450413" };
            var orderidList2 = new List<string> { "go170515175858243986", "go170516170213450413", "go170516170213450413" };
            var result3 = _dbcontext.Query<OrderStatusLog>("SELECT * FROM health_order.hm_orderstatuslog2 WHERE hm_orderid=ANY(@ids)", new { ids = orderidList2 });

            //var result4 = _dbcontext.Query<OrderStatusLog>("SELECT * FROM health_order.hm_orderstatuslog2 WHERE hm_reflog_id=1", null);

            return result;

            //using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            //    var result = await _dbcontext.QueryAsync<OrderStatusLog>("SELECT * FROM health_order.hm_orderstatuslog2 WHERE hm_reflog_id=ANY(@ids)", new { ids = intlist });
            //    var result2 = await _dbcontext.ExecuteAsync("UPDATE health_order.hm_grouporderprogress SET hm_go_paystatus=hm_go_paystatus+1 WHERE grouporder_id=@orderId AND hm_go_paystatus<3", new { orderId = "go170516170213450413" });

            //    if (result2 < 1)
            //    {
            //        throw new Exception("–ﬁ∏ƒ ß∞‹£°");
            //    }

            //    transaction.Complete();

            //    return result;
            //}

        }

        // GET: api/Orderstatuslog/5
        [HttpGet("{id}", Name = "Get")]
        public OrderStatusLogDomain Get(int id)
        {
            //var _orderStatusLogService2 = new OrderStatusLogService(new OrderStatusLogRepository(_dbcontext));

            var orderStatusLog = _orderStatusLogService.GetOrderStatusLogById(id);
            var grouporderprogress = _groupOrderProgressService.GetGroupOrderProgressByOrderId(orderStatusLog.hm_orderid);

            //if (orderStatusLog == null)
            //{
            //    return new OrderStatusLogDomain();
            //}
            return orderStatusLog;
            //try
            //{
            //    using (var transaction = new TransactionScope())
            //    {
            //        var orderStatusLog = _orderStatusLogService.GetOrderStatusLogById(id);
            //        var grouporderprogress = _groupOrderProgressService.GetGroupOrderProgressByOrderId(orderStatusLog.hm_orderid);
            //        var updatePayStatusResult = _groupOrderProgressService.UpdatePayStatus(orderStatusLog.hm_orderid);

            //        if (orderStatusLog == null)
            //        {
            //            return new OrderStatusLogDomain();
            //        }
            //        transaction.Complete();

            //        return orderStatusLog;
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}


        }


        //[HttpPost("{id}")]
        //public int Post(string id)
        //{
        //    //var db = new DBContext(ConnectionManager.GetConntionString("U_ConnectionString"));
        //    var transaction = _dbcontext.BeginTransaction();
        //    //var groupOrderProgressService = new GroupOrderProgressService(new GroupOrderProgressRepository(_dbcontext));
        //    //var orderStatusLogService = new OrderStatusLogService(new OrderStatusLogRepository(_dbcontext));

        //    try
        //    {
        //        //var grouporderprogress = groupOrderProgressService.GetGroupOrderProgressByOrderId(id, transaction);
        //        //if (grouporderprogress.hm_go_paystatus==2)
        //        //{
        //        //    var result = orderStatusLogService.ReamrkOrderStatusLog(id, transaction);
        //        //    db.Commit();
        //        //}
        //        var reamrkOrderStatusLogResult = _orderStatusLogService.ReamrkOrderStatusLog(id, transaction);
        //        var updatePayStatusResult = _groupOrderProgressService.UpdatePayStatus(id, transaction);
        //        if (updatePayStatusResult > 0)
        //        {
        //            //_dbcontext.Commit();
        //            _dbcontext.Rollback();
        //        }
        //        else
        //        {
        //            _dbcontext.Commit();
        //            //_dbcontext.Rollback();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _dbcontext.Rollback(); ;
        //        throw ex;
        //    }

        //    return 1;
        //}

        // PUT: api/Orderstatuslog/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
