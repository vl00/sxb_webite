namespace ProductManagement.Framework.MongoDb
{
    public class MongoDbConfig<T> : MongoDbConfig { 
    
    }

    /// <summary>
    /// Mongodb数据库配置信息
    /// </summary>
    public class MongoDbConfig
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        /// <summary>
        /// 写关注
        /// </summary>
        public string WriteCountersign { get; set; }
    }
}
