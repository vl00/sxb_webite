using Microsoft.EntityFrameworkCore;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class QuestionsAnswersInfoRepository : EntityFrameworkRepository<QuestionsAnswersInfo>, IQuestionsAnswersInfoRepository
    {
        //private readonly EntityFrameworkRepository<QuestionsAnswersInfo> efRepository;
        private readonly CommentsManageDbContext _dbContext;

        public QuestionsAnswersInfoRepository(CommentsManageDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public new int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public new IEnumerable<QuestionsAnswersInfo> GetList(Expression<Func<QuestionsAnswersInfo, bool>> where = null)
        {
            return base.GetList(where);
        }

        public QuestionsAnswersInfo GetModelById(Guid Id)
        {
            return base.GetList(x => x.Id == Id).Include(x => x.QuestionInfo).Include(x => x.ParentAnswerInfo).FirstOrDefault();
        }

        public List<QuestionsAnswersInfo> GetQuestionsAnswerByAdminId(Guid Id, int PageIndex, int PageSize, out int Total)
        {
            Total = 0;
            if (PageIndex == 0)
            {
                var rez = base.GetList(x => x.UserId == Id && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderByDescending(x => x.CreateTime);
                if (rez != null)
                {
                    Total = rez.Count();
                    return rez.ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Total = base.GetList(x => x.UserId == Id && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
                var rez = base.GetList(x => x.UserId == Id && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderByDescending(x => x.CreateTime).Skip((PageIndex - 1) * PageSize).Take(PageSize);
                if (rez != null)
                {
                    return rez.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public bool SetTop(Guid answerId, bool isTop)
        {
            return ExecuteNonQuery("update QuestionsAnswersInfos Set IsTop = @isTop Where Id = @id",
                 new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@isTop",isTop),
                    new System.Data.SqlClient.SqlParameter("@id",answerId)
                  }) > 0;
        }
        public bool SetNotTopByQuestionId(Guid questionId)
        {
            return ExecuteNonQuery("update QuestionsAnswersInfos Set IsTop = 'false' Where QuestionInfoId = @questionId;",
                 new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@questionId",questionId)
                  }) > 0;
        }


        public int Insert(QuestionsAnswersInfo model)
        {
            return base.Add(model);
        }

        public bool isExists(Expression<Func<QuestionsAnswersInfo, bool>> where)
        {
            return base.GetList(where) == null;
        }
        public new int Update(QuestionsAnswersInfo model)
        {
            return base.Update(model);
        }

        public List<QuestionsAnswersInfo> PageQuestionsAnswer(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null)
        {
            var result = base.GetList();
            if (schoolIds != null)
            {
                if (schoolIds.Count > 0)
                    result = result.Where(q => schoolIds.Contains(q.QuestionInfo.SchoolId));
                else
                    result = result.Where(q => false);
            }
            if (startTime > DateTime.MinValue && endTime < DateTime.MaxValue)
            {
                result = result.Where(p => p.CreateTime >= startTime && p.CreateTime < endTime);
            }

            total = result.Count();
            var rez = result.Include(q => q.QuestionInfo).OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            if (rez != null)
            {
                return rez.ToList();
            }
            return null;
        }

        public List<QuestionsAnswersInfo> PageQuestionsAnswerByExamineState(int pageIndex, int pageSize, int examineState, out int total)
        {
            var result = base.GetList();
            if (examineState == 0)
            {
                result = result.Where(q => q.State == ExamineStatus.Unread).Include(q => q.QuestionInfo).OrderByDescending(q => q.CreateTime);
            }
            else
            {
                result = result.Where(q => q.State != ExamineStatus.Unread).Include(q => q.QuestionInfo).Include(q => q.QuestionsAnswerExamine).OrderByDescending(q => q.QuestionsAnswerExamine.ExamineTime);
            }

            total = result.Count();
            var rez = result.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            if (rez != null)
            {
                return rez.ToList();
            }
            return null;
        }

        public List<QuestionsAnswersInfo> ListAnswersById(IEnumerable<Guid> AnswerIds)
        {
            return base.GetList(q => AnswerIds.Contains(q.Id)).ToList();
        }

        /// <summary>
        /// 通过回复ID获取回答
        /// </summary>
        public QuestionsAnswersInfo QueryAnswerByReplyId(Guid ReplyId)
        {
            string sql = @"WITH T
                AS( 
                    SELECT * FROM QuestionsAnswersInfos WHERE Id=@replyId
                    UNION ALL 
                    SELECT a.*  
                    FROM QuestionsAnswersInfos a INNER JOIN T ON a.Id=T.ParentId  
                ) 
                SELECT * FROM T where T.ParentId is null;";
            var param = new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@replyId",ReplyId)
                  };
            return Query(sql, param).FirstOrDefault();
        }

        /// <summary>
        /// 批量通过回复ID获取回答
        /// </summary>
        public List<QuestionsAnswersInfo> ListAnswerByReplyIds(List<Guid> replyIds)
        {
            if (replyIds == null || replyIds.Count == 0)
            {
                return null;
            }
            List<SqlParameter> para = replyIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", replyIds.Select((q, idx) => "@id" + idx));
            string where = "";
            if (replyIds.Count > 0)
            {
                where += " and  sc.SchoolSectionId in (" + strpara + ") ";
            }
            string sql = @"WITH T
                AS( 
                    SELECT * FROM QuestionsAnswersInfos WHERE 1=1 " + where + @"
                    UNION ALL 
                    SELECT a.*  
                    FROM QuestionsAnswersInfos a INNER JOIN T ON a.Id=T.ParentId  
                ) 
                SELECT * FROM T where T.ParentId is null;";
            return Query(sql, para.ToArray()).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        public int UpdateAnswerLikeorReplayCount(Guid ReplayId, int operaValue, bool Field)
        {
            string sql = "";
            if (Field)
            {
                sql = "update QuestionsAnswersInfos set LikeCount = LikeCount + @operaValue where id = @Id";
            }
            else
            {
                sql = "update QuestionsAnswersInfos set ReplyCount = ReplyCount + @operaValue where id = @Id";
            }
            SqlParameter[] para = {
                new SqlParameter("@Id",ReplayId),
                new SqlParameter("@operaValue",operaValue)
            };
            return ExecuteNonQuery(sql, para);
        }
        /// <summary>
        /// 获取回答下所有回复
        /// </summary>
        public List<QuestionsAnswersInfo> ListReplyByAnswerId(Guid AnswerId)
        {
            string sql = @"WITH T
                AS( 
                    SELECT * FROM QuestionsAnswersInfos WHERE Id=@AnswerId
                    UNION ALL 
                    SELECT a.*  
                    FROM QuestionsAnswersInfos a INNER JOIN T ON a.ParentId=T.Id  
                ) 
                SELECT * FROM T ;";
            var param = new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@AnswerId",AnswerId)
                  };
            return Query(sql, param).ToList();
        }
        public int AnswerReplyTotalById(Guid answerId)
        {
            int total = base.GetList(x => x.ParentId == answerId).Count();
            return total;
        }

        public List<QuestionsAnswersInfoExt> PageReplyByAnswerId(Guid AnswerId, int ordertype, int pageNo, int pageSize)
        {
            string order = "";
            if (ordertype == 0)
            {
                order += "T.LikeCount desc,";
            }
            string sql = @"WITH T
                AS( 
                    SELECT * FROM QuestionsAnswersInfos WHERE Id=@AnswerId
                    UNION ALL 
                    SELECT a.*  
                    FROM QuestionsAnswersInfos a INNER JOIN T ON a.ParentId=T.Id  
                ) 
                SELECT T.*,b.UserId as ParentUserId,b.IsAnony as ParentUserIdIsAnony FROM T left join QuestionsAnswersInfos b on T.ParentId = b.Id
                where T.ParentId is not null
                order by " + order + @" T.CreateTime desc offset @offset rows fetch next @limit rows only;";
            var param = new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@AnswerId",AnswerId),
                     new System.Data.SqlClient.SqlParameter("@offset",(pageNo-1)*pageSize),
                      new System.Data.SqlClient.SqlParameter("@limit",pageSize),
                  };
            return Query<QuestionsAnswersInfoExt>(sql, param).ToList();
        }



        public List<QuestionsAnswersInfoExt> PageDialog(Guid id, List<Guid> userId, int pageIndex, int pageSize)
        {
            List<SqlParameter> para = userId.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();

            string strpara = string.Join(",", userId.Select((q, idx) => "@id" + idx));

            string sql = @"WITH T1
                AS( 
                    SELECT b1.*FROM QuestionsAnswersInfos b1
                                        WHERE b1.Id=@replyId
                    UNION ALL 
                    SELECT a1.*
                    FROM QuestionsAnswersInfos a1
                                        INNER JOIN T1 ON a1.Id=T1.ParentId 
                                        and a1.UserId in (" + strpara + @")
                ) ,
                            T2 AS( 
                    SELECT b1.*FROM QuestionsAnswersInfos b1
                                        WHERE b1.Id=@replyId
                    UNION ALL 
                    SELECT a1.*
                    FROM QuestionsAnswersInfos a1
                                        INNER JOIN T2 ON a1.ParentId=T2.Id 
                                        and a1.UserId in (" + strpara + @")
                )
                                select T.*,b.UserId as ParentUserId,b.IsAnony as ParentUserIdIsAnony from(
                                SELECT T1.* FROM T1
                                Union
                SELECT T2.* FROM T2) T left join QuestionsAnswersInfos b on T.ParentId = b.Id
                                 order by T.CreateTime offset @offset rows fetch next @limit rows only ;";
            para.Add(new SqlParameter("@replyId", id));
            para.Add(new SqlParameter("@offset", (pageIndex - 1) * pageSize));
            para.Add(new SqlParameter("@limit", pageSize));
            return Query<QuestionsAnswersInfoExt>(sql, para.ToArray()).ToList();
        }

        public List<QuestionsAnswersInfo> PageAnswerByUserId(Guid UserId, bool IsSelf, int PageIndex, int PageSize, List<Guid> AnswerIds = null)
        {
            List<SqlParameter> para = new List<SqlParameter>();
            string sql = @"select * from 
                            QuestionsAnswersInfos 
                                where State in (0,1,2,3) AND ParentId IS NULL ";

            if (AnswerIds != null)
            {
                sql += " and Id in @AnswerIds ";
                para.Add(new SqlParameter("@AnswerIds", AnswerIds));
            }
            else
            {
                sql += " and UserId = @UserId ";
                para.Add(new SqlParameter("@UserId", UserId));
            }

            if (!IsSelf)
            {
                sql += " and IsAnony = 0";
            }

            sql += " order by CreateTime desc offset @offset rows fetch next @limit rows only;";

            para.Add(new SqlParameter("@offset", (PageIndex - 1) * PageSize));
            para.Add(new SqlParameter("@limit", PageSize));

            List<QuestionsAnswersInfo> answersInfos = Query<QuestionsAnswersInfo>(sql, para.ToArray()).ToList();

            if (!answersInfos.Any())
            {
                return new List<QuestionsAnswersInfo>();
            }
            return answersInfos;
        }

        /// <summary>
        /// 查询一批问题下最热门的n条回答
        /// </summary>
        /// <param name="QuestionInfoIds"></param>
        /// <param name="Take"></param>
        /// <returns></returns>
        public List<QuestionsAnswersInfo> QuestionAnswersOrderByRole(List<Guid> QuestionInfoIds, int Take)
        {
            List<SqlParameter> para = QuestionInfoIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", QuestionInfoIds.Select((q, idx) => "@id" + idx));
            string sql = @"SELECT
	                        * 
                        FROM
	                        (
	                        SELECT
		                        *,
		                        row_number () OVER ( partition BY answer.QuestionInfoId ORDER BY answer.IsTop, answer.UserRole DESC, ( LikeCount + ReplyCount ) DESC ) AS TopLike 
	                        FROM
		                        (
		                        SELECT
			                        *,
		                        CASE
				
				                        WHEN PostUserRole = '1' THEN
				                        2 
				                        WHEN PostUserRole = '2' THEN
				                        1 ELSE 0 
			                        END AS UserRole 
		                        FROM
			                        QuestionsAnswersInfos 
		                        WHERE
			                        QuestionInfoId IN (" + strpara + @") 
                                    AND ParentId is null
			                        AND State in (0,1,2,3)
		                        ) AS answer 
	                        ) AS A 
                        WHERE
	                        A.TopLike <= @Take";

            para.Add(new SqlParameter("@Take", Take));
            return Query<QuestionsAnswersInfo>(sql, para.ToArray())?.ToList();
        }

        public List<QuestionsAnswersInfo> ReplyNewest(List<Guid> replyIds)
        {
            List<SqlParameter> para = replyIds.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", replyIds.Select((q, idx) => "@id" + idx));

            string sql = @"
                select * from QuestionsAnswersInfos as p inner join 
                (select ParentId,max(CreateTime) as CreateTime from QuestionsAnswersInfos where ParentId in (" + strpara + @")
	                group by ParentId) as s on p.ParentId = s.ParentId and p.CreateTime = s.CreateTime
            ";

            return Query<QuestionsAnswersInfo>(sql, para.ToArray())?.ToList();
        }

        public List<QuestionsAnswersInfo> GetCurrentUserNewestAnswer(int PageIndex, int PageSize, Guid UserId)
        {
            string sql = @"select 	                        a.Id,
	                        a.QuestionInfoId,
	                        a.State,
	                        a.UserId,
	                        a.IsSchoolPublish,
	                        a.IsAttend,
	                        a.IsAnony,
	                        a.Content,
	                        a.LikeCount,
	                        a.IsSettlement,
	                        a.CreateTime,
	                        a.IsTop,
	                        a.ParentId,
	                        a.ReplyCount,
	                        a.PostUserRole,
	                        a.UserInfoExId
                        from QuestionsAnswersInfos as a
	                        left join QuestionInfos as q
	                        on a.QuestionInfoId = q.Id
                        where q.UserId = @userId
                        order by a.CreateTime desc OFFSET ((@pageIndex-1)*@pageSize) ROW FETCH NEXT @pageSize rows only";

            SqlParameter[] para = {
                new SqlParameter("@userId",UserId),
                new SqlParameter("@pageIndex",PageIndex),
                new SqlParameter("@pageSize",PageSize)
            };
            return Query<QuestionsAnswersInfo>(sql, para)?.ToList();
        }

        public QuestionsAnswersInfo GetFirstParent(Guid Id)
        {
            string sql = @"
                            with temp as
	                        (
		                        select * from QuestionsAnswersInfos where Id = @Id
		                        union all
		                        select p.* from QuestionsAnswersInfos as p inner join temp on  temp.ParentId = p.Id
	                        )
	                        select *　from temp where ParentId is null
                        ";
            SqlParameter[] para = {
                new SqlParameter("@Id",Id)
            };
            return Query<QuestionsAnswersInfo>(sql, para).FirstOrDefault();
        }

        public int QuestionAnswer(Guid userId)
        {
            return base.GetCount(x => x.UserId == userId && x.ParentId == null);
        }

        public int AnswerReplyTotal(Guid userId)
        {
            return base.GetCount(x => x.UserId == userId && x.ParentId != null);
        }

        public List<QuestionAnswerAndReply> CurrentPublishQuestionAnswerAndReply(Guid UserId, int PageIndex, int PageSize)
        {
            string sql = @"select * from (
                            select 
	                            a.Id,
	                            a.QuestionInfoId,
	                            '00000000-0000-0000-0000-000000000000' as ParentId,
	                            a.UserId,
	                            a.IsAnony,
	                            a.Content,
	                            a.LikeCount,
	                            a.ReplyCount,
	                            a.CreateTime,
	                             0 as Type
                            from QuestionsAnswersInfos as a where a.ParentId is null
                            union
                            select 
	                            a.Id,
	                            a.QuestionInfoId,
	                            a.ParentId,
	                            a.UserId,
	                            a.IsAnony,
	                            a.Content,
	                            a.LikeCount,
	                            a.ReplyCount,
	                            a.CreateTime,
	                            1 as Type
                            from QuestionsAnswersInfos as a where a.ParentId is not null) as a where a.UserId = @UserId
                                order by a.CreateTime desc OFFSET ((@PageIndex-1)*@PageSize) ROW FETCH NEXT @PageSize rows only";

            SqlParameter[] para = {
                new SqlParameter("@PageIndex",PageIndex),
                new SqlParameter("@PageSize",PageSize),
                new SqlParameter("@UserId",UserId)
            };
            return Query<QuestionAnswerAndReply>(sql, para)?.ToList();
        }

        public List<QuestionAnswerAndReply> CurrentLikeQuestionAndAnswer(Guid UserId, int PageIndex, int PageSize)
        {
            string sql = @"SELECT
	                        * 
                        FROM
	                        (
	                        SELECT
		                        a.Id,
		                        a.QuestionInfoId,
		                        '00000000-0000-0000-0000-000000000000' AS ParentId,
		                        s.UserId as LikeUser,
                                a.UserId,
		                        a.IsAnony,
		                        a.Content,
		                        a.LikeCount,
		                        a.ReplyCount,
		                        a.CreateTime,
		                        0 AS Type 
	                        FROM
		                        QuestionsAnswersInfos AS a 
		                        RIGHT JOIN SchoolCommentLikes as s
		                        on a.Id = s.SourceId and s.LikeType = 4 and s.LikeStatus = 1
	                        WHERE
		                        a.ParentId IS NULL and a.Id is NOT NULL
                        UNION
	                        SELECT
		                        a.Id,
		                        a.QuestionInfoId,
		                        a.ParentId,
		                        s.UserId  as LikeUser,
                                a.UserId,
		                        a.IsAnony,
		                        a.Content,
		                        a.LikeCount,
		                        a.ReplyCount,
		                        a.CreateTime,
		                        1 AS Type 
	                        FROM
		                        QuestionsAnswersInfos AS a
		                        RIGHT JOIN SchoolCommentLikes as s
		                        on a.Id = s.SourceId and s.LikeType = 4 and s.LikeStatus = 1
				                        where a.ParentId is not null and a.Id is NOT NULL) as a where a.LikeUser = @UserId
                                                        order by a.CreateTime desc OFFSET ((@PageIndex-1)*@PageSize) ROW FETCH NEXT @PageSize rows only";

            SqlParameter[] para = {
                new SqlParameter("@PageIndex",PageIndex),
                new SqlParameter("@PageSize",PageSize),
                new SqlParameter("@UserId",UserId)
            };
            return Query<QuestionAnswerAndReply>(sql, para)?.ToList();
        }

        public bool CheckAnswerDistinct(string content)
        {
            return base.GetList(x => x.Content == content).FirstOrDefault() == null;
        }

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public bool UpdateViewCount(Guid answerId)
        {
            string sql = "update QuestionsAnswersInfos set ViewCount = ViewCount + 1 where id = @Id";
            return ExecuteNonQuery(sql, new SqlParameter[] { new SqlParameter("@Id", answerId) }) > 0;
        }

        public List<QuestionsAnswersInfoExt> GetAnswerHottestReplys(Guid answerReplyId)
        {
            string sql = @"select 
	                         top 3
		                        w.*,
	                         case
		                        when w.PostUserRole = 2 then 1
		                        when w.PostUserRole = 1 then 2
		                        when w.PostUserRole = 0 then 3
		                        else 4
	                        end as Weight
                        from QuestionsAnswersInfos as w
	                        where w.ParentId = @answerReplyId
	                        order by Weight,(ReplyCount + LikeCount) desc";
            var para = new SqlParameter[] {
                new SqlParameter("@answerReplyId",answerReplyId)
            };

            return Query<QuestionsAnswersInfoExt>(sql, para)?.ToList();
        }

        /// <summary>
        /// 根据回答id，得到问题内容，回答内容
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public List<AnswerReply> GetQuestionAnswerReplyByIds(List<Guid> Ids)
        {
            List<SqlParameter> para = Ids.Select((q, idx) => new SqlParameter("@id" + idx, q)).ToList();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));

            return Query<AnswerReply>(@"select 
				        q.Content as QuestionContent,
				        q.UserId as QuestionUserId,
				        q.Id as QuestionId,
                        q.SchoolSectionId as SchoolSectionId,
                        q.IsAnony as QuestionIsAnony,
				        p.Content as AnswerContent,
				        p.UserId as AnswerUserId,
				        p.Id as AnswerId,
                        p.IsAnony as AnswerIsAnony,
				        a.Content as ReplyContent,
				        a.UserId as ReplyUserId,
                        a.ReplyCount as ReplyCount,
				        a.Id as ReplyId,
                        a.IsAnony as ReplyIsAnony,
				        (case when p.id is null then 1 else 0 end) as Type
			        from QuestionsAnswersInfos as a 
				        left join QuestionsAnswersInfos as p on a.ParentId = p.Id
				        left join QuestionInfos as q on a.QuestionInfoId = q.Id
				        where a.Id in (" + strpara + ")", para.ToArray()).ToList();
        }

        public int GetReplyCount(Guid parentId)
        {
            string sql = @"SELECT top 1 [ReplyCount] as count
                          FROM[iSchoolProduct].[dbo].[QuestionsAnswersInfos]
                          where ID = @parentId;";
            var para = new SqlParameter[] {
                new SqlParameter("@parentId",parentId)
            };
            return Query<IntCount>(sql, para).FirstOrDefault().Count;
        }

        public async Task<Dictionary<Guid, int>> GetAnswerCountByQuestionIDs(IEnumerable<Guid> ids)
        {
            var result = new Dictionary<Guid, int>();
            var str_SQL = $@"SELECT
	                            QuestionInfoId as [ID],
	                            Count(Id) as [Count]
                            FROM
	                            QuestionsAnswersInfos 
                            WHERE
	                            QuestionInfoId IN ('{string.Join("','", ids)}')
	                            AND FirstParentId IS NULL
	                            GROUP BY QuestionInfoId";
            await Task.Run(() =>
            {
                var finds = Query<IDCount>(str_SQL);
                if (finds?.Any() == true)
                {
                    result = finds.ToDictionary(k => k.ID, v => v.Count);
                }
            });
            return result;
        }

        class IntCount
        {
            public int Count { get; set; }
        }

        class IDCount
        {
            public Guid ID { get; set; }
            public int Count { get; set; }
        }
    }
}
