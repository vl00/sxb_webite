using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IToolTypesServices
    {

        IEnumerable<ToolModuleInfoDto> GetOnlineToolTypes(int cityId, int provinceId);
    }
}
