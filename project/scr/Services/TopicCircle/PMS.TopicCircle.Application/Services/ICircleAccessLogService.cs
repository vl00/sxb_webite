using PMS.TopicCircle.Domain.Entities;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public interface ICircleAccessLogService:IApplicationService<CircleAccessLog>
    {


        /// <summary>
        /// 记录访问圈子时间，目前暂时使用锁来解决并发引起的逻辑错误问题，后续可使用异步队列方式解决。
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool Record(Circle circle, UserInfo user);

        CircleAccessLog GetLatest(Guid circleId, Guid userId);

    }
}
