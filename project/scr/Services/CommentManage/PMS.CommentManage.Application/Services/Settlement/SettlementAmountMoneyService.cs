using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;

namespace PMS.CommentsManage.Application.Services.Settlement
{
    public class SettlementAmountMoneyService : ISettlementAmountMoneyService
    {
        ISettlementAmountMoneyRepository _settlementAmountMoney;
        ISettlementViewRepository _settlementView;
        public SettlementAmountMoneyService(ISettlementAmountMoneyRepository settlementAmountMoney, ISettlementViewRepository settlementView)
        {
            _settlementAmountMoney = settlementAmountMoney;
            _settlementView = settlementView;
        }

        public SettlementAmountMoney GetNewSettlement(Guid AdminId, DateTime write) 
        {
            return _settlementAmountMoney.GetNewSettlement(AdminId,write);
        }

        public int Add(SettlementAmountMoney enetity)
        {
            return _settlementAmountMoney.Add(enetity);
        }

        public int BeginFirstAdd(PartTimeJobAdmin partTimeJob)
        {
            SettlementAmountMoney enetity = new SettlementAmountMoney();
            enetity.Id = Guid.NewGuid();
            enetity.PartTimeJobAdminId = partTimeJob.Id;
            enetity.SettlementAmount = 0;
            enetity.SettlementStatus = Domain.Common.SettlementStatus.Ongoing;
            enetity.TotalAnswerSelected = 0;
            enetity.TotalSchoolCommentsSelected = 0;

            //兼职领队结算第一周期开始
            if (partTimeJob.Role == Domain.Common.AdminUserRole.JobLeader)
            {
                
            }
            //兼职
            else if (partTimeJob.Role == Domain.Common.AdminUserRole.JobMember)
            {

            }
            return _settlementAmountMoney.Add(enetity);
        }

        public int NextSettlementData(PartTimeJobAdmin partTimeJob,int PartJobRole)
        {
            return _settlementAmountMoney.NextSettlementData(partTimeJob, PartJobRole);
        }

        public void Settlement()
        {
            _settlementAmountMoney.Settlement();
        }

        public int Update(SettlementAmountMoney enetity)
        {
            return _settlementAmountMoney.Update(enetity);
        }

        public List<PartTimeJobDto> PartTimeJobDtos() 
        {
            return _settlementAmountMoney.GetPartTimeJob().Select(x => new PartTimeJobDto() {
                    JobId = x.JobId,
                    UserId = x.UserId,
                    OpenId = x.OpenId,
                    SettlementAmount = (x.TotalSchoolCommentsSelected + x.TotalAnswerSelected) >= 10 ? 10 * 3 : (x.TotalSchoolCommentsSelected + x.TotalAnswerSelected) >= 5 ? (5 * 3) : 0
            })?.ToList();
        }

        public List<JobSettlementMoney> settlementMoney(List<Guid> AdminIds)
        {
            return _settlementAmountMoney.settlementMoney(AdminIds);
        }

        public List<SettlementStatusModel> GetSettlementStatuses()
        {
            return _settlementView.GetSettlementStatuses();
        }
    }
}
