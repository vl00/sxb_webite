using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IInterestRepository
    {
        bool SetUserInterest(Interest interest);
        bool SetDeviceInterest(Interest interest);
        bool UserInterestExists(Guid userID);
        Interest GetUserInterest(Guid? userID, Guid? uuID);
        List<InterestItem> GetInterestColumns();
    }
}
