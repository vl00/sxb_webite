using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices.IProcViewService
{
    public interface ISettlementViewService
    {
        List<SettlementAmountMoney> GetSettlementViewByIds(List<Guid> Ids);
        List<SettlementView> GetSettlementViews(Guid ParentId, int Status, int PageIndex, int PageSize, int TotalSearch, DateTime QueryTime, out int Total);
        int UpdateStatus(Guid AdmindId, string Ids, int Stauts);
    }
}
