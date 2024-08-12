using PMS.OperationPlateform.Domain.Entitys.Signup;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Repository.Repositories
{
    public class HurunRepository : IHurunRepository

    {
        private OperationCommandDBContext _DB;

        public HurunRepository(OperationCommandDBContext context)
        {
            _DB = context;
        }

        public async Task<bool> Add(HurunReportSignup entity)
        {
            try
            {
                return await _DB.InsertAsync(entity) > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<bool> CheckExist(string name, string contact)
        {
            var str_SQL = $@"Select Count(1) From SignupHurunReport Where Name = @name and Contact = @contact;";
            return await _DB.QuerySingleAsync<int>(str_SQL, new { name, contact }) > 0;
        }
    }
}
