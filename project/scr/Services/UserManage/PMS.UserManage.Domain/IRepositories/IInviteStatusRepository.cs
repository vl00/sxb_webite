using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IInviteStatusRepository
    {
        void AddInviteStatus(List<InviteStatus> models);
        bool UpdateInviteStatu(bool Read, Guid UserId = default, Guid SenderId = default, List<Guid> DataId = default);
        List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size);

        List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId);
    }
}
