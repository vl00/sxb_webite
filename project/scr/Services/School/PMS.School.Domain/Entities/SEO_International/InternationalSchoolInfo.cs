using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace PMS.School.Domain.Entities.SEO_International
{
    [Table("SEO_InternationalSchoolInfo")]
    public class InternationalSchoolInfo
    {
        [ExplicitKey]
        public Guid SchoolID { get; set; }
        public string ImagePath { get; set; }
        public string SchoolName { get; set; }
        public string Stage { get; set; }
        public string Course { get; set; }
        public string Introduction { get; set; }
        public Guid PageID { get; set; }
        public bool Only_Show_Log { get; set; }
        public IEnumerable<string> Stages
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(Stage);
                }
                catch
                {

                }
                return new string[1] { Stage };
            }
        }
        public IEnumerable<string> Courses
        {
            get
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(Course);
                }
                catch
                {
                }
                return new string[1] { Course };
            }
        }
    }
}
