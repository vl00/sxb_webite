﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Models.SchoolActivity
{
    public class SignInRequest
    {

        public Guid ActivityId { get; set; }

        public string Mobile { get; set; }
    }
}
