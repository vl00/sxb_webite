using MongoDB.Bson;
using MongoDB.Driver;
using ProductManagement.Framework.MongoDb;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.OperationPlateform.Domain.IMongo;
using System.Linq;
using PMS.OperationPlateform.Domain.MongoModel;
using Newtonsoft.Json;
using PMS.OperationPlateform.Domain.MongoModel.Base;
using System.Text.RegularExpressions;

namespace PMS.OperationPlateform.Mongo.Repository
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly IMongoService _mongo;
        public readonly IMongoDatabase _database;
        public readonly IMongoCollection<Statistics> _collection;

        public StatisticRepository(IMongoService<IStatisticsMongoProvider> mongo)
        {
            _mongo = mongo;
            _database = _mongo.GetDatabase("ischoollog");
            _collection = _database.GetCollection<Statistics>("statistics");
        }

        /// <summary>
        /// 获取指定时间之后访问过的url
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="starTime"></param>
        /// <returns></returns>
        public List<string> GetHitUrls(Guid userId, string[] urls, DateTime starTime)
        {
            if (urls == null)
            {
                return new List<string>();
            }

            var query = _collection.AsQueryable()
                .Where(s => s.userid == userId.ToString().ToLower()
                     && urls.Contains(s.url) && s.time >= starTime)
                //.Distinct(new Statistics.StatisticsComparer())
                .GroupBy(s => s.url)
                .Select(s => s.Key)
                //.Distinct()
                ;

            return query.ToList();
        }

        public List<DaysStatistics> GetUrlViewCount(string matchUrl, DateTime starTime, DateTime endTime)
        {
            if (matchUrl == null)
            {
                return new List<DaysStatistics>();
            }

            var query = _collection.AsQueryable()
                .Where(s => s.time >= starTime.AddHours(8) && s.time < endTime.AddHours(8))
                .Where(s => s.@event == "onshow")
                .Where(s => s.url.StartsWith(matchUrl))
                //.Where(s => new Regex($"^{matchUrl}").IsMatch(s.url))
                .GroupBy(s => new { s.userid, s.url })
                .Select(s => new
                {
                    url = s.Key.url,
                    pv = s.Count()
                })
                .GroupBy(s => s.url)
                .Select(s => new DaysStatistics()
                {
                    date = starTime.Date,
                    url = s.Key,
                    uv = s.Count(),
                    pv = s.Sum(p => p.pv)
                })
                ;
            return query.ToList();
        }

        /// <summary>
        ///  获取链接浏览数（PV、UV）
        /// </summary>
        public UrlViewCount GetUrlViewCount(string url, DateTime starTime)
        {
            List<string> pipelineJson = new List<string>
            {
                "{ $project: { 'url': 1 , 'userid': { '$cond' : [ { '$eq' : [ '$userid' , ''] } , '$deviceid' , '$userid']}, 'event': 1 ,'fw': 1 , 'params': 1}}"
            };

            List<string> querys = new List<string>();

            querys.Add("{ 'url' : { $in: ['" + url + "']} }");
            //querys.Add("{$and: [{ 'time': { $gte: new Date('" + starTime.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "') }  }] }");

            pipelineJson.Add("{ $match: { $and:  [" + string.Join(",", querys) + "]}}");
            pipelineJson.Add("{ $group: { '_id': { 'url':'$url','userid':'$userid','fw':'$fw'},  'count': { '$sum': 1}} }");
            pipelineJson.Add("{ $group: { '_id': { 'url':'$_id.url','fw':'$_id.fw'}, 'pv': { '$sum':'$count'}, 'uv':{ '$sum' : 1}}}");
            pipelineJson.Add("{ $project: { '_id':0, 'url':'$_id.url','fw':'$_id.fw','params':'$_id.params', 'pv':1, 'uv':1 }}");

            Console.WriteLine(string.Join(",", pipelineJson));

            //PS:ISODate的时间是UTC时间

            var resultBson = GetAggregate(pipelineJson);
            var result = new List<UrlViewCount>();
            foreach (var item in resultBson)
            {
                result.Add(new UrlViewCount
                {
                    Url = item["url"].AsString,
                    //Date = item["date"].AsString,
                    Fw = item["fw"].AsString,
                    Pv = item["pv"].AsInt32,
                    Uv = item["uv"].AsInt32,
                    //Params = JsonConvert.DeserializeObject<Dictionary<string, string>>(item["params"].ToJson())
                });
            }
            return result.FirstOrDefault();
        }

        /// <summary>
        ///  获取链接浏览数（PV、UV）
        /// </summary>
        public List<UrlViewCount> GetUrlViewCount(List<string> urls, List<string> fw, List<DateTime> dates)
        {
            List<string> pipelineJson = new List<string>
            {
                "{ $project: { time: { \"$add\":[\"$time\", 28800000] },day: {$substr: [{ \"$add\":[\"$time\", 28800000] }, 0, 10] }, \"url\": 1 , \"userid\": { \"$cond\" : [ { \"$eq\" : [ \"$userid\" , \"\"] } , '$deviceid' , \"$userid\"]}, \"event\": 1 ,\"fw\": 1 , \"params\": 1}}"
            };

            List<string> querys = new List<string>();
            if (urls.Any())
            {
                List<string> urlQuerys = new List<string>();
                foreach (var item in urls)
                {
                    urlQuerys.Add("{ \"url\" :'" + item.ToLower() + "'}");
                }
                string urlstr = string.Join(",", urlQuerys);
                querys.Add("{ $or:[ " + urlstr + " ]}");
            }
            if (fw.Any())
            {
                List<string> fwQuerys = new List<string>();
                foreach (var item in fw)
                {
                    fwQuerys.Add("{ \"fw\" :'" + item + "'}");
                }
                string fwstr = string.Join(",", fwQuerys);
                querys.Add("{ $or:[ " + fwstr + " ]}");
            }
            if (dates.Any())
            {
                List<string> dateQuerys = new List<string>();
                foreach (var item in dates)
                {
                    dateQuerys.Add("{$and: [{ \"time\": { $gte: new Date('" + item.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "') } }, { \"time\": { $lt: new Date('" + item.AddHours(8).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "') } }]}");
                }
                string datestr = string.Join(",", dateQuerys);
                querys.Add("{ $or:[ " + datestr + " ]}");
            }
            pipelineJson.Add("{ $match: { $and:  [  { 'event': null } ," + string.Join(",", querys) + "]}}");
            pipelineJson.Add("{ $group: {  '_id': { 'date':'$day','url':'$url','userid':'$userid','fw':'$fw','params':'$params'},  'count': { '$sum': 1}} }");
            pipelineJson.Add("{ $group: { '_id': { 'date':\"$_id.date\",'url':'$_id.url','fw':'$_id.fw','params':'$_id.params'}, 'pv': { '$sum':\"$count\"}, 'uv':{ \"$sum\" : 1}}}");
            pipelineJson.Add("{ $project: { \"_id\":0, \"url\":\"$_id.url\",'fw':'$_id.fw','params':'$_id.params', \"date\":\"$_id.date\", \"pv\":1, \"uv\":1 }}");

            Console.WriteLine(string.Join(",", pipelineJson));

            //PS:ISODate的时间是UTC时间

            var resultBson = GetAggregate(pipelineJson);
            var result = new List<UrlViewCount>();
            foreach (var item in resultBson)
            {
                result.Add(new UrlViewCount
                {
                    Url = item["url"].AsString,
                    Date = item["date"].AsString,
                    Fw = item["fw"].AsString,
                    Pv = item["pv"].AsInt32,
                    Uv = item["uv"].AsInt32,
                    Params = JsonConvert.DeserializeObject<Dictionary<string, string>>(item["params"].ToJson())
                });
            }
            return result;
        }

        /// <summary>
        /// 获取链接浏览时长
        /// </summary>
        public List<UrlTimes> GetUrlTimes(List<string> urls, List<DateTime> dates)
        {
            List<string> pipelineJson = new List<string>
            {
                "{ $project: { time: { \"$add\":[\"$time\", 28800000] },day: {$substr: [{ \"$add\":[\"$time\", 28800000] }, 0, 10] }, \"url\": 1 , \"userid\": 1, \"deviceid\": 1 ,\"event\": 1 ,\"fw\": 1 , \"params\": 1}}"
            };

            List<string> querys = new List<string>();
            if (urls.Any())
            {
                List<string> urlQuerys = new List<string>();
                foreach (var item in urls)
                {
                    urlQuerys.Add("{ \"url\" :'" + item + "'}");
                }
                string urlstr = string.Join(",", urlQuerys);
                querys.Add("{ $or:[ " + urlstr + " ]}");
            }
            if (dates.Any())
            {
                List<string> dateQuerys = new List<string>();
                foreach (var item in dates)
                {
                    dateQuerys.Add("{$and: [{ \"time\": { $gte: new Date('" + item.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "') } }, { \"time\": { $lt: new Date('" + item.AddHours(8).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss") + "') } }]}");
                }
                string datestr = string.Join(",", dateQuerys);
                querys.Add("{ $or:[ " + datestr + " ]}");
            }

            pipelineJson.Add("{ $match: { $and:  [  { 'event': \"tp\"  } ," + string.Join(",", querys) + "]}}");
            pipelineJson.Add("{ $group: { '_id': { 'date':'$day','fw':'$fw'},  'times': { '$sum': '$params.time'} }}");
            pipelineJson.Add("{ $project: { \"_id\":0,'fw':'$_id.fw', \"date\":\"$_id.date\", \"times\":1}}");

            var resultBson = GetAggregate(pipelineJson);
            List<UrlTimes> timesResult = new List<UrlTimes>();
            foreach (var item in resultBson)
            {
                var a = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<UrlTimes>(item);
                timesResult.Add(a);
            }
            return timesResult;
        }


        /// <summary>
        /// 获取链接跳转数
        /// </summary>
        public int GetUrlJumpCount(List<string> urls, DateTime startDate)
        {
            List<string> builder1 = new List<string>();

            foreach (var item in urls)
            {
                builder1.Add("{ \"referer\" :'" + item + "'}");
            }
            string urlstr = string.Join(",", builder1);

            List<string> pipelineJson = new List<string>
            {
                 "{ $project: { time: { \"$add\":[\"$time\", 28800000] },day: {$substr: [{ \"$add\":[\"$time\", 28800000] }, 0, 10] }, \"referer\": 1, \"event\": 1 ,\"url\":1,\"module\":1}}",
                 "{ $match: {  $and:  [ { 'event': null },{ 'module': {$ne:'GetAdvs'} },{ 'module':{$ne:'GetJsdk'}} ,{ \"time\": { $gte: new Date('" + startDate.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "') } }, { \"time\": { $lt: new Date('" + startDate.AddHours(8).AddDays(1).ToString("yyyy-MM-dd  HH:mm:ss") + "') } } , { $or:[ " + urlstr + " ]} ] } }",
                 "{ $group: {  '_id': 1,  'count': { '$sum': 1}} }",
                 "{ $project: { \"_id\":0,  \"count\":1 }  }"
            };
            var resultBson = GetAggregate(pipelineJson);

            if (resultBson.Any())
            {
                return resultBson[0]["count"].AsInt32;
            }
            return 0;
        }


        /// <summary>
        /// 获取链接分享数
        /// </summary>
        public int GetUrlShareCount(List<string> urls, DateTime startDate)
        {
            List<string> builder1 = new List<string>();

            foreach (var item in urls)
            {
                builder1.Add("{ \"url\" :'" + item + "'}");
            }
            string urlstr = string.Join(",", builder1);

            List<string> pipelineJson = new List<string>
            {
                 "{ $project: { time: { \"$add\":[\"$time\", 28800000] },day: {$substr: [{ \"$add\":[\"$time\", 28800000] }, 0, 10] }, \"referer\": 1, \"event\": 1 ,\"url\":1,\"module\":1}}",
                 "{ $match: {  $and:  [ { 'event': 'share' },{ \"time\": { $gte: new Date('" + startDate.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "') } }, { \"time\": { $lt: new Date('" + startDate.AddHours(8).AddDays(1).ToString("yyyy-MM-dd  HH:mm:ss") + "') } } , { $or:[ " + urlstr + " ]} ] } }",
                 "{ $group: {  '_id': 1,  'count': { '$sum': 1}} }",
                 "{ $project: { \"_id\":0,  \"count\":1 }  }"
            };
            var resultBson = GetAggregate(pipelineJson);

            if (resultBson.Any())
            {
                return resultBson[0]["count"].AsInt32;
            }
            return 0;
        }


        public List<BsonDocument> GetAggregate(List<string> pipelineJson)
        {
            string collectionName = "statistics";
            IList<IPipelineStageDefinition> stages = new List<IPipelineStageDefinition>();

            foreach (var line in pipelineJson)
            {
                stages.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>(line));
            }

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(stages);

            //查询结果
            var result = GetCollection(collectionName).Aggregate(pipeline).ToList();
            return result;
        }
        private IMongoCollection<BsonDocument> GetCollection(string collectionNmae)
        {
            //return _mongo.ImongdDb.GetCollection<BsonDocument>(collectionNmae);
            return _database.GetCollection<BsonDocument>(collectionNmae);
        }
    }
}
