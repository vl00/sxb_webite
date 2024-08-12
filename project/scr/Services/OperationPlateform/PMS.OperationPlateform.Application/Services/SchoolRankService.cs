using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.OperationPlateform.Application.Services
{
    using Domain.IRespositories;
    using Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using Dapper;
    using System.Threading.Tasks;
    using PMS.OperationPlateform.Application.Dtos;

    public class SchoolRankService : IServices.ISchoolRankService
    {
        ISchoolRankRepository schoolRankRepository;

        ISchoolRankBindsRepository schoolRankBindsRepository;


        public SchoolRankService(
            ISchoolRankRepository schoolRankRepository,
            ISchoolRankBindsRepository schoolRankBindsRepository
            )
        {
            this.schoolRankRepository = schoolRankRepository;
            this.schoolRankBindsRepository = schoolRankBindsRepository;
        }

        public IEnumerable<SchoolRank> GetAll()
        {
            return this.schoolRankRepository.GetRankList();
        }


        public IEnumerable<H5SchoolRankInfoDto> GetH5SchoolRankInfoBy(Guid[] schoolIds)
        {
            List<H5SchoolRankInfoDto> result = new List<H5SchoolRankInfoDto>();
            if (schoolIds == null || !schoolIds.Any())
            {
                return result;
            }
            //寻找出有这些学校的榜单
            var targetSchools = this.schoolRankBindsRepository.Select(
                @"
                    SchoolId IN @SchoolIds
                    AND EXISTS(SELECT 1 FROM SchoolRank WHERE SchoolRank.Id = SchoolRankBinds.RankId)
               ",
                new { SchoolIds = schoolIds }, null, null);
            //根据榜单ID 找出 该出现的榜单
            var ids = targetSchools.GroupBy(sb => sb.RankId).Select(s => s.Key);
            var targetRanks = this.schoolRankRepository.Select("Id IN @Ids", new { Ids = ids },null,new[] { "*"});
            //构建 H5SchoolRankInfoDto
            foreach (var school in schoolIds)
            {
                var sbs = targetSchools.Where(sb => sb.SchoolId == school);
                if (!sbs.Any()) { continue; }
                var first = sbs.OrderBy(sb => sb.Sort).First();
                var rank = targetRanks.Where(r => r.Id == first.RankId).First();

                if (rank == null) continue;

                H5SchoolRankInfoDto h5SchoolRankInfoDto = new H5SchoolRankInfoDto()
                {
                    Cover = rank.Cover,
                    RankId = rank.Id,
                    Ranking = (int)first.Sort,
                    SchoolId = school,
                    Title = rank.Title,
                    No = rank.No,
                    Count = targetRanks.ToList().Count,
                };
                result.Add(h5SchoolRankInfoDto);

            }
            return result;
        }

        public SchoolRank GetSchoolRank(Guid? id, long? no = null)
        {
            if (id == null && no == null) {
                throw new ArgumentException("id和no不能全为空，必须有一个有值。");
            }
            return this.schoolRankRepository.Select("Id=@id OR No=@no", new { id, no },null,new[] { "*"}).FirstOrDefault();
        }
        public IEnumerable<SchoolRank> GetSchoolRanks(IEnumerable<Guid> ids)
        {

            return this.schoolRankRepository.Select("Id in @ids", new { ids }, null, new[] { "*" });
        }


        public IEnumerable<SchoolRank> GetSchoolRanks(Guid schoolId, int takeAdjacentCount)
        {
            //查询出学部所在的榜单
            var schoolRanks = this.schoolRankRepository.Select(
                  where: @"
                    EXISTS(SELECT RankId FROM  SchoolRankBinds WHERE RankId=SchoolRank.Id  AND  SchoolId = @sid) 
                    AND IsShow = 1",
                  fileds: new[] { "*" },
                  param: new { sid = schoolId },
                  order: "[RANK]");
            //根据榜单列表，查询出其旗下的学部列表
            var srIds = schoolRanks.Select(sr => sr.Id);
            var schoolRankBinds = this.schoolRankBindsRepository.Select(
                  where: @" RankId in @srIds",
                  param: new { srIds });

            schoolRanks = schoolRanks.Select(sr =>
             {

                 var targetSrb = schoolRankBinds.Where(srb => sr.Id == srb.RankId).OrderBy(s => s.Sort).ToList();
                 var targetItem = targetSrb.Where(srb => srb.SchoolId == schoolId).FirstOrDefault();
                 targetSrb = Adjacent(targetSrb, targetItem, takeAdjacentCount);    //计算出了targetSrb 和 targetItem 相邻的2个项
                 sr.SchoolRankBinds = targetSrb;
                 return sr;
             });

            return schoolRanks;

        }




        /// <summary>
        /// 计算相邻的项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="org"></param>
        /// <param name="count">期待的相邻数量</param>
        /// <returns></returns>
        List<T> Adjacent<T>(List<T> list, T targetItem, int count)
        {
            if (list.Count <= count)
            {
                return list;
            }
            List<T> result = new List<T>();
            int left = count / 2; //理想状态
            int right = count - left;
            int idx_length = list.Count - 1; //索引长度
            var index = list.ToList().IndexOf(targetItem); //目标索引
            var left_remain = index - 0;    //左边剩余数
            var right_remain = idx_length - index; //右边剩余数
            //自动调整左右的数量，让理想值<=剩余值
            if (left > left_remain)
            {
                right += left - left_remain;
                left = left_remain;
            }
            if (right > right_remain)
            {
                left += right - right_remain;
                right = right_remain;
            }


            for (int i = index - left; i < index; i++)
            {
                result.Add(list[i]);
            }

            result.Add(list[index]);
            for (int i = index + 1; i <= index + right; i++)
            {
                result.Add(list[i]);
            }






            //if (list.Count > count)
            //{
            //    var index = list.ToList().IndexOf(targetItem);

            //    if (((list.Count() - 1) - index) > count)
            //    {
            //        var newList = new List<T>();

            //        for (int i = index; i <= index + count; i++)
            //        {
            //            newList.Add(list[i]);
            //        }
            //        list = newList;
            //    }
            //    else
            //    {
            //        var newList = new List<T>();

            //        for (int i = index; i >= index - count; i--)
            //        {
            //            newList.Add(list[i]);
            //        }
            //        list = newList;
            //    }

            //}
            //return list;
            return result;

        }




        //获取推荐榜单
        public IEnumerable<SchoolRank> GetRecommendSchoolRanks(List<Article_SchoolTypes> schoolTypes, int cityID, int count)
        {
            List<SchoolRank> schoolRanks = new List<SchoolRank>();
            DynamicParameters parameters = new DynamicParameters();
            StringBuilder where = new StringBuilder("EXISTS(SELECT 1 FROM  SchoolRankTypeBinds WHERE  SchoolRank.Id = SchoolRankTypeBinds.SchoolRankId ");
            if (schoolTypes != null && schoolTypes.Count > 0)
            {
                where.Append("AND   SchoolRankTypeBinds.SchoolTypeId IN @schoolTypeIds");
                parameters.Add("schoolTypeIds", schoolTypes.Select(st => st.Id));
            }
            where.Append(" )");
            where.Append(" AND SchoolRank.IsShow =1");
            if (cityID > 0)
            {
                where.Append(" AND EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId AND cityID=@cityID )");
                parameters.Add("cityID", cityID);
            }
            var result = this.schoolRankRepository.SelectPage(
                where: where.ToString(),
                param: parameters,
                order: "[rank]",
                 fileds: new[] { "*" },
                isPage: true,
                offset: 0,
                limit: count
                );
            schoolRanks.AddRange(result.Item1);

            if (result.Item1.Count() < count)
            {
                //如果数据不足，拿全国数据来补。
                DynamicParameters waitingParameter = new DynamicParameters();
                StringBuilder waitingWhere = new StringBuilder("EXISTS(SELECT 1 FROM  SchoolRankTypeBinds WHERE  SchoolRank.Id = SchoolRankTypeBinds.SchoolRankId ");
                if (schoolTypes != null && schoolTypes.Count > 0)
                {
                    waitingWhere.Append("AND   SchoolRankTypeBinds.SchoolTypeId IN @schoolTypeIds");
                    waitingParameter.Add("schoolTypeIds", schoolTypes.Select(st => st.Id));
                }
                waitingWhere.Append(" )");
                waitingWhere.Append(" AND SchoolRank.IsShow =1");
                if (cityID > 0)
                {
                    waitingWhere.Append(" AND NOT EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId )");
                }

                var waitingCount = count - result.Item1.Count();
                var waitingResult = this.schoolRankRepository.SelectPage(
               where: waitingWhere.ToString(),
               param: waitingParameter,
               order: "[rank]",
                fileds: new[] { "*" },
               isPage: true,
               offset: 0,
               limit: waitingCount
               );
                schoolRanks.AddRange(waitingResult.Item1);

            }

            return schoolRanks;

        }

        //获取推荐榜单
        public IEnumerable<SchoolRank> GetRecommendSchoolRanksSchool(List<Article_SchoolTypes> schoolTypes, int cityID, int count)
        {
            List<SchoolRank> schoolRanks = new List<SchoolRank>();

            DynamicParameters parameters = new DynamicParameters();
            StringBuilder where = new StringBuilder("EXISTS(SELECT 1 FROM  SchoolRankTypeBinds WHERE  SchoolRank.Id = SchoolRankTypeBinds.SchoolRankId ");

            if (schoolTypes != null && schoolTypes.Count > 0)
            {
                where.Append("AND   SchoolRankTypeBinds.SchoolTypeId IN @schoolTypeIds");
                parameters.Add("schoolTypeIds", schoolTypes.Select(st => st.Id));
            }
            where.Append(" )");
            where.Append(" AND SchoolRank.IsShow =1");
            where.Append(" AND EXISTS(SELECT RankId FROM  SchoolRankBinds,SchoolRank WHERE RankId=SchoolRank.Id ) AND IsShow = 1");
            if (cityID > 0)
            {
                where.Append(" AND EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId AND cityID=@cityID ) ");
                parameters.Add("cityID", cityID);
            }

            var result = this.schoolRankRepository.SelectPage(
                where: where.ToString(),
                param: parameters,
                order: "[rank]",
                isPage: true,
                offset: 0,
                limit: count
                );
            schoolRanks.AddRange(result.Item1);

            if (result.Item1.Count() < count)
            {
                //如果数据不足，拿全国数据来补。
                DynamicParameters waitingParameter = new DynamicParameters();
                StringBuilder waitingWhere = new StringBuilder("EXISTS(SELECT 1 FROM  SchoolRankTypeBinds WHERE  SchoolRank.Id = SchoolRankTypeBinds.SchoolRankId ");
                if (schoolTypes != null && schoolTypes.Count > 0)
                {
                    waitingWhere.Append("AND   SchoolRankTypeBinds.SchoolTypeId IN @schoolTypeIds");
                    waitingParameter.Add("schoolTypeIds", schoolTypes.Select(st => st.Id));
                }
                waitingWhere.Append(" )");
                waitingWhere.Append(" AND SchoolRank.IsShow =1");
                waitingWhere.Append(" AND EXISTS(SELECT RankId FROM  SchoolRankBinds,SchoolRank WHERE RankId=SchoolRank.Id ) AND IsShow = 1");

                if (cityID > 0)
                {
                    waitingWhere.Append(" AND NOT EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId )");
                }

                var waitingCount = count - result.Item1.Count();
                var waitingResult = this.schoolRankRepository.SelectPage(
               where: waitingWhere.ToString(),
               param: waitingParameter,
               order: "[rank]",
               isPage: true,
               offset: 0,
               limit: waitingCount
               );
                schoolRanks.AddRange(waitingResult.Item1);

            }
            //取出所有的榜单ID 查询其旗下所有学校
            var srIds = schoolRanks.Select(sr => sr.Id);
            var srbs = this.schoolRankBindsRepository.Select("RankId in @srIds", new { srIds });

            schoolRanks = schoolRanks.Select(sr =>
             {
                 sr.SchoolRankBinds = srbs.Where(srb => srb.RankId == sr.Id).ToList();
                 return sr;
             }).ToList();


            return schoolRanks;

        }

        /// <summary>
        /// 根据学校年纪查询学校排行榜
        /// </summary>
        public (IEnumerable<SchoolRank> schoolRanks, int total) GetSchoolRanksByGrades(int cityId, int? gradeId, int offset = 0, int limit = 20)
        {
            DynamicParameters parameters = new DynamicParameters();
            List<string> andWhere = new List<string>();
            if (gradeId != null)
            {
                andWhere.Add(@"EXISTS(SELECT 1 FROM SchoolRankTypeBinds  
                    JOIN Article_SchoolTypes ON SchoolRankTypeBinds.SchoolTypeId = Article_SchoolTypes.Id
                     WHERE SchoolRankTypeBinds.SchoolRankId = SchoolRank.Id AND Article_SchoolTypes.SchoolGrade = @gradeId)");
                parameters.Add("gradeId", gradeId);
            }
            if (cityId > 0)
            {
                andWhere.Add(@"(EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id=SchoolRankAreaBinds.SchoolRankId and  CityId=@cityId)
                                OR
                                NOT EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId)
                                )");
                parameters.Add("cityId", cityId);

            }
            andWhere.Add("IsShow=1");
            andWhere.Add("CreateTime<=GETDATE()");


            return this.schoolRankRepository.SelectPage(
                 where: string.Join(" AND ", andWhere),
                 param: parameters,
                 fileds: new[] { "*" },
                 order: "ToTop DESC,RANK,CreateTime DESC,ID",
                 isPage: true,
                 offset: offset,
                 limit: limit
                 );
        }

        public IEnumerable<H5SchoolRankInfoDto> GetRankInfoBySchID(Guid schoolId)
        {
            List<H5SchoolRankInfoDto> result = new List<H5SchoolRankInfoDto>();

            //寻找出有这些学校的榜单
            var targetSchools = this.schoolRankBindsRepository.Select(
                @"
                    SchoolId = @SchoolId
                    AND EXISTS(SELECT 1 FROM SchoolRank WHERE SchoolRank.Id = SchoolRankBinds.RankId)
               ",
                new { SchoolId = schoolId }, null, null);
            //根据榜单ID 找出 该出现的榜单
            var ids = targetSchools.GroupBy(sb => sb.RankId).Select(s => s.Key);
            var targetRanks = this.schoolRankRepository.Select("Id IN @Ids", new { Ids = ids }, null, null);

            foreach (var rankItem in targetRanks)
            {
                var sbs = targetSchools.Where(sb => sb.SchoolId == schoolId && sb.RankId == rankItem.Id).First(); ;

                H5SchoolRankInfoDto h5SchoolRankInfoDto = new H5SchoolRankInfoDto()
                {
                    Cover = rankItem.Cover,
                    RankId = rankItem.Id,
                    Ranking = (int)sbs.Sort,
                    SchoolId = schoolId,
                    Title = rankItem.Title,
                    Count = targetRanks.ToList().Count
                };
                result.Add(h5SchoolRankInfoDto);
            }

            return result;
        }

        public (IEnumerable<SchoolRank> schoolranks, int total) GetTheNationwideAndNewsRanks(int city, int offset = 0, int limit = 3)
        {
            List<SchoolRank> schoolRanks = new List<SchoolRank>();
            if (city > 0)
            {
                //有带城市的，根据城市获取前三个榜单
                var rank1 = this.schoolRankRepository.SelectPage(where: "IsShow = 1 AND EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId AND SchoolRankAreaBinds.CityId = @cityId)",
                            param: new { cityId = city },
                            offset: offset,
                            limit: limit,
                            isPage:true,
                            fileds: new[] { "*" },
                            order: " ToTop DESC , [RANK] ASC");
                schoolRanks.AddRange(rank1.Item1);
            }
            if (schoolRanks.Count < limit)
            {
                //如果上面处理后的list不足够 limit 条，尝试去全国的榜单的补上，差多少补多少
                var balance = limit - schoolRanks.Count;
                var rank2 = this.schoolRankRepository.SelectPage(
               where: @"NOT EXISTS(SELECT 1 FROM SchoolRankAreaBinds WHERE SchoolRank.Id = SchoolRankAreaBinds.SchoolRankId)
                    AND
                    IsShow = 1",
               param: null,
               offset: 0,
               limit: balance,
               order: " ToTop DESC , [RANK] ASC"
               );
                if (rank2.Item1.Any())
                {
                    schoolRanks.AddRange(rank2.Item1.Take(balance));
                }
            }
            if (schoolRanks.Any())
            {
               var schoolBinds = GetSchoolBinds(schoolRanks.Select(sr => sr.Id).ToList());
                schoolRanks.ForEach(sr =>
                {
                    sr.SchoolRankBinds = schoolBinds.Where(sb => sb.RankId == sr.Id).ToList();
                });
                return (schoolRanks, schoolRanks.Count);
            }
            else {
                return (null, 0);
            }
        }


        IEnumerable<SchoolRankBinds> GetSchoolBinds(List<Guid> rankIds)
        {
            var bindItems = this.schoolRankBindsRepository.Select(
                " RankId in @rankIds ",
                new { rankIds },
                " Sort  ASC");

            return bindItems;
        }


    }
}
