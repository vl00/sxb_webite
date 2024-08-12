using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface ISystemEventLogRepository:IRepository<SystemEventLog>
    {

        Task<IEnumerable<SystemEventLogExportDataDto>> ExportData(string plateform, string where = null,object param = null,string orderBy = null);

        /// <summary>
        /// 获取每日Pv Uv
        /// </summary>
        /// <param name="startWithUrl"></param>
        /// <param name="startTime"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        Task<IEnumerable<SystemEventLogPvUvDto>> GetDaysPvUv(string startWithUrl, DateTime? startTime, DateTime? endTime);
        Task<IEnumerable<string>> GetHitUrls(Guid userId, string[] urls, DateTime startTime);
    }
}
