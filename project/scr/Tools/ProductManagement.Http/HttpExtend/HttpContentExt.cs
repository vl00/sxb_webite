using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Dynamic;

namespace System.Net.Http
{
    public static class HttpContentExt
    {
        public static async Task<T> ReadAs<T>(this HttpContent content) {
            string json =  await content.ReadAsStringAsync();
            

            return JsonConvert.DeserializeObject<T>(json); 
        }
        public static async Task<dynamic> ReadAsDynamic(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)
                .Aggregate(new ExpandoObject() as IDictionary<string, object>, (epob, dic) => {
                    epob.Add(dic.Key, dic.Value); 
                    return epob;
                });
            return result;
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string requestUri, T content) where T : class
        {
            string contentStr = JsonConvert.SerializeObject(content);
            HttpContent httpContent = new StringContent(contentStr,encoding: Encoding.UTF8,"application/json");
            return await client.PostAsync(requestUri, httpContent);
        }


    }
}
