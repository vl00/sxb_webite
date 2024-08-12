using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace PMS.CommentsManage.Repository.Repositories.ProcViewRepositories
{
    public class ViewExaminerTotalRepository : EntityFrameworkRepository<ViewExaminerTotal>,IViewExaminerTotalRepository
    {
        private readonly CommentsManageDbContext _dbContext;

        public ViewExaminerTotalRepository(CommentsManageDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public ViewExaminerTotal GetViewExaminerTotal()
        {
            return Query("select * from ExaminerTotal").FirstOrDefault();
        }

        public SupplierTotal GetSupplierTotal(Guid SupplierId,DateTime BeginTime,DateTime EndTime) 
        {
            if (BeginTime.Year == 2019 && BeginTime.Month == 12)
            {
                BeginTime = DateTime.Parse("2019-12-11 00:00:00");
            }

            string sql = @"select
	            a.PartTimeJobAdminTotal,
	            b.CommitCommentTotal,
	            d.ExaminerCommentTotal,
	            f.SelectedCommentTotal,
                c.CommitAnswerTotal,
	            e.ExaminerAnswerTotal,
	            g.SelectedAnswerTotal,
	            h.ShieldJobTotal,
	            i.ShieldJobCommentTotal,
	            k.ShieldJobSelectedCommentTotal,
	            j.ShieldJobAnswerTotal,
	            l.ShieldJobSelectedAnswerTotal
            from 
	            (select count(1) as JobPartTotal,1 as Id from PartTimeJobAdminRoles where ParentId = @SupplierId ) as s FULL JOIN
                            (
	                	            select count(p1.Id) as PartTimeJobAdminTotal,1 as Id from PartTimeJobAdmins as s
												RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = s.Id
		                    RIGHT JOIN PartTimeJobAdminRoles as p on p1.ParentId = p.AdminId and p.ParentId = @SupplierId
	                    where s.RegesitTime >= @BeginTime and s.RegesitTime <= @EndTime
                            ) as a on s.Id = a.Id
	                            FULL JOIN 
                            (	select count(c.Id) as CommitCommentTotal,1 as Id from SchoolComments as c
									            LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
		                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
	                            where c.AddTime >= @BeginTime and c.AddTime <= @EndTime
                            ) as b on a.Id = b.Id
                            FULL JOIN
                            (select count(a.Id) as CommitAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									            LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                            ) as c on b.Id = c.Id
                            FULL JOIN
                            (select count(c.Id) as ExaminerCommentTotal,1 as Id from SchoolComments as c
									            LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where c.State <> 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime 
                            ) as d on d.Id = c.Id
                            FULL JOIN
                            (select count(a.Id) as ExaminerAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									            LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where a.State <> 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                            ) as e on e.Id = d.Id
                            FULL JOIN
                            (select count(c.Id) as SelectedCommentTotal,1 as Id from SchoolComments as c
									            LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where s.Role = 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime and c.State = 3
                            ) as f on f.Id = e.Id
                            FULL JOIN
                            (select count(a.Id) as SelectedAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									            LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where s.Role = 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime and a.State = 3
                            ) as g on g.Id = f.Id
                            FULL JOIN
                            (select count(a.Id) as ShieldJobTotal,1 as Id from
	                            PartTimeJobAdmins as a right join PartTimeJobAdminRoles as s on s.AdminId = a.Id
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where s.Shield = 1 and a.RegesitTime >= @BeginTime and a.RegesitTime <= @EndTime) as h on h.Id = g.Id
                            FULL JOIN
                            (
                            select count(c.Id) as ShieldJobCommentTotal,1 as Id from SchoolComments as c
									            LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where s.Shield = 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime
                            ) as i on i.Id = h.Id
                            FULL JOIN
                            (select count(a.Id) as ShieldJobAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									            LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where s.Shield = 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                            ) as j on i.Id = j.Id
                            FULL JOIN
                            (select count(c.Id) as ShieldJobSelectedCommentTotal,1 as Id from SchoolComments as c
									            LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where s.Shield = 1 and s.Role = 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime and c.State = 3
                            ) as k on k.Id = j.Id
                            FULL JOIN
                            (select count(a.Id) as ShieldJobSelectedAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									            LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime <= @EndTime
	                            RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                            RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId and p.ParentId = @SupplierId
                            where a.State = 3 and s.Shield = 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                            ) as l on l.Id = k.Id";

            SqlParameter[] para = {
                new SqlParameter("@SupplierId",SupplierId),
                new SqlParameter("@BeginTime",BeginTime),
                new SqlParameter("@EndTime",EndTime)
            };
            return Query<SupplierTotal>(sql, para).FirstOrDefault();
        }

        public SupplierTotal GetLeaderCurrentMonthTotal(Guid ParentId, DateTime BeginTime, DateTime EndTime,string Phone) 
        {
            if(BeginTime.Year == 2019 && BeginTime.Month == 12) 
            {
                BeginTime = DateTime.Parse("2019-12-11 00:00:00");
            }


            List<SqlParameter> para = new List<SqlParameter>();
            string phoneStr = "";
            string phoneStr1 = "";

            if (Phone != "" && Phone != null)
            {
                para.Add(new SqlParameter("@Phone", Phone));
                phoneStr += "  and s.Phone like @phone";
                phoneStr1 += "  and p.Phone like @phone";
            }


            string sql = @"SELECT
	                            * 
                            FROM
	                            (
	                            SELECT COUNT
		                            ( s.Id ) AS PartTimeJobAdminTotal,
		                            1 AS Id 
	                            FROM
		                            PartTimeJobAdmins AS s 
																	RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
	                            WHERE
		                            r.ParentId = @ParentId 
		                            AND s.RegesitTime >= @BeginTime 
		                            AND s.RegesitTime <= @EndTime 
                        "+ phoneStr + @"
	                            ) AS a
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS CommitCommentTotal,
		                            1 AS Id 
	                            FROM
		                            SchoolComments 
	                            WHERE
		                            CommentUserId IN 
																( SELECT s.Id FROM PartTimeJobAdmins AS s 
																		RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
																	WHERE r.ParentId = @ParentId AND s.RegesitTime >= @BeginTime AND s.RegesitTime <= @EndTime " + phoneStr + @"
																) 
		                            AND AddTime >= @BeginTime 
		                            AND AddTime <= @EndTime 
	                            ) AS b ON a.Id = b.Id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) CommitAnswerTotal,
		                            1 AS Id 
	                            FROM
		                            QuestionsAnswersInfos 
	                            WHERE
		                            UserId IN ( SELECT s.Id FROM PartTimeJobAdmins AS s 
																		RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
																	WHERE r.ParentId = @ParentId AND s.RegesitTime >= @BeginTime AND s.RegesitTime <= @EndTime " + phoneStr + @"
																 ) 
		                            AND CreateTime >= @BeginTime 
		                            AND CreateTime <= @EndTime 
	                            ) AS c ON b.Id = c.Id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ExaminerCommentTotal,
		                            1 AS Id 
	                            FROM
		                            SchoolComments 
	                            WHERE
		                            CommentUserId IN (  SELECT s.Id FROM PartTimeJobAdmins AS s 
																		RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
																	WHERE r.ParentId = @ParentId AND s.RegesitTime >= @BeginTime AND s.RegesitTime <= @EndTime " + phoneStr + @"
																 ) 
		                            AND AddTime >= @BeginTime 
		                            AND AddTime <= @EndTime 
		                            AND State <> 1 
	                            ) AS d ON c.id = d.id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ExaminerAnswerTotal,
		                            1 AS Id 
	                            FROM
		                            QuestionsAnswersInfos 
	                            WHERE
		                            UserId IN ( SELECT s.Id FROM PartTimeJobAdmins AS s 
																		RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
																	WHERE r.ParentId = @ParentId AND s.RegesitTime >= @BeginTime AND s.RegesitTime <= @EndTime " + phoneStr + @"
																) 
		                            AND CreateTime >= @BeginTime 
		                            AND CreateTime <= @EndTime 
		                            AND State <> 1 
	                            ) f ON f.Id = d.Id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS SelectedCommentTotal,
		                            1 AS Id 
	                            FROM
		                            SchoolComments 
	                            WHERE
		                            CommentUserId IN ( SELECT s.Id FROM PartTimeJobAdmins AS s 
																		RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
																	WHERE r.ParentId = @ParentId AND s.RegesitTime >= @BeginTime AND s.RegesitTime <= @EndTime " + phoneStr + @"
																 ) 
		                            AND AddTime >= @BeginTime 
		                            AND AddTime <= @EndTime 
		                            AND State = 3 
	                            ) AS g ON g.Id = f.Id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS SelectedAnswerTotal,
		                            1 AS Id 
	                            FROM
		                            QuestionsAnswersInfos 
	                            WHERE
		                            UserId IN ( SELECT s.Id FROM PartTimeJobAdmins AS s 
																		RIGHT JOIN PartTimeJobAdminRoles as r on s.Id = r.AdminId
																	WHERE r.ParentId = @ParentId AND s.RegesitTime >= @BeginTime AND s.RegesitTime <= @EndTime " + phoneStr + @"
																) 
		                            AND CreateTime >= @BeginTime 
		                            AND CreateTime <= @EndTime 
		                            AND State = 3 
	                            ) AS k ON k.id = g.id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ShieldJobTotal,
		                            1 AS Id 
	                            FROM
		                            PartTimeJobAdmins AS p
		                            RIGHT JOIN PartTimeJobAdminRoles AS r ON p.Id = r.AdminId 
		                            AND r.Shield = 1 
	                            WHERE
		                            p.RegesitTime >= @BeginTime 
		                            AND p.RegesitTime <= @EndTime 
                                "+ phoneStr1 + @"
		                            AND r.ParentId =@ParentId
	                            ) AS r ON r.id = k.id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ShieldJobCommentTotal,
		                            1 AS Id 
	                            FROM
		                            SchoolComments 
	                            WHERE
		                            CommentUserId IN (
		                            SELECT
			                            p.Id 
		                            FROM
			                            PartTimeJobAdmins AS p
			                            RIGHT JOIN PartTimeJobAdminRoles AS r ON p.Id = r.AdminId 
			                            AND r.Shield = 1 
		                            WHERE
			                            p.RegesitTime >= @BeginTime 
			                            AND p.RegesitTime <= @EndTime 
			                            AND r.ParentId =@ParentId
                                        " + phoneStr1 + @"
		                            ) 
		                            AND AddTime >= @BeginTime 
		                            AND AddTime <= @EndTime 
	                            ) AS w ON w.id = r.id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ShieldJobAnswerTotal,
		                            1 AS Id 
	                            FROM
		                            QuestionsAnswersInfos 
	                            WHERE
		                            UserId IN (
		                            SELECT
			                            p.Id 
		                            FROM
			                            PartTimeJobAdmins AS p
			                            RIGHT JOIN PartTimeJobAdminRoles AS r ON p.Id = r.AdminId 
			                            AND r.Shield = 1 
		                            WHERE
			                            p.RegesitTime >= @BeginTime 
			                            AND p.RegesitTime <= @EndTime 
			                            AND r.ParentId =@ParentId
                                        " + phoneStr1 + @"
		                            ) 
		                            AND CreateTime >= @BeginTime 
		                            AND CreateTime <= @EndTime 
	                            ) AS z ON z.id = r.id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ShieldJobSelectedCommentTotal,
		                            1 AS Id 
	                            FROM
		                            SchoolComments 
	                            WHERE
		                            CommentUserId IN (
		                            SELECT
			                            p.Id 
		                            FROM
			                            PartTimeJobAdmins AS p
			                            RIGHT JOIN PartTimeJobAdminRoles AS r ON p.Id = r.AdminId 
			                            AND r.Shield = 1 
		                            WHERE
			                            p.RegesitTime >= @BeginTime 
			                            AND p.RegesitTime <= @EndTime 
			                            AND r.ParentId =@ParentId
  " + phoneStr1 + @"
		                            ) 
		                            AND AddTime >= @BeginTime 
		                            AND AddTime <= @EndTime 
		                            AND State = 3 
	                            ) AS v ON v.id = z.id
	                            FULL JOIN (
	                            SELECT COUNT
		                            ( 1 ) AS ShieldJobSelectedAnswerTotal,
		                            1 AS Id 
	                            FROM
		                            QuestionsAnswersInfos 
	                            WHERE
		                            UserId IN (
		                            SELECT
			                            p.Id 
		                            FROM
			                            PartTimeJobAdmins AS p
			                            RIGHT JOIN PartTimeJobAdminRoles AS r ON p.Id = r.AdminId 
			                            AND r.Shield = 1 
		                            WHERE
			                            p.RegesitTime >= @BeginTime 
			                            AND p.RegesitTime <= @EndTime 
			                            AND r.ParentId =@ParentId
  " + phoneStr1 + @"
		                            ) 
		                            AND CreateTime >= @BeginTime 
		                            AND CreateTime <= @EndTime 
	                            AND State = 3 
	                            ) AS n ON n.id = v.id
                            ";
          


            para.Add(new SqlParameter("@ParentId", ParentId));
            para.Add(new SqlParameter("@BeginTime", BeginTime));
            para.Add(new SqlParameter("@EndTime", EndTime));
          

            return Query<SupplierTotal>(sql, para.ToArray()).FirstOrDefault();
        }

        public SysAdminQuerySupplierTotal SysAdminQuerySupplierTotal(DateTime BeginTime,DateTime EndTime) 
        {
            if (BeginTime.Year == 2019 && BeginTime.Month == 12)
            {
                BeginTime = DateTime.Parse("2019-12-11 00:00:00");
            }

            string sql = @"select
	                        d.ExaminerCommentTotal,
	                        f.SelectedCommentTotal,
                          c.CommitAnswerTotal,
	                        e.ExaminerAnswerTotal,
	                        g.SelectedAnswerTotal,
	                        h.ShieldJobTotal,
	                        i.ShieldJobCommentTotal,
	                        k.ShieldJobSelectedCommentTotal,
	                        j.ShieldJobAnswerTotal,
	                        l.ShieldJobSelectedAnswerTotal
                        from 
                                        (select count(a.Id) as CommitAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									                        LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime >= @BeginTime and e.ExamineTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                                        ) as c
                                        FULL JOIN
                                        (select count(c.Id) as ExaminerCommentTotal,1 as Id from SchoolComments as c
									                        LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime >= @BeginTime and e.UpdateTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where c.State <> 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime 
                                        ) as d on d.Id = c.Id
                                        FULL JOIN
                                        (select count(a.Id) as ExaminerAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									                        LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime >= @BeginTime and e.ExamineTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where a.State <> 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                                        ) as e on e.Id = d.Id
                                        FULL JOIN
                                        (select count(c.Id) as SelectedCommentTotal,1 as Id from SchoolComments as c
									                        LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime >= @BeginTime and e.UpdateTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where s.Role = 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime and c.State = 3
                                        ) as f on f.Id = e.Id
                                        FULL JOIN
                                        (select count(a.Id) as SelectedAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									                        LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime >= @BeginTime and e.ExamineTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where s.Role = 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime and a.State = 3
                                        ) as g on g.Id = f.Id
                                        FULL JOIN
                                        (select count(a.Id) as ShieldJobTotal,1 as Id from
	                                        PartTimeJobAdmins as a right join PartTimeJobAdminRoles as s on s.AdminId = a.Id
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where s.Shield = 1 and a.RegesitTime >= @BeginTime and a.RegesitTime <= @EndTime) as h on h.Id = g.Id
                                        FULL JOIN
                                        (
                                        select count(c.Id) as ShieldJobCommentTotal,1 as Id from SchoolComments as c
									                        LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime >= @BeginTime and e.UpdateTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where s.Shield = 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime
                                        ) as i on i.Id = h.Id
                                        FULL JOIN
                                        (select count(a.Id) as ShieldJobAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									                        LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime >= @BeginTime and e.ExamineTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = p.ParentId and p1.Role = 3
                                        where s.Shield = 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                                        ) as j on i.Id = j.Id
                                        FULL JOIN
                                        (select count(c.Id) as ShieldJobSelectedCommentTotal,1 as Id from SchoolComments as c
									                        LEFT JOIN SchoolCommentExamines as e on e.SchoolCommentId = c.Id and e.UpdateTime >= @BeginTime and e.UpdateTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on c.CommentUserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = s.ParentId and p1.Role = 3
                                        where s.Shield = 1 and s.Role = 1 and c.AddTime >= @BeginTime and c.AddTime <= @EndTime and c.State = 3
                                        ) as k on k.Id = j.Id
                                        FULL JOIN
                                        (select count(a.Id) as ShieldJobSelectedAnswerTotal,1 as Id from QuestionsAnswersInfos as a
									                        LEFT JOIN QuestionsAnswerExamines as e on e.QuestionsAnswersInfoId = a.Id and e.ExamineTime >= @BeginTime and e.ExamineTime <= @EndTime
	                                        RIGHT JOIN PartTimeJobAdminRoles as s on a.UserId = s.AdminId
	                                        RIGHT JOIN PartTimeJobAdminRoles as p on s.ParentId = p.AdminId
									                        RIGHT JOIN PartTimeJobAdminRoles as p1 on p1.AdminId = s.ParentId and p1.Role = 3
                                        where a.State = 3 and s.Shield = 1 and a.CreateTime >= @BeginTime and a.CreateTime <= @EndTime
                                        ) as l on l.Id = k.Id";

            SqlParameter[] para = {
                new SqlParameter("@BeginTime",BeginTime),
                new SqlParameter("@EndTime",EndTime)
            };

            return Query<SysAdminQuerySupplierTotal>(sql, para).FirstOrDefault();
        }

    }
}
