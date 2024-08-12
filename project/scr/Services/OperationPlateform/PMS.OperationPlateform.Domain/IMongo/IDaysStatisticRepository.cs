using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.OperationPlateform.Domain.MongoModel;
using PMS.OperationPlateform.Domain.MongoModel.Base;

namespace PMS.OperationPlateform.Domain.IMongo
{
    public interface IDaysStatisticRepository
    {
        Task AddAsync(IEnumerable<DaysStatistics> daysStatistics);
    }
}
