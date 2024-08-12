using AutoMapper;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Report
{
    public class ReportTypeService : IReportTypeService
    {
        private IReportTypeRepositories _reportTypeRepositories;
        private IMapper _mapper;
        public ReportTypeService(IMapper mapper, IReportTypeRepositories reportTypeRepositories)
        {
            _reportTypeRepositories = reportTypeRepositories;
            _mapper = mapper;
        }

        public List<ReportTypeVo> GetReportTypes()
        {
            var report = _reportTypeRepositories.GetReportTypes();
            return _mapper.Map<List<ReportType>, List<ReportTypeVo>>(report);
        }

    }
}
