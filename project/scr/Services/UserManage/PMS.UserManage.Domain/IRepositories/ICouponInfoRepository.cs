using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ICouponInfoRepository
    {
        Task<CouponInfo> Get(Guid id);

        Task<bool> Add(CouponInfo couponInfo);


        /// <summary>
        /// 获取优惠券的规则
        /// </summary>
        /// <param name="couponId"></param>
        /// <param name="direct"></param>
        /// <returns></returns>
        Task<IEnumerable<CouponRule>> GetCouponRule(Guid couponId,bool direct);

    }
}
