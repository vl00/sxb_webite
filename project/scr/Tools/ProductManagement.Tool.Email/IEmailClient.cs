using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagement.Tool.Email
{
    public interface IEmailClient
    {
        Task NotifyByMailAsync(string subject, string body, string[] to, List<object> att = null, string[] cc = null);

        string MakeEmailBody(string name, params string[] contentLines);
    }
}
