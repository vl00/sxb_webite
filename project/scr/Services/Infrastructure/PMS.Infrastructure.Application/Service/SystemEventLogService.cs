using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.Service
{
   public class SystemEventLogService: ISystemEventLogService
    {
        ISystemEventLogRepository  _repository;
        public SystemEventLogService(ISystemEventLogRepository systemEventLogRepository)
        {
            _repository = systemEventLogRepository;
        }


        public async Task<bool> AddLog(SystemEventLog systemEventLog)
        {
           return await _repository.AddAsync(systemEventLog);
        }

        public async Task<IEnumerable<SystemEventLogExportDataDto>> ExportPaidQAData(DateTime btime,DateTime etime)
        {
            string where = @"
(
EventId = 'pageView' and jsn.pagePath like @pagePath
Or
EventId IN 
    (
    'recommendconsultpage_hotconsultlist_click'
    ,'recommendconsultpage_startconsult_click'
    ,'hotconsultpage_phasenavigation_click'
    ,'hotconsultpage_quickconsult_click'
    ,'hotconsultpage_viewdetails_click'
    ,'consultdetailpage_consult_click'
    ,'consultdetailpage_interest_click'
    ,'consultdetailpage_recommendexpertconsult_click'
    ,'talentlistpage_consult_click'
    ,'expertdetailpage_interest_click'
    ,'consultdetailpage_consult_click'
    ,'chatroompage_transferconsult_click'
    ,'chatroompage_recommendconsult_click'
    ,'chatroompage_recommendcontrol_click'
    ,'consultpaypage_pay_click'
    )
)
AND (CreateTime BETWEEN @btime and @etime)";
           return await  _repository.ExportData("ask",where, new { btime, etime ,jsonpath= "$.pagePath" }, "CreateTime desc");
        }

        public async Task<IEnumerable<SystemEventLogPvUvDto>> GetDaysPvUv(string startWithUrl, DateTime? startTime, DateTime? endTime)
        {
            return await _repository.GetDaysPvUv(startWithUrl, startTime, endTime);
        }
    }
}
