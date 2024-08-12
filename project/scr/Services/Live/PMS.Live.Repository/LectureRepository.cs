using PMS.Live.Domain.Dtos;
using PMS.Live.Domain.Entities;
using PMS.Live.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Live.Repository
{
    public class LectureRepository : ILectureRepository
    {
        LiveDBContext _DB;
        public LectureRepository(LiveDBContext dBContext)
        {
            _DB = dBContext;
        }

        public Task<IEnumerable<LectureInfo>> GetByDate(DateTime beginTime, DateTime endTime)
        {
            var str_SQL = $@"SELECT
	                            id,
	                            userCount
                            FROM
	                            lecture_v2 
                            WHERE
	                            createtime BETWEEN @beginTime 
	                            AND @endTime
	                            AND status IN ( 2, 3, 4, 5 ) 
                                AND show = 1
                            ORDER BY
	                            createtime DESC";
            return _DB.QueryAsync<LectureInfo>(str_SQL, new { beginTime, endTime });
        }

        public async Task<IEnumerable<LectureInfo>> GetByIDs(IEnumerable<Guid> ids)
        {
            var str_SQL = $@"SELECT
	                            * 
                            FROM
	                            lecture_v2 
                            WHERE
                                show = 1
	                            AND id IN @IDs";
            return await _DB.QueryAsync<LectureInfo>(str_SQL, new { ids });
        }

        public async Task<LectureInfo> GetByID(Guid id)
        {
            var str_SQL = $@"SELECT
	                            * 
                            FROM
	                            lecture_v2 
                            WHERE
	                            id = @ID";
            return await _DB.QuerySingleAsync<LectureInfo>(str_SQL, new { id });
        }

        public async Task<(LectureInfo, Guid)> GetByIDWithLectorUserID(Guid id)
        {
            var str_SQL = $@"SELECT
	                            * 
                            FROM
	                            lecture_v2 
                            WHERE
	                            id = @ID
                                AND show = 1";
            var lecture = await _DB.QuerySingleAsync<LectureInfo>(str_SQL, new { id });
            var lectorUserID = Guid.Empty;
            if (lecture != null)
            {
                lectorUserID = _DB.ExecuteScalar<Guid>("Select UserID From lector where id = @Lector_id", new { lecture.Lector_id });
            }
            return (lecture, lectorUserID);
        }

        public async Task<IEnumerable<LectorLiveStatusDto>> GetLectorLiveStatus(IEnumerable<Guid> userIDs)
        {
            var str_SQL = $@"SELECT
	                            l.ID AS LectureID,
	                            l.lector_id AS LectorID,
	                            l.status ,
	                            u.nickname ,
                                u.ID as userID
                            FROM
	                            lecture_v2 AS l
	                            LEFT JOIN lector AS lt ON lt.id = l.lector_id
	                            LEFT JOIN iSchoolUser.dbo.userInfo AS u ON u.id = lt.userID
	                        WHERE	
                                u.id in  @userIDs
                                AND l.status = 4
                                AND l.show = 1
                                AND l.type != 3";
            return await _DB.QueryAsync<LectorLiveStatusDto>(str_SQL, new { userIDs });
        }

        public async Task<IEnumerable<LectureStatusDto>> GetLectureStatus(IEnumerable<Guid> ids)
        {
            var str_SQL = $@"SELECT
	                            ID,
	                            status 
                            FROM
	                            lecture_v2
	                            WHERE Id IN @ids";
            return await _DB.QueryAsync<LectureStatusDto>(str_SQL, new { ids });
        }

        public Task<IEnumerable<LectureInfo>> GetByDate(DateTime beginTime, DateTime endTime, int count = 0)
        {
            if (count < 1) count = 10;
            var str_SQL = $@"SELECT TOP {count}
	                            *
                            FROM
	                            lecture_v2 
                            WHERE
	                            createtime BETWEEN @beginTime 
	                            AND @endTime
	                            AND status IN ( 2, 3, 4, 5 ) 
                                AND Type != 3
                                AND Show = 1
                            ORDER BY
	                            createtime DESC";
            return _DB.QueryAsync<LectureInfo>(str_SQL, new { beginTime, endTime });
        }
    }
}
