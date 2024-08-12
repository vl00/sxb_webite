using System.ComponentModel;

namespace PMS.PaidQA.Domain.Enums
{
    public enum AssessType
    {
        Unknow = 0,
        /// <summary>
        /// 获取专属幼升小学位分析
        /// </summary>
        [Description("获取专属幼升小学位分析")]
        Type1 = 1,
        /// <summary>
        /// 10秒锁定梦中情校：民办小学版
        /// </summary>
        [Description("10秒锁定梦中情校：民办小学版")]
        Type2 = 2,
        /// <summary>
        /// 10秒锁定梦中情校：民办初中版
        /// </summary>
        [Description("10秒锁定梦中情校：民办初中版")]
        Type3 = 3,
        /// <summary>
        /// 10秒锁定梦中情校：国际高中版
        /// </summary>
        [Description("10秒锁定梦中情校：国际高中版")]
        Type4 = 4
    }
}
