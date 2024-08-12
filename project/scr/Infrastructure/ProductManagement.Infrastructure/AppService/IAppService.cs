using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.AppService
{
    /// <summary>
    /// 应用服务接口
    /// </summary>
    /// <typeparam name="TcDataObject">命令处理</typeparam>
    /// <typeparam name="TqDataObject">查询处理</typeparam>
    public interface IAppService<DataObject> : IcAppService<DataObject>, IqAppService<DataObject>
    {

    }
}
