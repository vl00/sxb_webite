using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    public class LSRFLeveInfoRepository : ILSRFLeveInfoRepository
    {

        protected OperationCommandDBContext db;

        public LSRFLeveInfoRepository(OperationCommandDBContext dBContext)
        {
            this.db = dBContext;
        }

        public bool Insert(LSRFLeveInfo lSRFLeveInfo)
        {
            //return db.Insert<LSRFLeveInfo>(lSRFLeveInfo) > 0;
            return db.Execute("insert into LSRFLeveInfo(UserId, FullName, Phone, City, Type, Area, Stage, CourseSetting, SchId, CreateTime, Creator, UpdateTime, Updator) VALUES(@UserId,@FullName, @Phone, @City, @Type, @Area, @Stage, @CourseSetting, @SchId, default, @Creator, @UpdateTime, @Updator)",lSRFLeveInfo ) > 0;
        }
    }
}
