using System;
namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IPayOrderRepository
    {
        int GetPayedOrderCount();
    }
}
