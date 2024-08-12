using Dapper.Contrib.Extensions;
using System;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table(nameof(CommonSetting))]
    public class CommonSetting
    {
        [ExplicitKey]
        public Guid ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
