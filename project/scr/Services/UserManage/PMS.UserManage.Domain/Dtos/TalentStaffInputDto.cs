using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Dtos
{
    public class TalentStaffInputDto: TalentStaff
    {
        //如果返回“检查到运营管理平台的达人列表-员工管理中删除过该用户”，可以添加参数IsAdd:true，确认要提交
        public bool IsAdd { get; set; } = false;
    }
}
