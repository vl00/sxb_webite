using PMS.School.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PMS.School.Domain.Enum
{
    /// <summary>
    /// 科目枚举
    /// </summary>
    public enum SubjectEnum
    {
        /// <summary>
        /// 语文
        /// </summary>
        [Description("语文")]
        [Contact( ContactAttributeType = ContactAttributeType.TalentRegion,Value = "杯赛考证")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "语文阅读")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "语文")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "国学类")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        Chinese = 101,
        /// <summary>
        /// 数学
        /// </summary>
        [Description("数学")] 
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "杯赛考证")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "数学思维")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "数学")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "奥数")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        Math = 102,
        /// <summary>
        /// 英语
        /// </summary>
        [Description("英语")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolCourseSetting, Value = "国际课程")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "留学备考")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "出国准备")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "国外生活")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "杯赛考证")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "英语培养")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "英语")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        English = 103,
        /// <summary>
        /// Steam
        /// </summary>
        [Description("Steam")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "科创特长")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "Steam")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "信息工程")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "科技")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "计算机")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "化学")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "物理")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "机器人课程")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "人工智能")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "编程")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        Steam = 104,
        /// <summary>
        /// 绘画
        /// </summary>
        [Description("绘画")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "艺术特长")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "绘本阅读")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "绘画")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "美术")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "美学")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "绘本")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "音乐类")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        Draw = 105,
        /// <summary>
        /// 音乐
        /// </summary>
        [Description("音乐")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "艺术特长")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "绘本阅读")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "音乐")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "声乐")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        Music = 106,
        /// <summary>
        /// 思维
        /// </summary>
        [Description("思维")]
        [Contact(ContactAttributeType = ContactAttributeType.TalentRegion, Value = "素质培养")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        Thought = 107,
        /// <summary>
        /// 编程
        /// </summary>
        [Description("编程")]
        Programming = 108,
        /// <summary>
        /// 科学
        /// </summary>
        [Description("科学")]
        Science = 109,
        /// <summary>
        /// 棋类
        /// </summary>
        [Description("棋类")]
        ChessAndCards = 110,
        /// <summary>
        /// 早教
        /// </summary>
        [Description("早教")]
        EarlyEducation = 111,
        /// <summary>
        /// 综合素养
        /// </summary>
        [Description("综合素养")]
        Enm112 = 112,
        /// <summary>
        /// 其他
        /// </summary>
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "体育")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolSpecialCourse, Value = "其他")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleType, Value = "all")]
        [Description("其他")]
        Other = 199
    }

    public static class SubjectEnumHelper {
       public static  IEnumerable<SubjectEnum> GetSubjectEnums(
           string articleType = null
           , IEnumerable<string> talentRegions = null
           , IEnumerable<string> specialCourses = null
           , IEnumerable<string> courseSettings = null)
        {
            List<SubjectEnum> subjectEnums = new List<SubjectEnum>();
            var _type = typeof(SubjectEnum);
            var fields = _type.GetFields();
            foreach (var field in fields)
            {
                var contacts = field.GetCustomAttributes(typeof(ContactAttribute), false)?.Select(o => (o as ContactAttribute));
                var contactGroups = contacts.GroupBy(o => (o.ContactAttributeType));
                if (contactGroups?.Any() == true)
                {
                    foreach (var contactGroup in contactGroups)
                    {
                        bool canAdd = false;
                        switch (contactGroup.Key)
                        {
                            case ContactAttributeType.TalentRegion:
                                if (talentRegions?.Any() == true)
                                {
                                    canAdd = contactGroup.Any(c => talentRegions.Any(t => c.Value.Equals(t, StringComparison.CurrentCultureIgnoreCase)));
                                }
                                break;
                            case ContactAttributeType.SchoolSpecialCourse:
                                if (specialCourses?.Any() == true)
                                {
                                    canAdd = contactGroup.Any(c => specialCourses.Any(t => c.Value.Equals(t, StringComparison.CurrentCultureIgnoreCase)));
                                }
                                break;
                            case ContactAttributeType.SchoolCourseSetting:
                                if (courseSettings?.Any() == true)
                                {
                                    canAdd = contactGroup.Any(c => courseSettings.Any(t => c.Value.Equals(t, StringComparison.CurrentCultureIgnoreCase)));
                                }
                                break;
                            case ContactAttributeType.ArticleType:
                                canAdd = contactGroup.Any(c => c.Value.Equals(articleType, StringComparison.CurrentCultureIgnoreCase));
                                break;
                            default:
                                break;
                        }
                        if (canAdd)
                        {
                            var subjectEnum = (SubjectEnum)field.GetValue(null);
                            subjectEnums.Add(subjectEnum);
                            break;
                        }
                    }
                }
            }
            return subjectEnums;
        }



    }


}
