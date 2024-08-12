using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolImageService
    {
        List<SchoolImage> GetImageByDataSourceId(Guid DataSoucreId);
        int AddSchoolImage(SchoolImage schoolImage);
    }
}
