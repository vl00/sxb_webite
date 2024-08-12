using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 管理员操作
    /// </summary>
    public interface IPartTimeJobAdminRepository : IAppService<PartTimeJobAdmin>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="phone">账号</param>
        /// <param name="pwd">第一次默认密码为父级初始码</param>
        /// <returns></returns>
        PartTimeJobAdmin Login(string phone, string pwd);
        /// <summary>
        /// 检测手机号是否存在
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        bool CheckPhoneExists(string phone);
        /// <summary>
        /// 检测邀请码是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        bool CheckCodeExists(string code);
        /// <summary>
        /// 更改账号状态
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool CheckStatus(Guid Id);
        /// <summary>
        /// 获取所有的供应商
        /// </summary>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetAllSupplierList();
        /// <summary>
        /// 获取供应商下所有的兼职领队
        /// </summary>
        /// <param name="SupplierId"></param>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetCurrentAllItem(Guid SupplierId);
        /// <summary>
        /// 账号管理分页查询
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetPartTimeJobAdminPage(int Role, int PageIndex, int PageSize, out int total);
        /// <summary>
        /// 检测原始密码是否正确
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Pwd"></param>
        /// <returns></returns>
        bool CheckOldPassword(Guid Id, string Pwd);
        /// <summary>
        /// 检测是否已修改过密码
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool CheckResetPassword(Guid Id);
        /// <summary>
        /// 获取最顶级父级
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        int GetSupplierSettlementType(Guid AdminId);
        /// <summary>
        /// 根据code获取兼职领队
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        PartTimeJobAdmin GetJobAdminByCode(string code);
        /// <summary>
        /// 获取顶级父级信息
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        PartTimeJobAdmin GetTopParent(Guid AdminId,int role);
        /// <summary>
        /// 根据用户角色获取用户信息
        /// </summary>
        /// <param name="role"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetPartTimeJobAdminByRole(int role,string phone, int pageIndex, int pageSize, out int total);
        /// <summary>
        /// 根据id获取名称
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetJobAdminNameByIds(List<Guid> Ids);
        /// <summary>
        /// 根据用户id 用户角色 获取用户信息
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="Role"></param>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetPartTimeJobAdminByIdRoles(List<Guid> Ids, int Role);
        /// <summary>
        /// 根据电话号码获取管理员信息
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        List<PartTimeJobAdmin> GetAdminIdByPhone(string phone);
    }
}
