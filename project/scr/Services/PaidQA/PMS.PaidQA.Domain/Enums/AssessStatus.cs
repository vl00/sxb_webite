using System.ComponentModel;

namespace PMS.PaidQA.Domain.Enums
{
    public enum AssessStatus
    {
        Unknow = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Processing = 1,
        /// <summary>
        /// 已结束
        /// </summary>
        [Description("已结束")]
        Finish = 2
    }
}
