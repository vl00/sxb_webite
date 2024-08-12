using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.SignalR
{
    public class OrderChatHubHelper
    {

        //public static string GroupName(Guid orderId)
        //{
        //    return $"orderGroup_{orderId}";
        //}
        public static string OrderUserGroupName(Guid orderId,Guid userId)
        {
            return $"orderGroup_{orderId}_{userId}";
        }
    }
}
