using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.RequestModel
{
    public class UserMessage
    {
        /*
         userID（受邀人）
        type
        title（消息标题）
        content（消息内容）
        dataID（跳转到详情页的ID）
        dataType（dataID类型）
        eID（学部ID）
        */
        public Guid userID { get; set; }
        public int type { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public Guid dataID { get; set; }
        public int dataType { get; set; }
        public Guid eID { get; set; }
        public bool IsAnony { get; set; }
    }
}
