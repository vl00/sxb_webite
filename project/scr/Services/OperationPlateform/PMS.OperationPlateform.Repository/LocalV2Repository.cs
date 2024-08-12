using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.IRespositories;
    using Domain.Entitys;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class LocalV2Repository : ILocalV2Repository
    {
        private ISchoolDataDBContext db;

        public LocalV2Repository(ISchoolDataDBContext schoolDBContext)
        {
            this.db = schoolDBContext;
        }

        public local_v2 GetById(int id)
        {
            string sql = @"SELECT id,name,parentid parent FROM KeyValue WHERE type=1 AND IsValid=1 AND  id=@id";
            return this.db.QuerySingle<local_v2>(sql, new { id = id });
        }

        public IEnumerable<local_v2> GetByParent(int pid)
        {
            string sql = @"SELECT id,name,parentid parent FROM KeyValue WHERE type=1 AND IsValid=1 AND  parentid=@parent";
            return this.db.Query<local_v2>(sql, new { parent = pid });
        }
    }
}