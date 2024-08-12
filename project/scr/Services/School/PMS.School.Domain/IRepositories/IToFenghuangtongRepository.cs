using iSchool;
using PMS.School.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IToFenghuangtongRepository
    {        
        Guid GetSchoolEid(long no);

        Task<SchExtDto1> GetSchExtDto1(Guid eid);
    }
}
