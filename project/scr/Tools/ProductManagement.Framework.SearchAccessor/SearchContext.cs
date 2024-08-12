using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Elasticsearch.Net;
using Nest;

namespace ProductManagement.Framework.SearchAccessor
{
    internal class SearchContext : ISearch
    {
        readonly SearchConfig searchConfig;

        private ElasticClient _client;

        private string _defaultIndex = "defaultindex";

        public SearchContext(SearchConfigOptions option)
        {

            searchConfig = option.ConnectionConfig;
        }

        public ElasticClient GetClient()
        {
            if (_client != null)
                return _client;
            InitClient();
            return _client;
        }

        private void InitClient()
        {
            if (!string.IsNullOrWhiteSpace(searchConfig.DefultIndexName))
            {
                _defaultIndex = searchConfig.DefultIndexName;
            }

            var lst = searchConfig.ServerUrl.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var nodeUris = lst.Select(x => new Uri(x)).ToArray();
            var pool = new StaticConnectionPool(nodeUris);
            var settings = new ConnectionSettings(pool);
            settings.DefaultIndex(_defaultIndex);
            _client = new ElasticClient(settings);
        }

        
    }
}