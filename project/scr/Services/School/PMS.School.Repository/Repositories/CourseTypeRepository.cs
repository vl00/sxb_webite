using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ProductManagement.Framework.MSSQLAccessor.DBContext;

namespace PMS.School.Repository.Repositories
{
    public class CourseTypeRepository : ICourseTypeRepository
    {
        ISchoolDataDBContext dbc;

        public CourseTypeRepository(ISchoolDataDBContext dbc)
        {
            this.dbc = dbc;
        }

        public List<CourseType> GetCourses() 
        {
            string sql = "select Id,Name from  CourseType where IsDel = 0";
            return dbc.Query<CourseType>(sql, new { }).ToList();
        }

    }
}
