using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IArticleCommentRepository:IBaseRepository<comment>
    {

        IEnumerable<dynamic> Statistics_CommentsCount(Guid[] forumIds);

    }
}
