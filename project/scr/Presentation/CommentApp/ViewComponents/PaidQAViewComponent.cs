using Microsoft.AspNetCore.Mvc;
using PMS.PaidQA.Application.Services;
using Sxb.Web.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.Web.ViewComponents
{
    public class PaidQAViewComponent : ViewComponent
    {
        IOrderService _orderService;
        IHotTypeService _hotTypeService;
        ITalentSettingService _talentSettingService;
        IMessageService _messageService;

        public PaidQAViewComponent(IHotTypeService hotTypeService, IOrderService orderService, ITalentSettingService talentSettingService,
            IMessageService messageService)
        {
            _messageService = messageService;
            _talentSettingService = talentSettingService;
            _hotTypeService = hotTypeService;
            _orderService = orderService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade">1:幼儿园 2:小学 3:初中 4:高中</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(byte grade,Guid schoolExtId)
        {
            var result = new List<PaidQAViewModel>();
            var schoolPaidTalents = _talentSettingService.GetDetailsBySchool(schoolExtId).GetAwaiter().GetResult();

            ViewBag.HasPaidTalent = schoolPaidTalents?.Any() == true;
            if (ViewBag.HasPaidTalent)
            {
                
                foreach (var talentSetting in schoolPaidTalents)
                {
                    var item = new PaidQAViewModel()
                    {
                        NickName = talentSetting.NickName,
                        HeadImgUrl = talentSetting.HeadImgUrl,
                        RegionNames = talentSetting.TalentRegions?.Select(p => p.Name),
                        TalentUserID = talentSetting.TalentUserID,
                        AuthName = talentSetting.AuthName,
                        TalentType = talentSetting.TalentType,
                    };
                    result.Add(item);
                }
            }
            else {
                var gradeType = 0;
                var random = new Random();
                switch (grade)
                {
                    case 1:
                        gradeType = 1;
                        break;
                    case 2:
                        gradeType = random.Next(2, 4);
                        break;
                    case 3:
                        gradeType = 4;
                        break;
                    case 4:
                        gradeType = 5;
                        break;
                    default:
                        break;
                }
                if (gradeType < 1) return View(result);
                var ids = _hotTypeService.GetRandomOrderIDByGrade(gradeType);
                if (ids.Result?.Any() == true)
                {
                    var orders = _orderService.ListByIDs(ids.Result);
                    if (orders.Result?.Any() == true)
                    {
                        var talentSettings = _talentSettingService.GetDetails(orders.Result.Select(p => p.AnswerID).Distinct());
                        var contents = _messageService.GetOrdersQuetion(orders.Result.Select(p => p.ID).Distinct());
                        foreach (var order in orders.Result)
                        {
                            var item = new PaidQAViewModel();
                            var talentSetting = talentSettings.Result.FirstOrDefault(p => p.TalentUserID == order.AnswerID);
                            if (talentSetting != null)
                            {
                                item.NickName = talentSetting.NickName;
                                item.HeadImgUrl = talentSetting.HeadImgUrl;
                                item.RegionNames = talentSetting.TalentRegions?.Select(p => p.Name);
                                item.TalentUserID = talentSetting.TalentUserID;
                                item.AuthName = talentSetting.AuthName;
                                item.TalentType = talentSetting.TalentType;
                            }
                            var content = contents.Result.FirstOrDefault(p => p.OrderID == order.ID);
                            if (content != null)
                            {

                                if (!string.IsNullOrWhiteSpace(content.Content))
                                {
                                    try
                                    {
                                        var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<PMS.PaidQA.Domain.Message.TXTMessage>(content.Content);
                                        if (messageContent != null) item.Content = messageContent.Content;
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                            result.Add(item);
                        }
                    }
                }

            }
            return View(result);

        }
    }
}
