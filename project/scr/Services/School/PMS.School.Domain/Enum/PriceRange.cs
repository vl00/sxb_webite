using PMS.School.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.School.Domain.Enum
{
    public enum PriceRange
    {
        Default = 0,
        [Contact(ContactAttributeType = ContactAttributeType.SchoolType, Value = "国际")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolType, Value = "民办")]
        [Contact(ContactAttributeType = ContactAttributeType.SchoolType, Value = "外籍")]
        Over100 =1,
        [Contact(ContactAttributeType = ContactAttributeType.SchoolType, Value = "其它")]
        Under100 = 2,
    }

    public static class PriceRangeHelper {

        public static PriceRange GetPrice(
            List<string> schoolTypes = null
            )
        {
            var _type = typeof(PriceRange);
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
                            case ContactAttributeType.SchoolType:
                                canAdd = contactGroup.Any(c => schoolTypes.Any(t => c.Value.Equals(t, StringComparison.CurrentCultureIgnoreCase)));
                                break;
                        }
                        if (canAdd)
                        {
                            var priceRange = (PriceRange)field.GetValue(null);
                            return priceRange;
                        }
                    }
                }
            }
            return  PriceRange.Default;
        }

    }
}
