using PMS.TopicCircle.Application.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public  interface IFileUploadService
    {

        /// <summary>
        /// 上传圈子背景图
        /// </summary>
        /// <param name="fielName"></param>
        /// <param name="file"></param>
        Task<UploadImgeResponseDto> UploadCircleCover(string fielName, Stream file);

        /// <summary>
        /// 上传话题评论图片
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<UploadImgeResponseDto> UploadTopicImage(string filename, Stream file);
        List<UploadImgeResponseDto> UploadTopicImage(Dictionary<string, Stream> imgs);
    }
}
