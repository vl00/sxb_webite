using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Result.File;
using Sxb.Web.Areas.Tool.Models.Image;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Tool.Controllers
{

    [Route("Tool/[controller]/[action]")]
    public class ImageController : ControllerBase
    {
        private ILogger _logger;

        private IFileServiceClient _fileServiceClient;

        public ImageController(IFileServiceClient fileServiceClient, ILogger<ImageController> logger)
        {
            _fileServiceClient = fileServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// 批量上传话题图片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Description("批量上传图片")]
        public async Task<ResponseResult> UploadImages(int[] indexs = null,string type = "paidqa")
        {

            var imgs = Request.Form.Files;
            if (!imgs.Any())
            {
                return ResponseResult.Failed("请选择图片");
            }
            else
            {

                var imagesResults = new List<UploadImagesResult>(imgs.Count);
                int index = 0;
                foreach (var file in imgs)
                {
                    var result = await ImageHandle(type,file.FileName, file.OpenReadStream());
                    UploadImagesResult imgResult;
                    if (result != null)
                    {
                        
                        //上传成功情况
                        imgResult = new UploadImagesResult()
                        {
                            Url = result.Url,
                            CdnUrl = result.cdnUrl,
                            Success = true,
                        };
                        if (result.Compress != null && result.Compress.Status == 0)
                        {
                            imgResult.ThumbnailUrl = result.Compress.CdnUrl;
                        }
                    }
                    else
                    {
                        //上传失败情况
                        imgResult = new UploadImagesResult()
                        {
                            Success = false
                        };
                    }
                    if (indexs != null && indexs.Any() && index < indexs.Length)
                    {
                        imgResult.Index = indexs[index++];
                    }
                    imagesResults.Add(imgResult);
              
                }
                if (imagesResults.Any())
                {
                    return ResponseResult.Success(imagesResults);
                }
                else {
                    return ResponseResult.Failed();
                }
            }
        }

        private async Task<UploadImgResponse> ImageHandle(string type,string filename,Stream file)
        {
            string ext = Path.GetExtension(filename);

            UploadImgResponse uploadReturn = null;
            try
            {
                uploadReturn = await this._fileServiceClient.UploadImg(type, $"{Guid.NewGuid()}{ext}", file,"","750*750");

                if (uploadReturn != null && uploadReturn.status == 0)
                {
                    return uploadReturn;
                }
                else
                {
                    throw new Exception($"调用图片上传服务时，发生异常。服务返回状态Status={uploadReturn?.status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "图片上传错误,filename={0}", filename);
                return null;
            }



        }

    }
}
