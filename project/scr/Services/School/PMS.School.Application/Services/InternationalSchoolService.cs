using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.SEO_International;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class InternationalSchoolService : IInternationalSchoolService
    {
        IInternationalSchoolRepository _internationalSchoolRepository;
        public InternationalSchoolService(IInternationalSchoolRepository internationalSchoolRepository)
        {
            _internationalSchoolRepository = internationalSchoolRepository;
        }

        public async Task<IEnumerable<InternationalSchoolInfo>> GetSchoolByPageID(Guid pageID)
        {
            if (pageID == Guid.Empty) return null;
            return await _internationalSchoolRepository.GetSchools(pageID);
        }

        public async Task<InternationalPage> GetSelectedReading(int id)
        {
            if (id < 1) return null;
            return await _internationalSchoolRepository.GetSelectedReading(id);
        }
    }
}
