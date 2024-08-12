using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Application.Services.Settlement;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Web.Areas.PartTimeJob.Models.ViewEntity;

namespace ProductManagement.Web.Areas.PartTimeJob.Controllers
{
    /// <summary>
    /// 结算
    /// </summary>

    [Area("PartTimeJob")]
    public class SettlementController : BaseController
    {
        ISettlementAmountMoneyService _amountMoneyService;
        ISettlementViewService _settlementViewService;
        IPrtTimeJobMQService _jobMQService;
        IPartTimeJobAdminService _partTimeJobAdmin;

        public SettlementController(ISettlementAmountMoneyService amountMoneyService, ISettlementViewService settlementViewService, IPrtTimeJobMQService jobMQService, IPartTimeJobAdminService partTimeJobAdmin)
        {
            _amountMoneyService = amountMoneyService;
            _settlementViewService = settlementViewService;
            _jobMQService = jobMQService;
            _partTimeJobAdmin = partTimeJobAdmin;
        }


        public void test()
        {
            //_jobMQService.SendPartTimeJobMoney(new List<PMS.CommentsManage.Application.Services.Settlement.PartTimeJobDto>);


            _amountMoneyService.Settlement();
            //_jobMQService.SendPartTimeJobMoney(new PartTimeJobDto()
            //{
            //    SettlementAmount = 30,
            //    ActivityName = "兼职结算",
            //    OpenId = "oEo0iuNzUHkCKc9UvcH-XE10p6F4",
            //    UserId = Guid.Parse("8AAB9049-6397-48A6-A76A-BA697008E3F4"),
            //    JobId = Guid.Parse("7830F350-417F-415F-8C61-22273BCCC036"),
            //    Type = 1,
            //    Remark = "您有5条内容奖励红包已发出",
            //    AppName = "fwh"
            //});

            //parseInt(100 * (val * 1000) / 1000);
        }

        /// <summary>
        /// 供应商、兼职领队 结算页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (int.Parse(_admin.Role) != 5)
            {
                HttpContext.Response.Redirect("/PartTimeJob/Home/Welcome");
            }
            var rez = _amountMoneyService.GetSettlementStatuses();

            if (rez.Any()) 
            {
                var settlement = _settlementViewService.GetSettlementViewByIds(rez.Select(x => x.Id)?.ToList());
                List<Guid> AdminIds = rez.Select(p => p.PartTimeJobAdminId).ToList();
                var admins = _partTimeJobAdmin.GetList(x => AdminIds.Contains(x.Id));
                foreach (var item in rez)
                {
                    if (item.Status)
                    {
                        var model = settlement.Where(x => x.Id == item.Id).FirstOrDefault();
                        model.SettlementStatus = PMS.CommentsManage.Domain.Common.SettlementStatus.Settled;

                        int UpdatTotle = _amountMoneyService.Update(model);

                        var admin = admins.Where(x => x.Id == item.PartTimeJobAdminId).FirstOrDefault();
                        _amountMoneyService.NextSettlementData(admin, model.PartJobRole);
                    }
                    else
                    {
                        var model = settlement.Where(x => x.Id == item.Id).FirstOrDefault();
                        model.SettlementStatus = PMS.CommentsManage.Domain.Common.SettlementStatus.Settlement;

                        _amountMoneyService.Update(model);
                    }
                }
            }

            return View();
        }

        /// <summary>
        /// 修改结算方式
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateSettlementType()
        {
            return View();
        }

        public PageResult<List<SettlementViewVo>> GetSettlement(Guid ParentId,int Status,int page, int limit,int TotalSearch, DateTime date)
        {
            if (date == default(DateTime))
            {
                DateTime time = DateTime.Now;
                date = new DateTime(time.Year, time.Month, 1, 00, 00, 00);
            }
            else
            {
                date = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            }

            int total = 0;
            var rez = _settlementViewService.GetSettlementViews(ParentId, Status, page, limit, TotalSearch, date, out total);
            var data = Mapper.Map<List<SettlementView>,List<SettlementViewVo>>(rez);
            if (int.Parse(_admin.Role) != 5)
            {
                data.ForEach(x => { 
                    x.Phone = Regex.Replace(x.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                    DateTime endTime = DateTime.Parse(x.EndTime);
                    x.IsSettlement = true;
                    //检测是否可以进行结算满足条件（是否精选点评+精选回答 >= 5 并且 当前时间为该任务的结算日期）
                    
                });
            }

            for (int i = 0; i < data.Count(); i++)
            {
                //if ((x.TotalAnswerSelected+x.TotalSchoolCommentsSelected >= 5) && (endTime.Year == DateTime.Now.Year && endTime.Month == DateTime.Now.Month && endTime.Day == DateTime.Now.Day)) 
                //{
                //    x.IsSettlement = true;
                //}
                //else 
                //{
                //    x.IsSettlement = false;
                //}
                data[i].IsSettlement = true;
            }
            return new PageResult<List<SettlementViewVo>>() {
                rows = data,
                total = total
            };
        }

        public ModelResult<string> UpdateSettlemetStatus(string Ids,int status)
        {
            var SettlementIds = Ids.Split(",").ToList().Select(x => Guid.Parse(x)).ToList();
            var settlement = _settlementViewService.GetSettlementViewByIds(SettlementIds);

            var UpdateData = settlement.Where(x => (x.TotalAnswerSelected + x.TotalSchoolCommentsSelected) >= 5).ToList();
            var DataIds = UpdateData.Select(x => x.Id).ToList();

            int rez = _settlementViewService.UpdateStatus(_admin.Id, String.Join(',',DataIds), status);
            if (rez == 1 && status == 2) 
            {
                //var settlem = _settlementViewService.GetSettlementViewByIds(DataIds);
                var PushItems = _amountMoneyService.settlementMoney(DataIds);
                foreach (var item in PushItems)
                {
                    int Total = item.SelectTotal >= 10 ? 10 : item.SelectTotal >= 5 ? 5 : 0;
                    _jobMQService.SendPartTimeJobMoney(new PartTimeJobDto()
                    {
                        ActivityName = "兼职管理平台结算",
                        SettlementAmount = Convert.ToInt32(100 * (item.SettlementAmount * 1000) / 1000),
                        //SettlementAmount = 30,
                        AppName = item.appName,
                        JobId = item.JobId,
                        UserId = item.UserId,
                        OpenId = item.openID,
                        Type = 1,
                        Sender = _admin.Id.ToString(),
                        Remark = $"您有{Total}条内容奖励红包已发出"
                    });
                }
            }

            int i = settlement.Where(x => (x.TotalAnswerSelected + x.SettlementAmount) < 5).Count();
            string message = "";
            if (i > 0) 
            {
                message += "结算中存在未到达要求的数据";
            }

            int code = 200;
            if (rez == 0)
            {
                code = 500;

            }
            return new ModelResult<string>() {
                StatusCode = code,
                Message = message
            };
        }


    }
}