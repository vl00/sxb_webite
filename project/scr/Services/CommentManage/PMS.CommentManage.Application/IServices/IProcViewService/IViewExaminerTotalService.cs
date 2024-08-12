using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices.IProcViewService
{
    /// <summary>
    /// 总审核人员统计
    /// </summary>
    public interface IViewExaminerTotalService
    {
        /// <summary>
        /// 获取审核人员统计
        /// </summary>
        /// <returns></returns>
        ViewExaminerTotal GetViewExaminerTotal();
        /// <summary>
        /// /供应商页数据统计
        /// </summary>
        /// <param name="SupplierId"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        SupplierTotalDto GetSupplierTotal(Guid SupplierId, DateTime BeginTime, DateTime EndTime);
        /// <summary>
        /// /兼职领队页数据统计
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        SupplierTotalDto GetLeaderCurrentMonthTotal(Guid ParentId, DateTime BeginTime, DateTime EndTime, string Phone);
        /// <summary>
        /// 管理员查看供应商统计数据
        /// </summary>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        SysAdminQuerySupplierTotalDto SysAdminQuerySupplierTotal(DateTime BeginTime, DateTime EndTime);
    }
}
