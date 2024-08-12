using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class Push
    {
        public Push() { }
        public Push(bool initStatus)
        {
            this.Article = initStatus;
            this.School = initStatus;
            this.Invite = initStatus;
            this.Reply = initStatus;
        }
        public Guid UserID { get; set; }
        public bool Article { get; set; }
        public bool School { get; set; }
        public bool Invite { get; set; }
        public bool Reply { get; set; }
    }
}
