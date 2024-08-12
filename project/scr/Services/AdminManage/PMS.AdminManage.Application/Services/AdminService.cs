using System;
using PMS.AdminManage.Application.IServices;
using PMS.AdminManage.Domain.IRepositories;

namespace PMS.AdminManage.Application.Services
{
    public class AdminService: IAdminService
    {

        private readonly IAdminRepository _userRepository;
        public AdminService(IAdminRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool SyncUserInfo(Guid UserId, string userName, string phone, int role, string headImg)
        {
            var user = _userRepository.GetUserInfo(UserId);

            bool result;
            if (user == null)
            {
                result = _userRepository.AddUserInfo(new Domain.Entities.AdminInfo
                {
                    Id = UserId,
                    NickName = userName,
                    Phone = phone,
                    Role = 5,
                    HeadImager = headImg
                });
            }
            else
            {
                result = _userRepository.UpdateUserInfo(new Domain.Entities.AdminInfo
                {
                    Id = UserId,
                    NickName = userName,
                    Phone = phone,
                    Role = 5,
                    HeadImager = headImg
                });
            }
            return result;
        }
    }
}
