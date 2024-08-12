using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class SchoolOverViewService : ApplicationService<SchoolOverViewInfo>, ISchoolOverViewService
    {
        ISchoolOverViewRepository _schoolOverViewInfoRepository;
        public SchoolOverViewService(ISchoolOverViewRepository schoolOverViewInfoRepository) : base(schoolOverViewInfoRepository)
        {
            _schoolOverViewInfoRepository = schoolOverViewInfoRepository;
        }

        public async Task<bool> DeleteIfExisted(Guid eid)
        {
            if (eid == Guid.Empty) return false;
            return await _schoolOverViewInfoRepository.DeleteIFExist(eid);
        }

        public async Task<SchoolOverViewInfo> GetByEID(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _schoolOverViewInfoRepository.GetByAsync("[EID] = @eid", new { eid });
            if (finds?.Any() == true) return finds.First();
            return null;
        }

        public async Task<bool> ModifySchoolCertifications(Guid eid, string certifications)
        {
            if (eid == Guid.Empty || string.IsNullOrWhiteSpace(certifications)) return false;
            var entity = await GetByEID(eid);
            if (entity == null || entity.ID == Guid.Empty)
            {
                entity = new SchoolOverViewInfo()
                {
                    ID = Guid.NewGuid(),
                    EID = eid,
                    Certifications = certifications
                };
                return await AddAsync(entity);
            }
            else
            {
                entity.Certifications = certifications;
                return await UpdateAsync(entity);
            }
        }
    }
}
