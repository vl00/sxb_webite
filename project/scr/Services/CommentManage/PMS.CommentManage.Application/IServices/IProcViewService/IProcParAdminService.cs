using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices.IProcViewService
{
    /// <summary>
    /// 账号管理扩展接口
    /// </summary>
    public interface IProcParAdminService
    {
        /// <summary>
        /// 获取指定类型下的子集元素总数
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="subset"></param>
        /// <returns></returns>
        List<ProcGetAdminByRoleTypeEntity> ExecGetAdminByRoleType(Guid parentId, int type, int PageIndex, int PageSize, DateTime beginTime, DateTime endTime, out int TotalNumber);
    }
}
