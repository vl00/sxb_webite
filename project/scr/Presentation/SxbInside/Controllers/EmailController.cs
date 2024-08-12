using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Framework.Foundation;
using ProductManagement.Tool.Email;
using Sxb.Inside.RequestModel;
using Sxb.Inside.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class EmailController
    {
        private readonly IEmailClient _emailClient;
        public EmailController(IEmailClient emailClient)
        {
            _emailClient = emailClient;
        }
        // GET: api/values
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ResponseResult> SendEmail(SendEmailData data)
        {
            try
            {
                if (data.Attachments != null && data.Attachments.Any())
                {
                    List<object> atts = new List<object>();
                    foreach (var file in data.Attachments)
                    {
                        //var file = atta;
                        var fileName = file.FileName;
                        var contentType = file.ContentType;
                        var att = new Attachment(file.OpenReadStream(), fileName);
                        atts.Add(att);
                    }
                    await _emailClient.NotifyByMailAsync(data.Subject, data.Body, data.ToAddr, atts, data.CC);
                }
                else
                {
                    await _emailClient.NotifyByMailAsync(data.Subject, data.Body, data.ToAddr, null, data.CC);
                }

                return ResponseResult.Success("发送成功");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

    }
}
