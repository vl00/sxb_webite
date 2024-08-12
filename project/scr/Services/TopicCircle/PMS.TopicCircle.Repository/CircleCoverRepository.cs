using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Repository
{
    public class CircleCoverRepository : Repository<CircleCover,TopicCircleDBContext>, ICircleCoverRepository
    {
        public CircleCoverRepository(TopicCircleDBContext dBContext) : base(dBContext)
        {
        }
    }
}
