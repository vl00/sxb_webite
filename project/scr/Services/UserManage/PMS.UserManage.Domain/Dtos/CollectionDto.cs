using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class CollectionDto
    {
        public Guid DataID { get; set; }
        public int DataType { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? Time { get; set; }
    }
}
