using System;

namespace PMS.AspNetCoreHelper
{
    public class UserIdentity
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public int[] Role { get; set; }
        public bool IsAuth { get; set; }
    }
}