using System;
using System.Collections.Generic;
using iSchool;
using PMS.School.Domain.Common;

namespace PMS.School.Application.ModelDto
{

    public class GradeTypeBaseDto
    {
        public int GradeId { get; set; }
        public string GradeDesc { get; set; }
    }

    public class GradeTypeDto : GradeTypeBaseDto
    {
        public List<SchoolTypeDto> Types { get; set; }
    }

    public class SchoolTypeDto
    {
        public int GradeId { get; set; }
        public int TypeId { get; set; }
        public int Discount { get; set; }
        public int Diglossia { get; set; }
        public int Chinese { get; set; }

        public int International => TypeId == (int)SchoolType.International ? 1 : 0;

        //public string TypeValue => string.Format("{0}.{1}.{2}.{3}.{4}", GradeId, TypeId, Discount, Diglossia, Chinese);
        //新的筛选值
        public string TypeValue => new SchFType0((byte)GradeId, (byte)TypeId, Discount > 0, Diglossia > 0, Chinese > 0).ToString();

        public string TypeName { get; set; }
        public bool active { get; set; } = false;
    }
}
