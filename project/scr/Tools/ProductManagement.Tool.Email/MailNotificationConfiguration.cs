using System;
namespace ProductManagement.Tool.Email
{
    public class MailNotificationConfiguration
    {
        public string SMTPService { get; set; }
        public int SMTPPort { get; set; } = 25;
        public string EmailAccount { get; set; }
        public string EmailDisplayName { get; set; }
        public string Password { get; set; }
        public string[] EmailWhiteList { get; set; }
    }
}
