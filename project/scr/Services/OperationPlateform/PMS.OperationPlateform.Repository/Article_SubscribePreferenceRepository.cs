
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class Article_SubscribePreferenceRepository : BaseRepository<Article_SubscribePreference>, IArticle_SubscribePreferenceRepository
    {
        public Article_SubscribePreferenceRepository(OperationDBContext db) : base(db)
        {
        }


    }
}
