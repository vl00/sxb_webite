using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IArticle_SchoolBindRepository:IBaseRepository<Article_SchoolBind>
    {

        IEnumerable<Guid> GetArticleIds(Guid[] sids);
    }
}
