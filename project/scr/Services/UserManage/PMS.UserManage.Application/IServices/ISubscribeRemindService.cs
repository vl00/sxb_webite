using PMS.UserManage.Application.Services;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PMS.UserManage.Application.IServices
{
    public interface ISubscribeRemindService
    {
        bool Add(SubscribeRemindAddDto dto);
        bool Exists(string groupCode, Guid subjectId, Guid userId);
        IEnumerable<SubscribeRemind> GetPagination(string groupCode, int pageIndex = 1, int pageSize = 10);
    }
}