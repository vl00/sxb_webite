using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IMixService
    {
        Task BuildDaysStatisticsAsync(string matchUrl = "https://m3.sxkid.com/school_detail_wechat/data", DateTime? dayTime = null);
    }
}
