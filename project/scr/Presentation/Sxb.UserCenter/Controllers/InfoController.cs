using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Foundation;

namespace Sxb.UserCenter.Controllers
{
    public class InfoController : Base
    {
        public IsHost IsHost { get; }
        protected IAccountService _account;
        public InfoController(IAccountService account, IOptions<IsHost> _isHost)
        {
            IsHost = _isHost.Value;
            _account = account;
        }

        public IActionResult Index()
        {
            string uuid = Request.Cookies["uuid"];
            PMS.UserManage.Application.ModelDto.Info.Index model = new PMS.UserManage.Application.ModelDto.Info.Index();
            model.UserInfo = _account.GetUserInfo(userID);
            model.Interest = _account.GetUserInterest(userID, uuid);
            return iSchoolResult(model);
        }
        public IActionResult UpdateNickname(string nickname)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            var info = _account.GetUserInfo(userID);
            info.NickName = nickname;
            if (!_account.UpdateUserInfo(info))
            {
                json.status = 1;
                json.errorDescription = "更新失败";
            }
            return Json(json);
        }
        public IActionResult UploadHeadImg(List<IFormFile> files)
        {
            Stream fileStream;
            string ext;
            
            var info = _account.GetUserInfo(userID);
            FileUploadResponseModel result = new FileUploadResponseModel();
            if (files.Count > 0)
            {
                fileStream = files[0].OpenReadStream();
                ext = Path.GetExtension(files[0].FileName);
            }
            else if (Request.Form.Files.Count > 0)
            {
                fileStream = Request.Form.Files[0].OpenReadStream();
                ext = Path.GetExtension(Request.Form.Files[0].FileName);
            }
            else
            {
                result.status = 1;
                result.errorDescription = "没有文件";
                return Json(result);
            }
            result = HttpHelper.HttpPost<FileUploadResponseModel>($"{IsHost.FileUploadHost}/Upload/User?path={userID}&filename={Guid.NewGuid()}{ext}"
                , fileStream);
            info.HeadImgUrl = result.compress.cdnUrl;
            //if (!_account.UpdateUserInfo(info))
            //{
            //    return Json(new PMS.UserManage.Domain.Common.RootModel() { status = 1, errorDescription = "提交失败" });
            //}
            return Json(result);
        }
        [AllowAnonymous]
        public IActionResult SetUserInterest(Interest interest)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            string uuid = Request.Cookies["uuid"];
            if (!string.IsNullOrEmpty(uuid))
            {
                interest.UuID = Guid.Parse(MD5Helper.GetMD5(uuid));
            }
            interest.UserID = userID == Guid.Empty ? (Guid?)null : userID;
            if(interest.UuID==null && interest.UserID == null)
            {
                json.status = 1;
                json.errorDescription = "缺少标识";
                return Json(json);
            }
            if (!_account.SetUserInterest(interest, uuid))
            {
                json.status = 1;
                json.errorDescription = "修改失败";
            }
            return Json(json);
        }
        [AllowAnonymous]
        public IActionResult GetUserInterest(Guid? userID, string uuid)
        {
            userID = userID ?? base.userID;
            uuid = uuid ?? Request.Cookies["uuid"];
            PMS.UserManage.Application.ModelDto.Info.ApiInterest json = new PMS.UserManage.Application.ModelDto.Info.ApiInterest();
            if (userID.Value.Equals(Guid.Empty) && string.IsNullOrEmpty(uuid))
            {
                json.status = 1;
                json.errorDescription = "缺少标识";
                return Json(json);
            }
            return Json(_account.GetApiInterest(userID, uuid));
        }
        [AllowAnonymous]
        public IActionResult GetInterest()
        {
            string uuid = Request.Cookies["uuid"];
            PMS.UserManage.Application.ModelDto.Info.ApiInterest json = new PMS.UserManage.Application.ModelDto.Info.ApiInterest();
            if (userID.Equals(Guid.Empty) && string.IsNullOrEmpty(uuid))
            {
                json.status = 1;
                json.errorDescription = "缺少标识";
                return Json(json);
            }
            return Json(_account.GetUserInterest(userID, uuid));
        }
        public IActionResult Interest()
        {
            string uuid = Request.Cookies["uuid"];
            return View(_account.GetUserInterest(userID, uuid));
        }
    }
}