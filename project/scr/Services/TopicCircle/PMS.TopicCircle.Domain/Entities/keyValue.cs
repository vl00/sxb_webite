using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Domain.Entities
{
    [Table("keyValue")]
    public class keyValue
    {
        [ExplicitKey]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
