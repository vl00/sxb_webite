using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Operation
{
    /// <summary>
    /// 数据源最新【点赞总数 | 回复总数 | 当前用户是否点赞】
    /// </summary>
    public class OperationDataService : IOperationDataService
    {
        public IOperationDataRepositories _operationData;
        public OperationDataService(IOperationDataRepositories operationData) 
        {
            _operationData = operationData;
        }

        public List<OperationData> OperationLastedData(OperationType OperationType, Guid UserId, List<Guid> DataSource)
        {
            return _operationData.OperationLastedData(OperationType, UserId, DataSource);
        }
    }
}
