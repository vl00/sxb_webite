using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Threading;

namespace ProductManagement.Framework.MSSQLAccessor
{
    public class ConnectionsManager<T>
    {
        //private static readonly NpgsqlLogger Log = NpgsqlLogManager.Provider.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        private readonly ILogger<ConnectionsManager<T>> _log;
        private static readonly ThreadLocal<DataSourceEnum> threadLocal = new ThreadLocal<DataSourceEnum>();
        /// <summary>
        /// 主数据库连接串
        /// </summary>
        private readonly string _masterConnectionString;
        /// <summary>
        /// 从数据库连接串集合
        /// </summary>
        private readonly List<string> _slaverConnectionStrings = new List<string>();


        private IDbConnection _dbConnection;

        public ConnectionsManager(AccessorConfigOptions<T> configOptions, ILogger<ConnectionsManager<T>> log) : this(configOptions.ConnectionConfig, log)
        {

        }
        public ConnectionsManager(ConnectionConfig<T> connectionConfig)
        {
            _masterConnectionString = connectionConfig.LoadBalancer;
            _slaverConnectionStrings = connectionConfig.RealDbConnectionStrings;
        }

        public ConnectionsManager(ConnectionConfig<T> connectionConfig, ILogger<ConnectionsManager<T>> log)
        {
            _masterConnectionString = connectionConfig.LoadBalancer;
            _slaverConnectionStrings = connectionConfig.RealDbConnectionStrings;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>
        /// 当前线程数据源 
        /// </summary>
        /// <param name="sourceEnum"></param>     
        public DataSourceEnum DataSource
        {
            set { threadLocal.Value = value; }
            get { return threadLocal.Value; }
        }

        private IDbConnection GetConnection(string connectionString)
        {

            var connectionStringWithEnlist = connectionString;
            var conn = new SqlConnection(connectionStringWithEnlist);

            conn.Open();

            _dbConnection = conn;

            conn.Close();
            return conn;
        }



        public IDbConnection GetDbConnection()
        {
            if (DataSource == DataSourceEnum.MASTER)
            {
                return GetMasterConnection();
            }
            else
            {
                return GetSlaverConnection();
            }
        }

        private IDbConnection GetMasterConnection()
        {
            return GetConnection(_masterConnectionString);
        }
        private IDbConnection GetSlaverConnection()
        {
            int sc = _slaverConnectionStrings.Count;
            if (sc > 0)
            {
                Random random = new Random();
                int index = random.Next(0, sc);
                return GetConnection(_slaverConnectionStrings[index]);
            }
            else
            {
                _log.LogInformation("没有设置从库，将从建立主库连接");
                return GetMasterConnection();
            }
        }
    }
}
