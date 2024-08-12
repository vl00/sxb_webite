using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.File
{
    public class UploadImgResponse
    {

        public bool successs
        {
            get
            {
                return status == 0;
            }
        }
        public int status { get; set; }
        public string cdnUrl { get; set; }

        public string Url { get; set; }

        public UploadImgCompressResponse Compress { get; set; }
    }

    public class UploadImgCompressResponse
    {
        public int Status { get; set; }
        public string CdnUrl { get; set; }
        public string Url { get; set; }
    }
}
