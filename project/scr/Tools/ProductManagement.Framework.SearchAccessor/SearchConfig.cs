namespace ProductManagement.Framework.SearchAccessor
{
    /// <summary>
    /// 查询配置
    /// </summary>
    public class SearchConfig
    {
        /// <summary>
        /// ElasticSearch Uri
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 默认索引名
        /// </summary>
        public string DefultIndexName { get; set; }
    }
}
