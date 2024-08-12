using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 订单支付成功事件
    /// </summary>
    public class PaySuccessEvent: INotification
    {

        public Guid OrderID { get;  }
        public PaySuccessEvent(Guid  orderId)
        {
            OrderID = orderId;
        }
    }
}
