using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Index = PMS.UserManage.Application.ModelDto.Home.Index;

namespace PMS.UserManage.Application.Services
{
    public class HomeService : IHomeService
    {
        private IAccountService _account;
        private IVerifyService _verify;

        public HomeService(IAccountService account, IVerifyService verify) 
        {
            _account = account;
            _verify = verify;
        }

        public Index GetUserCenterHomeInfo(Guid userID, string cookieStr, string uuid)
        {
            var userInfo = _account.GetUserInfo(userID);
            var countData = new CountData();
            Index info = new Index()
            {
                islogin = true,
                nickname = userInfo.NickName,
                headImgUrl = userInfo.HeadImgUrl,
                mobile = userInfo.Mobile,
                regTime = userInfo.RegTime,
                interest = _account.GetUserInterest(userID, uuid),
                count = _account.GetCountData(userID, cookieStr),
                verify = _verify.GetVerifies(userID).Where(a => a.valid).Select(a => a.verifyType).ToList()
            };
            return info;
        }
    }
}
