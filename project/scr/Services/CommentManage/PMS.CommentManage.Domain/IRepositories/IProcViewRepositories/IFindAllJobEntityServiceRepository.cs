using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories
{
    /// <summary>
    /// 兼职数据概况
    /// </summary>
    public interface IFindAllJobEntityServiceRepository
    {
        List<ProcFindAllJobEntityList> FindAllJobEntityByLeaderId(Guid Id, int PageIndex, int PageSize,string phone, DateTime beginTime, DateTime endTime, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal);
    }
}
