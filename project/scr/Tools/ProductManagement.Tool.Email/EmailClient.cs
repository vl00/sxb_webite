using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ProductManagement.Tool.Email
{
    public class EmailClient: IEmailClient
    {

        private readonly MailNotificationConfiguration _notificationConfig;
        private readonly ILogger<EmailClient> _logger;

        public EmailClient(IOptionsSnapshot<MailNotificationConfiguration> options, ILogger<EmailClient> logger)
        {
            _notificationConfig = options.Value;
            _logger = logger;
        }

        public string MakeEmailBody(string name, params string[] contentLines)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyByMailAsync(string subject, string body, string[] to, List<object> atts = null, string[] cc = null)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(_notificationConfig.EmailAccount, _notificationConfig.EmailDisplayName),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                Subject = subject,
                Body = body
            };
            if (null != atts)
            {
                foreach (var att in atts)
                {
                    var inMailAttachment = att as Attachment;

                    if (inMailAttachment != null)
                    {
                        mail.Attachments.Add(inMailAttachment);
                    }
                }
            }
          
            to.ToList().ForEach(n => mail.To.Add(n));

            cc?.ToList().ForEach(ccAddress => mail.CC.Add(ccAddress));

            try
            {
                using var client = new SmtpClient
                {
                    Host = _notificationConfig.SMTPService, //mail server host
                    Port = _notificationConfig.SMTPPort,
                    Credentials = new NetworkCredential(_notificationConfig.EmailAccount, _notificationConfig.Password),//用户名和密码
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };


                if (mail.To.Count() > 0)
                    await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"发送邮件异常");
            }
            finally
            {
                if (mail.Attachments != null)
                    mail.Attachments.Dispose();
                mail.Dispose();
            }
        }
        //private bool IsProductEnvironment()
        //{
        //    return _env == "Production" || _env == "Stage";
        //}
    }
}
