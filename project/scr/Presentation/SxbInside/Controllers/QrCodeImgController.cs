using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Tool.QRCoder;
using Sxb.Inside.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class QrCodeImgController : Controller
    {
        [HttpPost]
        public ResponseResult Scan()
        {
            try
            {
                IFormFileCollection cols = Request.Form.Files;
                if (cols == null || cols.Count == 0)
                {
                    return ResponseResult.Failed("请上传图片");
                }
                var file = cols[0];
                string resultText = null;

                //定义图片数组后缀格式
                string[] LimitPictureType = { ".JPG", ".JPEG", ".GIF", ".PNG", ".BMP" };
                //获取图片后缀是否存在数组中
                string currentPictureExtension = Path.GetExtension(file.FileName).ToUpper();
                if (LimitPictureType.Contains(currentPictureExtension))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        Bitmap bmap = new Bitmap(stream);
                        resultText = QrCodeScan.Scan(bmap);
                    }
                }
                else
                {
                    ResponseResult.Failed("请上传指定格式的图片");
                }

                return ResponseResult.Success(new {
                    resultText
                });
            }
            catch (Exception ex)
            {

                return ResponseResult.Failed("图片识别失败");
            }
        }
    }
}
