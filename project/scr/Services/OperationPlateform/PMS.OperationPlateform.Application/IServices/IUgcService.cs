using PMS.OperationPlateform.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IUgcService
    {
        AppServiceResultDto Feedback(Guid id);
    }
}
