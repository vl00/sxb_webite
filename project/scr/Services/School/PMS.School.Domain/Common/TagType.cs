using System;
using System.ComponentModel;

namespace PMS.School.Domain.Common
{
    public enum TagType
    {
        /// <summary>
        /// 办学类型
        /// </summary>
        [Description("办学类型")]
        RunSchool = 1,
        /// <summary>
        /// 学校认证
        /// </summary>
        [Description("学校认证")]
        SchoolAccre = 2,
        /// <summary>
        /// 出国方向
        /// </summary>
        [Description("出国方向")]
        Abroad = 3,
        /// <summary>
        /// 招生对象
        /// </summary>
        [Description("招生对象")]
        Recruit = 4,
        /// <summary>
        /// 考试科目
        /// </summary>
        [Description("考试科目")]
        Subject = 5,
        /// <summary>
        /// 课程设置
        /// </summary>
        [Description("课程设置")]
        CourseSet = 6,
        /// <summary>
        /// 课程认证
        /// </summary>
        [Description("课程认证")]
        CourseAccre = 7,
        /// <summary>
        /// 特色课程
        /// </summary>
        [Description("特色课程")]
        CharacteristicCourse = 8,
    }
}
