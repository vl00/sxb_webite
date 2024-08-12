using PMS.UserManage.Application.ModelDto.Account;

namespace Sxb.UserCenter.Models.AccountViewModel
{
    public class AccountBind
    {
        /// <summary>
        /// 绑定电话
        /// </summary>
        public bool IsBindPhone { get; set; }
        /// <summary>
        /// 绑定微信
        /// </summary>
        public bool IsBindWX { get; set; }
        /// <summary>
        /// 绑定qq
        /// </summary>
        public bool IsBindQQ { get; set; }

        public BindInfo BindInfo { get; set; }

        /// <summary>
        /// 第三方帐号绑定结果
        /// </summary>
        public bool? IsBindSuccess { get; set; }
        public System.Guid UserID { get; set; }
    }
}
