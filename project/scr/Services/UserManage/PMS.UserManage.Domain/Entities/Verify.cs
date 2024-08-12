using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    /// <summary>
    /// 用户认证
    /// </summary>
    public class Verify
    {

        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public string RealName { get; set; }
        public byte IdType { get; set; }
        public string IdNumber { get; set; }
        public bool valid { get; set; }
        public DateTime time { get; set; }
        public byte verifyType { get; set; }
        public string intro1 { get; set; }
        public string intro2 { get; set; }
        public string platform { get; set; }
        public string account { get; set; }
    }
}
