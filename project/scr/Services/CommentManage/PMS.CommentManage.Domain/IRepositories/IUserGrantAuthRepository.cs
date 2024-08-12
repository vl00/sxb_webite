using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface IUserGrantAuthRepository
    {
        bool IsGrantAuth(Guid AdminId);

        bool Add(UserGrantAuth userGrant);
    }
}
