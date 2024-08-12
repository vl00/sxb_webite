using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices.IMQService
{
    public interface ISchoolCommentLikeMQService
    {
        void SchoolCommentLike(GiveLike giveLike);
    }
}
