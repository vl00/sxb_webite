using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.IRespositories;
    using Domain.Entitys;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class ActDaRenFristPhaseRepository : BaseRepository<ActDaRenFristPhase>, IActDaRenFristPhaseRepository
    {
        public ActDaRenFristPhaseRepository(OperationDBContext db) : base(db)
        {
        }
    }
}