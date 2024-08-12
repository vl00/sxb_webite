using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;

namespace Sxb.UserCenter.Controllers
{
    public class MessageController : Base
    {
        IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public IActionResult Index()
        {
            return View();
        }   
        public IActionResult Private(int page = 1)
        {
            return View();//_messageService.GetPrivateMessage(userID, page)
        }
        public IActionResult Follow(int page = 1)
        {
            return View(_messageService.GetFollowMessage(userID, page));
        }
        public IActionResult System(int page = 1)
        {
            return View(_messageService.GetSystemMessage(userID, page));
        }
        public IActionResult Add(Message model)
        {
            model.senderID = userID;
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (!_messageService.AddMessage(model))
            {
                json.status = 1;
                json.errorDescription = "";
            }
            return Json(json);
        }
        public IActionResult Remove(Guid msgID)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (!_messageService.RemoveMessage(msgID, userID))
            {
                json.status = 1;
                json.errorDescription = "删除失败";
            }
            return Json(json);
        }
        public IActionResult PushSetting()
        {
            return View(_messageService.GetPushSetting(userID));
        }
        [HttpPost]
        public IActionResult PushSetting(Push model)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            model.UserID = userID;
            if (!_messageService.SetPushSetting(model))
            {
                json.status = 1;
                json.errorDescription = "设置失败";
            }
            return Json(json);
        }
        public IActionResult PushHistory()
        {
            return View();
        }
        public IActionResult PushArticle()
        {
            return View();
        }
        public IActionResult PushSchool()
        {
            return View();
        }
        public IActionResult PushInvite()
        {
            return View();
        }
        public IActionResult PushReply()
        {
            return View();
        }
    }
}