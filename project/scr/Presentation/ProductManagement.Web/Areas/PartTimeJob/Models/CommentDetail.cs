using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 后台点评详情
    /// </summary>
    public class CommentDetail
    {
        public string commentUser { get; set; }
        public SchoolCommentDto schoolComment { get; set; }
        public List<SchoolTag>  tags { get; set; }
        public List<SchoolImage> schoolImages { get; set; }

        public string population { get; set; }
        public string teach { get; set; }
        public string hard { get; set; }
        public string envir { get; set; }
        public string manage { get; set; }
        public string life { get; set; }
    }
}
