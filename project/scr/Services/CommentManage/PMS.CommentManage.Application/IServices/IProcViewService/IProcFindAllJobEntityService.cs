using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices.IProcViewService
{
    /// <summary>
    /// 兼职人员信息管理
    /// </summary>
    public interface IProcFindAllJobEntityService
    {
        /// <summary>
        /// 执行存储过程得到兼职人员相关数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        List<ProcFindAllJobEntityList> FindAllJobEntityByLeaderId(Guid Id, int PageIndex, int PageSize,string Phone, DateTime beginTime, DateTime endTime, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal);
    }
}
