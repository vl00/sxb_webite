using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class ToolsService: IToolsService
    {
        IToolsRepository currentRepository;

        public ToolsService(IToolsRepository currentRepository)
        {
            this.currentRepository = currentRepository;
        }

        public IEnumerable<Tools> GetByToolTypeId(int toolTypeId, int cityId,int provinceId)
        {
            var tools = this.currentRepository.Select(" ToolTypeId=@ToolTypeId AND ((cityId is null And ProvinceId is null) OR (ProvinceId = @provinceId And cityId is null) OR (ProvinceId = @provinceId And cityId = @cityId)) AND isShow=1", new { ToolTypeId = toolTypeId, cityId = cityId,provinceId });
            return tools;
        }

    }
}
