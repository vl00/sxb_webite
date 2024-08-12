using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    using Domain.Entitys;
    public interface ILocalV2Repository
    {


        /// <summary>
        /// 通过ID查询地区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        local_v2 GetById(int id);


        /// <summary>
        /// 通过父ID查询地区信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        IEnumerable<local_v2> GetByParent(int pid);
    }
}
