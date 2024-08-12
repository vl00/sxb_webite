using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Auth
{
    public class UserGrantAuthService : IUserGrantAuthService
    {
        private IUserGrantAuthRepository _userGrantAuthRepository;

        public UserGrantAuthService(IUserGrantAuthRepository userGrantAuthRepository) 
        {
            _userGrantAuthRepository = userGrantAuthRepository;
        }

        public bool Add(UserGrantAuth userGrant)
        {
            return _userGrantAuthRepository.Add(userGrant);
        }

        public bool IsGrantAuth(Guid AdminId)
        {
            return _userGrantAuthRepository.IsGrantAuth(AdminId);
        }
    }
}
