using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.FeedBack
{
    public class FeedbackCorrectedVo
    {
        public List<string> ImagesArr { get; set; }
        public CorrectedType Type { get; set; }
        public string Content { get; set; }
        public Guid Eid { get; set; }
        public string Remark { get; set; }
    }
}
