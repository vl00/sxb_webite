using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using PMS.CommentsManage.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Jobs
{
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(90)]
    public class SettlementAmountMoneyJob : IRecurringJob
    {
        private ISettlementAmountMoneyService _moneyService;

        public SettlementAmountMoneyJob(ISettlementAmountMoneyService moneyService) 
        {
            _moneyService = moneyService;
        }

        public void Execute(PerformContext context)
        {
            //_moneyService.Settlement();
        }
    }
}
