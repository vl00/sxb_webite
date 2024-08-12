using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Foundation;
using Sxb.UserCenter.Models.CommonViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class CommonController : Base
    {
        private readonly ImageSetting _setting;

        public CommonController(IOptions<ImageSetting> set)
        {
            _setting = set.Value;
        }

        public ResponseResult AppUploadImager([FromBody]List<string> imagesArr)
        {
            Guid Id = Guid.NewGuid();
            try
            {
                List<string> ImagerUrl = new List<string>();

                for (int i = 0; i < imagesArr.Count(); i++)
                {
                    string baseStr = imagesArr[i];

                    string[] sources = baseStr.Split(',');
                    //base64转字节
                    byte[] postData = Convert.FromBase64String(sources[1]);
                    //字节转image
                    Image img = GetImageExtHelper.byte2img(postData);
                    //获取图片扩展名
                    string ext = GetImageExtHelper.GetImageExt(img);
                    string commentImg = $"Comment/{Id}/{i}.{ext}";
                    //上传图片
                    int statusCode = UploadImager.UploadImagerByBase64(_setting.UploadImager + commentImg, postData);
                    if (statusCode == 200)
                    {
                        ImagerUrl.Add("/images/school_v3/" + commentImg);
                    }
                }
                return ResponseResult.Success(new { ImagerUrl, Id }, "图片上传成功");
            }
            catch (Exception)
            {
                return ResponseResult.Failed(new { Id });
            }
        }


        public ResponseResult TalentUploadImager([FromBody] List<string> imagesArr)
        {
            Guid Id = Guid.NewGuid();
            try
            {
                List<string> ImagerUrl = new List<string>();

                for (int i = 0; i < imagesArr.Count(); i++)
                {
                    string baseStr = imagesArr[i];

                    string[] sources = baseStr.Split(',');
                    //base64转字节
                    byte[] postData = Convert.FromBase64String(sources[1]);
                    //字节转image
                    Image img = GetImageExtHelper.byte2img(postData);
                    //获取图片扩展名
                    string ext = GetImageExtHelper.GetImageExt(img);
                    string url = $"Talent/{Id}/{i}.{ext}";
                    //上传图片
                    int statusCode = UploadImager.UploadImagerByBase64(_setting.UploadImager + url, postData);
                    if (statusCode == 200)
                    {
                        ImagerUrl.Add("/images/school_v3/" + url);
                    }
                }
                return ResponseResult.Success(new { ImagerUrl, Id }, "图片上传成功");
            }
            catch (Exception)
            {
                return ResponseResult.Failed(new { Id });
            }
        }
    }
}