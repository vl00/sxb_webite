using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IArticle_SchoolTypeService
    {

        /// <summary>
        /// 筛选出用户已经关联的学校类型
        /// </summary>
        /// <param name="schoolGrade"></param>
        /// <param name="schoolType"></param>
        /// <returns></returns>
         IEnumerable<Article_SchoolTypes> OrCombination(
           List<int> schoolGrades,
           List<int> schoolTypes
           );


        }
}
