using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ISettlementAmountMoneyRepository:IAppService<SettlementAmountMoney>
    {
        /// <summary>
        /// 获取兼职人员正在进行的任务
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        SettlementAmountMoney GetNewSettlement(Guid AdminId,DateTime write);

        int Add(SettlementAmountMoney enetity);
        new int Update(SettlementAmountMoney enetity);
        void Settlement();

        /// <summary>
        /// 修改兼职父级的结算数据
        /// </summary>
        /// <param name="JobAdminId"></param>
        /// <param name="SelectedType">1：点评，2：问答</param>
        /// <returns></returns>
        int UpdateSettlementSelectedTotal(Guid JobAdminId, int SelectedType, DateTime WriteTime);
        /// <summary>
        /// 计算得到该管理员的下次结算日期
        /// </summary>
        /// <param name="partTimeJob"></param>
        /// <returns></returns>
        int NextSettlementData(PartTimeJobAdmin partTimeJob,int PartJobRole);
        /// <summary>
        /// 结算推送
        /// </summary>
        /// <returns></returns>
        List<PartTimeJob> GetPartTimeJob();
        /// <summary>
        /// 查询结算信息
        /// </summary>
        /// <param name="AdminIds"></param>
        /// <returns></returns>
        List<JobSettlementMoney> settlementMoney(List<Guid> AdminIds);
    }
}
