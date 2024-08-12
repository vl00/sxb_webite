using System;
namespace PMS.AdminManage.Application.IServices
{
    public interface IAdminService
    {
        bool SyncUserInfo(Guid UserId, string userName, string phone, int role, string headImg);
    }
}
