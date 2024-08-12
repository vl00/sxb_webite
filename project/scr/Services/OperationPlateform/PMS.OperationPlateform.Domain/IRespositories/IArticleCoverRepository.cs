using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IArticleCoverRepository:IBaseQueryRepository<article_cover>
    {
        IEnumerable<article_cover> GetCoversByIds(Guid[] articleIds);
    }
}
