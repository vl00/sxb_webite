using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
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
    /// 评价超时，系统自动好评
    /// </summary>
    public class AutoEvaluationTalentHandler : INotificationHandler<AutoEvaluationTalentEvent>
    {
        IServiceProvider _serviceProvider;
        ILogger _logger;


        public AutoEvaluationTalentHandler(
            ILogger<AutoEvaluationTalentHandler> logger,
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Handle(AutoEvaluationTalentEvent request, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                 IEvaluateService _evaluateService= scope.ServiceProvider.GetRequiredService<IEvaluateService>();
                try
                {
                    await _evaluateService.AutoNiceEvaluation();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "定时处理系统自动好评报错。");
                }
            }
        }
    }
}
