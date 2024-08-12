using iSchool;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface ISchService
    {
        /// <summary>
        /// 查询学部的简易信息, 包含旧id转换到新id
        /// </summary>
        Task<SchExtDto0> GetSchextSimpleInfo(Guid eid);
        Task<SchExtDto0> GetSchextSimpleInfoViaShortNo(string schoolNo);
        /// <summary>
        /// 附近学校 - 获取用户当前定位最接近的同学校类型的8所学校
        /// </summary>      
        Task<SchExtDto0[]> GetNearSchoolsBySchType((double Lng, double Lat) location, SchFType0[] schFTypes = null, int count = 8);
        /// <summary>
        /// 附近学校 - 获取当前分部最接近的同学校类型的8所学校
        /// </summary>      
        Task<IEnumerable<SchExtDto0>> GetNearSchoolsByEID(Guid eid, int count = 8);

        /// <summary>
        /// 记录选校关键词   
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="uid">用户id/（未登录）设备id</param>
        /// <param name="isLoginedUser">是否是已登录用户</param>
        /// <returns></returns>
        //Task RecordSearchSchoolKeyword(string keyword, Guid? uid, bool isLoginedUser);
        /// <summary>
        /// 选校关键词 - 网站中所有用户使用搜索栏搜索的热度最高的8个关键词
        ///     会在每月重新计数, 多次输入算多次
        ///     需要具体记录哪个用户使用过哪个词
        /// </summary>
        //Task<string[]> GetSearchSchoolKeywords(int count = 8);

        /// <summary>
        /// 获取毕业去向的大学榜单
        /// </summary>
        /// <param name="type">1=国际;2=国内</param>
        /// <returns></returns>
        Task<List<AchievementInfos>> GetCollegeRankList(int type = 1);
        /// <summary>
        /// 获取国内大学列表
        /// </summary>
        /// <returns></returns>
        Task<(Guid, string)[]> GetLocalColleges();
        /// <summary>
        /// 获取某年的国际榜单 //用于pc毕业去向搜索框模糊匹配
        /// </summary>
        /// <returns></returns>
        Task<(Guid, string)[]> GetInternationalRankSchools(string txt, int year, int count = 10);

        (Guid, int)[] GetSchoolextNo(Guid[] eids);

        /// <summary>
        /// 毕业生去往国外大学是否在国际大学榜单
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDto<Guid, string, double, int, int>>> GetForeignCollegeList(List<KeyValueDto<Guid, string, double, int>> achievement);

        /// <summary>
        /// 毕业生去往国内大学是否在国内大学榜单
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDto<Guid, string, double, int, int>>> GetDomesticCollegeList(List<KeyValueDto<Guid, string, double, int>> achievement);
    }
}
