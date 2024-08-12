using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.Verify;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace PMS.UserManage.Application.Services
{
    public class VerifyService : IVerifyService
    {
        private IVerifyRepository _verify;
        private IAccountService _account;
        public VerifyService(IVerifyRepository verify, IAccountService account)
        {
            _verify = verify;
            _account = account;
        }

        public List<Verify> GetVerifies(Guid userID)
        {
            return _verify.GetVerifies(userID);
        }
        public Senior CheckSeniorCondition(Guid userID, string cookieStr)
        {
            var countData = _account.GetCountData(userID, cookieStr);
            return new Senior()
            {
                isBindMobile = _account.IsBindPhone(userID),
                isPublishReach = countData.publish > 5,
                isReplyReach = countData.reply > 10
            };
        }
        public Official CheckOfficialCondition(Guid userID, string cookieStr)
        {
            var countData = _account.GetCountData(userID, cookieStr);
            return new Official()
            {
                isBindMobile = _account.IsBindPhone(userID)
            };
        }
        public bool SubmitVerify(Verify verifyInfo)
        {
            verifyInfo.Id = Guid.NewGuid();
            verifyInfo.time = DateTime.Now;
            return _verify.SubmitVerify(verifyInfo);
        }

        public bool IsClearHeadImg(Guid userID, string url)
        {
            var img = Image.FromFile(url);
            var pItems = img.PropertyItems;//将"其他"信息过滤掉
            return true;
        }
    }
}
