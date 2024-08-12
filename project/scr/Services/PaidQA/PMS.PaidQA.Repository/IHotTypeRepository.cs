using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
namespace PMS.PaidQA.Repository
{
    public interface IHotTypeRepository : IRepository<HotType>
    {
        Task<int> Count(string str_Where, object param);
        Task<IEnumerable<Guid>> GetAllOrderIDs();
        /// <summary>
        /// 根据分类ID获取
        /// </summary>
        /// <param name="hotTypeID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortType">排序
        /// <para>1.根据问答评分高到低</para>
        /// <para>2.根据浏览数</para>
        /// <para>3.根据Order.CreateTime</para>
        /// </param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetOrderIDByHotTypeID(Guid hotTypeID, int pageIndex = 1, int pageSize = 10, int sortType = 1);

        Task<IEnumerable<(Guid, int)>> GetViewCountByOrderIDs(IEnumerable<Guid> ids);
    }
}
