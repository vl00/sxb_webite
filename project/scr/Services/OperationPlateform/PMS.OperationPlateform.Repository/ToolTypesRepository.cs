using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class ToolTypesRepository : BaseRepository<ToolTypes>, IToolTypesRepository
    {
        public ToolTypesRepository(OperationDBContext db) : base(db)
        {
        }
    }
}
