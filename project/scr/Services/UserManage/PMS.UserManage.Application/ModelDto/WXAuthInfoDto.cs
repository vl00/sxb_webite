using System;
namespace PMS.UserManage.Application.ModelDto
{
    public class WXAuthInfoDto
    {
        public Guid UserId { get; set; }
        public string OpenId { get; set; }
        public string SessionKey { get; set; }
    }
}
