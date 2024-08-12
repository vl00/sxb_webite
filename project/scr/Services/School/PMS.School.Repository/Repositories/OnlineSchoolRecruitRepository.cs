using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class OnlineSchoolRecruitRepository : Repository<OnlineSchoolRecruitInfo, ISchoolDataDBContext>, IOnlineSchoolRecruitRepository
    {
        ISchoolDataDBContext _db;
        public OnlineSchoolRecruitRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteIFExist(Guid eid)
        {
            var str_SQL = "Select Count(1) From [OnlineSchoolRecruitInfo] Where [EID] = @eid;";
            var count = await _db.QuerySingleAsync<int>(str_SQL, new { eid });
            if (count > 0)
            {
                str_SQL = "Delete From [OnlineSchoolRecruitInfo] Where [EID] = @eid;";
                return (await _db.ExecuteAsync(str_SQL, new { eid })) > 0;
            }
            return false;
        }

        public async Task<bool> DeleteRecruitSchedules(string cityCode, string areaCode, string schFType, int recruitType)
        {
            var str_SQL = "Delete From [RecruitScheduleInfo] Where [CityCode] = @cityCode AND [SchFType] = @schFType AND [RecruitType] = @recruitType";
            if (string.IsNullOrWhiteSpace(areaCode) || areaCode == "0")
            {
                str_SQL += " AND [AreaCode] is null";
            }
            else
            {
                str_SQL += " AND [AreaCode] = @areaCode";
            }

            return (await _db.ExecuteAsync(str_SQL, new { cityCode, areaCode, schFType, recruitType }) > 0);
        }

        public async Task<int> GetRecentYear(Guid eid)
        {
            var str_SQL = @"SELECT
                             ISNULL(MAX([year]), 0)
                            FROM
	                            OnlineSchoolRecruitInfo 
                            WHERE
	                            EID = @eid;";
            return await _db.QuerySingleAsync<int>(str_SQL, new { eid });
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedule(int cityCode, IEnumerable<int> recruitTypes, string schFType, int? areaCode = null)
        {
            var str_SQL = "Select * From [RecruitScheduleInfo] Where [CityCode] = @cityCode AND [RecruitType] in @recruitTypes AND [SchFType] = @schFType";
            if (areaCode.HasValue && areaCode.Value > 0)
            {
                str_SQL += " AND [AreaCode] = @areaCode";
            }
            else
            {
                str_SQL += " AND [AreaCode] is null";
            }
            return await _db.QueryAsync<RecruitScheduleInfo>(str_SQL, new { cityCode, recruitTypes, schFType, areaCode });
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitScheduleByRecruitIDs(IEnumerable<Guid> recruitIDs)
        {
            var str_SQL = $"Select * From [RecruitScheduleInfo] Where [RecruitID] in @recruitIDs;";
            return await _db.QueryAsync<RecruitScheduleInfo>(str_SQL, new { recruitIDs });
        }

        public async Task<bool> InsertRecruitSchedule(RecruitScheduleInfo entity)
        {
            return (await _db.InsertAsync(entity)) > 0;
        }
    }
}
