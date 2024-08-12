using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using Sxb.Inside.Response;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class StatisticsController : Controller
    {

        private readonly IArticleService _articleService;
        private readonly IArticle_ShareService _shareService;

        public StatisticsController(
            IArticleService articleService, IArticle_ShareService shareService)
        {
            _articleService = articleService;
            _shareService = shareService;
        }


        public ResponseResult ViewStatistics()
        {
            _shareService.StatisticArticleShare();


            return ResponseResult.Success();
        }

        public ResponseResult Test()
        {

            _shareService.Test();

            return ResponseResult.Success();
        }
    }
}
