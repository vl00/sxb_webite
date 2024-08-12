using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain
{
    public interface IUgcLogRepository : IBaseRepository<UgcLog>
    {
        bool Update(UgcLog ugcLog);
        bool UpdateUserAreaByExtId(Guid extId, Guid userId, AreaType areaType);
    }
}
