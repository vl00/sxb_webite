using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.IRespositories;
    using Domain.Entitys;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class SchoolRankBindsRepository :BaseRepository<SchoolRankBinds>,ISchoolRankBindsRepository
    {
        public SchoolRankBindsRepository(OperationDBContext db) : base(db)
        {
        }
    }
}
