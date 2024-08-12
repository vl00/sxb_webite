using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories
{
    /// <summary>
    /// 根据角色类型获取该子集下的所有实体（存储过程：proc_GetAdminByRoleType 参数：@AdminByRoleTypeId(父级类型)，@OutRoleType(子级类型)）
    /// </summary>
    public interface IParAdminServiceRepository
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
