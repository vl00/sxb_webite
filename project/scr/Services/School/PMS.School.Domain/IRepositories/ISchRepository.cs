using iSchool;
using PMS.School.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface ISchRepository
    {
        SchExtDto0 GetSchextSimpleInfo(Guid eid);

        SchExtDto0[] GetNearSchoolsBySchType((double Lng, double Lat) location, SchFType0[] schFType, int count = 8);
        Task<IEnumerable<SchExtDto0>> GetNearSchoolsByEID(Guid eid, int count = 8);

        (Guid, string)[] GetLocalColleges();

        /// <summary>
        /// 获取学部总分数
        /// </summary>
        SchoolExtScore[] GetSchoolsTotalScores(Guid[] eids);
        Guid GetSchoolextID(string shortSchoolNo);

        (Guid, int)[] GetSchoolextNo(Guid[] eids);
    }
}
