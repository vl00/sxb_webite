using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 订单发生变更或者创建会发生这个事件
    /// </summary>
    public class OrderChangeEvent : INotification
    {
        public Guid  OrderID { get; set; }
        public OrderChangeEvent(Guid  orderID)
        {
            OrderID = orderID;
        }
    }
}
