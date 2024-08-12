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
   public  class ISchoolDBContext:DBContext<ISchoolDBContext>
    {


        public ISchoolDBContext(ConnectionsManager<ISchoolDBContext> connectionsManager, ILogger<ISchoolDBContext> log) : base(connectionsManager, log)
        {

        }





    

    }
}
