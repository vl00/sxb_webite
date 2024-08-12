using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    using Domain.DTOs;
    using PMS.OperationPlateform.Application.Dtos;
    using PMS.OperationPlateform.Domain.Entitys;
    using System.Threading.Tasks;

    public interface ISchoolRankService
    {
        IEnumerable<SchoolRank> GetAll();

        /// <summary>
        /// 查询H5前端所需的学校榜单信息
        /// </summary>
        /// <param name="schoolIds"></param>
        /// <returns></returns>
        IEnumerable<H5SchoolRankInfoDto> GetH5SchoolRankInfoBy(Guid[] schoolIds);


        /// <summary>
        /// 获取单个榜单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SchoolRank GetSchoolRank(Guid? id, long? no = null);

        IEnumerable<SchoolRank> GetSchoolRanks(IEnumerable<Guid> ids);
        /// <summary>
        /// 查询某个学部绑定的所有榜单及榜单旗下与当前学部相相邻学部
        /// </summary>
        /// <param name="schoolId">学部ID</param>
        /// <param name="takeAdjacentCount">指示取相邻的多少项</param>
        /// <returns></returns>
        IEnumerable<SchoolRank> GetSchoolRanks(Guid schoolId, int takeAdjacentCount);


        /// <summary>
        /// 获取推荐榜单
        /// </summary>
        /// <param name="schoolTypeId"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<SchoolRank> GetRecommendSchoolRanks(List<Article_SchoolTypes> schoolTypes, int cityID, int count);

        IEnumerable<SchoolRank> GetRecommendSchoolRanksSchool(List<Article_SchoolTypes> schoolTypes, int cityID, int count);

        (IEnumerable<SchoolRank> schoolRanks, int total) GetSchoolRanksByGrades(int cityId, int? gradeId, int offset = 0, int limit = 20);

        IEnumerable<H5SchoolRankInfoDto> GetRankInfoBySchID(Guid schoolId);


        /// <summary>
        /// 获取首页封面三大新榜
        /// </summary>
        /// <param name="city"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>

        (IEnumerable<SchoolRank> schoolranks,int total) GetTheNationwideAndNewsRanks(int city, int offset = 0, int limit = 3);

    }
}
