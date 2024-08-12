using System;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Framework.EntityFramework;

namespace PMS.CommentsManage.Repository
{
    public class CommentsEFRepository<Entity> : EntityFrameworkRepository<Entity> where Entity : class, new()
    {
        public CommentsEFRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        //public EntityFrameworkRepository<Entity> GetEFRepository() 
        //{
        //    return new EntityFrameworkRepository<Entity>(entityFramework);
        //}
    }
}
