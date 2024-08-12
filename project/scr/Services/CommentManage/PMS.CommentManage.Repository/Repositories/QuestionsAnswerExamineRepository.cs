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
    public class QuestionsAnswerExamineRepository : EntityFrameworkRepository<QuestionsAnswerExamine>, IQuestionsAnswerExamineRepository
    {
        //答题详情
        private readonly IQuestionsAnswersInfoRepository answerEfRepository;
        //审核日志
        private readonly IExaminerRecordRepository recordEfRepository;


        private readonly ISettlementAmountMoneyRepository _settlementAmountMoney;
        public QuestionsAnswerExamineRepository(CommentsManageDbContext dbContext,
            ISettlementAmountMoneyRepository settlementAmountMoney,
            IQuestionsAnswersInfoRepository answerEfRepo,
            IExaminerRecordRepository recordEfRepo):base(dbContext)
        {
            answerEfRepository = answerEfRepo;
            recordEfRepository = recordEfRepo;
            _settlementAmountMoney = settlementAmountMoney;
        }

        public override int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public bool ExecExaminer(Guid PartTimeJobAdminId, Guid TargetId, int Status, bool IsPartTimeJob)
        {

            try
            {
                BeginTransaction();
                string cmd = "update QuestionsAnswersInfos set State = @Status where id = @TargetId ";

                if (IsPartTimeJob)
                {
                    cmd += " and ((select count(1) from QuestionsAnswerExamines where QuestionsAnswersInfoId = @TargetId) = 0 or (select count(1) from QuestionsAnswerExamines where AdminId = @PartTimeJobAdminId and QuestionsAnswersInfoId = @TargetId) = 1)";
                }

                SqlParameter[] para = {
                new SqlParameter("@PartTimeJobAdminId",PartTimeJobAdminId),
                new SqlParameter("@TargetId",TargetId),
                new SqlParameter("@Status",Status)
            };
                int rez = ExecuteNonQuery(cmd, para);
                if (rez > 0)
                {
                    int status = 0;
                    var entity = GetList(x => x.QuestionsAnswersInfoId == TargetId).FirstOrDefault();
                    //添加审核记录，存在审核人员修改状态
                    if (entity == null)
                    {
                        QuestionsAnswerExamine commentExamine = new QuestionsAnswerExamine();
                        commentExamine.Id = Guid.NewGuid();
                        commentExamine.AdminId = PartTimeJobAdminId;
                        commentExamine.IsPartTimeJob = IsPartTimeJob;
                        commentExamine.QuestionsAnswersInfoId = TargetId;
                        Add(commentExamine);
                    }
                    else
                    {
                        status = (int)answerEfRepository.GetModelById(TargetId).State;
                    }

                    //修改EF中的实体状态数据
                    var oldEntity = answerEfRepository.GetModelById(TargetId);
                    oldEntity.State = (ExamineStatus)Status;
                    int StatusChange =  answerEfRepository.Update(oldEntity);

                    //添加本次审核操作日志
                    ExaminerRecord examinerRecord = new ExaminerRecord();
                    examinerRecord.ChangeFirstStatus = status;
                    examinerRecord.ChangeAfterStatus = Status;
                    examinerRecord.AdminId = PartTimeJobAdminId;
                    examinerRecord.TargetId = TargetId;
                    examinerRecord.ExaminerType = ExaminerRecordType.Answer;
                    examinerRecord.Id = Guid.NewGuid();
                    int examinerRecordTotal = recordEfRepository.Insert(examinerRecord);

                    int SelectedJobstatus = -1;
                    //修改结算记录表[状态为精选值]
                    if (Status == 3 && IsPartTimeJob && entity == null)
                    {
                        Guid userId = answerEfRepository.GetModelById(TargetId).UserId;
                        SelectedJobstatus = _settlementAmountMoney.UpdateSettlementSelectedTotal(userId, 2, answerEfRepository.GetModelById(TargetId).CreateTime);
                    }

                    if (SelectedJobstatus == -1)
                    {
                        if (rez > 0 && StatusChange > 0 && examinerRecordTotal > 0)
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
                        if (rez > 0 && StatusChange > 0 && examinerRecordTotal > 0 && SelectedJobstatus > 0)
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

        public List<QuestionsAnswerExamine> GetAnswerInfoByAdminId(Guid AdminId, int PageIndex, int PageSize,List<Guid> Ids, out int Total)
        {
            Total = 0;
            if (PageIndex == 0)
            {
                var rez = base.GetList(x => x.AdminId == AdminId).OrderByDescending(x=>x.ExamineTime);
                if (rez == null)
                {
                    return null;
                }
                else
                {
                    Total = rez.Count();
                    return rez.ToList();
                }
            }
            else
            {
                //var a = GetList(x => x.AdminId == AdminId);
                if (!Ids.Any())
                {
                    Total = base.GetList(x => x.AdminId == AdminId) == null ? 0 : GetList(x => x.AdminId == AdminId).Count();
                    if (Total == 0)
                        return null;
                    else
                        return base.GetList(x => x.AdminId == AdminId).OrderByDescending(x => x.ExamineTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                else 
                {
                    Total = base.GetList(x => x.AdminId == AdminId) == null ? 0 : GetList(x => x.AdminId == AdminId && Ids.Contains(x.QuestionsAnswersInfo.UserId)).Count();
                    if (Total == 0)
                        return null;
                    else
                        return base.GetList(x => x.AdminId == AdminId && Ids.Contains(x.QuestionsAnswersInfo.UserId)).OrderByDescending(x => x.ExamineTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
            }
        }

        public new IEnumerable<QuestionsAnswerExamine> GetList(Expression<Func<QuestionsAnswerExamine, bool>> where = null)
        {
            return base.GetList(where);
        }

        public QuestionsAnswerExamine GetModelById(Guid Id)
        {
            return GetAggregateById(Id);
        }

        public int Insert(QuestionsAnswerExamine model)
        {
            return Add(model);
        }

        public bool isExists(Expression<Func<QuestionsAnswerExamine, bool>> where)
        {
            return base.GetList(where) == null;
        }

        public new int Update(QuestionsAnswerExamine model)
        {
            return Update(model);
        }
    }
}
