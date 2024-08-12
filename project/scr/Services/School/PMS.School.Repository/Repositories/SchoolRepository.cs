using Dapper;
using Newtonsoft.Json.Converters;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.Enum;
using PMS.School.Domain.IRepositories;
using PMS.School.Infrastructure.Common;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly IEasyRedisClient _easyRedisClient;
        readonly ISchoolDataDBContext dbc;


        public SchoolRepository(ISchoolDataDBContext dbc, IEasyRedisClient easyRedisClient)
        {
            this.dbc = dbc;
            _easyRedisClient = easyRedisClient;
        }


        public List<SchoolExtension> GetSchoolExtensions(int PageIndex, int PageSize, string SchoolName = null)
        {
            string where = "";
            if (!string.IsNullOrWhiteSpace(SchoolName))
            {
                where += " and o.name like @schoolName ";
            }

            string sql = @"select s.sid as SchoolId, s.id as Id, (s.name) as 
             SchoolName,s.type as SchoolType,s.grade as SchoolGrade,e.lodging as Lodging,e.sdextern as Sdextern,e.latitude as Latitude,e.longitude 
             as Longitude from OnlineSchool as o inner join OnlineSchoolExtension as s on o.id = s.sid and s.IsValid = 1 left join 
             OnlineSchoolExtContent as e on s.id = e.eid where 1=1 " + where + @" and s.name is not null 
             order by s.name OFFSET (@PageIndex - 1) * @PageSize rows fetch next @PageSize rows only";

            var list = dbc.Query<SchoolExtension>(sql, new { PageIndex, PageSize, schoolName = "%{SchoolName}%" }).ToList();
            return list;
        }

        public List<SchoolExtension> GetSchoolExtensions(int pageIndex, int pageSize, int provinceCode, int cityCode, int grade, int type)
        {
            string where = "";
            if (provinceCode != 0)
            {
                where += " and c.province = @provinceCode ";
            }
            if (cityCode != 0)
            {
                where += " and c.city = @cityCode ";
            }
            if (grade != 0)
            {
                where += " and e.grade = @grade ";
            }
            if (type != 0)
            {
                where += " and e.type = @type ";
            }

            string sql = @"select s.name +'-'+e.name as SchoolName,
	            e.Id as Id,
	            e.Sid as SchoolId,
	            e.Type as SchoolType,
	            e.grade as SchoolGrade
            from 
	            OnlineSchoolExtension as e 
            left join OnlineSchool as s on e.sid = s.id
            left join OnlineSchoolExtContent as c on e.id = c.eid
            where s.IsValid = 1 and e.IsValid = 1 and s.status = 3  " + where + @" 
             order by e.id OFFSET (@pageIndex - 1) * @pageSize rows fetch next @pageSize rows only";

            var list = dbc.Query<SchoolExtension>(sql, new { pageIndex, pageSize, provinceCode, cityCode, grade, type }).ToList();
            return list;
        }
        public int GetSchoolExtensionsCount(int provinceCode, int cityCode, int grade, int type)
        {
            string where = "";
            if (provinceCode != 0)
            {
                where += " and c.province = @provinceCode ";
            }
            if (cityCode != 0)
            {
                where += " and c.city = @cityCode ";
            }
            if (grade != 0)
            {
                where += " and e.grade = @grade ";
            }
            if (type != 0)
            {
                where += " and e.type = @type ";
            }

            string sql = @"select count(1)
            from 
	            OnlineSchoolExtension as e 
            left join OnlineSchool as s on e.sid = s.id
            left join OnlineSchoolExtContent as c on e.id = c.eid
            where s.IsValid = 1 and e.IsValid = 1 and s.status = 3  " + where + @";";

            var result = dbc.Query<int>(sql, new { provinceCode, cityCode, grade, type }).FirstOrDefault();
            return result;
        }


        public List<SchoolExtension> GetCurrentSchoolAllExt(Guid sid)
        {
            string sql = @"select 
	                        e.id as Id,
	                        e.sid as SchoolId,
	                        o.name+'-'+e.name as SchoolName
                        from OnlineSchoolExtension as e
	                        left join OnlineSchool as o ON e.sid = o.id
                        where o.id = @sid and e.IsValid = 1 and o.Status = 3";

            return dbc.Query<SchoolExtension>(sql, new { sid })?.ToList();
        }

        public List<SchoolExtensionTotal> GetSchoolName(List<Guid> eid)
        {
            SqlCommand cmd = new SqlCommand(@"
select 
    e.id as Id,
    e.No as SchoolNo,
    e.sid as SchoolId,
    c.Sdextern,
    c.Lodging,
    e.Type as SchoolType,
    s.name+'-'+e.name as SchoolName,
    o.CommentCount as CommentTotal,
    o.AggScore as SchoolAvgScore
from 
    OnlineSchoolExtension as e
    left join OnlineSchool as  s on e.Sid = s.Id
    left join SchoolScores as o on o.SchoolSectionId = e.id
	left join OnlineSchoolExtContent as c on c.Eid = e.Id
where e.Id in @ExtId;");
            return dbc.Query<SchoolExtensionTotal>(cmd.CommandText, new { ExtId = eid })?.ToList();
        }

        public async Task<IEnumerable<(Guid ExtId, string SchoolName)>> GetSchoolNameOnlyAsync(IEnumerable<Guid> extIds)
        {
            if (extIds == null || !extIds.Any())
            {
                return Enumerable.Empty<(Guid, string)>();
            }

            var sql = @"
select 
    e.id as extId,
    s.name+'-'+e.name as SchoolName
from 
    OnlineSchoolExtension as e
    left join OnlineSchool as  s on e.Sid = s.Id
where e.Id in @extIds;";
            var data = await dbc.QueryAsync<(Guid, string)>(sql, new { extIds });
            return data;
        }

        public async Task<IEnumerable<SchoolNameTypeArea>> GetSchoolNameTypeAreaAsync(IEnumerable<Guid> extIds)
        {
            if (extIds == null || !extIds.Any())
            {
                return Enumerable.Empty<SchoolNameTypeArea>();
            }

            var inParamSql =  string.Join(",", extIds.Select(s => '\'' + s.ToString() + '\''));

            var sql = $@"
SELECT
    ext.id AS ExtId,
    sch.Name + '-' + ext.Name AS SchoolName, 
    ext.Type, ext.Grade, ext.Discount, ext.Diglossia, ext.Chinese,
    c.name AS CityName,
    a.name AS AreaName
FROM 
    dbo.OnlineSchoolExtension AS ext
    LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
    LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
    LEFT JOIN KeyValue AS c On content.city = c.id
    LEFT JOIN KeyValue AS a On content.area = a.id
where 1 = 1
    and ext.Id in ({inParamSql})
";
            var data = await dbc.QueryAsync<SchoolNameTypeArea>(sql, null);
            return data;
        }


        public List<SchoolInfoStatus> GetSchoolStatuse(List<Guid> eids)
        {
            SqlCommand cmd = new SqlCommand(@"
                        select e.id as Id,
                        e.sid as SchoolId,
                        s.name+'-'+e.name as SchoolName,
                        e.IsValid,
                        s.Status
                        from OnlineSchoolExtension as e
	                            left join OnlineSchool as  s on e.Sid = s.Id
                            where e.Id in @ExtId
                            ");
            return dbc.Query<SchoolInfoStatus>(cmd.CommandText, new { ExtId = eids })?.ToList();
        }


        public SchoolExtensionTotal GetSchoolInfo(Guid eid)
        {
            string sql = @"select s.name +'-'+e.name as SchoolName,
	                        e.Id as Id,
	                        e.Sid as SchoolId,
	                        e.Type as SchoolType,
	                        e.SchFtype as SchFtype,
	                        o.AggScore as SchoolAvgScore,
	                        c.lodging as Lodging,c.sdextern as Sdextern,
							e.grade as SchoolGrade,
                            e.No AS SchoolNo
                        from 
	                        OnlineSchoolExtension as e 
		                        left join [iSchoolProduct].[dbo].SchoolScores as o on o.SchoolSectionId = e.Id
		                        left join OnlineSchool as s on e.sid = s.id
		                        left join OnlineSchoolExtContent as c on c.Eid = e.Id
                        where e.Id = @eid";

            return dbc.Query<SchoolExtensionTotal>(sql, new { eid }).FirstOrDefault();
        }

        public int GetSchoolExtensionsCount(string schoolName = null)
        {
            string where = "";
            if (!string.IsNullOrWhiteSpace(schoolName))
            {
                where += " and o.name like @schoolName ";
            }

            string sql = @"SELECT
                             count(1) 
                            FROM
                                OnlineSchool AS o
                            inner JOIN OnlineSchoolExtension AS s ON o.id = s.sid 
                                AND s.IsValid = 1
                            where
                                o.name IS NOT NULL " + where + @";";

            var result = dbc.Query<int>(sql, new { schoolName = "%{schoolName}%" }).FirstOrDefault();
            return result;
        }


        public SchoolExtension GetSchoolExtensionById(Guid SchoolId, Guid BranchSchoolId)
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select s.sid as SchoolId,s.id  as Id,(o.name +' - ' +s.name) as ");
            stringBuilder.Append(" SchoolName,s.type as SchoolType,s.grade as SchoolGrade,e.lodging as Lodging,e.sdextern as Sdextern,e.latitude as Latitude,e.longitude ");
            stringBuilder.Append(" as Longitude from OnlineSchool as o right join OnlineSchoolExtension as s on o.id = s.sid  and s.IsValid = 1 left join ");
            stringBuilder.Append(" OnlineSchoolExtContent as e on s.id = e.eid where o.name is not null and s.id = @BranchSchoolId ");

            return dbc.Query<SchoolExtension>(stringBuilder.ToString(), new { SchoolId, BranchSchoolId }).FirstOrDefault();
        }

        public List<SchoolExtension> GetSchoolByIds(string Ids)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("select s.sid as SchoolId,s.id  as Id,(o.name  +' - ' + s.name) as ");
            stringBuilder.Append(" SchoolName,s.type as SchoolType,s.grade as SchoolGrade,e.lodging as Lodging,e.sdextern as Sdextern,e.latitude as Latitude,e.longitude ");
            stringBuilder.Append(" as Longitude,e.city as City ,s.No AS SchoolNo from OnlineSchool as o right join OnlineSchoolExtension as s on o.id = s.sid  and s.IsValid = 1 left join ");
            stringBuilder.Append(" OnlineSchoolExtContent as e on s.id = e.eid where s.id in @ids;");

            //SqlParameter[] para = {
            //    new SqlParameter("@ids",string.Join(",",Ids).ToList())
            //};
            List<Guid> ids = Ids.Split(',').Select(q => Guid.Parse(q)).ToList();


            var rez = dbc.Query<SchoolExtension>(stringBuilder.ToString(), new { ids });
            if (rez == null)
                return null;
            return rez.ToList();
        }



        public SchoolExtension GetSchoolExtension(Guid BranchId)
        {
            string sql = @"select row_number() over(order by s.id) as RowNumber,
            s.sid as SchoolId,s.id  as Id,(o.name + ' - ' + s.name) as  SchoolName,
            s.type as SchoolType,s.grade as SchoolGrade,e.lodging as Lodging,e.sdextern as Sdextern,e.latitude as Latitude,
            e.longitude as Longitude , s.No as SchoolNo
            from  OnlineSchoolExtension as s 
            left join OnlineSchool as o on o.id = s.sid   
            left join  OnlineSchoolExtContent as e on s.id = e.eid
            where s.IsValid = 1 and s.Id = @BranchId ";

            return dbc.Query<SchoolExtension>(sql, new { BranchId }).FirstOrDefault();
        }

        public List<SchoolBranch> GetAllSchoolBranch(Guid SchoolId)
        {
            string sql = @"select ext.id as Id,(sch.name +'-'+ ext.name) as Name,ect.City	
	                    from OnlineSchoolExtension AS ext
			                     INNER JOIN OnlineSchool AS sch ON ext.sid = sch.id
	                             INNER JOIN OnlineSchoolExtContent AS ect ON ect.eid = ext.Id   
                    where ext.sid = @SchoolId and ext.IsValid = 1 and sch.status = 3";

            return dbc.Query<SchoolBranch>(sql, new { SchoolId }).ToList();
        }

        public SchoolBranch SchoolBranchById(Guid Id)
        {
            string sql = "select s.id,s.name,s.type,e.lodging,e.sdextern from OnlineSchoolExtension as s inner join OnlineSchoolExtContent as e on s.id = e.eid where s.Id = @Id and s.IsValid = 1";

            return dbc.Query<SchoolBranch>(sql, new { Id }).FirstOrDefault();
        }

        public List<Guid> GetSchoolIdBySchoolName(string schoolName)
        {
            string sql = "select id from OnlineSchoolExtension where name like  @schoolName;";
            var Param = new { schoolName = "%{schoolName}%" };
            return dbc.Query<Guid>(sql, Param).ToList();
        }

        public List<Guid> GetSchoolAllHighSchool(Guid SchoolId, bool queryType, SchoolGrade grade = SchoolGrade.Defalut, SchoolType schoolType = SchoolType.unknown)
        {
            var para = new DynamicParameters();
            string sql = "select id from OnlineSchoolExtension where sid = @SchoolId ";

            if (queryType)
            {
                sql += " and grade = @grade";
                para.Add("@grade", (int)grade);
            }
            else
            {
                sql += " and type = @type";
                para.Add("@type", (int)schoolType);
            }

            para.Add("@SchoolId", SchoolId);

            return dbc.Query<Guid>(sql, para).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CityCode"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <param name="isLodging"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="order"></param>
        /// <param name="schoolArea"></param>
        /// <param name="queryType">true：点评，false：问题</param>
        /// <returns></returns>
        public List<SchoolExtensionTotal> AllSchoolSelected(int CityCode, List<SchoolGrade> grade, List<SchoolType> type, List<int> isLodging, int PageIndex, int PageSize, QuestionAndCommentOrder order, List<int> schoolArea = null, bool queryType = true)
        {
            var para = new DynamicParameters();
            string sql = @"SELECT
	                        o.name+ '-' + e.name AS SchoolName,
	                        o.id AS SchoolId,
	                        e.id AS Id,
	                        s.AggScore AS SchoolAvgScore,
	                        e.type AS SchoolType,
	                        c.lodging AS Lodging,c.sdextern as Sdextern,
	                        (select sum(CommentCount) from SchoolScores where SchoolId = s.SchoolId) AS SectionCommentTotal,
							(select sum(QuestionCount) from SchoolScores where SchoolId = s.SchoolId) AS SectionQuestionTotal,
                            e.No As SchoolNo
                        FROM
	                        SchoolScores as s 
	                        LEFT JOIN OnlineSchoolExtension as e on s.SchoolSectionId = e.id
	                        INNER JOIN OnlineSchoolExtContent AS c ON e.id = c.eid
	                        INNER JOIN OnlineSchool AS o ON o.id = e.sid
                            where 1 = 1 and c.city = @CityCode and e.IsValid = 1 AND o.status = 3 ";
            //学校年级类型
            if (grade.Count > 0)
            {
                para.Add("@grade", grade.Select(q => (int)q).ToList());
                sql += " AND e.grade in @grade ";
            }

            //学校类型查询
            if (type.Count > 0)
            {
                para.Add("@type", type.Select(q => (int)q).ToList());
                sql += " AND e.type in @type ";
            }

            //住宿
            if (isLodging.Count > 0)
            {
                para.Add("@lodging", isLodging.ToList());
                sql += " AND c.lodging in @lodging ";
            }

            //不带城市区域码
            if (schoolArea.Count > 0)
            {
                para.Add("@Area", schoolArea.ToList());
                sql += " AND  c.area in @Area ";
            }

            if (queryType)
            {
                sql += " and s.CommentCount <> 0 ";
                if ((int)order == -1)
                {
                    sql += "  order by s.CommentCount desc";
                }
                else if ((int)order == 1)
                {
                    sql += " order by s.LastCommentTime desc";
                }
                else if ((int)order == 3)
                {
                    sql += " order by SectionCommentTotal desc";
                }
            }
            else
            {
                sql += " and s.QuestionCount <> 0 ";
                if ((int)order == -1)
                {
                    sql += " order by s.QuestionCount desc";
                }
                else if ((int)order == 1)
                {
                    sql += " order by s.LastQuestionTime desc";
                }
                else if ((int)order == 3)
                {
                    sql += " order by SectionQuestionTotal desc";
                }
            }
            sql += " OFFSET (@PageIndex - 1) * @PageSize rows fetch next @PageSize rows only";
            para.Add("@CityCode", CityCode);
            para.Add("@PageIndex", PageIndex);
            para.Add("@PageSize", PageSize);
            return dbc.Query<SchoolExtensionTotal>(sql, para)?.ToList();
        }

        /// <summary>
        /// 获取学校分部的列表
        /// </summary>
        /// <param name="city"></param>
        /// <param name="grades"></param>
        /// <param name="orderBy"> 1推荐 2口碑 3.附近</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<SchoolExtListDto> GetSchoolExtList(double longitude, double latitude, int province, int city, int area,
            int[] grades, int orderBy, int distance, int[] types, int[] lodging, int index = 1, int size = 10)
        {
            //        var sql = @"SELECT 
            //             sch.name + '-' + ext.name AS name, sch.id AS sid,ext.id AS extid,
            //             charge.tuition,ext.type,ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,
            //             isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})',4326)),999999999) as Distance,
            //             content.Authentication,scores.score as Score,
            //            content.Characteristic,course.courses,content.lodging,
            //            p.name AS province,c.name AS city ,a.name AS area
            //            ,content.province as provincecode ,content.city AS citycode ,content.area AS areacode
            //             FROM dbo.OnlineSchoolExtension AS ext
            //            LEFT JOIN dbo.OnlineSchool AS sch
            //            ON ext.sid = sch.id
            //            LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge
            //            ON ext.id = charge.eid
            //            LEFT JOIN dbo.OnlineSchoolExtContent AS content
            //            ON ext.id = content.eid
            //            LEFT JOIN OnlineSchoolExtCourse AS course 
            //ON ext.id=course.eid 
            //            LEFT JOIN KeyValue AS p On content.province	 = p.id
            //            LEFT JOIN KeyValue AS c On content.city	 = c.id
            //            LEFT JOIN KeyValue AS a On content.area	 = a.id
            //            LEFT JOIN Score AS scores ON scores.eid=ext.id AND scores.indexid=22
            //            {4}
            //            where ext.IsValid = 1 AND sch.IsValid = 1 and sch.status=@status {2} order by {3}  OFFSET @pageIndex ROWS FETCH NEXT @pageSize ROWS ONLY";



            var sql = @"SELECT 
                 sch.name + '-' + ext.name AS name, sch.id AS sid,ext.id AS extid,
                 charge.tuition,ext.type,ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,
                 isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})',4326)),999999999) as Distance,
                 content.Authentication,scores.score as Score,
                content.Characteristic,content.lodging,content.sdextern,
               c.name AS city ,a.name AS area , ext.No as SchoolNo
                 FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch
                ON ext.sid = sch.id
                LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge
                ON ext.id = charge.eid
                LEFT JOIN dbo.OnlineSchoolExtContent AS content
                ON ext.id = content.eid
                LEFT JOIN KeyValue AS c On content.city	 = c.id
                LEFT JOIN KeyValue AS a On content.area	 = a.id
                LEFT JOIN dbo.ScoreTotal AS scores ON scores.eid=ext.id and scores.status=1
                {4}
                where ext.IsValid = 1 AND sch.IsValid = 1  and sch.status=@status {2} order by {3} 
                OFFSET @pageIndex ROWS FETCH NEXT @pageSize ROWS ONLY";

            //        var countSql = @"SELECT count(*) as Count FROM dbo.OnlineSchoolExtension AS ext
            //            LEFT JOIN dbo.OnlineSchool AS sch
            //            ON ext.sid = sch.id
            //            LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge
            //            ON ext.id = charge.eid
            //            LEFT JOIN dbo.OnlineSchoolExtContent AS content
            //            ON ext.id = content.eid
            //            LEFT JOIN OnlineSchoolExtCourse AS course 
            //ON ext.id=course.eid
            //             LEFT JOIN Score AS scores ON scores.eid=ext.id AND scores.indexid=22 {1}
            //            WHERE ext.IsValid = 1 AND sch.IsValid = 1 and sch.status=@status {0}";

            var countSql = @"SELECT count(*) as Count FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch
                ON ext.sid = sch.id
                LEFT JOIN dbo.OnlineSchoolExtContent AS content
                ON ext.id = content.eid 
                WHERE ext.IsValid = 1 AND sch.IsValid = 1 and sch.status=@status {0}";



            //left join
            var left = "";
            //where 条件
            StringBuilder where = new StringBuilder();
            //参数
            var para = new DynamicParameters();

            //order by 条件
            //1推荐 2口碑 3.附近
            var orderbyStr = " ext.id desc ";
            if (orderBy == 1)
            {
                orderbyStr = "scores.score desc";
            }
            else if (orderBy == 2)
            {
                left = " LEFT JOIN  dbo.SchoolScores AS score ON  ext.id=score.SchoolSectionId ";
                orderbyStr = "score.AggScore DESC";
            }
            else if (orderBy == 3)
            {
                //距离
                orderbyStr = " Distance ";
            }




            //学校状态
            para.Add("@status", (int)SchoolStatus.Success);
            if (grades.Count() > 0)
            {
                if (grades.Count() == 1)
                {
                    where.Append(" and ext.grade=@grade");
                    para.Add("@grade", grades.First());
                }
                else
                {
                    where.Append(" and ext.grade in (");
                    for (int i = 0; i < grades.Count(); i++)
                    {
                        where.Append("@grade" + i);
                        para.Add("@grade" + i, grades[i]);
                        if (i + 1 != grades.Count())
                            where.Append(",");
                    }
                    where.Append(") ");
                }
            }
            //if (province > 0)
            //{
            //    where.Append(" and content.province=@province");
            //    para.Add(new SqlParameter("province", province));
            //}
            if (city > 0)
            {
                where.Append(" and content.city=@city");
                para.Add("@city", city);
            }
            //if (area > 0)
            //{
            //    where.Append(" and content.area=@area");
            //    para.Add(new SqlParameter("area", area));
            //}
            if (types.Count() > 0)
            {
                if (types.Count() == 1)
                {
                    where.Append(" and ext.type=@type");
                    para.Add("@type", types.First());
                }
                else
                {
                    where.Append(" and ext.type in (");
                    for (int i = 0; i < types.Count(); i++)
                    {
                        where.Append("@type" + i);
                        para.Add("@type" + i, types[i]);
                        if (i + 1 != types.Count())
                            where.Append(",");
                    }
                    where.Append(") ");
                }
            }

            if (distance > 0)
            {
                where.Append(string.Format(" and isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999)<=@distance", longitude, latitude));
                para.Add("@distance", distance);
            }
            else if (orderBy == 3)
            {
                //附近学校
                //限制十公里
                where.Append(string.Format(" and  isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999)<=100000", longitude, latitude));
            }
            //0走读  1寄宿
            if (lodging != null && lodging.Count() > 0)
            {
                where.Append(" and content.lodging=@lodging");
                para.Add("@lodging", lodging.First());
            }

            //查询总条数
            SchoolExtConut count = null;
            if (orderBy == 1 || orderBy == 2)
            {

                count = await _easyRedisClient.GetOrAddAsync($"extlist_count_{city}_type_{string.Join(",", types)}_grade_{string.Join(",", grades)}_lodging_{string.Join(",", lodging)}",
                    () =>
                     {
                         return dbc.Query<SchoolExtConut>(string.Format(countSql, where.ToString()), para).FirstOrDefault();
                     }, DateTime.Now.AddMinutes(20));
            }
            else
            {
                count = dbc.Query<SchoolExtConut>(string.Format(countSql, where.ToString()), para).FirstOrDefault();
            }

            //经纬度
            //para.Add(new SqlParameter("longitude", longitude));
            //para.Add(new SqlParameter("latitude", latitude));
            para.Add("@pageIndex", (index - 1) * size);
            para.Add("@pageSize", size);

            IEnumerable<SchoolExtItemDto> list = null;
            //if ((orderBy == 1 || orderBy == 2) && index <= 10)
            //{
            //    list = await _easyRedisClient.GetOrAddAsync($"extlist_{city}_type_{string.Join(',', types)}_grade_{string.Join(',', grades)}_lodging_{string.Join(',', lodging)}_orderby_{orderBy}_index_{index}_lat_{latitude}_log_{longitude}",
            //        () =>
            //        {
            //            return dbc.Query<SchoolExtItemDto>(string.Format(sql, longitude, latitude, where.ToString(), orderbyStr, left), para.ToArray());

            //        }, DateTime.Now.AddMinutes(30));
            //}
            //else
            //{
            list = dbc.Query<SchoolExtItemDto>(string.Format(sql, longitude, latitude, where.ToString(), orderbyStr, left), para);
            //}

            SchoolExtListDto dto = new SchoolExtListDto();
            dto.List = list.ToList();
            dto.PageIndex = index;
            dto.PageCount = count.Count;
            return dto;
        }

        /// <summary>
        /// 获取学校分部的列表
        /// </summary>
        /// <param name="city"></param>
        /// <param name="grades"></param>
        /// <param name="orderBy"> 1推荐 2口碑 3.附近</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<SchoolExtListDto> GetSchoolExts(Guid extId, double longitude, double latitude, int province, int city, int area,
            int[] grades, int orderBy, int distance, int[] types, int[] lodging, string course, int? diglossia = null, int index = 1, int size = 10)
        {
            var sql = @"SELECT 
                 sch.name + '-' + ext.name AS name, sch.id AS sid,ext.id AS extid,charge.tuition,ext.type,ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})',4326)),999999999) as Distance,content.Authentication,scores.score as Score,hardware,content.Characteristic,content.lodging,c.name AS city ,a.name AS area
                 FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
                LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge ON ext.id = charge.eid
                LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
                LEFT JOIN dbo.OnlineSchoolExtLife AS life ON ext.id=life.eid
                LEFT JOIN dbo.OnlineSchoolExtCourse AS course ON ext.id=course.eid
                LEFT JOIN KeyValue AS c On content.city	 = c.id
                LEFT JOIN KeyValue AS a On content.area	 = a.id
                LEFT JOIN dbo.ScoreTotal AS scores ON scores.eid=ext.id and scores.status=1
                {4}
                where ext.IsValid = 1 AND sch.IsValid = 1 AND life.hardware is not null and sch.status=@status and ext.id<>@extId {2} order by {3} 
                OFFSET @pageIndex ROWS FETCH NEXT @pageSize ROWS ONLY";

            var countSql = @"SELECT count(1) as Count FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch
                ON ext.sid = sch.id
                LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid 
                LEFT JOIN dbo.OnlineSchoolExtLife AS life ON ext.id=life.eid
                LEFT JOIN dbo.OnlineSchoolExtCourse AS course ON ext.id=course.eid
                WHERE ext.IsValid = 1 AND life.hardware is not null AND sch.IsValid = 1 and sch.status=@status {0}";

            var left = "";
            //where 条件
            StringBuilder where = new StringBuilder();
            //where.Append(" and  life.hardware is not null ");
            //参数
            var para = new DynamicParameters();
            para.Add("@extId", extId);

            //order by 条件
            //1推荐 2口碑 3.附近
            var orderbyStr = " ext.id desc ";
            if (orderBy == 1)
            {
                orderbyStr = "scores.score desc";
            }
            else if (orderBy == 2)
            {
                left = " LEFT JOIN  dbo.SchoolScores AS score ON  ext.id=score.SchoolSectionId ";
                orderbyStr = "score.AggScore DESC";
            }
            else if (orderBy == 3)
            {
                //距离
                orderbyStr = " Distance ";
            }

            //学校状态
            para.Add("@status", (int)SchoolStatus.Success);
            if (grades.Count() > 0)
            {
                if (grades.Count() == 1)
                {
                    where.Append(" and ext.grade=@grade");
                    para.Add("@grade", grades.First());
                }
                else
                {
                    where.Append(" and ext.grade in (");
                    for (int i = 0; i < grades.Count(); i++)
                    {
                        where.Append("@grade" + i);
                        para.Add("@grade" + i, grades[i]);
                        if (i + 1 != grades.Count())
                            where.Append(",");
                    }
                    where.Append(") ");
                }
            }
            //if (province > 0)
            //{
            //    where.Append(" and content.province=@province");
            //    para.Add(new SqlParameter("province", province));
            //}
            if (city > 0)
            {
                where.Append(" and content.city=@city");
                para.Add("@city", city);
            }
            if (area > 0)
            {
                where.Append(" and content.area=@area");
                para.Add("@area", area);
            }
            if (types.Count() > 0)
            {
                if (types.Count() == 1)
                {
                    where.Append(" and ext.type=@type");
                    para.Add("@type", types.First());
                }
                else
                {
                    where.Append(" and ext.type in (");
                    for (int i = 0; i < types.Count(); i++)
                    {
                        where.Append("@type" + i);
                        para.Add("@type" + i, types[i]);
                        if (i + 1 != types.Count())
                            where.Append(",");
                    }
                    where.Append(") ");
                }
            }
            if (types.Count() == 1 && types.Any(t => t == 3))
            {
                where.Append(" and course.courses=@courses");
                para.Add("@courses", course);
            }
            if (distance > 0)
            {
                where.Append(string.Format(" and isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999)<=@distance", longitude, latitude));
                para.Add("@distance", distance);
            }
            else if (orderBy == 3)
            {
                //附近学校
                //限制十公里
                where.Append(string.Format(" and  isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999)<=100000", longitude, latitude));
            }
            //0走读  1寄宿
            if (lodging != null && lodging.Count() > 0)
            {
                where.Append(" and content.lodging=@lodging");
                para.Add("@lodging", lodging.First());
            }

            //查询总条数
            SchoolExtConut count = null;
            if (orderBy == 1 || orderBy == 2)
            {

                count = await _easyRedisClient.GetOrAddAsync($"extlist_count_{city}_type_{string.Join(",", types)}_grade_{string.Join(",", grades)}_lodging_{string.Join(",", lodging)}",
                    () =>
                    {
                        return dbc.Query<SchoolExtConut>(string.Format(countSql, where.ToString()), para).FirstOrDefault();
                    }, DateTime.Now.AddMinutes(20));
            }
            else
            {
                count = dbc.Query<SchoolExtConut>(string.Format(countSql, where.ToString()), para).FirstOrDefault();
            }

            //经纬度
            //para.Add(new SqlParameter("longitude", longitude));
            //para.Add(new SqlParameter("latitude", latitude));
            para.Add("@pageIndex", (index - 1) * size);
            para.Add("@pageSize", size);
            var querySql = string.Format(sql, longitude, latitude, where.ToString(), orderbyStr, left);
            var list = dbc.Query<SchoolExtItemDto>(querySql, para);
            SchoolExtListDto dto = new SchoolExtListDto();
            dto.List = list.ToList();
            dto.PageIndex = index;
            dto.PageCount = count.Count;
            return dto;
        }




        /// <summary>
        /// 获取学校的推荐学校
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>

        public List<KeyValueDto<Guid, Guid, string, string, int>> RecommendSchool(byte type, byte grade, int city, Guid extid, int top)
        {
            var sql = $@"SELECT  TOP {top} ext.sid AS [key],ext.id AS [value],sch.name AS message,ext.name AS data , ext.no as Other FROM dbo.OnlineSchoolExtension AS ext
            LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
            LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
            LEFT JOIN Score AS scores ON scores.eid=ext.id AND scores.indexid=22 
            WHERE content.city =@city and ext.IsValid=1 and sch.IsValid=1
            AND ext.type = @type AND ext.grade = @grade  and   ext.id!=@extid 
            AND sch.status = @status  ORDER BY scores.score DESC";

            var list = dbc.Query<KeyValueDto<Guid, Guid, string, string, int>>(sql, new { city, type, grade, status = (byte)SchoolStatus.Success, extid });
            return list.ToList();
        }


        /// <summary>
        /// 获取学校详情
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public SchoolExtDto GetSchoolExtDetails(Guid extId)
        {
            var sql = @"SELECT school.name+' - '+ext.name as Name,school.name as schoolname,ext.name as extname,
                        ext.No as SchoolNo,
                        school.name_e AS EName,ext.id as ExtId,ext.sid,
                        school.website,school.logo,Charge.tuition,Charge.applicationfee,
                        Charge.otherfee,ext.type,ext.discount,
                        ext.diglossia,ext.chinese,ext.SchFtype,school.intro,
                        content.tel,school.website,content.address,content.lodging,content.sdextern,content.canteen,
                        content.meal,content.characteristic,
                        content.authentication,content.foreignTea,content.abroad,
                        content.openhours,content.calendar,content.range,
                        content.afterclass,content.counterpart,content.province,
                        content.city,content.area,content.longitude,content.latitude,
                        ext.grade,content.studentcount,content.teachercount,content.studentcount,
                        content.tsPercent,recruit.age,recruit.maxage,
                        recruit.target,recruit.proportion,recruit.point,recruit.count,
                        course.courses,course.authentication AS CourseAuthentication,
                        course.characteristic as CourseCharacteristic,area.name AS areaname,city.name AS cityname,ext.HasSchoolBus,school.EduSysType
                        ,overviewinfo.RecruitWay
                        ,overviewinfo.OAName,overviewinfo.OAAppID,overviewinfo.OAAccount
                        ,overviewinfo.MPName,overviewinfo.MPAppID,overviewinfo.MPAccount
                        ,overviewinfo.VANAme,overviewinfo.VAAppID,overviewinfo.VAAccount
                        FROM dbo.OnlineSchoolExtension AS ext
                        LEFT JOIN dbo.OnlineSchool AS school ON ext.sid=school.id
                        LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id=content.eid AND content.IsValid=1
                        LEFT JOIN dbo.OnlineSchoolExtCharge AS Charge ON ext.id=Charge.eid AND Charge.IsValid=1
            LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id=recruit.eid AND recruit.IsValid=1
            LEFT JOIN dbo.OnlineSchoolExtCourse AS course ON  ext.id=course.eid AND course.IsValid=1
            LEFT JOIN dbo.KeyValue AS city ON city.id=content.city
            LEFT JOIN dbo.KeyValue AS area ON area.id=content.area
            LEFT JOIN SchoolOverViewInfo overviewinfo ON overviewinfo.EID = ext.id
                        WHERE ext.id=@extId  AND school.IsValid=1 AND ext.IsValid=1  and school.status=@status";

            var data = dbc.Query<SchoolExtDto>(sql, new { extId, status = (int)SchoolStatus.Success }).FirstOrDefault();
            if (data != null)
            {
                ////查询tags
                //var tagsSql = @"SELECT tags.id AS TagId,tags.name TagName,bind.dataID DataId 
                //FROM  dbo.GeneralTagBind AS bind
                //LEFT JOIN  dbo.GeneralTag AS tags
                //ON bind.tagID=tags.id  WHERE bind.dataType=2 AND bind.dataID=@ExtId";
                //data.Tags = dbc.Query<Tags>(tagsSql, new SqlParameter("ExtId", extId)).Select(p => p.TagName).ToList();
                //查询升学成绩

                StringBuilder achSQL = new StringBuilder();
                if (data.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
                {
                    //高中
                    achSQL.Append(@"SELECT ach.schoolId AS [key],college.name AS [value],ach.count AS [message],ach.year AS [data]  FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT JOIN dbo.College AS college ON ach.schoolId=college.id
                WHERE ach.IsValid=1 AND college.IsValid=1 AND ach.extId=@extId AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc) ORDER BY ach.year desc");
                }
                else if (data.Grade != (byte)SchoolGrade.Kindergarten && data.Type != (byte)SchoolType.SAR)
                {
                    achSQL.Append(@"SELECT ach.schoolId AS [key],sch.name AS [value],ach.count AS [message],ach.year AS [data]
                FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT  JOIN  dbo.OnlineSchoolExtension AS ext ON ach.schoolId =ext.id
                LEFT JOIN dbo.OnlineSchool AS sch ON sch.id=ext.sid
               WHERE ach.extId=@extId AND ach.IsValid=1  AND ext.IsValid=1  AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc)
                 ORDER BY ach.year,ach.count DESC");
                }

                ////获取最新录入的升学成绩的年份
                //IEnumerable<KeyValueDto<int>> year = new List<KeyValueDto<int>>();
                //var yearSql = @"select top 1 year as value from {0} WHERE extId=@extId AND IsValid=1 GROUP BY YEAR ORDER BY YEAR DESC";
                //if (data.Grade == (byte)SchoolGrade.PrimarySchool && new byte[] { (byte)SchoolType.Public, (byte)SchoolType.Private }.Contains(data.Type))
                //{
                //    //公办/民办小学
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlinePrimarySchoolAchievement"), new SqlParameter("extId", extId));

                //}
                //else if (data.Grade == (byte)SchoolGrade.Kindergarten)
                //{
                //    //幼儿园
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineKindergartenAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.JuniorMiddleSchool && new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(data.Type))
                //{
                //    //民办/公办初中
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineMiddleSchoolAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.JuniorMiddleSchool && data.Type == (byte)SchoolType.International)
                //{
                //    //国际初中
                //    year = dbc.Query<KeyValueDto<int>>
                //        (string.Format(yearSql, "OnlineSchoolAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.SeniorMiddleSchool && new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(data.Type))
                //{
                //    //民办/公办高中
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineHighSchoolAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.SeniorMiddleSchool && data.Type == (byte)SchoolType.International)
                //{
                //    //国际高中
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineSchoolAchievement"), new SqlParameter("extId", extId));
                //}

                //if (year.Count() > 0)
                //{
                //年份
                //data.AchYear = year.FirstOrDefault()?.Value;
                //data.AchYear = DateTime.Now.Year - 1;
                if (!new byte[] { (byte)SchoolGrade.Kindergarten, (byte)SchoolGrade.PrimarySchool }.Contains(data.Grade))
                {
                    //升学成绩
                    data.Achievement = dbc.Query<KeyValueDto<Guid, string, double, int>>(achSQL.ToString(), new { extId }).ToList();
                    if (data.Achievement != null && data.Achievement.Any())
                    {
                        data.AchYear = data.Achievement.FirstOrDefault().Data;
                    }
                    else
                    {
                        switch (data.Grade)
                        {
                            case (byte)SchoolGrade.JuniorMiddleSchool:
                                data.AchYear = dbc.Query<int>("select top 1 year from OnlineMiddleSchoolAchievement where extId=@extId and IsValid = 1 order by year desc;", new { extId }).FirstOrDefault();
                                break;
                            case (byte)SchoolGrade.SeniorMiddleSchool:
                                data.AchYear = dbc.Query<int>("select top 1 year from OnlineHighSchoolAchievement where extId=@extId  and IsValid = 1 order by year desc;", new { extId }).FirstOrDefault();
                                break;
                        }
                    }
                }
                //}
                //else
                //{
                //data.AchYear = null;
                //data.Achievement = new List<KeyValueDto<Guid, string, double>>();
                //}

            }
            return data;
        }


        /// <summary>
        /// 获取学校详情
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public SchoolExtAnyDto GetSchoolExtDetailsAny(Guid extId)
        {
            var sql = @"SELECT school.name+' - '+ext.name as Name,school.name as schoolname,ext.name as extname,
                        ext.No as SchoolNo,
                        ext.IsValid,
                        school.status,
                        school.name_e AS EName,ext.id as ExtId,ext.sid,
                        school.website,school.logo,Charge.tuition,Charge.applicationfee,
                        Charge.otherfee,ext.type,ext.discount,
                        ext.diglossia,ext.chinese,school.intro,
                        content.tel,school.website,content.address,content.lodging,content.sdextern,content.canteen,
                        content.meal,content.characteristic,
                        content.authentication,content.foreignTea,content.abroad,
                        content.openhours,content.calendar,content.range,
                        content.afterclass,content.counterpart,content.province,
                        content.city,content.area,content.longitude,content.latitude,
                        ext.grade,content.studentcount,content.teachercount,content.studentcount,
                        content.tsPercent,recruit.age,recruit.maxage,
                        recruit.target,recruit.proportion,recruit.point,recruit.count,
                        course.courses,course.authentication AS CourseAuthentication,
                        course.characteristic as CourseCharacteristic,area.name AS areaname,city.name AS cityname,ext.HasSchoolBus,school.EduSysType
                        ,overviewinfo.RecruitWay
                        ,overviewinfo.OAName,overviewinfo.OAAppID,overviewinfo.OAAccount
                        ,overviewinfo.MPName,overviewinfo.MPAppID,overviewinfo.MPAccount
                        ,overviewinfo.VANAme,overviewinfo.VAAppID,overviewinfo.VAAccount
                        FROM dbo.OnlineSchoolExtension AS ext
                        LEFT JOIN dbo.OnlineSchool AS school ON ext.sid=school.id
                        LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id=content.eid AND content.IsValid=1
                        LEFT JOIN dbo.OnlineSchoolExtCharge AS Charge ON ext.id=Charge.eid AND Charge.IsValid=1
            LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id=recruit.eid AND recruit.IsValid=1
            LEFT JOIN dbo.OnlineSchoolExtCourse AS course ON  ext.id=course.eid AND course.IsValid=1
            LEFT JOIN dbo.KeyValue AS city ON city.id=content.city
            LEFT JOIN dbo.KeyValue AS area ON area.id=content.area
            LEFT JOIN SchoolOverViewInfo overviewinfo ON overviewinfo.EID = ext.id
                        WHERE ext.id=@extId  ";

            var data = dbc.Query<SchoolExtAnyDto>(sql, new { extId }).FirstOrDefault();
            if (data != null)
            {
                ////查询tags
                //var tagsSql = @"SELECT tags.id AS TagId,tags.name TagName,bind.dataID DataId 
                //FROM  dbo.GeneralTagBind AS bind
                //LEFT JOIN  dbo.GeneralTag AS tags
                //ON bind.tagID=tags.id  WHERE bind.dataType=2 AND bind.dataID=@ExtId";
                //data.Tags = dbc.Query<Tags>(tagsSql, new SqlParameter("ExtId", extId)).Select(p => p.TagName).ToList();
                //查询升学成绩

                StringBuilder achSQL = new StringBuilder();
                if (data.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
                {
                    //高中
                    achSQL.Append(@"SELECT ach.schoolId AS [key],college.name AS [value],ach.count AS [message],ach.year AS [data]  FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT JOIN dbo.College AS college ON ach.schoolId=college.id
                WHERE ach.IsValid=1 AND college.IsValid=1 AND ach.extId=@extId AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc) ORDER BY ach.year desc");
                }
                else if (data.Grade != (byte)SchoolGrade.Kindergarten && data.Type != (byte)SchoolType.SAR)
                {
                    achSQL.Append(@"SELECT ach.schoolId AS [key],sch.name AS [value],ach.count AS [message],ach.year AS [data]
                FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT  JOIN  dbo.OnlineSchoolExtension AS ext ON ach.schoolId =ext.id
                LEFT JOIN dbo.OnlineSchool AS sch ON sch.id=ext.sid
               WHERE ach.extId=@extId AND ach.IsValid=1  AND ext.IsValid=1  AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc)
                 ORDER BY ach.year,ach.count DESC");
                }

                ////获取最新录入的升学成绩的年份
                //IEnumerable<KeyValueDto<int>> year = new List<KeyValueDto<int>>();
                //var yearSql = @"select top 1 year as value from {0} WHERE extId=@extId AND IsValid=1 GROUP BY YEAR ORDER BY YEAR DESC";
                //if (data.Grade == (byte)SchoolGrade.PrimarySchool && new byte[] { (byte)SchoolType.Public, (byte)SchoolType.Private }.Contains(data.Type))
                //{
                //    //公办/民办小学
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlinePrimarySchoolAchievement"), new SqlParameter("extId", extId));

                //}
                //else if (data.Grade == (byte)SchoolGrade.Kindergarten)
                //{
                //    //幼儿园
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineKindergartenAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.JuniorMiddleSchool && new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(data.Type))
                //{
                //    //民办/公办初中
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineMiddleSchoolAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.JuniorMiddleSchool && data.Type == (byte)SchoolType.International)
                //{
                //    //国际初中
                //    year = dbc.Query<KeyValueDto<int>>
                //        (string.Format(yearSql, "OnlineSchoolAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.SeniorMiddleSchool && new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(data.Type))
                //{
                //    //民办/公办高中
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineHighSchoolAchievement"), new SqlParameter("extId", extId));
                //}
                //else if (data.Grade == (byte)SchoolGrade.SeniorMiddleSchool && data.Type == (byte)SchoolType.International)
                //{
                //    //国际高中
                //    year = dbc.Query<KeyValueDto<int>>(string.Format(yearSql, "OnlineSchoolAchievement"), new SqlParameter("extId", extId));
                //}

                //if (year.Count() > 0)
                //{
                //年份
                //data.AchYear = year.FirstOrDefault()?.Value;
                //data.AchYear = DateTime.Now.Year - 1;
                if (!new byte[] { (byte)SchoolGrade.Kindergarten, (byte)SchoolGrade.PrimarySchool }.Contains(data.Grade))
                {
                    //升学成绩
                    data.Achievement = dbc.Query<KeyValueDto<Guid, string, double, int>>(achSQL.ToString(), new { extId }).ToList();
                    if (data.Achievement != null && data.Achievement.Any())
                    {
                        data.AchYear = data.Achievement.FirstOrDefault().Data;
                    }
                    else
                    {
                        switch (data.Grade)
                        {
                            case (byte)SchoolGrade.JuniorMiddleSchool:
                                data.AchYear = dbc.Query<int>("select top 1 year from OnlineMiddleSchoolAchievement where extId=@extId and IsValid = 1 order by year desc;", new { extId }).FirstOrDefault();
                                break;
                            case (byte)SchoolGrade.SeniorMiddleSchool:
                                data.AchYear = dbc.Query<int>("select top 1 year from OnlineHighSchoolAchievement where extId=@extId  and IsValid = 1 order by year desc;", new { extId }).FirstOrDefault();
                                break;
                        }
                    }
                }
                //}
                //else
                //{
                //data.AchYear = null;
                //data.Achievement = new List<KeyValueDto<Guid, string, double>>();
                //}

            }
            return data;
        }

        /// <summary>
        /// 获取学校毕业生升学去向
        /// </summary>
        /// <param name="data"></param>
        /// <param name="extId"></param>
        /// <returns></returns>
        public IEnumerable<KeyValueDto<Guid, string, double, int, int>> GetschoolChoice(string extId, int grade, int type)
        {
            StringBuilder achSQL = new StringBuilder();
            if (grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                //高中
                achSQL.Append(@"SELECT ach.schoolId AS [key],college.name AS [value],ach.count AS [message],ach.year AS [data],college.type AS [Other]
                                    FROM dbo.OnlineSchoolAchievement AS ach 
                                    LEFT JOIN dbo.College AS college ON ach.schoolId=college.id
                                    WHERE ach.IsValid=1 AND college.IsValid=1 AND ach.extId=@extId AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc) ORDER BY ach.year desc");
            }
            else if (grade != (byte)SchoolGrade.Kindergarten && type != (byte)SchoolType.SAR)
            {
                achSQL.Append(@"SELECT ach.schoolId AS [key],sch.name AS [value],ach.count AS [message],ach.year AS [data],ext.type AS [Other] 
                                    FROM dbo.OnlineSchoolAchievement AS ach 
                                    LEFT  JOIN  dbo.OnlineSchoolExtension AS ext ON ach.schoolId =ext.id
                                    LEFT JOIN dbo.OnlineSchool AS sch ON sch.id=ext.sid
                                    WHERE ach.extId=@extId AND ach.IsValid=1  AND ext.IsValid=1  AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc)
                                     ORDER BY ach.year,ach.count DESC");
            }
            //升学成绩
            var data = dbc.Query<KeyValueDto<Guid, string, double, int, int>>(achSQL.ToString(), new { extId }).ToList();
            return data;
        }


        /// <summary>
        /// 获取学校的划片范围信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public string GetSchoolExtRange(Guid extId)
        {
            var sql = @"SELECT content.range FROM dbo.OnlineSchoolExtContent AS content LEFT JOIN dbo.OnlineSchoolExtension AS ext ON content.eid = ext.id WHERE ext.IsValid = 1 AND content.eid = @eid";

            var data = dbc.Query<SchoolExtRange>(sql, new { eid = extId }).FirstOrDefault();
            return data.Range;
        }


        /// <summary>
        /// 根据学校id获取学校的地址
        /// </summary>
        /// <param name="extIds"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid>> GetSchoolExtAddress(params Guid[] extIds)
        {
            var cmd = new SqlCommand(@"SELECT eid AS [Value] ,address AS [KEY],No As [Other]
                FROM dbo.OnlineSchoolExtContent  ec join dbo.OnlineSchoolExtension ext on ec.eid=ext.id  WHERE eid IN @extids");
            //var addressParam = cmd.AddArrayParameters("ExtId", extIds, SqlDbType.UniqueIdentifier);
            var address = dbc.Query<KeyValueDto<Guid>>(cmd.CommandText, new { extids = extIds });
            return address.ToList();
        }
        public async Task<IEnumerable<KeyValueDto<Guid, int, string, string>>> GetSchoolExtAddress(IEnumerable<Guid> eids)
        {
            var str_SQL = @"SELECT
	                            ose.id AS [Key],
	                            ose.[No] AS [Value],
	                            osec.address AS [Data],
	                            os.name + ' - ' + ose.name AS [Message] 
                            FROM
	                            OnlineSchoolExtContent AS osec
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = osec.eid
	                            LEFT JOIN OnlineSchool AS os ON os.id = ose.sid 
                            WHERE
	                            osec.eid IN @eids";
            return await dbc.QueryAsync<KeyValueDto<Guid, int, string, string>>(str_SQL, new { eids });
        }
        /// <summary>
        /// 根据学校
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public KeyValueDto<double?> GetDistance(Guid extId, double longitude, double latitude)
        {
            var sql = "SELECT '{2}' as [key] ,isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999)  as [value]  FROM dbo.OnlineSchoolExtContent where eid=@extId ";
            var distance = dbc.Query<KeyValueDto<double?>>(string.Format(sql, longitude, latitude, extId.ToString()), new { extId });
            return distance.FirstOrDefault() ?? new KeyValueDto<double?> { Key = extId.ToString(), Value = 999999999 };
        }

        /// <summary>
        /// 获取学校的id
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid, Guid?, string>> GetSchoolId(params Guid[] extId)
        {
            var cmd = new SqlCommand(@"SELECT  ext.id AS [Key],sch.id AS [value] ,''AS message  
            FROM dbo.OnlineSchoolExtension AS ext 
            LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
            WHERE ext.IsValid=1 AND  sch.IsValid=1 and sch.status=3  AND ext.id in @extIds");
            //var param = cmd.AddArrayParameters("ExtId", extId, SqlDbType.UniqueIdentifier);
            var data = dbc.Query<KeyValueDto<Guid, Guid?, string>>(cmd.CommandText, new { extIds = extId });
            return data.ToList();
        }
        /// <summary>
        /// 获取学校的videos
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public List<KeyValueDto<DateTime, string, byte>> GetExtViedeos(Guid extId, byte type = 0)
        {
            var sql = @"SELECT CreateTime AS [key],videoUrl AS [value],type AS message FROM dbo.OnlineSchoolVideo WHERE eid=@eid AND IsValid=1 ";
            if (type != 0)
            {
                sql += $"and type={type}";
            }
            var result = dbc.Query<KeyValueDto<DateTime, string, byte>>(sql, new { eid = extId }).ToList();
            return result;
        }

        /// <summary>
        /// 获取学校招生计划更多信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public SchoolExtRecruitDto GetSchoolExtRecruit(Guid extId)
        {
            //Data,Contact,subjects,past,scholar
            //      var countField = $"(select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id  and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Count)}' and Content <>'' order by year desc) as Count ";
            //      var ageField = $"(select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Age)}' order by year desc) as Age";
            //      var maxAgeField = $"(select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.MaxAge)}' order by year desc) as MaxAge";
            //      var tarGetField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Target)}' order by year desc) as Target";
            //      var dataField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Data)}' order by year desc) as Data";
            //      var contactField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Contact)}' order by year desc) as Contact";
            //      var subjectsField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Subjects)}' order by year desc) as Subjects";
            //      var pastexamField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Pastexam)}' order by year desc) as Pastexam";
            //      var scholarshipField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Scholarship)}' order by year desc) as ScholarShip";
            //      var sql = @" SELECT recruit.proportion,
            //                  recruit.point,
            //                  recruit.date,quality.teacher,quality.principal,
            //                  quality.schoolhonor,quality.studenthonor,life.hardware,life.community,life.timetables,
            //                  life.schedule,life.diagram,ext.name AS extname,sch.name AS schoolname
            //                  ";
            //      sql += $",{dataField},";
            //      sql += $"{countField},";
            //      sql += $"{ageField},";
            //      sql += $"{maxAgeField},";
            //      sql += $"{tarGetField},";
            //      sql += $"{contactField},";
            //      sql += $"{subjectsField},";
            //      sql += $"{pastexamField},";
            //      sql += $"{scholarshipField} ";
            //      sql += @"    FROM dbo.OnlineSchoolExtension AS ext 
            //                  LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id =recruit.eid
            //                  LEFT JOIN  dbo.OnlineSchoolExtQuality AS quality ON ext.id=quality.eid
            //                  LEFT JOIN dbo.OnlineSchoolExtLife AS life ON ext.id=life.eid
            //LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
            //                  WHERE ext.IsValid=1 AND ext.id=@extid AND sch.IsValid=1 AND sch.status=@status";
            var sql = @" SELECT recruit.age,recruit.maxage,recruit.target,recruit.proportion,recruit.count,
                        recruit.point,recruit.scholarship,recruit.data,recruit.contact,recruit.subjects,
                        recruit.date,recruit.pastexam,quality.teacher,quality.principal,
                        quality.schoolhonor,quality.studenthonor,life.hardware,life.community,life.timetables,
                        life.schedule,life.diagram,ext.name AS extname,sch.name AS schoolname
                         FROM dbo.OnlineSchoolExtension AS ext 
                        LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id =recruit.eid
                        LEFT JOIN  dbo.OnlineSchoolExtQuality AS quality ON ext.id=quality.eid
                        LEFT JOIN dbo.OnlineSchoolExtLife AS life ON ext.id=life.eid
						LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
                        WHERE ext.IsValid=1 AND ext.id=@extid AND sch.IsValid=1 AND sch.status=@status";
            var data = dbc.Query<SchoolExtRecruitDto>(sql, new { extId, status = (byte)SchoolStatus.Success });
            return data.FirstOrDefault();
        }

        /// <summary>
        /// 获取学校招生计划更多信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public SchoolExtRecruitDto GetSchoolExtRecruitAny(Guid extId)
        {
            //Data,Contact,subjects,past,scholar
            //      var countField = $"(select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id  and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Count)}' and Content <>'' order by year desc) as Count ";
            //      var ageField = $"(select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Age)}' order by year desc) as Age";
            //      var maxAgeField = $"(select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.MaxAge)}' order by year desc) as MaxAge";
            //      var tarGetField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Target)}' order by year desc) as Target";
            //      var dataField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Data)}' order by year desc) as Data";
            //      var contactField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Contact)}' order by year desc) as Contact";
            //      var subjectsField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Subjects)}' order by year desc) as Subjects";
            //      var pastexamField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Pastexam)}' order by year desc) as Pastexam";
            //      var scholarshipField = $"(select top 1 Content from OnlineSchoolYearFieldContent where eid=ext.id and IsValid=1  and field='{EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Scholarship)}' order by year desc) as ScholarShip";
            //      var sql = @" SELECT recruit.proportion,
            //                  recruit.point,
            //                  recruit.date,quality.teacher,quality.principal,
            //                  quality.schoolhonor,quality.studenthonor,life.hardware,life.community,life.timetables,
            //                  life.schedule,life.diagram,ext.name AS extname,sch.name AS schoolname
            //                  ";
            //      sql += $",{dataField},";
            //      sql += $"{countField},";
            //      sql += $"{ageField},";
            //      sql += $"{maxAgeField},";
            //      sql += $"{tarGetField},";
            //      sql += $"{contactField},";
            //      sql += $"{subjectsField},";
            //      sql += $"{pastexamField},";
            //      sql += $"{scholarshipField} ";
            //      sql += @"    FROM dbo.OnlineSchoolExtension AS ext 
            //                  LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id =recruit.eid
            //                  LEFT JOIN  dbo.OnlineSchoolExtQuality AS quality ON ext.id=quality.eid
            //                  LEFT JOIN dbo.OnlineSchoolExtLife AS life ON ext.id=life.eid
            //LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
            //                  WHERE ext.IsValid=1 AND ext.id=@extid AND sch.IsValid=1 AND sch.status=@status";
            var sql = @" SELECT recruit.age,recruit.maxage,recruit.target,recruit.proportion,recruit.count,
                        recruit.point,recruit.scholarship,recruit.data,recruit.contact,recruit.subjects,
                        recruit.date,recruit.pastexam,quality.teacher,quality.principal,
                        quality.schoolhonor,quality.studenthonor,life.hardware,life.community,life.timetables,
                        life.schedule,life.diagram,ext.name AS extname,sch.name AS schoolname
                         FROM dbo.OnlineSchoolExtension AS ext 
                        LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id =recruit.eid
                        LEFT JOIN  dbo.OnlineSchoolExtQuality AS quality ON ext.id=quality.eid
                        LEFT JOIN dbo.OnlineSchoolExtLife AS life ON ext.id=life.eid
						LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
                        WHERE ext.id=@extid ";
            var data = dbc.Query<SchoolExtRecruitDto>(sql, new { extId });
            return data.FirstOrDefault();
        }
        /// <summary>
        /// 获取学校学部的升学成绩
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <returns></returns>
        public object GetAchievementData(Guid extId, byte type, byte grade, int? year)
        {
            var sql = @" SELECT * FROM {0} WHERE year=@year AND extId=@extId";
            if (grade == (byte)SchoolGrade.Kindergarten)
            {
                //幼儿园
                return dbc.Query<KindergartenAchievement>(string.Format(sql, "OnlineKindergartenAchievement"), new { extId, year });
            }
            else if (new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(type) && grade == (byte)SchoolGrade.PrimarySchool)
            {
                //民办/公办小学
                return dbc.Query<PrimarySchoolAchievement>(string.Format(sql, "OnlinePrimarySchoolAchievement"), new { extId, year });
            }
            else if (new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(type) && grade == (byte)SchoolGrade.JuniorMiddleSchool)
            {
                //民办/公办初中
                return dbc.Query<MiddleSchoolAchievement>(string.Format(sql, "OnlineMiddleSchoolAchievement"), new { extId, year }).FirstOrDefault();
            }
            else if (new byte[] { (byte)SchoolType.International, (byte)SchoolType.ForeignNationality }.Contains(type) && grade == (byte)SchoolGrade.JuniorMiddleSchool)
            {
                //国际/外籍初中
                return dbc.Query<MiddleSchoolAchievement>(string.Format(sql, "OnlineMiddleSchoolAchievement"), new { extId, year }).FirstOrDefault();
            }
            else if (new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(type) && grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                //民办/公办高中
                return dbc.Query<HighSchoolAchievement>(string.Format(sql, "OnlineHighSchoolAchievement"), new { extId, year }).FirstOrDefault();
            }
            return null;
        }

        public List<int> GetAchievementYears(Guid extId, byte type, byte grade)
        {
            var sql = @"Select DISTINCT([year]) as year from {0} WHERE IsValid = 1 And extId=@extId
                        UNION
                        Select DISTINCT([year]) as year from OnlineSchoolAchievement WHERE IsValid = 1 And extId=@extId ;";
            if (grade == (byte)SchoolGrade.Kindergarten)
            {
                //幼儿园
                return dbc.Query<int>(string.Format(sql, "OnlineKindergartenAchievement"), new { extId }).ToList();
            }
            else if (new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(type) && grade == (byte)SchoolGrade.PrimarySchool)
            {
                //民办/公办小学
                return dbc.Query<int>(string.Format(sql, "OnlinePrimarySchoolAchievement"), new { extId }).ToList();
            }
            else if (new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(type) && grade == (byte)SchoolGrade.JuniorMiddleSchool)
            {
                //民办/公办初中
                return dbc.Query<int>(string.Format(sql, "OnlineMiddleSchoolAchievement"), new { extId }).ToList();
            }
            else if (new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public }.Contains(type) && grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                //民办/公办高中
                return dbc.Query<int>(string.Format(sql, "OnlineHighSchoolAchievement"), new { extId }).ToList();
            }
            return null;
        }
        public List<Tag> GetTagList()
        {
            var sql = @"SELECT
	                        id,
	                        name,
	                        spellcode,
	                        type,
	                        sort,
                        NO 
                        FROM
	                        tag 
                        WHERE
	                        IsValid = 1;";
            var result = dbc.Query<Tag>(sql, new { }).ToList();

            //var tags = result.Where(p => p.Type == 3).ToList();
            //var names = new List<string>() { "英国", "美国", "加拿大", "新加坡", "澳洲", "法国", "中国香港", "韩国", "日本", "德国", "新西兰", "瑞士", "亚洲其他", "欧洲其他", "其他" };
            //var index = 1;
            //foreach (var name in names)
            //{
            //    var find = tags.FirstOrDefault(p => p.Name == name.ToLower());
            //    if (find != null)
            //    {
            //        find.Sort = index;
            //        dbc.ExecuteScalar<int>($"update tag set sort = {index} where id = '{find.Id}';");
            //    }
            //    index++;
            //}

            //var tags = result.Where(p => p.Type == 6).ToList();
            //var names = new List<string>() { "国家义务教育课程", "IB", "AP", "A-level", "IGCSE", "VCE", "STEAM课程", "双语课程", "国际课程", "IPC", "DP", "IB-PYP", "IB-DP", "IP-MYP", "SAT", "QCE", "IMYC" };
            //var index = 1;
            //foreach (var name in names)
            //{
            //    var find = tags.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            //    if (find != null)
            //    {
            //        find.Sort = index;
            //        dbc.ExecuteScalar<int>($"update tag set sort = {index} where id = '{find.Id}';");
            //    }
            //    index++;
            //}

            return result;
        }


        public async Task<IEnumerable<TagFlat>> GetTagFlats()
        {
            var sql = $@" 
SELECT
	T.Id,
	T.Name,
    T.No,
	T.SpellCode,
	T.Type as RootId,
	Root.name AS RootName,
	T.subdivision AS ParentId,
	Parent.Name AS ParentName
FROM
	dbo.Tag T
	INNER JOIN TagType Root ON Root.id = T.Type AND Root.IsValid = 1
	LEFT JOIN TagType Parent ON Parent.id = T.subdivision AND Parent.IsValid = 1
WHERE
	T.IsValid = 1
ORDER BY
	Root.Sort, Parent.Sort, T.Sort
                ";
            return await dbc.QueryAsync<TagFlat>(sql, new { });
        }

        /// <summary>
        /// 根据分部ID获取学校分部信息
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        public List<SchoolExtItemDto> GetSchoolExtListByBranchIds(List<Guid> eids, double latitude = 0, double longitude = 0, bool readIntro = false)
        {
            if (eids.Count == 0)
                return new List<SchoolExtItemDto>();

            //var tuitionField = " (select top 1 (case when content is null or content = '' then null else content end ) from OnlineSchoolYearFieldContent where eid=ext.id and field='Tuition' order by year desc) as tuition ,";
            //+ tuitionField
            //+ @"ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,
            var sql = @"SELECT 
                 sch.name + '-' + ext.name AS name, sch.id AS sid,ext.id AS extid,
                content.creationdate,content.studentcount,content.teachercount,
                charge.tuition,ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,
                 content.Authentication,content.Characteristic,course.courses,content.Lodging,content.sdextern,
                p.name AS province,c.name AS city ,a.name AS area,
                content.province as provincecode ,content.city AS citycode ,content.area AS areacode,
                (select top 1 scores.score from Score AS scores
					where scores.eid=ext.id AND scores.indexid=22
				) as score ,ext.IsValid as extValid,sch.IsValid as schoolValid,sch.status {0},
                  ext.No as SchoolNo
                 FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch
                ON ext.sid = sch.id
                LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge
                ON ext.id = charge.eid and  charge.IsValid=1
                LEFT JOIN dbo.OnlineSchoolExtContent AS content
                ON ext.id = content.eid and content.IsValid=1
                LEFT JOIN OnlineSchoolExtCourse AS course 
				ON ext.id=course.eid and course.IsValid=1
                LEFT JOIN KeyValue AS p On content.province	 = p.id
                LEFT JOIN KeyValue AS c On content.city	 = c.id
                LEFT JOIN KeyValue AS a On content.area	 = a.id
                --LEFT JOIN Score AS scores ON scores.eid=ext.id AND scores.indexid=22 
                where 1=1  --and content.IsValid=1 and  charge.IsValid=1 and course.IsValid=1
                --and ext.IsValid = 1 AND sch.IsValid = 1 and sch.status=@status
                and ext.id in @eids
                order by ext.id;";

            string selectSql = "";
            if (latitude != 0 && longitude != 0)
            {
                selectSql += string.Format(",isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999) as Distance"
                    , longitude, latitude);
            }
            if (readIntro)
            {
                selectSql += ",sch.Intro,ext.ExtIntro";
            }
            var list = dbc.Query<SchoolExtItemDto>(
                string.Format(sql, selectSql),
                new { status = SchoolStatus.Success, eids }
            ).ToList();
            return list;
        }

        /// <summary>
        /// 根据学校ID获取学校分部信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<SchoolExtItemDto> GetSchoolExtListBySchoolId(Guid sid, double latitude = 0, double longitude = 0)
        {

            var sql = @"SELECT 
                 sch.name + '-' + ext.name AS name, sch.id AS sid,ext.id AS extid,
                 charge.tuition,ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,
                 content.Authentication,content.Characteristic,course.courses,content.Lodging,content.sdextern,
                p.name AS province,c.name AS city ,a.name AS area,
                content.province as provincecode ,content.city AS citycode ,content.area AS areacode,
                (select top 1 scores.score from Score AS scores
					where scores.eid=ext.id AND scores.indexid=22
				) as score  ,ext.IsValid as extValid,sch.IsValid as schoolValid, sch.status {0} , ext.No as SchoolNo
                 FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch
                ON ext.sid = sch.id
                LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge
                ON ext.id = charge.eid
                LEFT JOIN dbo.OnlineSchoolExtContent AS content
                ON ext.id = content.eid
                LEFT JOIN OnlineSchoolExtCourse AS course 
				ON ext.id=course.eid
                LEFT JOIN KeyValue AS p On content.province	 = p.id
                LEFT JOIN KeyValue AS c On content.city	 = c.id
                LEFT JOIN KeyValue AS a On content.area	 = a.id
                --LEFT JOIN Score AS scores ON scores.eid=ext.id AND scores.indexid=22
                where 1=1
                and ext.IsValid = 1 AND sch.IsValid = 1 and sch.status=@status
                and sch.id = @sid
                order by ext.id;";

            string selectSql = "";
            if (latitude != 0 && longitude != 0)
            {
                selectSql += string.Format(",isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999) as Distance"
                    , longitude, latitude);
            }

            var list = dbc.Query<SchoolExtItemDto>(
                string.Format(sql, selectSql),
                new { sid, status = SchoolStatus.Success }
            ).ToList();
            return list;
        }


        public List<SchoolExtItemDto> GetSchoolExtFilterList(double longitude, double latitude,
            List<int> province, List<int> city, List<int> area, List<Guid> MetroLineIds, List<int> MetroStationIds,
            List<int> gradeIds, List<int> typeIds, List<bool> discount, List<bool> diglossia, List<bool> chinese,
            int orderBy, decimal distance, int minCost, int maxCost, bool? lodging,
            List<Guid> authIds, List<Guid> characIds, List<Guid> abroadIds, List<Guid> courseIds, int index = 1, int size = 10)
        {
            var tuitionField = " (select top 1 content from OnlineSchoolYearFieldContent where eid=ext.id and field='Tuition' order by year desc) as tuition ,";


            var sql = @"SELECT 
                 sch.name + '-' + ext.name AS name, sch.id AS sid,ext.id AS extid,"
                + tuitionField
                + @"ext.type,ext.grade,ext.discount,ext.diglossia,ext.chinese,
                 isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999) as Distance,
                 content.Authentication,content.Characteristic,course.courses,content.Lodging,content.sdextern,
                p.name AS province,c.name AS city ,a.name AS area,
                content.province as provincecode ,content.city AS citycode ,content.area AS areacode,
                scores.score as score
                 FROM dbo.OnlineSchoolExtension AS ext
                LEFT JOIN dbo.OnlineSchool AS sch
                ON ext.sid = sch.id
                LEFT JOIN  dbo.OnlineSchoolExtCharge AS charge
                ON ext.id = charge.eid
                LEFT JOIN dbo.OnlineSchoolExtContent AS content
                ON ext.id = content.eid
                LEFT JOIN OnlineSchoolExtCourse AS course 
				ON ext.id=course.eid
                LEFT JOIN KeyValue AS p On content.province	 = p.id
                LEFT JOIN KeyValue AS c On content.city	 = c.id
                LEFT JOIN KeyValue AS a On content.area	 = a.id
                LEFT JOIN Score AS scores ON scores.eid=ext.id AND scores.indexid=22
                {2}
                {3}
                {4}
                where 1=1 and ext.IsValid = 1 AND sch.IsValid = 1 and sch.status=@status
                {5}
                order by {6}  OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";


            //where 条件
            StringBuilder where = new StringBuilder();
            //参数
            var para = new DynamicParameters();

            if (gradeIds.Count > 0)
            {
                where.Append(" and ext.grade in @gradeId ");
                para.Add("@gradeId", gradeIds);
            }

            if (typeIds.Count > 0)
            {
                where.Append(" and ext.type in @typeId ");
                para.Add("@typeId", gradeIds);
            }

            if (discount.Count > 0 && diglossia.Count > 0 && chinese.Count > 0)
            {

                where.Append(" and ext.discount in @discount ");
                para.Add("@discount", discount);

                where.Append(" and ext.diglossia in @diglossia ");
                para.Add("@diglossia", diglossia);

                where.Append(" and ext.chinese in @chinese ");
                para.Add("@chinese", chinese);
            }

            if (province.Count > 0)
            {
                where.Append(" and content.province in @provinceId ");
                para.Add("@provinceId", province);
            }
            if (city.Count > 0)
            {
                where.Append(" and content.city in @cityId ");
                para.Add("@cityId", city);
            }
            if (area.Count > 0)
            {
                where.Append(" and content.area in @areaId ");
                para.Add("@areaId", area);
            }

            string metroSql = "";
            if (MetroLineIds.Count > 0)
            {
                para.Add("@lineId", MetroLineIds);
                string metroWhere = " mli.metro_info_id in @lineId ";

                if (MetroStationIds.Count > 0)
                {
                    para.Add("@stationId", MetroStationIds);
                    metroWhere += " and mli.id in @stationId";
                }

                metroSql = string.Format(@"inner join (
                    select smb.eid 
                    from 
                    schoolext_metroline_bind smb
                    inner join metro_line_info mli on mli.id = smb.lid
                    where {0}
                    GROUP BY smb.eid 
                    ) MM on MM.eid = ext.id", metroWhere);
            }



            if (distance > 0)
            {
                where.Append(string.Format(" and isnull(LatLong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326)),999999999)<=@distance", longitude, latitude));
                para.Add("@distance", distance);
            }

            if (minCost + maxCost > 0)
            {
                where.Append(" and charge.tuition BETWEEN @minCost AND @maxCost");
                para.Add("@minCost", minCost);
                para.Add("@maxCost", maxCost);
            }

            if (lodging != null)
            {
                where.Append(" and content.lodging = @lodging ");
                para.Add("@lodging", lodging);
            }

            string tagSql = "";
            if (authIds.Count > 0 || characIds.Count > 0 || abroadIds.Count > 0)
            {
                string tagApply = "";
                string tagWhere = "";

                if (authIds.Count > 0)
                {
                    para.Add("@authId", authIds);

                    tagApply += @"  CROSS APPLY  
                                 OPENJSON (sec.authentication, N'$')  
                                       WITH ([Value] varchar(200) N'$.Value')  
                              AS Auth ";
                    tagWhere += $" and Auth.[Value] in @authId ";
                }
                if (characIds.Count > 0)
                {
                    para.Add("@characId", characIds);

                    tagApply += @"  CROSS APPLY  
                                 OPENJSON (sec.characteristic, N'$')  
                                       WITH ([Value] varchar(200) N'$.Value')  
                              AS Charac ";

                    tagWhere += $" and Charac.[Value] in @characId ";
                }
                if (abroadIds.Count > 0)
                {
                    para.Add("@abroadId", abroadIds);
                    tagApply += @"  CROSS APPLY  
                                 OPENJSON (sec.abroad, N'$')  
                                       WITH ([Value] varchar(200) N'$.Value')  
                              AS Abroad ";

                    tagWhere += $" and Abroad.[Value] in @abroadId ";
                }

                tagSql = string.Format(@"inner join (
                    SELECT sec.eid
                    from 
                    OnlineSchoolExtContent AS sec
                    {0}
                    where 1=1 {1}
                    GROUP BY sec.eid
                    ) T on T.eid = ext.id", tagApply, tagWhere);
            }

            string courseSql = "";
            if (courseIds.Count > 0)
            {
                para.Add("@courseId", courseIds);
                courseSql = @"inner join (
                    SELECT course.eid
                    from 
                    OnlineSchoolExtCourse AS course
                      CROSS APPLY  
                                 OPENJSON (course.courses, N'$')  
                                       WITH ([Value] varchar(200) N'$.Value')  
                              AS Courses 
                    where Courses.[Value] in @courseId
                    GROUP BY course.eid
                    ) TT on TT.eid = ext.id";
            }


            var orderbyStr = " ext.id desc ";
            switch (orderBy)
            {
                case 1:
                    orderbyStr = " Distance desc ";
                    break;
                case 2:
                    orderbyStr = " Distance asc ";
                    break;
                case 3:
                    orderbyStr = " tuition desc ";
                    break;
                case 4:
                    orderbyStr = " tuition asc ";
                    break;
            }
            //学校状态
            para.Add("@status", (int)SchoolStatus.Success);
            para.Add("offset", (index - 1) * size);
            para.Add("limit", size);
            var list = dbc.Query<SchoolExtItemDto>(
                string.Format(sql, longitude, latitude, metroSql, tagSql, courseSql, where.ToString(), orderbyStr),
                para
            ).ToList();
            return list;
        }

        public List<Tags> GetAuthTagsBySchoolType(bool International)
        {
            string sql = @"select [Value] AS DataId, [Key] AS TagName
                            from 
                            OnlineSchoolExtContent AS content
                            CROSS APPLY  
                                 OPENJSON (content.authentication, N'$')  
                                       WITH (
					                             [Key] varchar(200) N'$.Key',
					                             [Value] uniqueidentifier N'$.Value'
					                             )  
                              AS Auth
                            inner join dbo.OnlineSchoolExtension AS ext on ext.id = content.eid {0}
                            GROUP BY [Key],[Value]";
            string where;
            if (International)
            {
                where = " and ext.type = '3'  ";
            }
            else
            {
                where = " and ext.type in ('0','1','2','80','99')  ";
            }
            var list = dbc.Query<Tags>(
                string.Format(sql, where),
                new { }
            ).ToList();
            return list;
        }
        public List<Tags> GetCharacTagsBySchoolType(bool International)
        {
            string sql = @"select [Value] AS DataId, [Key] AS TagName
                            from 
                            OnlineSchoolExtContent AS content
                            CROSS APPLY  
                                 OPENJSON (content.characteristic, N'$')  
                                       WITH (
					                             [Key] varchar(200) N'$.Key',
					                             [Value] uniqueidentifier N'$.Value'
					                        )  
                              AS Charac
                            inner join dbo.OnlineSchoolExtension AS ext on ext.id = content.eid {0}
                            GROUP BY [Key],[Value]";
            string where;
            if (International)
            {
                where = " and ext.type = '3'  ";
            }
            else
            {
                where = " and ext.type in ('0','1','2','80','99')";
            }
            var list = dbc.Query<Tags>(
                string.Format(sql, where),
                new { }
            ).ToList();
            return list;
        }

        /// <summary>
        ///获取升学成绩
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<KeyValueDto<int, string, double, byte, Guid>> GetAchievementList(Guid extId, byte grade)
        {
            StringBuilder sql = new StringBuilder();
            //高中
            if (grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                sql.Append(@"SELECT ach.year AS [key],college.name AS [value],ach.count AS [message],college.type AS [data],college.id as other  FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT JOIN dbo.College AS college ON ach.schoolId = college.id
                WHERE ach.IsValid = 1 AND college.IsValid = 1 AND ach.extId = @extId  ORDER BY ach.CreateTime");
            }
            else
            {
                sql.Append(@"SELECT ach.year AS [key],sch.name AS [value],ach.count AS [message],ext.sid as other
                FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT  JOIN  dbo.OnlineSchoolExtension AS ext ON ach.schoolId =ext.id
                LEFT JOIN dbo.OnlineSchool AS sch ON sch.id=ext.sid
               WHERE ach.extId=@extId AND ach.IsValid=1  AND ext.IsValid=1 
               ORDER BY ach.year,ach.count DESC");
            }

            return dbc.Query<KeyValueDto<int, string, double, byte, Guid>>(sql.ToString(), new { extId }).ToList();

        }

        /// <summary>
        ///获取年份升学成绩
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid, string, double, int>> GetYearAchievementList(Guid extId, byte grade, int year)
        {
            StringBuilder sql = new StringBuilder();
            //高中
            if (grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                sql.Append(@"SELECT ach.schoolId AS [key],college.name AS [value],ach.count AS [message],ach.year AS [data]  FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT JOIN dbo.College AS college ON ach.schoolId=college.id
                WHERE ach.IsValid=1 AND college.IsValid=1 AND ach.extId=@extId and ach.year=@year  ORDER BY ach.year desc");
            }
            else
            {
                sql.Append(@"SELECT ach.schoolId AS [key],sch.name AS [value],ach.count AS [message],ach.year AS [data]
                FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT  JOIN  dbo.OnlineSchoolExtension AS ext ON ach.schoolId =ext.id
                LEFT JOIN dbo.OnlineSchool AS sch ON sch.id=ext.sid
               WHERE ach.extId=@extId AND ach.IsValid=1  AND ext.IsValid=1 and ach.year=@year
               ORDER BY ach.year,ach.count DESC");
            }

            return dbc.Query<KeyValueDto<Guid, string, double, int>>(sql.ToString(), new { extId, year }).ToList();

        }
        /// <summary>
        /// 根据学校id获取学校的分部
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid>> GetSchoolExtName(Guid sid)
        {
            var sql = @"SELECT name AS [key],id AS [value] , no as [year] FROM dbo.OnlineSchoolExtension WHERE sid=@sid AND IsValid=1";
            return dbc.Query<KeyValueDto<Guid>>(sql, new { sid }).ToList();
        }
        /// <summary>
        /// 获取学校附近的学位房
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public List<KeyValueDto<double, double, string>> GetBuildingData(Guid extId, double? latitude, double? longitude, float distance)
        {
            var sql = @"
        SELECT top 20  longitude AS [key],latitude AS [value],name AS message FROM building2
         WHERE latitude IS NOT NULL AND longitude IS NOT NULL AND
          latlong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326))<=@distance  order by  latlong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326))";
            var result = dbc.Query<KeyValueDto<double, double, string>>(string.Format(sql, longitude, latitude), new { distance });
            return result.ToList();
        }
        public List<KeyValueDto<double, double, string, int>> GetBuildingPriceData(Guid extId, double? latitude, double? longitude, float distance)
        {
            var sql = @"
        SELECT top 20  longitude AS [key],latitude AS [value],name AS message,price as [Data] FROM building2
         WHERE latitude IS NOT NULL AND longitude IS NOT NULL AND
          latlong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326))<=@distance  order by  latlong.STDistance(geography::STPointFromText('POINT ({0} {1})', 4326))";
            var result = dbc.Query<KeyValueDto<double, double, string, int>>(string.Format(sql, longitude, latitude), new { distance });
            return result.ToList();
        }
        public List<SmallLocation> GetSchoolExtRangePoints(Guid extId)
        {

            var sql = @"
        SELECT   longitude as Longitude,latitude as Latitude,name as Name FROM SchoolExtRangeMapPoints
         WHERE latitude IS NOT NULL AND longitude IS NOT NULL and eid=@eid";
            var result = dbc.Query<SmallLocation>(sql, new { eid = extId }).ToList();
            //取凸边形
            if (result.Any())
                return new PolygonHelper(result).GetPolygon();
            return new List<SmallLocation>();
        }

        /// <summary>
        /// 根据分部id获取学校的分部地址详情信息
        /// </summary>
        /// <param name="ExtId"></param>
        /// <returns></returns>
        public List<SchoolExtInfoDto> GetSchoolExtInfoDto(List<Guid> ExtId)
        {
            SqlCommand cmd = new SqlCommand(@"select 
	                        e.id as Id,
	                        e.Sid as SchoolId,
	                        e.grade as Grade,
	                        e.type as Type,
	                        s.province as Province,
	                        s.city as City,
	                        s.area as Area,
	                        s.latitude as Latitude,
	                        s.longitude as Longitude
	                        from OnlineSchoolExtension as e
	                        left join OnlineSchoolExtContent as s on e.Id = s.eid
	                        where e.Id in @extIds");
            var para = cmd.AddArrayParameters("ExtId", ExtId, SqlDbType.UniqueIdentifier);
            return dbc.Query<SchoolExtInfoDto>(cmd.CommandText, new { extIds = ExtId })?.ToList();
        }

        /// <summary>
        /// 获取学校特征
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public SchoolExtCharacterDto GetExtCharacter(Guid extId)
        {
            var sql = @"SELECT  schoolcredentials, teacherpower,coursetype,comfort,trafficeasy,safety,
                regionaleconomic,populationdensity,edufund,
                hardware,upgradeeasy,entereasy,cost,totalscore FROM dbo.SchoolCharacter where eid=@eid";
            var result = dbc.Query<SchoolExtCharacterDto>(sql, new { eid = extId });
            if (result.Count() == 0)
                return null;
            else
                return result.FirstOrDefault();
        }

        /// <summary>
        /// 获取学校分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public List<SchoolExtScore> GetExtScore(Guid extId)
        {
            var sql = @"SELECT  [id],[indexid] ,[score] ,[eid] FROM  [Score] where eid=@eid";
            var result = dbc.Query<SchoolExtScore>(sql, new { eid = extId }).ToList();
            return result;
        }
        /// <summary>
        /// 获取有效学校分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SchoolExtScore>> GetValidExtScore(Guid extId)
        {
            var str_SQL = @"SELECT
	                            s.* 
                            FROM
	                            Score AS s
	                            LEFT JOIN ScoreIndex AS si ON si.id = s.indexid 
                            WHERE
	                            si.isvalid = 1 
	                            AND s.eid = @extId";

            return await dbc.QueryAsync<SchoolExtScore>(str_SQL, new { extId });
        }

        /// <summary>
        /// 获取周边分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public AmbientScore GetAmbientScore(Guid extId)
        {
            var sql = @"SELECT [eid]
      ,[museum]
      ,[latitude]
      ,[longitude]
      ,[traininfo]
      ,[view]
      ,[metro]
      ,[play]
      ,[market]
      ,[library]
      ,[shoppinginfo]
      ,[police]
      ,[bookmarket]
      ,[river]
      ,[bus]
      ,[rubbish]
      ,[subway]
      ,[gdid]
      ,[schoolname]
      ,[poiinfo]
      ,[hospital]
      ,[buildingprice]
  FROM [GDParams] where eid=@eid";
            var result = dbc.Query<AmbientScore>(sql, new { eid = extId }).ToList();
            if (result.Count() == 0)
                return null;
            else
                return result.FirstOrDefault();
        }

        /// <summary>
        /// 获取学校分数细项名称
        /// </summary>
        /// <returns></returns>
        public List<SchoolExtScoreIndex> GetExtScoreIndex()
        {
            var sql = @"SELECT  [id] ,[index_name] ,[level],[parentid] FROM [ScoreIndex]";
            var result = dbc.Query<SchoolExtScoreIndex>(sql, new { }).ToList();
            return result;
        }




        /// <summary>
        /// 导出学校数据到ES
        /// </summary>
        /// <returns></returns>
        public List<SchoolDataES> GetSchoolData(int index, int size, DateTime lastTime)
        {
            var sql = @"SELECT  sch.name+' - '+ext.name as Name,sch.name as schoolname,ext.name as extname,
				                    sch.name_e AS EName,ext.id as Id,ext.sid as SchoolId,content.latitude,content.longitude,
				                    content.city AS cityCode,content.area AS areaCode,c.name AS city ,a.name AS area,
				                    charge.tuition,ext.grade, ext.type,ext.discount,ext.diglossia,ext.chinese,ext.SchFtype,
                                    content.Lodging,content.sdextern,content.Canteen,
                                    content.studentcount,content.teachercount,
				                    content.Authentication,content.Characteristic,content.abroad,course.courses,ttt.tagName ,
				                    metro_info_id as MetroLineId,lid as MetroStationId,ext.ModifyDateTime as UpdateTime,
                                    ext.IsValid,sch.status,score15,score16,score17,score18,score19,score20,score21,score22
                    FROM dbo.OnlineSchoolExtension AS ext
                    LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
                    LEFT JOIN dbo.OnlineSchoolExtCharge AS charge ON ext.id = charge.eid
                    LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
                    LEFT JOIN OnlineSchoolExtCourse AS course ON ext.id=course.eid
                    LEFT JOIN KeyValue AS c On content.city	 = c.id
                    LEFT JOIN KeyValue AS a On content.area	 = a.id
                    left join (SELECT t2.dataID, tagName = STUFF((
                                SELECT ',' + tag.name
                                FROM GeneralTagBind AS tagBind
                                inner join GeneralTag tag on tagBind.tagID = tag.id
                                WHERE tagBind.dataID = t2.dataID 
                                FOR XML PATH('')
                                ), 1, 1, '') 
                    FROM GeneralTagBind AS t2
                    where  t2.dataType = 2 and t2.dataID is not null
                    GROUP BY t2.dataID) AS ttt on (ttt.dataID = ext.id or ttt.dataID = ext.sid)
                    left join (SELECT  p2.eid, lid = STUFF((
                                SELECT ',' + CONVERT(varchar,lid)
                                FROM schoolext_metroline_bind AS p1
                                WHERE p1.eid = p2.eid
                                FOR XML PATH('')
                                ), 1, 1, '') FROM schoolext_metroline_bind AS p2
                    GROUP BY p2.eid) AS msb on msb.eid = ext.id
                    left join (SELECT  p2.eid, metro_info_id =
						                    STUFF((
						                    SELECT ','+ CONVERT(varchar(36),cast(m.metro_info_id AS char(255)))
						                    FROM schoolext_metroline_bind AS p1
						                    inner join metro_line_info AS m on m.id = p1.lid
						                    WHERE p1.eid = p2.eid
						                    GROUP BY m.metro_info_id
						                    FOR XML PATH('')
						                    ), 1, 1, '') FROM schoolext_metroline_bind AS p2
						                    GROUP BY p2.eid) AS ms on ms.eid = ext.id
                    left join (
                        select  scr.eid,
                        avg(CASE WHEN scr.indexid = 15 THEN scr.score ELSE null END) AS score15,
                        avg(CASE WHEN scr.indexid = 16 THEN scr.score ELSE null END) AS score16,
                        avg(CASE WHEN scr.indexid = 17 THEN scr.score ELSE null END) AS score17,
                        avg(CASE WHEN scr.indexid = 18 THEN scr.score ELSE null END) AS score18,
                        avg(CASE WHEN scr.indexid = 19 THEN scr.score ELSE null END) AS score19,
                        avg(CASE WHEN scr.indexid = 20 THEN scr.score ELSE null END) AS score20,
                        avg(CASE WHEN scr.indexid = 21 THEN scr.score ELSE null END) AS score21,
                        avg(CASE WHEN scr.indexid = 22 THEN scr.score ELSE null END) AS score22
                        from Score scr 
                        where scr.status = 1 
						GROUP BY scr.eid
                    ) as score on score.eid = ext.id
                    --where content.city in ('440100','310100')
                    where 1=1 and content.latitude <= 90 and ext.ModifyDateTime > @lastTime {0}
                    --and ext.id = '24d02767-4196-429b-b9b9-04e1089a495e'
                    order by ext.ModifyDateTime OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";

            string where = "";
            if (lastTime == Convert.ToDateTime("1999-01-01"))
            {
                where += " and ext.IsValid = 1 and sch.status = 3 ";
            }

            var result = dbc.Query<SchoolDataES>(string.Format(sql, where), new
            {
                offset = index * size,
                limit = size,
                lastTime
            }, commandTimeout: 10000);
            return result.ToList();
        }

        public List<SchoolDataES> GetSchoolDataBySchoolId(Guid schoolId)
        {
            var sql = @"SELECT  sch.name+' - '+ext.name as Name,sch.name as schoolname,ext.name as extname,
				                    sch.name_e AS EName,ext.id as Id,ext.sid as SchoolId,content.latitude,content.longitude,
				                    content.city AS cityCode,content.area AS areaCode,c.name AS city ,a.name AS area,
				                    charge.tuition,ext.grade, ext.type,ext.discount,ext.diglossia,ext.chinese,ext.SchFtype,
                                    content.Lodging,content.sdextern,content.Canteen,
                                    content.studentcount,content.teachercount,
				                    content.Authentication,content.Characteristic,content.abroad,course.courses,ttt.tagName ,
				                    metro_info_id as MetroLineId,lid as MetroStationId,ext.ModifyDateTime as UpdateTime,
                                    ext.IsValid,sch.status,score15,score16,score17,score18,score19,score20,score21,score22
                    FROM dbo.OnlineSchoolExtension AS ext
                    LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
                    LEFT JOIN dbo.OnlineSchoolExtCharge AS charge ON ext.id = charge.eid
                    LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
                    LEFT JOIN OnlineSchoolExtCourse AS course ON ext.id=course.eid
                    LEFT JOIN KeyValue AS c On content.city	 = c.id
                    LEFT JOIN KeyValue AS a On content.area	 = a.id
                    left join (SELECT t2.dataID, tagName = STUFF((
                                SELECT ',' + tag.name
                                FROM GeneralTagBind AS tagBind
                                inner join GeneralTag tag on tagBind.tagID = tag.id
                                WHERE tagBind.dataID = t2.dataID 
                                FOR XML PATH('')
                                ), 1, 1, '') 
                    FROM GeneralTagBind AS t2
                    where  t2.dataType = 2 and t2.dataID is not null
                    GROUP BY t2.dataID) AS ttt on (ttt.dataID = ext.id or ttt.dataID = ext.sid)
                    left join (SELECT  p2.eid, lid = STUFF((
                                SELECT ',' + CONVERT(varchar,lid)
                                FROM schoolext_metroline_bind AS p1
                                WHERE p1.eid = p2.eid
                                FOR XML PATH('')
                                ), 1, 1, '') FROM schoolext_metroline_bind AS p2
                    GROUP BY p2.eid) AS msb on msb.eid = ext.id
                    left join (SELECT  p2.eid, metro_info_id =
						                    STUFF((
						                    SELECT ','+ CONVERT(varchar(36),cast(m.metro_info_id AS char(255)))
						                    FROM schoolext_metroline_bind AS p1
						                    inner join metro_line_info AS m on m.id = p1.lid
						                    WHERE p1.eid = p2.eid
						                    GROUP BY m.metro_info_id
						                    FOR XML PATH('')
						                    ), 1, 1, '') FROM schoolext_metroline_bind AS p2
						                    GROUP BY p2.eid) AS ms on ms.eid = ext.id
                    left join (
                        select  scr.eid,
                        avg(CASE WHEN scr.indexid = 15 THEN scr.score ELSE null END) AS score15,
                        avg(CASE WHEN scr.indexid = 16 THEN scr.score ELSE null END) AS score16,
                        avg(CASE WHEN scr.indexid = 17 THEN scr.score ELSE null END) AS score17,
                        avg(CASE WHEN scr.indexid = 18 THEN scr.score ELSE null END) AS score18,
                        avg(CASE WHEN scr.indexid = 19 THEN scr.score ELSE null END) AS score19,
                        avg(CASE WHEN scr.indexid = 20 THEN scr.score ELSE null END) AS score20,
                        avg(CASE WHEN scr.indexid = 21 THEN scr.score ELSE null END) AS score21,
                        avg(CASE WHEN scr.indexid = 22 THEN scr.score ELSE null END) AS score22
                        from Score scr 
                        where scr.status = 1 
						GROUP BY scr.eid
                    ) as score on score.eid = ext.id
                    where 1=1 and ext.sid = @schoolId;";

            var result = dbc.Query<SchoolDataES>(sql, new { schoolId });
            return result.ToList();
        }

        public bool InsertEESchoolData(bool isFirst)
        {
            var truncateSql = "truncate table Lyega_allSchextSimpleInfo;";
            var sql = @"
            insert into Lyega_allSchextSimpleInfo
            select e.id as eid,e.sid,s.name+'-'+e.name as name,
            e.grade,e.type,e.discount,e.diglossia,e.chinese,
            (case when e.IsValid = 0 or s.IsValid = 0 then '已下线'
            when  s.status=0 then '初始状态' when s.status=1 then '编辑中'
            when s.status=2 then '审核中' when s.status=3 then '成功'
            when s.status=4 then '需修改' else null end)as status,
            k1.name as province,k2.name as city,k3.name as area,
(case when os.id is null then e.ModifyDateTime else os.ModifyDateTime end) as Time
            from dbo.School s
            left join dbo.OnlineSchool os on os.id = s.id 
            inner join dbo.SchoolExtension e on e.sid=s.id
            left join dbo.SchoolExtContent c on c.eid=e.id and c.IsValid=1
            left join dbo.KeyValue k1 on k1.id=c.province and k1.type=1
            left join dbo.KeyValue k2 on k2.id=c.city and k2.type=1
            left join dbo.KeyValue k3 on k3.id=c.area and k3.type=1
            {0} ;";

            string extsql = "";
            if (!isFirst)
            {
                extsql = " where s.IsValid=1 and e.IsValid=1";
            }

            var trunResult = dbc.Execute(truncateSql, new { }) > 0;
            var result = dbc.Execute(string.Format(sql, extsql), new { }) > 0;
            return result;
        }

        public List<EESchool> GetEESchoolData(int pageIndex, int pageSize, DateTime time)
        {
            var sql = @"select eid as id,sid,name,status,grade,type,
            province,city,area,Time
            from Lyega_allSchextSimpleInfo where Time >= @time --and Time < @endtime
            order by Time OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";


            List<SqlParameter> para = new List<SqlParameter>
            {
                new SqlParameter("offset", pageIndex * pageSize),
                new SqlParameter("limit", pageSize),
                new SqlParameter("time", time),
                //new SqlParameter("endtime", Convert.ToDateTime("2019-09-01")),
            };
            var result = dbc.Query<EESchool>(sql, new
            {
                offset = pageIndex * pageSize,
                limit = pageSize,
                time
            });
            return result.ToList();
        }

        public List<SchoolCQDto> QuerySchoolCard(bool QueryComment, int City, int PageIndex, int PageSize)
        {
            var para = new DynamicParameters();
            string sql = @"select s.SchoolSectionId,
			                 s.SchoolId,
			                 s.AggScore,
			                 s.CommentCount,
			                 s.QuestionCount,
			                 l.SchName+' - '+l.Extname as SchoolName,
			                 l.lodging,l.sdextern,
			                 l.type as Type
                            from SchoolScores as s
	                LEFT JOIN Lyega_OLschextSimpleInfo as l
	                on l.eid = s.SchoolSectionId";

            if (City > 0)
            {
                sql += " where l.city = @city ";
                para.Add("@city", City);
            }

            if (QueryComment)
            {
                sql += " order by s.CommentCount desc ";
            }
            else
            {
                sql += " order by s.QuestionCount desc ";
            }

            sql += @"OFFSET @pageindex ROWS FETCH NEXT @pagesize ROWS ONLY";
            para.Add("@pageindex", (PageIndex - 1) * PageSize);
            para.Add("@pagesize", PageSize);

            return dbc.Query<SchoolCQDto>(sql, para)?.ToList();
        }

        /// <summary>
        /// 获取评分学部信息
        /// </summary>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <param name="scoreType">1.评论 2.学校</param>
        /// <returns></returns>
        public List<KeyValueDto<Guid, byte, long, Guid>> GetScoreExtData(int city, int top, byte scoreType)
        {
            StringBuilder sql = new StringBuilder();
            if (scoreType == 1)
            {
                sql.Append($@" SELECT * FROM(
                SELECT ext.id as [key],ext.grade as [value],message=ROW_NUMBER() OVER(PARTITION BY ext.grade ORDER BY score.score desc),ext.sid as Data FROM dbo.OnlineSchoolExtension AS ext 
                LEFT JOIN dbo.Score AS score ON ext.id=score.eid  
                LEFT JOIN  dbo.OnlineSchoolExtContent AS content ON ext.id=content.eid 
                LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
                WHERE {(city == 0 ? " " : "content.city=@city and")} ext.IsValid=1 AND sch.IsValid=1 AND sch.status=@status)a WHERE a.message<=@top");
            }
            else
            {
                sql.Append($@"SELECT * FROM(
                SELECT ext.id as [key],ext.grade as value,message=ROW_NUMBER() OVER(PARTITION BY ext.grade ORDER BY score.AggScore DESC),ext.sid as Data FROM dbo.OnlineSchoolExtension AS ext 
				LEFT JOIN dbo.SchoolScores AS score ON score.SchoolSectionId=ext.id
                LEFT JOIN  dbo.OnlineSchoolExtContent AS content ON ext.id=content.eid 
                LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid=sch.id
                WHERE {(city == 0 ? " " : "content.city=@city and")} ext.IsValid=1 AND sch.IsValid=1 AND sch.status=@status)a WHERE a.message<=@top");

            }
            var list = dbc.Query<KeyValueDto<Guid, byte, long, Guid>>(sql.ToString(), new { status = (byte)SchoolStatus.Success, top, city }).ToList();
            return list;
        }

        public bool InsertSchoolScore()
        {
            var truncateSql = "truncate table Lyega_SchoolScore;";
            var sql = @"
                        insert into Lyega_SchoolScore
                        select  ext.id as eid,
                        avg(CASE WHEN scr.indexid = 15 THEN scr.score ELSE null END) AS score15,
                        avg(CASE WHEN scr.indexid = 16 THEN scr.score ELSE null END) AS score16,
                        avg(CASE WHEN scr.indexid = 17 THEN scr.score ELSE null END) AS score17,
                        avg(CASE WHEN scr.indexid = 18 THEN scr.score ELSE null END) AS score18,
                        avg(CASE WHEN scr.indexid = 19 THEN scr.score ELSE null END) AS score19,
                        avg(CASE WHEN scr.indexid = 20 THEN scr.score ELSE null END) AS score20,
                        avg(CASE WHEN scr.indexid = 22 THEN scr.score ELSE null END) AS score22
                        --into Lyega_SchoolScore
                        from OnlineSchoolExtension ext
                        inner join Score scr on ext.id = scr.eid
                        left join OnlineSchool sch on sch.id = ext.sid
                        where scr.status = 1 AND ext.IsValid = 1 AND sch.IsValid = 1 AND sch.status = 3
                        GROUP BY ext.id ;";

            var trunResult = dbc.Execute(truncateSql, new { }) > 0;
            var result = dbc.Execute(sql, new { }) > 0;
            return result;
        }

        public List<SchoolScoreData> GetSchoolScoreData(int pageNo, int pageSize)
        {
            var sql = @"select eid,score15,score16,score17,score18,score19,score20,score22
            from Lyega_SchoolScore --where eid = '85c4fe43-a0f5-4586-bbca-0b3f842f1f26'
            order by eid OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";


            var result = dbc.Query<SchoolScoreData>(sql, new { offset = pageNo * pageSize, limit = pageSize });
            return result.ToList();
        }

        public List<DisparityDto> GetSchoolDisparities(List<Guid> extIds, double longitude, double latitude)
        {
            string sql = string.Format(@"select eid as ExtId,Round((isnull( LatLong.STDistance ( geography :: STPointFromText ( 'POINT ({0} {1})', 4326 )), 999999999 ) / 1000),1) as Distance 
                    from OnlineSchoolExtContent where eid in @extIds ;", longitude, latitude);

            return dbc.Query<DisparityDto>(sql, new { extIds })?.ToList();
        }


        public List<SchoolSearchImport> SchoolSearchImport(int PageIndex, DateTime CreateTime)
        {
            string DateCondition = "";

            if (CreateTime != default)
            {
                DateCondition = " and sch.CreateTime = @CreateTime";
            }

            string sql = @"	
                        select 
			                sch.Id,
			                sch.name,
			                sch.CreateTime,
			                sch.Creator,
			                (
				                select ext.id,ext.name,ext.grade,ext.type,con.province,con.city,con.area 
					                from SchoolExtension as ext
					                inner join SchoolExtContent as con on ext.id = con.eid
					                where ext.sid = sch.id and ext.IsValid = 1
					                 FOR JSON PATH
			                ) as ExtDetail,
			                (
				                select top 1 Modifier,Status from SchoolAudit where sid = sch.Id ORDER BY CreateTime desc  FOR JSON PATH
			                ) as AuditDetail
		                from School as sch
											where sch.IsValid = 1 " + DateCondition + @"
			                order by CreateTime  asc 
			                OFFSET (@PageIndex - 1) * 1000 ROWS FETCH NEXT 1000 ROWS ONLY";

            if (CreateTime != default)
            {
                return dbc.Query<SchoolSearchImport>(sql, new { PageIndex, CreateTime })?.ToList();
            }
            else
            {
                return dbc.Query<SchoolSearchImport>(sql, new { PageIndex })?.ToList();
            }
        }

        public List<SchoolSearchImport> BDSchDataSearch(List<Guid> SchId)
        {
            string sql = @"	
                        select 
			                sch.Id,
			                sch.name,
			                sch.CreateTime,
			                sch.Creator,
			                (
				                select ext.id,ext.name,ext.grade,ext.type,con.province,con.city,con.area 
					                from SchoolExtension as ext
					                inner join SchoolExtContent as con on ext.id = con.eid
					                where ext.sid = sch.id and ext.IsValid = 1
					                 FOR JSON PATH
			                ) as ExtDetail,
			                (
				                select top 1 Modifier,Status from SchoolAudit where sid = sch.Id ORDER BY CreateTime desc  FOR JSON PATH
			                ) as AuditDetail
		                from School as sch
											where sch.IsValid = 1 and sch.id in @SchId
			                ";
            return dbc.Query<SchoolSearchImport>(sql, new { SchId })?.ToList();
        }

        public SchoolFeedback SchoolFeedback(Guid eid)
        {
            string sql = @"select 
	                    (s.name + '-' + e.name) as name,
	                    c.address,
	                    e.grade,
                        e.type,
                        e.SchFType,
						c.latitude as Lat,
						c.longitude as Lng
                    from OnlineSchoolExtension as e
	                    left join OnlineSchoolExtContent as c on e.id = c.eid
	                    left join OnlineSchool as s on s.id = e.sid
	                    where e.id = @eid";

            return dbc.Query<SchoolFeedback>(sql, new { eid }).FirstOrDefault();
        }

        public SchoolExtension GetSchoolByNo(long no)
        {
            var sql = $@"
select 
    e.id as Id,
    e.No as SchoolNo,
    e.sid as SchoolId,
    c.Sdextern,
    c.Lodging,
    e.Type as SchoolType,
    s.name+'-'+e.name as SchoolName
from 
    OnlineSchoolExtension as e
    left join OnlineSchool as  s on e.Sid = s.Id
    left join SchoolScores as o on o.SchoolSectionId = e.id
	left join OnlineSchoolExtContent as c on c.Eid = e.Id
where e.No = @no";

            return dbc.Query<SchoolExtension>(sql, new { no }).FirstOrDefault();

        }


        public async Task<IEnumerable<(long No, Guid ExtId)>> GetExtIdByNosAsync(IEnumerable<long> nos)
        {
            if (nos == null || !nos.Any())
            {
                return Enumerable.Empty<(long No, Guid ExtId)>();
            }

            var inParamSql = string.Join(",", nos.Select(s => '\'' + s.ToString() + '\''));
            var sql = $@"
select e.No, e.Id from OnlineSchoolExtension as e 
where e.No in ({inParamSql})";

            return  await dbc.QueryAsync<(long, Guid)>(sql, new { nos });

        }

        public async System.Threading.Tasks.Task<List<SchoolImageDto>> GetSchoolImages(Guid eid, int[] type)
        {
            var str_SQL = @"SELECT
                                url,
	                            surl,
	                            imageDesc,
	                            type,
                                sort
                            FROM
                                [dbo].[OnlineSchoolImage]
                            WHERE
                                IsValid = 1
                                And eid = @eid";
            if (type?.Any() == true)
            {
                str_SQL += $" And type in ({string.Join(",", type)})";
            }
            var result = await dbc.QueryAsync<SchoolImageDto>(str_SQL, new { eid });
            return result?.ToList();
        }

        public async Task<List<KeyValueDto<DateTime, string, byte, string, string>>> GetSchoolVideos(Guid extId, byte type = 0)
        {
            var sql = @"SELECT
	                        CreateTime AS [key],
	                        videoUrl AS [value],
	                        type AS message ,
                            videoDesc as [Data],
                            cover as [Other]
                        FROM
	                        dbo.OnlineSchoolVideo 
                        WHERE
	                        eid = @eid 
	                        AND IsValid = 1";
            if (type != 0)
            {
                sql += $"and type={type}";
            }
            var result = await dbc.QueryAsync<KeyValueDto<DateTime, string, byte, string, string>>(sql, new { eid = extId });
            return result?.ToList();
        }

        public async Task<string> GetCounterPartByEID(Guid eid)
        {
            var str_SQL = $"Select [counterpart] From [OnlineSchoolExtContent] Where [eid] = @eid";
            return await dbc.QuerySingleAsync<string>(str_SQL, new { eid });
        }

        public async Task<IEnumerable<KeyValuePair<Guid, SchoolGrade>>> GetGrades(IEnumerable<Guid> eids)
        {
            var str_SQL = $"Select ID as [Key],grade as [Value] From OnlineSchoolExtension Where id in @eids;";
            return await dbc.QueryAsync<KeyValuePair<Guid, SchoolGrade>>(str_SQL, new { eids });
        }

        public async Task<IEnumerable<KeyValuePair<Guid, (int, int)>>> GetCityAndAreaByEIDs(IEnumerable<Guid> eids)
        {
            var str_SQL = $@"SELECT
	                            EID,
                                City,
                                Area
                            FROM
	                            OnlineSchoolExtContent
                            Where Eid in @eids ;";
            var finds = await dbc.QueryAsync<(Guid, int, int)>(str_SQL, new { eids });
            if (finds?.Any() == true)
            {
                return finds.Select(p => new KeyValuePair<Guid, (int, int)>(p.Item1, (p.Item2, p.Item3)));
            }
            return null;
        }

        /// <summary>
        /// 获取学校基本信息
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        //public SchoolExtension GetSchoolPartial(Guid eid) 
        //{
        //    string sql = @"select e.Id,
        //                 e.sid as SchoolId,
        //                 e.grade as SchoolGrade,
        //                 e.type as SchoolType,
        //                 s.name+'-'+e.name as SchoolName,
        //                 c.lodging as Lodging
        //                from OnlineSchoolExtension as e
        //                 left join OnlineSchool as s on e.sid = s.Id
        //                 left join OnlineSchoolExtContent as c on c.eid = e.Id
        //                where e.Id = @eid";

        //    SqlParameter[] para = {
        //        new SqlParameter("@eid",eid)
        //    };
        //    return dbc.Query<SchoolExtension>(sql, para).FirstOrDefault();
        //}


        public async Task<IEnumerable<SchoolExtAggDto>> GetSchoolExtAggs(IEnumerable<Guid> extIds)
        {
            var sql = $@"
select 
    e.id as ExtId,
    s.name+'-'+e.name as SchoolName,
    e.Type as SchoolType,
    e.No as SchoolNo,
    e.sid as SchoolId,
    c.Sdextern,
    c.Lodging,
    (
        select top 1 scores.score from Score AS scores
		where scores.eid=e.id AND scores.indexid=22
	) as score 
from 
    OnlineSchoolExtension as e
    left join OnlineSchool as s on e.Sid = s.Id
    left join OnlineSchoolExtContent as c on c.Eid = e.Id
WHERE 
    e.id in @extIds AND s.IsValid=1 AND e.IsValid=1 and s.status=@status
";
            return await dbc.QueryAsync<SchoolExtAggDto>(sql, new { extIds, status = (int)SchoolStatus.Success });
        }


        public async Task<IEnumerable<Guid>> GetAvailableExtIds(IEnumerable<Guid> extIds)
        {
            var sql = $@"
select 
    e.id as ExtId
from 
    OnlineSchoolExtension as e
WHERE
    e.id in @extIds AND e.IsValid=1
";
            return await dbc.QueryAsync<Guid>(sql, new { extIds });
        }


        public async Task<IEnumerable<string>> GetTagTypes(Guid eid) {
            string sql = @"DECLARE @coursesjson NVARCHAR(MAX)
SELECT @coursesjson = courses  FROM OnlineSchoolExtCourse
		where eid = @eid  and ISJSON (courses)= 1
SELECT * into #temptags FROM OPENJSON(@coursesjson)  
WITH (   
	[Key]   varchar(200) '$.Key' ,
	[Value]   varchar(200) '$.Value' 
) 

SELECT [name] FROM TagType
WHERE 
TagType.IsValid =1
AND
EXISTS(
	SELECT 1 FROM Tag 
	WHERE 
	TagType.id = TAG.[type] 
	AND 
	EXISTS(SELECT 1 FROM #temptags WHERE #temptags.[Value] = Tag.id )
	)";
         return   (await dbc.QueryAsync(sql, new { eid })).Select(s => (string)s.name);
        }
    }

}

#region ef 生成in 语句跟参数

/// <summary>
/// ef 生成in 语句跟参数
/// </summary>

public static class SqlCommandExt
{

    /// <summary>
    /// This will add an array of parameters to a SqlCommand. This is used for an IN statement.
    /// Use the returned value for the IN part of your SQL call. (i.e. SELECT * FROM table WHERE field IN ({paramNameRoot}))
    /// </summary>
    /// <param name="cmd">The SqlCommand object to add parameters to.</param>
    /// <param name="paramNameRoot">What the parameter should be named followed by a unique value for each value. This value surrounded by {} in the CommandText will be replaced.</param>
    /// <param name="values">The array of strings that need to be added as parameters.</param>
    /// <param name="dbType">One of the System.Data.SqlDbType values. If null, determines type based on T.</param>
    /// <param name="size">The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.</param>
    public static SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, string paramNameRoot, IEnumerable<T> values, SqlDbType? dbType = null, int? size = null)
    {
        /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
         * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
         * IN statement in the CommandText.
         */
        var parameters = new List<object>();
        var parameterNames = new List<string>();
        var paramNbr = 1;
        foreach (var value in values)
        {
            var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
            parameterNames.Add(paramName);
            SqlParameter p = new SqlParameter(paramName, value);
            if (dbType.HasValue)
                p.SqlDbType = dbType.Value;
            if (size.HasValue)
                p.Size = size.Value;
            cmd.Parameters.Add(p);
            parameters.Add(p);
        }

        cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(",", parameterNames));

        return parameters.Select(x => ((ICloneable)x).Clone()).Select(p => (SqlParameter)p).ToArray();
    }

}
#endregion

