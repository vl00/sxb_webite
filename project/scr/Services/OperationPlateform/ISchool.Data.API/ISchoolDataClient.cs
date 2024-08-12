using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using iSchool.Data.API.Model;
using Refit;
using Newtonsoft.Json;
using System.Linq;

namespace iSchool.Data.API
{
    public static class HttpContentExt
    {
        public async static Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(jsonString);

        }


    }

    public class ISchoolDataClient : ITagServices, ISchoolServices
    {
        private readonly HttpClient _httpClient;

        public ISchoolDataClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<AddTagResult> AddTags([Body(BodySerializationMethod.Serialized)] string[] names)
        {
            throw new NotImplementedException();
        }

        public async Task<QuerySchoolExtentionReuslt> GetByFinalType([Body(BodySerializationMethod.Serialized)] GetByFinalTypeRequest req)
        {
            string url = "/api/School/GetByFinalType";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsAsync<QuerySchoolExtentionReuslt>();

            return result;

        }

        public async Task<QueryTagsResult> GetByIds([AliasAs("Ids"), Query(CollectionFormat.Multi)] string[] ids)
        {
            string url = "/api/GeneralTag/GetByIds";
            var _ids = ids.Select(id => string.Format("ids={0}", id));
            string _params = string.Join("&", _ids);

            var response = await _httpClient.GetAsync(url + "?" + _params);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<QueryTagsResult>();

            return result;
        }

        public Task<QueryTagsResult> GetByName([AliasAs("name")] string name, [AliasAs("like")] bool like)
        {
            throw new NotImplementedException();
        }

        public Task<QueryUniversityResult> GetlistByName([Body(BodySerializationMethod.Serialized)] string[] names)
        {
            throw new NotImplementedException();
        }

        public Task<QuerySchoolResult> GetSchoolBy([AliasAs("Name"), Query] string name, [AliasAs("Count"), Query] int count = 20)
        {
            throw new NotImplementedException();
        }

        Task<QuerySchoolResult> ISchoolServices.GetByIds(string[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
