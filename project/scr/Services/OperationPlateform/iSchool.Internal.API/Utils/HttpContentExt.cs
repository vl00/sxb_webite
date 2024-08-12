using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace System.Net.Http
{
    public static class HttpClientExt
    {
        public async static Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var contentString = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(contentString);


        }

        public async static Task<T> GetAsAsync<T>(this HttpClient client,string requestUrl)
        {
           var response = await  client.GetAsync(requestUrl);
           return await response.Content.ReadAsAsync<T>();
        }



    }
}
