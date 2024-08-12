using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ImportAndExport.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using PMS.CommentsManage.Application.IServices;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using ImportAndExport.Models.ImportViewModels;
using PMS.UserManage.Domain.Common;
using Microsoft.Extensions.Configuration;
using ProductManagement.Framework.MSSQLAccessor;
using ImportAndExport.Repository;
using ImportAndExport.Entity;
using System.Text.Encodings;
using ImportAndExport.Utils;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace ImportAndExport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionInfo;
        private readonly IQuestionsAnswersInfoService _questionsAnswers;
        private readonly ISchoolCommentScoreService _commentScoreService;

        CommentRepository commentRep;
        SchoolRepository schoolRepository;
        UserInfoRepository UserInfoRepository;
        int MaxTime = 2;

        int ImportType = 0;


        public HomeController(ISchoolCommentService commentService,
            IQuestionsAnswersInfoService questionsAnswers,
            IQuestionInfoService questionInfo,
            ISchoolCommentScoreService commentScoreService)
        {
            _commentService = commentService;
            _questionsAnswers = questionsAnswers;
            _questionInfo = questionInfo;
            _commentScoreService = commentScoreService;


            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot Configuration = builder.Build();

            var PGconfig = Configuration.GetSection("DbConnections").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var Manager = new ConnectionsManager<DataDbContext>(PGconfig);

            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<DBContext<DataDbContext>>();

            var PGconfig1 = Configuration.GetSection("DbConnections1").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var Manager1 = new ConnectionsManager<DataDbContext>(PGconfig1);

            var PGconfig2 = Configuration.GetSection("DbConnections2").Get<List<ConnectionConfig<DataDbContext>>>().FirstOrDefault();
            var Manager2 = new ConnectionsManager<DataDbContext>(PGconfig2);

            schoolRepository = new SchoolRepository(new DataDbContext(Manager1, loggerFactory.CreateLogger<DBContext<DataDbContext>>()));

            commentRep = new CommentRepository(new DataDbContext(Manager, loggerFactory.CreateLogger<DBContext<DataDbContext>>()), schoolRepository);

            UserInfoRepository = new UserInfoRepository(new DataDbContext(Manager2, loggerFactory.CreateLogger<DBContext<DataDbContext>>()));
        }

        public void PartJobAdmin() 
        {
            var Admins = schoolRepository.GetPartJobAdminIds();
            List<PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole> jobAdminRoles = new List<PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole>();
            List<PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin> admins = schoolRepository.GetPartTimeJobAdminRoles(Admins.Select(x => Guid.Parse(x.ParentId.ToString())).ToList());
            List<PMS.CommentsManage.Domain.Entities.SettlementAmountMoney> settlements = new List<PMS.CommentsManage.Domain.Entities.SettlementAmountMoney>();

            List<SelectedTotals> CommentSelectedTotals = schoolRepository.selectedTotals(Admins.Select(x => Guid.Parse(x.ParentId.ToString())).ToList());
            List<SelectedTotals> AnswerSelectedTotals = schoolRepository.QselectedTotals(Admins.Select(x => Guid.Parse(x.ParentId.ToString())).ToList());

            foreach (PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin item in Admins) 
            {
                PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole jobAdminRole = new PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole();

                var parentAdmin = admins.Where(x => item.ParentId == x.Id).FirstOrDefault();
                int role = (int)parentAdmin.Role == 3 ? 2 : (int)parentAdmin.Role == 2 ? 1 : 1;
                jobAdminRole.AdminId = item.Id;
                jobAdminRole.ParentId = item.ParentId;
                jobAdminRole.Role = role;
                jobAdminRole.CreateTime = item.RegesitTime;

                jobAdminRoles.Add(jobAdminRole);

                PMS.CommentsManage.Domain.Entities.SettlementAmountMoney settlement = new PMS.CommentsManage.Domain.Entities.SettlementAmountMoney();
                settlement.PartTimeJobAdminId = item.Id;
                settlement.PartJobRole = role;
                settlement.SettlementStatus = PMS.CommentsManage.Domain.Common.SettlementStatus.Ongoing;
                settlement.BeginTime = item.RegesitTime;
                settlement.EndTime = DateTime.Parse("2019-12-31 23:59:59");
                var C =  CommentSelectedTotals.Where(x => x.CommentUserId == item.Id).FirstOrDefault();
                settlement.TotalSchoolCommentsSelected = C == null ? 0 : C.SelectedTotal;
                var Q = AnswerSelectedTotals.Where(x => x.CommentUserId == item.Id).FirstOrDefault();
                settlement.TotalAnswerSelected = Q == null ? 0 : Q.SelectedTotal;
                settlement.AddTime = item.RegesitTime;

                settlements.Add(settlement);

            }
            
            schoolRepository.InsertPartAdminRole(jobAdminRoles,settlements);
        }

        public void SelectedTotal() 
        {
            var AllJob = schoolRepository.partTimeJobAdmins();
            var Settlement = schoolRepository.GetSettlementAmountMoney(AllJob.Select(x => x.Id).ToList());
            foreach (var item in AllJob)
            {
                if(item.Id == Guid.Parse("BF8DC5BE-299B-46A4-9AB1-5DE62821664D")) 
                {
                
                }
                int commentSelectedTotal = commentRep.GetCommentSelected(item.Id);
                int answerSelectedTotal = commentRep.GetAnswerSelected(item.Id);
                schoolRepository.UpdateSettlement(item.Id, commentSelectedTotal, answerSelectedTotal);
            }
        }

        /// <summary>
        /// 随机时间生成
        /// </summary>
        /// <param name="time">邀请日期</param>
        /// <returns></returns>
        public static DateTime GetRandomTime(DateTime time,int Day)
        {
            int d = 0, h = 0, m = 0, s = 0;

            Random random = new Random();

            int minDay = time.Day;
            int maxDay = time.AddDays(Day + 1).Day;

            if (minDay >= maxDay)
            {
                int a = 0;
            }

            d = random.Next(minDay, maxDay);
            if (d == time.Day)
            {
                if (time.Hour + 1 > 22)
                {
                    h = 23;
                }
                else
                {
                    h = random.Next(time.Hour + 1, 22);
                }
            }
            else 
            {
                h = random.Next(7, 22);
            }

            if (h == 23)
            {
                m = random.Next(0, 30);
            }
            else 
            {
                m = random.Next(0, 60);
            }

            s = random.Next(0, 60);

            DateTime newDateTime =  new DateTime(time.Year, time.Month,d,h,m,s);
            return newDateTime;
        }

        int currentPageIndex = 0;

        public int Change(int PageIndex) 
        {
            try
            {
                int total = commentRep.JobSelectedTotal();
                int PageSize = Convert.ToInt32(Math.Ceiling((total * 1.0) / 1000));

                for (int i = PageIndex; i <= PageSize; i++)
                {

                    currentPageIndex = i;
                    var comments = commentRep.GetSelectedComment(i);
                    List<JobStateChangeRecord> records = new List<JobStateChangeRecord>();
                    List<SchoolComment> comments1 = new List<SchoolComment>();
                    foreach (var item in comments)
                    {
                        var a = HttpHelper.connection<result>(item.Content, "http://129.204.154.70:8765/templatecheck").Result;
                        if (a != null)
                        {
                            if (a.istemplate)
                            {
                                JobStateChangeRecord jobState = new JobStateChangeRecord();
                                jobState.DataSourceId = item.Id;
                                jobState.Type = 1;
                                records.Add(jobState);

                                comments1.Add(item);
                            }
                        }
                    }
                    commentRep.InsertChangeRecord(records);
                    commentRep.UpdateState(comments1);
                }

                return currentPageIndex;
            }
            catch (Exception ex)
            {
                return Change(currentPageIndex);
            }
        }


        public void leaderSettlement() 
        {
            var allLeader = schoolRepository.getLeader();
            var allleaderSettlemnt = schoolRepository.GetSettlementAmountMoney(allLeader.Select(x => x.Id).ToList());
            foreach (var item in allLeader)
            {
                if(item.Id == Guid.Parse("E59BB9DC-200C-4F35-9A82-028C6069FC38")) 
                {
                
                }
                var set = schoolRepository.SettlementAmountMoneyByLeaderId(item.Id);
                //var sett = allleaderSettlemnt.Where(x => x.PartTimeJobAdminId == item.Id).FirstOrDefault();
                schoolRepository.UpdateSettlement(item.Id, set.TotalSchoolCommentsSelected, set.TotalAnswerSelected);
            }
        }

        public void RunSchoolScore(int pageIndex) 
        {
            int PageIndex = pageIndex;
            try
            {
                for (int i = PageIndex; i <= 852; i++)
                {
                    var Score = commentRep.GetSchoolComScores(i);

                    bool succes = commentRep.PushSchoolCommentSocre(Score);
                    if (succes)
                    {
                        Console.WriteLine($"评论分数统计第{PageIndex}成功");
                    }
                    PageIndex = i;
                }
            }
            catch (Exception)
            {
                RunSchoolScore(PageIndex);
            }
        }

        public void ChangeQuestionTotal(int pageIndex) 
        {
            int PageIndex = pageIndex;
            try
            {
                for (int i = PageIndex; i <= 81; i++)
                {
                    var score = commentRep.GetQuestionSchoolComScores(i);

                    var List = score.Where(x => !x.IsExist).ToList();
                    if (List.Any()) 
                    {
                        List<SchoolComScore> comScores = new List<SchoolComScore>();
                        foreach (var x in List)
                        {
                            comScores.Add(new SchoolComScore()
                            {
                                SchoolSectionId = x.SchoolSectionId,
                                SchoolId = x.SchoolId,
                                QuestionCount = x.QuestionCount,
                                LastQuestionTime = x.LastQuestionTime,
                                CreateTime = DateTime.Now,
                                AggScore = 0,
                                AttendCommentCount = 0,
                                CommentCount = 0,
                                EnvirScore = 0,
                                HardScore = 0,
                                LastCommentTime = DateTime.Parse("1/1/1753 12:00:00"),
                                LifeScore = 0,
                                ManageScore = 0,
                                TeachScore = 0,
                                UpdateTime = x.LastQuestionTime
                            });
                        }

                        commentRep.PushSchoolCommentSocre(comScores);
                    }

                    var Up = score.Where(x => x.IsExist).ToList();
                    if (Up.Any()) 
                    {
                        List<SchoolComScore> UpcomScores = new List<SchoolComScore>();
                        foreach (var x in Up)
                        {
                            UpcomScores.Add((new SchoolComScore
                            {
                                SchoolSectionId = x.SchoolSectionId,
                                QuestionCount = x.QuestionCount,
                                LastQuestionTime = x.LastQuestionTime
                            }));
                        }
                    }
                    

                    PageIndex = i;
                    Console.WriteLine($"第{i}页导入成功");
                }
            }
            catch (Exception ex)
            {
                ChangeQuestionTotal(PageIndex);
            }
        }

        public IActionResult Index()
        {
            //Change(1);
            //ChangeQuestionTotal(1);
            //RunSchoolScore(355);
            //for (int i = 1; i <= 4; i++)
            //{
            //    var answers = commentRep.UpdateQuestionAnswerTotal(i);
            //    if (answers.Count() > 0) 
            //    {
            //        foreach (var item in answers)
            //        {
            //            schoolRepository.UpdateQuestionReply(item.AnswerTotal, item.QuestionInfoId);
            //        }
            //    }
            //}

            //leaderSettlement();
            //Change(1);


            //PartJobAdmin();
            string filePath = @"D:\Upload\Read\六个六\数据异议版\1.xlsx";
            IWorkbook wk;

            using (FileStream file = System.IO.File.OpenRead(filePath))
            {
                //npoi 
                if (Path.GetExtension(filePath) == ".xls")
                {
                    wk = new HSSFWorkbook(file);
                }
                else
                {
                    wk = new XSSFWorkbook(file);
                }
            }

            List<ChangeCommentStatus> model = ExcelToList<ChangeCommentStatus>(wk.GetSheetAt(0), out string error);
            List<ChangeStatusRez> rez = new List<ChangeStatusRez>();
            foreach (var item in model)
            {
                item.Phone = item.Phone.Replace("****","____");
               var cm =  commentRep.GetCommentByPhoneContent(item.Phone, item.Content);
                if (cm != null)
                {
                    rez.Add(new ChangeStatusRez() { Id = cm.Id, Stauts = item.Status,contet = cm.Content });
                }
            }
            var c = rez.Select(y => y.contet).ToList();
            var a = model.Where(x => !c.Contains(x.Content)).ToList();

            //List<ChangeStatusRez> rez1 = new List<ChangeStatusRez>();
            //List<ChangeCommentStatus> succes = new List<ChangeCommentStatus>();

            //foreach (var item in a)
            //{
            //    item.Phone = item.Phone.Replace("****", "____");
            //    var cm = commentRep.GetCommentByPhoneContent(item.Phone, item.Content);
            //    if (cm != null)
            //    {
                    
            //        rez1.Add(new ChangeStatusRez() { Id = cm.Id, Stauts = item.Status, contet = cm.Content });
            //    }
            //    else 
            //    {
            //        succes.Add(item);
            //    }
            //}

            //rez1.Clear();
            //foreach (var item in succes)
            //{
            //    item.Phone = item.Phone.Replace("****", "____");
            //    var cm = commentRep.GetCommentByPhoneContent(item.Phone, item.Content);
            //    if (cm != null)
            //    {

            //        rez1.Add(new ChangeStatusRez() { Id = cm.Id, Stauts = item.Status, contet = cm.Content });
            //    }
            //}



            //List<Guid> userId = new List<Guid>();
            //foreach (var item in succes)
            //{
            //    userId.Add(commentRep.GetUserIdByPhone(item.Phone));
            //}

            //List<ChangeStatusRez> rez2 = new List<ChangeStatusRez>();
            //var com = commentRep.GetSchoolCommentByUserId(userId);
            //var coms = succes.Select(x => x.Content).ToList();
            //List<Guid> ids = new List<Guid>();

            //StringBuilder content = new StringBuilder();
            //foreach (var item in com)
            //{
            //    if (coms.Contains(item.Content))
            //    {
            //        //ids.Add(item.Id);
            //        rez2.Add(new ChangeStatusRez() { Id = item.Id, Stauts = 2 });
            //    }
            //    else 
            //    {
            //        content.Append(item.Content+"\n");
            //    }
            //}


            //using (FileStream fs = new FileStream(@"D:\info.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            //{
            //    StreamWriter sw = new StreamWriter(fs);
            //    sw.Write(content);
            //    sw.Flush();
            //    sw.Close();
            //    fs.Close();
            //}

            foreach (var item in rez)
            {
                commentRep.UpdateStatus(item);
            }

            //List<SchoolComment> comments = new List<SchoolComment>();
            //foreach (var item in model)
            //{
            //    item.Phone = item.Phone.Replace("****", "____");
            //    var com = commentRep.GetCommentByPhoneContent(item.Phone, item.Content);
            //    if (com != null) 
            //    {
            //        comments.Add(com);
            //    }
            //}

            //var cs = model.Select(y => y.Content).ToList();
            //var s = comments.Select(x => x.Content).ToList();

            //var n = cs.Where(x => !s.Contains(x)).ToList();

            //int i = 0;
            //foreach (var item in comments)
            //{
            //    i += commentRep.UpdateCommentState(item);
            //}
            //var Admins = schoolRepository.GetPartTimeJobAdmin(model.Select(x => x.Id).ToList());
            //List<PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole> jobAdminRoles = new List<PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole>();
            //List<PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin> admins = schoolRepository.GetPartTimeJobAdminRoles(Admins.Select(x => Guid.Parse(x.ParentId.ToString())).ToList());
            //List<PMS.CommentsManage.Domain.Entities.SettlementAmountMoney> settlements = new List<PMS.CommentsManage.Domain.Entities.SettlementAmountMoney>();

            //List<SelectedTotals> CommentSelectedTotals = schoolRepository.selectedTotals(Admins.Select(x => Guid.Parse(x.ParentId.ToString())).ToList());
            //List<SelectedTotals> AnswerSelectedTotals = schoolRepository.QselectedTotals(Admins.Select(x => Guid.Parse(x.ParentId.ToString())).ToList());

            //foreach (PMS.CommentsManage.Domain.Entities.PartTimeJobAdmin item in Admins)
            //{
            //    PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole jobAdminRole = new PMS.CommentsManage.Domain.Entities.PartTimeJobAdminRole();

            //    var parentAdmin = admins.Where(x => item.ParentId == x.Id).FirstOrDefault();
            //    if(parentAdmin == null) 
            //    {
            //        continue;
            //    }
            //    int role = (int)parentAdmin.Role == 3 ? 2 : (int)parentAdmin.Role == 2 ? 1 : 1;
            //    jobAdminRole.AdminId = item.Id;
            //    jobAdminRole.ParentId = item.ParentId;
            //    jobAdminRole.Role = role;
            //    jobAdminRole.CreateTime = item.RegesitTime;

            //    jobAdminRoles.Add(jobAdminRole);

            //    PMS.CommentsManage.Domain.Entities.SettlementAmountMoney settlement = new PMS.CommentsManage.Domain.Entities.SettlementAmountMoney();
            //    settlement.Id = Guid.NewGuid();
            //    settlement.PartTimeJobAdminId = item.Id;
            //    settlement.PartJobRole = role;
            //    settlement.SettlementStatus = PMS.CommentsManage.Domain.Common.SettlementStatus.Ongoing;
            //    settlement.BeginTime = item.RegesitTime;
            //    settlement.EndTime = DateTime.Parse("2019-12-31");
            //    var C = CommentSelectedTotals.Where(x => x.CommentUserId == item.Id).FirstOrDefault();
            //    settlement.TotalSchoolCommentsSelected = C == null ? 0 : C.SelectedTotal;
            //    var Q = AnswerSelectedTotals.Where(x => x.CommentUserId == item.Id).FirstOrDefault();
            //    settlement.TotalAnswerSelected = Q == null ? 0 : Q.SelectedTotal;
            //    settlement.AddTime = item.RegesitTime;

            //    settlements.Add(settlement);
            //}

            //schoolRepository.Insert(settlements);
            //commentRep.Delete(model.Select(x=>x.Id).ToList());
            //int a = 0;
            //string gu = "a6b0405781874fe493f2dd06c4993d5";
            //bool rez = Guid.TryParse(gu, out Guid g);
            //var t = UserInfoRepository.InviteAnswers(DateTime.Parse("2019-11-01 00:00:00"),DateTime.Parse("2019-11-10 23:59:59"));
            //DateTime dateTime = GetRandomTime(DateTime.Parse("2019-01-31 11:23:00"));

            //var t = commentRep.Test();

            //List<SchoolComment> comments = new List<SchoolComment>()
            //{
            //    new SchoolComment(){
            //        Id = Guid.Parse("DAA476B6-8E35-46AB-B4CB-008AA9541B04"),
            //        Content = "导入数据测试1111111",
            //        SchoolId = Guid.Parse("97952996-5E9B-406C-9BEF-6223A4C1F729"),
            //        SchoolSectionId = Guid.Parse("97952996-5E9B-406C-9BEF-6223A4C1F729"),
            //        CommentUserId = Guid.Parse("402E2D80-B7B7-4DAA-B381-FAB2A34CD372"),
            //        AddTime = DateTime.Now,
            //        PostUserRole = 0,
            //        CommentScore = new SchoolCommentScore()
            //        {
            //            CommentId = Guid.Parse("DAA476B6-8E35-46AB-B4CB-008AA9541B04"),
            //            AggScore = 100,
            //            IsAttend = true,
            //            TeachScore = 100,
            //            EnvirScore = 100,
            //            HardScore = 100,
            //            LifeScore = 100,
            //            ManageScore = 100
            //        }
            //    },
            //    new SchoolComment(){
            //            Id = Guid.Parse("C278576D-99FE-48C6-A4E1-01C112C1545D"),
            //            Content = "导入数据测试222222",
            //            SchoolId = Guid.Parse("97952996-5E9B-406C-9BEF-6223A4C1F729"),
            //            SchoolSectionId = Guid.Parse("97952996-5E9B-406C-9BEF-6223A4C1F729"),
            //            CommentUserId = Guid.Parse("402E2D80-B7B7-4DAA-B381-FAB2A34CD372"),
            //            AddTime = DateTime.Now,
            //            PostUserRole = 0,
            //            CommentScore = new SchoolCommentScore()
            //            {
            //                CommentId = Guid.Parse("C278576D-99FE-48C6-A4E1-01C112C1545D"),
            //                AggScore = 100,
            //                IsAttend = true,
            //                TeachScore = 100,
            //                EnvirScore = 100,
            //                HardScore = 100,
            //                LifeScore = 100,
            //                ManageScore = 100
            //            }
            //        }
            //};

            //commentRep.ExecuteTransaction(comments);

            return View();
        }

        

        [HttpPost]
        public JsonResult Import(List<IFormFile> files,int type) 
        {
            try
            {
                ImportType = type;
                List<Tuple<bool, string>> ErrorLogs = new List<Tuple<bool, string>>();
                if (files.Count > 0)
                {
                    bool status = false;

                    //将上传的excel保存至服务器
                    string filePath = UploadFile(files[0]);

                    IWorkbook wk;

                    using (FileStream file = System.IO.File.OpenRead(filePath))
                    {
                        //npoi 
                        if (Path.GetExtension(filePath) == ".xls")
                        {
                            wk = new HSSFWorkbook(file);
                        }
                        else 
                        {
                            wk = new XSSFWorkbook(file);
                        }

                       int Sheets =  wk.NumberOfSheets;

                        for (int i = 0; i < Sheets; i++)
                        {
                            ISheet st = wk.GetSheetAt(i);
                            //读取对应sheet，对应标识码：  _1：comment，_2：answer，_3：question
                            if (st.SheetName.Contains("_1") || st.SheetName.Contains("_2") || st.SheetName.Contains("_3") || st.SheetName.Contains("_4"))
                            {
                                int identity = int.Parse(st.SheetName.Split("_")[1]);
                                switch (identity)
                                {
                                    case 1:
                                        //ErrorLogs.Add(ImportComment(st));
                                        break;
                                    case 2:
                                        //ErrorLogs.Add(ImportAnswer(st));
                                        break;
                                    case 3:
                                        //status = ImportQuestion(st);
                                        break;
                                    case 4:
                                        ErrorLogs.Add(ImportQuestionAndAnwer(st));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else 
                            {
                                continue;
                            }
                        }
                    }
                    return new JsonResult(new { status = true , message = "导出日志：\n" + string.Join("\n", ErrorLogs.Select(x => x.Item2).ToList()) });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "导入失败：无文件上传" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "导入失败，异常错误：" + ex.Message });
            }
        }

        /// <summary>
        /// 导入 comment
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, string> ImportComment(ISheet st) 
        {
            string error = @"写点评导入错误：\n";
            try
            {
                List<CommentViewModel> model = ExcelToList<CommentViewModel>(st, out error);

                if (model != null)
                {
                    List<SchoolComment> comments = new List<SchoolComment>();
                    foreach (var item in model)
                    {
                        SchoolComment comment = new SchoolComment();
                        comment.SchoolSectionId = item.Eid;
                        comment.SchoolId = item.Sid;
                        comment.Content = item.CommentContent;
                        comment.IsAnony = false;
                        comment.ImportType = (ImportType)ImportType;

                        if (item.RespondentId == default(Guid))
                        {
                            //用户池数据
                        }
                        else
                        {
                            comment.CommentUserId = item.RespondentId;
                            comment.PostUserRole = (int)UserRole.Daren;
                        }

                        comment.AddTime = GetRandomTime(DateTime.Now, MaxTime);

                        SchoolCommentScore score = new SchoolCommentScore();
                        score.CommentId = comment.Id;
                        score.EnvirScore = item.EnvirScore * 20;
                        score.HardScore = item.HardScore * 20;
                        score.LifeScore = item.LifeScore * 20;
                        score.ManageScore = item.ManageScore * 20;
                        score.TeachScore = item.TeachScore * 20;
                        score.IsAttend = true;
                        score.AggScore = (item.EnvirScore + item.HardScore + item.LifeScore + item.ManageScore + item.TeachScore) / 5;
                        comment.CommentScore = score;
                        comments.Add(comment);
                    }

                    List<Guid> Eids = comments.Where(x => x.SchoolId == default(Guid)).GroupBy(x => x.SchoolSectionId).Select(x => x.Key)?.ToList();
                    int takeTotal = comments.Where(x => x.CommentUserId == default(Guid)).Count();
                    if (Eids.Count() > 0)
                    {
                        var schools = schoolRepository.GetSchoolIdByEids(Eids);
                        comments.Where(x => x.SchoolId == default(Guid))?.ToList().ForEach(x =>
                        {
                            var sid = schools.Find(s => s.Eid == x.SchoolSectionId)?.Sid;
                            if (sid == null || sid == Guid.Empty) 
                            {
                                error += $"未导入数据错误提示：{model.Where(m=>m.Eid == x.SchoolSectionId).FirstOrDefault().SchoolNameFull}学校未能找到\n";
                            }
                            x.SchoolId = sid ?? Guid.Empty;
                        });
                    }

                    var ids = comments.Where(x => x.SchoolId == Guid.Empty).Select(x => x.Id).ToList();

                    var co = comments.Where(x => ids.Contains(x.Id)).Select(x=>x).ToList();



                    if (takeTotal > 0)
                    {
                        var userIds = schoolRepository.GetUserIds(takeTotal);
                        comments.Where(x => x.CommentUserId == default(Guid))?.ToList().ForEach(x =>
                        {
                            var commentUserIds = comments.Select(x => x.CommentUserId).ToList();
                            x.PostUserRole = 0;
                            x.CommentUserId = userIds.Where(x => !commentUserIds.Contains(x)).FirstOrDefault();
                        });
                    }

                    var data = comments.Where(x => x.SchoolId != Guid.Empty).OrderBy(x => x.AddTime).ToList();
                    //var empty = comments.Where(x => x.SchoolId == Guid.Empty).ToList();

                    int dataPageSize = data.Count() / 1000 + 1;

                    for (int i = 0; i < dataPageSize; i++)
                    {
                        //数据入库
                        var rez = commentRep.ExecuteTransaction(data.Skip(i * 1000).Take(1000).ToList());

                        if (rez.Item1)
                        {
                            error += $"点评第{i + 1}页导入成功";
                        }
                        else
                        {
                            error += $"点评第{i + 1}页导入失败，异常错误：{rez.Item2}";
                        }
                    }
                    return new Tuple<bool, string>(true, error);
                }
                else 
                {
                    error += "点评导入成功：无数据导入\n";
                    return new Tuple<bool, string>(true, error);
                }
            }
            catch (Exception ex)
            {
                //ImportComment(st);
                error += "点评导入失败,错误异常：" + ex.Message;
                return new Tuple<bool, string>(false, error);
            }
        }

        /// <summary>
        /// 导入 question[邀请提问]
        /// </summary>
        /// <returns></returns>
        public bool ImportQuestion(ISheet st)
        {
            string error = @"提问导入错误：\n";
            List<QuestionViewModel> model = ExcelToList<QuestionViewModel>(st,out error);

            if (model != null) 
            {
                foreach (var item in model)
                {
                    //QuestionInfo question = new QuestionInfo();
                    //question.SchoolSectionId = item.Eid;
                    //question.SchoolId = item.Sid;
                    //question.Content = item.CommentContent;


                    ////提问Id为空，获取用户池 随机获取用户数据
                    //if (item.CommitQuestionUserId == default(Guid))
                    //{
                    //    //用户池随机获取用户
                    //    //question.UserId
                    //}
                    //else 
                    //{
                    //    question.UserId = item.CommitQuestionUserId;
                    //    question.PostUserRole = UserRole.Daren;
                    //}

                    //question.CreateTime = GetRandomTime(item.InviteTime);
                    //question.IsHaveImagers = false;

                    //_questionInfo.AddQuestion(question);
                }
            }
            return false;
        }

        /// <summary>
        /// 导入回答
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, string> ImportAnswer(ISheet st)
        {
            try
            {
                string error = @"回答导入错误：\n";
                List<AnswerViewModel> model = ExcelToList<AnswerViewModel>(st, out error);

                if (model != null)
                {
                    List<QuestionsAnswersInfo> answersInfos = new List<QuestionsAnswersInfo>();
                    foreach (AnswerViewModel item in model)
                    {
                        QuestionsAnswersInfo answer = new QuestionsAnswersInfo();
                        answer.QuestionInfoId = item.QuestionId;
                        answer.CreateTime = GetRandomTime(DateTime.Now, MaxTime);
                        answer.IsSchoolPublish = false;
                        answer.IsAttend = item.IsAttend;
                        answer.IsAnony = item.IsAnony;
                        answer.Content = item.AnswerContent;
                        answer.ImportType = (ImportType)ImportType;

                        if (item.RespondentId == default(Guid))
                        {
                            //获取用户池数据
                        }
                        else
                        {
                            answer.UserId = item.UserId;
                            answer.PostUserRole = UserRole.Daren;
                        }

                        answersInfos.Add(answer);
                        //_questionsAnswers.Insert(answer);
                    }

                    int takeTotal = answersInfos.Where(x => x.UserId == default(Guid)).Count();

                    if (takeTotal > 0)
                    {
                        var userIds = schoolRepository.GetUserIds(takeTotal);
                        answersInfos.Where(x => x.UserId == default(Guid))?.ToList().ForEach(x =>
                        {
                            var commentUserIds = answersInfos.Select(x => x.UserId).ToList();
                            x.PostUserRole = 0;
                            x.UserId = userIds.Where(x => !commentUserIds.Contains(x)).FirstOrDefault();
                        });
                    }

                    var rez = commentRep.AnswerExecuteTransaction(answersInfos);
                    if (rez.Item1)
                    {
                        return new Tuple<bool, string>(true, "问答导入成功");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "问答导入失败，异常：" + rez.Item2);
                    }
                }
                else 
                {
                    return new Tuple<bool, string>(false, "问答导入成功，暂无数据导入。");
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "问答导入失败，异常：" + ex.Message);
            }
        }

        public Tuple<bool, string> ImportQuestionAndAnwer(ISheet st) 
        {
            string error = @"写点评导入错误：\n";
            try
            {
                List<QuestionAndAnswer> model = ExcelToList<QuestionAndAnswer>(st, out error);

                if (model != null)
                {
                    List<QuestionInfo> questions = new List<QuestionInfo>();

                    foreach (var item in model)
                    {
                            QuestionInfo question = new QuestionInfo();
                            question.Content = item.QuestionContent;
                            question.IsAnony = false;
                            question.SchoolSectionId = item.Eid;
                            question.CreateTime = GetRandomTime(DateTime.Parse("2019-11-22 16:30"), MaxTime);
                            question.PostUserRole = 0;
                            question.IsAnony = false;
                            question.ImportType = (ImportType)ImportType;
                            question.IsContrast = item.IsContact;


                            if(item.AnswerContent != null && item.AnswerContent != "") 
                            {
                                List<QuestionsAnswersInfo> answer = new List<QuestionsAnswersInfo>();

                                QuestionsAnswersInfo aw = new QuestionsAnswersInfo();
                                aw.Content = item.AnswerContent;
                                aw.QuestionInfoId = question.Id;
                                aw.PostUserRole = 0;
                                aw.IsSchoolPublish = false;
                                aw.IsAnony = false;
                                aw.IsAttend = true;
                                aw.CreateTime = GetRandomTime(question.CreateTime, MaxTime);
                                aw.ImportType = (ImportType)ImportType;
                                aw.IsContrast = item.IsContact;

                                answer.Add(aw);

                                question.answer = answer;
                            }

                            questions.Add(question);
                    }

                    if (questions.Count() > 0)
                    {
                        List<Guid> Eids = questions.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList();

                        int userTakeTotal = questions.Count();
                        int answerTakeTotal = questions.Where(x=>x.answer != null).Select(x => x.answer.Count()).Sum();

                        var qaUser = schoolRepository.GetUserIds(userTakeTotal);
                        var anUser = schoolRepository.GetUserIds(answerTakeTotal);


                        int answerTake = 0;
                        for (int i = 0; i < questions.Count(); i++)
                        {
                            questions[i].UserId = qaUser[i];
                            if (questions[i].answer != null) 
                            {
                                for (int j = 0; j < questions[i].answer.Count(); j++)
                                {
                                    questions[i].answer[j].UserId = anUser[answerTake];
                                    answerTake++;
                                }
                            }
                        }

                        var schools = schoolRepository.GetSchoolIdByEids(Eids);

                        int total = questions.Where(x => x.SchoolId == default(Guid)).Count();

                        questions.Where(x => x.SchoolId == default(Guid))?.ToList().ForEach(x =>
                        {
                            var sid = schools.Find(s => s.Eid == x.SchoolSectionId)?.Sid;
                            if (sid == null || sid == Guid.Empty)
                            {
                                //var a = "问题："+x.Content+"学校未找到";
                                //error += $"未导入数据错误提示：{model.Where(m => m.Eid == x.SchoolSectionId).FirstOrDefault().SchoolNameFull}学校未能找到\n";
                                error += "问题：" + x.Content + "学校未找到\n";
                            }
                            x.SchoolId = sid  ?? Guid.Empty;
                        });
                    }

                    var rez = commentRep.QAExecuteTransaction(questions);
                    if (rez.Item1)
                    {
                        error += "问答导入成功\n";
                        return new Tuple<bool, string>(true, error);
                    }
                    else
                    {
                        error += "问答导入失败，异常：" + rez.Item2 + "\n";
                        return new Tuple<bool, string>(false, error);
                    }
                    return new Tuple<bool, string>(false,"");
                }
                else 
                {
                    error += "问答导入成功：无数据导入\n";
                    return new Tuple<bool, string>(true, error);
                }
            }
            catch (Exception ex)
            {
                error += "问答导入失败，异常：" + ex.Message+ "\n";
                return new Tuple<bool, string>(false, error);
            }
        }


        /// <summary>
        /// Excel转list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<T> ExcelToList<T>(ISheet sheet,out string error) where T : new()
        {
            //错误信息
            StringBuilder ErrorBuilder = new StringBuilder();
            //结果
            List<T> TModelList = new List<T>();
            //Excel中的表头信息
            Dictionary<int, string> SheetDictionary = new Dictionary<int, string>();
            //model中的信息
            Dictionary<int, ExcelModel> TModelDictionary = new Dictionary<int, ExcelModel>();

                List<int> CellNumber = new List<int>();
                //获取列头
                IRow HeadRow = sheet.GetRow(0);
                int EmptyCell = 0;
                for (int i = 0; i < HeadRow.LastCellNum; i++)
                {
                    if (HeadRow.GetCell(i) == null)
                    {
                        EmptyCell = 1;
                        continue;
                    }
                    if (EmptyCell != 0)
                    {
                        SheetDictionary.Add(i - 1, HeadRow.GetCell(i).StringCellValue);
                        EmptyCell = 0;
                    }
                    else
                    {
                        SheetDictionary.Add(i, HeadRow.GetCell(i).StringCellValue);
                    }
                }
                //获取对象中所有描述、字段信息
                MemberInfo[] propertyInfos = typeof(T).GetProperties();
                for (int k = 0; k < propertyInfos.Length; k++)
                {
                    ExcelModel excel = new ExcelModel();
                    //获取描述
                    DescriptionAttribute desc = (DescriptionAttribute)propertyInfos[k].GetCustomAttribute(typeof(DescriptionAttribute));
                    if (desc != null)
                    {
                        excel.Description = desc.Description;
                        //获取属性
                        excel.Property = (PropertyInfo)propertyInfos[k];
                        TModelDictionary.Add(k, excel);
                    }
                }
                //获取处头部之外的所有行
                for (int j = 1; j <= sheet.LastRowNum; j++)
                {
                    T model = new T(); ;
                    IRow row = sheet.GetRow(j);
                    for (int p = 0; p < row.LastCellNum; p++)
                    {
                            ICell cell = row.GetCell(p);
                            //查找Excel字典中与model字典中对应的字段
                            ExcelModel excelModel = TModelDictionary.Where(x => x.Value.Description == SheetDictionary[p]).Select(x => x.Value).FirstOrDefault();
                            if (excelModel != null && cell != null)
                            {
                                //Numeric(int，double，datetime)
                                if (cell.CellType == CellType.Numeric)
                                {
                                    if (HSSFDateUtil.IsCellDateFormatted(cell) && excelModel.Property.PropertyType == typeof(DateTime))
                                    {
                                        excelModel.Property.SetValue(model, cell.DateCellValue);
                                    }
                                    else if (excelModel.Property.PropertyType == typeof(double))
                                    {
                                        excelModel.Property.SetValue(model, Double.Parse(cell.NumericCellValue.ToString()));
                                    }
                                    else if (excelModel.Property.PropertyType == typeof(bool))
                                    {
                                        excelModel.Property.SetValue(model, Convert.ToBoolean(int.Parse(cell.NumericCellValue.ToString())));
                                        
                                    }
                                    else if (excelModel.Property.PropertyType == typeof(decimal))
                                    {
                                        excelModel.Property.SetValue(model, decimal.Parse(cell.NumericCellValue.ToString()));
                                    }
                                    else
                                    {
                                        excelModel.Property.SetValue(model, int.Parse(cell.NumericCellValue.ToString()));
                                    }
                                }
                                else if (cell.CellType == CellType.String && excelModel.Property.PropertyType == typeof(string))
                                {
                                    excelModel.Property.SetValue(model, cell.StringCellValue);
                                }
                                else if (cell.CellType == CellType.String && excelModel.Property.PropertyType == typeof(Guid))
                                {
                                    if(cell.StringCellValue != "" && cell.StringCellValue != null) 
                                    {
                                        Guid rez = Guid.Empty;
                                        if (Guid.TryParse(cell.StringCellValue, out rez))
                                        {
                                            excelModel.Property.SetValue(model, rez);
                                        }
                                        else 
                                        {
                                            ErrorBuilder.Append("第" + j + "行的第" + p + @"列数据格式不正确，数据值为：" + cell.StringCellValue + " \n");
                                        }
                                    }
                                }
                                else if (cell.CellType == CellType.String && excelModel.Property.PropertyType == typeof(int))
                                {
                                    int rez = 0;
                                    if (int.TryParse(cell.StringCellValue, out rez))
                                    {
                                        excelModel.Property.SetValue(model, rez);
                                    }
                                }
                                else if (cell.CellType == CellType.String && excelModel.Property.PropertyType == typeof(decimal))
                                {
                                    decimal rez = 0;
                                    if(decimal.TryParse(cell.StringCellValue,out rez)) 
                                    {
                                        excelModel.Property.SetValue(model, rez);
                                    }
                                }
                                else if (cell.CellType == CellType.String && excelModel.Property.PropertyType == typeof(DateTime)) 
                                {
                                    DateTime rez;
                                    if(DateTime.TryParse(cell.StringCellValue,out rez)) 
                                    {
                                        excelModel.Property.SetValue(model, rez);
                                    }
                                }
                                else
                                {
                                    ErrorBuilder.Append("第" + j + "行的第" + p + @"列数据格式不正确！\n");
                                }
                        }
                    }
                    if (model != null)
                    {
                        TModelList.Add(model);
                    }
                }
            error = ErrorBuilder.ToString();
            return TModelList.Count == 0 ? null : TModelList;
        }


        //检查该字符位数值
        public static bool IsNumeric(string str) //接收一个string类型的参数,保存到str里
        {
            if (str == null || str.Length == 0)    //验证这个参数是否为空
                return false;                           //是，就返回False
            ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
            byte[] bytestr = ascii.GetBytes(str);         //把string类型的参数保存到数组里

            foreach (byte c in bytestr)                   //遍历这个数组里的内容
            {
                if (c < 48 || c > 57)                          //判断是否为数字
                {
                    return false;                              //不是，就返回False
                }
            }
            return true;                                        //是，就返回True
        }

        //文件上传至服务器
        public string UploadFile(IFormFile file) 
        {
            string filePath = @"D:\Upload\Excel\"+file.FileName;

            //检测该上传的文件名是否存在
            if (System.IO.File.Exists(filePath)) 
            {
                filePath = filePath.Insert(filePath.IndexOf('.'), $"{String.Join("", DateTime.Now.ToString().Split("/")).Replace(" ", "").Replace(":", "")}");
            }

            if (Path.GetExtension(filePath) == ".xls" || Path.GetExtension(filePath) == ".xlsx")
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            else 
            {
                throw new Exception("上传文件格式不正确");
            }
            return filePath;
        }


        public JsonResult Export(DateTime startTime,DateTime endTime,int type)
        {
            try
            {
                //创建一个工作簿
                HSSFWorkbook workbook = new HSSFWorkbook();

                bool IsExport = false;
                string errorLogs = "";

                string filename = "";
                if (type == 1)
                {
                    //月导出【达人总数据】   
                }
                else if (type == 2)
                {
                    //达人邀请数据

                    filename = "commentAndanswer" + DateTime.Now.ToString("yyyyMMddHHmmss");

                    var commentExport = InviteComments(startTime, endTime, workbook);
                    var answerExport = InviteAnswers(startTime, endTime, workbook);
                    if (commentExport.Item1 && answerExport.Item1)
                    {
                        IsExport = true;
                    }
                    else
                    {
                        errorLogs = commentExport.Item2 + answerExport.Item2;
                    }
                }

                if (IsExport)
                {
                    using (FileStream fs = System.IO.File.OpenWrite(@"D:\Upload\Export\" + filename + ".xls")) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
                    {
                        workbook.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
                    }

                    return Json(new { success = true, filename });
                }
                else
                {
                    return Json(new { success = false, logs = errorLogs });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, logs = ex.Message });
            }
        }

        public FileResult DownloadExcel(string filename) 
        {
            var stream = System.IO.File.OpenRead("D:\\Upload\\Export\\" + filename + ".xls");  //创建文件流
            return File(stream, "application/Excel", filename+ ".xls");
        }

        public Tuple<bool,string> InviteComments(DateTime startTime, DateTime endTime, HSSFWorkbook workbook) 
        {
            try
            {
                //工作簿中必须带有一个sheet
                ISheet sheet = CreateHeaderTitle<InviteComment>(workbook, "邀请点评_1");

                var comments = UserInfoRepository.InviteComments(startTime, endTime);
                for (int i = 0; i < comments.Count(); i++)
                {
                    //列名
                    IRow row = sheet.CreateRow(i + 1);

                    ICell cell = row.CreateCell(0);
                    cell.SetCellValue(comments[i].InviteTime);
                    cell.SetCellType(CellType.Numeric);

                    ICell cell9 = row.CreateCell(1);
                    cell9.SetCellValue(comments[i].DarenName.ToString());
                    cell9.SetCellType(CellType.String);

                    ICell cell1 = row.CreateCell(2);
                    cell1.SetCellValue(comments[i].Sid.ToString());
                    cell1.SetCellType(CellType.String);

                    ICell cell2 = row.CreateCell(3);
                    cell2.SetCellValue(comments[i].Sname);
                    cell2.SetCellType(CellType.String);

                    ICell cell3 = row.CreateCell(4);
                    cell3.SetCellValue(comments[i].Eid.ToString());
                    cell3.SetCellType(CellType.String);

                    ICell cell4 = row.CreateCell(5);
                    cell4.SetCellValue(comments[i].Ename);
                    cell4.SetCellType(CellType.String);

                    ICell cell5 = row.CreateCell(6);
                    cell5.SetCellValue(comments[i].InviteUser.ToString());
                    cell5.SetCellType(CellType.String);

                    ICell cell6 = row.CreateCell(7);
                    cell6.SetCellValue(comments[i].ReceiveUser.ToString());
                    cell6.SetCellType(CellType.String);
                }

                return new Tuple<bool, string>(true,"comment sheet 导出成功");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }  
        }

        public Tuple<bool, string> InviteAnswers(DateTime startTime, DateTime endTime,HSSFWorkbook workbook)
        {
            try
            {
                //工作簿中必须带有一个sheet
                ISheet sheet = CreateHeaderTitle<InviteAnswer>(workbook, "邀请回答_2");

                var answers = UserInfoRepository.InviteAnswers(startTime, endTime);
                for (int i = 0; i < answers.Count(); i++)
                {
                    //列名
                    IRow row = sheet.CreateRow(i + 1);

                    ICell cell = row.CreateCell(0);
                    cell.SetCellValue(answers[i].InviteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    cell.SetCellType(CellType.String);

                    ICell cell11 = row.CreateCell(1);
                    cell11.SetCellValue(answers[i].DarenName.ToString());
                    cell11.SetCellType(CellType.String);

                    ICell cell1 = row.CreateCell(2);
                    cell1.SetCellValue(answers[i].Sid.ToString());
                    cell1.SetCellType(CellType.String);

                    ICell cell2 = row.CreateCell(3);
                    cell2.SetCellValue(answers[i].Sname);
                    cell2.SetCellType(CellType.String);

                    ICell cell3 = row.CreateCell(4);
                    cell3.SetCellValue(answers[i].Eid.ToString());
                    cell3.SetCellType(CellType.String);

                    ICell cell4 = row.CreateCell(5);
                    cell4.SetCellValue(answers[i].Ename);
                    cell4.SetCellType(CellType.String);

                    ICell cell5 = row.CreateCell(6);
                    cell5.SetCellValue(answers[i].QuestionId.ToString());
                    cell5.SetCellType(CellType.String);

                    ICell cell6 = row.CreateCell(7);
                    cell6.SetCellValue(answers[i].InviteUser.ToString());
                    cell6.SetCellType(CellType.String);

                    ICell cell7 = row.CreateCell(8);
                    cell7.SetCellValue(answers[i].ReceiveUser.ToString());
                    cell7.SetCellType(CellType.String);

                    ICell cell8 = row.CreateCell(9);
                    cell8.SetCellValue("");
                    cell8.SetCellType(CellType.String);

                    ICell cell9 = row.CreateCell(10);
                    cell9.SetCellValue(answers[i].Content);
                    cell9.SetCellType(CellType.String);
                }

                return new Tuple<bool, string>(true,"answer sheet 导出成功");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            
        }

        /// <summary>
        /// 创建sheet 并创建第一行title
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public ISheet CreateHeaderTitle<T>(HSSFWorkbook workbook,string sheetName) where T :new()
        {
            T model = new T();
            PropertyInfo[] properties = typeof(T).GetProperties();

            //工作簿中必须带有一个sheet
            ISheet sheet = workbook.CreateSheet(sheetName);

            //列名
            IRow cellsOne = sheet.CreateRow(0);

            //排除所有不需要导出的字段
            List<int> Barring = new List<int>();

            for (int k = 0; k < properties.Length; k++)
            {
                ICell cell1 = cellsOne.CreateCell(k);
                //获取属性的描述
                DescriptionAttribute desc = (DescriptionAttribute)properties[k].GetCustomAttribute(typeof(DescriptionAttribute));
                if (desc != null)
                {
                    cell1.SetCellValue(desc.Description);
                    cell1.SetCellType(CellType.String);
                }
                else
                {
                    //字段没带描述（无需导出）
                    Barring.Add(k);
                }
            }

            return sheet;
        }

    }




    //泛型对象信息存储
    public class ExcelModel
    {
        public string Description { get; set; }

        public PropertyInfo Property { get; set; }
    }
}
