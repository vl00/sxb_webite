using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{
    using Microsoft.Extensions.Logging;
    using ProductManagement.Framework.MSSQLAccessor;

    /// <summary>
    /// 访问运营管理平台库的DBContext
    /// </summary>
    public class OperationDBContext : DBContext<OperationDBContext>
    {
        public OperationDBContext(ConnectionsManager<OperationDBContext> connectionsManager, ILogger<OperationDBContext> log) : base(connectionsManager, log)
        {
        }



       
    }
}