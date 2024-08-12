using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    public interface IBaseQueryRepository<T> where T : class
    {
        /// <summary>
        /// 取筛选结果集的第一条返回
        /// </summary>
        /// <param name="where"></param>
        /// <param name="param"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        T TakeFirst(string where, object param, string order = null);

        (IEnumerable<T>, int total) SelectPage(
            string where,
            object param = null,
            string order = null,
            string[] fileds = null,
            bool isPage = false,
            int offset = 0,
            int limit = 20
            );

        IEnumerable<T> Select(
              string where,
              object param = null,
              string order = null,
              string[] fileds = null);
    }
}