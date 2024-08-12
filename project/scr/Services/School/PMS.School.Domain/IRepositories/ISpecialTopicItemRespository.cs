using PMS.School.Domain.Entities.SpecialTopic;
using System;
using System.Collections.Generic;

namespace PMS.School.Domain.IRespository
{
    public interface ISpecialTopicItemRespository
    {
        IEnumerable<SpecialTopicItem> Page(int offset, int limit, Guid id, string orderby = default, string asc = "desc");
    }
}