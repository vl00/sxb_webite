using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IVerifyRepository
    {
        List<Verify> GetVerifies(Guid userID);
        bool SubmitVerify(Verify verifyInfo);
    }
}
