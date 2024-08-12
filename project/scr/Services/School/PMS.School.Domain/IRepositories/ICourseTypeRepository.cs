using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.IRepositories
{
    public interface ICourseTypeRepository
    {
        List<CourseType> GetCourses();
    }
}
