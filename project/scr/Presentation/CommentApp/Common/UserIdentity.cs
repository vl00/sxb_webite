using System;

namespace Sxb.Web.Common
{
    public class UserIdentity
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public int[] Role { get; set; }
        public bool IsAuth { get; set; }
    }
}
