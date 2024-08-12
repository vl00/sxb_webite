using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IUserGrantAuthService
    {
        bool IsGrantAuth(Guid AdminId);

        bool Add(UserGrantAuth userGrant);
    }
}
