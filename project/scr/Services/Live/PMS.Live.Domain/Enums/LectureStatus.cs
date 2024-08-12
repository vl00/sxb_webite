using System.ComponentModel;

namespace PMS.Live.Domain.Enums
{
    /// <summary>
    /// 直播状态
    /// <para>
    /// 0待审核 1审核中  2未开课   3待开课  4开课中   5已结束  6 审核失败
    /// </para>
    /// </summary>
    public enum LectureStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitingVerify = 0,
        /// <summary>
        /// 审核中
        /// </summary>
        [Description("审核中")]
        Verifying = 1,
        /// <summary>
        /// 未开课
        /// </summary>
        [Description("未开课")]
        NotLiving = 2,
        /// <summary>
        /// 待开课
        /// </summary>
        [Description("待开课")]
        WaitingLiving = 3,
        /// <summary>
        /// 开课中
        /// </summary>
        [Description("开课中")]
        Living = 4,
        /// <summary>
        /// 已结束
        /// </summary>
        [Description("已结束")]
        LiveEnded = 5,
        /// <summary>
        /// 审核失败
        /// </summary>
        [Description("审核失败")]
        VerifyFail = 6
    }
}
