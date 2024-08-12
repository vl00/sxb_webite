using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class ToppingRepository : Repository<Topping, TopicCircleDBContext>, IToppingRepository
    {
        public ToppingRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
        }
    }
}
