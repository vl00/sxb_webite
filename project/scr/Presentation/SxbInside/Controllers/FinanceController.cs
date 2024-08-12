using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PMS.Infrastructure.Application.IService;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Message;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Tool.Email;
using Sxb.Inside.Response;

namespace Sxb.Inside.Controllers
{
    public class FinanceController : Controller
    {
        private readonly IEmailClient _emailClient;
        FinanceDBContext _financeDB;
        IUserService _userService;
        PaidQADBContext _paidDB;


        public FinanceController(IEmailClient emailClient, FinanceDBContext financeDBContext, IUserService userService, PaidQADBContext paidQADBContext)
        {
            _userService = userService;
            _financeDB = financeDBContext;
            _emailClient = emailClient;
            _paidDB = paidQADBContext;
        }

        [HttpGet]
        public async Task<ResponseResult> ExportUserPaidEmail()
        {
            var result = ResponseResult.Failed();
            var endTime = DateTime.Now;
            var startTime = endTime.AddDays(-90);
            var str_SQL = $@"SELECT
	                            * 
                            FROM
	                            PayOrder 
                            WHERE
	                            OrderStatus = 6 
	                            AND System IN ( 0, 2 )
	                            AND CreateTime >= @startTime
	                            AND CreateTime <= @endTime;";
            var orders = await _financeDB.QueryAsync<dynamic>(str_SQL, new { startTime, endTime });
            if (orders == null || !orders.Any()) return result;

            var userIDs = orders.Select(p => (Guid)p.UserId).Distinct();
            if (userIDs == null || !userIDs.Any()) return result;
            var userInfos = await _userService.GetUserInfos(userIDs);

            str_SQL = $"select * From [Order] where id in @ids";
            var paidQAOrderIDs = orders.Where(p => ((int)p.System) == 0).Select(p => (Guid)p.OrderId).Distinct();
            var paidQAOrders = await _paidDB.QueryAsync<Order>(str_SQL, new { ids = paidQAOrderIDs });
            var anwserUserIDs = paidQAOrders.Select(p => p.AnswerID).Distinct();
            var talentGrades = await _paidDB.QueryAsync<dynamic>("Select * from TalentGrade WHERE TalentUserID in @ids;", new { ids = anwserUserIDs });
            var grades = await _paidDB.QueryAsync<Grade>("Select * From Grade;", new { });
            var regions = await _paidDB.QueryAsync<RegionType>("Select * from RegionType;", new { });
            var talentRegions = await _paidDB.QueryAsync<TalentRegion>("Select * From TalentRegion Where UserID in @ids", new { ids = anwserUserIDs });

            var excelFile = new XSSFWorkbook();
            var sheet = excelFile.CreateSheet();
            var headerRow = sheet.CreateRow(0);
            var headerStyle = excelFile.CreateCellStyle();
            headerStyle.IsLocked = true;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.FillForegroundColor = HSSFColor.Yellow.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            var headerFontStyle = excelFile.CreateFont();
            headerFontStyle.IsBold = true;
            headerStyle.SetFont(headerFontStyle);
            var cell = headerRow.CreateCell(0);
            cell.SetCellValue("用户ID");
            cell = headerRow.CreateCell(1);
            cell.SetCellValue("用户昵称");
            cell = headerRow.CreateCell(2);
            cell.SetCellValue("最近交易距离当前天数");
            cell = headerRow.CreateCell(3);
            cell.SetCellValue("累计单数");
            cell = headerRow.CreateCell(4);
            cell.SetCellValue("累计交易金额");
            cell = headerRow.CreateCell(5);
            cell.SetCellValue("付费咨询");
            cell = headerRow.CreateCell(6);
            cell.SetCellValue("课程");
            cell = headerRow.CreateCell(7);
            cell.SetCellValue("幼儿入园");
            cell = headerRow.CreateCell(8);
            cell.SetCellValue("幼升小");
            cell = headerRow.CreateCell(9);
            cell.SetCellValue("小升初");
            cell = headerRow.CreateCell(10);
            cell.SetCellValue("中考");
            cell = headerRow.CreateCell(11);
            cell.SetCellValue("高考");
            cell = headerRow.CreateCell(12);
            cell.SetCellValue("政策解读");
            cell = headerRow.CreateCell(13);
            cell.SetCellValue("学校秘典");
            cell = headerRow.CreateCell(14);
            cell.SetCellValue("小道内幕");
            cell = headerRow.CreateCell(15);
            cell.SetCellValue("升学助手");
            cell = headerRow.CreateCell(16);
            cell.SetCellValue("赛培特长");
            cell = headerRow.CreateCell(17);
            cell.SetCellValue("国际学校");
            cell = headerRow.CreateCell(18);
            cell.SetCellValue("学区问宅");
            cell = headerRow.CreateCell(19);
            cell.SetCellValue("辅导机构");
            cell = headerRow.CreateCell(20);
            cell.SetCellValue("育儿课堂");
            cell = headerRow.CreateCell(21);
            cell.SetCellValue("学科学习");
            cell = headerRow.CreateCell(22);
            cell.SetCellValue("注册日期");
            cell = headerRow.CreateCell(23);
            cell.SetCellValue("最近登录日期");
            cell = headerRow.CreateCell(24);
            cell.SetCellValue("最近消费日期");

            foreach (var item in headerRow.Cells)
            {
                item.CellStyle = headerStyle;
            }
            var rowIndex = 1;
            var dataRowStyle = excelFile.CreateCellStyle();
            dataRowStyle.BorderLeft = BorderStyle.Thin;
            dataRowStyle.BorderRight = BorderStyle.Thin;
            dataRowStyle.BorderTop = BorderStyle.Thin;
            dataRowStyle.BorderBottom = BorderStyle.Thin;

            foreach (var user in userInfos)
            {
                var userOrderCount = orders.Count(p => p.UserId == user.Id);
                var userPaidQAOrders = paidQAOrders.Where(p => p.CreatorID == user.Id);
                if (userPaidQAOrders == null || !userPaidQAOrders.Any()) continue;


                var row = sheet.CreateRow(rowIndex++);
                cell = row.CreateCell(0);
                cell.SetCellValue(user.Id.ToString());
                cell = row.CreateCell(1);
                cell.SetCellValue(user.NickName);
                cell = row.CreateCell(2);
                cell.SetCellValue((int)(endTime - (DateTime)orders.Where(p => p.UserId == user.Id).OrderByDescending(p => p.CreateTime).First().CreateTime).TotalDays);
                cell = row.CreateCell(3);
                cell.SetCellValue(userOrderCount);
                cell = row.CreateCell(4);
                cell.SetCellValue(orders.Where(p => p.UserId == user.Id).Sum(p => (double)p.TotalAmount));
                cell = row.CreateCell(5);
                var a = orders.Count(p => p.UserId == user.Id && p.System == 0);
                var b = orders.Count(p => p.UserId == user.Id);
                var paidQAPercent = (int)(((double)orders.Count(p => p.UserId == user.Id && p.System == 0)) / ((double)orders.Count(p => p.UserId == user.Id)) * 100);
                cell.SetCellValue($"{paidQAPercent}%");
                cell = row.CreateCell(6);
                cell.SetCellValue($"{100 - paidQAPercent}%");

                //if (userPaidQAOrders.Count() > 5)
                //{

                //}

                //var userPaidQAOrderTalentGrades = new List<dynamic>();

                //foreach (var userOrder in userPaidQAOrders)
                //{
                //    userPaidQAOrderTalentGrades.AddRange(talentGrades.Where(p=>p.TalentUserID == userOrder.AnswerID));
                //}

                var userPaidQAOrderTalentGrades = talentGrades.Where(p => userPaidQAOrders.Select(x => x.AnswerID).ToList().Contains(p.TalentUserID));
                cell = row.CreateCell(7);//幼儿入园
                var grade1ID = grades.FirstOrDefault(p => p.Sort == 1).ID;
                var grade1Percent = ((double)userPaidQAOrderTalentGrades.Count(p => p.GradeID == grade1ID) / (double)userPaidQAOrderTalentGrades.Count() * 100);
                cell.SetCellValue($"{grade1Percent.ToString("N2")}%");
                if (grade1Percent == 0) cell.SetCellValue("/");
                cell = row.CreateCell(8);
                var grade2ID = grades.FirstOrDefault(p => p.Sort == 2).ID;
                var grade2Percent = ((double)userPaidQAOrderTalentGrades.Count(p => p.GradeID == grade2ID) / (double)userPaidQAOrderTalentGrades.Count() * 100);
                cell.SetCellValue($"{grade2Percent.ToString("N2")}%");
                if (grade2Percent == 0) cell.SetCellValue("/");
                cell = row.CreateCell(9);
                var grade3ID = grades.FirstOrDefault(p => p.Sort == 3).ID;
                var grade3Percent = ((double)userPaidQAOrderTalentGrades.Count(p => p.GradeID == grade3ID) / (double)userPaidQAOrderTalentGrades.Count() * 100);
                cell.SetCellValue($"{grade3Percent.ToString("N2")}%");
                if (grade3Percent == 0) cell.SetCellValue("/");
                cell = row.CreateCell(10);
                var grade4ID = grades.FirstOrDefault(p => p.Sort == 4).ID;
                var grade4Percent = ((double)userPaidQAOrderTalentGrades.Count(p => p.GradeID == grade4ID) / (double)userPaidQAOrderTalentGrades.Count() * 100);
                cell.SetCellValue($"{grade4Percent.ToString("N2")}%");
                if (grade4Percent == 0) cell.SetCellValue("/");
                cell = row.CreateCell(11);
                var grade5ID = grades.FirstOrDefault(p => p.Sort == 5).ID;
                var grade5Percent = ((double)userPaidQAOrderTalentGrades.Count(p => p.GradeID == grade5ID) / (double)userPaidQAOrderTalentGrades.Count() * 100);
                cell.SetCellValue($"{grade5Percent.ToString("N2")}%");
                if (grade5Percent == 0) cell.SetCellValue("/");

                //var eqwea = new List<TalentRegion>();
                //foreach (var userOrder in userPaidQAOrders)
                //{
                //    eqwea.AddRange(talentRegions.Where(p=>p.UserID == userOrder.AnswerID));
                //}

                var userPaidQAOrderTalentRegions = talentRegions.Where(p => userPaidQAOrders.Select(x => x.AnswerID).ToList().Contains(p.UserID.Value));
                var regionIndex = 12;
                foreach (var region in regions.Where(p => p.PID == null).OrderBy(p => p.Sort))
                {
                    cell = row.CreateCell(regionIndex);
                    var t_Percent = ((double)userPaidQAOrderTalentRegions.Count(p => regions.Where(x => x.PID == region.ID).Select(x => x.ID).ToList().Contains(p.RegionTypeID.Value))
                        / (double)userPaidQAOrderTalentRegions.Count() * 100);
                    if (t_Percent == 0)
                    {
                        cell.SetCellValue("/");
                    }
                    else
                    {
                        cell.SetCellValue($"{t_Percent.ToString("N2")}%");
                    }
                    regionIndex++;
                }

                cell = row.CreateCell(22);
                cell.SetCellValue(user.RegTime.ToString("yyyy-MM-dd"));
                cell = row.CreateCell(23);
                cell.SetCellValue(user.LoginTime.ToString("yyyy-MM-dd"));
                cell = row.CreateCell(24);
                cell.SetCellValue(((DateTime)orders.Where(p => p.UserId == user.Id).OrderByDescending(p => p.CreateTime).First().CreateTime).ToString("yyyy-MM-dd"));
            }

            for (int i = 0; i < 25; i++)
            {
                sheet.AutoSizeColumn(i, true);
            }

            //if (System.IO.File.Exists("r:/export.xlsx")) System.IO.File.Delete("r:/export.xlsx");
            //using (var fs = System.IO.File.OpenWrite(@"r:/export.xlsx"))
            //{
            //    excelFile.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
            //    Console.WriteLine("生成成功");
            //}

            using (MemoryStream stream = new MemoryStream())
            {
                excelFile.Write(stream, true);
                stream.Seek(0, SeekOrigin.Begin);

                var att = new Attachment(stream, "export.xlsx");
                att.ContentDisposition.FileName = "export.xlsx";
                await _emailClient.NotifyByMailAsync("用户消费导出", "", new string[1] { "kison@sxkid.com" }, new List<object>() { att }, new List<string>() { "shenhao@sxkid.com", "jeffrey.hu@sxkid.com", "yuanxinying@sxkid.com", "ken@sxkid.com" }.ToArray());
            }

            return ResponseResult.Success();
        }

    }
}
