using PMS.SignalR.Clients.PaidQAClient.Models;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.SignalR.Clients.PaidQAClient
{
    public interface IOrderChatClient
    {

        Task ReceiveMessage(ReceiveMessageResult result);

        Task ReceiveOrderChange(ReceiveOrderChangeResult result);
    }
}
