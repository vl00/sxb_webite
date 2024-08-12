using PMS.School.Domain.Entities.SEO_International;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IInternationalSchoolService
    {
        Task<InternationalPage> GetSelectedReading(int id);
        Task<IEnumerable<InternationalSchoolInfo>> GetSchoolByPageID(Guid pageID);
    }
}
