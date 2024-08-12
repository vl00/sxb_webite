using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories
{
    public interface ISettlementViewRepository
    {
        List<SettlementAmountMoney> GetSettlementViewByIds(List<Guid> Ids);
        List<SettlementView> GetSettlementViews(Guid ParentId, int Status, int PageIndex,int PageSize, int TotalSearch, DateTime QueryTime,out int Total);
        int UpdateStatus(Guid AdmindId, string Ids, int Stauts);
        List<SettlementStatusModel> GetSettlementStatuses();
    }
}
