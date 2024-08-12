using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IArticleCoverService
    {
        IEnumerable<article_cover> GetCoversByIds(Guid[] articleIds);
    }
}
