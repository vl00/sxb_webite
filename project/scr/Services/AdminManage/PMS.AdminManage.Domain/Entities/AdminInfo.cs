using System;
namespace PMS.AdminManage.Domain.Entities
{
    public class AdminInfo
    {
        public Guid Id { get; set; }

        public string NickName { get; set; }

        public string Phone { get; set; }

        public string HeadImager { get; set; }

        public int Role { get; set; }
    }
}
