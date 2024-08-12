using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.Message;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.Services
{
    public class MessageService : IMessageService
    {
        /// <summary>
        /// 存入message的最大字符数
        /// </summary>
        public int ContentMaxLength { get; } = 200;

        //系统消息账号
        private readonly Guid SysMessage = Guid.Parse("79AB946D-F6AE-4A31-9530-4B8C8EC7CA25");

        private IMessageRepository _message;
        private INewAccountRepository _account;
        private ISysMessageService _sysMessage;
        private ISchoolInfoService _schoolInfoService;

        private readonly IEasyRedisClient _easyRedisClient;

        private readonly string _inviteReplyRedisKey = "myInviteReplyHash";

        public IsHost _IsHost { get; set; }
        public MessageService(IMessageRepository message,
            INewAccountRepository account,
            ISysMessageService sysMessage,
            IOptions<IsHost> IsHost, IEasyRedisClient easyRedisClient, ISchoolInfoService schoolInfoService)
        {
            _message = message;
            _account = account;
            _IsHost = IsHost.Value;

            _sysMessage = sysMessage;
            _easyRedisClient = easyRedisClient;
            _schoolInfoService = schoolInfoService;
        }

        public int GetMessageCount(Guid userID)
        {
            return _message.GetMessageCount(userID);
        }

        public bool AddMessage(Message model)
        {
            //if (model.DataType == 2 && model.Type == 1) 
            //{
            //    //推送系统消息
            //    _sysMessage.AddSysMessage(new SysMessage() { 
            //        SenderUserId = SysMessage,
            //        UserId = model.userID,
            //        Type = SysMessageType.InviteChange,
            //        DataId = model.DataID,
            //        Content = "您的提问有人回答了，点击查看>>"
            //    });
            //}
            if (!string.IsNullOrWhiteSpace(model.Content) && model.Content.Length > ContentMaxLength)
                model.Content = model.Content.Substring(0, ContentMaxLength);

            return _message.AddMessage(model);
        }

        public List<ApiMessageModel> GetFollowMessage(Guid userID, int page)
        {
            var dboList = _message.GetUserMessage(userID, new byte[] { 5, 6 }, page);
            List<Guid> SenderIDs = dboList.FindAll(a => a.senderID != Guid.Empty).Select(a => a.senderID).ToList();
            List<UserInfo> userInfos = _account.ListUserInfo(SenderIDs);

            List<Guid> EIDs = dboList.FindAll(a => a.EID != null).Select(a => a.EID.Value).ToList();
            var schoolList = GetSchoolInfos(EIDs);

            List<ApiMessageModel> list = new List<ApiMessageModel>();
            foreach (var msg in dboList)
            {
                var user = userInfos.Find(a => a.Id == msg.senderID);
                var school = schoolList.Find(a => a.SchoolSectionId == msg.EID);
                list.Add(new ApiMessageModel()
                {
                    Id = msg.Id,
                    SenderID = msg.senderID,
                    Nickname = user == null ? string.Empty : user.NickName,
                    HeadImgUrl = user == null ? string.Empty : user.HeadImgUrl,
                    Content = msg.Content,
                    DataID = msg.DataID,
                    Type = msg.Type,
                    Time = CommonHelper.GetTimeString(msg.time),
                    SchoolModel = school
                });
            }
            return list;
        }


        private List<SchoolInfoDto> GetSchoolInfos(List<Guid> ids)
        {
            var schools = _schoolInfoService.GetSchoolSectionByIds(ids);
            if (schools == null || !schools.Any())
                return new List<SchoolInfoDto>();
            return schools.ToList();
        }


        public List<ApiMessageModel> GetPrivateMessage(Guid userID, int page, int size = 10, List<byte> messageType = default, bool isAuth = false, bool? read = null, bool? handled = false)
        {
            byte[] type = messageType.ToArray();

            var dboList = _message.GetUserMessage(userID, type, page, size, isAuth, read, handled) ?? new List<Message>();
            List<Guid> SenderIDs = dboList.FindAll(a => a.senderID != Guid.Empty).Select(a => a.senderID).ToList();
            List<UserInfo> userInfos = _account.ListUserInfo(SenderIDs) ?? new List<UserInfo>();

            List<Guid> EIDs = dboList.FindAll(a => a.EID != null).Select(a => a.EID.Value).ToList();
            List<SchoolInfoDto> schoolList = GetSchoolInfos(EIDs);

            List<ApiMessageModel> list = new List<ApiMessageModel>();
            foreach (var msg in dboList)
            {
                var user = userInfos.Find(a => a.Id == msg.senderID);
                var school = schoolList.Find(a => a.SchoolSectionId == msg.EID);

                string nickname = msg.IsAnony ? "匿名用户" : user == null ? string.Empty : user.NickName;
                string headimgurl = msg.IsAnony ? "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" : user == null ? string.Empty : user.HeadImgUrl;
                list.Add(new ApiMessageModel()
                {
                    Id = msg.Id,
                    SenderID = msg.senderID,
                    Nickname = nickname,
                    HeadImgUrl = headimgurl,
                    Content = msg.Content,
                    DataID = msg.DataID,
                    Type = msg.Type,
                    Time = CommonHelper.GetTimeString(msg.time),
                    DataType = msg.DataType,
                    SchoolModel = school
                });
            }
            return list;
        }

        public Push GetPushSetting(Guid userID)
        {
            return _message.GetPushSetting(userID);
        }

        public List<MessageModel> GetSystemMessage(Guid userID, int page)
        {
            var dboList = _message.GetUserMessage(userID, new byte[] { (byte)MessageType.Notice }, page);
            List<MessageModel> list = new List<MessageModel>();
            foreach (var msg in dboList)
            {
                list.Add(new MessageModel()
                {
                    Id = msg.Id,
                    SenderID = msg.senderID,
                    Nickname = "通知",
                    Content = msg.Content,
                    DataID = msg.DataID,
                    Type = msg.Type,
                    Time = CommonHelper.GetTimeString(msg.time),
                });
            }
            return list;
        }

        public List<Message> GetUserMessage(Guid userID, byte[] type, int page, int pageSize = 10)
        {
            return _message.GetUserMessage(userID, type, page, pageSize);
        }

        public bool RemoveMessage(Guid msgID, Guid userID)
        {
            return _message.RemoveMessage(msgID, userID);
        }

        public bool SetPushSetting(Push model)
        {
            return _message.SetPushSetting(model);
        }

        public List<InviteSelf> GetMessageBySenderUser(Guid userId, List<int> type, int page, int size)
        {
            return _message.GetMessageBySenderUser(userId, type, page, size);
        }

        public List<InviteUserInfo> GetInviteUserInfos(List<Guid> dataId, Guid senderId)
        {
            return _message.GetInviteUserInfos(dataId, senderId);
        }

        public bool UpdateMessageHandled(bool handled, MessageType type, MessageDataType dataType, Guid userId = default, Guid senderId = default, List<Guid> dataId = null)
        {
            //bool handled, bool read, MessageType type, MessageDataType dataType, Guid? userId, Guid? senderId, List<Guid> dataId
            bool read = handled;
            Guid? _userId = userId;
            Guid? _senderId = senderId;

            if (userId == Guid.Empty)
                _userId = null;

            if (_senderId == Guid.Empty)
                _senderId = null;

            Task.Run(async () =>
            {
                await AddMyInviteRedisData(type, dataType, _userId, _senderId, dataId);
            });

            return _message.UpdateMessageHandled(handled, read, type, dataType, _userId, _senderId, dataId);
        }

        /// <summary>
        /// 判断我的邀请是否有更新
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<bool> CheckMyInviteRedisData(Guid userId, Guid dataId, MessageType type)
        {
            var hashKey = _inviteReplyRedisKey;
            var key = GetMyInviteKey(userId, dataId, type);
            return await _easyRedisClient.HashGetAsync<int>(hashKey, key) > 0;
        }

        /// <summary>
        /// 我的邀请 添加有更新的数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataType"></param>
        /// <param name="userId"></param>
        /// <param name="senderId"></param>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public async Task<bool> AddMyInviteRedisData(MessageType type, MessageDataType dataType, Guid? userId, Guid? senderId, List<Guid> dataIds = null)
        {
            //限定时间
            //DateTime startTime = DateTime.Now.AddDays(30);
            var msgs = _message.GetMessages(senderId, userId, dataIds.ToArray(), new MessageType[] { type }, new MessageDataType[] { dataType }, null, ignore: null, startTime: null, endTime: null, null, null);

            var dic = msgs.GroupBy(s => GetMyInviteKey(s.senderID, s.DataID, s.Type), t => 1)
                .ToDictionary(s => s.Key, g => g.Count());

            var hashKey = _inviteReplyRedisKey;
            await _easyRedisClient.HashSetAsync(hashKey, dic);
            return true;
        }

        private string GetMyInviteKey(Guid userId, Guid dataId, MessageType type)
        {
            return GetMyInviteKey(userId, dataId, (byte)type);
        }
        private string GetMyInviteKey(Guid userId, Guid dataId, byte type)
        {
            return userId.ToString() + dataId.ToString() + (int)type;
        }

        /// <summary>
        /// 我的邀请 清除有更新的数据(已读)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<bool> ClearMyInviteRedisData(Guid userId, Guid dataId, MessageType type)
        {
            var hashKey = _inviteReplyRedisKey;
            var key = GetMyInviteKey(userId, dataId, type);
            return await _easyRedisClient.HashDeleteAsync(hashKey, key);
        }

        public List<InviteSelf> GetMessageBySenderUser(Guid userId, int page, int size)
        {
            return _message.GetMessageBySenderUser(userId, page, size);
        }

        public List<DataMsg> GetDataMsgs(Guid userId, MessageType? type, MessageDataType? dataType, bool? ignore, int page, int size)
        {
            return _message.GetDataMsgs(userId, type, dataType, ignore, page, size);
        }

        public List<MessageTipsTotal> TotalTips(Guid UserId)
        {
            return _message.TotalTips(UserId, null);
        }

        public void UpdateMessageState(List<int> type, Guid userId)
        {
            _message.UpdateMessageState(type, userId);
        }

        public void IgnoreInvite(List<Guid> ids, Guid userId, bool isAll)
        {
            _message.IgnoreInvite(ids, userId, isAll);
        }

        public List<DataMsg> GetReplyME(Guid UserID, int page, int size)
        {
            return _message.GetReplyME(UserID, page, size);
        }

        /// <summary>
        /// 我的点赞列表
        /// </summary>
        /// <param name="searchType">0 点评/点评回复  1问答</param>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<DataMsg> GetMyLike(int searchType, Guid userId, int page, int size)
        {
            return _message.GetMyLike(searchType, userId, page, size);
        }

        public List<DataMsg> GetMyReply(int type, Guid userId, int page, int size)
        {
            return _message.GetMyReply(type, userId, page, size);
        }

        public List<Message> GetMessageJob(DateTime startTime, DateTime endTime, int page, int size)
            => _message.GetMessageJob(startTime, endTime, page, size);

        public int GetMessageJobTotal(DateTime startTime, DateTime endTime)
            => _message.GetMessageJobTotal(startTime, endTime);


        public long GetMessageTotal(Guid? senderId, Guid? userId, MessageType? type, MessageDataType? dataType, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime)
        {
            MessageType[] types = type != null ? new MessageType[] { type.Value } : null;
            MessageDataType[] dataTypes = dataType != null ? new MessageDataType[] { dataType.Value } : null;
            return _message.GetMessageTotal(senderId, userId, types, dataTypes, read, ignore, startTime, endTime, pushStartTime: null, pushEndTime: null);
        }

        public long GetMessageTotal(Guid? senderId, Guid? userId, MessageType[] types, MessageDataType[] dataTypes, bool? read, bool? ignore, DateTime? startTime, DateTime? endTime, DateTime? pushStartTime, DateTime? pushEndTime)
        {
            return _message.GetMessageTotal(senderId, userId, types, dataTypes, read, ignore, startTime, endTime, pushStartTime, pushEndTime);
        }

        public List<Guid> GetMessageUserIds(Guid senderId, Guid[] userIds, Guid dataId, MessageType type, MessageDataType dataType, Guid? eId)
        {
            return _message.GetMessageUserIds(senderId, userIds, dataId, type, dataType, eId).ToList();
        }

    }
}
