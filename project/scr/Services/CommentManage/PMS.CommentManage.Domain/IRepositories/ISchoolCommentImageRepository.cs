using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ISchoolImageRepository
    {
        List<SchoolImage> GetImageByDataSourceId(Guid DataSoucreId);

        List<SchoolImage> GetImageByDataSourceId(List<Guid> DataSoucreIds, ImageType type);

        int AddSchoolImage(SchoolImage schoolImage);
        List<SchoolImage> GetSchoolImageList(Expression<Func<SchoolImage, bool>> where = null);
    }
}
