using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Web.Areas.PartTimeJob.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using ProductManagement.Web.Authentication;
using ProductManagement.Web.Authentication.Attribute;
using IdentityModel;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;

namespace ProductManagement.Web.Areas.PartTimeJob.Controllers
{
    /// <summary>
    /// 公用
    /// </summary>

    [Area("PartTimeJob")]
    public class HomeController : BaseController
    {
        //用户service
        IPartTimeJobAdminService timeJobAdminService;

        public HomeController(IPartTimeJobAdminService partTimeJobAdmin)
        {
            timeJobAdminService = partTimeJobAdmin;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ModelResult<string>> Login(string phone,string pwd)
        {
            PartTimeJobAdmin admin = timeJobAdminService.Login(phone, pwd);
            if (admin != null)
            {
                _admin = Mapper.Map<PartTimeJobAdmin, PartTimeJobAdminVo>(admin);

                //创建身份：多个属性构建成一个身份【身份信息】
                var claims = new List<Claim>() {
                    new Claim(JwtClaimTypes.Name,_admin.Name),
                    new Claim(JwtClaimTypes.Id,_admin.Id.ToString()),
                    new Claim(JwtClaimTypes.Role,((int)admin.Role).ToString()),
                    new Claim(JwtClaimTypes.PhoneNumber,_admin.Phone),
                    new Claim(JwtClaimTypes.Subject,_admin.InvitationCode)
                    };

                //创建票证 
                var identity = new ClaimsIdentity("Cookie", JwtClaimTypes.Name, JwtClaimTypes.Role);
                identity.AddClaims(claims);
                //身份持有者
                var princiapl = new ClaimsPrincipal(identity);

                //配置用户票据是否安全，过期时间、是否持久化连接
                var properties = new AuthenticationProperties();
                var titck = new AuthenticationTicket(princiapl, properties,"CookieScheme");
                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, princiapl);

                int statusCode = 0;
                //第一次登录邀请用户重置密码
                if (timeJobAdminService.CheckResetPassword(_admin.Id))
                {
                    statusCode = 2;
                }
                else
                {
                    statusCode = 1;
                }
                return new ModelResult<string>() { StatusCode = statusCode, Message = "登录成功" };
            }
            else
            {
                return new ModelResult<string>() { StatusCode = -1,Message="账号或密码错误" };
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// 当前账号的个人资料
        /// </summary>
        /// <returns></returns>
        [Admin]
        public IActionResult Self(bool isFirst)
        {
            //var a = User.Identity.IsAuthenticated;
            ViewBag.isFirst = isFirst;
            _admin.Password = timeJobAdminService.CheckResetPassword(_admin.Id) ? "********" : "";
            ViewBag._admin = _admin;
            return View();
        }


        /// <summary>
        /// 检测密码是否输入正确
        /// </summary>
        /// <param name="Pwd"></param>
        /// <returns></returns>
        public ModelResult<bool> CheckOldPassword(string Pwd)
        {
           bool rez = timeJobAdminService.CheckOldPassword(_admin.Id, Pwd);
            return new ModelResult<bool>() {
                StatusCode = 200,
                Data = rez
            };
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ModelResult<string> UpdatePassword(string oldPwd,string pwd)
        {
            var CheckOldPwd = CheckOldPassword(oldPwd);
            //检测原密码是否正确
            if (CheckOldPwd.Data)
            {
                //var model = timeJobAdminService.GetModelById(_admin.Id);
                //model.Password = pwd;
                //int rez = timeJobAdminService.Update(model);

                //将新密码写入缓存
                MemoryCacheService.SetChacheValue(_admin + "_newPwd", DesTool.Md5(pwd));

                return new ModelResult<string>()
                {
                    StatusCode = 200,
                    Data = "修改成功，请进行提交操作"
                };
            }
            else
            {
                return new ModelResult<string>()
                {
                    StatusCode = 201,
                    Data = "原始密码错误"
                };
            }
        }
       
        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ModelResult<string>> UpdateInfo(string name)
        {
            try
            {
                var model = timeJobAdminService.GetModelById(_admin.Id);

                //获取缓存中的密码，检测是否需要进行修改密码
                string newpwd = MemoryCacheService.Get<string>(_admin + "_newPwd");

                //需要进行修改密码操作
                if (newpwd != null)
                {
                    //在写入缓存时，已经进行过一次加密处理
                    model.Password = newpwd;
                }

                model.Name = name;
                int rez = timeJobAdminService.Update(model);

                MemoryCacheService.Remove(_admin + "_newPwd");

                if (rez > 0)
                {
                    //创建身份：多个属性构建成一个身份【身份信息】
                    var claims = new List<Claim>() {
                    new Claim(JwtClaimTypes.Name,name),
                    new Claim(JwtClaimTypes.Id,_admin.Id.ToString()),
                    new Claim(JwtClaimTypes.Role,(_admin.Role).ToString()),
                    new Claim(JwtClaimTypes.PhoneNumber,_admin.Phone),
                    new Claim(JwtClaimTypes.Subject,_admin.InvitationCode)
                    };

                    //创建票证 
                    var identity = new ClaimsIdentity("Cookie", JwtClaimTypes.Name, JwtClaimTypes.Role);
                    identity.AddClaims(claims);
                    //身份持有者
                    var princiapl = new ClaimsPrincipal(identity);

                    //配置用户票据是否安全，过期时间、是否持久化连接
                    var properties = new AuthenticationProperties();
                    var titck = new AuthenticationTicket(princiapl, properties, "CookieScheme");

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, princiapl);
                }

                return new ModelResult<string>()
                {
                    StatusCode = 200,
                    Data = "修改成功"
                };
            }
            catch (Exception ex)
            {
                return new ModelResult<string>()
                {
                    StatusCode = 500
                };
            }
        }

        /// <summary>
        /// 欢迎界面
        /// </summary>
        /// <returns></returns>
        [Admin]
        public IActionResult Welcome()
        {
            return View();
        }
    }
}