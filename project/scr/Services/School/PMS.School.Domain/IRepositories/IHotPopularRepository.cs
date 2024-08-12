using iSchool;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IHotPopularRepository
    {
        // 热门学校
        SimpleHotSchoolDto[] GetHotVisitSchools(int citycode, SchFType0? schtype, int count = 6);

        // 周边学校
        SmpNearestSchoolDto[] GetNearestSchools(Guid eid, int count = 6);
        // 周边学校
        SmpNearestSchoolDto[] GetNearestSchools(int citycode, (double Lng, double Lat) lnglat, SchFType0[] schtypes, int count = 6);

        // 首页热门推荐
        (Guid Sid, Guid Eid)[] GetHotVisitSchextIds(int citycode, byte? grade, byte? type, int day = 7, int count = 6);
    }
}
