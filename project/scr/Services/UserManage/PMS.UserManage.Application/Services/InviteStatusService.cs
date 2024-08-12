using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.Services
{
    public class InviteStatusService : IInviteStatusService
    {
        private IInviteStatusRepository _statusRepository;
        public InviteStatusService(IInviteStatusRepository statusRepository) 
        {
            _statusRepository = statusRepository;
        }

        public void AddInviteStatus(List<InviteStatus> models)
        {
            _statusRepository.AddInviteStatus(models);
        }

        public List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId)
        {
            return _statusRepository.GetInviteUserInfos(dataId, senderId);
        }

        public List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size)
        {
            return _statusRepository.GetMessageBySenderUser(userId, page, size);
        }

        /// <summary>
        /// 更新邀请表状态
        /// </summary>
        /// <param name="Read">是否已阅</param>
        /// <param name="UserId">被邀请者</param>
        /// <param name="SenderId">邀请者</param>
        /// <param name="DataId">数据id</param>
        /// <returns></returns>
        public bool UpdateInviteStatu(bool Read, Guid UserId = default, Guid SenderId = default, List<Guid> DataId = null)
        {
            return _statusRepository.UpdateInviteStatu(Read, UserId, SenderId, DataId);
        }
    }
}
