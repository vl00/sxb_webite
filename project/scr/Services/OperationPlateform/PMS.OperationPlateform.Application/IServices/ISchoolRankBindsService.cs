using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface ISchoolRankBindsService
    {

        /// <summary>
        /// 查询某个榜单下的列表项
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns></returns>
        IEnumerable<SchoolRankBinds> GetSchoolRankBinds(SchoolRank rank);

        /// <summary>
        /// 查询某个榜单下的列表项(分页)
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        (IEnumerable<SchoolRankBinds>, int total) GetSchoolRankBinds_PageVersion(SchoolRank rank, int offset = 0, int limit = 30);
    }
}
