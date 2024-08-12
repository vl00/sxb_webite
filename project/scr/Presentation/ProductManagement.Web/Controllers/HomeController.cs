using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Application.Services.Settlement;

namespace ProductManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        IPrtTimeJobMQService _jobMQService;

        public HomeController(IPrtTimeJobMQService jobMQService) 
        {
            _jobMQService = jobMQService;
        }

        public IActionResult Index()
        {
            //_jobMQService.SendPartTimeJobMoney(new List<PMS.CommentsManage.Application.Services.Settlement.PartTimeJobDto>);

            //List<PartTimeJobDto> jobs = new List<PartTimeJobDto>()
            //{
            //    new PartTimeJobDto() 
            //    { 
            //        SettlementAmount = 0.01M,
            //        ActivityName = "兼职结算",
            //        OpenId = "oVgTu0FnReiBTheLuoxjquT6LBz4",
            //        UserId = Guid.Parse("8AAB9049-6397-48A6-A76A-BA697008E3F4"),
            //        JobId = Guid.Parse("7830F350-417F-415F-8C61-22273BCCC036")
            //    }
            //};

            //_jobMQService.SendPartTimeJobMoney(jobs);
            //parseInt(100 * (val * 1000) / 1000);


            return View();
        }
    }

}
