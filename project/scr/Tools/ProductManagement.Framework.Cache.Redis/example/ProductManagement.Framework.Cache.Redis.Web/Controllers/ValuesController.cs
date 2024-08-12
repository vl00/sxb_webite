using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProductManagement.Framework.Cache.Redis.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        private readonly ICacheRedisClient _cacheRedisClient;

        private readonly IEasyRedisClient _easyRedisClient;

        public ValuesController(ICacheRedisClient cacheRedisClient,
            IEasyRedisClient easyRedisClient)
        {
            _cacheRedisClient = cacheRedisClient;
            _easyRedisClient = easyRedisClient;
        }

        [HttpGet]
        public async Task<string> EasyRedisClientTest()
        {
            await _easyRedisClient.AddAsync("test", new TestValue { Id = "test", Name = "TestName" });
            //var testValue = await _easyRedisClient.CacheRedisClient.SearchKeysAsync("ProductManagement.UserInfo:*");

            //await _easyRedisClient.GetOrAddAsync("test1", () =>
            //{
            //    return new TestValue { Id = "test1", Name = "TestName" };
            //});

            //var testValue1 = await _easyRedisClient.GetAsync<TestValue>("test1");

            //await _easyRedisClient.GetOrAddAsync<TestValue>("test2", () =>
            //{
            //    return null;
            //});

            //var testValue2 = await _easyRedisClient.GetAsync<TestValue>("test2");


            //var testValue3 = await _easyRedisClient.GetOrAddAsync<TestValue>("test1", () =>
            //{
            //    return null;
            //});

            //return $"{testValue}_{testValue1}";

            return "abc";
        }
        [HttpGet]
        public async Task<string> EasyRedisClientTest2()
        {
            await _easyRedisClient.AddAsync("test2", new TestValue { Id = "test", Name = "TestName" });
            //var testValue = await _easyRedisClient.CacheRedisClient.SearchKeysAsync("ProductManagement.UserInfo:*");

            //await _easyRedisClient.GetOrAddAsync("test1", () =>
            //{
            //    return new TestValue { Id = "test1", Name = "TestName" };
            //});

            //var testValue1 = await _easyRedisClient.GetAsync<TestValue>("test1");

            //await _easyRedisClient.GetOrAddAsync<TestValue>("test2", () =>
            //{
            //    return null;
            //});

            //var testValue2 = await _easyRedisClient.GetAsync<TestValue>("test2");


            //var testValue3 = await _easyRedisClient.GetOrAddAsync<TestValue>("test1", () =>
            //{
            //    return null;
            //});

            //return $"{testValue}_{testValue1}";

            return "abc";
        }

        //[HttpGet("{key}")]
        //public string Get(string key)
        //{
        //    var testValue = _cacheRedisClient.GetAsync<TestValue>(key+1);
        //    var s = _cacheRedisClient.GetAsync<string>(key);
        //    return $"{s}_{testValue}";
        //}

        //[HttpPost]
        //public void Post([FromBody]string key)
        //{
        //    _cacheRedisClient.AddAsync(key+1, new TestValue{Id = key + 1, Name = "TestName"});
        //    _cacheRedisClient.AddAsync(key, "test value");
        //}

        //[HttpPut("{key}")]
        //public void Put(string key, [FromBody]string value)
        //{
        //    _cacheRedisClient.ReplaceAsync(key + 1, new TestValue { Id = key + 2, Name = value });
        //    _cacheRedisClient.ReplaceAsync(key, value);
        //}

        //[HttpDelete("{key}")]
        //public void Delete(string key)
        //{
        //    _cacheRedisClient.RemoveAsync(key + 1);
        //    _cacheRedisClient.RemoveAsync(key);
        //}
    }

    public class TestValue
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Id}-{Name}";
        }
    }
}
