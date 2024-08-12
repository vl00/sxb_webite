﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class CooperationController : Base
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}