using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.School.Domain.IRepositories;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    /// <summary>
    /// 学校留资活动
    /// </summary>
    public class UniversityService : IUniversityService
    {

        private readonly IUniversityRepository _universityRepository;
        private readonly IUniversitySearch _universitySearch;

        public UniversityService(IUniversityRepository universityRepository, IUniversitySearch universitySearch)
        {
            _universityRepository = universityRepository;
            _universitySearch = universitySearch;
        }

        public async Task<University> Get(int id)
        {
            var university = await _universityRepository.GetById(id);
            if (university.IsDeleted)
            {
                return null;
            }
            return university;
        }


        /// <summary>
        /// 获取高校分页列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityId"></param>
        /// <param name="type"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<SearchUniversity>> GetPagination(string keyword, int? cityId, int? type, int pageIndex, int pageSize)
        {
            return await _universitySearch.SearchUniversitys(
                keyword,
                cityId,
                exculdeCityId: null,
                type,
                pageIndex,
                pageSize);
        }

        /// <summary>
        /// 获取高校推荐列表
        /// </summary>
        /// <param name="exculdeCityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<SearchUniversity>> GetRecommends(int exculdeCityId, int pageIndex, int pageSize)
        {
            return await _universitySearch.SearchUniversitys(
                keyword: string.Empty,
                cityId: null,
                exculdeCityId,
                type: null,
                pageIndex,
                pageSize);
        }
    }
}
