using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.API.Http.Result.FinanceCenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    /// <summary>
    /// 支付中心服务客户端
    /// </summary>
    public interface IFinanceCenterServiceClient
    {

        /// <summary>
        /// 新增支付订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayCenterResponseResult<WechatJsPayParam>> AddPayOrder(AddPayOrderRequest request);


        Task<PayCenterResponseResult<RefundResult>> Refund(RefundRequest request);
        /// <summary>
        /// 给达人添加收益
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayCenterResponseResult<WalletResult>> Wallet(WalletRequest request);

        /// <summary>
        /// 检查订单的支付状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayCenterResponseResult<CheckPayStatusResult>> CheckPayStatus(CheckPayStatusRequest request);
    }
}
