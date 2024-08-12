using iSchool;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IMongo;
using PMS.OperationPlateform.Domain.IRespositories;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    public class MixService : IMixService
    {
        private readonly IBaseRepository<Wiki> _wikiRepository;
        private readonly IBaseRepository<AggSchool> _aggSchoolRepository;
        private readonly IBaseRepository<RecommendOrg> _recommendOrgRepository;
        private readonly IStatisticRepository _statisticRepository;
        private readonly IDaysStatisticRepository _daysStatisticRepository;

        public MixService(IBaseRepository<Wiki> wikiRepository, IBaseRepository<AggSchool> aggSchoolRepository, IBaseRepository<RecommendOrg> recommendOrgRepository, IStatisticRepository statisticRepository, IDaysStatisticRepository daysStatisticRepository)
        {
            _wikiRepository = wikiRepository;
            _aggSchoolRepository = aggSchoolRepository;
            _recommendOrgRepository = recommendOrgRepository;
            _statisticRepository = statisticRepository;
            _daysStatisticRepository = daysStatisticRepository;
        }

        public async Task BuildDaysStatisticsAsync(string matchUrl = "https://m3.sxkid.com/school_detail_wechat/data", DateTime? dayTime = null)
        {
            dayTime = dayTime == null ? DateTime.Today.AddDays(-1) : dayTime;

            var startTime = dayTime.Value.Date;
            var endTime = startTime.AddDays(1);
            var vc = _statisticRepository.GetUrlViewCount(matchUrl, startTime, endTime);
            if (vc.Count > 0)
            {
                await _daysStatisticRepository.AddAsync(vc);
            }
        }
    }
}
