using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    /// <summary>
    /// 凤凰通会将电话号码+用户姓名+学校ID等报名信息传过来，我们把信息保存到指定活动里面就可以
    /// </summary>
    public class SchoolActicityUnCheckRegisterDto
    {
        /// <summary>
        /// 8ACA4465-5DDB-4765-BDDB-148E11D2FC07
        /// </summary>
        public string Key { get; set; }

        public Guid ActivityId => Guid.Parse("8ACA4465-5DDB-4765-BDDB-148E11D2FC07");
        //public string Phone { get; set; }
        //public string Name { get; set; }
        //public Guid? ExtId { get; set; }
        public List<Extension> Extensions { get; set; }

        public class Extension
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
