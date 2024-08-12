using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.Common
{
    public class FileUploadResponseModel : RootModel
    {
        public string url { get; set; }
        public string cdnUrl { get; set; }
        public FileUploadResponseModel compress { get; set; }
    }
}
