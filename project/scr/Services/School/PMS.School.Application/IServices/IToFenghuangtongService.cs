using iSchool;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IToFenghuangtongService
    {
        /// <summary>通过学校ID查询学校名称+学部名称+经纬度坐标</summary>
        /// <param name="eid">学部id</param>
        /// <param name="eno">学部no</param>
        Task<SchExtDto1> GetSchoolExtLatLongInfo(Guid eid = default, long eno = default);
        
    }
}
