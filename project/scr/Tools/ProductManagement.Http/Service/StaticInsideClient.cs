using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.StaticInside;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest;

namespace ProductManagement.API.Http.Service
{
    public class StaticInsideClient : HttpBaseClient<StaticInsideConfig>, IStaticInsideClient
    {
        public StaticInsideClient(HttpClient client, IOptions<StaticInsideConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
        }

        /// <summary>
        ///  "school", "article", "schoolrank", "topic", "talent", "comment", "question", "course", "evaluation", "live"
        /// </summary>
        public enum StaticInsideType { 
            School,
            Article,
            SchoolRank,
            Topic,
            Circle,
            Talent,
            Comment,
            Question,
            Course,
            Evaluation,
            Live
        }


        public async Task<IEnumerable<StaticDataUV>> GetDatUv(StaticInsideType type, int cityId, int days = 7)
        {
            //https://static-inside.sxkid.com/440100/7/article.json
            var url = $"/{cityId}/{days}/{type.ToString().ToLower()}.json";
            try
            {
                var result = await GetAsync<List<StaticDataUV>>(url, null);
                return result ?? Enumerable.Empty<StaticDataUV>();
            }
            catch
            {
                return Enumerable.Empty<StaticDataUV>();
            }
        }
    }
}
