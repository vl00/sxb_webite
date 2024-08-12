using MediatR;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.MediatR.Events.PaidQA
{

    /// <summary>
    /// 用户提问机会用完事件
    /// </summary>
    public class UserAskRemainNoneEvent : INotification
    {
        public Order Order { get; }
        public UserAskRemainNoneEvent(Order order)
        {
            Order = order;
        }
    }
}
