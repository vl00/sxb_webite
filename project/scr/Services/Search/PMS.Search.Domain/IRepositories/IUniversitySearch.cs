using PMS.Search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Search.Domain.IRepositories
{
    public interface IUniversitySearch
    {
        Task<List<SearchUniversity>> SearchUniversitys(string keyword, int? cityId, int? exculdeCityId, int? type, int pageIndex, int pageSize);
    }
}
