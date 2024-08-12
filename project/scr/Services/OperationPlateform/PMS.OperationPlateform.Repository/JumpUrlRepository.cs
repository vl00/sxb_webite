using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Repository
{
    public class JumpUrlRepository: IJumpUrlRepository
    {
        protected OperationCommandDBContext db;

        public JumpUrlRepository(OperationCommandDBContext dBContext)
        {
            this.db = dBContext;
        }

        public async Task<JumpUrl> GetJumpUrl(string Id)
        {
            var result = await db.QueryAsync<JumpUrl>("select * from JumpUrl where id = @id;", new { id = Id });
            return result.FirstOrDefault();
        }

        public async Task<bool> AddJumpUrl(string Id ,string url ,string fw)
        {
            var result = await db.ExecuteAsync("INSERT INTO [dbo].[JumpUrl] ([Id], [Url], [Fw], [CreateTime], [PV]) VALUES (@id, @url,@fw,getdate(), 1);", new { id = Id, url ,fw });
            return result>0;
        }
        public async Task<bool> IncreJumpUrl(string Id)
        {
            var result = await db.ExecuteAsync("UPDATE [dbo].[JumpUrl] SET [PV] = [PV]+1 WHERE [Id] = @id;", new { id = Id });
            return result > 0;
        }
    }
}
