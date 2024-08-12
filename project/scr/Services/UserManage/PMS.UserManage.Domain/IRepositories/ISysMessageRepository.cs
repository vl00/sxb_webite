using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ISysMessageRepository
    {
        List<SysMessage> GetMessageByType(Guid SenderUserId, int Type, int Page, int Size);
        List<Guid> CheckMessageIdIsExist(List<Guid> DataIds, Guid UserId);
        bool AddSysMessage(List<SysMessage> message);

        Task<int> AddSysMessageState(List<SysMessageState> messageStates);

        Task<int> UpdateMessageRead(List<Guid> dataIds, Guid UserId);

        List<SysMessageTips> GetSysMessageTips(Guid userId, int page);

        List<DynamicItem> GetDynamicItems(Guid userId, int page, int size, bool IsSelf = true);
        List<DynamicItem> GetDynamicItems(List<Guid> userIds, int page, int size, bool IsSelf = true);
        List<SysMessageTips> GetLiveMessageTips(Guid userId, int page, int size);



        List<SysMessageDetail> GetSysMessages(Guid? senderId, Guid? userId, SysMessageType[] types, bool? isRead, DateTime? startTime, DateTime? endTime, int page = 1);
        long GetSysMessageTotal(Guid? senderId, Guid? userId, SysMessageType[] types, bool? isRead, DateTime? startTime, DateTime? endTime);
    }
}
