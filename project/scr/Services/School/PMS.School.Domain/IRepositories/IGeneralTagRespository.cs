using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRespository
{
    public interface IGeneralTagRespository
    {
        /// <summary>
        /// ����ID��ȡ��ǩ
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, string>> GetByIDs(IEnumerable<Guid> ids);


        /// <summary>
        ///����dataId��ȡ��ǩ
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, string>>> GetByDataId(Guid dataId);
        /// <summary>
        /// �������ƻ�ȡ��ǩ
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>

        Task<Dictionary<Guid, string>> GetByNames(IEnumerable<string> names);
    }
}