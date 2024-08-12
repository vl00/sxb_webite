using Microsoft.Extensions.Options;
using Sxb.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.App_Start
{
    public class Tools
    {
        public ImageSetting _setting;
        public Tools(IOptions<ImageSetting> set)
        {
            _setting = set.Value;
        }

        public ImageSetting GetSetting()
        {
            return _setting;
        }

    }
}
