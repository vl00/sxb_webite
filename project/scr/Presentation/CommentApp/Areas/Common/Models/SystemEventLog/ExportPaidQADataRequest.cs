using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models.SystemEventLog
{
    public class ExportPaidQADataRequest: ExportDataToMailRequest
    {
        public DateTime BTime { get; set; } = DateTime.Parse("1753/1/1 12:00");

        public DateTime ETime { get; set; } = DateTime.Now;


    }
}
