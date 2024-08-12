using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Data.SqlClient;
using PMS.CommentsManage.Domain.Entities.Total;
using System.Linq;

namespace PMS.CommentsManage.Repository.Repositories
{
    /// <summary>
    /// 审核操作日志
    /// </summary>
    public class ExaminerRecordRepository : EntityFrameworkRepository<ExaminerRecord>, IExaminerRecordRepository
    {

        public ExaminerRecordRepository(CommentsManageDbContext dbContext):base(dbContext)
        {
        }

        public new int Delete(Guid Id)
        {
           return base.Delete(Id);
        }

        public new IEnumerable<ExaminerRecord> GetList(Expression<Func<ExaminerRecord, bool>> where = null)
        {
            return base.GetList(where);
        }

        public  ExaminerRecord GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public  int Insert(ExaminerRecord model)
        {
            return base.Add(model);
        }

        public  bool isExists(Expression<Func<ExaminerRecord, bool>> where)
        {
            return base.GetList(where) == null;
        }

        public new int Update(ExaminerRecord model)
        {
            return base.Update(model);
        }

        public int ExaminerTotal(Guid UserId,bool ExaminerStatus) 
        {
            string sql = @"select sum(t.Total) as Total from 
                        (
	                        select count(e.Id) as Total from ExaminerRecords as e
		                        left join SchoolComments as s	on e.TargetId = s.Id
	                        where e.ExaminerType = 1 and 
		                        s.CommentUserId = @UserId 
                    ";

                    if (ExaminerStatus) 
                    {
                        sql += " and (e.ChangeAfterStatus = 3) ";
                    }
                    else 
                    {
                        sql += " and (e.ChangeAfterStatus = 2 or e.ChangeAfterStatus = 4) ";
                    }

                    sql += @"
		                        
	                        union
	                        select count(e.Id) as Total from ExaminerRecords as e
		                        left join QuestionsAnswersInfos as q on e.TargetId = q.Id
	                        where q.UserId = @UserId ";

                    if (ExaminerStatus)
                    {
                        sql += " and (e.ChangeAfterStatus = 3) ";
                    }
                    else
                    {
                        sql += " and (e.ChangeAfterStatus = 2 or e.ChangeAfterStatus = 4) ";
                    }

                    sql +=" ) as t";

            var entity = Query<BaseTotal>(sql, new SqlParameter[] { new SqlParameter("@UserId", UserId) }).FirstOrDefault();
            if (entity == null) 
            {
                return 0;
            }
            else 
            {
                return entity.Total;
            }
        }

        public ExaminerTotal GetCurrentExaminerCount(Guid AdminId) 
        {
                 string sql = @"
                        select 
	                        WaitSchoolCommentTotal,
	                        WaitSchoolAnswerTotal,
	                        SuccessCommentTotal,
	                        SuccessSchoolAnswerTotal,
	                        ExaminerSelectedCommentTotal,
	                        ExaminerSelectedAnswerTotal
                        from (
	                        select * from 
	                        (	select count(1) as WaitSchoolCommentTotal,1 as Id1 from SchoolComments
		                        as s
			                        left join PartTimeJobAdminRoles as p on s.CommentUserId = p.AdminId
		                        where p.Role = 1 and p.Shield = 0 and s.State = 1 and s.AddTime >= '2019-12-11 00:00:00'
	                        ) as waitComment
	                        FULL JOIN
	                        ( select count(1) as WaitSchoolAnswerTotal,1 as Id2 from QuestionsAnswersInfos as q
		                        left join PartTimeJobAdminRoles as p on p.AdminId = q.UserId
	                        where p.Role = 1 and p.Shield = 0 and q.State = 1 and q.CreateTime >= '2019-12-11 00:00:00'
	                        ) as watiAnswer on waitComment.Id1 = watiAnswer.Id2
	                        FULL JOIN 
	                        (
		                        select count(1) as SuccessCommentTotal,1 as Id3 from SchoolCommentExamines where AdminId = @AdminId
	                        ) as SuccessComment on SuccessComment.Id3 = watiAnswer.Id2
	                        FULL JOIN 
	                        (
		                        select count(1) as SuccessSchoolAnswerTotal,1 as Id4 from  QuestionsAnswerExamines where AdminId = @AdminId
	                        ) as SuccessSchoolAnswer on SuccessSchoolAnswer.Id4 = SuccessComment.Id3
	                        FULL JOIN 
	                        (
		                        select sum(c.ExaminerSelectedCommentTotal) as ExaminerSelectedCommentTotal,1 as Id5 from (
			                        select count(1) as ExaminerSelectedCommentTotal from ExaminerRecords where AdminId = @AdminId 
				                         and ExaminerType = 1 and ChangeAfterStatus = 3
				                        GROUP BY TargetId
		                        ) as c
	                        ) as ExaminerSelectedComment on ExaminerSelectedComment.Id5 = SuccessSchoolAnswer.Id4
	                        FULL JOIN 
	                        (
		                        select sum(c.ExaminerSelectedAnswerTotal) as ExaminerSelectedAnswerTotal,1 as Id6 from (
			                        select count(1) as ExaminerSelectedAnswerTotal from ExaminerRecords where AdminId = @AdminId 
				                         and ExaminerType = 2 and ChangeAfterStatus = 3
				                        GROUP BY TargetId
		                        ) as c
	                        ) as ExaminerSelectedAnswer on ExaminerSelectedComment.Id5 = ExaminerSelectedAnswer.Id6
                        ) as t
                    ";

            SqlParameter[] para = {
                new SqlParameter("@AdminId",AdminId)
            };

            var data = Query<ExaminerTotal>(sql, para).FirstOrDefault();
            if (data != null) 
            {
                double selectedTotal = (data.ExaminerSelectedCommentTotal + data.ExaminerSelectedAnswerTotal * 1.0) / (data.SuccessCommentTotal + data.SuccessSchoolAnswerTotal * 1.0) * 100.0;
                data.ExaminerSelectedTotal = decimal.Parse(selectedTotal.ToString("f2"));
            }
            return data;
        }

    }
}
