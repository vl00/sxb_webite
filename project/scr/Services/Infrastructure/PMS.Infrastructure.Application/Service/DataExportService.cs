using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.Service
{
   public  class DataExportService: IDataExportService
    {
        IDataExportRepository _repository;
        public DataExportService(IDataExportRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<dynamic>> ExportPaidPageRecords(DateTime btime, DateTime etime)
        {
            return await _repository.ExportPageRecords(btime, etime);
        }

        public async Task<IEnumerable<dynamic>> ExportPaidFWRecords(DateTime btime, DateTime etime)
        {
            return await _repository.ExportFWRecords(btime, etime);
        }
    }
}
