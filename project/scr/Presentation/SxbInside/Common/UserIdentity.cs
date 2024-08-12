using System;

namespace Sxb.Inside.Common
{
    public class UserIdentity
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public int[] Role { get; set; }
    }
}
