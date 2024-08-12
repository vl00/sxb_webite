using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SchoolTagRepository : EntityFrameworkRepository<SchoolTag>, ISchoolTagRepository
    {
        public SchoolTagRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public List<SchoolTag> GetSchoolTagByCommentId(Guid Id)
        {
            return base.GetList(x => x.SchoolCommentId == Id).Include(q => q.SchoolComment).Include(q => q.Tag)?.ToList();
        }
        public List<SchoolTag> GetSchoolTagByCommentId(List<Guid> Ids)
        {
            return base.GetList(x => Ids.Contains(x.SchoolCommentId)).Include(q=>q.SchoolComment).Include(q=>q.Tag)?.ToList();
        }

        public List<SchoolTag> GetSchoolTagBySchoolId(Guid Id)
        {
            return base.GetList(x => x.SchoolId == Id).Include(q => q.SchoolComment).Include(q => q.Tag)?.ToList();
        }

        public new int Add(SchoolTag tag)
        {
            return base.Add(tag);
        }

    }

}
