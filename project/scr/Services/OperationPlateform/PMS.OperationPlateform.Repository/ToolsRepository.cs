using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class ToolsRepository : BaseRepository<Tools>, IToolsRepository
    {
        public ToolsRepository(OperationDBContext db) : base(db)
        {
        }

        public IEnumerable<dynamic> GetGroupByCityId()
        {
            throw new NotImplementedException();
        }

    }
}
