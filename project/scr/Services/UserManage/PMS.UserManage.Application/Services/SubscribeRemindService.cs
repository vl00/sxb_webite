using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;

namespace PMS.UserManage.Application.Services
{


    public class SubscribeRemindService : ISubscribeRemindService
    {
        private readonly ILogger<SubscribeRemindService> _logger;
        private readonly ISubscribeRemindRepository _subscribeRemindRepository;

        public SubscribeRemindService(ISubscribeRemindRepository subscribeRemindRepository, ILogger<SubscribeRemindService> logger)
        {
            _subscribeRemindRepository = subscribeRemindRepository;
            _logger = logger;
        }

        public bool Add(SubscribeRemindAddDto dto)
        {
            var startTime = dto.StartTime != null ? dto.StartTime.Value : DateTime.Now;
            var endTime = dto.EndTime != null ? dto.EndTime.Value : DateTime.Now.AddYears(1000);

            bool exists = Exists(dto.GroupCode, dto.SubjectId, dto.UserId);
            if (exists)
            {
                _logger.LogError("已添加, dto={0}", JsonConvert.SerializeObject(dto));
                return false;
            }

            var subscribeRemind = new SubscribeRemind()
            {
                Id = Guid.NewGuid(),
                GroupCode = dto.GroupCode,
                UserId = dto.UserId,
                SubjectId = dto.SubjectId,
                IsEnable = true,
                IsValid = true,
                StartTime = startTime,
                EndTime = endTime,
                CreateTime = DateTime.Now,
            };
            return _subscribeRemindRepository.Add(subscribeRemind);
        }

        public bool Exists(string groupCode, Guid subjectId, Guid userId)
        {
            return _subscribeRemindRepository.Exists(groupCode, subjectId, userId);
        }

        public IEnumerable<SubscribeRemind> GetPagination(string groupCode, int pageIndex = 1, int pageSize = 10)
        {
            return _subscribeRemindRepository.GetPagination(groupCode, pageIndex, pageSize);
        }
    }
}
