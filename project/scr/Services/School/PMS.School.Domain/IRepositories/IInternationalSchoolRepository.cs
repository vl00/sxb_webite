using PMS.School.Domain.Entities.SEO_International;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IInternationalSchoolRepository
    {

        Task<InternationalPage> GetSelectedReading(int id);
        Task<IEnumerable<InternationalSchoolInfo>> GetSchools(Guid pageID);
    }
}
