using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.Infrastructure.Domain.Enums
{
    public enum SxbClientType
    {
        [DefaultValue("Unknow")]
        Unknow,
        [DefaultValue("H5")]
        H5,
        [DefaultValue("PC")]
        PC
    }
}
