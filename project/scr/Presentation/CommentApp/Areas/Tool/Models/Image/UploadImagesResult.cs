using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Tool.Models.Image
{
    public class UploadImagesResult
    {

        public int Index { get; set; }

        public string Url { get; set; }

        public string CdnUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool Success { get; set; }

    }
}
