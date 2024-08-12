using PMS.CommentsManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IReportTypeService
    {
        List<ReportTypeVo> GetReportTypes();
    }
}
