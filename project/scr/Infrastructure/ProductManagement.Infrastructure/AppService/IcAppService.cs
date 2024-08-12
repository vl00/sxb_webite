using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.AppService
{
    /// <summary>
    /// 命令处理底层接口
    /// </summary>
    /// <typeparam name="TcDataObject"></typeparam>
    public interface IcAppService<DataObject>
    {
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="model"></param>
        int Insert(DataObject model);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="model"></param>
        int Update(DataObject model);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="Id"></param>
        int Delete(Guid Id);
    }
}
