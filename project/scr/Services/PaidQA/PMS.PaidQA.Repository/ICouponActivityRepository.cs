using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public interface ICouponActivityRepository:IRepository<CouponActivity>
    {
        Task<IEnumerable<CouponQAExample>> GetCouponQAExample(Guid activityId);

         Task<bool> InsertExampleQA(IEnumerable<CouponQAExample> couponQAExamples);
         Task<bool> InsertSpecialTalent(IEnumerable<CouponSpecialTalent> couponSpecialTalents);

        /// <summary>
        /// 生成随机达人
        /// </summary>
        /// <returns></returns>
        Task<CouponSpecialTalent> GetRandomSpecialTalent(Guid activityId);
    }
}
