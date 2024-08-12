using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public class CircleAccessLogService : ApplicationService<CircleAccessLog>, ICircleAccessLogService
    {
        private readonly static object _lock = new object();
        ICircleAccessLogRepository _repository;
        public CircleAccessLogService(ICircleAccessLogRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public CircleAccessLog GetLatest(Guid circleId, Guid userId)
        {
            CircleAccessLog circleAccessLog = this._repository.GetBy(" CIRCLEID=@circleId AND USERID=@userId ", new
            {
                circleId = circleId,
                userId = userId
            }, "createtime desc").FirstOrDefault();
            return circleAccessLog;
        }

        public  bool Record(Circle circle, UserInfo user)
        {
            lock (_lock) {
                CircleAccessLog circleAccessLog = GetLatest(circle.Id, user.Id);
                if (circleAccessLog != null)
                {
                    //修改log时间
                    circleAccessLog.CreateTime = DateTime.Now;
                    return  _repository.UpdateAsync(circleAccessLog, null, new[] { "createtime" }).GetAwaiter().GetResult();
                }
                else
                {
                    //插入log记录
                    circleAccessLog = new CircleAccessLog()
                    {
                        CircleId = circle.Id,
                        UserId = user.Id,
                        CreateTime = DateTime.Now
                    };
                    return  _repository.AddAsync(circleAccessLog).GetAwaiter().GetResult();
                }
            }
        }


    }
}
