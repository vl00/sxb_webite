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
        /// ��ȡֱ��ר�����д���
        /// </summary>
        /// <param name="id">ר��ID</param>
        /// <param name="limit">����</param>
        /// <returns></returns>
        IEnumerable<SpecialTopicUserDto> GetLiveTopicUsers(Guid id, int limit);

        /// <summary>
        /// ����ID��ȡ��������
        /// </summary>
        /// <param name="id">ר��ID</param>
        /// <returns></returns>
        SpecialTopic GetByID(Guid id);
    }
}