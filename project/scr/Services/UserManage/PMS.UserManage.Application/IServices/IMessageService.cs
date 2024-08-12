using PMS.UserManage.Application.ModelDto.Message;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.IServices
{
    public interface IMessageService
    {
        int ContentMaxLength { get;}
        int GetMessageCount(Guid userID);
        List<DataMsg> GetDataMsgs(Guid userId, MessageType? type, MessageDataType? dataType, bool? ignore, int page, int size);
        List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size);
        bool UpdateMessageHandled(bool handled, MessageType type, MessageDataType dataType, Guid UserId = default, Guid SenderId = default, List<Guid> DataId = default);
        bool AddMessage(Message model);
        bool RemoveMessage(Guid msgID, Guid userID);
        List<Message> GetUserMessage(Guid userID, byte[] type, int page, int pageSize = 10);
        Push GetPushSetting(Guid userID);
        bool SetPushSetting(Push model);

        List<MessageModel> GetSystemMessage(Guid userID, int page);
        List<ApiMessageModel> GetFollowMessage(Guid userID, int page);
        List<ApiMessageModel> GetPrivateMessage(Guid userID, int page, int size = 10, List<byte> messageType = null, bool isAuth = false, bool? read = null, bool? handled = false);
        List<InviteSelf> GetMessageBySenderUser(Guid userId, List<int> type,int page,int size);
        List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId);
        List<MessageTipsTotal> TotalTips(Guid UserId);
        void UpdateMessageState(List<int> type, Guid userId);
        void IgnoreInvite(List<Guid> ids, Guid userId, bool isAll);
        List<DataMsg> GetReplyME(Guid UserID, int page, int size);

        List<DataMsg> GetMyLike( int searchType, Guid userId, int page, int size);
        List<DataMsg> GetMyReply(int type, Guid UserID, int page, int size);

        List<Message> GetMessageJob(DateTime startTime, DateTime endTime, int page, int size);
        int GetMessageJobTotal(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取消息数量
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <param name="dataType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        long GetMessageTotal(Guid? senderId, Guid? userId, MessageType? type, MessageDataType? dataType, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime);
        long GetMessageTotal(Guid? senderId, Guid? userId, MessageType[] types, MessageDataType[] dataTypes, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime, DateTime? pushStartTime, DateTime? pushEndTime);
        Task<bool> ClearMyInviteRedisData(Guid userId, Guid dataId, MessageType type);
        Task<bool> CheckMyInviteRedisData(Guid userId, Guid dataId, MessageType type);
        List<Guid> GetMessageUserIds(Guid senderId, Guid[] userIds, Guid dataId, MessageType type, MessageDataType dataType, Guid? eId);
    }
}
