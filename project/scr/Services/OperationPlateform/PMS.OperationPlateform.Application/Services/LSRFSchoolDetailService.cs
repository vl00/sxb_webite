using Newtonsoft.Json;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Services
{
    public class LSRFSchoolDetailService : ILSRFSchoolDetailService
    {
        private ILSRFSchoolDetailRepository _schoolDetailRepository;
        public LSRFSchoolDetailService(ILSRFSchoolDetailRepository schoolDetailRepository) 
        {
            _schoolDetailRepository = schoolDetailRepository;
        }

        public bool CheckSchIsLeaving(Guid SchId)
        {
            return _schoolDetailRepository.CheckSchIsLeaving(SchId);
        }

        public List<LSRFSchoolDetail> GetAdvertisementSchool(CourseType courseType, LSRFSchoolType DataType, List<Guid> SchIds, int PageNo = 1, int PageSize = 6)
        {
            return _schoolDetailRepository.GetAdvertisementSchool(courseType,DataType,SchIds, PageNo, PageSize);
        }

        public KeyValue GetCourseTypeAdveTotal(CourseType courseType)
        {
            var temp = _schoolDetailRepository.GetCourseTypeAdveTotal(courseType);
            if (temp.Value != "") 
            {
                return  new KeyValue() { Value = temp.Value };
            }
            return new KeyValue() { Value = "0" };
        }

        public KeyValue GetCurrentCourseTotal(CourseType courseType, List<Guid> SchIds)
        {
            return _schoolDetailRepository.GetCurrentCourseTotal(courseType,SchIds);
        }

        public List<LSRFSchoolDetail> GetLSRFSchools(CourseType courseType, List<Guid> SchIds, int PageNo = 1, int PageSize = 6)
        {
            if(PageSize == 0) 
            {
                return new List<LSRFSchoolDetail>();
            }
            return _schoolDetailRepository.GetLSRFSchools(courseType,SchIds,PageNo,PageSize);
        }

        public List<SingSchoolRank> GetSchoolRankByEid(List<Guid> SchId)
        {
            return _schoolDetailRepository.GetSchoolRankByEid(SchId);
        }

        public List<LSRFSchoolDetail> SchDistinct(int courseType)
        {
            return _schoolDetailRepository.SchDistinct(courseType);
        }
    }
}
