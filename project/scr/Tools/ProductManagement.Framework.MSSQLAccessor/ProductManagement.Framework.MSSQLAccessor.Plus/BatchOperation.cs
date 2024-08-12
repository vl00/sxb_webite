using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dapper;
using System.Transactions;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using ProductManagement.Framework.MSSQLAccessor;

namespace ProductManagement.Framework.MSSQLAccessor.Plus
{
    public class BatchOperation<T>
    {
        private readonly ILogger<BatchOperation<T>> _log;

        private readonly ConnectionConfig<T> _connectionConfig;

        //private readonly IDbConnection _dbConnection;

        //private IDbTransaction _dbtransaction;

        //private int _batchSize = 2000;      

        //public BatchOperation(ConnectionConfig connectionConfig)
        //{
        //    _connectionConfig = connectionConfig;
        //}

        public BatchOperation(ConnectionConfig<T> connectionConfig, ILogger<BatchOperation<T>> log)
        {
            _connectionConfig = connectionConfig;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public BatchOperation(ConnectionConfig<T> connectionConfig, ILoggerFactory loggerFactory)
        {
            _connectionConfig = connectionConfig;
            _log = loggerFactory.CreateLogger<BatchOperation<T>>();
        }

        /// <summary>
        /// 批量执行
        /// </summary>
        /// <param name="sql">语句</param>
        /// <param name="listParam">参数列表</param>
        /// <param name="batchSize">一次执行行数，默认2000</param>
        /// <param name="threadPoolSize">线程池大小，建议设置范围10~60</param>
        /// <returns></returns>
        public int BatchExecute(string sql, List<object> listParam, int batchSize = 2000, int threadPoolSize = 10)
        {
            int result = 0;
            int totalBatch = listParam.Count / batchSize;
            int batchTotalRecord = totalBatch * batchSize;

            //for (int i = 0; i < totalBatch; i++)
            //{
            //    int index = i == 0 ? 0 : i * _batchSize;

            //    using (var transaction = new TransactionScope())
            //    {
            //        for (int j = 0; j < _batchSize; j++)
            //        {
            //            result = result + _dbConnection.Execute(sql, listParam[index + j]);
            //        }
            //        transaction.Complete();
            //    }
            //}

            var sw = Stopwatch.StartNew();

            var obj = new Object();
            ParallelLoopResult task = Parallel.For(0, totalBatch, new ParallelOptions() { MaxDegreeOfParallelism = threadPoolSize }, i =>
            {
                //var conn = new ConnectionsManager(_connectionConfig).GetDbConnection();
                int record = 0;

                int index = i == 0 ? 0 : i * batchSize;

                try
                {
                    using (var conn = new ConnectionsManager<T>(_connectionConfig).GetDbConnection())
                    {
                        using (var transaction = new TransactionScope())
                        {
                            for (int j = 0; j < batchSize; j++)
                            {
                                record = record + conn.Execute(sql, listParam[index + j]);
                            }
                            transaction.Complete();
                        }
                    }

                    lock (obj)
                    {
                        result = result + record;
                    }
                }
                catch (Exception ex)
                {
                    //log
                    _log.LogError(ex, ex.Message);
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine("{0}, task: {1} , thread: {2} , result: {3}", i, Task.CurrentId, Thread.CurrentThread.ManagedThreadId, result);
            });

            //ParallelLoopResult task = Parallel.For(0, totalBatch, new ParallelOptions() { MaxDegreeOfParallelism = threadPoolSize },
            //    () => { return 0; },
            //    (i, state, local) =>
            //    {
            //        var conn = new ConnectionsManager(_connectionConfig).GetDbConnection();
            //        int record = 0;

            //        int index = i == 0 ? 0 : i * batchSize;

            //        try
            //        {
            //            using (var transaction = new TransactionScope())
            //            {
            //                for (int j = 0; j < batchSize; j++)
            //                {
            //                    record = record + conn.Execute(sql, listParam[index + j]);
            //                }
            //                transaction.Complete();
            //            }

            //            //result = result + record;
            //            local += record;
            //        }
            //        catch (Exception ex)
            //        {
            //            //log
            //            _log.LogError(ex, ex.Message);
            //            //Console.WriteLine(ex.Message);
            //        }

            //        Console.WriteLine("{0}, task: {1} , thread: {2} , result: {3}", i, Task.CurrentId, Thread.CurrentThread.ManagedThreadId, local);
            //        return local;
            //    },
            //    (finalResult) => { Interlocked.Add(ref result, finalResult); });

            if (batchTotalRecord < listParam.Count)
            {
                //var conn = new ConnectionsManager(_connectionConfig).GetDbConnection();
                int record = 0;

                try
                {
                    using (var conn = new ConnectionsManager<T>(_connectionConfig).GetDbConnection())
                    {
                        using (var transaction = new TransactionScope())
                        {
                            for (int j = 0; j < listParam.Count - batchTotalRecord; j++)
                            {
                                record = record + conn.Execute(sql, listParam[batchTotalRecord + j]);
                            }
                            transaction.Complete();
                        }
                    }

                    result = result + record;

                }
                catch (Exception ex)
                {
                    //log
                    _log.LogError(ex, ex.Message);
                    //Console.WriteLine(ex.Message);
                }

                Console.WriteLine("thread: {0} , result:{1}", Thread.CurrentThread.ManagedThreadId, result);
            }
            Console.WriteLine("records: {0} times: {1}s speed:{2}", result, sw.Elapsed.TotalSeconds.ToString(), result / sw.Elapsed.TotalSeconds);

            return result;
        }

        ///// <summary>
        ///// 异步批量执行
        ///// </summary>
        ///// <param name="sql">语句</param>
        ///// <param name="listParam">参数列表</param>
        ///// <param name="batchSize">一次执行行数，默认2000</param>
        ///// <param name="threadPoolSize">线程池大小，建议设置范围10~60</param>
        ///// <returns></returns>
        //public async Task<int> BatchExecuteAsync(string sql, List<object> listParam, int batchSize = 2000, int threadPoolSize = 10)
        //{
        //    int result = 0;
        //    int totalBatch = listParam.Count / batchSize;
        //    int batchTotalRecord = totalBatch * batchSize;

        //    var sw = Stopwatch.StartNew();

        //    var obj = new Object();
        //    ParallelLoopResult task = Parallel.For(0, totalBatch, new ParallelOptions() { MaxDegreeOfParallelism = threadPoolSize }, async i =>
        //    {
        //        //var conn = new ConnectionsManager(_connectionConfig).GetDbConnection();
        //        int record = 0;

        //        int index = i == 0 ? 0 : i * batchSize;

        //        try
        //        {
        //            using (var conn = new ConnectionsManager(_connectionConfig).GetDbConnection())
        //            {
        //                //await conn.OpenAsync();
        //                //var transaction = conn.BeginTransaction();
        //                //for (int j = 0; j < batchSize; j++)
        //                //{
        //                //    record = record + await conn.ExecuteAsync(sql, listParam[index + j], transaction);
        //                //}
        //                //transaction.Commit();
        //                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //                {
        //                    for (int j = 0; j < batchSize; j++)
        //                    {
        //                        record = record + await conn.ExecuteAsync(sql, listParam[index + j]);
        //                    }
        //                    transaction.Complete();
        //                }
        //            }

        //            lock (obj)
        //            {
        //                result = result + record;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //log
        //            _log.LogError(ex, ex.Message);
        //            Console.WriteLine(ex.Message);
        //        }

        //        Console.WriteLine("{0}, task: {1} , thread: {2} , result: {3}", i, Task.CurrentId, Thread.CurrentThread.ManagedThreadId, result);
        //    });

        //    if (batchTotalRecord < listParam.Count)
        //    {
        //        //var conn = new ConnectionsManager(_connectionConfig).GetDbConnection();
        //        int record = 0;

        //        try
        //        {
        //            using (var conn = new ConnectionsManager(_connectionConfig).GetDbConnection())
        //            {
        //                //await conn.OpenAsync();
        //                //var transaction = conn.BeginTransaction();
        //                //for (int j = 0; j < listParam.Count - batchTotalRecord; j++)
        //                //{
        //                //    record = record + await conn.ExecuteAsync(sql, listParam[batchTotalRecord + j]);
        //                //}
        //                //transaction.Commit();
        //                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //                {
        //                    for (int j = 0; j < listParam.Count - batchTotalRecord; j++)
        //                    {
        //                        record = record + await conn.ExecuteAsync(sql, listParam[batchTotalRecord + j]);
        //                    }
        //                    transaction.Complete();
        //                }
        //            }

        //            result = result + record;

        //        }
        //        catch (Exception ex)
        //        {
        //            //log
        //            _log.LogError(ex, ex.Message);
        //            //Console.WriteLine(ex.Message);
        //        }

        //        Console.WriteLine("thread: {0} , result:{1}", Thread.CurrentThread.ManagedThreadId, result);
        //    }
        //    Console.WriteLine("records: {0} times: {1}s speed:{2}", result, sw.Elapsed.TotalSeconds.ToString(), result / sw.Elapsed.TotalSeconds);

        //    return result;
        //}

    }

}
