using Microsoft.Extensions.Options;
using PMS.Infrastructure.Domain.IRepositories;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IMongo;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.OperationPlateform.Repository;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    public class DataPacketService : IDataPacketService
    {
        private readonly IDataPacketRepository _dataPacketRepository;
        private readonly ISystemEventLogRepository _systemEventLogRepository;
        private readonly IStatisticRepository _statisticRepository;
        private readonly IEasyRedisClient _easyRedisClient;

        public DataPacketService(IDataPacketRepository dataPacketRepository, ISystemEventLogRepository systemEventLogRepository, IStatisticRepository statisticRepository, IEasyRedisClient easyRedisClient)
        {
            _dataPacketRepository = dataPacketRepository;
            _systemEventLogRepository = systemEventLogRepository;
            _statisticRepository = statisticRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<IEnumerable<DataPacket>> GetList(DataPacketStep? step, DataPacketStatus? status, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            return await _dataPacketRepository.GetList(step, status, startTime, endTime, pageIndex, pageSize);
        }

        public async Task<IEnumerable<DataPacket>> GetStep(DataPacketStep step, bool uncheckTime = false)
        {
            var nextStep = step + 1;
            var hour = nextStep.GetDefaultValue<int>();

            var status = DataPacketStatus.Subscribe;
            var startTime = DateTime.Now.AddDays(-7); //删选七天内
            var endTime = DateTime.Now.AddHours(-hour); //关注公众号时间达到 24/36/48
            if (uncheckTime)
            {
                //不论有没有达到24小时, 都发送
                endTime = DateTime.Now;
            }

            var pageIndex = 1;
            var pageSize = 9999999;
            return await _dataPacketRepository.GetList(step, status, startTime, endTime, pageIndex, pageSize);
        }

        public DataPacket Add(DataPacket dataPacket)
        {
            var existDataPacket = _dataPacketRepository.TakeFirst("UserId = @userId and ScanPage = @scanPage", new { userId = dataPacket.UserId, scanPage = dataPacket.ScanPage });
            if (existDataPacket != null)
            {
                existDataPacket.ScanCount++;
                Update(existDataPacket);
                return existDataPacket;
            }

            dataPacket.Id = Guid.NewGuid();
            dataPacket.Code = string.IsNullOrWhiteSpace(dataPacket.Code) ? dataPacket.Id.ToString() : dataPacket.Code;
            dataPacket.ScanTime = dataPacket.ScanTime != null ? dataPacket.ScanTime : DateTime.Now;
            dataPacket.Status = DataPacketStatus.Sacn;
            dataPacket.ScanCount = 1;
            return _dataPacketRepository.Add(dataPacket);
        }

        public bool Update(DataPacket dataPacket)
        {
            return _dataPacketRepository.Update(dataPacket) != null;
        }


        public bool SubscribeWxCallback(Guid id, Guid userId)
        {
            var dataPacket = _dataPacketRepository.TakeFirst("Id = @id", new { id });
            if (dataPacket == null)
            {
                return false;
            }

            //扫别人的生成二维码, 再建立一条自己的记录
            if (dataPacket.UserId != userId)
            {
                dataPacket = Add(new DataPacket()
                {
                    Code = dataPacket.Code,
                    UserId = userId,
                    ScanTime = dataPacket.ScanTime,
                    ScanPage = dataPacket.ScanPage,
                    DataId = dataPacket.DataId
                });
            }

            //更新记录为已关注
            UpdateSubscribe(dataPacket);
            return _dataPacketRepository.Update(dataPacket) != null;
        }

        public bool UpdateSubscribe(DataPacket dataPacket)
        {
            //以前未从资料包广告关注过
            if (dataPacket.Status != DataPacketStatus.Subscribe)
            {
                dataPacket.SubscribeTime = DateTime.Now;
                dataPacket.Status = DataPacketStatus.Subscribe;
                dataPacket.Step = DataPacketStep.Subcribe;
            }
            return Update(dataPacket);
        }


        public bool UpdateStep(Guid id, DataPacketStep step)
        {
            var dataPacket = _dataPacketRepository.TakeFirst("Id = @id", new { id });
            if (dataPacket == null)
            {
                return false;
            }
            if (dataPacket.Status != DataPacketStatus.Subscribe)
            {
                return false;
            }

            dataPacket.Step = step;
            return Update(dataPacket);
        }

        /// <summary>
        /// 资料包数据总表
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<DataPacketSummary>> Summary(DateTime startDate, DateTime endDate)
        {
            var groups = await _dataPacketRepository.GetGroups(startDate, endDate.AddDays(1));
            var summarys = groups.Select(s => new DataPacketSummary()
            {
                Date = s.Date,
                ScanPage = s.ScanPage,
                SubscribeCount = s.SubcribeCount,
                MiddlePageUV = s.ScanCount
            }).ToList();

            //var days = (endDate.Date - startDate.Date).Days;
            foreach (var item in summarys)
            {
                var curStartDate = item.Date.Date;
                var curEndDate = curStartDate.AddDays(1).AddMilliseconds(-1);

                var scanPageView = _statisticRepository.GetUrlViewCount(item.ScanPage, curStartDate) ?? new Domain.MongoModel.UrlViewCount();
                item.ScanPagePV = scanPageView.Pv;
                item.ScanPageUV = scanPageView.Uv;
            }
            return summarys;
        }

        /// <summary>
        /// 资料包用户行为表
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<DataPacketUserSummary>> UserSummary(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.AddDays(1);

            var groups = await _dataPacketRepository.GetSubscribeUsers(startDate, endDate);
            var summarys = groups.Select(s => new DataPacketUserSummary()
            {
                UserId = s.UserId,
                OpenId = s.OpenId,
                NickName = s.NickName,
                SubscribeTime = s.SubscribeTime,
                UnSubscribeTime = s.LastUnSubscribeTime,
                IsSubcribe = s.IsSubcribe
            }).ToList();

            // new[] { "幼升小", "小升初", "中考", "高考" };
            string[] subscribeKeys = await _easyRedisClient.GetAsync<string[]>(RedisKeys.AdDataPacketSubscribeKeys);
            //new[] { "幼升小加群", "小升初加群", "中考加群", "高考加群" };
            string[] keys48 = await _easyRedisClient.GetAsync<string[]>(RedisKeys.AdDataPacket48hKeys);

            string[] subscribeUrls = await _easyRedisClient.GetAsync<string[]>(RedisKeys.AdDataPacketSubscribeUrlKeys);
            string[] urls24 = await _easyRedisClient.GetAsync<string[]>(RedisKeys.AdDataPacket24hUrlKeys);
            string[] urls36 = await _easyRedisClient.GetAsync<string[]>(RedisKeys.AdDataPacket36hUrlKeys);

            //string[] subscribeUrls = new[] {
            //    "https://m.sxkid.com/?fw=hyy",
            //    "https://m.sxkid.com/live/client/livelist.html?fw=hyy",
            //    "https://m.sxkid.com/mechanism/?fw=hyy",
            //    //"https://m.sxkid.com/ask/home/?fw=hyy",
            //};
            //string[] urls24 = new[] {
            //    "https://m.sxkid.com/school/extschoolfilter/?fw=001",
            //    "https://m.sxkid.com/org/?fw=001",
            //    //"https://m.sxkid.com/ask/home/?fw=001",
            //};
            //string[] urls36 = new[] {
            //    "https://m.sxkid.com/org/course?fw=002",
            //    //"https://m.sxkid.com/ask/home/?fw=002",
            //};
            foreach (var item in summarys)
            {
                var userId = item.UserId;
                var itemStartTime = item.SubscribeTime;
                var item24StartTime = itemStartTime.AddHours(DataPacketStep.Above24h.GetDefaultValue<int>());
                var item36StartTime = itemStartTime.AddHours(DataPacketStep.Above36h.GetDefaultValue<int>());
                var item48StartTime = itemStartTime.AddHours(DataPacketStep.Above48h.GetDefaultValue<int>());

                //回复的欢迎语关键字
                var contents = await _dataPacketRepository.GetWeixinReplyMsgs(item.OpenId, itemStartTime, subscribeKeys);
                item.ReplyKeyBySubscribe = string.Join(",", contents);

                //回复的48小时关键字
                contents = await _dataPacketRepository.GetWeixinReplyMsgs(item.OpenId, item48StartTime, keys48);
                item.ViewPageUrlBy48h = string.Join(",", contents);

                //欢迎语点击链接地址
                item.ViewPageUrlBySubscribe = await GetHitUrls(userId, subscribeUrls, itemStartTime);

                //24小时点击链接地址
                item.ViewPageUrlBy24h = await GetHitUrls(userId, urls24, item24StartTime);

                //36小时点击链接地址
                item.ViewPageUrlBy36h = await GetHitUrls(userId, urls36, item36StartTime);

            }
            return summarys;
        }

        public async Task<string> GetHitUrls(Guid userId, string[] urlsSearchMongo, DateTime startTime)
        {
            if (urlsSearchMongo == null)
            {
                return default;
            }
            //ask的埋点数据不在mongodb中
            string[] urlsSearchSql = urlsSearchMongo.Where(s => 
                s.StartsWith("https://m.sxkid.com/ask/home/")
                || s.StartsWith("https://m3.sxkid.com/ask/home/")
                || s.StartsWith("https://m4.sxkid.com/ask/home/")
             ).ToArray();

            //mongodb中记录的点击url
            var hitMongoUrls = _statisticRepository.GetHitUrls(userId, urlsSearchMongo, startTime);
            //sql server中记录的点击url
            var hitSqlUrls = await _systemEventLogRepository.GetHitUrls(userId, urlsSearchSql, startTime);

            return string.Join(",", hitMongoUrls.Concat(hitSqlUrls));
        }
    }
}