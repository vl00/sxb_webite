using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PMS.PaidQA.Application.Services;
using Sxb.PCWeb.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.PCWeb.ViewComponents
{
    public class PaidQAViewComponent : ViewComponent
    {
        IOrderService _orderService;
        IHotTypeService _hotTypeService;
        ITalentSettingService _talentSettingService;
        IMessageService _messageService;
        IConfiguration _configuration;

        public PaidQAViewComponent(IHotTypeService hotTypeService, IOrderService orderService, ITalentSettingService talentSettingService,
            IMessageService messageService,IConfiguration configuration)
        {
            _messageService = messageService;
            _talentSettingService = talentSettingService;
            _hotTypeService = hotTypeService;
            _orderService = orderService;
            _configuration = configuration;
            if (_configuration != null)
            {
                var baseUrl = _configuration.GetSection("ExternalInterface").GetValue<string>("OrganizationAddress");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    //_apiUrl = $"{baseUrl}api/ToSchools/hotsell/coursesandorgs?minage={{0}}&maxage={{1}}";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade">1:幼儿园 2:小学 3:初中 4:高中</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(byte grade)
        {
            var gradeType = 0;
            var random = new Random();
            var result = new List<PaidQAViewModel>();
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
                    var userInfos = _talentSettingService.GetDetails(orders.Result.Select(p => p.AnswerID).Distinct());
                    var contents = _messageService.GetOrdersQuetion(orders.Result.Select(p => p.ID).Distinct());
                    foreach (var order in orders.Result)
                    {
                        var item = new PaidQAViewModel();
                        var userinfo = userInfos.Result.FirstOrDefault(p => p.TalentUserID == order.AnswerID);
                        if (userinfo != null)
                        {
                            item.NickName = userinfo.NickName;
                            item.HeadImgUrl = userinfo.HeadImgUrl;
                            item.RegionNames = userinfo.TalentRegions?.Select(p => p.Name);
                            item.TalentUserID = userinfo.TalentUserID;
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
            return View(result);
        }

    }
}
