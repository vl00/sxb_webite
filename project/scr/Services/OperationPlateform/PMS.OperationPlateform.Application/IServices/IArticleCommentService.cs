using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IArticleCommentService
    {

        IEnumerable<dynamic> Statistics_CommentsCount(Guid[] forumIds);
    }
}
