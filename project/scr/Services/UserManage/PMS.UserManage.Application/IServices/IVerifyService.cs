using PMS.UserManage.Application.ModelDto.Verify;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.IServices
{
    public interface IVerifyService
    {
        bool SubmitVerify(Verify verifyInfo);
        Official CheckOfficialCondition(Guid userID, string cookieStr);
        Senior CheckSeniorCondition(Guid userID, string cookieStr);
        List<Verify> GetVerifies(Guid userID);
        bool IsClearHeadImg(Guid userID, string url);
    }
}
