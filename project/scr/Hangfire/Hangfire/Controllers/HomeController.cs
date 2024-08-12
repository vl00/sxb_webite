using System;
using Hangfire.ConsoleWeb.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hangfire.ConsoleWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        public IActionResult Index()
        {
            return Ok("ok");
        }

        public IActionResult Check()
        {
            _logger.LogInformation("start Check");
            BackgroundJob.Enqueue<ICheckJob>(c => c.Check(null));
            return Ok("ok");
        }
    }
}
