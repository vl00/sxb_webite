using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor
{
    public class DBContext<T> : IUnitOfWork, IDisposable, IDBContext
    {
        //private static readonly NpgsqlLogger Log = NpgsqlLogManager.Provider.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        protected readonly ILogger<DBContext<T>> _log;

        private readonly ConnectionsManager<T> _connectionsManager;

        private IDbConnection dbConnection;
        protected IDbConnection _dbConnection => dbConnection ?? GetDbConnection();

        private IDbTransaction _dbtransaction;

        public DBContext(ConnectionsManager<T> connectionsManager, ILogger<DBContext<T>> log)
        {
            //_dbConnection = connectionsManager.GetDbConnection();
            _connectionsManager = connectionsManager;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        //默认库
        private IDbConnection GetDbConnection()
        {
            dbConnection = GetDbConnection(_connectionsManager.DataSource);
            return dbConnection;
        }

        //根据数据类型选择读写库
        private IDbConnection GetDbConnection(DataSourceEnum dataSource)
        {
            _connectionsManager.DataSource = dataSource;
            return _connectionsManager.GetDbConnection();
        }

        public IEnumerable<T> Query<T>(string sql, object param, IDbTransaction transaction, CommandType? commandType = null)
        {
            return GetDbConnection(DataSourceEnum.SLAVE).Query<T>(sql, param, transaction, true, null, commandType);
        }

        public IEnumerable<dynamic> Query(string sql, object param, IDbTransaction transaction = null)
        {
            return this._dbConnection.Query(sql, param, transaction);
        }

        public SqlMapper.GridReader QueryMultiple(string sql, object param, IDbTransaction tran = null, CommandType? commandType = null)
        {
            return this._dbConnection.QueryMultiple(sql, param, tran, commandType: commandType);
        }
        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param, IDbTransaction tran = null, CommandType? commandType = null)
        {
            return await this._dbConnection.QueryMultipleAsync(sql, param, tran, commandType: commandType);
        }

        public IEnumerable<TReturn> Query<TFrist, TSecond, TReturn>(string sql, Func<TFrist, TSecond, TReturn> map, object _params = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return this._dbConnection.Query(sql, map, _params, transaction, buffered, splitOn, commandTimeout, commandType);
        }



        public int Execute(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return conn.Execute(sql, param);
                }
                //return _dbConnection.Execute(sql, param);
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return conn.Execute(sql, param);
                    }
                }
                else
                {
                    throw ex;
                }

            }


        }


        public T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            return this._dbConnection.ExecuteScalar<T>(sql, param, transaction);
        }
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            return await this._dbConnection.ExecuteScalarAsync<T>(sql, param, transaction);
        }

        public async Task<int> ExecuteAsync(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return await conn.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return await conn.ExecuteAsync(sql, param);
                    }
                }
                else
                {
                    throw ex;
                }

            }


        }



        public int Execute(string sql, object param, IDbTransaction transaction)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                return _dbConnection.Execute(sql, param, transaction);
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    return _dbConnection.Execute(sql, param, transaction);
                }
                else
                {
                    throw ex;
                }

            }

        }

        public int ExecuteUow(string sql, object param)
        {
            return Execute(sql, param, _dbtransaction);
        }

        public Task<int> ExecuteAsync(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                return _dbConnection.ExecuteAsync(sql, param, transaction, commandTimeout);
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    return _dbConnection.ExecuteAsync(sql, param, transaction, commandTimeout);
                }
                else
                {
                    throw ex;
                }

            }

        }

        public IEnumerable<T> Query<T>(string sql, object param, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return conn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return conn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }

        public IEnumerable<T> QueryUow<T>(string sql, object param)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param), "参数不能为空！");

            try
            {
                return _dbConnection.Query<T>(sql, param, _dbtransaction);
            }
            catch (Exception ex)
            {
                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    return _dbConnection.Query<T>(sql, param, _dbtransaction);
                }
                throw ex;
            }
        }

        //public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, IDbTransaction transaction = null)
        //{
        //    return await this._dbConnection.QueryAsync<T>(sql, param, transaction);
        //}
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return await this._dbConnection.QueryAsync(sql, map, param, transaction,buffered,splitOn,commandTimeout, commandType);
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param)
        {

            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return await conn.QueryAsync<T>(sql, param);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return await conn.QueryAsync<T>(sql, param);
                    }
                }
                else
                {
                    throw ex;
                }

            }

        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await this._dbConnection.QueryAsync<T>(sql, param, transaction,commandTimeout, commandType);
        }
        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return await conn.QueryAsync(sql, param, transaction, commandTimeout, commandType);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return await conn.QueryAsync(sql, param, transaction, commandTimeout, commandType);
                    }
                }
                else
                {
                    throw ex;
                }

            }

        }

        //public IEnumerable<T> Query<T>(string sql, object param, IDbTransaction transaction)
        //{
        //    if (param == null)
        //    {
        //        throw new ArgumentNullException("param", "参数不能为空！");
        //    }

        //    try
        //    {
        //        return _dbConnection.Query<T>(sql, param, transaction);
        //    }
        //    catch (PostgresException ex)
        //    {

        //        if (ex.Message == "53000: Failed to get pooled connections")
        //        {
        //            //log
        //            //Log.Log(NpgsqlLogLevel.Warn, 0, ex.Message, ex);
        //            _log.LogError(ex, ex.Message);
        //            return _dbConnection.Query<T>(sql, param, transaction);
        //        }
        //        else
        //        {
        //            throw ex;
        //        }

        //    }

        //}

        //public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, IDbTransaction transaction)
        //{
        //    if (param == null)
        //    {
        //        throw new ArgumentNullException("param", "参数不能为空！");
        //    }

        //    try
        //    {
        //        return await _dbConnection.QueryAsync<T>(sql, param, transaction);
        //    }
        //    catch (PostgresException ex)
        //    {

        //        if (ex.Message == "53000: Failed to get pooled connections")
        //        {
        //            //log
        //            //Log.Log(NpgsqlLogLevel.Warn, 0, ex.Message, ex);
        //            _log.LogError(ex, ex.Message);
        //            return await _dbConnection.QueryAsync<T>(sql, param, transaction);
        //        }
        //        else
        //        {
        //            throw ex;
        //        }

        //    }

        //}

        public T QuerySingle<T>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return conn.QuerySingleOrDefault<T>(sql, param);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return conn.QuerySingleOrDefault<T>(sql, param);
                    }
                }
                else
                {
                    throw ex;
                }

            }

        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                using (var conn = _connectionsManager.GetDbConnection())
                {
                    return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
                }
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    using (var conn = _connectionsManager.GetDbConnection())
                    {
                        return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
                    }
                }
                else
                {
                    throw ex;
                }

            }

        }



        public async Task<T> QuerySingleAsync<T>(string sql, object param, IDbTransaction transaction)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "参数不能为空！");
            }

            try
            {
                return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
            }
            catch (Exception ex)
            {

                if (ex.Message == "53000: Failed to get pooled connections")
                {
                    _log.LogError(ex, ex.Message);
                    return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
                }
                else
                {
                    throw ex;
                }

            }

        }

        /*封装 Contrib的方法*/

        public T Get<T, TId>(TId id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this._dbConnection.Get<T>(id, transaction, commandTimeout);
        }
        public async Task<T> GetAsync<T, TId>(TId id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await this._dbConnection.GetAsync<T>(id, transaction, commandTimeout);
        }

        public IEnumerable<T> GetAll<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this._dbConnection.GetAll<T>(transaction, commandTimeout);
        }

        public long Insert<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            List<T> inserts = new List<T>() { entityToInsert };
            return this._dbConnection.Insert<IEnumerable<T>>(inserts, transaction, commandTimeout);
        }

        public async Task<int> InsertAsync<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            List<T> inserts = new List<T>() { entityToInsert };
            return await this._dbConnection.InsertAsync<IEnumerable<T>>(inserts, transaction, commandTimeout);
        }

        public long Inserts<T>(IEnumerable<T> inserts, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this._dbConnection.Insert<IEnumerable<T>>(inserts, transaction, commandTimeout);
        }

        public async Task<long> InsertsAsync<T>(IEnumerable<T> inserts, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await this._dbConnection.InsertAsync<IEnumerable<T>>(inserts, transaction, commandTimeout);
        }
        public bool Update<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this._dbConnection.Update<T>(entityToInsert, transaction, commandTimeout);
        }
        public async Task<bool> UpdateAsync<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (transaction == null)
            {
                transaction = _dbtransaction;
            }
            return await this._dbConnection.UpdateAsync<T>(entityToInsert, transaction, commandTimeout);
        }

        public bool Delete<T>(T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this._dbConnection.Delete<T>(model, transaction, commandTimeout);
        }
        public async Task<bool> DeleteAsync<T>(T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await this._dbConnection.DeleteAsync<T>(model, transaction, commandTimeout);
        }

        public bool DeleteAll<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this._dbConnection.DeleteAll<T>(transaction, commandTimeout);
        }




        public IDbTransaction BeginTransaction()
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }
            _dbtransaction = _dbConnection.BeginTransaction();
            return _dbtransaction;
        }
        public void Commit()
        {
            _dbtransaction.Commit();
            _dbConnection.Close();
        }
        public void Rollback()
        {
            _dbtransaction.Rollback();
            _dbConnection.Close();
        }
        public void Dispose()
        {
            _dbConnection.Close();
        }

        public K Tran<K>(Func<IDbTransaction, K> func)
        {
            if (_dbConnection.State == ConnectionState.Closed) { this._dbConnection.Open(); }
            using (var tran = _dbConnection.BeginTransaction())
            {
                try
                {
                    var result = func(tran);
                    tran.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    _log.LogError(ex, null);
                    throw ex;
                }
                finally
                {
                    _dbConnection.Close();
                }
            }
        }



        /// <summary>
        /// 指定字段更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public T Update<T>(T model, IDbTransaction transaction = null, params string[] fields)
        {
            StringBuilder strSql = new StringBuilder();

            Type type = typeof(T);

            strSql.AppendFormat("update {0} set ", type.Name);

            //找出主键名称
            string primaryKey = this.GetPrimaryKey(type).Name;

            if (fields == null || fields.Count() == 0)
            {
                //如果未指定字段，从模型属性中取
                foreach (var prop in this.GetFiledNames(type))
                {
                    if (prop.Name == primaryKey)
                        continue;
                    strSql.AppendFormat("[{0}]=@{0},", prop.Name);
                }
            }
            else
            {
                //如果指定了字段，从字段中取
                foreach (var filedName in fields)
                {
                    strSql.AppendFormat("[{0}]=@{0},", filedName);
                }
            }
            strSql.Remove(strSql.Length - 1, 1);
            strSql.Append(" where ");
            strSql.AppendFormat("[{0}]=@{0}", primaryKey);
            bool success = this._dbConnection.Execute(strSql.ToString(), model, transaction) > 0;
            if (success)
                return model;
            else
                return default(T);
        }


        /// <summary>
        /// 指定字段更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="primaryKey"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<T> UpdateAsync<T>(T model, IDbTransaction transaction = null, params string[] fields)
        {
            StringBuilder strSql = new StringBuilder();

            Type type = typeof(T);

            strSql.AppendFormat("update [{0}] set ", type.Name);

            //找出主键名称
            string primaryKey = this.GetPrimaryKey(type).Name;

            if (fields == null || !fields.Any())
            {
                //如果未指定字段，从模型属性中取
                foreach (var prop in this.GetFiledNames(type))
                {
                    if (prop.Name == primaryKey)
                        continue;
                    strSql.AppendFormat("[{0}]=@{0},", prop.Name);
                }
            }
            else
            {
                //如果指定了字段，从字段中取
                foreach (var filedName in fields)
                {
                    strSql.AppendFormat("[{0}]=@{0},", filedName);
                }
            }
            strSql.Remove(strSql.Length - 1, 1);
            strSql.Append(" where ");
            strSql.AppendFormat("[{0}]=@{0}", primaryKey);
            if (transaction == null)
            {
                transaction = _dbtransaction;
            }
            bool success = await this._dbConnection.ExecuteAsync(strSql.ToString(), model, transaction) > 0;
            if (success)
                return model;
            else
                return default(T);
        }

        /// <summary>
        /// 单表带条件分页查询，
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="fileds"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public (IEnumerable<T>, int total) GetBy<T>(
         string where,
         object param = null,
         string order = null,
         string[] fileds = null,
         bool isPage = false,
         int offset = 0,
         int limit = 20,
         IDbTransaction transaction = null) where T : class
        {
            Type typeInfo = typeof(T);
            if (fileds == null || fileds.Length <= 0)
                fileds = GetFiledNames(typeInfo).Select(filed => filed.Name).ToArray();
            string tableName = GetTableName(typeInfo);
            var primary = this.GetPrimaryKey(typeInfo);

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT {0} FROM {1} ", string.Join(",", fileds), tableName);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" WHERE {0} ", where);
            }

            if (string.IsNullOrEmpty(order))
            {
                sql.AppendFormat("ORDER BY {0} ", primary.Name);
            }
            else
            {
                sql.AppendFormat("ORDER BY {0} ", order);
            }

            if (isPage)
            {
                sql.AppendFormat(@"OFFSET {0} ROWS
                                    FETCH NEXT {1} ROWS ONLY", offset, limit);
                sql.Append(";");

                StringBuilder static_sql = new StringBuilder();
                static_sql.AppendFormat("SELECT count(1) count FROM {0} ", tableName);
                if (!string.IsNullOrEmpty(where))
                {
                    static_sql.AppendFormat(" WHERE {0} ", where);
                }
                static_sql.Append(";");
                sql.Append(static_sql);
                using (var multi = this._dbConnection.QueryMultiple(sql.ToString(), param))
                {
                    var list = multi.Read<T>();
                    int t = multi.ReadFirst()?.count ?? 0;

                    return (list, t);
                }
            }
            else
            {
                var result = this._dbConnection.Query<T>(sql.ToString(), param, transaction);
                return (result, -1);
            }
        }

        /// <summary>
        /// 单表带条件查询，
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="fileds"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public IEnumerable<T> GetBy<T>(
         string where,
         object param = null,
         string order = null,
         string[] fileds = null,
         IDbTransaction transaction = null) where T : class
        {
            Type typeInfo = typeof(T);
            if (fileds == null || fileds.Length <= 0)
                fileds = GetFiledNames(typeInfo).Select(filed => filed.Name).ToArray();
            string tableName = GetTableName(typeInfo);
            var primary = this.GetPrimaryKey(typeInfo);

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT {0} FROM {1} ", string.Join(",", fileds), tableName);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" WHERE {0} ", where);
            }

            if (string.IsNullOrEmpty(order))
            {
                sql.AppendFormat("ORDER BY [{0}] ", primary.Name);
            }
            else
            {
                sql.AppendFormat("ORDER BY {0} ", order);
            }
            sql.Append(";");

            var result = this._dbConnection.Query<T>(sql.ToString(), param, transaction);
            return result;
        }


        /// <summary>
        /// 单表带条件查询，
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="fileds"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetByAsync<T>(
         string where,
         object param = null,
         string order = null,
         string[] fileds = null,
         IDbTransaction transaction = null) where T : class
        {
            Type typeInfo = typeof(T);
            if (fileds == null || fileds.Length <= 0)
                fileds = GetFiledNames(typeInfo).Select(filed => $"[{filed.Name}]").ToArray();
            string tableName = GetTableName(typeInfo);
            var primary = this.GetPrimaryKey(typeInfo);

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT {0} FROM {1} ", string.Join(",", fileds), tableName);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" WHERE {0} ", where);
            }

            if (string.IsNullOrEmpty(order))
            {
                sql.AppendFormat("ORDER BY [{0}] ", primary.Name);
            }
            else
            {
                sql.AppendFormat("ORDER BY {0} ", order);
            }
            sql.Append(";");

            var result = await this._dbConnection.QueryAsync<T>(sql.ToString(), param, transaction);
            return result;
        }


        /// <summary>
        /// 单表待条件制定(top)查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="top"></param>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="order"></param>
        /// <param name="fileds"></param>
        /// <returns></returns>
        public IEnumerable<T> GetBy<T>(
         int top,
         string where,
         object param = null,
         string order = null,
         string[] fileds = null) where T : class
        {
            top = top < 1 ? 1 : top;
            Type typeInfo = typeof(T);
            if (fileds == null || fileds.Length <= 0)
                fileds = GetFiledNames(typeInfo).Select(filed => filed.Name).ToArray();
            string tableName = GetTableName(typeInfo);
            var primary = this.GetPrimaryKey(typeInfo);

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" SELECT top {2} {0} FROM {1} ", string.Join(",", fileds), tableName, top);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" WHERE {0} ", where);
            }

            if (string.IsNullOrEmpty(order))
            {
                sql.AppendFormat("ORDER BY {0} ", primary.Name);
            }
            else
            {
                sql.AppendFormat("ORDER BY {0} ", order);
            }
            sql.Append(";");

            var result = this._dbConnection.Query<T>(sql.ToString(), param);
            return result;
        }




        /// <summary>
        /// 获取实体的TableName
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private string GetTableName(Type typeInfo)
        {
            var tableAttr = typeInfo.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
            {
                return tableAttr.Name;
            }
            else
            {
                return typeInfo.Name;
            }
        }

        /// <summary>
        /// 获取实体字段列表
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private PropertyInfo[] GetFiledNames(Type typeInfo)
        {
            List<PropertyInfo> exffectivePInfo = new List<PropertyInfo>();
            var p = typeInfo.GetProperties();
            foreach (var item in p)
            {
                if (!item.IsDefined(typeof(ComputedAttribute)))
                {
                    if (item.IsDefined(typeof(WriteAttribute)))
                    {
                        var write = item.GetCustomAttribute<WriteAttribute>();
                        if (write.Write)
                        {
                            exffectivePInfo.Add(item);
                        }
                    }
                    else
                    {
                        exffectivePInfo.Add(item);
                    }
                }
            }
            return exffectivePInfo.ToArray();
        }

        /// <summary>
        /// 获取实体主键
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private PropertyInfo GetPrimaryKey(Type typeInfo)
        {
            var primary = typeInfo.GetProperties().Where(p =>
            {
                return p.IsDefined(typeof(KeyAttribute)) || p.IsDefined(typeof(ExplicitKeyAttribute));
            }).FirstOrDefault();
            return primary;
        }

        /// <summary>
        /// 单表带条件查询[可指示分页]
        /// </summary>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="order"></param>
        /// <param name="fileds"></param>
        /// <param name="isPage"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>

        public (IEnumerable<T>, int total) SelectPage<T>(string where, object param = null, string order = null, string[] fileds = null, bool isPage = false, int offset = 0, int limit = 20) where T : class
        {
            return GetBy<T>(where, param, order, fileds, isPage, offset, limit);
        }

        public IEnumerable<T> Select<T>(string where, object param = null, string order = null, string[] fileds = null) where T : class
        {
            return GetBy<T>(where, param, order, fileds, null);
        }

        public T Update<T>(T model, params string[] fileds) where T : class
        {
            return Update<T>(model, null, fileds);
        }

        public T Add<T>(T model) where T : class
        {
            long statu = Insert<T>(model);
            if (statu > 0)
            {
                return model;
            }
            else
            {
                return null;
            }
        }

        public bool Delete<T>(T model) where T : class
        {
            return Delete(model);
        }

        public T TakeFirst<T>(string where = null, object param = null, string order = null) where T : class
        {
            var _type = typeof(T);
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat($" SELECT * FROM {GetTableName(_type)} ");
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" WHERE {0} ", where);
            }

            if (string.IsNullOrEmpty(order))
            {
                sql.AppendFormat("ORDER BY {0} ", GetPrimaryKey(_type).Name);
            }
            else
            {
                sql.AppendFormat("ORDER BY {0} ", order);
            }
            return Query<T>(sql.ToString(), param, null, System.Data.CommandType.Text).FirstOrDefault();
        }

    }
}
