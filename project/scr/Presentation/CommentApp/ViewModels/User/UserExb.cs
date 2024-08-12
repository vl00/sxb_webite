using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Models.User
{
    public class UserExb
    {
        public Guid Id { get; set; }
        public string HeadImage { get; set; }
        public string UserName { get; set; }
        public int Role { get; set; }
        /// <summary>
        /// 是否为在读生
        /// </summary>
        public bool IsStudent { get; set; }
    }
}
