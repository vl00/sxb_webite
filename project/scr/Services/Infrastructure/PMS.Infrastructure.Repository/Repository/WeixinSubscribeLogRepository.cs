using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using System.ComponentModel;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using WeChat.Model;
using PMS.Infrastructure.Domain.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Infrastructure.Repository.Repository
{
    public class WeixinSubscribeLogRepository : IWeixinSubscribeLogRepository
    {
        private JcDbContext _dbContext;
        public WeixinSubscribeLogRepository(JcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(WeixinSubscribeLog weixinSubscribeLog)
        {
            return _dbContext.Add(weixinSubscribeLog) != null;

            //        var result = await _dbContext.ExecuteAsync($@"
            //INSERT INTO [dbo].[weixin_subscribe_log]
            //    ([OpenId]
            //    ,[SubscribeTime]
            //    ,[FromWhere]
            //    ,[Channel]
            //    ,[AppId]
            //    ,[DataId]
            //    ,[DataUrl]
            //    ,[DataJson]
            //    ,[Type]
            //)
            //VALUES
            //    (@OpenId
            //    , @SubscribeTime
            //    , @FromWhere
            //    , @Channel
            //    , @AppId
            //    , @DataId
            //    , @DataUrl
            //    , @DataJson
            //    , @Type
            //) ", weixinSubscribeLog);
            //            return result > 0;
        }

        public async Task<IEnumerable<WeixinSubscribePvDto>> GetSubscribePvAsync(string[] types, string[] weChatEvents, bool? isFirstSubscribe, DateTime startTime, DateTime endTime)
        {
            if (weChatEvents == null || !weChatEvents.Any())
            {
                return Enumerable.Empty<WeixinSubscribePvDto>();
            }

            var sql = $@"
SELECT 
	FromWhere, 
	Date,
	SUM(Pv) as Pv, 
	COUNT(1) as Uv
FROM
(
	select
	    FromWhere, COUNT(1) as Pv, CONVERT(varchar(10), CreateTime, 111) as Date
	from
	    weixin_subscribe_log
	where
	    1=1
	    AND FromWhere IS NOT NULL
        AND Type in @types
        AND (@isFirstSubscribe is null or IsFirstSubscribe = @isFirstSubscribe)
        AND WeChatEvent in @weChatEvents
        AND CreateTime between @startTime and @endTime
	group by
	    OpenId, FromWhere, CONVERT(varchar(10), CreateTime, 111)
) AS T
group by
	FromWhere, Date
";
            return await _dbContext.QueryAsync<WeixinSubscribePvDto>(sql, new { types, weChatEvents, isFirstSubscribe, startTime, endTime });

        }
    }
}
