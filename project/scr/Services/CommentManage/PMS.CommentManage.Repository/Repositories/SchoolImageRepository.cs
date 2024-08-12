using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SchoolImageRepository : EntityFrameworkRepository<SchoolImage>, ISchoolImageRepository
    {
        public SchoolImageRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public List<SchoolImage> GetImageByDataSourceId(Guid DataSoucreId)
        {
           return base.GetList(x => x.DataSourcetId == DataSoucreId).ToList();
        }

        public List<SchoolImage> GetImageByDataSourceId(List<Guid> DataSoucreIds, ImageType type)
        {
            return base.GetList(x => DataSoucreIds.Contains(x.DataSourcetId) && x.ImageType == type).OrderByDescending(x => x.AddTime)?.ToList();
        }

        public int AddSchoolImage(SchoolImage schoolImage)
        {
            return base.Add(schoolImage);
        }

        public List<SchoolImage> GetSchoolImageList(Expression<Func<SchoolImage, bool>> where)
        {
            return base.GetList(where).ToList();
        }
    }
}
