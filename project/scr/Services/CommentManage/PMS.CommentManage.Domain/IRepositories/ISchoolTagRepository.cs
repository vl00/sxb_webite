using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ISchoolTagRepository
    {
        List<SchoolTag> GetSchoolTagBySchoolId(Guid Id);
        List<SchoolTag> GetSchoolTagByCommentId(Guid Id);
        List<SchoolTag> GetSchoolTagByCommentId(List<Guid> Ids);
        int Add(SchoolTag tag);
    }
}
