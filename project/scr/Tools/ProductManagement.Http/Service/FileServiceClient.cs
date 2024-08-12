using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Result.File;
using ProductManagement.Framework.Foundation;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class FileServiceClient : HttpBaseClient<FileConfig>, IFileServiceClient
    {
        public FileServiceClient(HttpClient client, IOptions<FileConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
            base._client.BaseAddress = new Uri(config.Value.ServerUrl);

        }


        public async Task<UploadImgResponse> UploadImg(string type, string filenName, Stream stream, string path = "", string resize = "")
        {
            string url = $"/upload/{type}?filename={filenName}";
            if (!string.IsNullOrWhiteSpace(path))
            {
                url += $"&path={path}";
            }
            if (!string.IsNullOrWhiteSpace(resize))
            {
                url += $"&resize={resize}";
            }
            var content = new StreamContent(stream);
            var response = await _client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var o = JsonConvert.DeserializeObject<UploadImgResponse>(result);
            if (o.status == 0)
            {
                return o;
            }
            else
            {
                return null;
            }
        }

        public async Task<UploadImgResponse> UploadDataPacketImage(string filenName, Stream stream, string path = "", string resize = "")
        {
            var type = "datapacket";
            return await UploadImg(type, filenName, stream, path, resize);
        }

        public async Task<UploadImgResponse> UploadUserImage(string url)
        {
            var type = "user";

            var resp = await _client.GetAsync(url);
            var stream = await resp.Content.ReadAsStreamAsync();

            var contentTpye = resp.Content.Headers.ContentType.MediaType;
            var ext = MimeTypeMap.GetExtension(contentTpye);
            return await UploadImg(type, Guid.NewGuid().ToString() + ext, stream, path: "wxheadimg");
        }


        public async Task<string> ConvertToSxbImg(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "".ToHeadImgUrl();
            }

            if (url.IsSxkidDomain())
            {
                return url;
            }

            var uploadImgResponse = await UploadUserImage(url);
            return uploadImgResponse.cdnUrl ?? "".ToHeadImgUrl();
        }
    }
}
