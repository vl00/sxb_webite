using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Domain;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Framework.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Data.SqlClient;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.AppService;
using PMS.CommentsManage.Repository.Interface;

namespace PMS.CommentsManage.Repository
{
    /// <summary>
    /// 管理员操作
    /// </summary>
    public class PartTimeJobAdminRepository : EntityFrameworkRepository<PartTimeJobAdmin>, IPartTimeJobAdminRepository
    {

       // private readonly EntityFrameworkRepository<PartTimeJobAdmin> efRepository;
        private readonly CommentsManageDbContext _dbContext;
        private IPartTimeJobAdminRolereRepository _jobAdminRolereRepository;

        public PartTimeJobAdminRepository(CommentsManageDbContext dbContext, 
            IPartTimeJobAdminRolereRepository jobAdminRolereRepository):base(dbContext)
        {
            _dbContext = dbContext;
            _jobAdminRolereRepository = jobAdminRolereRepository;
        }

        public bool CheckCodeExists(string code)
        {
            
            return base.GetList(x => x.InvitationCode == code).FirstOrDefault() == null ? false : true;
        }


        public bool CheckPhoneExists(string phone)
        {
            return base.GetList(x => x.Phone == phone).FirstOrDefault() == null ? false : true;
        }

        public bool CheckStatus(Guid Id)
        {
            PartTimeJobAdmin partTimeJobAdmin = GetAggregateById(Id);
            partTimeJobAdmin.Prohibit = !partTimeJobAdmin.Prohibit;
            base.Update(partTimeJobAdmin);
            return partTimeJobAdmin.Prohibit;
        }

        public new int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public List<PartTimeJobAdmin> GetAllSupplierList()
        {
            return base.GetList(x => (int)x.Role == 3).ToList();
        }


        public new IEnumerable<PartTimeJobAdmin> GetList(Expression<Func<PartTimeJobAdmin, bool>> where = null)
        {
            var data = base.GetList(where);
            if(data.Count()!=0) 
            {
                return data.ToList();
            }
            return null;
        }

        public  PartTimeJobAdmin GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdminPage(int Role, int PageIndex, int PageSize, out int total)
        {
            //查出所有的供应商id
            total = _jobAdminRolereRepository.GetList(x => x.Role == Role).Count();

            return _jobAdminRolereRepository.GetList(x => x.Role == Role).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => x.PartTimeJobAdmin).ToList();

            //var rez = GetList(x => x.PartTimeJobAdminRoles.Select(r=>r.Role).ToList().Contains(Role));
            //total = rez.Count();
            //return rez.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }

        public  int Insert(PartTimeJobAdmin model)
        {
            return base.Add(model);
        }

        public  bool isExists(Expression<Func<PartTimeJobAdmin, bool>> where)
        {
            return base.GetList(where).FirstOrDefault() == null;
        }


        public  int Update(PartTimeJobAdmin model)
        {
            return base.Update(model);
        }

        public List<PartTimeJobAdmin> GetCurrentAllItem(Guid SupplierId)
        {
            SqlParameter[] para = {
                new SqlParameter("@SupplierId",SupplierId)
            };
            List<PartTimeJobAdmin> partTimeJobAdmins = QueryByProc("proc_FindCurrentAllItem", out List<object> obj, para)?.ToList();
            return partTimeJobAdmins;
        }

        public PartTimeJobAdmin Login(string phone, string pwd)
        {
            //检测是否为第一次登录，判断密码是否为空
            PartTimeJobAdmin entity = base.GetList(x => x.Phone == phone).FirstOrDefault();
            
            if (entity != null)
            {
                //用户角色
                //var role = _jobAdminRolereRepository.GetPartJobRoles(entity.Id).OrderByDescending(x=>x.Role).Select(x=>x.Role).FirstOrDefault();
                var role = entity.PartTimeJobAdminRoles.OrderByDescending(x => x.Role).Select(x => x.Role).FirstOrDefault();
                //1：兼职，2：兼职领队，3：供应商，4：审核，5：管理员
                //兼职人员无法登录后台，且进行检测是否被禁用
                if (role < 2 || entity.Prohibit)
                {
                    return null;
                }

                //账号正确
                if (entity.Password == "" || entity.Password == null)
                {
                    //第一次登录密码为注册码
                    if (pwd == entity.InvitationCode)
                    {
                        return entity;
                    }
                    else
                    {
                        //注册码错误，登录失败
                        return null;
                    }
                }
                else
                {
                    if (DesTool.Md5(pwd).ToUpper() == entity.Password.ToUpper())
                    {
                        return entity;
                    }
                    else
                    {
                        //密码错误
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public bool CheckOldPassword(Guid Id, string Pwd)
        {
            PartTimeJobAdmin partTimeJobAdmin = base.GetAggregateById(Id);
            //第一次修改密码原始密码为邀请码
            if (partTimeJobAdmin.Password == null)
            {
                if (partTimeJobAdmin.InvitationCode == Pwd)
                {
                    return true;
                }
            }
            else
            {
                if (partTimeJobAdmin.Password == DesTool.Md5(Pwd))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckResetPassword(Guid Id)
        {
            var entity = base.GetAggregateById(Id);
            if (entity.Password == "" || entity.Password == null)
            {
                return false;
            }
           return true;
        }

        public int GetSupplierSettlementType(Guid AdminId)
        {
            string sql = @"	with temp as
	        (
		        select * from PartTimeJobAdmins where Id = @Id
		        union all
		        select p.* from PartTimeJobAdmins as p inner join temp on  temp.ParentId = p.Id
	        )
	        select *　from temp where ParentId = '00000000-0000-0000-0000-000000000000'";

            SqlParameter[] para = {
                new SqlParameter("@Id",AdminId)
            };

            var admin = Query<PartTimeJobAdmin>(sql, para).FirstOrDefault();
            if (admin == null)
            {
                return 0;
            }
            else
            {
                return (int)admin.SettlementType;
            }
        }

        public PartTimeJobAdmin GetTopParent(Guid AdminId,int role)
        {
            string sql = @"		with temp as
	        (
		        select * from PartTimeJobAdminRoles where AdminId = @Id and Role = @role
		        union all
		        select p.* from PartTimeJobAdminRoles as p inner join temp on  temp.ParentId = p.AdminId  and p.Role <> 1
	        )
	        select AdminId　from temp where ParentId = '00000000-0000-0000-0000-000000000000'";

            SqlParameter[] para = {
                new SqlParameter("@Id",AdminId),
                new SqlParameter("@role",role)
            };

            var admin = Query<PartTimeJobAdminRole>(sql, para).FirstOrDefault();
            var parentAdmin = base.GetAggregateById(admin.AdminId);

            return parentAdmin;
        }


        public PartTimeJobAdmin GetJobAdminByCode(string code)
        {
            return base.GetList(x =>  x.InvitationCode.ToLower() == code.ToLower()).FirstOrDefault();
        }


        public List<PartTimeJobAdmin> GetPartTimeJobAdminByRole(int role,string phone,int pageIndex,int pageSize,out int total) 
        {
            List<SqlParameter> para = new List<SqlParameter>();
            string sql = @"	select
                                a.Id,
                                 a.Name,
                                 a.RegesitTime,
                                 a.Phone,
                                 a.Prohibit,
                                 a.SettlementType,
                                 r.ParentId,
                                 a.InvitationCode
                                from PartTimeJobAdminRoles as r
		                        left join PartTimeJobAdmins as a on r.AdminId = a.Id
	                        where r.Role = @role
	                        ";


            string sqlTotal = @"select
                                    count(a.Id) as Total
                                from PartTimeJobAdminRoles as r
		                        left join PartTimeJobAdmins as a on r.AdminId = a.Id
	                        where r.Role = @role";

            para.Add(new SqlParameter("@role", role));
            para.Add(new SqlParameter("@pageindex", pageIndex));
            para.Add(new SqlParameter("@pagesize", pageSize));

            List<SqlParameter> Total = new List<SqlParameter>();
            Total.Add(new SqlParameter("@role", role));

            if (phone != "" && phone != null)
            {
                sql += " and a.phone like @phone ";
                sqlTotal += " and a.phone like @phone ";
                para.Add(new SqlParameter("@phone", phone));
                Total.Add(new SqlParameter("@phone", phone));
            }

            total = Query<UserPublishTotal>(sqlTotal, Total.ToArray()).FirstOrDefault().Total;

            sql += " ORDER BY a.RegesitTime desc OFFSET (@pageindex-1)*@pagesize ROWS FETCH NEXT @pagesize ROWS ONLY ";

            return Query<PartTimeJobAdmin>(sql,para.ToArray())?.ToList();
        }

        //public int GetPartTimeJobAdminByRole

        public List<PartTimeJobAdmin> GetJobAdminNameByIds(List<Guid> Ids) 
        {
            return base.GetList(x => Ids.Contains(x.Id)).Select(x => new PartTimeJobAdmin() { Id = x.Id, Name = x.Name,ParentId = x.ParentId })?.ToList();
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdminByIdRoles(List<Guid> Ids, int Role) 
        {

            List<SqlParameter> para = new List<SqlParameter>();
            string strpara = string.Join(",", Ids.Select((q, idx) => "@id" + idx));
            para = Ids.Select((i, e) => new SqlParameter("@id" + e, i)).ToList();

            para.Add(new SqlParameter("@role", Role));

            string sql = @"	select
                                a.Id,
                                 a.Name,
                                 a.RegesitTime,
                                 a.Phone,
                                 a.Prohibit,
                                 a.SettlementType,
                                 r.ParentId,
                                 a.InvitationCode
                                from PartTimeJobAdminRoles as r
		                        left join PartTimeJobAdmins as a on r.AdminId = a.Id
	                        where r.Role = @role and r.AdminId in ("+ strpara + ")";
            return Query<PartTimeJobAdmin>(sql,para.ToArray())?.ToList();
        }


        public List<PartTimeJobAdmin> GetAdminIdByPhone(string phone) 
        {
            string sql = "select Id from PartTimeJobAdmins where Phone like @phone";
            SqlParameter[] para = {
                new SqlParameter("@phone",phone)
            };

            return Query<PartTimeJobAdmin>(sql, para.ToArray())?.ToList();
        }


    }
}
