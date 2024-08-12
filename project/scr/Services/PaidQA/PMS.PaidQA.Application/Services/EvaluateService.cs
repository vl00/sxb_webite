using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.PaidQA.Domain.EntityExtend;
using MediatR;
using PMS.MediatR.Events.PaidQA;

namespace PMS.PaidQA.Application.Services
{
    public class EvaluateService : ApplicationService<Evaluate>, IEvaluateService
    {
        IMediator _mediator;
        IEvaluateRepository _evaluateRepository;
        IOrderRepository _orderRepository;
        IEvaluateTagRelationRepository _evaluateTagRelationRepository;
        IEvaluateTagsRepository _evaluateTagsRepository;
        public EvaluateService(IEvaluateRepository evaluateRepository, IOrderRepository orderRepository, IEvaluateTagRelationRepository evaluateTagRelationRepository,
            IEvaluateTagsRepository evaluateTagsRepository, IMediator mediator) : base(evaluateRepository)
        {
            _evaluateTagsRepository = evaluateTagsRepository;
            _evaluateRepository = evaluateRepository;
            _orderRepository = orderRepository;
            _evaluateTagRelationRepository = evaluateTagRelationRepository;
            _mediator = mediator;
        }

        public async Task<bool> UserCreateEvaluate(Order order, Evaluate evaluate)
        {
            bool result = false;
            if (order.Status == Domain.Enums.OrderStatus.Finish && !order.IsEvaluate)
            {
                //必须是已结束的订单并且还没评价过的订单才能评价

                using (var tran = _evaluateRepository.BeginTransaction())
                {
                    try
                    {
                        //创建评价同时需要修改订单状态
                        evaluate.ID = Guid.NewGuid();
                        evaluate.OrderID = order.ID;
                        evaluate.CreateTime = DateTime.Now;
                        evaluate.IsAuto = false;
                        evaluate.IsValid = true;
                        bool addEvaluateResult = await _evaluateRepository.AddAsync(evaluate, tran);
                        //修改订单状态
                        order.IsEvaluate = true;
                        order.UpdateTime = DateTime.Now;
                        bool updateOrderResult = await _orderRepository.UpdateAsync(order
                            , order.CreatorID
                            , tran
                            , new[] { "IsEvaluate", "UpdateTime" }
                            ,"用户进行评价。");
                        if (evaluate.EvaluateTagRelations != null && evaluate.EvaluateTagRelations.Any())
                        {
                            //构建订单标签关系
                            evaluate.EvaluateTagRelations.ForEach(etr =>
                            {
                                etr.ID = Guid.NewGuid();
                                etr.TagID = etr.TagID;
                                etr.EvaluateID = evaluate.ID;
                            });
                            bool addTagRelations = await _evaluateTagRelationRepository.AddsAsync(evaluate.EvaluateTagRelations, tran);
                            if (!addTagRelations)
                            {
                                tran.Rollback();
                                result = false;
                            }
                        }
                        tran.Commit();
                        if (addEvaluateResult && updateOrderResult)
                        {
                            result = true;
                        }
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        result = false;
                    }
                }
            }
            if (result)
            {
                //触发发表评价事件
                await _mediator.Publish(new OrderEvaluteEvent(order));
            }

            return result;
        }

        public async Task<IEnumerable<Evaluate>> GetByOrderIDs(IEnumerable<Guid> IDs)
        {
            if (IDs?.Any() == true)
            {
                var str_Where = $"OrderID in @IDs and IsValid != 0";
                var finds = await Task.Run(() =>
                {
                    return _evaluateRepository.GetBy(str_Where, new { IDs },"IsAuto", fileds: new string[] { "*" });
                });
                if (finds?.Any() == true)
                {
                    return finds;
                }
            }
            return new Evaluate[0];
        }

        public async Task<double> GetTalentAvgScope(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return default;
            return await _evaluateRepository.GetAvgScoreByTalentUserID(talentUserID);
        }

        public async Task<IEnumerable<EvaluateTagCountingExtend>> GetEvaluateTagCountingByTalentUserID(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return null;
            return await _evaluateTagsRepository.GetEvaluateTagCountingByTalentUserID(talentUserID);

        }

        public async Task<(IEnumerable<Evaluate>, int)> PageByTalentUserID(Guid talentUserID, int pageIndex = 1, int pageSize = 10, Guid? tagID = null)
        {
            return await _evaluateRepository.PageByAnwserUserID(talentUserID, pageIndex, pageSize, tagID);
        }

        public  IEnumerable<EvaluateTags> GetEvaluateTags()
        {
          return   _evaluateTagsRepository.GetBy(null);
        }

        /// <summary>
        /// 评价超时，系统自动好评
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AutoNiceEvaluation()
        {
            var where = "Status = 4 AND IsEvaluate = 0 AND FinishTime < DATEADD(HOUR,-24,GETDATE())";
            //查询需要自动好评的订单
            var data = _orderRepository.GetBy(where, new { });
            if (null == data || !data.Any()) { return false; }
            //每个订单系统自动好评都单独完成，互不影响
            foreach (var item in data)
            {
                using (var tran = _orderRepository.BeginTransaction())
                {
                    try
                    {
                        var sql = "UPDATE [Order] SET IsEvaluate = 1,UpdateTime = GETDATE() WHERE ID = @ID AND IsEvaluate = 0";
                        var param = new { ID = item.ID };
                        //自动好评，修改订单数据
                        var niceEva = await _orderRepository.ExecuteAsync(sql, param, tran) > 0;
                        //新增评论数据
                        var evaluate = new Evaluate()
                        {
                            ID = Guid.NewGuid(),
                            Content = "评价超时，系统自动好评",
                            OrderID = item.ID,
                            Score = 10,
                            CreateTime = DateTime.Now,
                            IsAuto = true,
                            IsValid = true
                        };
                        var evas = await _evaluateRepository.AddAsync(evaluate, tran);
                        if (niceEva && evas)
                        {
                            tran.Commit();
                            await _mediator.Publish(new OrderEvaluteEvent(item));
                        }
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                    }
                }
            }
            return true;
        }
    }
}
