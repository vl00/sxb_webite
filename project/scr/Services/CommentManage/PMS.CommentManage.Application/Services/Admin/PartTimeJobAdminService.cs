using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.AppService;

namespace PMS.CommentsManage.Application.Services.Admin
{
    public class PartTimeJobAdminService: AppService<PartTimeJobAdmin>,IPartTimeJobAdminService
    {
        private readonly IPartTimeJobAdminRepository _repo;
        public PartTimeJobAdminService(IPartTimeJobAdminRepository repo)
        {
            _repo = repo;
        }

        public bool CheckCodeExists(string code)
        {
            return _repo.CheckCodeExists(code);
        }

        public bool CheckOldPassword(Guid Id, string Pwd)
        {
            return _repo.CheckOldPassword(Id, Pwd);
        }

        public bool CheckPhoneExists(string phone)
        {
            return _repo.CheckPhoneExists(phone);
        }

        public bool CheckResetPassword(Guid Id)
        {
            return _repo.CheckResetPassword(Id);
        }

        public bool CheckStatus(Guid Id)
        {
            return _repo.CheckStatus(Id);
        }

        public override int Delete(Guid Id)
        {
            return _repo.Delete(Id);
        }

        public List<PartTimeJobAdmin> GetAllSupplierList()
        {
            return _repo.GetAllSupplierList();
        }

        public List<PartTimeJobAdmin> GetCurrentAllItem(Guid SupplierId)
        {
            return _repo.GetCurrentAllItem(SupplierId);
        }

        public override IEnumerable<PartTimeJobAdmin> GetList(Expression<Func<PartTimeJobAdmin, bool>> where = null)
        {
            return _repo.GetList(where);
        }

        public override PartTimeJobAdmin GetModelById(Guid Id)
        {
            return _repo.GetModelById(Id);
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdminPage(int Role, int PageIndex, int PageSize, out int total)
        {
            return _repo.GetPartTimeJobAdminPage(Role, PageIndex, PageSize, out total);
        }

        public int GetSupplierSettlementType(Guid AdminId)
        {
            return _repo.GetSupplierSettlementType(AdminId);
        }

        public override int Insert(PartTimeJobAdmin model)
        {
            model.InvitationCode = RandomCode();
            return _repo.Insert(model);
        }

        public override bool isExists(Expression<Func<PartTimeJobAdmin, bool>> where)
        {
            return _repo.isExists(where);
        }

        public PartTimeJobAdmin Login(string phone, string pwd)
        {
            return _repo.Login(phone, pwd);
        }

        public override int Update(PartTimeJobAdmin model)
        {
           return _repo.Update(model);
        }

        public string RandomCode()
        {
            List<string> code = new List<string>();
            for (int i = 65; i <= 90; i++)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] btNumber = new byte[] { (byte)i };
                code.Add(asciiEncoding.GetString(btNumber));
            }

            for (int i = 0; i <= 9; i++)
            {
                code.Add(i.ToString());
            }

            string str = "";
            for (int i = 0; i <= 3; i++)
            {
                Random random = new Random();
                int rez = random.Next(0, code.Count);
                str += code[rez];
            }

            //检测随机生成邀请码是否存在，注册码系统中唯一（用来登录，以及查找父级） ， 存在则继续递归生成邀请码
            return CheckCodeExists(str) == false ? str : RandomCode();
        }

        public PartTimeJobAdmin GetJobAdminByCode(string code)
        {
            return _repo.GetJobAdminByCode(code);
        }


        public PartTimeJobAdmin GetTopParent(Guid AdminId,int role) 
        {
            return _repo.GetTopParent(AdminId, role);
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdminByRole(int role,string phone, int pageIndex, int pageSize, out int total) 
        {
            return _repo.GetPartTimeJobAdminByRole(role, phone, pageIndex, pageSize,out total);
        }

        public List<PartTimeJobAdmin> GetJobAdminNameByIds(List<Guid> Ids) 
        {
            return _repo.GetJobAdminNameByIds(Ids);
        }

        public List<PartTimeJobAdmin> GetPartTimeJobAdminByIdRoles(List<Guid> Ids,int Role) 
        {
            return _repo.GetPartTimeJobAdminByIdRoles(Ids, Role);
        }

        public List<PartTimeJobAdmin> GetAdminIdByPhone(string phone)
        {
            return _repo.GetAdminIdByPhone(phone);
        }
    }
}
