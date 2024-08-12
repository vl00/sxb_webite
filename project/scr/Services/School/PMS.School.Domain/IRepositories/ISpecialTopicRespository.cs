using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.SpecialTopic;
using PMS.School.Domain.Enum;
using System;
using System.Collections.Generic;

namespace PMS.School.Domain.IRespository
{
    public interface ISpecialTopicRespository
    {
        IEnumerable<SpecialTopic> Page(int offset, int limit, string city, SpecialTopicType type, string orderby = default, string asc = "desc");
        int GetAllDataCount(string where);

        /// <summary>
        /// 获取直播专题所有达人
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <param name="limit">行数</param>
        /// <returns></returns>
        IEnumerable<SpecialTopicUserDto> GetLiveTopicUsers(Guid id, int limit);

        /// <summary>
        /// 根据ID获取单条数据
        /// </summary>
        /// <param name="id">专题ID</param>
        /// <returns></returns>
        SpecialTopic GetByID(Guid id);
    }
}