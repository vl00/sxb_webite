using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.CommentsManage.Domain.Common;
using System.Linq;
using System.Linq.Dynamic;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System.Data.SqlClient;
using PMS.CommentsManage.Application.Common;
using System.Linq.Expressions;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SettlementAmountMoneyRepository : EntityFrameworkRepository<SettlementAmountMoney> , ISettlementAmountMoneyRepository
    {
        private readonly ISchoolCommentRepository commentRepository;
        private readonly IQuestionsAnswersInfoRepository answerRepository;

        private readonly IPartTimeJobAdminRolereRepository _partTimeJobAdmin;

        private readonly IPartTimeJobAdminRepository _partJobAdmin;

        public SettlementAmountMoneyRepository(CommentsManageDbContext repository, ICommentsManageDbContext dbContext, 
            IPartTimeJobAdminRolereRepository partTimeJobAdmin, IPartTimeJobAdminRepository partJobAdmin,
            ISchoolCommentRepository commentRepo, IQuestionsAnswersInfoRepository answerRepo
            ) : base(repository)
        {
            commentRepository = commentRepo;
            answerRepository = answerRepo;

            _partTimeJobAdmin = partTimeJobAdmin;
            _partJobAdmin = partJobAdmin;
        }


        public new int Add(SettlementAmountMoney enetity)
        {
            return base.Add(enetity);
        }

        public SettlementAmountMoney GetNewSettlement(Guid AdminId,DateTime write) 
        {
            return base.GetList(x => x.PartTimeJobAdminId == AdminId && x.PartJobRole == 1 && (x.SettlementStatus == SettlementStatus.Ongoing || x.SettlementStatus == SettlementStatus.DelaySettlement) && write >= x.BeginTime  && write <= x.EndTime).FirstOrDefault();
        }

        /// <summary>
        /// 管理员结算
        /// </summary>
        public void Settlement()
        {
            var AllSupplierAdmin = _partJobAdmin.GetList(x => x.Role == AdminUserRole.JobMember || x.Role == AdminUserRole.JobLeader);
            foreach (var item in AllSupplierAdmin)
            {
                //检测兼职人员是否还存在延期的任务结算
                if (item.Role == AdminUserRole.JobMember)
                {
                    var DelaySettlement = item.SettlementAmountMoneys.Where(x => x.PartJobRole == 1 && x.SettlementStatus == SettlementStatus.DelaySettlement || x.SettlementStatus == SettlementStatus.Delay);
                    foreach (var delayData in DelaySettlement)
                    {
                        int TotalSelected = delayData.TotalSchoolCommentsSelected + delayData.TotalAnswerSelected;
                        
                        if (delayData.SettlementStatus == SettlementStatus.DelaySettlement)
                        {
                            //延期任务已完成，可正常结算
                            delayData.SettlementAmount = AdminSettlementFun.JobAdminComputeBalance(new SettlementView() { Role = (int)item.Role, TotalAnswerSelected = delayData.TotalAnswerSelected, TotalSchoolCommentsSelected = delayData.TotalSchoolCommentsSelected }); ;
                            delayData.SettlementStatus = SettlementStatus.Unsettled;
                        }
                        else
                        {
                            //继续延期

                            //检测是否还存在可审核的数据
                            var UnreadCommentTotal = commentRepository.GetList(x => x.CommentUserId == item.Id && x.State == ExamineStatus.Unread && x.AddTime >= delayData.BeginTime && x.AddTime <= delayData.EndTime)?.Count();
                            var UnreadAnswerTotal = answerRepository.GetList(x => x.UserId == item.Id && x.State == ExamineStatus.Unread && x.CreateTime >= delayData.BeginTime && x.CreateTime <= delayData.EndTime)?.Count();

                            //检测剩余未审核的数据总条数能否完成当期任务
                            if ((UnreadCommentTotal + UnreadAnswerTotal) >= 5 - TotalSelected)
                            {
                                delayData.SettlementStatus = SettlementStatus.Delay;
                            }
                            else
                            {
                                delayData.SettlementStatus = SettlementStatus.Fail;
                            }
                        }

                        delayData.AddTime = DateTime.Now;
                        //提交本次延期任务状态
                        base.Update(delayData);
                    }
                }

                //检测当前是否为结算日期
                var settlement = base.GetList(x => x.PartTimeJobAdminId == item.Id && DateTime.Now >= x.EndTime).FirstOrDefault();
                if (settlement != null)
                {
                    //该阶段，已成功入选精选的点评、问答
                    int TotalSelected = settlement.TotalSchoolCommentsSelected + settlement.TotalAnswerSelected;
                    
                    //供应商结算 | 兼职领队结算
                    if (item.Role == AdminUserRole.Supplier || item.Role == AdminUserRole.JobLeader)
                    {
                        settlement.SettlementAmount = AdminSettlementFun.JobAdminComputeBalance(new SettlementView() { Role = (int)item.Role,TotalAnswerSelected = settlement.TotalAnswerSelected, TotalSchoolCommentsSelected = settlement.TotalSchoolCommentsSelected });
                        settlement.SettlementStatus = SettlementStatus.Unsettled;
                    }
                    //兼职结算
                    else if (item.Role == AdminUserRole.JobMember)
                    {
                        if (TotalSelected >= 5)
                        {
                            //该阶段任务完成，可正常结算
                            settlement.SettlementAmount = AdminSettlementFun.JobAdminComputeBalance(new SettlementView() { Role = (int)item.Role, TotalAnswerSelected = settlement.TotalAnswerSelected, TotalSchoolCommentsSelected = settlement.TotalSchoolCommentsSelected }); ;
                            settlement.SettlementStatus = SettlementStatus.Unsettled;
                        }
                        else
                        {
                            //检测是否还存在可审核的数据
                            var UnreadCommentTotal = commentRepository.GetList(x => x.CommentUserId ==item.Id &&  x.State == ExamineStatus.Unread && x.AddTime >= settlement.BeginTime && x.AddTime <= settlement.EndTime)?.Count();
                            var UnreadAnswerTotal = answerRepository.GetList(x => x.UserId == item.Id && x.State == ExamineStatus.Unread && x.CreateTime >= settlement.BeginTime && x.CreateTime <= settlement.EndTime)?.Count();

                            //检测剩余未审核的数据总条数能否完成当期任务
                            if ((UnreadCommentTotal + UnreadAnswerTotal) >= 5 - TotalSelected)
                            {
                                settlement.SettlementStatus = SettlementStatus.Delay;
                            }
                            else
                            {
                                settlement.SettlementStatus = SettlementStatus.Fail;
                            }
                        }
                    }
                    settlement.AddTime = DateTime.Now;
                    //提交本次结算
                    Update(settlement);
                    NextSettlementData(item,settlement.PartJobRole);
                }
            }
        }

        //得到该管理员下一次结算数据
        public int NextSettlementData(PartTimeJobAdmin admin,int PartJobRole)
        {//_partJobAdmin
            SettlementAmountMoney settlement = new SettlementAmountMoney();
            settlement.Id = Guid.NewGuid();
            settlement.PartTimeJobAdminId = admin.Id;


            //if (admin.SettlementType == SettlementType.SettlementContract)
            //{
                //供应商下一次结算日期
                if ((AdminUserRole)PartJobRole == AdminUserRole.Supplier)
                {
                    settlement.BeginTime = DateTime.Now;
                    DateTime CurrentDateLastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1);
                    DateTime LastDay = new DateTime(CurrentDateLastDay.Year, CurrentDateLastDay.Month, CurrentDateLastDay.Day, 23, 25, 59);
                    settlement.EndTime = LastDay;
                }
                //兼职、领队
                else if ((AdminUserRole)PartJobRole == AdminUserRole.JobMember || (AdminUserRole)PartJobRole == AdminUserRole.JobLeader)
                {
                    var upperSettlement = GetList(x => x.PartTimeJobAdminId == admin.Id && x.PartJobRole ==  PartJobRole).OrderByDescending(x => x.EndTime).FirstOrDefault();

                    if (upperSettlement == null) 
                    {
                        var parent = _partJobAdmin.GetTopParent(admin.Id, admin.PartTimeJobAdminRoles.OrderByDescending(x => x.Role).Select(x => x.Role).FirstOrDefault());
                        var parentsettlem = GetList(x => x.PartTimeJobAdminId == parent.Id && x.SettlementStatus == SettlementStatus.Ongoing).FirstOrDefault();
                        settlement.BeginTime = parentsettlem.BeginTime;
                        //DateTime CurrentDate = DateTime.Now.AddDays(+14);
                        //DateTime NextSettlementTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day, 23, 59, 59);
                        settlement.EndTime = parentsettlem.EndTime;
                    }
                    else 
                    {
                        //settlement.BeginTime = upperSettlement.BeginTime.AddMonths(+1);
                        //DateTime CurrentDateLastDay = upperSettlement.EndTime.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1);
                        //settlement.EndTime = upperSettlement.EndTime.AddMonths(+1);
                        var nextTime = upperSettlement.BeginTime.AddMonths(+1);
                        DateTime CurrentBeginTime = new DateTime(nextTime.Year, nextTime.Month, 1, 0, 0, 0);
                        DateTime CurrentEndTime = new DateTime(CurrentBeginTime.Year, CurrentBeginTime.Month, CurrentBeginTime.AddMonths(+1).AddDays(-1).Day, 23, 59, 59, 59);
                        settlement.BeginTime = CurrentBeginTime;
                        settlement.EndTime = CurrentEndTime;
                    }
                }
            //}
            //else 
            //{
            //    //供应商下一次结算日期
            //    if ((AdminUserRole)PartJobRole == AdminUserRole.Supplier)
            //    {
            //        settlement.BeginTime = DateTime.Now;
            //        DateTime CurrentDateLastDay = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1);
            //        DateTime LastDay = new DateTime(CurrentDateLastDay.Year, CurrentDateLastDay.Month, CurrentDateLastDay.Day, 23, 25, 59);
            //        settlement.EndTime = LastDay;
            //    }
            //    //兼职、领队
            //    else if ((AdminUserRole)PartJobRole == AdminUserRole.JobMember || (AdminUserRole)PartJobRole == AdminUserRole.JobLeader)
            //    {
            //        settlement.BeginTime = DateTime.Now;
            //        DateTime CurrentDate = DateTime.Now.AddDays(+14);
            //        DateTime NextSettlementTime = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day, 23, 59, 59);
            //        settlement.EndTime = NextSettlementTime;
            //    }
            //}

            settlement.SettlementAmount = 0;
            settlement.SettlementStatus = SettlementStatus.Ongoing;
            settlement.TotalAnswerSelected = 0;
            settlement.TotalSchoolCommentsSelected = 0;
            settlement.PartJobRole = PartJobRole;

            return base.Add(settlement);
        }

        /// <summary>
        /// 修改兼职父级的结算数据
        /// </summary>
        /// <param name="JobAdminId"></param>
        /// <param name="SelectedType">1：点评，2：问答</param>
        /// <returns></returns>
        public int UpdateSettlementSelectedTotal(Guid JobAdminId,int SelectedType,DateTime WriteTime)
        {
            int TaskCommentJob = 0, leaderCommentTask = 0, supplierCommentTask = 0;
            int TaskAnswerJob = 0, leaderAnswerTask = 0, supplierAnswerTask = 0;

            //兼职信息
            PartTimeJobAdminRole partTime = _partTimeJobAdmin.GetList(x => x.AdminId == JobAdminId && x.Role == 1).FirstOrDefault();
            //获取当前兼职的领队数据
            //PartTimeJobAdmin Leader = adminRepository.GetList(x => x.Id == partTime.ParentId).FirstOrDefault();
            PartTimeJobAdminRole Leader = _partTimeJobAdmin.GetList(x => x.AdminId == partTime.ParentId && x.Role == 2).FirstOrDefault();
            //获取该领队的供应商
            //PartTimeJobAdmin supplier = adminRepository.GetList(x => x.Id == Leader.ParentId).FirstOrDefault();
            //得到该点评所处的任务周期
            var JobTask = base.GetList(x => (x.PartTimeJobAdminId == JobAdminId && x.PartJobRole == 1 && x.BeginTime <= WriteTime && x.EndTime >= WriteTime) && (x.SettlementStatus == SettlementStatus.Ongoing || x.SettlementStatus == SettlementStatus.Delay)).FirstOrDefault();

            if(JobTask == null) 
            {
                return 1;
            }

            //兼职点评
            TaskCommentJob = JobTask.TotalSchoolCommentsSelected;
            //兼职回答
            TaskAnswerJob = JobTask.TotalAnswerSelected;


            if (SelectedType == 1)
            {
                //JobTask.TotalSchoolCommentsSelected += 1;
                TaskCommentJob += 1;
            }
            else
            {
                //JobTask.TotalAnswerSelected += 1;
                TaskAnswerJob += 1;
            }

            //检测该任务是否为延期任务
            if (JobTask.SettlementStatus == SettlementStatus.Delay)
            {
                //该任务已成功入选精选点评的总数
                int SelectedTotal = JobTask.TotalSchoolCommentsSelected + JobTask.TotalAnswerSelected;

                //此次是否能正常结算
                if (SelectedTotal >= 5)
                {
                    //在本期任务周期完结后进行结算
                    JobTask.SettlementStatus = SettlementStatus.DelaySettlement;
                }
                else
                {
                    //检测是否还存在为审核的记录
                    var UnreadCommentTotal = commentRepository.GetList(x => x.CommentUserId == JobAdminId && x.State == ExamineStatus.Unread && x.AddTime >= JobTask.BeginTime && x.AddTime <= JobTask.EndTime)?.Count();
                    var UnreadAnswerTotal = answerRepository.GetList(x => x.UserId == JobAdminId && x.State == ExamineStatus.Unread && x.CreateTime >= JobTask.BeginTime && x.CreateTime <= JobTask.EndTime)?.Count();

                    //存在的点评以不足完成任务，此次任务失败
                    if ((UnreadCommentTotal + UnreadAnswerTotal) < (5 - SelectedTotal))
                    {
                        JobTask.SettlementStatus = SettlementStatus.Fail;
                    }
                }
            }

            //修改领队当前结算记录表
            SettlementAmountMoney leaderSettlement = GetList(x => x.PartTimeJobAdminId == Leader.AdminId && x.PartJobRole == 2 && x.SettlementStatus == SettlementStatus.Ongoing).FirstOrDefault();

            //检测该兼职领队是否存在开启的任务，如果没开启则进行自动补充一个任务
            if (leaderSettlement == null)
            {
                Guid LeaderTaskJobId = Guid.NewGuid();
                Add(new SettlementAmountMoney() { 
                    Id = LeaderTaskJobId,
                    AddTime = DateTime.Now,
                    PartTimeJobAdminId = Leader.AdminId,
                    PartJobRole = 2,
                    SettlementAmount = 0,
                    TotalAnswerSelected = 0,
                    TotalSchoolCommentsSelected = 0,
                    SettlementStatus = SettlementStatus.Ongoing,
                    BeginTime = JobTask.BeginTime,
                    EndTime = JobTask.EndTime
                });

                leaderSettlement = GetAggregateById(LeaderTaskJobId);

                //领队旗下点评
                leaderCommentTask = leaderSettlement.TotalSchoolCommentsSelected;
                //领队旗下问答
                leaderAnswerTask = leaderSettlement.TotalAnswerSelected;
            }
            else 
            {
                //领队旗下点评
                leaderCommentTask = leaderSettlement.TotalSchoolCommentsSelected;
                //领队旗下问答
                leaderAnswerTask = leaderSettlement.TotalAnswerSelected;
            }
            
            
            
            //供应商当前任务结算记录表
            //var supplierSettlement = GetList(x => x.PartTimeJobAdminId == supplier.Id && x.SettlementStatus == SettlementStatus.Ongoing).FirstOrDefault();
            ////供应商旗下点评
            //supplierCommentTask = supplierSettlement.TotalSchoolCommentsSelected;
            ////供应商旗下问答
            //supplierAnswerTask = supplierSettlement.TotalAnswerSelected;
            
            if (SelectedType == 1)
            {
                leaderCommentTask += 1;
                //supplierCommentTask += 1;
                //leaderSettlement.TotalSchoolCommentsSelected += 1;
                //supplierSettlement.TotalSchoolCommentsSelected += 1;
            }
            else
            {
                leaderAnswerTask += 1;
                //supplierAnswerTask += 1;
                //leaderSettlement.TotalAnswerSelected += 1;
                //supplierSettlement.TotalAnswerSelected += 1;
            }

            //供应商
            //supplierSettlement.TotalSchoolCommentsSelected = supplierCommentTask;
            //supplierSettlement.TotalAnswerSelected = supplierAnswerTask;

            //兼职领队
            leaderSettlement.TotalAnswerSelected = leaderAnswerTask;
            leaderSettlement.TotalSchoolCommentsSelected = leaderCommentTask;

            //兼职
            JobTask.TotalAnswerSelected = TaskAnswerJob;
            JobTask.TotalSchoolCommentsSelected = TaskCommentJob;
            
            Update(leaderSettlement);
            //Update(supplierSettlement);
            Update(JobTask);

            return 1;
        }

        //public void JobSettlement()
        //{

        //    /*
        //        var rez = adminRepository.GetList(x => x.Role == UserRole.JobLeader || x.Role == UserRole.JobMember);
        //        foreach (var item in rez)
        //        {
        //            int CommentSelected = 0;
        //            int AnswerSelected = 0;

        //            SettlementAmountMoney amountMoney = new SettlementAmountMoney();
        //            amountMoney.Id = Guid.NewGuid();
        //            amountMoney.PartTimeJobAdminId = item.Id;
        //            amountMoney.AddTime = DateTime.Now;

        //            //从未结算过，第一次结算
        //            if (item.SettlementAmountMoneys == null)
        //            {
        //                //14天计算日期开始时间
        //                //结算日期是否为第14天晚上23.59分，还是说需要精确到入驻系统中的时分秒

        //                //时间检测
        //                TimeSpan timeSpan = item.RegesitTime - DateTime.Now;
        //                if (timeSpan.Days >= 14)
        //                {
        //                    //if (item.Role == UserRole.JobLeader)
        //                    //{
        //                    //    //统计该兼职领队下所有兼职人员的精选数
        //                    //    var CurrentLeaderAllJob = adminRepository.GetList(x => x.ParentId == item.Id).ToList();
        //                    //    foreach (var Job in CurrentLeaderAllJob)
        //                    //    {
        //                    //        //当期所有的精选点评、问答
        //                    //        CommentSelected += commentRepository.GetCount(x => x.CommentUserId == item.Id && x.AddTime >= item.RegesitTime && x.AddTime <= DateTime.Now && x.State == ExamineStatus.Highlight);
        //                    //        AnswerSelected += answerRepository.GetCount(x => x.UserId == item.Id && x.CreateTime >= item.RegesitTime && x.CreateTime <= DateTime.Now && x.State == ExamineStatus.Highlight);
        //                    //    }

        //                    //    amountMoney.TotalAnswerSelected += AnswerSelected;
        //                    //    amountMoney.TotalSchoolCommentsSelected += CommentSelected;
        //                    //}
        //                    //else
        //                    //{
        //                        //兼职结算统计（统计（入驻系统时间），与当前时间内已精选的点评，且未结算）
        //                        var Comment = commentRepository.GetList(x => x.CommentUserId == item.Id && x.AddTime >= item.RegesitTime && x.AddTime <= DateTime.Now);
        //                        var Answer  = answerRepository.GetList(x => x.UserId == item.Id && x.CreateTime >= item.RegesitTime && x.CreateTime <= DateTime.Now);

        //                        amountMoney.TotalSchoolCommentsSelected = Comment != null ? Comment.Where(x => x.State == ExamineStatus.Highlight).Count() : 0;
        //                        amountMoney.TotalAnswerSelected = Answer != null ? Answer.Where(x => x.State == ExamineStatus.Highlight).Count() : 0;

        //                        int NotExaminerComment = Comment != null ? Comment.Where(x => x.State == ExamineStatus.Unread).Count() : 0;
        //                        int NotExaminerAnswer = Answer != null ? Answer.Where(x => x.State == ExamineStatus.Unread).Count() : 0;
                            
        //                        if (CommentSelected + AnswerSelected >= 5)
        //                        {
        //                            //当前周期任务已完成
        //                            amountMoney.SettlementStatus = SettlementStatus.Unsettled;
        //                            amountMoney.SettlementAmount = 35;

        //                            //将该阶段所有点评、问答，状态设置为已结算
        //                        }
        //                        else
        //                        {
        //                            //当前周期任务未完成，检测是否还存有未审核的数据
                                
        //                            //还差的指标数据值
        //                            int LackIndex = 5 - (CommentSelected + AnswerSelected);
        //                            //检测剩余的数据总额是否能完成当期任务
        //                            if (NotExaminerAnswer >= LackIndex || NotExaminerComment >= LackIndex)
        //                            {
        //                                //当期任务延后结算，在下一次任务期再进行检测
        //                                amountMoney.SettlementStatus = SettlementStatus.Delay;
        //                            }
        //                            else
        //                            {
        //                                //当期任务失败
        //                                amountMoney.SettlementStatus = SettlementStatus.Fail;
        //                            }
        //                        }
        //                   // }
        //                }
        //            }
        //            else
        //            {
        //                //存在结算记录
        //                var newData = item.SettlementAmountMoneys.OrderBy("AddTime desc").FirstOrDefault();
        //                TimeSpan timeSpan = newData.AddTime - DateTime.Now;
        //                if (timeSpan.Days >= 14)
        //                {

        //                }
        //            }
        //        }
        //    */

        //}

        public new int Update(SettlementAmountMoney enetity)
        {
            return base.Update(enetity);
        }

        public List<PartTimeJob> GetPartTimeJob() 
        {
            string sql = @"select 
			                         s.Id as JobId,
			                         s.TotalSchoolCommentsSelected,
			                         s.TotalAnswerSelected,
			                         u.id as UserId,
			                         u.nickname,
			                         o.appName,
			                         o.openID
	                        from [iSchoolProduct].[dbo].SettlementAmountMoneys as s
	                        left join [iSchoolUser].[dbo].UserInfo as u	on s.PartTimeJobAdminId = u.Id
	                        left JOIN [iSchoolUser].[dbo].[openid_weixin] as o on o.userID = u.Id
                        where s.PartTimeJobAdminId = 'D7DE3C3A-4C2D-4B11-8FE8-822022CB3D94'";

            return base.Query<PartTimeJob>(sql, null)?.ToList();
        }

        public List<JobSettlementMoney> settlementMoney(List<Guid> AdminIds) 
        {
            List<SqlParameter> para = AdminIds.Select((e, i) => new SqlParameter("@" + i, e)).ToList();
            string paraStr = String.Join(",", AdminIds.Select((e, i) => "@" + i).ToList());

            string sql = @"
                            select * from
                            (
	                            select  
			                            u.id as UserId,
			                            m.Id as JobId,
			                            m.SettlementAmount,
			                            s.openID,
			                            row_number () OVER ( partition BY s.userID ORDER BY s.weight ) AS weight,
                                        m.TotalSchoolCommentsSelected + m.TotalAnswerSelected as SelectTotal,
			                            s.appName
			                            from 
			                            [iSchoolProduct].[dbo].[SettlementAmountMoneys] as m 
			                            LEFT JOIN
				                            [iSchoolUser].[dbo].[userInfo] as u on m.PartTimeJobAdminId = u.Id
					                            LEFT JOIN 			
									                            (select 
												                            openID,
												                            userID,
												                            appName,
												                            case
													                            when appName = 'fwh' then 1
													                            when appName = 'app' then 2
													                            when appName = 'web' then 3
												                            end as weight
											                            from [iSchoolUser].[dbo].[openid_weixin]) as s on u.id = s.userID
								                            where m.id in (" + paraStr + @")
											                            and s.weight is not null
                            ) as tab where tab.weight = 1";

            return Query<JobSettlementMoney>(sql, para.ToArray())?.ToList();
        }

        public new IEnumerable<SettlementAmountMoney> GetList(Expression<Func<SettlementAmountMoney, bool>> where = null)
        {
            return base.GetList(where);
        }

        public int Insert(SettlementAmountMoney model)
        {
            return base.Add(model);
        }

        public SettlementAmountMoney GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public bool isExists(Expression<Func<SettlementAmountMoney, bool>> where)
        {
            return base.GetList(where) == null;
        }
    }
}
