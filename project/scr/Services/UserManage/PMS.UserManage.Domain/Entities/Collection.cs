using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public  class Collection
    {
        public Guid dataID { get; set; }
        public int dataType { get; set; }
        public Guid userID { get; set; }
        public DateTime? time { get; set; }


    }
}
