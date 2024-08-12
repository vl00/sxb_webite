using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.Toolibrary;
using ProductManagement.Web.Areas.PartTimeJob.Models;
using ProductManagement.Framework.Foundation;
using PMS.CommentsManage.Domain.Common;
using PMS.School.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using PMS.CommentsManage.Application.Common;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Castle.Core.Logging;

namespace ProductManagement.Web.Areas.PartTimeJob.Controllers
{
    /// <summary>
    /// 审核
    /// </summary>
    [Area("PartTimeJob")]
    public class AdminExaminerController : BaseController
    {
        private IPartTimeJobAdminRolereService _adminRolereService;
        //审核记录
        IExaminerRecordService _recordService;
        //管理员
        IPartTimeJobAdminService _partTimeJobAdmin;
        //点评
        ISchoolCommentService _schoolComment;
        //审核统计
        IViewExaminerTotalService _viewExaminerTotal;
        //问答详情
        IQuestionsAnswersInfoService _questionsAnswersInfo;
        //问答审核
        IQuestionsAnswerExamineService _questionsAnswerExamine;
        //点评审核
        ISchoolCommentExaminerService _schoolCommentExaminer;
        //学校
        ISchoolService _schoolService;

        ISettlementAmountMoneyService _amountMoneyService;

        ISchoolInfoService _schoolInfoService;

        IUserService _userService;

        public AdminExaminerController(IUserService userService, 
            IPartTimeJobAdminService partTimeJobAdmin, ISchoolCommentService schoolComment, 
            IViewExaminerTotalService viewExaminerTotal, IQuestionsAnswersInfoService questionsAnswersInfo,
            IQuestionsAnswerExamineService questionsAnswerExamine, ISchoolCommentExaminerService schoolCommentExaminer, 
            ISchoolService schoolService, ISchoolInfoService schoolInfoService, ISettlementAmountMoneyService amountMoneyService,
            IPartTimeJobAdminRolereService adminRolereService,
            IExaminerRecordService recordService)
        {
            _userService = userService;
            _partTimeJobAdmin = partTimeJobAdmin;
            _schoolComment = schoolComment;
            _viewExaminerTotal = viewExaminerTotal;
            _questionsAnswersInfo = questionsAnswersInfo;
            _questionsAnswerExamine = questionsAnswerExamine;
            _schoolCommentExaminer = schoolCommentExaminer;
            _schoolService = schoolService;

            _amountMoneyService = amountMoneyService;
            _adminRolereService = adminRolereService;
            _schoolInfoService = schoolInfoService;
            _recordService = recordService;
        }

        public IActionResult Index()
        {
            Dictionary<string, decimal> dir = new Dictionary<string, decimal>();
            int currentRole = int.Parse(_admin.Role);
            dir.Add("role", currentRole);
            //检测当前账号类型【5：管理员 | 4：审核】
            if (currentRole == 5)
            {
                var examinerTotal = _viewExaminerTotal.GetViewExaminerTotal();
                dir.Add("TotalExaminerAdmin", examinerTotal.TotalExaminer);
                dir.Add("ExaminerCommentTotal", examinerTotal.ExaminerCommentTotal);
                dir.Add("ExaminerAnswerTotal", examinerTotal.ExaminerAnswerTotal);
            }
            else
            {
                var examinerTotal = _recordService.GetCurrentExaminerCount(_admin.Id);

                dir.Add("ToatalComment", examinerTotal.SuccessCommentTotal);
                dir.Add("ToatalAnswer", examinerTotal.SuccessSchoolAnswerTotal);
                dir.Add("NoExaminerComment", examinerTotal.WaitSchoolCommentTotal);
                dir.Add("NoExaminerAnswer", examinerTotal.WaitSchoolAnswerTotal);
                dir.Add("ExaminerSelectedCommentTotal", examinerTotal.ExaminerSelectedCommentTotal);
                dir.Add("ExaminerSelectedAnswerTotal", examinerTotal.ExaminerSelectedAnswerTotal);
                dir.Add("ExaminerSelectedTotal",examinerTotal.ExaminerSelectedTotal);
            }
            ViewBag.Examiner = dir;
            return View();
        }

        public ModelResult<Dictionary<string, decimal>> CurrentAdminExaminer()
        {
            Dictionary<string, decimal> dir = new Dictionary<string, decimal>();
            //var admin = _partTimeJobAdmin.GetModelById(_admin.Id);
            ////该审核人员问答审核
            //_questionsAnswerExamine.GetAnswerInfoByAdminId(admin.Id, 1, 1, new List<Guid>(), out int examinerAnswer);
            ////该审核人员的点评审核
            //_schoolCommentExaminer.GetSchoolCommentByAdminId(admin.Id, 1, 1, new List<Guid>(), out int examinerComment);
            //ExaminerStatistics commentStat = _schoolComment.GetExaminerStatistics();
            //ExaminerStatistics answerStat = _questionsAnswersInfo.GetExaminerStatistics();

            ////未审核的点评
            //int noExaminerComment = commentStat.NoExaminerTotal;
            ////未审核的问答
            //int noExaminerAnswer = answerStat.NoExaminerTotal;
            var examinerTotal = _recordService.GetCurrentExaminerCount(_admin.Id);

            dir.Add("ToatalComment", examinerTotal.SuccessCommentTotal);
            dir.Add("ToatalAnswer", examinerTotal.SuccessSchoolAnswerTotal);
            dir.Add("NoExaminerComment", examinerTotal.WaitSchoolAnswerTotal);
            dir.Add("NoExaminerAnswer", examinerTotal.WaitSchoolAnswerTotal);
            dir.Add("ExaminerSelectedCommentTotal", examinerTotal.ExaminerSelectedCommentTotal);
            dir.Add("ExaminerSelectedAnswerTotal", examinerTotal.ExaminerSelectedAnswerTotal);
            dir.Add("ExaminerSelectedTotal", examinerTotal.ExaminerSelectedTotal);
            return new ModelResult<Dictionary<string, decimal>>()
            {
                Data = dir
            };
        }

        /// <summary>
        /// 根据点评状态获取该审核人的所有点评，未阅的点评是所有的审核人可见的
        /// </summary>
        /// <param name="AdminId"></param>
        /// <param name="type">0未审核，1：该管理员的成功审核</param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public PageResult<List<ExaminerCommentVo>> GetCurrentExaminerAdminComment(Guid AdminId,int type, int limit, int page,int status,int SettlementType, string phone = "")
        {
            List<Guid> AdminIds = new List<Guid>();

            if (phone != null) 
            {
                phone = phone.Replace("****", "____");
                //根据电话号码获取管理员信息
                var PartTimeJob = _partTimeJobAdmin.GetAdminIdByPhone(phone);

                if (PartTimeJob.Any())
                {
                    AdminIds = PartTimeJob.Select(x => x.Id).ToList();
                }
            }


            if (SettlementType != 0) 
            {

                if (type == -1)
                {
                    //屏蔽
                    AdminIds.AddRange(_partTimeJobAdmin.GetList(x => x.SettlementType == (PMS.CommentsManage.Domain.Common.SettlementType)SettlementType && x.PartTimeJobAdminRoles.Where(r => r.Role == 1).FirstOrDefault().Shield == true).Select(x => x.Id).ToList());
                }
                else 
                {
                    AdminIds.AddRange(_partTimeJobAdmin.GetList(x => x.SettlementType == (PMS.CommentsManage.Domain.Common.SettlementType)SettlementType && x.PartTimeJobAdminRoles.Where(r=>r.Role == 1).FirstOrDefault().Shield == false).Select(x => x.Id).ToList());
                }
            }

            List<ExaminerCommentVo> examiners = new List<ExaminerCommentVo>();
            int total = 0;
            if (type == 1)
            {
                var ExaminerAdmin = _partTimeJobAdmin.GetModelById(AdminId);
                var allExaminers = _schoolCommentExaminer.GetSchoolCommentByAdminId(ExaminerAdmin.Id, page, limit,AdminIds, out total);

                if (allExaminers != null)
                {

                    var userInfo = _userService.ListUserInfo(allExaminers.Select(x => x.SchoolComment.CommentUserId).ToList());

                    var schoolInfos = _schoolInfoService.GetSchoolName(allExaminers.GroupBy(x => x.SchoolComment.SchoolSectionId).Select(x => x.Key).ToList());

                    foreach (SchoolCommentExamine item in allExaminers)
                    {
                        ExaminerCommentVo examinerCommentVo = new ExaminerCommentVo();
                        examinerCommentVo.id = item.SchoolCommentId;
                        //手机号掩码
                        examinerCommentVo.Phone = Regex.Replace(_partTimeJobAdmin.GetModelById(item.SchoolComment.CommentUserId).Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                        examinerCommentVo.School = schoolInfos.Where(x=>x.SchoolSectionId == item.SchoolComment.SchoolSectionId).FirstOrDefault().SchoolName;
                        examinerCommentVo.Status = item.SchoolComment.State.Description();
                        examinerCommentVo.ExaminerTime = item.AddTime?.ToString("yyyy-MM-dd");
                        examinerCommentVo.ExaminerAdminNaem = _partTimeJobAdmin.GetModelById(item.AdminId).Name;
                        examinerCommentVo.CommentCotent = item.SchoolComment.Content;
                        
                        examinerCommentVo.CommentWriteAdmin = userInfo.Find(x=>x.Id == item.SchoolComment.CommentUserId).NickName;
                        examiners.Add(examinerCommentVo);
                    }
                }
            }
            else if(type == 0 || type == -1)
            {
                if(type == -1 && !AdminIds.Any()) 
                {
                    AdminIds.AddRange(_adminRolereService.GetList(x => x.Shield == true && x.Role == 1).Select(x => x.AdminId).ToList());
                }
                
                var allExaminers = _schoolComment.QueryAllExaminers(AdminIds, page, limit, out total);

                if (allExaminers != null)
                {
                    var schoolInfos = _schoolInfoService.GetSchoolName(allExaminers.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList());

                    var userInfo = _userService.ListUserInfo(allExaminers.Select(x => x.CommentUserId).ToList());
                    foreach (SchoolComment item in allExaminers)
                    {
                        ExaminerCommentVo examinerCommentVo = new ExaminerCommentVo();
                        examinerCommentVo.id = item.Id;
                        examinerCommentVo.CommentCotent = item.Content;
                        examinerCommentVo.School = schoolInfos.Where(x=>x.SchoolSectionId == item.SchoolSectionId).FirstOrDefault().SchoolName;
                        examinerCommentVo.Status = item.State.Description();
                        examinerCommentVo.Phone = Regex.Replace(_partTimeJobAdmin.GetModelById(item.CommentUserId).Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                        examinerCommentVo.CommentWriteAdmin = userInfo.Find(x => x.Id == item.CommentUserId).NickName;
                        examiners.Add(examinerCommentVo);
                    }
                }
            }
            if (int.Parse(_admin.Role) != 5)
            {
                examiners.ForEach(x=>x.Phone = Regex.Replace(x.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2"));
            }
            return new PageResult<List<ExaminerCommentVo>>()
            {
                total = total,
                rows = examiners
            };
        }

        /// <summary>
        /// 获取所有的审核人员信息
        /// </summary>
        /// <returns></returns>
        public PageResult<List<AllExaminerAdminInfoVo>> GetAllExaminerAdminInfo(int limit, int page)
        {
            var allExaimer =   _partTimeJobAdmin.GetList(x => (int)x.Role == 4).Skip((page - 1) * limit).Take(limit).ToList();
            var total = _partTimeJobAdmin.GetList(x => (int)x.Role == 4).Count();
            List<AllExaminerAdminInfoVo> result = new List<AllExaminerAdminInfoVo>();
            foreach (PartTimeJobAdmin item in allExaimer)
            {
                AllExaminerAdminInfoVo model = new AllExaminerAdminInfoVo();
                model.Id = item.Id;
                model.Name = item.Name;
                model.Phone = Regex.Replace(item.Phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                model.SettlementType = item.SettlementType.Description();

                int commentExamin = 0, answerExamin = 0;
                
                //获取该审核，总点评审核数
                _schoolCommentExaminer.GetSchoolCommentByAdminId(item.Id, 1, 1,new List<Guid>(), out commentExamin);
                model.ExaminerCommentTotal = commentExamin;

                //获取该审核总审核问答数
                _questionsAnswerExamine.GetAnswerInfoByAdminId(item.Id, 1, 1, new List<Guid>(), out answerExamin);
                model.ExaminerAnswerTotal = answerExamin;

                result.Add(model);
            }

            return new PageResult<List<AllExaminerAdminInfoVo>>() {
                rows = result,
                total = total
            };
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="examinerType">审核类型，1：点评，2：问答</param>
        /// <param name="targetId">审核目标id</param>
        /// <param name="status">审核状态</param>
        /// <returns></returns>
        public ModelResult<bool> DataExaminer(int examinerType,Guid targetId,int status)
        {
            List<object> list = new List<object>();
            bool rez;

            if (examinerType == 1)
            {
                rez = _schoolCommentExaminer.ExecExaminer(_admin.Id, targetId, status);
            }
            else
            {
                rez = _questionsAnswerExamine.ExecExaminer(_admin.Id, targetId, status);
            }

            if (rez) 
            {
                Guid ReceiveId = Guid.Empty;
                int total = 0;
                bool IsComplete = false;


                DateTime write;
                if(examinerType == 1) 
                {
                    var comment = _schoolComment.QueryComment(targetId);
                    ReceiveId = comment.UserId;
                    write = comment.CreateTime;
                }
                else 
                {
                    var answer =  _questionsAnswersInfo.GetListDto(x => x.Id == targetId).FirstOrDefault();
                    ReceiveId = answer.UserId;
                    write = DateTime.Parse(answer.AddTime);
                }

                //检测是否有发送过两次，每个账号最多有一个任务周期
                var Job = _amountMoneyService.GetNewSettlement(ReceiveId, write);

                if (Job != null) 
                {
                    //检测短信推送
                    if (status == 3)
                    {
                        total = Job.TotalAnswerSelected + Job.TotalSchoolCommentsSelected;
                        IsComplete = true;
                    }
                    else if (status == 4 || status == 2)
                    {
                        total = _recordService.ExaminerTotal(ReceiveId, false);
                        IsComplete = false;
                    }

                    //检测是否达到短信发送要求
                    bool IsSend = AdminSettlementFun.IsPushShortMessage(total);

                    if (IsSend)
                    {
                        var userInfo = _userService.GetUserInfo(ReceiveId);
                        var send = new TencentCloudSmsService();

                        if (IsComplete)
                        {
                            int SelectedTotal = Job.TotalAnswerSelected + Job.TotalSchoolCommentsSelected;
                            //只有精选5条或者10条才推送短信  
                            if (SelectedTotal == 5 || SelectedTotal == 10)
                            {
                                //如果小于十，代表该任务还没有完成过两次，可以进行短信推送
                                send.SendCode(true, long.Parse(userInfo.Mobile), new List<object>() { 5 });

                            }
                        }
                        else
                        {
                            send.SendCode(false, long.Parse(userInfo.Mobile), new List<object>() { 5 });
                        }
                    }
                }
            }

            return new ModelResult<bool>() {
                Data = rez,
                StatusCode = 200
            };
        }

        public ModelResult<Dictionary<string, int>> AllExaminerTotal()
        {
            Dictionary<string, int> dir = new Dictionary<string, int>();

            return new ModelResult<Dictionary<string, int>>() {
                Data = dir
            };
        }

        /// <summary>
        /// 得到该审核人员审核的问答详情
        /// </summary>
        /// <param name="AdminId"></param>
        /// <param name="type">0未审核，1：该管理员的成功审核</param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public PageResult<List<ExaminerAnswerVo>> GetCurrentExaminerAdminAnswer(Guid AdminId, int type, int limit, int page, int status,int SettlementType, string phone)
        {

            List<Guid> AdminIds = new List<Guid>();
            if (phone != null)
            {
                phone = phone.Replace("****", "____");
                //根据电话号码获取管理员信息
                var PartTimeJob = _partTimeJobAdmin.GetAdminIdByPhone(phone);

                if (PartTimeJob.Any())
                {
                    AdminIds = PartTimeJob.Select(x => x.Id).ToList();
                }
            }

            if (SettlementType != 0 && !AdminIds.Any())
            {
                AdminIds.AddRange(_partTimeJobAdmin.GetList(x => x.SettlementType == (PMS.CommentsManage.Domain.Common.SettlementType)SettlementType)?.Select(x => x.Id).ToList());
            }

            int total = 0;
            List<ExaminerAnswerVo> examiners = new List<ExaminerAnswerVo>();
            
            if (type == 1)
            {
                var ExaminerAdmin = _partTimeJobAdmin.GetModelById(AdminId);
                var allExaminers = _questionsAnswerExamine.GetAnswerInfoByAdminId(ExaminerAdmin.Id, page, limit,AdminIds, out total);

                if (allExaminers != null)
                {

                    var schoolInfos = _schoolInfoService.GetSchoolName(allExaminers.GroupBy(x => x.QuestionsAnswersInfo.QuestionInfo.SchoolSectionId).Select(x => x.Key).ToList());

                    var userInfo = _userService.ListUserInfo(allExaminers.Select(x => x.QuestionsAnswersInfo.UserId).ToList());

                    foreach (QuestionsAnswerExamine item in allExaminers)
                    {
                        ExaminerAnswerVo examinerCommentVo = new ExaminerAnswerVo();
                        examinerCommentVo.id = item.QuestionsAnswersInfo.Id;

                        string UserMobile = userInfo.Find(x => x.Id == item.QuestionsAnswersInfo.UserId).Mobile;
                        if(UserMobile != null && UserMobile != "")
                        {
                            //手机号掩码
                            examinerCommentVo.Phone = Regex.Replace(UserMobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                        }
                        examinerCommentVo.School = schoolInfos.Where(x=>x.SchoolSectionId == item.QuestionsAnswersInfo.QuestionInfo.SchoolSectionId).FirstOrDefault().SchoolName;
                        examinerCommentVo.Status = item.QuestionsAnswersInfo.State.Description();
                        examinerCommentVo.ExaminerTime = item.ExamineTime?.ToString("yyyy-MM-dd");
                        examinerCommentVo.ExaminerAdminNaem = _partTimeJobAdmin.GetModelById(item.AdminId).Name;
                        examinerCommentVo.AnswerCotent = item.QuestionsAnswersInfo.Content;
                        examinerCommentVo.Question = item.QuestionsAnswersInfo.QuestionInfo.Content;
                        examinerCommentVo.AnswerWriteName = userInfo.Find(x => x.Id == item.QuestionsAnswersInfo.UserId).NickName;
                        examiners.Add(examinerCommentVo);
                    }
                }
            }
            else if(type == 0 || type == -1)
            {
                List<QuestionsAnswersInfo> allAnswers = new List<QuestionsAnswersInfo>();

                if (type == 0)
                {
                    AdminIds.AddRange(_adminRolereService.GetList(x => x.Shield == false && x.Role == 1).Select(x => x.AdminId).ToList());
                }
                else if(type == -1 && !AdminIds.Any())
                {

                    AdminIds.AddRange(_adminRolereService.GetList(x => x.Shield == true && x.Role == 1).Select(x => x.AdminId).ToList());
                }
                
                if (!AdminIds.Any())
                {
                    total = _questionsAnswersInfo.GetList(x => (int)x.State == 1 && AdminIds.Contains(x.UserId) && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
                    allAnswers = _questionsAnswersInfo.GetList(x => (int)x.State == 1 && AdminIds.Contains(x.UserId) && x.PostUserRole == PMS.UserManage.Domain.Common.UserRole.JobMember && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderByDescending(x => x.CreateTime).Skip((page - 1) * limit).Take(limit).ToList();
                }
                else 
                {
                    total = _questionsAnswersInfo.GetList(x => (int)x.State == 1 && AdminIds.Contains(x.UserId) && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).Count();
                    allAnswers = _questionsAnswersInfo.GetList(x => (int)x.State == 1 && AdminIds.Contains(x.UserId) && x.CreateTime >= DateTime.Parse("2019-12-11 00:00:00")).OrderByDescending(x => x.CreateTime).Skip((page - 1) * limit).Take(limit).ToList();
                }

                if (allAnswers != null)
                {
                    var userInfo = _userService.ListUserInfo(allAnswers.Select(x => x.UserId).ToList());

                    var schoolInfos = _schoolInfoService.GetSchoolName(allAnswers.GroupBy(x => x.QuestionInfo.SchoolSectionId).Select(x => x.Key).ToList());

                    foreach (QuestionsAnswersInfo item in allAnswers)
                    {
                        ExaminerAnswerVo examinerCommentVo = new ExaminerAnswerVo();
                        examinerCommentVo.id = item.Id;
                        examinerCommentVo.AnswerCotent = item.Content;

                        string UserMobile = userInfo.Find(x => x.Id == item.UserId).Mobile;
                        if (UserMobile != null && UserMobile != "")
                        {
                            //手机号掩码
                            examinerCommentVo.Phone = Regex.Replace(UserMobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                        }

                        examinerCommentVo.Question = item.QuestionInfo.Content;
                        examinerCommentVo.School = schoolInfos.Where(x=>x.SchoolSectionId == item.QuestionInfo.SchoolSectionId).FirstOrDefault().SchoolName;
                        examinerCommentVo.Status = item.State.Description();
                        examinerCommentVo.AnswerWriteName = userInfo.Find(x => x.Id == item.UserId).NickName;
                        examiners.Add(examinerCommentVo);
                    }
                }
            }
            return new PageResult<List<ExaminerAnswerVo>>()
            {
                total = total,
                rows = examiners
            };
        }


    }

    public class TencentCloudSmsService
    {
        private static readonly HttpClient _httpClient =
            new HttpClient { BaseAddress = new Uri("https://yun.tim.qq.com") };
        private readonly string _appId = "1400013556";
        private readonly string _appKey = "ba4604e9bba557e6792c876c5e609df2";
        private const string SIGNATURE = "";
        private const int success = 493903;
        private const int fail = 484477;

        public async Task SendCode(bool isSuccess, long mobile, List<object> p)
        {
            List<object> param = p;
            var random = GetRandom();
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var data = new
            {
                tel = new { nationcode = 86, mobile = mobile.ToString() },
                sign = SIGNATURE,
                tpl_id = isSuccess ? success : fail,
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
            var stringContent = new StringContent(
                    JsonConvert.SerializeObject(value),
                    Encoding.UTF8,
                    "application/json");
            return await httpClient.PostAsync(requestUri, stringContent);
        }
    }
}