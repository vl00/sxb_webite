using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ProductManagement.Framework.MSSQLAccessor.DBContext
{

    public class ISchoolDataDBContext : DBContext<ISchoolDataDBContext>
    {

        public ISchoolDataDBContext(ConnectionsManager<ISchoolDataDBContext> connectionsManager, ILogger<ISchoolDataDBContext> log) : base(connectionsManager, log)
        {

        }



    }
}
