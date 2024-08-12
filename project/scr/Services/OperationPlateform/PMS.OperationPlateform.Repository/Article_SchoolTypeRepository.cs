using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using PMS.OperationPlateform.Domain.IRespositories;
    using PMS.OperationPlateform.Domain.Entitys;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class Article_SchoolTypeRepository : BaseRepository<Article_SchoolTypes>, IArticle_SchoolTypeRepository
    {
        public Article_SchoolTypeRepository(OperationDBContext db) : base(db)
        {
        }




    }
}
