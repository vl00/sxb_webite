using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IGeneralTagService
    {
        /// <summary>
        /// 根据ID获取标签
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, string>> GetByIDs(IEnumerable<Guid> ids);

        /// <summary>
        ///根据dataId获取标签
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, string>>> GetByDataId(Guid dataId);
        /// <summary>
        /// 根据名称获取标签
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>

        Task<Dictionary<Guid, string>> GetByNames(IEnumerable<string> names);

        Task<Guid> GetTagIDByNameForArticle(string name);
    }
}
