using System.ComponentModel;

namespace PMS.PaidQA.Domain.Enums
{
    public enum OrderTagType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        Unknow = 0,
        /// <summary>
        /// 领域
        /// </summary>
        [Description("领域")]
        Region = 1,
        /// <summary>
        /// 学段
        /// </summary>
        [Description("学段")]
        Grade = 2
    }
}
