using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.IRespositories
{
    using Entitys;
    using PMS.OperationPlateform.Domain.ModelViews;

    /// <summary>
    /// 学校数据仓储访问
    /// </summary>
    public interface ISchoolRepository
    {

        /// <summary>
        /// 根据学校ID查询学校分部 
        /// </summary>
        /// <param name="sid">学校ID</param>
        /// <returns></returns>

        IEnumerable<OnlineSchoolExtension> GetSchoolExtensionByParent(Guid sid);

        /// <summary>
        /// 查询用户关注的学部所关联的地区和类型
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>

        IEnumerable<UserScribeSchoolAreasAndSchoolTypes> GetUserSubsccribeSchoolAreasAndTypes(Guid userId);



    }
}
