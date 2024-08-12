using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.School.Domain.Enum
{
    public enum SchoolExtFiledYearTag
    {
        [Description("招生年龄段起始")]
        Age,
        [Description("招生年龄段结尾")]
        MaxAge,
        [Description("招生人数")]
        Count,
        [Description("报名所需资料")]
        Data,
        [Description("报名方式")]
        Contact,
        [Description("奖学金计划")]
        Scholarship,
        [Description("招生对象")]
        Target,
        [Description("考试科目")]
        Subjects,
        [Description("往期考试内容")]
        Pastexam,
        [Description("学校划片范围")]
        Range,
        [Description("申请费用")]
        Applicationfee,
        [Description("学费")]
        Tuition
    }
    
}
