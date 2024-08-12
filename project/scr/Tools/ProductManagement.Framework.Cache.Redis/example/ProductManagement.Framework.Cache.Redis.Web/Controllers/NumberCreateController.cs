using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagement.Framework.Cache.Redis.Web.Controllers
{
    [Route("api/[controller]")]
    public class NumberCreateController : Controller
    {
        private readonly INumberCreater _numberCreater;

        public NumberCreateController(INumberCreater numberCreater)
        {
            _numberCreater = numberCreater;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> number = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                number.Add(_numberCreater.Generate("TT"));
            }
            
            return number;
        }
    }
}
