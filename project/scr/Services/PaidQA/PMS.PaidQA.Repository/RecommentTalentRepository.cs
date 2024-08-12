using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class RecommentTalentRepository : Repository<RecommentTalent, PaidQADBContext>, IRecommentTalentRepository
    {
        private PaidQADBContext _paidQADBContext;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dBContext"></param>
        public RecommentTalentRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }

        /// <summary>
        /// 达人列表，按用户关注达人领域查询，按粉丝排序
        /// </summary>
        /// <returns></returns>
        public async Task<List<TalentExtend>> GetTalentList(string userId)
        {
            var str_SQL = $@"SELECT us.nickname AS NickName,us.id as TalentId,us.HeadImgUrl, tlt.Type as [TalentType]
                            ,(SELECT COUNT(1) FROM iSchoolUser.dbo.collection AS coll WHERE coll.dataType IN(4,5) AND coll.dataID = us.id) AS FansCount
                            FROM TalentRegion AS talRn
                            LEFT JOIN TalentSetting AS talset ON talset.TalentUserID = talRn.UserID
                            LEFT JOIN iSchoolUser.dbo.userinfo AS us ON us.id = talset.TalentUserID
                            LEFT JOIN iSchoolUser.dbo.talent AS tlt ON tlt.user_id = us.id
                            WHERE talRn.RegionTypeID IN(
                            SELECT talRn.RegionTypeID FROM TalentRegion AS talRn WHERE UserID IN(
                            SELECT coll.dataID FROM iSchoolUser.dbo.collection AS coll 
                            WHERE coll.dataType IN (4,5) AND coll.UserID = @userId)GROUP BY talRn.RegionTypeID)
                            AND tlt.isdelete = 0 AND certification_status = 1 AND status = 1 AND talset.IsEnable = 1                          
                            GROUP BY us.nickname,us.id,us.HeadImgUrl,tlt.Type  
                            ORDER BY FansCount DESC";
            var data = await _paidQADBContext.QueryAsync<TalentExtend>(str_SQL, new { userId= userId });
            return data.ToList();
        }

        /// <summary>
        /// 上学问达人列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<TalentExtend>> GetTalentList()
        {
            var sql = @"SELECT us.nickname AS NickName,us.id as TalentId,us.HeadImgUrl, tlt.Type as [TalentType],
                        (SELECT rnType.Name FROM RegionType AS rnType WHERE rnType.ID = (
                        SELECT TOP 1 talRn.RegionTypeID FROM TalentRegion AS talRn WHERE talRn.UserID = us.id) ) AS Tag
                        FROM RecommentTalent AS rtal
                        LEFT JOIN iSchoolUser.dbo.userinfo AS us ON us.id = rtal.UserID
						LEFT JOIN iSchoolUser.dbo.talent AS tlt ON tlt.user_id = us.id
						WHERE tlt.isdelete = 0 AND certification_status = 1 AND status = 1
						ORDER BY rtal.Sort ASC";
            var data = await _paidQADBContext.QueryAsync<TalentExtend>(sql, new { });
            return data.ToList();
        }
    }
}
