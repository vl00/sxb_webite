using PMS.School.Application.IServices;
using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Application.Services
{
    public class CourseTypeService : ICourseTypeService
    {
        private ICourseTypeRepository _courseTypeRepository;
        public CourseTypeService(ICourseTypeRepository courseTypeRepository) 
        {
            _courseTypeRepository = courseTypeRepository;
        }

        public List<CourseType> GetCourses()
        {
            return _courseTypeRepository.GetCourses();
        }
    }
}
