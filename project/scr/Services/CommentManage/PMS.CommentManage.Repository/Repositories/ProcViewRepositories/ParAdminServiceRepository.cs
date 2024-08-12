using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PMS.CommentsManage.Repository.Repositories.ProcViewRepositories
{
    public class ParAdminServiceRepository : EntityFrameworkRepository<ProcGetAdminByRoleTypeEntity>,IParAdminServiceRepository
    {
        private readonly CommentsManageDbContext _dbContext;

        public ParAdminServiceRepository(CommentsManageDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProcGetAdminByRoleTypeEntity> ExecGetAdminByRoleType(Guid parentId, int type, int PageIndex, int PageSize,DateTime beginTime,DateTime endTime, out int TotalNumber)
        {
            string sqlTotal = @"select count(1) as Total from PartTimeJobAdminRoles as r
	                                RIGHT join PartTimeJobAdmins as a on r.AdminId = a.Id and a.RegesitTime >= @beginTime and a.RegesitTime <= @endTime
	                                left join (select count(1) as Total,ParentId from PartTimeJobAdminRoles GROUP BY ParentId) as c on c.ParentId = a.id
                                where r.Role = @type and r.ParentId = @parentId";

            TotalNumber =Query<BaseTotal>(sqlTotal, new SqlParameter[] 
                {
                    new SqlParameter("@type",type), 
                    new SqlParameter("@parentId", parentId),
                    new SqlParameter("@beginTime",beginTime),
                    new SqlParameter("@endTime",endTime)
                }).FirstOrDefault().Total;

            string sql = @"select a.Id,
			                 a.Phone,
			                 a.Name,
			                 a.SettlementType,
			                 a.InvitationCode,
			                 c.Total from PartTimeJobAdminRoles as r
	                RIGHT join PartTimeJobAdmins as a on r.AdminId = a.Id and a.RegesitTime >= @beginTime and a.RegesitTime <= @endTime
	                left join (select count(1) as Total,ParentId from PartTimeJobAdminRoles GROUP BY ParentId) as c on c.ParentId = a.id
                where r.Role = @type and r.ParentId = @parentId
	                order by c.Total desc  OFFSET (@PageIndex - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";

            SqlParameter[] para = {
                new SqlParameter("@Type",type),
                new SqlParameter("@PageIndex",PageIndex),
                new SqlParameter("@PageSize",PageSize),
                new SqlParameter("@parentId",parentId),
                new SqlParameter("@beginTime",beginTime),
                new SqlParameter("@endTime",endTime)
            };

            return  Query<ProcGetAdminByRoleTypeEntity>(sql, para)?.ToList();
            //SqlParameter[] para = {
            //    new SqlParameter("@TotalNumber",SqlDbType.Int),
            //    new SqlParameter("@ParentId",parentId),
            //    new SqlParameter("@Type",type),
            //    new SqlParameter("@PageIndex",PageIndex),
            //    new SqlParameter("@PageSize",PageSize)
            //};

            //para[0].Direction = ParameterDirection.Output;

            //var data = efRepository.QueryByProc("proc_GetAdminByRoleType", out List<object> obj, para);
            //if (data != null)
            //{

            //    TotalNumber = (int)obj[0];
            //    return data.ToList();
            //}
            //else
            //{
            //    TotalNumber = 0;
            //    return null;
            //}
        }

    }
}
