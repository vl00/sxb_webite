using iSchool;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class WikiService : IWikiService
    {
        private readonly IBaseRepository<Wiki> _wikiRepository;
        private readonly IBaseRepository<AggSchool> _aggSchoolRepository;
        private readonly IBaseRepository<RecommendOrg> _recommendOrgRepository;

        public WikiService(IBaseRepository<Wiki> wikiRepository, IBaseRepository<AggSchool> aggSchoolRepository, IBaseRepository<RecommendOrg> recommendOrgRepository)
        {
            _wikiRepository = wikiRepository;
            _aggSchoolRepository = aggSchoolRepository;
            _recommendOrgRepository = recommendOrgRepository;
        }

        public string GetWikiContentByName(string name)
        {
            var wiki = _wikiRepository.TakeFirst(" name = @name ", new { name });
            return wiki?.Content;
        }


        public IEnumerable<Guid> GetAggSchools(IEnumerable<Guid> sortExtIds)
        {
            var aggSchools = _aggSchoolRepository.Select(" ExtId IN @extIds ", new { extIds = sortExtIds });

            //使用源序列
            foreach (var item in sortExtIds)
            {
                if (aggSchools.Any(s => s.ExtId == item))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<string> GetRecommends(RecommendOrgGroupCode groupCode, RecommendOrgDataType dataType
            , int cityId, int areaId, Guid sId, Guid extId, string[] schoolTypes)
        {

            var schoolTypeSql = new StringBuilder();
            if (schoolTypes != null)
            {
                foreach (var item in schoolTypes)
                {
                    schoolTypeSql.Append(" OR CHARINDEX('").Append(item).Append("', SchoolTypes) > 0 ");
                }
            }
            var where = $@"
	            IsDeleted = 0
                AND GroupCode = @groupCode
	            AND DataType = @dataType
	            AND StartTime <= getdate()
	            AND EndTime >= getdate()
	            AND (
		            (
			            SId = @sId
			            AND(ExtIds IS NULL OR ExtIds = '全部' OR CHARINDEX(@extId, ExtIds) > 0)
		            )
		            OR(
			            (CityId <= 0 or CityId = @cityId)
			            AND(
				            AreaIds IS NULL OR AreaIds = '全部' OR CHARINDEX(@areaId, AreaIds) > 0)
			            AND(
				            SchoolTypes IS NULL OR SchoolTypes = '全部' {schoolTypeSql})
		            )
	            )
            ";
            var recommend = _recommendOrgRepository.TakeFirst(where,
                new
                {
                    groupCode = groupCode.ToString(),
                    dataType = (int)dataType,
                    cityId,
                    areaId = areaId == 0 ? "全部" : areaId.ToString(),
                    sId,
                    extId = extId.ToString(),
                },
                " RecommendType ASC, CreateTime DESC ");

            if (recommend != null && recommend.DataIds != null)
            {
                var dataIds = recommend.DataIds.Split('\u002C');
                foreach (var item in dataIds)
                {
                    //if (Guid.TryParse(item, out Guid id))
                    {
                        yield return item.Trim();
                    }
                }
            }
            //return Enumerable.Empty<Guid>();
        }
    }
}
