using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.Account
{
    public class BindInfo: RootModel
    {
        public string Mobile { get; set; }
        public string UnionID_Weixin { get; set; }
        public Guid? UnionID_QQ { get; set; }
    }
}
