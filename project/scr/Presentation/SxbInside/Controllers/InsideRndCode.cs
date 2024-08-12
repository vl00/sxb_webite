using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Inside.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    [Route("api/[controller]")]
    public class InsideRndCode : Controller
    {
        private readonly IEasyRedisClient _easyRedisClient;

        public InsideRndCode(IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
        }

        [HttpGet]
        public ResponseResult Get()
        {
            var rndCode = _easyRedisClient.GetStringAsync("login:RndCode-Inside");
            return ResponseResult.Success(new { rndCode });
        }

        // POST api/values
        [HttpPost]
        public ResponseResult Post()
        {
            Random ran = new Random();
            int RandKey = ran.Next(100000, 999999);

            string rndCode = RandKey.ToString();
            _easyRedisClient.AddAsync("login:RndCode-Inside", rndCode);
            return ResponseResult.Success(new { rndCode });
        }
    }
}
