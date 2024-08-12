using ProductManagement.Framework.MSSQLAccessor;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;


namespace ProductManagement.Framework.MSSQLAccessor.Test
{
    public class ConnectionManagerTest
    {
        public IConfigurationRoot Configuration { get; set; }
        [Fact]
        public void CanGetConntionString()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("pgConnectionSettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
            var PGconfig = Configuration.GetSection("DbConnectionStrings").Get<List<ConnectionConfig>>().FirstOrDefault();

            var conn =new ConnectionsManager(PGconfig).GetDbConnection();
            Assert.True(conn!=null);
        }
    }
}
