using PMS.OperationPlateform.Domain;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class UgcLogRepository : BaseRepository<UgcLog>, IUgcLogRepository
    {
        public UgcLogRepository(OperationDBContext db) : base(db)
        {
        }

        public bool Update(UgcLog ugcLog)
        {
            var sql = $@" update UgcLog set TriggerTime = getdate(), TriggerTimes = TriggerTimes + 1 where Id = @Id ";
            return _db.ExecuteUow(sql, ugcLog) > 0;
        }

        public bool UpdateUserAreaByExtId(Guid extId, Guid userId, AreaType areaType)
        {
            var areaSql = areaType == AreaType.City ? "SEC.City" : "SEC.Area";
            var sort = DateTime.Now.Ticks;
            var fromType = UgcUserAreaFromType.Feedback;
            var sql = $@" 
-- 类型 0 城市  1区
-- 来源 0 userinfo表 1 浏览直播记录  2 浏览文章记录 3. 浏览反馈
INSERT INTO iSchoolArticle.dbo.UgcUserArea
(
	Id, UserId, AreaParentId, AreaId, Sort,
	AreaType, FromType, FromId, CreateTime, IsDeleted
)
SELECT
	NEWID(), @userId, KVP.Id, KV.Id, @sort+KV.Id,
	@areaType, @fromType, SEC.eid, getdate(), 0
FROM
	iSchoolData.dbo.SchoolExtContent SEC
	INNER JOIN iSchoolData.dbo.KeyValue KV ON KV.Id = {areaSql}
	left JOIN iSchoolData.dbo.KeyValue KVP ON KVP.Id = KV.ParentId
WHERE
	1=1
	AND SEC.Eid = @extId
	AND EXISTS (
		SELECT 1 FROM iSchoolArticle.dbo.UgcUser UU
		WHERE UU.UserId = @userId
	)
	AND NOT EXISTS (
		SELECT 1 FROM iSchoolArticle.dbo.UgcUserArea UUA 
		WHERE UUA.AreaId = KV.Id AND IsDeleted = 0
	)

";
            return _db.ExecuteUow(sql, new { extId, areaType, fromType, userId, sort }) > 0;
        }
    }
}
