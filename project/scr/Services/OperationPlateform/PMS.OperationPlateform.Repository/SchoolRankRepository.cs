using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.IRespositories;
    using Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class SchoolRankRepository : BaseRepository<SchoolRank>, ISchoolRankRepository
    {
        public SchoolRankRepository(OperationDBContext db) : base(db)
        {
        }

        public IEnumerable<SchoolRank> GetRankList()
        {
            return _db.GetAll<SchoolRank>();
        }

        //public IEnumerable<SchoolRank> GetH5SchoolRankInfoBy(Guid[] schoolIds)
        //{
        //    string sql = @"
        //        SELECT SchoolRank.*,SchoolRankBinds.*  FROM SchoolRankBinds
        //        JOIN SchoolRank ON SchoolRankBinds.RankId = SchoolRank.Id
        //        WHERE 
        //        SchoolRank.IsShow = 1 
        //        AND SchoolId in @schoolIds";
        //    this.Select(
        //        where:

        //        )



        //    IEnumerable<SchoolRank> schoolRanks =     this.db.Query<SchoolRank, SchoolRankBinds, SchoolRank>(sql, (sr, sb) =>
        //    {
        //        //H5SchoolRankInfoDto h5SchoolRankInfoDto = new H5SchoolRankInfoDto()
        //        //{
        //        //    RankId = sr.Id,
        //        //    SchoolId = sb.SchoolId,
        //        //    Title = sr.Title,
        //        //    Ranking = (int)sr.Rank,
        //        //    Cover = sr.Cover,

        //        //};
        //        //return h5SchoolRankInfoDto;
        //        sr.SchoolRankBinds = sb;
        //        return sr;

        //    }, new { schoolIds });


        //    return schoolRanks;


        //}
    }
}
