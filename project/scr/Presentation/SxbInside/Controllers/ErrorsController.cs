using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Sxb.Inside.Controllers
{
    public class ErrorsController : Controller
    {
        private IWebHostEnvironment _env;

        public ErrorsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("errors/{statusCode}")]
        [Description("错误页")]
        public IActionResult CustomError(int statusCode)
        {
            var filePath = $"{_env.WebRootPath}/errors/{(statusCode == 406 ? "schoolerror" : "404")}.html";
            return new PhysicalFileResult(filePath, new MediaTypeHeaderValue("text/html"));
        }

        public IActionResult Test()
        {
            throw new Exception();
        }
    }
}
