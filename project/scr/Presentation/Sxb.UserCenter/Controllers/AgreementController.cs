using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class AgreementController : Base
    {

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult TermsOfService()
        {
            return View();
        }
        public IActionResult Disclaimer()
        {
            return View();
        }
        public IActionResult PrivilegeUser()
        {
            return View();
        }
        public IActionResult ContentAuthorization()
        {
            return View();
        }
    }
}