using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor
{
    public interface IDBContext
    {
        T Add<T>(T model) where T : class;
        IDbTransaction BeginTransaction();
        void Commit();
        bool Delete<T>(T model) where T : class;
        bool Delete<T>(T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        bool DeleteAll<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        int Execute(string sql, object param);
        int Execute(string sql, object param, IDbTransaction transaction);
        Task<int> ExecuteAsync(string sql, object param);
        Task<int> ExecuteAsync(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null);
        T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null);
        int ExecuteUow(string sql, object param);
        T Get<T, TId>(TId id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        IEnumerable<T> GetAll<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<T> GetAsync<T, TId>(TId id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        IEnumerable<T> GetBy<T>(int top, string where, object param = null, string order = null, string[] fileds = null) where T : class;
        (IEnumerable<T>, int total) GetBy<T>(string where, object param = null, string order = null, string[] fileds = null, bool isPage = false, int offset = 0, int limit = 20, IDbTransaction transaction = null) where T : class;
        IEnumerable<T> GetBy<T>(string where, object param = null, string order = null, string[] fileds = null, IDbTransaction transaction = null) where T : class;
        Task<IEnumerable<T>> GetByAsync<T>(string where, object param = null, string order = null, string[] fileds = null, IDbTransaction transaction = null) where T : class;
        long Insert<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<int> InsertAsync<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        long Inserts<T>(IEnumerable<T> inserts, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<long> InsertsAsync<T>(IEnumerable<T> inserts, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        IEnumerable<dynamic> Query(string sql, object param, IDbTransaction transaction = null);
        IEnumerable<T> Query<T>(string sql, object param, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<T> Query<T>(string sql, object param, IDbTransaction transaction, CommandType? commandType = null);
        IEnumerable<TReturn> Query<TFrist, TSecond, TReturn>(string sql, Func<TFrist, TSecond, TReturn> map, object _params = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        SqlMapper.GridReader QueryMultiple(string sql, object param, IDbTransaction tran = null, CommandType? commandType = null);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param, IDbTransaction tran = null, CommandType? commandType = null);
        T QuerySingle<T>(string sql, object param);
        Task<T> QuerySingleAsync<T>(string sql, object param);
        Task<T> QuerySingleAsync<T>(string sql, object param, IDbTransaction transaction);
        IEnumerable<T> QueryUow<T>(string sql, object param);
        void Rollback();
        IEnumerable<T> Select<T>(string where, object param = null, string order = null, string[] fileds = null) where T : class;
        (IEnumerable<T>, int total) SelectPage<T>(string where, object param = null, string order = null, string[] fileds = null, bool isPage = false, int offset = 0, int limit = 20) where T : class;
        T TakeFirst<T>(string where = null, object param = null, string order = null) where T : class;
        K Tran<K>(Func<IDbTransaction, K> func);
        bool Update<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        T Update<T>(T model, IDbTransaction transaction = null, params string[] fields);
        T Update<T>(T model, params string[] fileds) where T : class;
        Task<bool> UpdateAsync<T>(T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        Task<T> UpdateAsync<T>(T model, IDbTransaction transaction = null, params string[] fields);
    }
}