using PMS.PaidQA.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface INotificationService
    {

        /// <summary>
        /// 通知用户评论
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiUserEvalute(Guid orderId);

        /// <summary>
        /// 通知用户已经创建了提问
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiUserCreateQuestion(Guid orderId);


        /// <summary>
        /// 通知转单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiTransting(Guid orderId);


        /// <summary>
        /// 通知用户订单超时
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiUserOrderTimeOut(Guid orderId);

        /// <summary>
        /// 通知达人订单超时
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiTalentOrderTimeOut(Guid orderId);

        /// <summary>
        /// 通知用户订单已被拒绝
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiUserRefusOrder(Guid orderId);

        /// <summary>
        /// 通知用户订单退款处理
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task NotifiUserRefundAmount(Guid orderId);

        Task NotifiUserAsk(Guid orderId);
        /// <summary>
        /// 通知用户优惠券过期。
        /// </summary>
        /// <returns></returns>
        Task NotifiExpireCoupon(Guid takeId);

        Task NotifiReply(Guid orderId);


    }
}
