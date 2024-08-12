using PMS.School.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models.Search
{
    public class SchoolGradeDetailVM
    {
        public int GradeId { get; set; }
        public string GradeDesc { get; set; }
        public IEnumerable<SchoolTypeDetailVM> Types { get; set; }

        public class SchoolTypeDetailVM
        {
            public int GradeId { get; set; }
            public int TypeId { get; set; }
            public string TypeValue { get; set; }
            public string TypeName { get; set; }
        }

        public static List<SchoolGradeDetailVM> ConvertTo(List<GradeTypeDto> gradeTypeDtos)
        {
            return gradeTypeDtos
                    .OrderByDescending(s => s.GradeId)
                    //仅返回id和desc
                    .Select(s => new SchoolGradeDetailVM
                    {
                        GradeId = s.GradeId,
                        GradeDesc = s.GradeDesc,
                        Types = s.Types.Select(t => new SchoolTypeDetailVM()
                        {
                            GradeId = t.GradeId,
                            TypeId = t.TypeId,
                            TypeValue = t.TypeValue,
                            TypeName = t.TypeName,
                        })
                    }).ToList();
        }
    }
}
