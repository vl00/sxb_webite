using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.Search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IUniversityService
    {
        Task<University> Get(int id);

        /// <summary>
        /// 获取高校分页列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityId"></param>
        /// <param name="type"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<SearchUniversity>> GetPagination(string keyword, int? cityId, int? type, int pageIndex, int pageSize);

        /// <summary>
        /// 获取高校推荐列表
        /// </summary>
        /// <param name="exculdeCityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<SearchUniversity>> GetRecommends(int exculdeCityId, int pageIndex, int pageSize);
    }
}
