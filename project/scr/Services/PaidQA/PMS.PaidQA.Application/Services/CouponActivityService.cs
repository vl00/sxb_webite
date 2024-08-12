using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PMS.PaidQA.Domain.Dtos;
using ProductManagement.Framework.Cache.Redis;
using PMS.UserManage.Application.IServices;

namespace PMS.PaidQA.Application.Services
{
    public class CouponActivityService : ApplicationService<CouponActivity>, ICouponActivityService
    {
        ICouponActivityRepository _repository;
        IUserService _userService;
        IEasyRedisClient _easyRedisClient;
        public CouponActivityService(ICouponActivityRepository repository, IEasyRedisClient easyRedisClient, IUserService userService) : base(repository)
        {
            _repository = repository;
            _easyRedisClient = easyRedisClient;
            _userService = userService;
        }
        public async Task<IEnumerable<BroadcastUser>> GetBroadcastUsers()
        {
            var result = await _easyRedisClient.GetOrAddAsync("BroadcastUsers", async () =>
            {
                var users = await _userService.GetRandomUsers(100);
                Random r = new Random();
                IEnumerable<BroadcastUser> broadcastUsers = users.Select(s =>
                {
                    return new BroadcastUser()
                    {
                        NickName = s.NickName,
                        Second = r.Next(0, 60).ToString("D2"),
                        Minute = r.Next(0, 30).ToString("D2")
                    };
                });
                return broadcastUsers;

            }, TimeSpan.FromMinutes(30));
            return result;
        }

        public async Task<IEnumerable<CouponQAExample>> GetCouponQAExample(Guid activityId)
        {
            return await _repository.GetCouponQAExample(activityId);
        }

        public async  Task<CouponActivity> GetEffectActivity()
        {
           return (await _repository.GetByAsync("GETDATE() BETWEEN StartTime AND OverTime")).FirstOrDefault();
        }

        public async Task<CouponActivity> GetEffectActivity(Guid activityId)
        {
            return (await _repository.GetByAsync("GETDATE() BETWEEN StartTime AND OverTime AND Id=@Id ",new { Id = activityId })).FirstOrDefault();
        }

        public async Task<CouponSpecialTalent> GetRandomSpecialTalent(Guid activityId)
        {
            return (await _repository.GetRandomSpecialTalent(activityId));
        }



        public async Task<bool> InsertExampleQA(IEnumerable<CouponQAExample> couponQAExamples)
        {

            return await _repository.InsertExampleQA(couponQAExamples);
        }

        public async Task<bool> InsertSpecialTalent(IEnumerable<CouponSpecialTalent> couponSpecialTalents)
        {
            return await _repository.InsertSpecialTalent(couponSpecialTalents);
        }
    }
}
