using iSchool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    [OpenApiIgnore]
    [Authorize]
    public class CorrectController : Controller
    {
        ISchCorrectService schCorrectService;
        IWebHostEnvironment hostingEnvironment;
        ImageSetting imgsett;

        public CorrectController(ISchCorrectService schCorrectService, IWebHostEnvironment hostingEnvironment, IOptions<ImageSetting> imgsett)
        {
            this.schCorrectService = schCorrectService;
            this.hostingEnvironment = hostingEnvironment;
            this.imgsett = imgsett.Value;
        }

        /// <summary>
        /// 学校纠错-主页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Description("学校纠错页")]
        public IActionResult Index(Guid eid)
        {
            //var types = SchFTypeUtil.Dict.Select(kv =>
            //{
            //    var g = kv.Key.Substring(0, kv.Key.IndexOf('.'));
            //    return (g.GetEnum<SchoolGrade>().Description(), kv.Key, kv.Value);
            //})
            //.GroupBy(_ => _.Item1, _ => _.Value)
            //.ToDictionary(_ => _.Key, _ => _.ToArray());


            var dto = schCorrectService.GetSchSourceInfo(eid);

            //ViewBag.SchTypes = dto.SchType;
            var type = "";
            try { type = ((SchoolGrade)SchFType0.Parse(dto.SchType).Grade).Description(); } catch { }
            ViewBag.grade = type;

            return View(dto);
        }
        /// <summary>
        /// 学校纠错-提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("提交学校纠错")]
        public IActionResult AddCorrect([FromBody]ExtraSchCorrect0Dto dto)
        {
            dto.Creator = User.Identity.GetId();
            var b = schCorrectService.Insert(dto);
            return Json(ResponseResult.Success(b));
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadImg()
        {
            try { Directory.CreateDirectory(hostingEnvironment.WebRootPath + @"\imgs\temp"); } catch { }

            var file = Request.Form.Files.FirstOrDefault();
            if (file == null)
            {
                return Json(ResponseResult.Failed("上传文件不能为空"));
            }

            var fid = Request.Form["id"].ToString();
            var index = Convert.ToInt32(Request.Form["index"]);
            var total = Convert.ToInt32(Request.Form["total"]);
            var blockSize = Convert.ToInt32(Request.Form["size"]);
            var ext = Request.Form["ext"];

            if (index == 1 && string.IsNullOrEmpty(fid)) fid = Guid.NewGuid().ToString("n");

            var path = Path.Combine(hostingEnvironment.WebRootPath, $"imgs/temp/{fid}.{ext}");
            using (var steam = file.OpenReadStream())
            {   var fs = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
          
                fs.Seek((index - 1) * blockSize, SeekOrigin.Begin);
                await steam.CopyToAsync(fs);
                await fs.FlushAsync();
                steam.Close();
            }

            if (index < total)
            {
                return Json(ResponseResult.Success(new
                {
                    id = fid
                    //src = 
                }, null));
            }

            using (var fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.Begin);

                var _code = upload_img($"{imgsett.UploadImager}{fid}.{ext}", fs);
                if (_code == 200)
                {
                    return Json(ResponseResult.Success(new
                    {
                        id = fid,
                        src = $"{imgsett.QueryImager.TrimEnd('/')}/images/school_v3/{fid}.{ext}",
                    }, null));
                }
                else
                {
                    return Json(ResponseResult.Failed("上传失败"));
                }
            }

            int upload_img(string url, Stream fs)
            {
                var code = 500;
                var Req = (HttpWebRequest)WebRequest.Create(url);
                Req.Method = "POST";
                Req.Timeout = 30000;
                Req.ContentLength = fs.Length;
                using (var sr_s = Req.GetRequestStream())
                {
                    fs.CopyTo(sr_s);
                    var res = (HttpWebResponse)Req.GetResponse();
                    if (Req.HaveResponse)
                    {
                        using (var st = res.GetResponseStream())
                        {
                            var re = new StreamReader(st);
                            var rez = re.ReadToEnd();
                            re.Close();
                            code = rez == "0" ? 200 : 500;
                        }
                    }
                }
                return code;
            }
        }
    }
}