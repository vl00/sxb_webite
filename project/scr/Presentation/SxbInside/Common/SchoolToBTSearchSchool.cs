using Newtonsoft.Json;
using PMS.School.Domain.Entities;
using PMS.Search.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Inside.Common
{
    public static class SchoolToBTSearchSchool
    {
        public static List<BTSearchSchool> SchoolDtoToBTSearchSchool(List<SchoolSearchImport> data) 
        {
            List<BTSearchSchool> schoolImports = new List<BTSearchSchool>();
            data.ForEach(x => {
                BTSearchSchool searchSchool = new BTSearchSchool();
                searchSchool.UpdateTime = x.CreateTime;
                searchSchool.Creator = x.Creator;
                searchSchool.Id = x.Id;
                searchSchool.Name = x.Name;

                var extDetails = JsonConvert.DeserializeObject<List<SchoolExtDetail>>(x.ExtDetail ?? "");

                if (extDetails != null)
                {
                    if (extDetails.Any())
                    {
                        searchSchool.SchoolExtDetails = extDetails;
                    }
                }

                var auditDetail = JsonConvert.DeserializeObject<List<AuditDetail>>(x.AuditDetail ?? "");
                if (auditDetail != null)
                {
                    searchSchool.Modifier = auditDetail.FirstOrDefault().Modifier;
                    searchSchool.Status = auditDetail.FirstOrDefault().Status;
                }
                schoolImports.Add(searchSchool);
            });

            return schoolImports;
        }
    }
}
