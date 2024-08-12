using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.SpecialTopic;
using PMS.School.Domain.Enum;
using PMS.School.Domain.IRespository;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;

namespace PMS.School.Repository.Repositories
{
    public class SpecialTopicRespository : ISpecialTopicRespository

    {
        private ISchoolDataDBContext _DB;

        public SpecialTopicRespository(ISchoolDataDBContext context)
        {
            _DB = context;
        }

        public int GetAllDataCount(string where)
        {
            return _DB.ExecuteScalar<int>($"Select Count(ID) From SpecialTopic {where};", new { });
        }

        public SpecialTopic GetByID(Guid id)
        {
            return _DB.Get<SpecialTopic, Guid>(id);
        }

        public IEnumerable<SpecialTopicUserDto> GetLiveTopicUsers(Guid id, int limit)
        {
            var str_SQL = $@"SELECT
                                Top {limit}
	                            sti.TargetUserName as UserName,
	                            sti.TargetUserID as ID,
	                            sti.TargetUserImgUrl as ImgUrl
                            FROM
	                            SpecialTopicItem as sti
                            LEFT JOIN 
                                iSchoolLive.dbo.lecture_v2 as l on l.ID = sti.TargetID
                            WHERE
	                            SpecialTopicID = @id and l.show = 1
                            Order by
                                [Index] asc";
            return _DB.Query<SpecialTopicUserDto>(str_SQL, new { id });
        }

        public IEnumerable<SpecialTopic> Page(int offset, int limit, string city, SpecialTopicType type, string orderby = null, string asc = "desc")
        {
            if (string.IsNullOrWhiteSpace(orderby)) orderby = "ID";
            var str_Where = string.Empty;
            if (type > SpecialTopicType.Unknow)
            {
                str_Where += " AND Type = @type";
            }
            if (!string.IsNullOrWhiteSpace(city) && city != "0")
            {
                str_Where += $" AND CityCode in (@city,0)";
            }
            if (limit < 1) limit = 10;
            var str_SQL = $@"SELECT
	                            * 
                            FROM
	                            SpecialTopic 
                            WHERE
                                1 = 1
                                {str_Where}
                            ORDER BY
	                            case WHEN CityCode = {city} then 0 else 1 end,
                                [{orderby}] {asc} OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";
            return _DB.Query<SpecialTopic>(str_SQL, new { city, offset, limit, type });
        }
    }
}