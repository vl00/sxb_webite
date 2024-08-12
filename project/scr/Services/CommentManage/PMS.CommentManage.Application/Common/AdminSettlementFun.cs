using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Common
{
    /// <summary>
    /// 结算计算
    /// </summary>
    public static class AdminSettlementFun
    {
        //审核通过单价
        private static int UnitPrice = 3;

        //【审核通过 | 失败】每满推送短信
        private static int ConditionTotal = 5;

        /// <summary>
        /// 兼职领队结算
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns></returns>
        public static float JobAdminComputeBalance(SettlementView settlement)
        {
            if ((AdminUserRole)settlement.Role == AdminUserRole.JobLeader)
            {
                return (settlement.TotalAnswerSelected + settlement.TotalSchoolCommentsSelected) * 2;
            }
            else
            {
                int CompleteTotal = settlement.TotalAnswerSelected + settlement.TotalSchoolCommentsSelected;
                if(CompleteTotal >= 5 && CompleteTotal < 10)
                {
                    //if(CompleteTotal > 5)
                    //{
                    //    //最多只能结算5条金额
                    //    return 5 * 7;
                    //}
                    //else
                    //{
                    return 5 * UnitPrice;
                    //}
                }
                else if (CompleteTotal >= 10) 
                {
                    return 10 * UnitPrice;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 是否进行短信推送
        /// </summary>
        /// <param name="Total"></param>
        /// <returns></returns>
        public static bool IsPushShortMessage(int Total) 
        {
            return Total % ConditionTotal == 0;
        }

    }
}
