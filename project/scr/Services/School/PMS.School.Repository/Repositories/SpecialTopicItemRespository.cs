using PMS.School.Domain.Entities.SpecialTopic;
using PMS.School.Domain.IRespository;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;

namespace PMS.School.Repository.Repositories
{
    public class SpecialTopicItemRespository : ISpecialTopicItemRespository

    {
        private ISchoolDataDBContext _DB;

        public SpecialTopicItemRespository(ISchoolDataDBContext context)
        {
            _DB = context;
        }

        public IEnumerable<SpecialTopicItem> Page(int offset, int limit, Guid id, string orderby = null, string asc = "desc")
        {
            if (string.IsNullOrWhiteSpace(orderby)) orderby = "ID";
            var str_SQL = $@"SELECT
	                            * 
                            FROM
	                            SpecialTopicItem 
                            WHERE
                                SpecialTopicID = @id
                            ORDER BY
	                            [{orderby}] {asc} OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";
            return _DB.Query<SpecialTopicItem>(str_SQL, new { id, offset, limit });
        }
    }
}