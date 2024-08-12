using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Services
{
    public interface ICircleCoverService : IApplicationService<CircleCover>
    {
        IEnumerable<CircleCover> GetCover(IEnumerable<Guid> circleIds);
    }
}
