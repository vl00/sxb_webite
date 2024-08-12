using MediatR;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{
    public class AssessStatisticsHandler : INotificationHandler<AssessStatisticsEvent>
    {
        IEasyRedisClient _easyRedisClient;
        IAssessStatisticsService _assessStatisticsService;
        string _formatRedisKey = "AssessStatistics:{0}:AssessType_{1}";

        public AssessStatisticsHandler(IEasyRedisClient easyRedisClient, IAssessStatisticsService assessStatisticsService)
        {
            _assessStatisticsService = assessStatisticsService;
            _easyRedisClient = easyRedisClient;
        }



        public async Task Handle(AssessStatisticsEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var date = DateTime.Now.Date;
                var redisBaseKey = string.Format(_formatRedisKey, date.ToString("yyyy-MM-dd"), (int)notification.AssessType);

                var find = await _easyRedisClient.HashGetAsync<AssessStatisticsInfo>(redisBaseKey, "Entity");
                if (find == null || find.ID == Guid.Empty)
                {
                    find = await _assessStatisticsService.Get(notification.AssessType, date);
                    if (find == null || find.ID == Guid.Empty)
                    {
                        find = new AssessStatisticsInfo()
                        {
                            AssessType = notification.AssessType,
                            CountTime = date,
                            ID = Guid.NewGuid()
                        };
                        _assessStatisticsService.Add(find);
                    }
                }

                var userIDExist = true;
                if (notification.UserID.HasValue && notification.UserID.Value != Guid.Empty)
                {
                    userIDExist = await _easyRedisClient.HashExistsAsync($"{redisBaseKey}:StatisticsType_{notification.StatisticsType}", notification.UserID.ToString());
                    if (!userIDExist)
                    {
                        await _easyRedisClient.HashSetAsync($"{redisBaseKey}:StatisticsType_{notification.StatisticsType}", notification.UserID.ToString(), 1, TimeSpan.FromDays(2));
                    }
                }



                switch (notification.StatisticsType)
                {
                    case 1:
                        find.HomeIndexPV++;
                        if (!userIDExist) find.HomeIndexUV++;
                        break;
                    case 2:
                        find.FinishPV++;
                        if (!userIDExist) find.FinishUV++;
                        break;
                    case 3:
                        if (!userIDExist) find.FollowCount++;
                        break;
                }
                //await _easyRedisClient.HashDeleteAsync($"{redisBaseKey}", "Entity");

                await _easyRedisClient.HashSetAsync($"{redisBaseKey}", "Entity", find, TimeSpan.FromDays(2));

                _assessStatisticsService.Update(find);
            }
            catch (Exception)
            {
                //_logger.LogError(ex.Message + ex.StackTrace);
            }
        }
    }
}
