using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels
{
    public class CityVo
    {
        public string Key { get; set; }
        public IEnumerable<CityCodes> CityCodes { get; set; }

    }
    public class CityCodes
    {
        public int AdCode { get; set; }
        public string Name { get; set; }
        public string url { get; set; }
        public string pinyin { get; set; }
    }
}
