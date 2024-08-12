using PMS.School.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.School.Domain.Enum
{
    /// <summary>
    /// 年龄范围
    /// </summary>
    public enum AgeRange
    {
        [Contact(ContactAttributeType = ContactAttributeType.TalentGrade, Value = "幼儿入园")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleGradeType, Value = "幼儿园")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolRecruitAgeRange, Value = "0-3")]
        R0_3,
        [Contact(ContactAttributeType = ContactAttributeType.TalentGrade, Value = "幼升小")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleGradeType, Value = "幼儿园")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolRecruitAgeRange, Value = "4-6")]
        R4_6,
        [Contact(ContactAttributeType = ContactAttributeType.TalentGrade, Value = "小升初")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleGradeType, Value = "小学")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolRecruitAgeRange, Value = "7-12")]
        R7_12,
        [Contact(ContactAttributeType = ContactAttributeType.TalentGrade, Value = "中考")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleGradeType, Value = "初中")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolRecruitAgeRange, Value = "13-15")]
        R13_15,
        [Contact(ContactAttributeType = ContactAttributeType.TalentGrade, Value = "高考")]
        [Contact(ContactAttributeType = ContactAttributeType.ArticleGradeType, Value = "高中")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolRecruitAgeRange, Value = "16-18")]
        R16_18,
        
    }

    public static class AgeRangeHelper
    {
        public static (int minage, int maxage) GetAgeRange(this AgeRange ageRange)
        {
            var name = ageRange.ToString();
            var agerange = name.Substring(1).Split(new[] { '_' },StringSplitOptions.RemoveEmptyEntries);
            return (int.Parse(agerange[0]), int.Parse(agerange[1]));
        }

       public static IEnumerable<AgeRange> GetAgeRanges(
             IEnumerable<string> talentGrades = null
            , IEnumerable<string> articleGradeTypes = null
            , IEnumerable<(byte minage, byte maxage)> ages = null)
        {
            List<AgeRange> ageRanges = new List<AgeRange>();
            var _type = typeof(AgeRange);
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
                            case ContactAttributeType.TalentGrade:
                                if (talentGrades?.Any() == true)
                                {
                                    canAdd = contactGroup.Any(c => talentGrades.Any(t => c.Value.Equals(t, StringComparison.CurrentCultureIgnoreCase)));
                                }
                                break;
                            case ContactAttributeType.ArticleGradeType:
                                if (articleGradeTypes?.Any() == true)
                                {
                                    canAdd = contactGroup.Any(c => articleGradeTypes.Any(t => c.Value.Equals(t, StringComparison.CurrentCultureIgnoreCase)));
                                }
                                break;
                            case ContactAttributeType.SchoolRecruitAgeRange:
                                if (ages?.Any() == true)
                                {

                                   IEnumerable<(int minage,int maxage)> ageconditions =  contactGroup.Select(c => {
                                        var arr = c.Value.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                                        return ( int.Parse(arr[0]) , int.Parse(arr[1]));
                                    });
                                    if (ageconditions?.Any() == true)
                                    {
                                        canAdd = ageconditions.Any(condition => ages.Any(age =>
                                             (age.minage >= condition.minage && age.minage <= condition.maxage)
                                             ||
                                             (age.maxage >= condition.minage && age.maxage <= condition.maxage)
                                        ));
                                    }
                                }
                                break;
                        }
                        if (canAdd)
                        {
                            var ageRange = (AgeRange)field.GetValue(null);
                            ageRanges.Add(ageRange);
                            break;
                        }
                    }
                }
            }
            return ageRanges;
        }
    }
}
