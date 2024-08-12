using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.IRepositories;

namespace PMS.CommentsManage.Repository.Repositories.ProcViewRepositories
{
    public class SettlementViewRepository : EntityFrameworkRepository<SettlementView>, ISettlementViewRepository
    {
        readonly ISettlementAmountMoneyRepository settlementrepository;
        readonly IExaminerRecordRepository examinerRecordrepository;

        public SettlementViewRepository(CommentsManageDbContext repository,
            ISettlementAmountMoneyRepository settlementrepo, IExaminerRecordRepository examinerRecordrepo):base(repository)
        {
            settlementrepository = settlementrepo;
            examinerRecordrepository = examinerRecordrepo;
        }



        public List<SettlementAmountMoney> GetSettlementViewByIds(List<Guid> Ids) 
        {
            if (!Ids.Any()) 
            {
                return new List<SettlementAmountMoney>();
            }
            return settlementrepository.GetList(x => Ids.Contains(x.Id))?.ToList();
        }

        public List<SettlementView> GetSettlementViews(Guid ParentId, int Status, int PageIndex, int PageSize, int TotalSearch, DateTime QueryTime, out int Total)
        {
            DateTime BeginTime = QueryTime;

            DateTime EndTime  = new DateTime(BeginTime.Year, BeginTime.Month, BeginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);

            List<SqlParameter> para = new List<SqlParameter>();

            para.Add(new SqlParameter("@ParentId", ParentId));
            para.Add(new SqlParameter("@PageIndex", PageIndex));
            para.Add(new SqlParameter("@PageSize", PageSize));
            //para.Add(new SqlParameter("@Status", Status));

            string stateQuery = "";
            if (Status == 7)
            {
                stateQuery = "(0,4,-1) ";
            }
            else if (Status == 8)
            {
                stateQuery = "(3) ";
            }
            else 
            {
                stateQuery = "(1,2) ";
            }

            List<SqlParameter> total = new List<SqlParameter>();


            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("select * from (select row_number() over(order by m.Id) as rowIndex,m.Id SettlementId,a.Id as AdminId,r.ParentId as ParentId,");
            //stringBuilder.Append("	r1.Total AS JobTotal,");
            //stringBuilder.Append(" a.Name as AdminNmae,a.Phone,a.Role,a.SettlementType,m.TotalAnswerSelected,m.TotalSchoolCommentsSelected,m.SettlementAmount,");
            //stringBuilder.Append("m.SettlementStatus,m.BeginTime,m.EndTime,e.ExaminerTime as PassTime");
            //stringBuilder.Append(" from SettlementAmountMoneys as m left join PartTimeJobAdminRoles as r ON r.AdminId = m.PartTimeJobAdminId  and r.role = m.partjobrole left join (select count(1) as Total,ParentId from PartTimeJobAdminRoles GROUP BY ParentId) as r1 on r1.ParentId = m.PartTimeJobAdminId inner join PartTimeJobAdmins as a on m.PartTimeJobAdminId = a.Id ");
            //stringBuilder.Append("	left join ExaminerRecords as e on m.id = e.TargetId and e.ChangeAfterStatus = 3  where a.ParentId = @ParentId and ");

            //if (QueryTime.Year != 2000)
            //{
            //    para.Add(new SqlParameter("@BeginTime", BeginTime));
            //    para.Add(new SqlParameter("@EndTime", EndTime));
            //    stringBuilder.Append(" m.BeginTime >= @BeginTime and m.EndTime <= @EndTime ");
            //}
            //else 
            //{
            //    para.Add(new SqlParameter("@BeginTime", BeginTime));
            //    stringBuilder.Append(" m.BeginTime >= @BeginTime ");
            //}

            //stringBuilder.Append("  and m.SettlementStatus = @Status) as tab  ");
            //stringBuilder.Append(" where rowIndex between @PageSize * (@PageIndex - 1) +  1 and @PageSize * @PageIndex");

            string stringBuilder = @"SELECT
	                                    * 
                                    FROM
	                                    (
	                                    SELECT
		                                    row_number () OVER ( ORDER BY m.Id ) AS rowIndex,
		                                    m.Id SettlementId,
		                                    a.Id AS AdminId,
		                                    r.ParentId AS ParentId,
		                                    r1.Total AS JobTotal,
			                                    a.Name AS AdminNmae,
			                                    a.Phone,
			                                    a.Role,
			                                    a.SettlementType,
			                                    m.TotalAnswerSelected,
			                                    m.TotalSchoolCommentsSelected,
			                                    m.SettlementAmount,
			                                    m.SettlementStatus,
			                                    m.BeginTime,
			                                    m.EndTime,
			                                    e.ExaminerTime AS PassTime 
		                                    FROM
			                                    SettlementAmountMoneys AS m
			                                    right join PartTimeJobAdminRoles as r ON r.AdminId = m.PartTimeJobAdminId and r.role = m.partjobrole
			                                    left join (select count(1) as Total,ParentId from PartTimeJobAdminRoles GROUP BY ParentId) as r1 on r1.ParentId = r.AdminId
			                                    left JOIN PartTimeJobAdmins AS a ON r.AdminId = a.Id
			                                    LEFT JOIN ExaminerRecords AS e ON m.id = e.TargetId 
			                                    AND e.ChangeAfterStatus = 3 
		                                    WHERE
			                                    r.ParentId = @ParentId AND";


                                    if (QueryTime.Year != 2000)
                                    {
                                        para.Add(new SqlParameter("@BeginTime", BeginTime));
                                        para.Add(new SqlParameter("@EndTime", EndTime));
                                        stringBuilder+=" m.BeginTime >= @BeginTime and m.EndTime <= @EndTime ";
                                    }
                                    else
                                    {
                                        para.Add(new SqlParameter("@BeginTime", BeginTime));
                                        stringBuilder += " m.BeginTime >= @BeginTime ";
                                    }

                                    if (TotalSearch != 0)
                                    {
                                        stringBuilder+=" AND (m.TotalAnswerSelected + m.TotalSchoolCommentsSelected) >= @TotalSearch ";
                                        para.Add(new SqlParameter("@TotalSearch", TotalSearch));
                                    }

            stringBuilder +=  @"  AND m.SettlementStatus in "+ stateQuery + @" 
		                                    ) AS tab 
	                                    WHERE
	                                    rowIndex between @PageSize * (@PageIndex - 1) +  1 and @PageSize * @PageIndex";

            string sql = @"	select
		                            count(1)
	                            FROM
			                            SettlementAmountMoneys AS m
			                            right join PartTimeJobAdminRoles as r ON r.AdminId = m.PartTimeJobAdminId and r.role = m.partjobrole
			                            left join (select count(1) as Total,ParentId from PartTimeJobAdminRoles GROUP BY ParentId) as r1 on r1.ParentId = r.AdminId
			                            left JOIN PartTimeJobAdmins AS a ON r.AdminId = a.Id
			                            LEFT JOIN ExaminerRecords AS e ON m.id = e.TargetId 
			                            AND e.ChangeAfterStatus = 3 
		                            WHERE
			                            r.ParentId = @ParentId 
			                            AND m.SettlementStatus in " + stateQuery;

            if (TotalSearch != 0) 
            {
                sql += " AND (m.TotalAnswerSelected + m.TotalSchoolCommentsSelected) >= @TotalSearch ";
                total.Add(new SqlParameter("@TotalSearch", TotalSearch));
            }

            
            total.Add(new SqlParameter("@ParentId", ParentId));
            //total.Add(new SqlParameter("@Status", Status));
            total.Add(new SqlParameter("@BeginTime", BeginTime));
            if (QueryTime.Year != 2000)
            {
                sql += " and BeginTime >= @BeginTime and EndTime <= @EndTime ";
                //EndTime = QueryTime.AddDays(14).AddHours(23).AddMinutes(59).AddSeconds(59);
                total.Add(new SqlParameter("@EndTime", EndTime));
            }
            else
            {
                sql += " and BeginTime >= @BeginTime ";
            }

            //SqlParameter[] total = {
            //    new SqlParameter("@ParentId",ParentId),
            //    new SqlParameter("@BeginTime",BeginTime),
            //    new SqlParameter("@EndTime",EndTime),
            //    new SqlParameter("@Status",Status)
            //};

            Total = (int)base.ExecuteScalar(sql, total.ToArray());
            var data= base.Query(stringBuilder.ToString(), para.ToArray());
            if (data == null)
                return null;
            return data.ToList();
        }

        public int UpdateStatus(Guid AdmindId,string Ids,int Stauts)
        {
            string[] SettlementIds = Ids.Split(',');
            int SuccessCount = 0;

            for (int i = 0; i < SettlementIds.Length; i++)
            {
                Guid CurrentId =  Guid.Parse(SettlementIds[i]);
                var cuurentSettlementData = settlementrepository.GetModelById(CurrentId);

                ExaminerRecord examinerRecord = new ExaminerRecord();
                examinerRecord.Id = Guid.NewGuid();
                examinerRecord.AdminId = AdmindId;
                examinerRecord.ExaminerType = ExaminerRecordType.Settlement;
                examinerRecord.TargetId = CurrentId;
                examinerRecord.ChangeFirstStatus = (int)cuurentSettlementData.SettlementStatus;
                examinerRecord.ChangeAfterStatus = Stauts;
                examinerRecordrepository.Insert(examinerRecord);

                cuurentSettlementData.SettlementStatus = (SettlementStatus)Stauts;
                cuurentSettlementData.SettlementAmount = (cuurentSettlementData.TotalSchoolCommentsSelected + cuurentSettlementData.TotalAnswerSelected) >= 5 && (cuurentSettlementData.TotalSchoolCommentsSelected + cuurentSettlementData.TotalAnswerSelected) < 10 ? 15 : (cuurentSettlementData.TotalSchoolCommentsSelected + cuurentSettlementData.TotalAnswerSelected) >= 10 ? 30 : 0;
                SuccessCount += settlementrepository.Update(cuurentSettlementData);
            }

            return SuccessCount == SettlementIds.Length ? 1 : 0;
        }

        public List<SettlementStatusModel> GetSettlementStatuses() 
        {
            string sql = @"select * from
                    (select m.Id,
				         r.Status,
                         m.PartTimeJobAdminId,
				         row_number () OVER ( partition BY r.DataSourceId ORDER BY r.SendTime desc ) AS OrderD from SettlementAmountMoneys as m
		        left join [iSchoolProduct].[dbo].[PartJobTimeSettlementRe] as r on m.Id = r.DataSourceId
			        where m.SettlementStatus = 2 and Status is not null) as t where t.OrderD = 1";
            return Query<SettlementStatusModel>(sql, null)?.ToList();
        }

    }
}
