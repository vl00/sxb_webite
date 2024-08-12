using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IMessageRepository
    {
        int GetMessageCount(Guid userID);
        void UpdateMessageState(List<int> type, Guid userId);
        bool UpdateReadState(bool IsRead, MessageType type, MessageDataType dataType, Guid UserId = default, Guid SenderId = default, List<Guid> DataId = default);
        bool UpdateMessageHandled(bool handled, bool read, MessageType type, MessageDataType dataType, Guid? userId, Guid? senderId, List<Guid> dataId);
        List<DataMsg> GetDataMsgs(Guid userId, MessageType? type, MessageDataType? dataType, bool? ignore, int page, int size);
        List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size);
        bool AddMessage(Message model);
        bool RemoveMessage(Guid msgID, Guid userID);
        List<Message> GetUserMessage(Guid userID, byte[] type, int page, int pageSize = 10, bool isAuth = false, bool? read = null, bool? handled = null);
        Push GetPushSetting(Guid userID);
        bool SetPushSetting(Push model);
        List<InviteSelf> GetMessageBySenderUser(Guid userId, List<int> type, int page, int size);
        List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId);
        List<MessageTipsTotal> TotalTips(Guid UserId, DateTime? startTime);
        void IgnoreInvite(List<Guid> ids, Guid userId, bool isAll);
        List<DataMsg> GetReplyME(Guid UserID, int page, int size);
        List<Message> GetMessageJob(DateTime startTime, DateTime endTime, int page, int size);
        int GetMessageJobTotal(DateTime startTime, DateTime endTime);
        long GetMessageTotal(Guid? senderId, Guid? userId, MessageType[] types, MessageDataType[] dataTypes, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime, DateTime? pushStartTime, DateTime? pushEndTime);
        /// <summary>
        /// 我的点赞列表
        /// </summary>
        /// <param name="searchType">0 点评/点评回复  1问答</param>
        /// <param name="userID"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        List<DataMsg> GetMyLike(int searchType, Guid userID, int page, int size);
        List<DataMsg> GetMyReply(int type, Guid userID, int page, int size);
        bool UpdateMessageIgnore(bool ignore, bool read, MessageType type, MessageDataType dataType, Guid? userId, Guid? senderId, List<Guid> dataId);
        IEnumerable<Message> GetMessages(Guid? senderId, Guid? userId, Guid[] dataIds, MessageType[] types, MessageDataType[] dataTypes, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime, DateTime? pushStartTime, DateTime? pushEndTime);
        bool Update(Guid[] ids);
        IEnumerable<Guid> GetMessageUserIds(Guid senderId, Guid[] userIds, Guid dataId, MessageType type, MessageDataType dataType, Guid? eId);
    }
}
