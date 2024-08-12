using ImportAndExport.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using PMS.CommentsManage.Domain.Entities;

namespace ImportAndExport.Repository
{
    public class SchoolRepository
    {
        private DataDbContext _context;
        public SchoolRepository(DataDbContext context)
        {
            _context = context;
        }


        public SettlementAmountMoney SettlementAmountMoneyByLeaderId(Guid leaderId) 
        {
            string sql = @"select sum(s.TotalSchoolCommentsSelected) as TotalSchoolCommentsSelected,sum(s.TotalAnswerSelected) as TotalAnswerSelected from PartTimeJobAdminRoles as r
	                        left join SettlementAmountMoneys as s on r.AdminId = s.PartTimeJobAdminId
                        where r.ParentId = @leaderId and s.PartJobRole = 1";

            return _context.Query<SettlementAmountMoney>(sql, new { leaderId }).FirstOrDefault();
        }

        public void UpdateQuestionReply(int total,Guid id) 
        {
            string sql = "update QuestionInfos set ReplyCount = @total where Id = @id";
            _context.Execute(sql, new { total, id });
        }

        public List<PartTimeJobAdmin> partTimeJobAdmins()
        {
            string sql = @"select p.* from PartTimeJobAdmins as p
	                        left join PartTimeJobAdminRoles as r on p.Id = r.AdminId
                        where p.SettlementType = 1 and r.Role = 1";
            return _context.Query<PartTimeJobAdmin>(sql, new { })?.ToList();
        }

        public List<PartTimeJobAdmin> getLeader() 
        {
            string sql = @"select p.* from PartTimeJobAdminRoles as r
	            left join PartTimeJobAdmins as p  on p.Id = r.AdminId
            where r.role = 2 and p.SettlementType = 1";
            return _context.Query<PartTimeJobAdmin>(sql, new { })?.ToList();
        }

        public List<SettlementAmountMoney> GetSettlementAmountMoney(List<Guid> AdminId) 
        {
            string sql = "select * from SettlementAmountMoneys where PartTimeJobAdminId in @AdminId and PartJobRole = 2";
            return _context.Query<SettlementAmountMoney>(sql, new { AdminId= AdminId }).ToList();
        }

        public void UpdateSettlement(Guid userId,int commentTotal,int answerTotal) 
        {
            string sql = "UPDATE SettlementAmountMoneys SET TotalSchoolCommentsSelected = @commentTotal ,TotalAnswerSelected = @answerTotal where PartJobRole = 2 and PartTimeJobAdminId = @userId";
            int i = _context.Execute(sql, new { commentTotal , answerTotal , userId });
        }

        public List<SchoolSectionIdAndSchoolId> GetSchoolIdByEids(List<Guid> Eids)
        {
            string sql = @"select 
	                        id as Eid,
	                        sid as Sid
                        from OnlineSchoolExtension where id in @Ids";

            SqlParameter[] para = {
                new SqlParameter("@Ids",Eids)
            };
            return _context.Query<SchoolSectionIdAndSchoolId>(sql, new { Ids = Eids })?.ToList();
        }

        public List<Guid> GetUserIds(int take) 
        {
            string sql = "select top " + take + " id from userinfotmp where status = 0";
            return _context.Query<Guid>(sql, new {  })?.ToList();
        }

        public bool Updateuserinfotmpstate(List<Guid> userIds) 
        {
            string sql = "update userinfotmp set Status += 1 where id in @userIds";
            return _context.Execute(sql, new { userIds }) > 0;
        }


        public List<PartTimeJobAdmin> GetPartJobAdminIds() 
        {
            string sql = @"select p.* from PartTimeJobAdmins as p
	                        left join PartTimeJobAdminRoles as r on p.Id = r.AdminId 
		                        where r.id is null
	                        order by RegesitTime desc";

            return _context.Query<PartTimeJobAdmin>(sql,new { })?.ToList();
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdmin(List<Guid> Ids) 
        {
            string sql = "select * from PartTimeJobAdmins where Id in @Ids";
            return _context.Query<PartTimeJobAdmin>(sql, new { Ids = Ids }).ToList();
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdminRoles(List<Guid> Ids) 
        {
            string sql = @"	select 
		                        p.Id,
		                        r.Role
	                        from PartTimeJobAdminRoles as r
		                        left join PartTimeJobAdmins as p on r.AdminId = p.Id
	                        where r.AdminId in @Ids ";

            SqlParameter[] para = 
            {
                new SqlParameter("@Ids",Ids)
            };

            return _context.Query<PartTimeJobAdmin>(sql, new { Ids = Ids })?.ToList();
        }

        public List<SelectedTotals> selectedTotals(List<Guid> Ids) 
        {
            string sql = @"select count(1) as SelectedTotal,CommentUserId from SchoolComments 
	                        where State = 3 and CommentUserId in @Ids
		                        GROUP BY CommentUserId";

            SqlParameter[] para = {
                new SqlParameter("@Ids",Ids)
            };
            return _context.Query<SelectedTotals>(sql, new { Ids = Ids })?.ToList();
        }

        public List<SelectedTotals> QselectedTotals(List<Guid> Ids)
        {
            string sql = @"select count(1) as SelectedTotal,UserId as  CommentUserId from QuestionsAnswersInfos
	                            where State = 3 and UserId in @Ids
		                            GROUP BY UserId";

            SqlParameter[] para = {
                new SqlParameter("@Ids",Ids)
            };
            return _context.Query<SelectedTotals>(sql, new { Ids = Ids })?.ToList();
        }

        public void Insert(List<SettlementAmountMoney> amountMoneys) 
        {
            string sqlB = @"insert into SettlementAmountMoneys values(NEWID(),@PartTimeJobAdminId,@TotalSchoolCommentsSelected,@TotalAnswerSelected,@BeginTime,@EndTime,@AddTime,@SettlementAmount,@SettlementStatus,@PartJobRole)";
            int x = _context.Execute(sqlB, amountMoneys);
        }

        public void InsertPartAdminRole(List<PartTimeJobAdminRole> adminRoles,List<SettlementAmountMoney> amountMoneys) 
        {
            string sqlA = "insert into PartTimeJobAdminRoles VALUES(@AdminId,@Role,@CreateTime,@ParentId,DEFAULT)";
            string sqlB = @"insert into SettlementAmountMoneys values(NEWID(),@PartTimeJobAdminId,@TotalSchoolCommentsSelected,@TotalAnswerSelected,@BeginTime,@EndTime,@AddTime,@SettlementAmount,@SettlementStatus,@PartJobRole)";

            int e = _context.Execute(sqlA, adminRoles);
            int x =_context.Execute(sqlB, amountMoneys);

            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    if (e == x)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        
                    }
                }
                catch (Exception ex)
                {
                    //todo:!!!transaction rollback can not work.

                    //回滚事务
                    transaction.Rollback();
                    
                }
                finally
                {
                    _context.Dispose();
                }
            }
        }

    }
}
