using ProductManagement.Tool.HttpRequest.Option;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Option.File
{
    public class UploadImgOption : BaseOption
    {
        private string type;

        private string filename;

        public UploadImgOption(string type,string filename)
        {
            this.type = type;
            this.filename = filename;

        }
        public override string UrlPath => $"/upload/{type}?filename={filename}";
    }
}
