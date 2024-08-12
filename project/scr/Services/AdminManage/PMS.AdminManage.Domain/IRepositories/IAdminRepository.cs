using System;
using PMS.AdminManage.Domain.Entities;

namespace PMS.AdminManage.Domain.IRepositories
{
    public interface IAdminRepository
    {
        AdminInfo GetUserInfo(Guid userId);
        bool AddUserInfo(AdminInfo user);
        bool UpdateUserInfo(AdminInfo user);
    }
}
