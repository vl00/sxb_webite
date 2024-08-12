using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.OperationPlateform.Application.Services
{
    using IServices;
    using Domain.Entitys;
    using Domain.Enums;
    using Domain.IRespositories;
    using Dapper;

    public class Article_SchoolTypeService:IArticle_SchoolTypeService
    {

        IArticle_SchoolTypeRepository article_SchoolTypeRepository;

        public Article_SchoolTypeService(IArticle_SchoolTypeRepository article_SchoolTypeRepository)
        {
            this.article_SchoolTypeRepository = article_SchoolTypeRepository;
        }


        public IEnumerable<Article_SchoolTypes> OrCombination(
            List<int> schoolGrades,
            List<int> schoolTypes
            )
        {
            List<string> whereOr = new  List<string>();
            DynamicParameters parameters = new DynamicParameters();
            if (schoolGrades != null && schoolGrades.Any())
            {
                whereOr.Add("SchoolGrade in @SchoolGrades");
                parameters.Add("SchoolGrades",schoolGrades);
            }
            if(schoolTypes !=null && schoolTypes.Any())
            {
                whereOr.Add(" SchoolType in @SchoolTypes");
                parameters.Add("SchoolTypes", schoolTypes);

            }

          return  this.article_SchoolTypeRepository.Select(
                where: string.Join(" OR ",whereOr),
                param: parameters
                );


        }
    }
}
