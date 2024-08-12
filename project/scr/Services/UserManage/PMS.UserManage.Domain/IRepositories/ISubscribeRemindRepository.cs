
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ISubscribeRemindRepository
    {
        bool Add(SubscribeRemind subscribeRemind);
        bool Exists(string groupCode, Guid subjectId, Guid userId);
        IEnumerable<SubscribeRemind> GetPagination(string groupCode, int pageIndex = 1, int pageSize = 10);
        bool Update(SubscribeRemind subscribeRemind);
    }
}