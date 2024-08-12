using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.Services
{
    public class SysMessageService : ISysMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ISysMessageRepository _sysmessageRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        private readonly string _NewSysMessageRedisKey = "SysMsgDate:UserId:{0}";

        public SysMessageService(ISysMessageRepository sysmessageRepository, IEasyRedisClient easyRedisClient, IMessageRepository messageRepository)
        {
            _sysmessageRepository = sysmessageRepository;
            _easyRedisClient = easyRedisClient;
            _messageRepository = messageRepository;
        }

        public bool AddSysMessage(List<SysMessage> message)
            => _sysmessageRepository.AddSysMessage(message);

        public async Task<int> AddSysMessageState(List<SysMessageState> messageStates)
            => await _sysmessageRepository.AddSysMessageState(messageStates);

        public List<Guid> CheckMessageIdIsExist(List<Guid> DataIds, Guid UserId)
            => _sysmessageRepository.CheckMessageIdIsExist(DataIds, UserId);

        public List<DynamicItem> GetDynamicItems(Guid userId, int page, int size, bool IsSelf = true)
            => _sysmessageRepository.GetDynamicItems(userId, page, size, IsSelf);

        public List<DynamicItem> GetDynamicItems(List<Guid> userIds, int page, int size, bool IsSelf = true)
            => _sysmessageRepository.GetDynamicItems(userIds, page, size, IsSelf);

        public List<SysMessageTips> GetLiveMessageTips(Guid userId, int page, int size)
            => _sysmessageRepository.GetLiveMessageTips(userId, page, size);

        public List<SysMessage> GetMessageByType(Guid SenderUserId, int Type, int Page, int Size)
            => _sysmessageRepository.GetMessageByType(SenderUserId, Type, Page, Size);

        public List<SysMessageTips> GetSysMessageTips(Guid userId, int page)
        {
            var data = _sysmessageRepository.GetSysMessageTips(userId, page);
            return data;
        }

        public List<SysMessageDetail> GetSysMessages(Guid? senderId, Guid? userId, SysMessageType[] types, bool? isRead, DateTime? startTime, DateTime? endTime, int page = 1)
        {
            return _sysmessageRepository.GetSysMessages(senderId, userId, types, isRead, startTime, endTime, page);
        }

        public List<SysMessageDetail> GetSysMessages(Guid? senderId, Guid? userId, int page = 1)
        {
            senderId = senderId == Guid.Empty ? null : senderId;
            userId = userId == Guid.Empty ? null : userId;
            return _sysmessageRepository.GetSysMessages(senderId, userId, null, null, null, null, page);
        }

        public Task<int> UpdateMessageRead(List<Guid> dataIds, Guid UserId)
            => _sysmessageRepository.UpdateMessageRead(dataIds, UserId);

        /// <summary>
        /// 获取是否有未读的消息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> HasSysMessage(Guid userId)
        {
            var key = string.Format(_NewSysMessageRedisKey, userId);

            //上次查询时间
            long lastTime = await _easyRedisClient.GetAsync<long>(key);
            DateTime? startTime = null; // DateTime.Now.AddDays(-30)
            if (lastTime > 0)
                startTime = new DateTime(lastTime);

            //从上次查询时间开始, 新消息数量
            var total = _sysmessageRepository.GetSysMessageTotal(null, userId, null, false, startTime, null);
            var messageTotal = _messageRepository.TotalTips(userId, startTime).Count;

            return total + messageTotal > 0;
        }

        /// <summary>
        /// 更新上次查询消息时间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> RefreshSysMessageTime(Guid userId)
        {
            var key = string.Format(_NewSysMessageRedisKey, userId);
            long lastTime = DateTime.Now.Ticks;
            return await _easyRedisClient.AddAsync(key, lastTime, TimeSpan.FromDays(7));
        }
    }
}
