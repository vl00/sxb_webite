using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Application.Services
{
    public class PayWalletLogService : ApplicationService<PayWalletLog>, IPayWalletLogService
    {
        IPayWalletLogRepository _repository;

        public PayWalletLogService(IPayWalletLogRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
