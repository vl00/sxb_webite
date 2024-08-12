using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    using IServices;
    using Domain.Entitys;
    using Domain.IRespositories;

    public class SchoolRankBindsService:ISchoolRankBindsService
    {
        ISchoolRankBindsRepository  schoolRankBindsRepository;

        public SchoolRankBindsService(
            ISchoolRankBindsRepository schoolRankBindsRepository
            )
        {
            this.schoolRankBindsRepository = schoolRankBindsRepository;
        }


        public IEnumerable<SchoolRankBinds> GetSchoolRankBinds(SchoolRank rank)
        {
            return this.schoolRankBindsRepository.Select(" RankId=@rankId ", new { rankId = rank.Id }, "sort");
        }

        public (IEnumerable<SchoolRankBinds>, int total) GetSchoolRankBinds_PageVersion(SchoolRank rank, int offset = 0, int limit = 30)
        {
            return this.schoolRankBindsRepository.SelectPage(" RankId=@rankId ", new { rankId = rank.Id }, "sort",null,true,offset,limit);
        }

      

    }
}
