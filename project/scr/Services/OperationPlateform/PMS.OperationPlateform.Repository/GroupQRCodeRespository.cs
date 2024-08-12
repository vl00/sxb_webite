
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class GroupQRCodeRespository : BaseRepository<GroupQRCode>,IGroupQRCodeRespository
    {
        public GroupQRCodeRespository(OperationDBContext db) : base(db)
        {
        }

        public IEnumerable<GroupQRCode> GetGroupQRCodesBy(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new List<GroupQRCode>();
            }
            string sql = @"SELECT * FROM GroupQRCode where Id in @ids";
            return this._db.Query<GroupQRCode>(sql, new { ids });
        }
    }
}
