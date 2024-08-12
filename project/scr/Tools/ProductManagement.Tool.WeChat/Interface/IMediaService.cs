using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.Interface
{
    public enum MediaType {
        image,
        voice,
        video,
        thumb,
    }

    /// <summary>
    /// 提供微信素材管理服务
    /// </summary>
    public interface IMediaService
    {

        /// <summary>
        /// 添加零时素材
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaType"></param>
        /// <param name="buffer"></param>
        /// <param name="filename">这个参数的后缀必须与mediaType对应，如image=>a.jpeg。微信依靠这个校验文件类型。</param>
        /// <returns></returns>
        Task<string> AddTempMedia(string accessToken, MediaType mediaType, byte[] buffer, string filename);
    }
}
