using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class TagRepositories : EntityFrameworkRepository<CommentTag>, ITagRepositories
    {
        public TagRepositories(CommentsManageDbContext repository) : base(repository)
        {
        }

        public new int Add(CommentTag tag)
        {
            return base.Add(tag);
        }


        public CommentTag CheckTagIsExists(string TagName)
        {
            return base.GetList(x => x.Content == TagName).FirstOrDefault();
        }
        
    }
}
