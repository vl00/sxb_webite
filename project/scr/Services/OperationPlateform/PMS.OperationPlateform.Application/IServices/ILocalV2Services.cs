using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
   public interface ILocalV2Services
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
