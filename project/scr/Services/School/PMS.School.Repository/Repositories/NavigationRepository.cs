using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class NavigationRepository : INavigationRepository
    {
        ISchoolDataDBContext _DB;

        public NavigationRepository(ISchoolDataDBContext db)
        {
            _DB = db;
        }

        public async Task<IEnumerable<NavigationDto>> GetList(int count = 8)
        {
            var str_SQL = $@"SELECT TOP
	                            {count} * 
                            FROM
	                            Navigation 
                            ORDER BY
	                            [Index]";
            return await _DB.QueryAsync<NavigationDto>(str_SQL, new { });
        }

        public async Task<IEnumerable<PCNavigationDto>> GetPCList()
        {
            var str_SQL = "Select * from PCNavigationInfo WHERE Enabled = 1;";
            var navigations = await _DB.QueryAsync<PCNavigationInfo>(str_SQL, new { });
            if (navigations?.Any() == true)
            {
                var result = new List<PCNavigationDto>();
                var ids = navigations.Select(p => p.ID);
                var str_SQL_Item = "Select * from PCNavigationItemInfo WHERE PCNavigationID in @ids;";
                var items = await _DB.QueryAsync<PCNavigationItemInfo>(str_SQL_Item, new { ids });
                var str_SQL_Recommend = "Select * from PCNavigationRecommendInfo WHERE PCNavigationID in @ids;";
                var recommends = await _DB.QueryAsync<PCNavigationRecommendInfo>(str_SQL_Recommend, new { ids });

                foreach (var item in navigations.OrderBy(p => p.Index))
                {
                    var dto = new PCNavigationDto()
                    {
                        Navigation = item,
                        Items = items.Where(p => p.PCNavigationID == item.ID).OrderBy(p => p.Index),
                        Recommends = recommends.Where(p => p.PCNavigationID == item.ID).OrderBy(p => p.Index)
                    };
                    result.Add(dto);
                }
                return result;
            }
            return null;
        }
    }
}
