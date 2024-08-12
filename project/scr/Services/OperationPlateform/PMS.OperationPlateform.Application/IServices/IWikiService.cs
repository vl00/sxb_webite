using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IWikiService
    {
        /// <summary>
        /// 获取聚合页广告学校
        /// </summary>
        /// <param name="sortExtIds">需要判断的学校</param>
        /// <returns>有广告的extId</returns>
        IEnumerable<Guid> GetAggSchools(IEnumerable<Guid> sortExtIds);

        IEnumerable<string> GetRecommends(RecommendOrgGroupCode groupCode, RecommendOrgDataType dataType
            , int cityId, int areaId, Guid sId, Guid extId, string[] schoolTypes);

        string GetWikiContentByName(string name);
    }
}
