using System;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.IRepositories;

namespace PMS.Infrastructure.Application.Service
{
    public class PayOrderService: IPayOrderService
    {
        private readonly IPayOrderRepository _payOrderRepository;
        public PayOrderService(IPayOrderRepository payOrderRepository)
        {
            _payOrderRepository = payOrderRepository;
        }

        public int GetPayedOrderCount()
        {
            return _payOrderRepository.GetPayedOrderCount();
        }
    }
}
