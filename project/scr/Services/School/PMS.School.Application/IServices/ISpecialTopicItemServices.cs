using PMS.School.Domain.Entities.SpecialTopic;
using System;
using System.Collections.Generic;

namespace PMS.School.Application.IServices
{
    public interface ISpecialTopicItemServices
    {
        IEnumerable<SpecialTopicItem> Page(int offset, int limit, Guid id, string orderby = default, string asc = "desc");
    }
}
