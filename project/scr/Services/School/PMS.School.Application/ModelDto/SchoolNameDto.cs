using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;

namespace PMS.School.Application.ModelDto
{
    public class SchoolNameDto
    {
        public Guid ExtId { get; set; }
   
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        public string ShortId { get; set; }
    }
}
