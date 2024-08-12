using PMS.School.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Application.IServices
{
    public interface ICourseTypeService
    {
        List<CourseType> GetCourses();
    }
}
