using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMS.CommentsManage.Domain.Entities;

namespace PMS.CommentsManage.Repository.Interface
{
    public interface ICommentsManageDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        //CommentsManageDbContext GetContext();
    }
}
