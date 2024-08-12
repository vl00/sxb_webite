using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ProductManagement.Framework.MSSQLAccessor.Plus;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManagement.Framework.MSSQLAccessor;

namespace HealthMall.Framework.PGAccessor.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("pgConnectionSettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            var PGconfig = configuration.GetSection("DbConnectionStrings").Get<List<ConnectionConfig>>().FirstOrDefault();

            var conn = new ConnectionsManager(PGconfig).GetDbConnection();

            var serviceCollection = new ServiceCollection()
                .AddLogging(bu =>
                {
                    bu
                        .AddConfiguration(configuration.GetSection("Logging"))
                        .AddConsole();
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            ILoggerFactory loggerFactory= serviceProvider.GetRequiredService<ILoggerFactory>();

            string sql = "INSERT INTO public.hm_orderstatuslog (" +
                            "hm_orderid," +
                            "hm_ordertype," +
                            "hm_orderstatus," +
                            "hm_createtime," +
                            "hm_remark) " +
                            "VALUES(" +
                            "@hm_orderid,@hm_ordertype,@hm_orderstatus,statement_timestamp(),@hm_remark" +
                            ");";

            //var sql = "UPDATE public.hm_orderstatuslog SET hm_orderstatus=2 WHERE hm_reflog_id=@hm_reflog_id;";

            //var sql = "DELETE FROM public.hm_orderstatuslog WHERE hm_reflog_id=@hm_reflog_id;";

            Console.WriteLine("batchSize:");
            var batchSize = int.Parse(Console.ReadLine());
            Console.WriteLine("threadPoolSize:");
            var threadPoolSize = int.Parse(Console.ReadLine());
            Console.WriteLine("records:");
            var records = int.Parse(Console.ReadLine());

            var list = new List<object>();
            for (int i = 1; i <= records; i++)
            {
                list.Add(new Orderstatuslog { hm_orderid = "test" + i, hm_remark = "test" + i });
            }
            //var listint = conn.Query<int>("select hm_reflog_id from public.hm_orderstatuslog;").AsList();
            //for (int i = 0; i < listint.Count; i++)
            //{
            //    list.Add(new { hm_reflog_id = listint[i] });
            //}

            var result = new BatchOperation(PGconfig, loggerFactory).BatchExecute(sql, list, batchSize, threadPoolSize);

            Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
    public class Orderstatuslog
    {
        public string hm_orderid { get; set; }
        public int hm_ordertype { get; set; } = 1;
        public int hm_orderstatus { get; set; } = 1;
        public DateTime hm_createtime { get; set; } = DateTime.Now;
        public string hm_remark { get; set; }
    }
}
