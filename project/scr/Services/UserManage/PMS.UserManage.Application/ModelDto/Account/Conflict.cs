using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.Account
{
    public class Conflict : RootModel
    {
        public UserInfo myInfo { get; set; }
        public List<UserInfo> list { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 0：手机号；1：微信，2：QQ
        /// </summary>
        public byte dataType { get; set; }
    }
}
