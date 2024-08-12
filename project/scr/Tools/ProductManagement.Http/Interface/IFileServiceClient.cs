using ProductManagement.API.Http.Result.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{

    /// <summary>
    /// 文件管理服务客户端
    /// </summary>
    public interface IFileServiceClient
    {
        /// <summary>
        /// 非sxkid.com域下的图片, 转换为sxkid.com图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<string> ConvertToSxbImg(string url);

        /// <summary>
        /// 上传资料包广告图片,中间页图片
        /// </summary>
        /// <param name="filenName"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <param name="resize"></param>
        /// <returns></returns>
        Task<UploadImgResponse> UploadDataPacketImage(string filenName, Stream stream, string path = "", string resize = "");
        Task<UploadImgResponse> UploadUserImage(string url);
        Task<UploadImgResponse> UploadImg(string type, string filenName,Stream stream, string path = "", string resize = "");
    }
}
