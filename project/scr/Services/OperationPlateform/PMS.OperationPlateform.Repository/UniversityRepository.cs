using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PMS.OperationPlateform.Repository
{
    public class UniversityRepository : BaseRepository<University>, IUniversityRepository
    {
        public UniversityRepository(OperationQueryDBContext db) : base(db)
        {
        }
    }
}
