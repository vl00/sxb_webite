using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ProductManagement.Infrastructure.AppService
{
    /// <summary>
    /// 查询处理底层接口
    /// </summary>
    /// <typeparam name="TqDataObject"></typeparam>
    public interface IqAppService<DataObject>
    {
        /// <summary>
        /// 根据id获取实体
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        DataObject GetModelById(Guid Id);
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IEnumerable<DataObject> GetList(Expression<Func<DataObject, bool>> where = null);
        /// <summary>
        /// 检测实体是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        bool isExists(Expression<Func<DataObject, bool>> where);
    }
}
