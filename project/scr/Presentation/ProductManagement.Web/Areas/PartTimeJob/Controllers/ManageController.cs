using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.UserManage.Application.IServices;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Web.Areas.PartTimeJob.Models;
using ProductManagement.Web.Authentication.Attribute;
using PMS.CommentsManage.Domain.Common;

namespace ProductManagement.Web.Areas.PartTimeJob.Controllers
{
    /// <summary>
    /// 管理员管理
    /// </summary>

    [Area("PartTimeJob")]
    public class ManageController : BaseController
    {
        //用户service
        private readonly IPartTimeJobAdminService timeJobAdminService;
        //开启结算任务
        private readonly ISettlementAmountMoneyService _amountMoneyService;
        //用户角色
        private IPartTimeJobAdminRolereService _adminRolereService;

        private readonly IUserService _userService;

        public ManageController(ISettlementAmountMoneyService amountMoneyService, IPartTimeJobAdminService partTimeJobAdmin, IUserService userService, IPartTimeJobAdminRolereService adminRolereService)
        {
            _amountMoneyService = amountMoneyService;
            timeJobAdminService = partTimeJobAdmin;
            _userService = userService;
            _adminRolereService = adminRolereService;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 账号管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Home()
        {
            if (int.Parse(_admin.Role) != 5)
            {
                HttpContext.Response.Redirect("/PartTimeJob/Home/Welcome");
            }
            return View();
        }

        /// <summary>
        /// 修改账号状态
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Admin]
        public ModelResult<bool> CheckStatus(Guid Id)
        {
            try
            {
                bool rez = timeJobAdminService.CheckStatus(Id);

                return new ModelResult<bool>()
                {
                    StatusCode = 200,
                    Data = rez
                };
            }
            catch (Exception ex)
            {
                return new ModelResult<bool>()
                {
                    StatusCode = 500,
                    Message = "失败："+ex.Message
                };
            }
        }

        /// <summary>
        /// 修改账号状态
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Admin]
        public ModelResult<bool> CheckShieldStatus(Guid Id)
        {
            try
            {
                bool rez = _adminRolereService.UpdateAdminShield(Id);

                return new ModelResult<bool>()
                {
                    StatusCode = 200,
                    Data = rez
                };
            }
            catch (Exception ex)
            {
                return new ModelResult<bool>()
                {
                    StatusCode = 500,
                    Message = "失败：" + ex.Message
                };
            }
        }

        [HttpGet]
        public PageResult<List<PartTimeJobAdminVo>> GetPartTimeJobAdminPage(int role,int limit,int page)
        {
            int count = 0;
            var data = Mapper.Map<List<PartTimeJobAdmin>, List<PartTimeJobAdminVo>>(timeJobAdminService.GetPartTimeJobAdminPage(role,page, limit, out count).ToList());
            
            return new PageResult<List<PartTimeJobAdminVo>>() {
                rows = data,
                total = count,
                StatusCode = 200
            };
        }

        /// <summary>
        /// 账号注册弹出层
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ModelResult<string> Register(PartTimeJobAdmin partTimeJobAdmin)
        {
            try
            {
                partTimeJobAdmin.Id = Guid.NewGuid();
                partTimeJobAdmin.RegesitTime = DateTime.Now;

                if (!timeJobAdminService.CheckPhoneExists(partTimeJobAdmin.Phone))
                {
                    //保存用户操作时新建的用户角色信息
                    AdminUserRole registerRole = partTimeJobAdmin.Role;

                    if (partTimeJobAdmin.Role == AdminUserRole.JobLeader || partTimeJobAdmin.Role == AdminUserRole.JobMember)
                    {
                        partTimeJobAdmin.SettlementType = (SettlementType)timeJobAdminService.GetSupplierSettlementType(partTimeJobAdmin.ParentId);

                        partTimeJobAdmin.Role = AdminUserRole.JobMember;
                    }

                    var user = _userService.GetUserInfo(partTimeJobAdmin.Phone);
                    if (user != null)
                    {
                        partTimeJobAdmin.Id = user.Id;
                    }

                    int rez = timeJobAdminService.Insert(partTimeJobAdmin);
                    if (rez > 0)
                    {
                        //添加用户角色
                        _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = partTimeJobAdmin.Id , Role = (int)registerRole,ParentId = partTimeJobAdmin.ParentId });

                        //检测添加后的账号身份是否可以开启一个结算任务

                        //再查一次
                        var Jobadmin = timeJobAdminService.GetModelById(partTimeJobAdmin.Id);
                        Jobadmin.PartTimeJobAdminRoles = _adminRolereService.GetList(x => x.AdminId == Jobadmin.Id).ToList();

                            //开启结算周期任务
                        _amountMoneyService.NextSettlementData(Jobadmin, (int)registerRole);

                            ////开启添加至User表
                            //if (partTimeJobAdmin.Role == AdminUserRole.JobMember && user == null)
                            //{
                            //{
                            //    _userService.InsertUserInfo(partTimeJobAdmin.Id,
                            //    partTimeJobAdmin.Name,
                            //    partTimeJobAdmin.Phone, "/images/AppComment/defaultUserHeadImage.png");
                            //}
                        

                        return new ModelResult<string>()
                        {
                            StatusCode = 200,
                            Message = "添加成功"
                        };
                    }
                    else
                    {
                        return new ModelResult<string>()
                        {
                            StatusCode = 500,
                            Message = "添加失败"
                        };
                    }
                }
                else
                {
                    return new ModelResult<string>()
                    {
                        StatusCode = 500,
                        Message = "电话号码已存在"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ModelResult<string>()
                {
                    StatusCode = 500,
                    Message = "异常错误："+ex.Message
                };
            }
        }

        /// <summary>
        /// 获取所有的供应商
        /// </summary>
        /// <returns></returns>
        public PageResult<List<PartTimeJobAdminVo>> GetAllSupplier()
        {
            List<PartTimeJobAdminVo> rez = Mapper.Map<List<PartTimeJobAdmin>,List<PartTimeJobAdminVo>>(timeJobAdminService.GetAllSupplierList());
            
            return new PageResult<List<PartTimeJobAdminVo>>() {
                StatusCode = 200,
                rows = rez,
                total=rez.Count
            };
        }

        /// <summary>
        /// 获取供应商下所有兼职领队
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public PageResult<List<PartTimeJobAdmin>> GetCurrentAllItem(Guid Id)
        {
            List<PartTimeJobAdmin> data = timeJobAdminService.GetCurrentAllItem(Id);

            int total = data == null ? 0 : data.Count();
            return new PageResult<List<PartTimeJobAdmin>>() {
                rows = data,
                TotalCount = total,
                StatusCode = 200
            };
        }

        /// <summary>
        /// 加载兼职领队账号信息
        /// </summary>
        /// <returns></returns>
        public PageResult<List<JobLeaderAccountVo>> GetAllJobLeader(int limit, int page)
        {
            int total = _adminRolereService.GetList(x => (int)x.Role == 2).Count();
            var jobLeaders = timeJobAdminService.GetPartTimeJobAdminByRole(2,"",page,5,out int totala);

            List<JobLeaderAccountVo> jobs = new List<JobLeaderAccountVo>();

            List<Guid> parentIds = jobLeaders.Select(x => x.ParentId)?.Distinct().ToList();
            List<PartTimeJobAdmin> parentAdmin = new List<PartTimeJobAdmin>();
            if (parentIds.Any()) 
            {
                parentAdmin = timeJobAdminService.GetJobAdminNameByIds(parentIds);
            }

            foreach (PartTimeJobAdmin item in jobLeaders)
            {
                JobLeaderAccountVo jobLeader = new JobLeaderAccountVo();
                jobLeader.Id = item.Id;
                jobLeader.Name = item.Name;
                if (int.Parse(_admin.Role) != 5)
                {
                    jobLeader.Phone = Regex.Replace(item.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                }
                else
                {
                    jobLeader.Phone = item.Phone;
                }
                jobLeader.RegisterTime = item.RegesitTime.ToString("yyyy-MM-dd");
                jobLeader.Supplier = parentAdmin.Where(x=>x.Id == item.ParentId).FirstOrDefault().Name;
                jobLeader.InvitationCode = item.InvitationCode;
                jobLeader.Prohibit = item.Prohibit;

                jobs.Add(jobLeader);
            }

            return new PageResult<List<JobLeaderAccountVo>>() {
                total = total,
                rows = jobs
            };
        }

        /// <summary>
        /// 加载所有兼职人员账号信息
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public PageResult<List<JobAccountVo>> GetAllJob(int limit, int page, string phone = "")
        {
            int total = 0;

            if (phone != null && phone != "")
            {
                if (phone.Length >= 11)
                {
                    phone = phone.Replace(phone.Trim().Substring(3, 4), "____");
                }
            }

            List<JobAccountVo> jobs = new List<JobAccountVo>();
            var data = timeJobAdminService.GetPartTimeJobAdminByRole(1, phone, page, 5,out total);
            //total = data.Count();
            if (!data.Any()) 
            {
                return new PageResult<List<JobAccountVo>>()
                {
                    total = total,
                    rows = jobs
                };
            }

            List<Guid> parentIds = data.Select(x => x.ParentId)?.Distinct().ToList();
            List<PartTimeJobAdmin> supParentAdmin = new List<PartTimeJobAdmin>();
            List<PartTimeJobAdmin> ledParentAdmin = new List<PartTimeJobAdmin>();
            if (parentIds.Any())
            {
                ledParentAdmin = timeJobAdminService.GetPartTimeJobAdminByIdRoles(parentIds,2);
            }

            if (ledParentAdmin.Any()) 
            {
                supParentAdmin = timeJobAdminService.GetPartTimeJobAdminByIdRoles(ledParentAdmin.Select(x => x.ParentId).Distinct().ToList(),3);
            }
            var JobAdminIds = data.Select(a => a.Id).ToList();
            var checkShield = _adminRolereService.GetList(x => JobAdminIds.Contains(x.AdminId) && x.Role == 1).Select(x=> new PartTimeJobAdminRole() { AdminId = x.AdminId , Shield = x.Shield } );

            foreach (PartTimeJobAdmin item in data)
            {
                JobAccountVo job = new JobAccountVo();
                job.Id = item.Id;
                job.Name = item.Name;
                if (int.Parse(_admin.Role) != 5)
                {
                    job.Phone = Regex.Replace(item.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                }
                else
                {
                    job.Phone = item.Phone;
                }
                job.RegisterTime = item.RegesitTime.ToString("yyyy-MM-dd");
                var leader = ledParentAdmin.Where(x => x.Id == item.ParentId).FirstOrDefault();
                job.Leader = leader.Name;

                var leaderJob = ledParentAdmin.Where(x => x.Id == item.Id).FirstOrDefault();
                if (leaderJob == null)
                {
                    job.Supplier = supParentAdmin.Where(x => x.Id == leader.ParentId).FirstOrDefault().Name;
                }
                else 
                {
                    job.Supplier = supParentAdmin.Where(x => x.Id == leaderJob.ParentId).FirstOrDefault().Name;
                }
                
                job.InvitationCode = item.InvitationCode;

                job.Shield = checkShield.Where(x => x.AdminId == item.Id).FirstOrDefault().Shield;
                jobs.Add(job);
            }

            return new PageResult<List<JobAccountVo>>() {
                total = total,
                rows = jobs
            };
        }

    }
}