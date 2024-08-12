using PMS.CommentsManage.Application.Services.Settlement;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISettlementAmountMoneyService
    {

        SettlementAmountMoney GetNewSettlement(Guid AdminId, DateTime write);

        int BeginFirstAdd(PartTimeJobAdmin partTimeJob);
        int Add(SettlementAmountMoney enetity);
        int Update(SettlementAmountMoney enetity);
        void Settlement();
        int NextSettlementData(PartTimeJobAdmin partTimeJob,int PartJobRole);
        List<PartTimeJobDto> PartTimeJobDtos();
        List<JobSettlementMoney> settlementMoney(List<Guid> AdminIds);
        List<SettlementStatusModel> GetSettlementStatuses();
    }
}
