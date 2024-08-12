using System;
namespace WeChat.Model
{
    public class UploadTempMediaResponse
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public string Type { get; set; }
        public string MediaId { get; set; }
        public long CreatedAt { get; set; }
    }
}
