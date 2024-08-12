using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Dapper.Contrib.Extensions;
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;
    using System.Linq;
    using System.Reflection;

    public class BaseQueryRepository<T> : IBaseQueryRepository<T> where T : class
    {
        protected IDBContext _db { get; }
        [Obsolete()]
        protected IDBContext db => _db;

        public BaseQueryRepository(OperationQueryDBContext db)
        {
            _db = db;
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

        public (IEnumerable<T>, int total) SelectPage(string where, object param = null, string order = null, string[] fileds = null, bool isPage = false, int offset = 0, int limit = 20)
        {
            return this._db.GetBy<T>(where, param, order, fileds, isPage, offset, limit);
        }

        public IEnumerable<T> Select(string where, object param = null, string order = null, string[] fileds = null)
        {
            return this._db.GetBy<T>(where, param, order, fileds,null);
        }

        public T TakeFirst(string where = null, object param = null, string order = null)
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
            return this._db.Query<T>(sql.ToString(), param, null, System.Data.CommandType.Text).FirstOrDefault();
        }

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

        private PropertyInfo GetPrimaryKey(Type typeInfo)
        {
            var primary = typeInfo.GetProperties().Where(p =>
            {
                return p.IsDefined(typeof(KeyAttribute)) || p.IsDefined(typeof(ExplicitKeyAttribute));
            }).FirstOrDefault();
            return primary;
        }
    }
}