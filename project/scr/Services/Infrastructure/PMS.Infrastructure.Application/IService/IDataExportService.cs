using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface IDataExportService
    {

        /// <summary>
        /// 导出付费问答页面记录表
        /// </summary>
        /// <param name="btime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> ExportPaidPageRecords(DateTime btime, DateTime etime);

        /// <summary>
        /// 导出付费问答渠道记录表
        /// </summary>
        /// <param name="btime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> ExportPaidFWRecords(DateTime btime, DateTime etime);
    }
}
