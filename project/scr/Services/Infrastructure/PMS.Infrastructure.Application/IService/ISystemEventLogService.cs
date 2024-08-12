using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface ISystemEventLogService
    {
        Task<bool> AddLog(SystemEventLog systemEventLog);
        Task<IEnumerable<SystemEventLogExportDataDto>> ExportPaidQAData(DateTime btime, DateTime etime);
        Task<IEnumerable<SystemEventLogPvUvDto>> GetDaysPvUv(string startWithUrl, DateTime? startTime, DateTime? endTime);
    }
}
