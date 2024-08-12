using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.TopicCircle.Repository
{
    public class KeyValueRepository : Repository<keyValue, ISchoolDBContext>, IKeyValueRepository
    {
        ISchoolDBContext _dbContext;
        public KeyValueRepository(ISchoolDBContext dBContext) : base(dBContext)
        {
            this._dbContext = dBContext;
        }

        public override keyValue Get<Tkey>(Tkey key)
        {
            string sql = "SELECT * FROM keyValue WHERE [key]=@key ";
            return this._dbContext.Query<keyValue>(sql, new { key }).FirstOrDefault();
        }



    }
}
