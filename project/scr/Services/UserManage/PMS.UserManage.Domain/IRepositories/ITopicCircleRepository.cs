using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ITopicCircleRepository
    {
        /// <summary>
        /// 获取达人的圈子
        /// </summary>
        /// <returns></returns>
        IEnumerable<Guid> GetCircle(Guid creator);

        /// <summary>
        /// 增加或更新关注圈子
        /// </summary>
        /// <param name="circleFollowers"></param>
        /// <returns></returns>
        bool AddOrUpdateCircleFollower(IEnumerable<CircleFollower> circleFollowers);
    }
}
