using PMS.PaidQA.Domain.Dtos;
using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface ICouponActivityService:IApplicationService<CouponActivity>
    {
        Task<bool> InsertExampleQA(IEnumerable<CouponQAExample> couponQAExamples);
        Task<bool> InsertSpecialTalent(IEnumerable<CouponSpecialTalent> couponSpecialTalents);
        Task<IEnumerable<BroadcastUser>> GetBroadcastUsers();
        Task<CouponActivity> GetEffectActivity();
        Task<CouponActivity> GetEffectActivity(Guid activityId);

        Task<IEnumerable<CouponQAExample>> GetCouponQAExample(Guid activityId);

        Task<CouponSpecialTalent> GetRandomSpecialTalent(Guid activityId);
    }
}
