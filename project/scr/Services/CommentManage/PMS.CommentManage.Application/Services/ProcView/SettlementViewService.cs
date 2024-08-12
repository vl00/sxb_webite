using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using PMS.CommentsManage.Application.Common;

namespace PMS.CommentsManage.Application.Services.ProcView
{
    public class SettlementViewService : ISettlementViewService
    {
        ISettlementViewRepository _settlementViewRepository;
        public SettlementViewService(ISettlementViewRepository settlementViewRepository)
        {
            _settlementViewRepository = settlementViewRepository;
        }

        public List<SettlementAmountMoney> GetSettlementViewByIds(List<Guid> Ids)
        {
            if (!Ids.Any()) 
            {
                return new List<SettlementAmountMoney>();
            }
            return _settlementViewRepository.GetSettlementViewByIds(Ids);
        }

        public List<SettlementView> GetSettlementViews(Guid ParentId, int Status, int PageIndex, int PageSize, int TotalSearch, DateTime QueryTime, out int Total)
        {
            List<SettlementView> settlement = _settlementViewRepository.GetSettlementViews(ParentId, Status, PageIndex, PageSize,TotalSearch, QueryTime, out Total);
            if(settlement == null)
            {
                return null;
            }
            else
            {
                settlement.ForEach(x=>{
                    x.SettlementAmount = AdminSettlementFun.JobAdminComputeBalance(x);
                });
                return settlement;
            }
        }

        public int UpdateStatus(Guid AdmindId, string Ids, int Stauts)
        {
            return _settlementViewRepository.UpdateStatus(AdmindId,Ids, Stauts);
        }
    }
}
