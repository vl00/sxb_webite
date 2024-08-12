using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class ToolTypesServices : IToolTypesServices
    {

        IToolTypesRepository  currentRepository;


        public ToolTypesServices(IToolTypesRepository currentRepository)
        {
            this.currentRepository = currentRepository;
        }

        public IEnumerable<ToolModuleInfoDto> GetOnlineToolTypes(int cityId,int provinceId)
        {
          var toolTypes =   this.currentRepository.Select(@"
                    EXISTS(
                    SELECT 1 FROM Tools WHERE ToolTypes.Id = Tools.ToolTypeId AND IsShow = 1 AND ((cityId is null And ProvinceId is null) OR (ProvinceId = @provinceId And cityId is null) OR (ProvinceId = @provinceId And cityId = @cityId))
                    )
                    OR ParentId = 0
                ", new { cityId = cityId,provinceId }, "parentId,Sort");


            List<ToolModuleInfoDto> datas = new List<ToolModuleInfoDto>();
            var firstModules = toolTypes.Where(t => t.ParentId == 0).OrderBy(t => t.Sort);
            foreach (var fm in firstModules)
            {
                ToolModuleInfoDto toolModuleInfoDto = new ToolModuleInfoDto();
                toolModuleInfoDto.FirstModule = fm;
                toolModuleInfoDto.SecondModule = toolTypes.Where(t => t.ParentId == fm.Id).OrderBy(t=>t.Sort).ToList();
                datas.Add(toolModuleInfoDto);
            }

            return datas;
        }
    }
}
