using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    using DTOs;
    using PMS.OperationPlateform.Domain.Entitys;

    public interface ISchoolRankRepository:IBaseRepository<SchoolRank>
    {

        IEnumerable<SchoolRank> GetRankList();



        /// <summary>
        /// 查询H5前端所需的学校榜单信息
        /// </summary>
        /// <param name="schoolIds"></param>
        /// <returns></returns>
        //IEnumerable<H5SchoolRankInfoDto> GetH5SchoolRankInfoBy(Guid[] schoolIds);

    }
}
