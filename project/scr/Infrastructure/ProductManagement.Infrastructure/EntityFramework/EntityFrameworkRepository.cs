using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using ProductManagement.Infrastructure.Toolibrary;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ProductManagement.Infrastructure.EntityFramework
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class EntityFrameworkRepository<Entity> : Repository<Entity> where Entity : class, new()
    {
        private readonly IEntityFrameworkRepositoryContext context;
        private readonly DbSet<Entity> _dbset;
        public EntityFrameworkRepository(IRepositoryContext repository) : base(repository)
        {
            if (repository is IEntityFrameworkRepositoryContext)
            {
                this.context = (repository as IEntityFrameworkRepositoryContext);
                this._dbset = this.context.dbContext.Set<Entity>();
            }
        }

        public override int Add(Entity Entity)
        {
            this._dbset.Add(Entity);
            return this.context.CommitAsync();
        }

        public override int Delete(Guid Id)
        {
            Entity entity = _dbset.Find(Id);
            _dbset.Remove(entity);
            return this.context.CommitAsync();
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

        public override IEnumerable<Entity> GetList(Expression<Func<Entity, bool>> where = null)
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

            if (where == null)
            {
                Entitys = Entitys.Where(where);
            }
            return Entitys.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }


        public override IEnumerable<Entity> QueryByProc(string procName, out List<object> outputData,params SqlParameter[] paras)
        {
            outputData = new List<object>();

            DbConnection dbConnection = context.dbContext.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            context.dbContext.Database.OpenConnection();
            

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

            context.dbContext.Database.CloseConnection();
            List<Entity> rez = dataTable.ToList<Entity>();
            return rez;
        }

        public override IEnumerable<Entity> Query(string sql, params SqlParameter[] paras)
        {
            DbConnection dbConnection = context.dbContext.Database.GetDbConnection();
            DbCommand dbCommand = dbConnection.CreateCommand();
            context.dbContext.Database.OpenConnection();

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
            context.dbContext.Database.CloseConnection();
            List<Entity> rez = dataTable.ToList<Entity>();
            return rez;
        }

        public override int Update(Entity Entity)
        {
            EntityEntry<Entity> entity = _dbset.Update(Entity);
            //数据更新，修改EF数据本源
            entity.State = EntityState.Modified;
            return context.CommitAsync();
        }

        public override int ExecuteNonQuery(string sql, params SqlParameter[] paras)
        {
            //DbConnection dbConnection = context.dbContext.Database.GetDbConnection();
            //DbCommand cmd = dbConnection.CreateCommand();
            //context.dbContext.Database.OpenConnection();
            // cmd.CommandText = sql;
            // if (paras != null)
            // {
            //     cmd.Parameters.AddRange(paras);
            // }
            // //int rez = cmd.ExecuteNonQuery();

            // int rez = context.CommitAsync();
            // //context.dbContext.Database.CloseConnection();
            // return rez;
            int rez = 0;
            if (paras != null)
            {
                rez = context.dbContext.Database.ExecuteSqlCommand(sql, paras);
            }
            else
            {
                rez = context.dbContext.Database.ExecuteSqlCommand(sql);
            }
            return rez;
        }

    }
}
