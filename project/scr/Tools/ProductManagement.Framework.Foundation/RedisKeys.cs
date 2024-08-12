using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public static class RedisKeys
    {
        /// <summary>
        /// 资料包页面的缓存key
        /// </summary>
        public static string AdDataPacketKey = "m:ad:dp:md5url:{0}";
        public static string AdDataPacketMiddlePageKey = "m:ad:dp:middle:page:{0}";

        //资料包订阅推送的关键字 new[] { "幼升小", "小升初", "中考", "高考" };
        public static string AdDataPacketSubscribeKeys = "m:ad:dp:subscribe:keys";
        //资料包48h推送的关键字 new[] { "幼升小加群", "小升初加群", "中考加群", "高考加群" };
        public static string AdDataPacket48hKeys = "m:ad:dp:48h:keys";
        //资料包订阅推送的链接
        public static string AdDataPacketSubscribeUrlKeys = "m:ad:dp:subscribe:urls";
        //资料包24h推送的链接
        public static string AdDataPacket24hUrlKeys = "m:ad:dp:24h:urls";
        //资料包36h推送的链接
        public static string AdDataPacket36hUrlKeys = "m:ad:dp:36h:urls";

        //搜索热词
        public static string HotwordsChannelKeys = "m:hotwords:channel_{0}";


        //pc聚合首页缓存
        public static string AggHomeSearchKey = "m:agghome:search";

        //48h浏览数最高的100所学校
        //public static string HotSchoolsKey = "m:hot:school:viewcount:desc";
        public static string HotHistoryKey = "m:hot:history:{0}:orderDescByViewCount";
        //最新发布的100篇文章
        public static string NewArticlesKey = "m:new:article:orderDescByPublishTime";

        //所有的学校标签
        public static string SchoolTagsKey = "m:school:tags";
        //热点词 - groupName
        public static string HotspotKey = "m:hotspot:{0}:{1}:{2}";

        //登录凭证
        public static string UserLoginCodeKey= "u:login:code:{0}";

        //学校学费区间的查询次数
        public static string SchoolCostSearch = "m:school:cost:search";

        //雄哥缓存
        public static string OrgUserSimpleUserId = "org:user:simple_{0}";

        //推荐学校缓存 extId - pageIndex - pageSize
        public static string RecommendSchoolKey = "m:recommend:school:{0}_{1}_{2}";

        //uv缓存 m:uv:school:440100
        public static string DataUvKey = "m:uv:{0}:{1}";
        //推荐缓存 m:uv:school:440100:1_10
        public static string DataUvRecommendKey = "m:recommend:uv:{0}:{1}:{2}_{3}";

        //聚合页右侧推荐数据
        public static string AggRecommendKey = "m:aggrecommend:{0}";
    }
}
