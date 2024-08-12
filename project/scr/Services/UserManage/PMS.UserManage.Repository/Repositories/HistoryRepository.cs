using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.UserManage.Domain.IRepositories;
using Org.BouncyCastle.Asn1.Crmf;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Repository.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly UserDbContext _dbcontext;

        public HistoryRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public bool AddHistory(Guid userID, Guid dataID, byte dataType)
        {
            return _dbcontext.Execute(@"
merge into history
using (select 1 as o) t
	on history.dataID=@dataID and history.userID=@userID
when not matched then insert 
	(dataID, dataType, userID, time, IsDelete, ViewCount) values (@dataID, @dataType, @userID, sysdatetime(), 0, 1)
when matched then update set 
	time=sysdatetime(),
	IsDelete=0,
	ViewCount=history.ViewCount+1
;", new { dataID, dataType, userID }) > 0;
        }

		[Obsolete]
        public bool RemoveHistory(Guid userID, Guid dataID)
        {
            //return _dbcontext.Execute(@"delete from history where dataID=@dataID and userID=@userID", new { dataID, userID }) > 0;
			throw new NotImplementedException();
        }
        public List<Guid> GetUserHistory(Guid userID, byte dataType, int page = 1, int pageSize = 10)
        {
            return _dbcontext.Query<Guid>(@"select dataID from history where 
userID=@userID and dataType=@dataType and (isdelete = 0 or isdelete is null)
order by time desc
offset (@page-1)*@pageSize rows fetch next @pageSize row only", new { userID, dataType, page, pageSize }).ToList();
        }

        public bool ChangeHistoryState(List<Guid> Ids, Guid UserId)
        {
            string sql = "update history set isdelete = 1 where userID = @UserId and dataID in @Ids";
            return _dbcontext.Execute(sql, new { UserId, Ids }) >= Ids.Count();
        }

        public bool ClearAllHistory(Guid userId, int type)
        {
            string sql = "update history set isdelete = 1 where (isdelete = 0 or isdelete is null) and userID = @UserId and dataType=@type;";
            return _dbcontext.Execute(sql, new { userId, type }) > 0;
        }

        /// <summary>
        /// 获取浏览过同类型学校的用户
        /// </summary>
        /// <param name="schFtype"></param>
        /// <param name="schoolExtId">排除指定学校</param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<UserInfo> GetHistoryUsers(string schFtype, Guid schoolExtId, Guid[] excludeUserIds, long offset, int size)
        {
            string sql = $@"
SELECT UI.*
FROM
	(
		SELECT
			H.UserId,
			H.Time,
			row_number() OVER(
				partition BY H.UserId 
				ORDER BY H.Time desc
			) AS rn
		FROM
			iSchoolUser.dbo.history H
		WHERE
			UserId != '00000000-0000-0000-0000-000000000000'
			AND EXISTS(
				SELECT 1 FROM  iSchoolData.dbo.SchoolExtension SE where SE.Id = H.DataId and SE.SchFtype = @schFtype
			)
			AND NOT EXISTS(
				SELECT 1 FROM  iSchoolUser.dbo.history H2 where H2.UserId = H.UserId and  H2.DataId = @schoolExtId
			)
	) AS Temp
	INNER JOIN iSchoolUser.dbo.Userinfo UI ON UI.Id = Temp.UserId
WHERE
	Temp.rn = 1
	AND Temp.UserId NOT IN @excludeUserIds
	AND NOT EXISTS(
		SELECT 1 FROM  iSchoolUser.dbo.Talent T 
		where 
			T.user_id = Temp.UserId
			AND T.isdelete = 0
		   AND T.status = 1
		   AND T.certification_status = 1
	)
ORDER BY
	Temp.Time DESC, Temp.UserId
offset @offset rows
FETCH next @size rows only
";
            return _dbcontext.Query<UserInfo>(sql, new { schFtype, schoolExtId, excludeUserIds, offset, size }).ToList();
        }

        public long GetHistoryUserTotal(Guid schoolExtId, Guid[] excludeUserIds)
        {
            string sql = $@"
SELECT count(1) as Total
FROM
	(
		SELECT
			H.UserId,
			H.Time,
			row_number() OVER(
				partition BY H.UserId 
				ORDER BY H.Time desc
			) AS rn
		FROM
			iSchoolUser.dbo.history H
		WHERE
			UserId != '00000000-0000-0000-0000-000000000000'
			AND H.DataId = @schoolExtId
	) AS Temp
WHERE
	Temp.rn = 1
	AND Temp.UserId NOT IN @excludeUserIds
	AND NOT EXISTS(
		SELECT 1 FROM  iSchoolUser.dbo.Talent T 
		where 
			T.user_id = Temp.UserId
			AND T.isdelete = 0
		   AND T.status = 1
		   AND T.certification_status = 1
	)
";
            return _dbcontext.Query<long>(sql, new { schoolExtId, excludeUserIds }).FirstOrDefault();
        }

        /// <summary>
        /// 获取浏览过同学校的用户
        /// </summary>
        /// <param name="schoolExtId"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<UserInfo> GetHistoryUsers(Guid schoolExtId, Guid[] excludeUserIds, long offset, int size)
        {
            string sql = $@"
SELECT UI.*
FROM
	(
		SELECT
			H.UserId,
			H.Time,
			row_number() OVER(
				partition BY H.UserId 
				ORDER BY H.Time desc
			) AS rn
		FROM
			iSchoolUser.dbo.history H
		WHERE
			UserId != '00000000-0000-0000-0000-000000000000'
			AND H.DataId = @schoolExtId
	) AS Temp
	INNER JOIN iSchoolUser.dbo.Userinfo UI ON UI.Id = Temp.UserId
WHERE
	Temp.rn = 1
	AND Temp.UserId NOT IN @excludeUserIds
	AND NOT EXISTS(
		SELECT 1 FROM  iSchoolUser.dbo.Talent T 
		where 
			T.user_id = Temp.UserId
			AND T.isdelete = 0
			AND T.status = 1
			AND T.certification_status = 1
	)
ORDER BY
	Temp.Time DESC, Temp.UserId
offset @offset rows
FETCH next @size rows only
";
            return _dbcontext.Query<UserInfo>(sql, new { schoolExtId, excludeUserIds, offset, size }).ToList();
        }


		public async Task<IEnumerable<Guid>> GetHistoryTopId(MessageDataType dataType, DateTime? startTime, DateTime? endTime, int size = 100)
		{
			var startTimeSql = startTime.HasValue ? "AND Time >= @startTime" : "";
			var endTimeSql = endTime.HasValue ? "AND Time < @endTime" : "";
			string sql = $@"
SELECT 
	TOP {size} 
	DataId,
	COUNT(1) AS ViewCount, 
	MAX(time) AS LastViewTime
FROM 
	iSchoolUser.dbo.History
WHERE
	(IsDelete = 0 or IsDelete is null)
	AND DataType = @dataType
	{startTimeSql}
	{endTimeSql}
GROUP BY DataId
ORDER BY ViewCount DESC, LastViewTime DESC
";
			return await _dbcontext.QueryAsync<Guid>(sql, new { dataType, startTime, endTime });
		}

		public async Task<IEnumerable<HistoryTopIdQueryDto>> GetHistoryTop(MessageDataType dataType, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize = 100)
		{
			var startTimeSql = startTime.HasValue ? "AND Time >= @startTime" : "";
			var endTimeSql = endTime.HasValue ? "AND Time < @endTime" : "";
			string sql = $@"
SELECT
	DataId,
	COUNT(1) AS ViewCount, 
	MAX(time) AS LastViewTime
FROM 
	iSchoolUser.dbo.History
WHERE
	(IsDelete = 0 or IsDelete is null)
	AND DataType = @dataType
	{startTimeSql}
	{endTimeSql}
GROUP BY DataId
ORDER BY ViewCount DESC, LastViewTime DESC
offset (@pageIndex-1)*@pageSize rows
FETCH next @pageSize rows only
";
            return await _dbcontext.QueryAsync<HistoryTopIdQueryDto>(sql, new { dataType, startTime, endTime, pageIndex, pageSize });
        }

    }
}
