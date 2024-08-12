using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolTagService
    {
        List<SchoolTag> GetSchoolTagBySchoolId(Guid Id);
        List<SchoolTag> GetSchoolTagByCommentId(Guid Id);
        int Add(SchoolTag tag);
    }
}
