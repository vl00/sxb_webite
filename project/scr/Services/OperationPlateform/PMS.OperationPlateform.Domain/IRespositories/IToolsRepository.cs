﻿using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IToolsRepository : IBaseRepository<Tools>
    {
        IEnumerable<dynamic> GetGroupByCityId();
    }
}
