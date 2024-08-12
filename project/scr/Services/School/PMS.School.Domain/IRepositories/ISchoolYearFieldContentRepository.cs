using iSchool.Domain;
using System.Collections.Generic;

namespace PMS.School.Domain.IRepositories
{
    public interface ISchoolYearFieldContentRepository
    {
        /// <summary>
        /// 获取最近一年的值
        /// </summary>
        /// <param name="eid">部id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        OnlineSchoolYearFieldContent GenRecentContent(string eid, string field);

        /// <summary>
        /// 获取所有年份的值
        /// </summary>
        /// <param name="eid">部id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        List<OnlineSchoolYearFieldContent> GenAllContent(string eid);

        /// <summary>
        /// 获取字段所有年份的值
        /// </summary>
        /// <param name="eid">部id</param>
        /// <param name="field">字段名</param>
        /// <returns></returns>
        List<OnlineSchoolYearFieldContent> GenAllFieldContent(string eid, string field);
        /// <summary>
        /// 获取字段指定年份的值
        /// </summary>
        /// <param name="eid">分部ID</param>
        /// <param name="field">字段名</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        List<OnlineSchoolYearFieldContent> GenFieldContent(string eid, string field, string year);

        /// <summary>
        /// 获取指定字段的年份
        /// </summary>
        /// <param name="eid">分部ID</param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        List<string> GetAllYears(string eid, string field);
    }
}
