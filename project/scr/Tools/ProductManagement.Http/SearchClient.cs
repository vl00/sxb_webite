using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest;

namespace ProductManagement.API.Http
{
    public class SearchClient : HttpBaseClient<SchoolSearch>, ISearchClient
    {
        public SearchClient(HttpClient client, IOptions<SchoolSearch> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            //Config = config.Value;
        }
        
        /// <summary>
        /// 搜索学校名称和城市ID
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public async  Task<SearchResult> Search(GetSchoolByNameAndAdCode para)
        {
            search search = new search(para);
            return await GetAsync<SearchResult, search>(search);
        }



    }
}
