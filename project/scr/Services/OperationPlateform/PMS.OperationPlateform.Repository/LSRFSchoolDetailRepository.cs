using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dapper;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.OperationPlateform.Repository
{
    public class LSRFSchoolDetailRepository : ILSRFSchoolDetailRepository
    {

        protected OperationCommandDBContext db;

        public LSRFSchoolDetailRepository(OperationCommandDBContext dBContext)
        {
            this.db = dBContext;
        }

        /// <summary>
        /// 获取除广告位之外的学校数据
        /// </summary>
        /// <param name="courseType"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<LSRFSchoolDetail> GetLSRFSchools(CourseType courseType,List<Guid> SchIds,int PageNo = 1,int PageSize = 6)
        {
            string querysql = "";
            if (SchIds.Any()) 
            {
                querysql += " and l.SchId not in @SchIds ";
            }

            string sql = @"
                             SELECT
	                            e.Id AS Eid,
	                            e.Sid,
	                            s.name + ' - ' + e.name AS Sname,
	                            c.abroad,
	                            c.authentication,
                                c.lodging,c.sdextern,
	                            s.intro,
	                            cou.courses,
	                            (
	                                SELECT
		                                top 1
		                                sub.content 
	                                FROM
		                                [iSchoolData].[dbo].[OnlineSchoolYearFieldContent] AS sub 
	                                WHERE
		                                sub.eid = l.SchId 
		                                AND sub.field = 'Subjects' 
	                                ORDER BY
		                                [YEAR] DESC
	                                ) AS Subjects,
	                            (
	                                SELECT TOP
		                                1 sub.content 
	                                FROM
		                                [iSchoolData].[dbo].[OnlineSchoolYearFieldContent] AS sub 
	                                WHERE
		                                sub.field = 'Count' 
		                                AND sub.eid = l.SchId 
	                                ORDER BY
		                                [YEAR] DESC
	                                ) AS [Count] ,
                                life.hardware,
	                            l.type
                            FROM
	                            LSRFSchools AS l
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON e.id = l.SchId
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchool] AS s ON s.id = e.sid
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtContent] AS c ON c.eid = l.SchId 
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtCourse] AS cou ON cou.eid = l.SchId
                                LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtLife] AS life ON life.eid = l.SchId
                            WHERE
	                            l.IsDel = 0  and l.Type = 1 and l.CourseType = @courseType " + querysql + @"
                            ORDER BY
	                            (CASE
		                            WHEN l.type = 3 THEN
		                            1 
		                            WHEN l.type = 1 THEN
		                            2 
		                            WHEN l.type = 2 THEN
		                            3 
	                            END ) ASC,
	                            Sort ASC
                       ";

            if (PageNo > 1)
            {
                sql += " OFFSET @PageNo ROWS FETCH NEXT @PageSize ROWS ONLY ";
            }
            else
            {
                sql += " OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY ";
            }

            return db.Query<LSRFSchoolDetail>(sql, new { courseType = (int)courseType , PageNo, PageSize, SchIds }).ToList();
        }

        public KeyValue GetCurrentCourseTotal(CourseType courseType,List<Guid> SchIds) 
        {
            string querysql = "";

            if (SchIds.Any()) 
            {
                querysql += " l.SchId not in @SchIds ";
            }

            string sql = @"
                        SELECT
	                        count(l.Id) as [Value]
                        FROM
	                        LSRFSchools AS l
                        WHERE
	                        l.IsDel = 0  AND l.Type = 1 and l.CourseType =  @courseType
            ";

            return db.Query<KeyValue>(sql, new { courseType, SchIds }).FirstOrDefault();
        }

        /// <summary>
        /// 获取该类型的广告位数据
        /// </summary>
        /// <param name="courseType"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<LSRFSchoolDetail> GetAdvertisementSchool(CourseType courseType, LSRFSchoolType DataType ,List<Guid> SchIds,int PageNo = 1, int PageSize = 6) 
        {

            string querysql = "";

            if (SchIds.Any()) 
            {
                querysql += " and l.SchId not in @SchIds";
            }
            //if (DataType == LSRFSchoolType.Advertise) 
            //{
            //    takeSql = " top 1 ";
            //}

            string sql = $@"
                            SELECT
	                            e.Id AS Eid,
	                            e.Sid,
	                            s.name + ' - ' + e.name AS Sname,
                                l.AdvPicUrl,
								l.AdvType,
	                            c.abroad,
                                c.lodging,c.sdextern,
	                            c.authentication,
	                            s.intro,
	                            cou.courses,
	                            (
	                                SELECT
		                                top 1
		                                sub.content 
	                                FROM
		                                [iSchoolData].[dbo].[OnlineSchoolYearFieldContent] AS sub 
	                                WHERE
		                                sub.eid = l.SchId 
		                                AND sub.field = 'Subjects' 
	                                ORDER BY
		                                [YEAR] DESC
	                                ) AS Subjects,
	                            (
	                                SELECT TOP
		                                1 sub.content 
	                                FROM
		                                [iSchoolData].[dbo].[OnlineSchoolYearFieldContent] AS sub 
	                                WHERE
		                                sub.field = 'Count' 
		                                AND sub.eid = l.SchId 
	                                ORDER BY
		                                [YEAR] DESC
	                                ) AS [Count] ,
                                life.hardware,
	                            l.type 
                            FROM
	                            LSRFSchools AS l
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON e.id = l.SchId
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchool] AS s ON s.id = e.sid
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtContent] AS c ON c.eid = l.SchId
	                            LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtCourse] AS cou ON cou.eid = l.SchId
                                LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtLife] AS life ON life.eid = l.SchId
                            WHERE
	                            l.IsDel = 0 and l.Type = @DataType and l.CourseType = @CourseType " + querysql + @"
                            ORDER BY 
	                            Sort ASC
                            OFFSET (@PageNo - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY 
                    ";

                return db.Query<LSRFSchoolDetail>(sql, new { DataType, courseType = (int)courseType, PageNo, PageSize,SchIds }).ToList();
        }

        public List<SingSchoolRank> GetSchoolRankByEid(List<Guid> SchIds) 
        {
            string sql = @"
                        select * from (
                        SELECT
	                        l.SchId,
	                        ROW_NUMBER() over(ORDER BY ( CASE WHEN l.type = 3 THEN 1 WHEN l.type = 1 THEN 2 WHEN l.type = 2 THEN 3 END ) ASC,Sort ASC) as [Rank] 
                        FROM
	                        LSRFSchools AS l
                        WHERE
	                        l.IsDel = 0 and l.type = 1 and l.coursetype = 0
                        ) as b where b.[SchId] in @SchIds";

            return db.Query<SingSchoolRank>(sql, new { SchIds }).ToList();
        }

        public KeyValue GetCourseTypeAdveTotal(CourseType courseType) 
        {
            string sql = @" select count(1) as [Value] from LSRFSchools where CourseType = @courseType and type = 3 ";
            return db.Query<KeyValue>(sql, new { courseType }).FirstOrDefault();
        }

        public bool CheckSchIsLeaving(Guid SchId) 
        {
            string sql = "select count(1) as value from LSRFSchools where SchId = @SchId";
            var temp = db.Query<KeyValue>(sql, new { SchId }).FirstOrDefault().Value;
            int value = temp == null || temp == "" ? 0 : int.Parse(temp);

            return value > 0;
        }

        public List<LSRFSchoolDetail> SchDistinct(int courseType) 
        {
            string sql = @"select SchId as Eid,MAX(Type) as Type from LSRFSchools
	                        WHERE CourseType = @courseType
                        GROUP BY SchId  HAVING count(SchId) > 1";

            return db.Query<LSRFSchoolDetail>(sql,new { courseType })?.ToList();
        }


    }
}
