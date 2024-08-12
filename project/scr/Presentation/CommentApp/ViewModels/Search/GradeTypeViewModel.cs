using System;
using System.Collections.Generic;

namespace Sxb.Web.ViewModels.Search
{
    public class GradeTypeViewModel
    {
        public int GradeId { get; set; }
        public string GradeDesc { get; set; }
        public List<TypeItemViewModel> Items { get; set; }
        
    }


    public class TypeItemViewModel
    {
        public int International { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
    }
}