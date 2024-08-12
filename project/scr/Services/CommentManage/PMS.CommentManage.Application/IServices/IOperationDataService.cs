using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IOperationDataService
    {
        List<OperationData> OperationLastedData(OperationType OperationType, Guid UserId, List<Guid> DataSource);
    }
}
