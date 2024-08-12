using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeChat.Interface;
using System.IO;

namespace WeChat.Implement
{
    public class MediaService : IMediaService
    {
        HttpClient _client;
        ILogger _logger;
        public MediaService(HttpClient client, ILogger<MediaService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> AddTempMedia(string accessToken, MediaType mediaType, byte[] buffer,string filename)
        {
            string url = $@"https://api.weixin.qq.com/cgi-bin/media/upload?access_token={accessToken}&type={mediaType}";
            string boundary = "--------------------------932990319647657801709709"; //乱取的，时间戳之类。
            MultipartFormDataContent  formdata =  new MultipartFormDataContent(boundary) ;
            ByteArrayContent byteArrayContent = new ByteArrayContent(buffer);
            formdata.Add(byteArrayContent, "media");
            byteArrayContent.Headers.Remove("Content-Disposition");
            byteArrayContent.Headers.TryAddWithoutValidation("Content-Disposition", $"form-data; name=\"media\"; filename=\"{filename}\"");
            byteArrayContent.Headers.Remove("Content-Type");
            byteArrayContent.Headers.TryAddWithoutValidation("Content-Type", $"{mediaType}/*");
            formdata.Headers.Remove("Content-Type");
            formdata.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            string result = await _client.PostAsync(url, formdata, _logger);
            var jobj = JObject.Parse(result);
            if (jobj.TryGetValue("media_id", out JToken value))
            {
                string mediaId = value.Value<string>();
                return mediaId;
            }
            else
            {
                _logger.LogWarning(result);
                return null;
            }
        }
    }
}
