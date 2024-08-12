using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Images
{
    public class SchoolImageService : ISchoolImageService
    {
        ISchoolImageRepository _repository;
        public SchoolImageService(ISchoolImageRepository repository)
        {
            _repository = repository;
        }

        public int AddSchoolImage(SchoolImage schoolImage)
        {
            return _repository.AddSchoolImage(schoolImage);
        }

        public List<SchoolImage> GetImageByDataSourceId(Guid DataSoucreId)
        {
            return _repository.GetImageByDataSourceId(DataSoucreId);
        }
    }
}
