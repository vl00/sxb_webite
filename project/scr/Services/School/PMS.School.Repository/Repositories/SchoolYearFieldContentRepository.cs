using iSchool.Domain;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using PMS.School.Domain.IRepositories;
using PMS.School.Infrastructure;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.School.Repository
{
    public class SchoolYearFieldContentRepository : ISchoolYearFieldContentRepository
    {
        private readonly ISchoolDataDBContext dbc;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly string fields;
        public SchoolYearFieldContentRepository(ISchoolDataDBContext dbc, IEasyRedisClient easyRedisClient)
        {
            this.dbc = dbc;
            _easyRedisClient = easyRedisClient;
            fields = @"    [Id]
                          ,[year]
                          ,[eid]
                          ,[field]
                          ,[content]
                          ,[IsValid] ";
        }
        /// <summary>
        /// 获取最近一年的值
        /// </summary>
        /// <param name="eid">部id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        public OnlineSchoolYearFieldContent GenRecentContent(string eid, string field)
        {
            var sql = $"select top 1 {fields} from OnlineSchoolYearFieldContent where eid=@eid and field=@field and IsValid=1 order by Year desc";
            return dbc.Query<OnlineSchoolYearFieldContent>(sql, new { eid, field }).FirstOrDefault();
        }

        /// <summary>
        /// 获取所有年份的值
        /// </summary>
        /// <param name="eid">部id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        public List<OnlineSchoolYearFieldContent> GenAllContent(string eid)
        {
            var sql = $"select {fields} from OnlineSchoolYearFieldContent where eid=@eid and IsValid=1 order by Year desc";
            return dbc.Query<OnlineSchoolYearFieldContent>(sql, new { eid }).ToList();
        }

        /// <summary>
        /// 获取字段所有年份的值
        /// </summary>
        /// <param name="eid">部id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        public List<OnlineSchoolYearFieldContent> GenAllFieldContent(string eid, string field)
        {
            var sql = $"select {fields} from OnlineSchoolYearFieldContent where eid=@eid and field=@field and IsValid=1 order by Year desc";
            return dbc.Query<OnlineSchoolYearFieldContent>(sql, new { eid, field }).ToList();
        }

        public List<string> GetAllYears(string eid, string field)
        {
            var str_SQL = $"Select years from OnlineYearExtField WHERE field = @field and eid = @eid;";
            var find = dbc.Query<string>(str_SQL, new { field, eid });
            if (find == null || !find.Any())
            {
                return new List<string>();
            }
            var result = find.First().Split(',').OrderByDescending(p => p).ToList();
            return result;
        }

        public List<OnlineSchoolYearFieldContent> GenFieldContent(string eid, string field, string year)
        {
            var str_SQL = $"Select {fields} From OnlineSchoolYearFieldContent_{year} where eid=@eid and field=@field and IsValid=1 order by Year desc";
            return dbc.Query<OnlineSchoolYearFieldContent>(str_SQL, new { eid, field }).ToList();
        }
    }
}
