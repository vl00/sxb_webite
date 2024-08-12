using System.Collections.Generic;

namespace Sxb.Web.Areas.School.Models.SchoolDetail
{
    public class GetCounterPartResponse
    {
        /// <summary>
        /// 对口学校
        /// </summary>
        public IEnumerable<(string, string)> CounterPart { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Years { get; set; }
    }
}
