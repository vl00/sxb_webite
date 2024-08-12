using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.DTOs;

namespace PMS.OperationPlateform.Repository
{

    public class DataPacketRepository : BaseRepository<DataPacket>, IDataPacketRepository
    {

        public DataPacketRepository(OperationDBContext db) : base(db)
        {
        }

        public async Task<IEnumerable<DataPacket>> GetList(DataPacketStep? step, DataPacketStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            string sql = @" 
SELECT
	DP.*,
    OW.OpenId
FROM
	iSchoolArticle.dbo.DataPacket DP
	INNER JOIN ISchoolUser.dbo.openid_weixin OW ON OW.UserId = DP.UserId AND OW.AppName = 'fwh'
WHERE
	OW.Valid = 1
	AND (@step is null or DP.Step = @step)
	AND (@status is null or DP.Status = @status)
	AND DP.SubscribeTime >= @startTime
	AND DP.SubscribeTime < @endTime
ORDER BY
	DP.Id
offset (@pageIndex-1)*@pageSize rows
FETCH next @pageSize rows only 
";
            return await _db.QueryAsync<DataPacket>(sql, new { step, status, startTime, endTime, pageIndex, pageSize });
        }


        public async Task<IEnumerable<DataPacketGroupDto>> GetGroups(DateTime startTime, DateTime endTime)
        {
            string sql = @" 
SELECT
	DP.ScanPage,
	CONVERT(varchar(100), DP.ScanTime, 111) AS Date,
	-- MIN(DP.ScanTime) as ScanTime,
	SUM(DP.ScanCount) AS ScanCount,
	SUM(case when DP.Status = 2 THEN 1 ELSE 0 END) AS SubcribeCount
FROM
	iSchoolArticle.dbo.DataPacket DP
WHERE
    1=1
	AND DP.SubscribeTime >= @startTime
	AND DP.SubscribeTime < @endTime
GROUP BY 
	DP.ScanPage, CONVERT(varchar(100), DP.ScanTime, 111)
";
            return await _db.QueryAsync<DataPacketGroupDto>(sql, new { startTime, endTime });
        }

        public async Task<IEnumerable<DataPacketUserDto>> GetSubscribeUsers(DateTime startTime, DateTime endTime)
        {
            string sql = @" 
SELECT
	DP.UserId,
	DP.SubscribeTime,
	UI.NickName,
	OW.OpenId,
	OW.Valid AS IsSubcribe,
	OW.LastUnSubscribeTime
FROM
	(
		SELECT
			DP.UserId, 
			Min(DP.SubscribeTime) AS SubscribeTime
		FROM
			iSchoolArticle.dbo.DataPacket DP
		WHERE
			DP.Status = 2
			AND DP.SubscribeTime >= @startTime
			AND DP.SubscribeTime < @endTime
		GROUP BY
		   DP.UserId
	) AS DP
	INNER JOIN ISchoolUser.dbo.openid_weixin OW ON OW.UserId = DP.UserId AND OW.AppName = 'fwh'
	INNER JOIN ISchoolUser.dbo.UserInfo UI ON UI.Id = DP.UserId
";
            return await _db.QueryAsync<DataPacketUserDto>(sql, new { startTime, endTime });
        }

        public async Task<IEnumerable<string>> GetWeixinReplyMsgs(string openId, DateTime startTime, string[] contents)
        {
			var contentSql = contents == null || contents.Length == 0 ? "" : " and RM.Content in @contents ";

			string sql = $@" 
SELECT
	distinct RM.Content
FROM
	ISchool.dbo.weixin_receive_message RM 
where
	RM.OpenId = @openId
	{contentSql}
	AND RM.CreateTime >= @startTime
";
            return await _db.QueryAsync<string>(sql, new { openId, startTime, contents });
        }
    }
}
