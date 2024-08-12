using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IDataExportRepository
    {


        Task<IEnumerable<dynamic>> ExportPageRecords(DateTime btime, DateTime etime);
        Task<IEnumerable<dynamic>> ExportFWRecords(DateTime btime, DateTime etime);
    }
}
