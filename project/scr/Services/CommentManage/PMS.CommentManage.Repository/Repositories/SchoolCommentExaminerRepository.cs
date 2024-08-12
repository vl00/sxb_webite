using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
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

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SchoolCommentExaminerRepository : EntityFrameworkRepository<SchoolCommentExamine>, ISchoolCommentExamineRepository
    {
        //点评操作日志
        private readonly IExaminerRecordRepository recordEfRepository;
        //管理员
        private readonly IPartTimeJobAdminRepository adminEfRepository;

        private readonly ISettlementAmountMoneyRepository _settlementAmountMoney;


        private readonly ISchoolCommentRepository _schoolCommentRepository;

        public SchoolCommentExaminerRepository(CommentsManageDbContext dbContext, 
            ISettlementAmountMoneyRepository settlementAmountMoney,
            ISchoolCommentRepository schoolCommentRepository,
            IExaminerRecordRepository recordEfRepo,
            IPartTimeJobAdminRepository adminEfRepo
            ) :base(dbContext)
        {
            recordEfRepository = recordEfRepo;
            adminEfRepository = adminEfRepo;
            _settlementAmountMoney = settlementAmountMoney;

            _schoolCommentRepository = schoolCommentRepository;
        }

        public override int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public bool ExecExaminer(Guid AdminId, Guid TargetId, int Status,bool IsPartTimeJob)
        {
            try
            {
                string cmd = "update SchoolComments set State = @Status where id = @TargetId ";

                if (IsPartTimeJob)
                {
                    cmd += " and ((select count(1) from SchoolCommentExamines where SchoolCommentId = @TargetId) = 0 or (select count(1) from SchoolCommentExamines where AdminId = @PartTimeJobAdminId and SchoolCommentId =@TargetId) = 1)";
                }

                SqlParameter[] para = {
                new SqlParameter("@PartTimeJobAdminId",AdminId),
                new SqlParameter("@TargetId",TargetId),
                new SqlParameter("@Status",Status)
            };

                BeginTransaction();

                int rez = ExecuteNonQuery(cmd, para);
                if (rez > 0)
                {
                    int status = 0;
                    var entity = GetList(x => x.SchoolCommentId == TargetId).FirstOrDefault();
                    //添加审核记录，存在审核人员修改状态
                    if (entity == null)
                    {
                        SchoolCommentExamine commentExamine = new SchoolCommentExamine();
                        commentExamine.Id = Guid.NewGuid();
                        commentExamine.AdminId = AdminId;
                        commentExamine.IsPartTimeJob = IsPartTimeJob;
                        commentExamine.SchoolCommentId = TargetId;
                        //commentExamine.AddTime = DateTime.Now;
                        //commentExamine.UpdateTime = DateTime.Now;
                        Add(commentExamine);
                    }
                    else
                    {
                        status = (int)_schoolCommentRepository.GetModelById(TargetId).State;
                    }

                    //更新EF中存储的实体数据
                    var oldEntity = _schoolCommentRepository.GetModelById(TargetId);

                    int changecommentstatus = ExecuteNonQuery("update SchoolComments set State = @state where Id = @commentId",new SqlParameter[] { new SqlParameter("@state", Status),new SqlParameter("@commentId", oldEntity.Id) });

                    //添加本次审核操作日志
                    ExaminerRecord examinerRecord = new ExaminerRecord();
                    examinerRecord.ChangeFirstStatus = status;
                    examinerRecord.ChangeAfterStatus = Status;
                    examinerRecord.AdminId = AdminId;
                    examinerRecord.IsPartTimeJob = IsPartTimeJob;
                    examinerRecord.TargetId = TargetId;
                    examinerRecord.ExaminerType = ExaminerRecordType.Comment;
                    examinerRecord.Id = Guid.NewGuid();

                    int Addrecord = recordEfRepository.Insert(examinerRecord);

                    int isSelectedJob = -1;
                    //修改结算记录表[状态为精选值]，且并无审核记录
                    if (Status == 3 && IsPartTimeJob && entity == null)
                    {

                        Guid CommentUserId = _schoolCommentRepository.GetModelById(TargetId).CommentUserId;
                        isSelectedJob = _settlementAmountMoney.UpdateSettlementSelectedTotal(CommentUserId, 1, oldEntity.AddTime);
                    }

                    //检测是否为精选审核
                    if (isSelectedJob == -1)
                    {
                        //非精选审核， rez>0 : 修改状态成功，changecommentstatus > 0  ef实体更新,Addrecord > 0 审核日志添加成功
                        if (rez > 0 && changecommentstatus > 0 && Addrecord > 0)
                        {
                            TranCommit();
                            return true;
                        }
                        else
                        {
                            Rollback();
                            return false;
                        }
                    }
                    else
                    {
                        //精选，在上面非精选基础上增加 是否结算周期数值更新成功
                        if (rez > 0 && changecommentstatus > 0 && Addrecord > 0 && isSelectedJob > 0)
                        {
                            TranCommit();
                            return true;
                        }
                        else
                        {
                            Rollback();
                            return false;
                        }
                    }
                }
                else
                {
                    Rollback();
                    return false;
                }
            }
            catch (Exception)
            {
                Rollback();
                return false;
            }
        }

        public new IEnumerable<SchoolCommentExamine> GetList(Expression<Func<SchoolCommentExamine, bool>> where = null)
        {
            return base.GetList(where);
        }

        public SchoolCommentExamine GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public  int Insert(SchoolCommentExamine model)
        {
            return base.Add(model);
        }

        public  bool isExists(Expression<Func<SchoolCommentExamine, bool>> where)
        {
            return base.GetList(where) == null;
        }

        public new int Update(SchoolCommentExamine model)
        {
            return base.Update(model);
        }

        public List<SchoolCommentExamine> GetSchoolCommentByAdminId(Guid AdminId, int PageIndex, int PageSize,List<Guid> Ids, out int Total, bool IsPartTimeJob )
        {
            Total = 0;
            //直接全量导出该管理员的审核数据
            if (PageIndex == 0)
            {
                var rez = base.GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob).OrderByDescending(x=>x.AddTime);
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

                if (!Ids.Any())
                {
                    Total = base.GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob) == null ? 0 : GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob).Count();
                    if (Total == 0)
                        return null;
                    else
                        return base.GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob).OrderByDescending(x => x.AddTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                else
                {
                    Total = base.GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob) == null ? 0 : GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob && Ids.Contains(x.SchoolComment.CommentUserId)).Count();
                    if (Total == 0)
                        return null;
                    else
                        return base.GetList(x => x.AdminId == AdminId && x.IsPartTimeJob == IsPartTimeJob && Ids.Contains(x.SchoolComment.CommentUserId)).OrderByDescending(x => x.AddTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
            }
        }

    }
}
