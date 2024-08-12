using System;
namespace PMS.UserManage.Application.ModelDto
{
    public class BDAuthInfoDto
    {
        public Guid UserId { get; set; }
        public string OpenId { get; set; }
        public string AccessKey { get; set; }
    }
}
