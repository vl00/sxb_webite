using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class Interest : RootModel
    {
        public Interest() { }
        public Interest(bool allTrue)
        {
            Grade_1 = true;
            Grade_2 = true;
            Grade_3 = true;
            Grade_4 = true;
            Nature_1 = true;
            Nature_2 = true;
            Nature_3 = true;
            Nature_4 = true;
            Lodging_0 = true;
            Lodging_1 = true;
        }
        public Guid? UuID { get; set; }
        public Guid? UserID { get; set; }
        public bool Grade_1 { get; set; }
        public bool Grade_2 { get; set; }
        public bool Grade_3 { get; set; }
        public bool Grade_4 { get; set; }
        public bool Nature_1 { get; set; }
        public bool Nature_2 { get; set; }
        public bool Nature_3 { get; set; }
        public bool Nature_4 { get; set; }
        public bool Lodging_0 { get; set; }
        public bool Lodging_1 { get; set; }
    }
}
