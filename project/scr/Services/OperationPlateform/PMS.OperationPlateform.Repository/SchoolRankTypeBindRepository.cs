using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class SchoolRankTypeBindRepository : BaseRepository<SchoolRankTypeBinds>, ISchoolRankTypeBindRepository
    {
        public SchoolRankTypeBindRepository(OperationDBContext db) : base(db)
        {
        }








    }
}
