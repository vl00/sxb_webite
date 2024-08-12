using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.Search.Domain.Entities;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IHotspotService
    {
        Task<IEnumerable<HotspotDto>> GetHotSchools(int cityId, ClientType clientType = ClientType.PC);
        Task<IEnumerable<HotspotDto>> GetHotWords(int cityId, ClientType clientType = ClientType.PC);
        Task<IEnumerable<HotspotDto>> GetPlaceholders(int cityId, ClientType clientType = ClientType.PC);
    }
}
