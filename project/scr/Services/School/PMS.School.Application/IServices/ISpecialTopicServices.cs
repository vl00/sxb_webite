using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.SpecialTopic;
using PMS.School.Domain.Enum;
using System;
using System.Collections.Generic;

namespace PMS.School.Application.IServices
{
    public interface ISpecialTopicServices
    {
        (IEnumerable<SpecialTopic>, int total) Page(int offset, int limit, string city, SpecialTopicType type, string orderby = default, string asc = "desc");

        /// <summary>
        /// 获取直播专题的所有达人
        /// </summary>
        /// <param name="id">直播专题ID</param>
        /// <param name="limit">行数</param>
        /// <returns></returns>
        IEnumerable<SpecialTopicUserDto> GetLiveTopicUsers(Guid id, int limit);

        /// <summary>
        /// 获取单个
        /// </summary>
        SpecialTopic Get(Guid id);
    }
}
