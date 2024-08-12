using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.SpecialTopic;
using PMS.School.Domain.IRespository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class SpecialTopicItemServices : ISpecialTopicItemServices
    {
        ISpecialTopicItemRespository _currentRespository;
        public SpecialTopicItemServices(ISpecialTopicItemRespository specialTopicItemRespository)
        {
            _currentRespository = specialTopicItemRespository;
        }

        IEnumerable<SpecialTopicItem> ISpecialTopicItemServices.Page(int offset, int limit, Guid id, string orderby, string asc)
        {
            return _currentRespository.Page(offset, limit, id, orderby, asc);
        }
    }
}