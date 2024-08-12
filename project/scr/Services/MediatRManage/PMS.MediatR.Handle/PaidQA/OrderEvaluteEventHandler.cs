using MediatR;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.RabbitMQ.Message;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{
    public class OrderEvaluteEventHandler : INotificationHandler<OrderEvaluteEvent>
    {

        IFinanceCenterServiceClient _financeCenterServiceClient;
        ILogger<OrderEvaluteEventHandler> _logger;
        IOrderService _orderService;
        IEventBus _eventBus;
        IPayWalletLogService _payWalletLogService;
        IMediator _mediator;
        public OrderEvaluteEventHandler(IFinanceCenterServiceClient financeCenterServiceClient, ILogger<OrderEvaluteEventHandler> logger, IOrderService orderService, IEventBus eventBus, IPayWalletLogService payWalletLogService, IMediator mediator)
        {
            _financeCenterServiceClient = financeCenterServiceClient;
            _logger = logger;
            _orderService = orderService;
            _eventBus = eventBus;
            _payWalletLogService = payWalletLogService;
            _mediator = mediator;
        }

        public async Task Handle(OrderEvaluteEvent notification, CancellationToken cancellationToken)
        {

            //await SetOrderCache(notification.Order);
            await _mediator.Publish(new OrderChangeEvent(notification.Order.ID));
            await ComputeWallet(notification.Order);
        }


        async Task SetOrderCache(Order order)
        {
            try
            {
                await _mediator.Publish(new OrderChangeEvent(order.ID));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "处理用户创建评价事件异常。");
            }
        }

        async Task ComputeWallet(Order order)
        {
            try
            {
                if (order.Amount <= 0) {
                    //免费订单不需要计算收益。
                    return;    
                }
                //给达人算收益
                var addWalletResult = await _financeCenterServiceClient.Wallet(new WalletRequest()
                {
                    userId = order.AnswerID,
                    amount = order.Amount,
                    statementType = (int)StatementTypeEnum.Incomings,
                    io = 2,
                    orderId = order.ID,
                    orderType = (int)OrderTypeEnum.Ask,
                    remark = "上学问收入"
                });
                if (!addWalletResult.succeed)
                {
                    
                    //尝试推队列
                    _eventBus.Publish(new SyncWalletMessage()
                    {
                        userId = order.AnswerID,
                        amount = order.Amount,
                        statementType = (int)StatementTypeEnum.Incomings,
                        io = 2,
                        orderId = order.ID,
                        orderType = (int)OrderTypeEnum.Ask,
                        remark = "上学问收入"
                    });
                    //插失败记录
                    PayWalletLog payWalletLog = new PayWalletLog()
                    {
                        Id = Guid.NewGuid(),
                        UserId = order.AnswerID,
                        Amount = order.Amount,
                        StatementType = (byte)StatementTypeEnum.Incomings,
                        IO = 2,
                        OrderId = order.ID,
                        OrderType = (byte)OrderTypeEnum.Ask,
                        Remark = "上学问收入",
                        CreatetTime = DateTime.Now,
                        FailedMsg = $"statu:{addWalletResult.status} msg:{addWalletResult.msg}"


                    };
                    bool logres = await _payWalletLogService.AddAsync(payWalletLog);
                    if (!logres) { throw new Exception("记录结算日志失败"); }


                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "上学问收益结算处理。");
            }
        }
    }


}
