using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface ISysMessageService
    {
        List<SysMessage> GetMessageByType(Guid SenderUserId, int Type, int Page, int Size);
        List<Guid> CheckMessageIdIsExist(List<Guid> DataIds, Guid UserId);
        bool AddSysMessage(List<SysMessage> message);
        Task<int> AddSysMessageState(List<SysMessageState> messageStates);

        Task<int> UpdateMessageRead(List<Guid> dataIds, Guid UserId);
        List<SysMessageTips> GetSysMessageTips(Guid userId, int page);
        List<SysMessageDetail> GetSysMessages(Guid? senderId, Guid? userId, int page = 1);
        List<SysMessageDetail> GetSysMessages(Guid? senderId, Guid? userId, SysMessageType[] types, bool? isRead, DateTime? startTime, DateTime? endTime, int page = 1);
        List<DynamicItem> GetDynamicItems(Guid userId, int page, int size, bool IsSelft = true);
        List<DynamicItem> GetDynamicItems(List<Guid> userIds, int page, int size, bool IsSelft = true);
        List<SysMessageTips> GetLiveMessageTips(Guid userId, int page, int size);
        Task<bool> HasSysMessage(Guid userId);
        Task<bool> RefreshSysMessageTime(Guid userId);
    }
}
