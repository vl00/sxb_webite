﻿using System;
namespace PMS.UserManage.Domain.Entities
{
    public class OpenIdWeixin
    {
        public string OpenId { get; set; }
        public Guid UserId { get; set; }
        public string AppName { get; set; }
        public string SessionKey { get; set; }
        public bool Valid { get; set; }
        public DateTime CreatTime { get; set; }
        public string AppId { get; set; }
    }
}
