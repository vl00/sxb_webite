using MediatR;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{

    /// <summary>
    /// 处理用户发消息
    /// </summary>
    public class UserSendMessageHandler : IRequestHandler<UserSendMessageRequest, bool>
    {
        IOrderService _orderService;
        public UserSendMessageHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<bool> Handle(UserSendMessageRequest request, CancellationToken cancellationToken)
        {
            return false;
        }

    }
}
