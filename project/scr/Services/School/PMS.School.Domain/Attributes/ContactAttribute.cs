using PMS.School.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Attributes
{

    [AttributeUsageAttribute(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class ContactAttribute:Attribute
    {

        public ContactAttributeType  ContactAttributeType { get; set; }

        public string Value { get; set; }


    }
}
