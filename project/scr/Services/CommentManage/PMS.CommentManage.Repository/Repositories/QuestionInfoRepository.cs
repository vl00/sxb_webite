using PMS.CommentsManage.Domain;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Domain.Query;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class QuestionInfoRepository : EntityFrameworkRepository<QuestionInfo>, IQuestionInfoRepository
    {
        private readonly EntityFrameworkRepository<QuestionInfo> efRepository;


        private readonly CommentsManageDbContext _dbContext;

        public QuestionInfoRepository(CommentsManageDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;


        }

        public new int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public new IEnumerable<QuestionInfo> GetList(Expression<Func<QuestionInfo, bool>> where = null)
        {
            return base.GetList(where);
        }

        public QuestionInfo GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public List<QuestionInfo> GetQuestionInfoByIds(List<Guid> Ids)
        {
            return base.GetList(x => Ids.Contains(x.Id))?.ToList();
        }
        public QuestionInfo GetQuestionByNo(long no)
        {
            return base.GetList(x => x.No == no).FirstOrDefault();
        }

        public int Insert(QuestionInfo model)
        {
            return base.Add(model);
        }

        public bool isExists(Expression<Func<QuestionInfo, bool>> where)
        {
            return base.GetList(where) == null;
        }

        public List<QuestionInfo> NewestSelectedQuestion(Guid BranchSchoolId)
        {
            //最新问题
            var NewQuestion = GetList(x => x.SchoolId == BranchSchoolId).OrderBy(x => x.CreateTime).FirstOrDefault();
            //点赞数 + 回答数最高
            //var SelectedQuestion = GetList(x=>x.SchoolId == BranchSchoolId).Select(x=>x.)

            return null;
        }

        public int TotalQuestion(List<Guid> SchoolIds)
        {
            //List<Guid> SchoolIds = new List<Guid>();

            //根据学校分部id得到学校id
            //Guid School = _Repository.GetSchoolExtension(SchoolId).SchoolId;
            //该分部下所有学校
            //SchoolIds.AddRange(_Repository.GetAllSchoolBranch(School).Select(x => x.Id));

            return base.GetCount(x => SchoolIds.Contains(x.SchoolSectionId));
        }



        public List<SchoolTotal> SchoolTotalQuestion(List<Guid> SchoolIds)
        {
            List<SqlParameter> para = SchoolIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", SchoolIds.Select((q, idx) => "@id" + idx));

            string sql = @"select SchoolId AS id, count(1) as total from QuestionInfos 
                            where 
                            SchoolId IN (" + strpara + @")
                            GROUP BY SchoolId;";

            return Query<SchoolTotal>(sql, para.ToArray())?.ToList();
        }
        public List<SchoolTotal> SchoolSectionTotalQuestion(List<Guid> SchoolExtIds)
        {
            List<SqlParameter> para = SchoolExtIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", SchoolExtIds.Select((q, idx) => "@id" + idx));

            string sql = @"select SchoolSectionId AS id, count(1) as total from QuestionInfos 
                            where 
                            SchoolSectionId IN (" + strpara + @")
                            GROUP BY SchoolSectionId;";
            return Query<SchoolTotal>(sql, para.ToArray())?.ToList();
        }


        public new int Update(QuestionInfo model)
        {
            return base.Update(model);
        }

        public List<SchoolQuestionTotal> CurrentQuestionTotalBySchoolId(Guid SchoolId)
        {

            //CurrentQuestionTotal questionTotal = new CurrentQuestionTotal();

            //List<Guid> SchoolIds = new List<Guid>();

            //根据学校分部id得到学校id
            //Guid School = _Repository.GetSchoolExtension(SchoolId).SchoolId;
            //////该分部下所有学校
            //var AllExtension = _Repository.GetSchoolExtName(School);
            //SchoolIds.AddRange(_Repository.GetAllSchoolBranch(School).Select(x => x.Id));

            string sql = @"
                            select 1 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos
                            where SchoolId = @SchoolId and QuestionInfos.State in (0,1,2,3)
                            UNION
                            select 2 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and CONTAINS(Content,'辟谣') and QuestionInfos.State in (0,1,2,3)
                            union
                            select 3 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and CONTAINS(Content,'师资') and QuestionInfos.State in (0,1,2,3)
                            union
                            select 4 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and CONTAINS(Content,'硬件') and QuestionInfos.State in (0,1,2,3)
                            union
                            select 5 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and CONTAINS(Content,'环境') and QuestionInfos.State in (0,1,2,3)
                            union
                            select 6 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and CONTAINS(Content,'学分') and QuestionInfos.State in (0,1,2,3)
                            union
                            select 7 as TotalType,count(1) as Total,'00000000-0000-0000-0000-000000000000' as SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and CONTAINS(Content,'校园') and QuestionInfos.State in (0,1,2,3)
                            union
                            select 8 as TotalType,count(1) as Total,SchoolSectionId from QuestionInfos where SchoolId = @SchoolId and QuestionInfos.state in (0,1,2,3) group by SchoolSectionId;
                            ";

            SqlParameter[] para = {
                new SqlParameter("@SchoolId",SchoolId)
            };
            return Query<SchoolQuestionTotal>(sql, para)?.ToList();
            //List<SchoolQuestionTotal> CurrentSchoolTotalInfo = Query<SchoolQuestionTotal>(sql, para)?.ToList();

            //for (int i = 0; i < AllExtension.Count(); i++)
            //{
            //    var item = CurrentSchoolTotalInfo.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
            //    int currentIndex = CurrentSchoolTotalInfo.IndexOf(item);
            //    if (item == null)
            //    {
            //        CurrentSchoolTotalInfo.Add(new SchoolQuestionTotal() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryQuestion.Other });
            //    }
            //    else
            //    {
            //        item.Name = AllExtension[i].Key;
            //        CurrentSchoolTotalInfo[currentIndex] = item;
            //    }
            //}
            //return CurrentSchoolTotalInfo;
        }

        public List<QuestionInfo> PageQuestionByUserId(Guid userId, bool isself, int pageIndex, int pageSize)
        {
            IEnumerable<QuestionInfo> question;
            question = base.GetList(s => s.UserId == userId && (s.State == ExamineStatus.Unknown || s.State == ExamineStatus.Unread || s.State == ExamineStatus.Readed || s.State == ExamineStatus.Highlight));

            if (!isself)
            {
                question = question.Where(x => x.IsAnony == false);
            }

            return question.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<QuestionInfo> PageQuestionByQuestionIds(List<Guid> questionIds, bool isself)
        {
            IEnumerable<QuestionInfo> question;
            question = base.GetList(s => questionIds.Contains(s.Id) && (s.State == ExamineStatus.Unknown || s.State == ExamineStatus.Unread || s.State == ExamineStatus.Readed || s.State == ExamineStatus.Highlight));

            if (!isself)
            {
                question = question.Where(x => x.IsAnony == false);
            }

            return question.OrderByDescending(x => x.CreateTime).ToList();
        }

        /// <summary>
        /// 最新问题查询
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="query"></param>
        /// <param name="Order"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<QuestionInfo> PageSchoolCommentBySchoolSectionIds(Guid schoolId, QueryQuestion query, SelectedQuestionOrder Order, int pageIndex, int pageSize, out int total)
        {
            IEnumerable<QuestionInfo> question;

            List<int> showState = new List<int> { 0, 1, 2, 3 };

            if (query == QueryQuestion.Other)
            {
                question = base.GetList(s => s.SchoolSectionId == schoolId && showState.Contains((int)s.State));
            }
            else
            {
                question = base.GetList(s => s.SchoolId == schoolId && showState.Contains((int)s.State));
            }

            switch (query)
            {
                case QueryQuestion.Envir:
                    question = question.Where(x => x.Content.Contains("环境"));
                    break;
                case QueryQuestion.Hard:
                    question = question.Where(x => x.Content.Contains("硬件"));
                    break;
                case QueryQuestion.Life:
                    question = question.Where(x => x.Content.Contains("校园"));
                    break;
                case QueryQuestion.Manage:
                    question = question.Where(x => x.Content.Contains("学分"));
                    break;
                case QueryQuestion.Teacher:
                    question = question.Where(x => x.Content.Contains("师资"));
                    break;
                case QueryQuestion.Rumor:
                    question = question.Where(x => x.Content.Contains("辟谣"));
                    break;
            }
            if ((int)Order == 0)
            {
                question = question.OrderByDescending(x => x.LikeCount + x.ReplyCount);
            }
            else if ((int)Order == 3)
            {
                question = question.OrderByDescending(x => x.ReplyCount);
            }
            else
            {
                question = question.OrderByDescending(x => x.CreateTime);
            }
            total = question.Count();
            return question.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        //public CurrentCommentTotal CurrentCommentTotalBySchoolId(Guid SchoolId)
        //{
        //    SqlParameter[] para = {
        //        new SqlParameter("@SchoolId",SchoolId)
        //    };
        //    CurrentCommentTotal commentTotal = new CurrentCommentTotal();

        //    string sql = "select count(1) from SchoolComments as s where SchoolSectionId = @SchoolId and (State = 3 or (select count(1) from SchoolCommentReplies where SchoolCommentId = s.Id) >= 1)";
        //    commentTotal.SelectedTotal = (int)ExecuteScalar(sql, para);

        //    sql = "select count(1) from SchoolComments where SchoolSectionId = @SchoolId";
        //    commentTotal.Total = (int)ExecuteScalar(sql, para);

        //    sql = "select count(1) from SchoolComments as c left join SchoolCommentScores as s on c.Id = s.CommentId where s.IsAttend = 1 and c.SchoolSectionId = @SchoolId";
        //    commentTotal.ComeHereTotal = (int)ExecuteScalar(sql, para);

        //    sql = "select count(1) from SchoolComments where SchoolSectionId = @SchoolId and RumorRefuting = 1";
        //    commentTotal.RefutingTotal = (int)ExecuteScalar(sql, para);

        //    sql = "select count(1) from SchoolComments as s where SchoolSectionId = @SchoolId and (select AggScore from SchoolCommentScores where CommentId = s.Id) >= 80";
        //    commentTotal.GoodTotal = (int)ExecuteScalar(sql, para);

        //    sql = "select count(1) from SchoolComments where IsHaveImagers = 1 and SchoolSectionId = @SchoolId";
        //    commentTotal.ImageTotal = (int)ExecuteScalar(sql, para);

        //    //获取该学校的高中部信息
        //    sql = "select id as Id from OnlineSchoolExtension as o inner join(select sid from OnlineSchoolExtension where id = @SchoolId) as s on o.sid = s.sid and o.grade = 4";
        //    var highSchool = _schoolRepository.Query<IdEntity>(sql, para).ToList();
        //    if (highSchool.Count() > 0)
        //    {

        //        List<SqlParameter> ids = highSchool.Select((item, index) => new SqlParameter("@id" + index, item.Id)).ToList();
        //        string strs = string.Join(',', highSchool.Select((item, index) => "@id" + index));

        //        sql = $"select count(1) from SchoolComments where SchoolSectionId in ({strs})";
        //        commentTotal.HighSchoolTotal = (int)ExecuteScalar(sql, ids.ToArray());
        //    }

        //    //获取该学校的高中部信息
        //    sql = "select id from OnlineSchoolExtension as o inner join(select sid from OnlineSchoolExtension where id = @SchoolId) as s on o.sid = s.sid and o.type = 3";
        //    var internationalSchool = _schoolRepository.Query<IdEntity>(sql, para).ToList();
        //    if (internationalSchool.Count() > 0)
        //    {

        //        List<SqlParameter> ids = internationalSchool.Select((item, index) => new SqlParameter("@id" + index, item.Id)).ToList();
        //        string strs = string.Join(',', internationalSchool.Select((item, index) => "@id" + index));

        //        sql = $"select count(1) from SchoolComments where SchoolSectionId in ({strs})";
        //        commentTotal.InternationalTotal = (int)ExecuteScalar(sql, ids.ToArray());
        //    }


        //    return commentTotal;
        //}

        public int UpdateQuestionLikeOrReplayCount(Guid QuestionId, int operaValue, bool Field)
        {
            string sql = "";
            if (Field)
            {
                sql = "update QuestionInfos set LikeCount = LikeCount + @operaValue where id = @Id";
            }
            else
            {
                sql = "update QuestionInfos set ReplyCount = ReplyCount + @operaValue where id = @Id";
            }
            SqlParameter[] para = {
                new SqlParameter("@Id",QuestionId),
                new SqlParameter("@operaValue",operaValue)
            };
            return ExecuteNonQuery(sql, para);
        }

        /// <summary>
        /// 获取该校下的精选问题
        /// </summary>
        /// <param name="schoolSectionId"></param>
        /// <returns></returns>
        public List<QuestionInfo> GetSchoolSelectedQuestion(List<Guid> schoolSectionIds, SelectedQuestionOrder Order)
        {
            if (schoolSectionIds.Count() == 0)
            {
                return new List<QuestionInfo>();
            }

            List<SqlParameter> para = schoolSectionIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", schoolSectionIds.Select((q, idx) => "@id" + idx));

            string order = "";
            if (Order == SelectedQuestionOrder.CreateTime)
            {
                order = " CreateTime desc ";
            }
            else
            {
                order = " t.IsTop, t.isSchoolUser DESC, ( LikeCount + ReplyCount ) DESC";
            }

            string sql = @"
                        select * from(
                        SELECT
	                        *,
	                        row_number () OVER ( partition BY t.SchoolSectionId ORDER BY " + order + @" ) AS TopLike 
                        FROM
	                        ( SELECT *, CASE WHEN PostUserRole = '1' THEN 1 ELSE 0 END AS isSchoolUser FROM QuestionInfos WHERE State in (0,1,2,3) and SchoolSectionId IN (" + strpara + @") ) AS t
                        ) s where s.TopLike = 1";

            if (Order == SelectedQuestionOrder.CreateTime)
            {
                sql += "  order by CreateTime desc";
            }
            return Query<QuestionInfo>(sql, para.ToArray())?.ToList();
        }

        public List<QuestionInfo> GetHotQuestionInfoBySchoolId(Guid schoolSectionId)
        {

            string sql = @"select Top 3 *,
            case when PostUserRole = '1' then 1 else 0 end as isSchoolUser from QuestionInfos
            where SchoolSectionId = @schoolSectionId and State in (0,1,2,3)
            order by IsTop desc,isSchoolUser desc, (LikeCount + ReplyCount) desc";
            SqlParameter[] para = {
                new SqlParameter("@schoolSectionId",schoolSectionId)
            };
            return Query<QuestionInfo>(sql, para).ToList();
        }

        public List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds)
        {
            List<SchoolSectionCommentOrQuestionTotal> totals = new List<SchoolSectionCommentOrQuestionTotal>();
            return base.GetList(x => SchoolSectionIds.Contains(x.SchoolSectionId)).GroupBy(x => x.SchoolSectionId).Select(x => new SchoolSectionCommentOrQuestionTotal() { SchoolSectionId = x.Key, Total = x.Count(c => c.SchoolSectionId == x.Key) })?.ToList();
        }

        public List<Guid> GetHotSchoolSectionId()
        {
            string sql = @"SELECT top 10 SchoolSectionId as Id FROM QuestionInfos as s right join
                                (select 
	                                Id,
	                                row_number() over(partition by SchoolSectionId order by ((LikeCount+ReplyCount)) desc) as TopLike
                                from QuestionInfos) as Hot
	                                on s.Id = Hot.Id where Hot.TopLike = 1 order by (LikeCount+ReplyCount) desc";

            return Query<SchoolIds>(sql, null).Select(x => x.Id)?.ToList();
        }

        public List<QuestionInfo> GetQuestionData(int pageNo, int pageSize, DateTime lastTime)
        {
            //return GetList(q => q.CreateTime > lastTime).Skip(pageNo * pageSize).Take(pageSize).OrderBy(q=>q.CreateTime).ToList();
            string sql = @"select 
	                        q.*
                        from QuestionInfos q
                        where q.CreateTime > @lastTime 
                        order by q.CreateTime asc 
                            OFFSET @OFFSET ROWS FETCH NEXT @pageSize ROWS ONLY";
            List<SqlParameter> para = new List<SqlParameter>() {
                new SqlParameter("@OFFSET", pageNo * pageSize),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@lastTime", lastTime),
            };
            var list = Query<QuestionInfo>(sql, para.ToArray()).ToList();
            return list;
        }

        public int SchoolCommentQuestionCountByTime(DateTime startTime, DateTime endTime)
        {
            //SqlParameter[] para = {
            //    new SqlParameter("@startTime",startTime),
            //    new SqlParameter("@endTime",endTime)
            //};
            //string sql = @"select count(DISTINCT(SchoolSectionId)) from QuestionInfos where CreateTime >= @startTime and CreateTime <=  @endTime group by SchoolSectionId ";
            return base.GetList(x => x.CreateTime >= startTime && x.CreateTime <= endTime).Select(x => new { x.SchoolSectionId }).GroupBy(x => x.SchoolSectionId).Count();
        }

        public List<QuestionTotal> PageQuestionTotalTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize)
        {
            string sql = @"
                            SELECT
                                    q1.SchoolId as School,
	                                q1.SchoolSectionId as SchoolSectionId,
	                                COUNT ( q1.Id ) AS Total,
	                                MAX ( q2.CreateTime ) AS CreateTime
                                FROM
	                                QuestionInfos AS q1
	                                LEFT JOIN ( SELECT SchoolSectionId, MAX ( CreateTime ) AS CreateTime FROM QuestionInfos GROUP BY SchoolSectionId ) AS q2 ON q1.SchoolSectionId = q2.SchoolSectionId 
	                                AND q1.CreateTime = q2.CreateTime 
													        LEFT JOIN [iSchoolUser].[dbo].[UserInfo] as u on q1.UserId = u.Id
                                WHERE
	                                q1.CreateTime >=  @startTime
	                                AND q1.CreateTime <= @endTime
													        AND ISNULL(u.channel,'-1') <> '8'
                                GROUP BY
	                                q1.SchoolSectionId,q1.SchoolId
                                ORDER BY
                                    CreateTime DESC
                                OFFSET @pageNo ROWS FETCH NEXT @pageSize ROWS ONLY";

            SqlParameter[] prar = {
                    new SqlParameter("@startTime",startTime),
                    new SqlParameter("@endTime",endTime),
                    new SqlParameter("@pageNo",(pageNo-1)*pageSize),
                    new SqlParameter("@pageSize",pageSize)
                };

            return Query<QuestionTotal>(sql, prar)?.ToList();
        }

        public int QuestionTotal(Guid userId)
        {
            return base.GetCount(x => x.UserId == userId);
        }


        public List<HotQuestionSchool> GetHotQuestionSchools(DateTime starTime, DateTime endTime, int count = 6)
        {
            //          string sql = @"
            //select top 6 *  from 
            //                       (select 
            //                                      s.Id as SchoolId,
            //	                          s.name+'-'+e.name as SchoolName,
            //		                         q.*,
            //		                         c.City,
            //		                         ROW_NUMBER() over(partition by c.City order by q.Total desc) rowNum	
            //		                        from [iSchoolData].[dbo].[OnlineSchoolExtContent] as c
            //                        RIGHT JOIN 
            //	                        (select 
            //				                         SchoolSectionId,
            //				                         sum(a.Total) as Total
            //	                        FROM QuestionInfos as q
            //		                        RIGHT JOIN
            //			                        (select QuestionInfoId,count(Id) as Total from QuestionsAnswersInfos	
            //					                        where CreateTime >= @starTime and CreateTime <= @endTime
            //					                        GROUP BY QuestionInfoId
            //			                        ) as a
            //		                        ON q.Id = a.QuestionInfoId
            //	                        GROUP BY SchoolSectionId) as q 
            //                        ON c.eid = q.SchoolSectionId
            //                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] as e	ON q.SchoolSectionId = e.Id
            //                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchool] as s ON e.sid = s.Id
            //												where e.IsValid = 1
            //                      ) as t 
            //                       where t.rowNum >= 1 and t.rowNum <= 6 order by t.Total desc";


            string sql = $@"SELECT
	                        c.SchoolSectionId,
	                        s.Id AS SchoolId,
	                        c.Total,
	                        s.name+ '-' + e.name AS SchoolName,
                            e.No AS SchoolNo
                        FROM
	                        (
	                        SELECT TOP
		                        {count} SchoolSectionId,
		                        COUNT ( 1 ) AS Total 
	                        FROM
		                        QuestionInfos AS c
		                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.Id
		                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchool] AS s ON e.sid = s.Id 
	                        WHERE
		                        c.CreateTime >= @starTime 
		                        AND c.CreateTime <= @endTime 
		                        AND e.IsValid = 1 
		                        AND s.IsValid = 1 
		                        AND s.status = 3 
		                        AND c.State in (0,1,2,3)
	                        GROUP BY
		                        SchoolSectionId 
	                        ORDER BY
		                        COUNT (1) DESC 
	                        ) AS c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.Id
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchool] AS s ON e.sid = s.Id 
                        ORDER BY
	                        c.Total DESC";

            SqlParameter[] para = {
                new SqlParameter("@starTime",starTime),
                new SqlParameter("@endTime",endTime)
            };
            return Query<HotQuestionSchool>(sql, para)?.ToList();
        }

        public List<QuestionInfo> GetHotQuestion(DateTime startTime, DateTime endTime)
        {
            string sql = @"select top 6* from QuestionInfos as q
	                    left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on e.id = q.SchoolSectionId
		                    where q.CreateTime >= @startTime and q.CreateTime <= @endTime  and q.State in (0,1,2,3) and e.IsValid = 1 
	                    order by (q.LikeCount + q.ReplyCount) desc ";

            return Query<QuestionInfo>(sql, new SqlParameter[] {
                new SqlParameter("@startTime",startTime),
                new SqlParameter("@endTime",endTime)
            })?.ToList();
        }

        /// <summary>
        /// 官网提问列表
        /// </summary>
        /// <param name="City"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public List<QuestionInfo> QuestionList_Pc(int City, int PageIndex, int PageSize)
        {

            List<SqlParameter> para = new List<SqlParameter>();
            string sql = @"select * from
                        (select q.*,
				                        row_number () 
			                        OVER ( partition BY q.SchoolSectionId 
							                        ORDER BY q.IsTop desc,q.CreateTime desc 
			                        ) AS row
	                        from 
		                        QuestionInfos as q
		                        LEFT JOIN [iSchoolData].dbo.OnlineSchoolExtContent as e
		                        on q.SchoolSectionId = e.eid";

            if (City > 0)
            {
                sql += " where e.city = @city ";
                para.Add(new SqlParameter("@city", City));
            }

            sql += @" ) as t
			    where t.row = 1
                  order by t.IsTop desc,t.CreateTime DESC 
                     OFFSET @pageindex ROWS FETCH NEXT @pagesize ROWS ONLY";

            para.Add(new SqlParameter("@pageindex", (PageIndex - 1) * PageSize));
            para.Add(new SqlParameter("@pagesize", PageSize));
            return Query(sql, para.ToArray())?.ToList();
        }

        public int QuestionListCount_Pc(int City)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            string sql = @"select count(1)
	                        from 
		                        QuestionInfos as q
		                        LEFT JOIN [iSchoolData].dbo.OnlineSchoolExtContent as e
		                        on q.SchoolSectionId = e.eid";

            if (City > 0)
            {
                sql += " where e.city = @city ";
                para.Add(new SqlParameter("@city", City));
            }
            return Query<IntCount>(sql, para.ToArray()).FirstOrDefault().Count;
        }

        public DateTime QueryQuestionTime(Guid Id)
        {
            string sql = "select CreateTime  from QuestionInfos where Id = @Id";
            return Query<QuestionInfo>(sql, new SqlParameter[] { new SqlParameter("@Id", Id) }).FirstOrDefault().CreateTime;
        }

        /// <summary>
        /// 通过学部ID统计问答数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        public SchQuestionData GetSchoolQuestionDataByID(Guid SchoolSectionId)
        {
            string sql = @"select count(q.Id) as QuestionCount,sum(q.LikeCount) as LikeCount,sum(q.ReplyCount) as ReplyCount,sum(q.ViewCount) as QuestionViewCount, sum(qa.ViewCount) as AnswerRepliyViewCount 
                            from QuestionInfos as q left join QuestionsAnswersInfos AS qa ON q.Id = qa.QuestionInfoId 
                            where q.SchoolSectionId = @SchoolSectionId  AND q.State in (0,1,2,3)
                            GROUP BY q.SchoolSectionId";

            return Query<SchQuestionData>(sql, new SqlParameter[]
                {
                    new SqlParameter("@SchoolSectionId",SchoolSectionId)
                }).FirstOrDefault();
        }
        public List<SchQuestionDataEx> GetSchoolQuestionDataByIDs(List<Guid> schoolSectionIds)
        {
            var ids = string.Join(",", schoolSectionIds.Select(p => $"'{p}'"));
            var str_SQL = $@"SELECT
	                            q.SchoolSectionId as SID,
	                            COUNT( q.Id ) AS QuestionCount,
	                            SUM ( q.LikeCount ) AS LikeCount,
	                            SUM ( q.ReplyCount ) AS ReplyCount,
	                            SUM ( q.ViewCount ) AS QuestionViewCount,
	                            SUM ( qa.ViewCount ) AS AnswerRepliyViewCount 
                            FROM
	                            QuestionInfos AS q
	                            LEFT JOIN QuestionsAnswersInfos AS qa ON q.Id = qa.QuestionInfoId 
                            WHERE
                                 q.SchoolSectionId in ({ids}) AND
	                             q.State < 4
                            GROUP BY
	                            q.SchoolSectionId";

            return Query<SchQuestionDataEx>(str_SQL).ToList();
        }
        public List<SchoolTotal> GetSchoolQuestionCountBuSchoolSectionIDs(List<Guid> guids, List<int> states = null)
        {
            var str_IDs = string.Join(",", guids.Select(p => $"'{p}'"));
            var str_States = "1,2,3,4";
            if (states?.Any() == true)
            {
                str_States = string.Join(",", states);
            }
            var str_SQL = $@"SELECT
	                            q.SchoolSectionId AS Id,
	                            COUNT (q.Id) AS Total
                            FROM
	                            QuestionInfos AS q
                            WHERE
	                            q.SchoolSectionId IN ({str_IDs}) 
	                            AND q.State IN ({str_States})
                            GROUP BY
	                            q.SchoolSectionId";
            return Query<SchoolTotal>(str_SQL).ToList();
        }

        class IntCount
        {
            public int Count { get; set; }
        }

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public bool UpdateViewCount(Guid questionId)
        {
            string sql = "update QuestionInfos set ViewCount = ViewCount + 1 where id = @Id";
            return ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@Id", questionId) }) > 0;
        }

        public List<SchoolTotal> GetAnswerCount(List<Guid> questionIds)
        {
            if (questionIds.Count == 0)
            {
                return new List<SchoolTotal>();
            }

            List<SqlParameter> para = questionIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", questionIds.Select((q, idx) => "@id" + idx));

            string sql = @"select Id,ReplyCount as Total from QuestionInfos where Id in (" + strpara + ");";

            return Query<SchoolTotal>(sql, para.ToArray()).ToList();
        }

        public List<SchoolTotal> GetQuestionAnswerCount(int pageNo, int pageSize)
        {
            string sql = @"select Id,ReplyCount as Total from QuestionInfos
                            Where ReplyCount > 0
                            ORDER BY CreateTime desc
		                    OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";
            SqlParameter[] para = {
                new SqlParameter("@offset",pageNo * pageSize),
                new SqlParameter("@limit",pageSize)
            };
            return Query<SchoolTotal>(sql, para.ToArray()).ToList();
        }

        public List<QuestionItem> GetQuestionItems(List<Guid> Ids)
        {
            if (!Ids.Any())
            {
                return new List<QuestionItem>();
            }

            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));

            string sql = @"select q.Id,q.Content,s.name+'-'+e.name as SchoolName from QuestionInfos as q
	                        left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on q.SchoolSectionId = e.id
	                        left join [iSchoolData].[dbo].[OnlineSchool] as s on e.sid = s.id
                        where q.Id in (" + strpara + ")";

            return Query<QuestionItem>(sql, para.ToArray()).ToList();
        }

        public int GetAnswerCount(Guid questionId)
        {
            string sql = @"SELECT top 1 [ReplyCount] as count
                          FROM [iSchoolProduct].[dbo].[QuestionInfos]
                          where ID = @questionId;";
            var para = new SqlParameter[] {
                new SqlParameter("@questionId",questionId)
            };
            return Query<IntCount>(sql, para).FirstOrDefault()?.Count ?? 0;
        }

        public List<QuestionInfo> GetHotQuestion(DateTime startTime, DateTime endTime, int count)
        {
            string sql = $@"select top {count} * from QuestionInfos as q
	                    left join [iSchoolData].[dbo].[OnlineSchoolExtension] as e on e.id = q.SchoolSectionId
		                    where q.CreateTime >= @startTime and q.CreateTime <= @endTime  and q.State <> 4 and e.IsValid = 1 
	                    order by (q.LikeCount + q.ReplyCount) desc ";

            return Query<QuestionInfo>(sql, new SqlParameter[] {
                new SqlParameter("@startTime",startTime),
                new SqlParameter("@endTime",endTime)
            })?.ToList();
        }

        public List<UserQuestionQueryDto> GetUserQuestions(IEnumerable<Guid> Ids)
        {
            if (!Ids.Any())
            {
                return new List<UserQuestionQueryDto>();
            }

            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));

            string sql = @"
SELECT
	QI.Id,
	QI.Content,
	QI.SchoolSectionId as ExtId,
	QI.UserId,
	QI.CreateTime,
	QI.LikeCount,
	QI.ReplyCount,
	QI.No,
	QI.IsAnony,
	(
		SELECT TOP 1 AI.Id FROM dbo.QuestionsAnswersInfos AI 
		WHERE AI.QuestionInfoId = QI.Id AND AI.State <> 4 ORDER BY AI.ReplyCount DESC, CreateTime DESC 
	) AS TopReplyAnswerId,
	(
		SELECT Count(1) FROM dbo.QuestionInfos QI2 
		WHERE QI2.SchoolSectionId = QI.SchoolSectionId AND QI.State <> 4
	) AS SchoolQuestionCount
FROM
   QuestionInfos QI
WHERE 
	QI.State <> 4 AND QI.Id in (" + strpara + ")" ;

            return Query<UserQuestionQueryDto>(sql, para.ToArray()).ToList();
        }


        public IEnumerable<Guid> GetAvailableIds(IEnumerable<Guid> Ids)
        {
            if (!Ids.Any())
            {
                return new List<Guid>();
            }
            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));
            var sql = $@"
SELECT
	QI.Id
FROM
    QuestionInfos QI
WHERE 
	QI.State <> 4 AND QI.Id in (" + strpara + ")";

            return Query<IdQueryDto>(sql, para.ToArray()).Select(s => s.Id);
        }
    }
}
