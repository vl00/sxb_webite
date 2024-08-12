using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class KeyValueRespository : BaseRepository<KeyValue>, IKeyValueRespository

    {
        public KeyValueRespository(OperationDBContext db) : base(db)
        {
        }

        public KeyValue GetKey(string key)
        {
            string sql = "SELECT * FROM KeyValue where [Key]=@key";
            return this._db.Query<KeyValue>(sql, new { key }).FirstOrDefault();
        }
    }
}
