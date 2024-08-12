using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Result.Org;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{

    public class OrgServiceClient : HttpBaseClient<OrgClientConfig>, IOrgServiceClient
    {
        ILogger<OrgServiceClient> _logger;
        public OrgServiceClient(HttpClient client, ILogger<OrgServiceClient> logger, IOptions<OrgClientConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
            base._client.BaseAddress = new Uri(config.Value.ServerUrl);
            _logger = logger;
        }
        public async Task<List<string>> OrgsOrderBanners(OrgOrderBannerRequest request)
        {
            try
            {
              
                string url = "/GetOrderBanner";
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
              
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var r=JsonConvert.DeserializeObject<OrgOrderBannerListResult>(responseContent);
                //var r= await response.Content.ReadAs<OrgOrderBannerListResult>();
                return r.data.ToList();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return new List<string>();
            }
        }
        public async Task<CardListResult<CourseCard>> CoursesLablesByIds(CoursesLablesByIdsRequest request)
        {
            try
            {
                string url = "/CoursesLablesByIds";
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<CardListResult<CourseCard>>();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }

        }

        public async Task<CardListResult<CourseCard>> CoursesLablesById_ss(CoursesLablesById_SSRequest request)
        {
            try
            {
                string url = "/CoursesLablesById_ss";

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<CardListResult<CourseCard>>();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }
        }


        public async Task<CardListResult<OrgEvalutionCard>> EvalsLablesByIds(EvalsLablesByIdsRequest request)
        {
            try
            {
                string url = "/EvalsLablesByIds";
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<CardListResult<OrgEvalutionCard>>();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }

        }

        public async Task<CardListResult<OrgEvalutionCard>> EvalsLablesById_ss(EvalsLablesById_SSRequest request)
        {
            try
            {
                string url = "/EvalsLablesById_ss";
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<CardListResult<OrgEvalutionCard>>();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }

        }

        public async Task<CardListResult<OrgCard>> OrgsLablesByIds(OrgsLablesByIdsRequest request)
        {
            try
            {
                string url = "/OrgsLablesByIds";
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<CardListResult<OrgCard>>();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }
        }

        public async Task<CardListResult<OrgCard>> OrgsLablesById_ss(OrgsLablesById_SSRequest request)
        {
            try
            {
                string url = "/OrgsLablesById_ss";
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAs<CardListResult<OrgCard>>();
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }
        }

        public async Task<IEnumerable<CourseDto>> GetCourses(IEnumerable<Guid> ids)
        {
            string url = "/api/mini/Courses/Search";
            var postData = new { ids };
            var result = await OrgPostJson<IEnumerable<CourseDto>>(url, postData);
            return result ?? Enumerable.Empty<CourseDto>();
        }

        public async Task<IEnumerable<EvaluationDto>> GetEvaluations(IEnumerable<Guid> ids)
        {
            string url = "/api/mini/Evaluations/Search";
            var postData = new { ids };
            var result = await OrgPostJson<IEnumerable<EvaluationDto>>(url, postData);
            return result ?? Enumerable.Empty<EvaluationDto>();
        }


        public async Task<IEnumerable<HotSellCourse>> GetHotSellCoursesByIds(IEnumerable<Guid> ids, IEnumerable<string> sIds)
        {
            string url = "/api/ToSchools/infos/courses";
            var postData = new { ids, sIds, mp = false }; //是否需要返回小程序二维码测试默认false, 正式默认true
            var result = await OrgPostJson<HotSellCourse.HttpWrapper>(url, postData);
            return result?.Courses ?? Enumerable.Empty<HotSellCourse>();
        }

        public async Task<IEnumerable<RecommendOrg>> GetRecommendOrgsByIds(IEnumerable<Guid> ids, IEnumerable<string> sIds)
        {
            string url = "/api/ToSchools/infos/orgs";
            var postData = new { ids, sIds, mp = false }; //是否需要返回小程序二维码测试默认false, 正式默认true
            var result = await OrgPostJson<RecommendOrg.HttpWrapper>(url, postData);
            return result?.Orgs ?? Enumerable.Empty<RecommendOrg>();
        }

        public async Task<T> OrgPostJson<T>(string url, object data)
            where T : class
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();

                var a = await response.Content.ReadAsStringAsync();

                var result = await response.Content.ReadAs<BaseResult<T>>();
                if (result != null && result.succeed)
                {
                    return result.data;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, null);
            }
            return default;
        }

        public async Task<IEnumerable<GG21Course>> GetGG21Courses(GetGG21CoursesRequest request)
        {

            try
            {
                string url = "/api/ToSchools/gg21";

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        request.price,
                        request.subjs,
                        request.mp,
                        ages = request.ages.Select(a => new { a.minAge, a.maxAge }),
                    }
                    );
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await base._client.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                var strRes = await response.Content.ReadAsStringAsync();
                var jobjRes = JObject.Parse(strRes);
                if (jobjRes["succeed"].Value<bool>())
                {
                    var jsonCourses = jobjRes["data"]["courses"].ToString();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<GG21Course>>(jsonCourses);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, null);
                return null;
            }

        }
    }
}
