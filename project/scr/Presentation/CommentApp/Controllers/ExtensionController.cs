using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.CommentsManage.Application.IServices;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Response;
using PMS.CommentsManage.Domain.Entities;
using System.Net.Http.Headers;

namespace Sxb.Web.Controllers
{
    [Authorize]
    public class ExtensionController : Controller
    {
        private IPartTimeJobAdminService _partTimeJobAdmin;
        private ISettlementAmountMoneyService _amountMoneyService;
        private IUserService _userService;
        private IPartTimeJobAdminRolereService _adminRolereService;

        //private string JobLeader = "XZQEJ7O5";
        private Guid JobLeaderId = Guid.Parse("55117958-4c96-45de-84fe-4570145f4f2f");

        public ExtensionController(IPartTimeJobAdminService partTimeJobAdmin,
            ISettlementAmountMoneyService amountMoneyService,
            IPartTimeJobAdminRolereService adminRolereService,
            IUserService userService)
        {
            _partTimeJobAdmin = partTimeJobAdmin;
            _amountMoneyService = amountMoneyService;
            _userService = userService;

            _adminRolereService = adminRolereService;
        }

        public IActionResult Index()
        {

            var user = User.Identity.GetUserInfo();

            ViewBag.UserInfo = _userService.CheckIsBinding(user.UserId);
            //ViewBag.UserInfo = new PMS.UserManage.Application.ModelDto.CheckbindingUserDto() { OpenId = 0, Mobile= "" };
            return View();
        }

        /// <summary>
        /// 邀请注册
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [Description("邀请注册")]
        public ResponseResult Register(string code)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                if (_partTimeJobAdmin.GetModelById(user.UserId) != null)
                {
                    return ResponseResult.Success(new { isSuccess = 0 });
                }

                var admin = _partTimeJobAdmin.GetJobAdminByCode(code);
                if (admin == null)
                {
                    return ResponseResult.Success(new { isSuccess = -1 });
                }
                else
                {
                    int settlement = 0;
                    if (admin.Role == PMS.CommentsManage.Domain.Common.AdminUserRole.Supplier)
                    {
                        settlement = (int)admin.SettlementType;
                    }
                    else
                    {
                        int roleV = admin.PartTimeJobAdminRoles.Select(x => x.Role).OrderByDescending(x => x).FirstOrDefault();
                        //获取顶级父级信息
                        var topParent = _partTimeJobAdmin.GetTopParent(admin.Id, roleV);
                        settlement = (int)topParent.SettlementType;
                    }

                    //获取用户信息
                    var userInfo = _userService.GetUserInfo(user.UserId);
                    if (userInfo.Mobile != "" && userInfo.Mobile != null)
                    {
                        if (_partTimeJobAdmin.GetModelById(userInfo.Id) == null)
                        {
                            //检测当前用户的父级的身份
                            int parentRole = admin.PartTimeJobAdminRoles.OrderByDescending(x => x.Role).Select(x => x.Role).FirstOrDefault(); ;
                            //int parentRole = (int)admin.Role == 3 ? 2 : (int)admin.Role == 2 ? 1 : 0;
                            //自己本身角色
                            int selfRole = parentRole == 3 ? 2 : parentRole == 2 ? 1 : 1;

                            var userunion_weixin = _userService.CheckIsBinding(userInfo.Id);

                            var JobAdmin = new PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin()
                            {
                                Id = userInfo.Id,
                                Name = userunion_weixin.NickName,
                                Phone = userInfo.Mobile,
                                ParentId = admin.Id,
                                //SettlementType = (PMS.CommentsManage.Domain.Common.SettlementType)admin.SettlementType,
                                SettlementType = (PMS.CommentsManage.Domain.Common.SettlementType)settlement,
                                RegesitTime = DateTime.Now,
                                Role = PMS.CommentsManage.Domain.Common.AdminUserRole.JobMember,
                                Prohibit = false
                            };
                            int rez = _partTimeJobAdmin.Insert(JobAdmin);

                            //录入扩展用户角色信息【兼职用户】
                            _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = userInfo.Id, Role = selfRole, ParentId = admin.Id });

                            if (parentRole == 1 && settlement != 2)
                            {
                                //查出该兼职的顶级父级
                                var TopParent = _partTimeJobAdmin.GetTopParent(admin.Id, 1);

                                //如果使用的邀请码为兼职用户的码，则该兼职用户自动升级为领队用户
                                _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = admin.Id, Role = 2, ParentId = TopParent.Id });

                                //并且开启一个新的领队任务周期
                                _amountMoneyService.NextSettlementData(admin, 2);
                            }

                            if (rez > 0)
                            {
                                //开启结算周期
                                _amountMoneyService.NextSettlementData(JobAdmin, selfRole);
                            }

                            var InvitaCode = _partTimeJobAdmin.GetModelById(JobAdmin.Id);

                            if (settlement != 2)
                            {
                                //邀请码发送
                                _ = new TencentCloudSmsService().SendCode(long.Parse(InvitaCode.Phone), new List<object>() { '"' + InvitaCode.InvitationCode + '"' + "，此邀请码您可以邀请更多人参与本次活动，如有疑问可关注公众号（sxkidcn）" });
                            }

                            return ResponseResult.Success(new { isSuccess = 1 });
                        }
                        else
                        {
                            return ResponseResult.Success(new { isSuccess = 0 });
                        }
                    }
                    else
                    {
                        return ResponseResult.Success(new { isSuccess = 99 });
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Description("注册上学帮兼职管理")]
        public ResponseResult RegisterSxbJobAdmin()
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                //获取用户信息
                var userInfo = _userService.GetUserInfo(user.UserId);
                if (userInfo.Mobile != "" && userInfo.Mobile != null)
                {
                    if (_partTimeJobAdmin.GetModelById(userInfo.Id) == null)
                    {
                        var userunion_weixin = _userService.CheckIsBinding(userInfo.Id);
                        var admin = new PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin()
                        {
                            Id = userInfo.Id,
                            Name = userunion_weixin.NickName,
                            Phone = userInfo.Mobile,
                            ParentId = JobLeaderId,
                            SettlementType = PMS.CommentsManage.Domain.Common.SettlementType.SettlementWeChat,
                            RegesitTime = DateTime.Now,
                            Role = PMS.CommentsManage.Domain.Common.AdminUserRole.JobMember,
                            Prohibit = false
                        };
                        int rez = _partTimeJobAdmin.Insert(admin);

                        //录入扩展用户角色信息【兼职用户】
                        _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = admin.Id, Role = 1, ParentId = JobLeaderId });

                        if (rez > 0)
                        {
                            //开启结算周期
                            _amountMoneyService.NextSettlementData(admin, 1);
                        }

                        var InvitaCode = _partTimeJobAdmin.GetModelById(admin.Id);


                        //邀请码发送
                        _ = new TencentCloudSmsService().SendCode(long.Parse(InvitaCode.Phone), new List<object>() { '"' + InvitaCode.InvitationCode + '"' + "，此邀请码您可以邀请更多人参与本次活动，如有疑问可关注公众号（sxkidcn）" });

                        return ResponseResult.Success(new { isSuccess = 1 });
                    }
                    else
                    {
                        return ResponseResult.Success(new { isSuccess = 0 });
                    }
                }
                else
                {
                    return ResponseResult.Success(new { isSuccess = 99 });
                }
            }
            catch (Exception ex)
            {

                return ResponseResult.Failed(ex.Message);
            }
        }
    }

    public class TencentCloudSmsService
    {
        private static readonly HttpClient _httpClient =
            new HttpClient { BaseAddress = new Uri("https://yun.tim.qq.com") };
        private readonly string _appId = "1400013556";
        private readonly string _appKey = "ba4604e9bba557e6792c876c5e609df2";
        private const string SIGNATURE = "";
        private const int success = 493788;

        public async Task SendCode(long mobile, List<object> p)
        {
            List<object> param = p;
            var random = GetRandom();
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var data = new
            {
                tel = new { nationcode = 86, mobile = mobile.ToString() },
                sign = SIGNATURE,
                tpl_id = success,
                @params = param.ToArray(),
                sig = ComputeSignature(mobile, random, timestamp),
                time = timestamp,
                extend = "",
                ext = ""
            };

            var url = $"/v5/tlssmssvr/sendsms?sdkappid={_appId}&random={random}";
            var response = await _httpClient.PostAsJsonAsync<dynamic>(url, data);
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync();
        }

        private string ComputeSignature(long mobile, int random, long timestamp)
        {
            var input = $"appkey={_appKey}&random={random}&time={timestamp}&mobile={mobile}";
            var hasBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hasBytes.Select(b => b.ToString("x2")));
        }

        private int GetRandom()
        {
            return new Random().Next(100000, 999999);
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            string jsoncontent = JsonConvert.SerializeObject(value);
            var stringContent = new StringContent(jsoncontent,Encoding.UTF8,"application/json");
            return await httpClient.PostAsync(requestUri, stringContent);
        }

    }
}