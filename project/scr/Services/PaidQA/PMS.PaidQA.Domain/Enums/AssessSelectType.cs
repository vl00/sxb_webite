using System.ComponentModel;

namespace PMS.PaidQA.Domain.Enums
{
    public enum AssessSelectType
    {
        Unknow = 0,
        /// <summary>
        /// 单选
        /// </summary>
        [Description("单选")]
        RadioBox = 1,
        /// <summary>
        /// 多选
        /// </summary>
        [Description("多选")]
        CheckBox = 2,
        /// <summary>
        /// 填空
        /// </summary>
        [Description("填空")]
        TextBox = 3
    }
}
