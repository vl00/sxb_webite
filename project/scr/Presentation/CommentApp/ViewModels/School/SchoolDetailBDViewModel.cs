using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.School
{
    public class SchoolDetailBDViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public InfoViewModel Info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InfoExtViewModel Info_ext { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ExtensionsList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        public string ErrMsg { get; set; }



        public class InfoViewModel
        {
            /// <summary>
            /// 
            /// </summary>
            public Guid Id { get; set; }
            /// <summary>
            /// 广州市白云区广外附属小学
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Name_e { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Website { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Intro { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Logo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Show { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Value { get; set; }
        }

       

        public class InfoExtViewModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string Collected { get; set; }
            /// <summary>
            /// 广州
            /// </summary>
            public string CityName { get; set; }
            /// <summary>
            /// 白云
            /// </summary>
            public string AreaName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<int> Grade { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> Tags_m { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> Tags_s { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Guid Id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Guid Sid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Nature { get; set; }
            /// <summary>
            /// 校本部
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Province { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int City { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Area { get; set; }
            /// <summary>
            /// 广东省广州市白云区白云大道北2号
            /// </summary>
            public string Address { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double? Latitude { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double? Longitude { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Tel { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double? Fee { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Teacher { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Admissions { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Feedesc { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Student { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Hardware { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Item> Quality { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Sort { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Video { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string VideoDesc { get; set; }
        }
    }
    
}
