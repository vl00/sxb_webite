using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Repository.Repositories.ProcViewRepositories
{
    public class FindAllJobEntityServiceRepository : EntityFrameworkRepository<ProcFindAllJobEntityList>, IFindAllJobEntityServiceRepository
    {
        private readonly CommentsManageDbContext _dbContext;

        public FindAllJobEntityServiceRepository(CommentsManageDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProcFindAllJobEntityList> FindAllJobEntityByLeaderId(Guid Id, int PageIndex, int PageSize,string Phone,DateTime beginTime,DateTime endTime, out ProcFindAllJobEntityTotal procFindAllJobEntityTotal)
        {
            string countSql = @" SELECT
	                            count(1) as JobTotal,
	                            sum(c.CommentTotal) AS TotalSchoolComment,
	                            sum(c.Selected) AS TotalSchoolCommentsSelected,
	                            sum(a.CommentTotal) AS TotalAnswer,
	                            sum(a.Selected) AS TotalAnswerSelected 
                            FROM
	                            PartTimeJobAdminRoles AS r
	                            LEFT JOIN PartTimeJobAdmins AS j ON j.id = r.AdminId
	                            LEFT JOIN (
	                            SELECT
		                            a.CommentUserId,
		                            COUNT ( 1 ) AS CommentTotal,
		                            MAX ( a1.t ) AS Selected 
	                            FROM
		                            SchoolComments AS a
		                            LEFT JOIN ( SELECT COUNT ( 1 ) AS t, CommentUserId FROM SchoolComments WHERE State = 3 and AddTime >= '2019-12-11 00:00:00' GROUP BY CommentUserId ) AS a1 ON a.CommentUserId = a1.CommentUserId 
																where  a.AddTime >= '2019-12-11 00:00:00'
	                            GROUP BY
		                            a.CommentUserId 
	                            ) AS c ON r.AdminId = c.CommentUserId
	                            LEFT JOIN (
	                            SELECT
		                            a.UserId,
		                            COUNT ( 1 ) AS CommentTotal,
		                            MAX ( a1.t ) AS Selected 
	                            FROM
		                            QuestionsAnswersInfos AS a
		                            LEFT JOIN ( SELECT COUNT ( 1 ) AS t, UserId FROM QuestionsAnswersInfos WHERE State = 3 and CreateTime >= '2019-12-11 00:00:00' GROUP BY UserId ) AS a1 ON a.UserId = a1.UserId 
																	where  a.CreateTime >= '2019-12-11 00:00:00'
	                            GROUP BY
		                            a.UserId 
	                            ) AS a ON a.UserId = r.AdminId 
                            WHERE
	                            r.ParentId =@Id
                ";

            if (Phone != null && Phone != "")
            {
                countSql += " and j.Phone like @phone";
                SqlParameter[] para1 = {
                    new SqlParameter("@Id",Id),
                    new SqlParameter("@phone",Phone)
                };
                procFindAllJobEntityTotal = Query<ProcFindAllJobEntityTotal>(countSql, para1).FirstOrDefault();
            }
            else 
            {
                SqlParameter[] para1 = {
                    new SqlParameter("@Id",Id)
                };
                procFindAllJobEntityTotal = Query<ProcFindAllJobEntityTotal>(countSql, para1).FirstOrDefault();
            }

            List<SqlParameter> para = new List<SqlParameter>();

            string sql = @"SELECT
	                        r.AdminId AS Id,
	                        j.Name,
	                        j.Phone,
                        CASE
		
		                        WHEN j.SettlementType = 1 THEN
		                        '微信现结' ELSE '另结' 
	                        END AS SettlementType,
	                        c.CommentTotal AS TotalSchoolComment,
	                        c.Selected AS TotalSchoolCommentsSelected,
	                        a.CommentTotal AS TotalAnswer,
	                        a.Selected AS TotalAnswerSelected 
                        FROM
	                        PartTimeJobAdminRoles AS r
	                        RIGHT JOIN PartTimeJobAdmins AS j ON j.id = r.AdminId  and j.RegesitTime >= @beginTime and j.RegesitTime <= @endTime
	                        LEFT JOIN (
					                        SELECT
				                        a.CommentUserId,
				                        COUNT ( 1 ) AS CommentTotal,
				                        max(a1.t) AS Selected
			                        FROM
				                        SchoolComments as a
					                        left join (select count(1) as t,CommentUserId from SchoolComments where State = 3 and AddTime >= '2019-12-11 00:00:00' GROUP BY CommentUserId)
						                        as a1 on a.CommentUserId = a1.CommentUserId
                                                	where  a.AddTime >= '2019-12-11 00:00:00'
			                        GROUP BY
				                        a.CommentUserId
	                        ) AS c ON r.AdminId = c.CommentUserId 
	                        LEFT JOIN (
					                        SELECT
				                        a.UserId,
				                        COUNT ( 1 ) AS CommentTotal,
				                        max(a1.t) AS Selected
			                        FROM
				                        QuestionsAnswersInfos as a
					                        left join (select count(1) as t,UserId from QuestionsAnswersInfos where State = 3 and CreateTime >= '2019-12-11 00:00:00'  GROUP BY UserId)
						                        as a1 on a.UserId = a1.UserId
                                            where  a.CreateTime >= '2019-12-11 00:00:00'
			                        GROUP BY
				                        a.UserId) AS a ON a.UserId = r.AdminId  
                        WHERE
	                        r.ParentId = @Id ";

            para.Add(new SqlParameter("@Id", Id));
            para.Add(new SqlParameter("@PageIndex", PageIndex));
            para.Add(new SqlParameter("@PageSize", PageSize));
            para.Add(new SqlParameter("@beginTime",beginTime));
            para.Add(new SqlParameter("@endTime", endTime));

            if (Phone != null && Phone != "")
            {
                sql += "  and j.Phone like @phone ";
                para.Add(new SqlParameter("@phone", Phone));
            }
                    sql += " order by c.CommentTotal desc  OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

            return Query<ProcFindAllJobEntityList>(sql, para.ToArray())?.ToList();
        }

    }
}
