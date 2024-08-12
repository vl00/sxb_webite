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
    /// 处理专家发送消息通知
    /// </summary>
    public class TalentSendMessageHandler : IRequestHandler<TalentSendMessageRequest, bool>
    {
        IOrderService _orderService;
        public TalentSendMessageHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<bool> Handle(TalentSendMessageRequest request, CancellationToken cancellationToken)
        {

            return false;

        }
    }
}
