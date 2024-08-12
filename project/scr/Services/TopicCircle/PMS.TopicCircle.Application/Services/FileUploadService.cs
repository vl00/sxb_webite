using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSchool.API;
using iSchool.API.Model;
using Microsoft.Extensions.Logging;
using PMS.TopicCircle.Application.Dtos;

namespace PMS.TopicCircle.Application.Services
{
    public class FileUploadService : IFileUploadService
    {
        FileServices _fileServices;
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(FileServices fileServices, ILogger<FileUploadService> logger)
        {
            this._fileServices = fileServices;
            _logger = logger;
        }

        /// <summary>
        /// 上传圈子背景图
        /// </summary>
        /// <param name="fielName"></param>
        /// <param name="file"></param>
        public async Task<UploadImgeResponseDto> UploadCircleCover(string filename, Stream file)
        {

            string ext = Path.GetExtension(filename);
            var uploadReturn = await this._fileServices.UploadTopicCircle($"{Guid.NewGuid()}{ext}", "/circleCover", file);
            if (uploadReturn != null && uploadReturn.status == 0)
            {
                return new UploadImgeResponseDto()
                {
                    Url = uploadReturn.Url,
                    CdnUrl = uploadReturn.cdnUrl
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上传话题/评论图片
        /// </summary>
        /// <param name="fielName"></param>
        /// <param name="file"></param>
        public async Task<UploadImgeResponseDto> UploadTopicImage(string filename, Stream file)
        {
            string ext = Path.GetExtension(filename);


            UploadImgResponse uploadReturn = null;
            try
            {
                uploadReturn = await this._fileServices.UploadTopicCircle($"{Guid.NewGuid()}{ext}", "/topicImage", file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "话题图片上传错误,filename={0}", filename);
            }


            if (uploadReturn != null && uploadReturn.status == 0)
            {
                return new UploadImgeResponseDto()
                {
                    Url = uploadReturn.Url,
                    CdnUrl = uploadReturn.cdnUrl
                };
            }
            return null;
        }


        /// <summary>
        /// 上传话题/评论图片
        /// </summary>
        /// <param name="fielName"></param>
        /// <param name="file"></param>
        public List<UploadImgeResponseDto> UploadTopicImage(Dictionary<string, Stream> imgs)
        {
            var results = new List<UploadImgeResponseDto>(imgs.Count);
            imgs.AsParallel().ForAll(async img =>
            {
                var result = await UploadTopicImage(img.Key, img.Value);
                if (result != null)
                    results.Add(result);
            });

            return results;
        }

    }
}
