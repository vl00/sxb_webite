using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sxb.Inside.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class TextSplitController : Controller
    {
        private readonly HttpClient _searchHttpClient;
        public TextSplitController(IHttpClientFactory httpClientFactory)
        {
            _searchHttpClient = httpClientFactory.CreateClient("search");
        }



        // GET: api/values
        [HttpPost]
        public async Task<ResponseResult> GetGram(string text)
        {
            string requestUrl = $"http://elastic:sxb123@10.1.0.7:9200/schoolindex_07/_analyze";
            var param = new
            {
                text,
                analyzer= "ik_smart"
            };

            string jsoncontent = JsonConvert.SerializeObject(param);
            var stringContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");

            _searchHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "elastic", "sxb123"))));


            var response = await _searchHttpClient.PostAsync(requestUrl, stringContent);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var words = JsonConvert.DeserializeObject<TextSplit>(jsonResponse);

            var respWords = new List<string>();

            for (int i = 0; i < words.Tokens.Count() - 1; i++)
            {
                StringBuilder gramWord = new StringBuilder();
                //gramWord.Append(words.Tokens[i].Token);
                int lastWordLength = words.Tokens.Count() - i;
                for (int j = 0; j < (lastWordLength >= 6 ? 6 : lastWordLength); j++)
                {
                    gramWord.Append(words.Tokens[i+j].Token);
                }
                respWords.Add(gramWord.ToString());
            }

            return ResponseResult.Success(new
            {
                //words,
                respWords
            });
        }

        public class TextSplit
        {
            public List<Word> Tokens { get; set; }

            public class Word
            {
                public string Token { get; set; }
            }
        }
    }
}
