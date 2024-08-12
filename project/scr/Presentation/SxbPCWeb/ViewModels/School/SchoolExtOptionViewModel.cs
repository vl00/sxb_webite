using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.ViewModels.School
{
    public class SchoolExtOptionViewModel
    {
        public List<OptionViewModel> TypeOption { get; set; }

        public int AreaSelect { get; set; }
        public int? GradeSelect { get; set; }
        public Guid? MetroSelect { get; set; }


        public int? SelectCity { get; set; }
        public List<OptionViewModel> SelectCitys{ get; set; }
        public KV LocalCity { get; set; }

        public List<OptionViewModel> HotCity { get; set; }

        public List<OptionViewModel> Areas { get; set; }
        public List<OptionViewModel> Metros { get; set; }

        public List<OptionViewModel> Ranges { get; set; }

        public List<OptionViewModel> Lodging { get; set; }
        public List<OptionViewModel> Canteen { get; set; }
        public List<OptionViewModel> Order { get; set; }


        public List<OptionViewModel> AbroadIds { get; set; }
        public List<OptionViewModel> AuthIds { get; set; }
        public List<OptionViewModel> CharacIds { get; set; }
        public List<OptionViewModel> CourseIds { get; set; }

        //学费
        public List<OptionViewModel> Costs { get; set; }
        public List<OptionViewModel> Students { get; set; }
        public List<OptionViewModel> Teachers { get; set; }



        public OptionViewModel ClearGrade { get; set; }
        public OptionViewModel ClearArea { get; set; }
        
        public OptionViewModel ClearAll { get; set; }

        public class KV
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }
    }


    //public class 

    public class OptionViewModel
    {
        public string Href { get; set; }
        public string Title { get; set; }
        public string OptionName { get; set; }
        public string OptionCode { get; set; }
        public bool Active { get; set; }
        public List<OptionViewModel> List { get; set; }
        public string RemoveHref { get; set; }
    }
}
