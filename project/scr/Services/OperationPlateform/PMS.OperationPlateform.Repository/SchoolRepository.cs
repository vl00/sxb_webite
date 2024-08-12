using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.Entitys;
    using Domain.IRespositories;
    using PMS.OperationPlateform.Domain.ModelViews;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class SchoolRepository : ISchoolRepository
    {
        ISchoolDataDBContext db;

        public SchoolRepository(ISchoolDataDBContext schoolDataDBContext)
        {
            this.db = schoolDataDBContext;
        }


        public IEnumerable<OnlineSchoolExtension> GetSchoolExtensionByParent(Guid sid)
        {
            string sql = @"
                        SELECT  OnlineSchoolExtension.id,OnlineSchoolExtension.sid,OnlineSchoolExtension.name ,
                        OnlineSchool.id,OnlineSchool.name
                        FROM OnlineSchoolExtension
                        JOIN OnlineSchool ON OnlineSchoolExtension.sid = OnlineSchool.id
                        WHERE sid =@sid ";
            return this.db.Query<OnlineSchoolExtension,OnlineSchool,OnlineSchoolExtension>(sql,(ose,os)=> {
                ose.Parent = os;
                return ose;
            }, new { sid = sid });
        }

        public IEnumerable<UserScribeSchoolAreasAndSchoolTypes> GetUserSubsccribeSchoolAreasAndTypes(Guid userId)
        {
            string sql = "EXEC usp_select_user_subscribe_school_areas_and_schooltype_info @userId";
           return  this.db.Query<UserScribeSchoolAreasAndSchoolTypes>(sql, new { userId = userId });
        }
    }
}
