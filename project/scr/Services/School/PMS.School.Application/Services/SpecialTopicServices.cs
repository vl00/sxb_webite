using Org.BouncyCastle.Asn1.Cmp;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.SpecialTopic;
using PMS.School.Domain.Enum;
using PMS.School.Domain.IRespository;
using PMS.UserManage.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMS.School.Application.Services
{
    public class SpecialTopicServices : ISpecialTopicServices
    {
        ISpecialTopicRespository _currentRespository;
        IUserService _userService;
        ITalentService _talentService;
        ICollectionService _collectionService;

        public SpecialTopicServices(ISpecialTopicRespository specialTopicRespository, IUserService userService, ITalentService talentService,
            ICollectionService collectionService)
        {
            _collectionService = collectionService;
            _talentService = talentService;
            _userService = userService;
            _currentRespository = specialTopicRespository;
        }

        public SpecialTopic Get(Guid id)
        {
            return _currentRespository.GetByID(id);
        }

        public IEnumerable<SpecialTopicUserDto> GetLiveTopicUsers(Guid id, int limit)
        {
            var finds = _currentRespository.GetLiveTopicUsers(id, limit);
            if (finds?.Any() == true)
            {
                var userIDs = finds.Select(p => p.ID).Distinct();
                var talents = _userService.GetTalentDetails(userIDs);

                foreach (var item in finds)
                {
                    item.Title = talents.FirstOrDefault(p => p.Id == item.ID)?.Certification_title;
                    item.Type = talents.FirstOrDefault(p => p.Id == item.ID)?.Role ?? 0;
                    item.FollowCount = _collectionService.GetUserFollowFans(item.ID)?.Fans ?? 0 + _collectionService.GetUserFollowFans(item.ID)?.Follow ?? 0;
                }
            }
            return finds.OrderByDescending(p => p.FollowCount).Distinct(new SpecialTopicUserDtoIDComparer());
        }
        class SpecialTopicUserDtoIDComparer : IEqualityComparer<SpecialTopicUserDto>
        {
            public bool Equals(SpecialTopicUserDto x, SpecialTopicUserDto y)
            {
                if (x == null)
                    return y == null;
                return x.ID == y.ID;
            }

            public int GetHashCode(SpecialTopicUserDto obj)
            {
                if (obj == null)
                    return 0;
                return obj.ID.GetHashCode();
            }
        }

        public (IEnumerable<SpecialTopic>, int total) Page(int offset, int limit, string city, SpecialTopicType type, string orderby = null, string asc = "desc")
        {
            if (limit < 1)
            {
                return (null, 0);
            }
            return (_currentRespository.Page(offset, limit, city, type, orderby, asc),
                _currentRespository.GetAllDataCount($"where cityCode in (0{(!string.IsNullOrWhiteSpace(city) ? $",{city}" : "")})"));
        }
    }
}