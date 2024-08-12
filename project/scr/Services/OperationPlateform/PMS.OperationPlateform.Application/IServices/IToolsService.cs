using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IToolsService
    {
        IEnumerable<Tools> GetByToolTypeId(int toolTypeId, int cityId, int provinceId);

    }
}
