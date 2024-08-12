using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using ProductManagement.Framework.Foundation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProductManagement.Framework.EntityFramework
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class EntityFrameworkRepository<Entity> : Repository<Entity> where Entity : class, new()
    {
        private readonly DbContext context;
        private readonly DbSet<Entity> _dbset;

        private IDbContextTransaction _contextTransaction;
        public EntityFrameworkRepository(DbContext repository) : base(repository)
        {
                this.context = repository;
                this._dbset = this.context.Set<Entity>();
        }

        private bool IsBeginTran = false;

        public void BeginTransaction()
        {
            _contextTransaction = context.Database.BeginTransaction();
            IsBeginTran = true;
        }

        public void Rollback()
        {
            if (_contextTransaction != null)
                _contextTransaction.Rollback();
        }

        public int CommitAsync()
        {
            return context.SaveChanges();
        }


        public void TranCommit()
        {
            if (_contextTransaction != null)
                _contextTransaction.Commit();
        }

        public override int Add(Entity Entity)
        {
            this._dbset.Add(Entity);
            return IsBeginTran ? 0: this.context.SaveChanges();
        }

        public override int Delete(Guid Id)
        {
            Entity entity = _dbset.Find(Id);
            _dbset.Remove(entity);
            return IsBeginTran ? 0 : this.context.SaveChanges();
        }

        public override Entity GetAggregateById(Guid Id)
        {
            return _dbset.Find(Id);
        }

        public override int GetCount(Expression<Func<Entity, bool>> where = null)
        {
            if (where != null)
                return _dbset.Count(where);
            else
                return _dbset.Count();
        }



        public override IQueryable<Entity> GetList(Expression<Func<Entity, bool>> where = null)
        {
            if (where == null)
                return _dbset;
            else
                return _dbset.Where(where);
        }

        public override IEnumerable<Entity> GetPageList(Expression<Func<Entity, bool>> where = null, string order = "", int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<Entity> Entitys = _dbset;

            if (order != "")
            {
                Entitys = Entitys.OrderBy(order);
            }

            if (where != null)
            {
                Entitys = Entitys.Where(where);
            }
            return Entitys.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public override IEnumerable<Entity> QueryByProc(string procName, out List<object> outputData,params SqlParameter[] paras)
        {
            outputData = new List<object>();

            DbConnection dbConnection = context.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            context.Database.OpenConnection();
            

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("exec [dbo].[{0}] ", procName);

            if (paras != null)
            {
                foreach (SqlParameter item in paras)
                {
                    sql.Append(item.ParameterName);
                    if (item.Direction == System.Data.ParameterDirection.Output)
                        sql.Append(" output");
                    sql.Append(",");
                }
                if (paras.Length > 0)
                    sql.Remove(sql.Length - 1, 1);

                dbCommand.Parameters.AddRange(paras.ToArray());
            }

            dbCommand.CommandText = sql.ToString();
            //dbCommand.CommandType = CommandType.StoredProcedure;

            DataTable dataTable = new DataTable();
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                dataTable.Load(dbDataReader);
            }

            foreach (var item in paras)
            {
                if (item.Direction == ParameterDirection.Output)
                {
                     outputData.Add(item.Value);
                }
            }

            context.Database.CloseConnection();
            List<Entity> rez = dataTable.ToList<Entity>();
            return rez;
        }

        public override IEnumerable<Entity> Query(string sql, params SqlParameter[] paras)
        {
            DbConnection dbConnection = context.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            context.Database.OpenConnection();

            if (paras != null)
            {
                dbCommand.Parameters.AddRange(paras);
            }
            dbCommand.CommandText = sql;
            DataTable dataTable = new DataTable();
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                dataTable.Load(dbDataReader);
            }
            context.Database.CloseConnection();
            List<Entity> rez = dataTable.ToList<Entity>();
            return rez ?? new List<Entity>();
        }
        public override IEnumerable<TEntity> Query<TEntity>(string sql, params SqlParameter[] paras)
        {
            DbConnection dbConnection = context.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            context.Database.OpenConnection();

            if (paras != null)
            {
                dbCommand.Parameters.AddRange(paras);
            }
            dbCommand.CommandText = sql;
            DataTable dataTable = new DataTable();
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                dataTable.Load(dbDataReader);
                dbDataReader.Close();
                dbCommand.Parameters.Clear();
            }
            context.Database.CloseConnection();
            List<TEntity> rez = dataTable.ToList<TEntity>();
            return rez ?? new List<TEntity>();
        }

        public override int Update(Entity Entity)
        {
            EntityEntry<Entity> entity = _dbset.Update(Entity);
            //数据更新，修改EF数据本源
            entity.State = EntityState.Modified;
            return IsBeginTran ? 0 : context.SaveChanges();
        }

        public override object ExecuteScalar(string sql, params SqlParameter[] paras)
        {
           DbConnection connection =  context.Database.GetDbConnection();
           DbCommand cmd = connection.CreateCommand();
           connection.Open();

            cmd.CommandText = sql;
            if (paras != null)
            {
                cmd.Parameters.AddRange(paras);
            }


            object obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            connection.Close();
            return obj == DBNull.Value ? null : obj;
        }

        public override int ExecuteNonQuery(string sql, params SqlParameter[] paras)
        {
            //DbConnection dbConnection = context.Database.GetDbConnection();
            //DbCommand cmd = dbConnection.CreateCommand();
            //context.Database.OpenConnection();
            // cmd.CommandText = sql;
            // if (paras != null)
            // {
            //     cmd.Parameters.AddRange(paras);
            // }
            // //int rez = cmd.ExecuteNonQuery();

            // int rez = context.CommitAsync();
            // //context.Database.CloseConnection();
            // return rez;
            int rez = 0;
            if (paras != null)
            {
                rez = context.Database.ExecuteSqlCommand(sql, paras);
            }
            else
            {
                rez = context.Database.ExecuteSqlCommand(sql);
            }
            return rez;
        }

        public override void TranAdd(Entity entity)
        {
            this._dbset.Add(entity);
        }

        public override IEnumerable<TEntity> QueryByProc<TEntity>(string procName, out List<object> outputData, params SqlParameter[] paras)
        {
            outputData = new List<object>();

            DbConnection dbConnection = context.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            context.Database.OpenConnection();


            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("exec [dbo].[{0}] ", procName);

            if (paras != null)
            {
                foreach (SqlParameter item in paras)
                {
                    sql.Append(item.ParameterName);
                    if (item.Direction == System.Data.ParameterDirection.Output)
                        sql.Append(" output");
                    sql.Append(",");
                }
                if (paras.Length > 0)
                    sql.Remove(sql.Length - 1, 1);

                dbCommand.Parameters.AddRange(paras.ToArray());
            }

            dbCommand.CommandText = sql.ToString();
            //dbCommand.CommandType = CommandType.StoredProcedure;

            DataTable dataTable = new DataTable();
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                dataTable.Load(dbDataReader);
            }

            foreach (var item in paras)
            {
                if (item.Direction == ParameterDirection.Output)
                {
                    outputData.Add(item.Value);
                }
            }

            context.Database.CloseConnection();
            List<TEntity> rez = dataTable.ToList<TEntity>();
            return rez;
        }
    }
}
