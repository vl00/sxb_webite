using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PMS.Infrastructure.Application.IService;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Domain.MongoModel;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.AspNetCoreHelper.Filters;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MongoDb;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Tool.Email;
using Sxb.Inside.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class PaidQAController : Controller
    {
        private readonly IPayOrderService _payOrderService;
        private readonly IEmailClient _emailClient;
        IUserService _userService;
        PaidQADBContext _paidDB;
        FinanceDBContext _financeDB;
        LogDBContext _logDBContext;
        IDataExportService _dataExportService;
        IMongoService<IStatisticsMongoProvider> _mongoService;

        public PaidQAController(IPayOrderService payOrderService
            , IEmailClient emailClient
            , PaidQADBContext paidQADBContext
            , IUserService userService
            , IDataExportService dataExportService
            , FinanceDBContext financeDB, LogDBContext logDBContext, IMongoService<IStatisticsMongoProvider> mongoService)
        {
            _mongoService = mongoService;
            _userService = userService;
            _paidDB = paidQADBContext;
            _payOrderService = payOrderService;
            _emailClient = emailClient;
            _dataExportService = dataExportService;
            _financeDB = financeDB;
            _logDBContext = logDBContext;
        }

        [HttpPost]
        public async Task<ResponseResult> SendPayOrderNumberEmail()
        {
            try
            {
                int count = _payOrderService.GetPayedOrderCount();
                string subject = "每日付费问答付款数量统计";
                string body = $"截止至{DateTime.Now:yyyy年MM月dd日}18点前的支付成功订单{count}笔。";
                List<string> toAddr = new List<string>
                {
                    "water@sxkid.com"
                };
                List<string> ccAddr = new List<string> {
                    "quanrw@sxkid.com","shenhao@sxkid.com","yuanxinying@sxkid.com","kison@sxkid.com","ken@sxkid.com"
                };

                await _emailClient.NotifyByMailAsync(subject, body, toAddr.ToArray(), null, ccAddr.ToArray());

                return ResponseResult.Success("发送成功");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ResponseResult> ExportPaidQAEmail()
        {
            var result = ResponseResult.Success();
            var endTime = DateTime.Now;
            var startTime = endTime.AddDays(-4);
            //var endTime = Convert.ToDateTime("2021-07-12");
            //var startTime = Convert.ToDateTime("2021-06-25");//Status = 4 And
            var str_Orders_SQL = $"Select * from [Order] WHERE Status = 4 And CreateTime >= @startTime And CreateTime <= @endTime Order By CreateTime";
            var finishOrders = await _paidDB.QueryAsync<Order>(str_Orders_SQL, new { startTime, endTime });
            var str_FinishOrderMsgs_SQL = $@"SELECT
	                                            m.* 
                                            FROM
	                                            Message AS m
	                                            LEFT JOIN [Order] AS o ON o.ID = m.OrderID 
                                            WHERE
	                                            o.IsBlocked = 0 
	                                            AND m.IsValid = 1 
	                                            AND m.SenderID != '00000000-0000-0000-0000-000000000000' 
	                                            AND o.Status = 4";
            var finishOrderMsgs = await _paidDB.QueryAsync<Message>(str_FinishOrderMsgs_SQL, new { });
            var str_User_SQL = $@"SELECT
	                                o.ID,
	                                (Select nickname from iSchoolUser.dbo.userinfo WHERE id = o.CreatorID) as [AskerName],
	                                (Select nickname from iSchoolUser.dbo.userinfo WHERE id = o.AnswerID) as [ReplyName]
                                FROM
	                                [Order] AS o
	                                WHERE o.Status = 4
	                                --AND o.Amount > 0.1
	                                ORDER BY AskerName";
            var users = await _paidDB.QueryAsync<OrderUserInfo>(str_User_SQL, new { });
            var str_Evaluate_SQL = $@"SELECT
	                                    e.*
                                    FROM
	                                    [dbo].[Evaluate] AS e
	                                    LEFT JOIN [Order] AS o ON o.ID = e.OrderID 
                                    WHERE
	                                    o.Status = 4";
            var evaluates = await _paidDB.QueryAsync<Evaluate>(str_Evaluate_SQL, new { });

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
            cell.SetCellValue("订单ID");
            cell = headerRow.CreateCell(1);
            cell.SetCellValue("付费用户微信名字");
            cell = headerRow.CreateCell(2);
            cell.SetCellValue("达人名字");
            cell = headerRow.CreateCell(3);
            cell.SetCellValue("付费费用");
            cell = headerRow.CreateCell(4);
            cell.SetCellValue("合计提问及追问次数");
            cell = headerRow.CreateCell(5);
            cell.SetCellValue("类型");
            cell = headerRow.CreateCell(6);
            cell.SetCellValue("QA内容");
            cell = headerRow.CreateCell(7);
            cell.SetCellValue("时间");
            cell = headerRow.CreateCell(8);
            cell.SetCellValue("用户评价");
            cell = headerRow.CreateCell(9);
            cell.SetCellValue("用户评分");
            cell = headerRow.CreateCell(10);
            cell.SetCellValue("订单所属渠道");
            cell = headerRow.CreateCell(11);
            cell.SetCellValue("订单状态（已付费未提问/已超时/进行中/已结束）");
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
            foreach (var order in finishOrders)
            {
                var msgs = finishOrderMsgs.Where(p => p.OrderID == order.ID).OrderBy(p => p.CreateTime);
                var row = sheet.CreateRow(rowIndex++);
                cell = row.CreateCell(0);
                cell.SetCellValue(order.NO);//订单ID
                var user = users.FirstOrDefault(p => p.ID == order.ID);
                if (user != null)
                {
                    cell = row.CreateCell(1);//付费用户微信名字
                    cell.SetCellValue(user.AskerName);
                    cell = row.CreateCell(2);//达人名字
                    cell.SetCellValue(user.ReplyName);
                }
                cell = row.CreateCell(3);//付费费用
                cell.SetCellValue(order.Amount.ToString());
                cell = row.CreateCell(10);//渠道
                cell.SetCellValue((order.OriginType??"").ToString());
                cell = row.CreateCell(11);//订单状态
                cell.SetCellValue(order.Status.Description());
                var evaluate = evaluates.FirstOrDefault(p => p.OrderID == order.ID);
                if (evaluate != null)
                {
                    cell = row.CreateCell(8);//用户评价
                    cell.SetCellValue(evaluate.Content);
                    cell = row.CreateCell(9);//用户评分
                    cell.SetCellValue((evaluate.Score / 2).ToString());
                }

                if (msgs?.Any() == true)
                {
                    cell = row.CreateCell(4);//合计提问及追问次数
                    cell.SetCellValue(msgs.Count(p => p.SenderID == order.CreatorID));
                    if (msgs.Count() > 1)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 0, 0));
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 1, 1));
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 2, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 3, 3));
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 4, 4));
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 8, 8));
                        sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum + msgs.Count() - 1, 9, 9));
                    }
                    foreach (var msg in msgs)
                    {
                        cell = row.CreateCell(5);//类型
                        cell.SetCellValue(msg.SenderID == order.CreatorID ? "Q" : "A");
                        cell = row.CreateCell(6);//QA内容
                        if (!string.IsNullOrWhiteSpace(msg.Content))
                        {
                            try
                            {
                                var messageContent = JsonConvert.DeserializeObject<TXTMessage>(msg.Content);
                                if (messageContent != null) cell.SetCellValue(messageContent.Content);
                            }
                            catch
                            {
                            }
                        }
                        cell = row.CreateCell(7);//时间
                        cell.SetCellValue(msg.CreateTime.ToString());
                        if (msgs.Last() != msg)
                        {
                            row = sheet.CreateRow(rowIndex++);
                        }
                    }
                }
            }

            sheet.AutoSizeColumn(0, true);
            sheet.AutoSizeColumn(1, true);
            sheet.AutoSizeColumn(2, true);
            sheet.AutoSizeColumn(3, true);
            sheet.AutoSizeColumn(4, true);
            sheet.AutoSizeColumn(5, true);
            sheet.AutoSizeColumn(6, true);
            sheet.AutoSizeColumn(7, true);
            sheet.AutoSizeColumn(8, true);
            sheet.AutoSizeColumn(9, true);
            sheet.AutoSizeColumn(10, true);
            sheet.AutoSizeColumn(11, true);

            using (MemoryStream stream = new MemoryStream())
            {
                excelFile.Write(stream, true);
                stream.Seek(0, SeekOrigin.Begin);

                var att = new Attachment(stream, "export.xlsx");
                att.ContentDisposition.FileName = "export.xlsx";
                await _emailClient.NotifyByMailAsync("付费问答导出", "", new string[1] { "water@sxkid.com" }, new List<object>() { att }, new List<string>() { "quanrw@sxkid.com","candy@sxkid.com","shenhao@sxkid.com", "jeffrey.hu@sxkid.com", "kison@sxkid.com", "candy@sxkid.com", "yuanxinying@sxkid.com", "ken@sxkid.com", "weiheng@sxkid.com" }.ToArray());
            }
            return result;
        }
        public class OrderUserInfo
        {
            public Guid ID { get; set; }
            public string AskerName { get; set; }
            public string ReplyName { get; set; }
        }




        [HttpPost]
        [ExportDataFilter(ExportDataToMailRequestParamName = "request")]
        public async Task<ResponseResult> ExportData([FromBody] ExportDataToMailRequest request)
        {

            List<object> attachments = new List<object>();
            DateTime etime = DateTime.Now.Date;
            DateTime btime = etime.AddDays(-7);
            var pageRecordDatas = await _dataExportService.ExportPaidPageRecords(btime, etime);
            var fwRecordDatas = await _dataExportService.ExportPaidFWRecords(btime, etime);
            IDictionary<string, IEnumerable<dynamic>> sheets = new Dictionary<string, IEnumerable<dynamic>>();
            sheets["付费问答页面记录表"] = pageRecordDatas;
            sheets["付费问答渠道记录表"] = fwRecordDatas;
            attachments.Add(new Attachment(contentStream: ExcelHelper.ToExcel(sheets), name: $"付费问答渠道及页面统计数据报表【{DateTime.Now.ToString("yyMMdd")}】.xlsx"));
            var exportUserRecordWeeklyAttachment = await ExportUserRecordWeekly(btime, etime);
            if (exportUserRecordWeeklyAttachment != null)
            {
                attachments.Add(exportUserRecordWeeklyAttachment);
            }
            var exportPaidQAOrdersByWeekAttachment = await ExportPaidQAOrdersByWeek(btime, etime);
            if (exportPaidQAOrdersByWeekAttachment != null)
            {
                attachments.Add(exportPaidQAOrdersByWeekAttachment);
            }

            //await _emailClient.NotifyByMailAsync("用户消费导出", "", new string[1] { "kison@sxkid.com" }, new List<object>() { att }, new List<string>() { "shenhao@sxkid.com", "jeffrey.hu@sxkid.com", "yuanxinying@sxkid.com", "ken@sxkid.com" }.ToArray());
            await _emailClient.NotifyByMailAsync($"付费问答数据统计报表【{DateTime.Now.ToString("yyMMdd")}】", "付费问答数据统计报表。", request.MainMails.ToArray(), attachments, request.CCMails.ToArray());
            return ResponseResult.Success("OK,请查收邮件");
        }



        /// <summary>
        /// 输出用户记录表
        /// <para>v5.3.3 付费咨询用户及业务数据统计</para>
        /// <para>最好每周1凌晨开始统计</para>
        /// </summary>
        /// <returns></returns>
        private async Task<Attachment> ExportUserRecordWeekly(DateTime btime, DateTime etime)
        {
            var users = await _userService.GetLastLoginUsers(btime);
            if (users == null || !users.Any()) return null;
            var userIDs = users.Select(p => p.Id).Distinct();
            var orders = new List<dynamic>();

            var userIDsIndex = 0;
            do
            {
                var partUserIDs = userIDs.Skip(userIDsIndex).Take(500);
                orders.AddRange(await _financeDB.QueryAsync<dynamic>(@"SELECT
	                                                                * 
                                                                FROM
	                                                                [dbo].[PayOrder] 
                                                                WHERE
	                                                                UserId IN @partUserIDs
	                                                                AND UpdateTime >= @btime 
	                                                                AND OrderStatus = 6 
                                                                    AND IsDelete = 0", new { btime, partUserIDs }));
                userIDsIndex += 500;
            } while (userIDsIndex <= userIDs.Count());

            if (orders == null) orders = new List<dynamic>();

            #region Excel初始化
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
            cell.SetCellValue("用户类型");
            cell = headerRow.CreateCell(3);
            cell.SetCellValue("用户来源");
            cell = headerRow.CreateCell(4);
            cell.SetCellValue("活动渠道");
            cell = headerRow.CreateCell(5);
            cell.SetCellValue("3天留存");
            cell = headerRow.CreateCell(6);
            cell.SetCellValue("有无交易");
            cell = headerRow.CreateCell(7);
            cell.SetCellValue("注册时间");
            cell = headerRow.CreateCell(8);
            cell.SetCellValue("最近登录时间");
            cell = headerRow.CreateCell(9);
            cell.SetCellValue("最近消费时间");
            cell = headerRow.CreateCell(10);
            cell.SetCellValue("最近交易距离天数");
            cell = headerRow.CreateCell(11);
            cell.SetCellValue("累计单数");
            cell = headerRow.CreateCell(12);
            cell.SetCellValue("累计交易金额");
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
            #endregion

            var logs = new List<dynamic>();
            var minDate = DateTime.MinValue;
            if (users.Any(p => p.RegTime >= btime))
            {
                minDate = users.Where(p => p.RegTime >= btime).Min(p => p.RegTime);
            }
            if (minDate > DateTime.MinValue)
            {
                minDate = minDate.Date.AddDays(1);
                var registerUserIDs = users.Where(p => p.RegTime >= btime).Select(p => p.Id).Distinct();
                
                var takeIndex = 0;
                do
                {
                    var tmp_IDs = registerUserIDs.Skip(takeIndex).Take(500);
                    logs.AddRange((await _logDBContext.QueryAsync<dynamic>($@"SELECT
	                                                                userId,
	                                                                COUNT(1) as [logCount]
                                                                FROM
	                                                                [dbo].[SxbLogs] 
                                                                WHERE
	                                                                [time] >= @minDate
	                                                                AND [time] < @etime
	                                                                AND userId IN ('{string.Join("','", tmp_IDs)}')
                                                                GROUP BY
	                                                                userId"
                                                                    , new { minDate, etime }
                                                                    , commandTimeout: (int)TimeSpan.FromMinutes(60).TotalSeconds)).ToList());
                    takeIndex += 500;
                } while (takeIndex < registerUserIDs.Count());



                if (logs?.Any() == true) logs = logs.Where(p => p.logCount > 0).ToList();
            }


            foreach (var user in users)
            {
                var row = sheet.CreateRow(rowIndex);
                cell = row.CreateCell(0);
                cell.SetCellValue(user.Id.ToString());
                cell = row.CreateCell(1);
                cell.SetCellValue(user.NickName);
                cell = row.CreateCell(2);
                if (user.RegTime >= btime)
                {
                    if (user.RegTime == user.LoginTime || user.LoginTime == default)
                    {
                        cell.SetCellValue("新注册用户");
                    }
                    else
                    {
                        cell.SetCellValue("新留存用户");
                    }
                }
                else
                {
                    cell.SetCellValue("旧留存用户");
                }
                cell = row.CreateCell(3);//用户来源
                cell.SetCellValue(user.Client.GetDescription());
                cell = row.CreateCell(4);//活动渠道
                cell.SetCellValue(user.Source);
                cell = row.CreateCell(5);//3天留存
                cell.SetCellValue(logs.Any(p => p.userId == user.Id.ToString()) ? "是" : "否");
                cell = row.CreateCell(6);
                cell.SetCellValue(orders.Any(p => p.UserId == user.Id) ? "有" : "无");
                cell = row.CreateCell(7);
                cell.SetCellValue(user.RegTime.ToString("yyyy-MM-dd"));
                cell = row.CreateCell(8);
                cell.SetCellValue(user.LoginTime.ToString("yyyy-MM-dd"));

                if (orders.Any(p => p.UserId == user.Id))
                {
                    cell = row.CreateCell(9);//最近消费时间
                    DateTime lastPayDate = orders.Where(p => p.UserId == user.Id).Max(p => p.CreateTime);
                    cell.SetCellValue(lastPayDate.ToString("yyyy-MM-dd"));
                    cell = row.CreateCell(10);//最近交易距离当前天数
                    cell.SetCellValue((etime - lastPayDate).Days);
                    cell = row.CreateCell(11);//累计单数
                    cell.SetCellValue(orders.Count(p => p.UserId == user.Id));
                    cell = row.CreateCell(12);//累计交易金额
                    cell.SetCellValue(orders.Where(p => p.UserId == user.Id).Sum(p => (decimal)p.TotalAmount).ToString());
                }
                rowIndex++;
            }

            for (int i = 0; i < 13; i++)
            {
                sheet.AutoSizeColumn(i, true);
            }

            MemoryStream stream = new MemoryStream();
            excelFile.Write(stream, true);
            stream.Seek(0, SeekOrigin.Begin);

            var att = new Attachment(stream, "export.xlsx");
            att.ContentDisposition.FileName = "export.xlsx";
            return att;

        }

        private async Task<Attachment> ExportPaidQAOrdersByWeek(DateTime btime, DateTime etime)
        {
            var str_SQL = $"Select * From [Order] Where CreateTime >= @btime AND CreateTime < @etime AND Status = 4";
            var orders = await _paidDB.QueryAsync<Order>(str_SQL, new { btime, etime });
            if (orders == null || !orders.Any()) return null;
            var userIDs = orders.Select(p => p.CreatorID).Distinct();
            var userInfos = await _userService.GetUserInfos(userIDs);
            var anwserUserIDs = orders.Select(p => p.AnswerID).Distinct();
            var talentRegions = await _paidDB.QueryAsync<TalentRegion>("Select * From TalentRegion Where UserID in @anwserUserIDs;", new { anwserUserIDs });
            var regionIDs = talentRegions.Select(p => p.RegionTypeID).Distinct();
            var regions = await _paidDB.QueryAsync<RegionType>("Select * From RegionType Where ID in @regionIDs", new { regionIDs });
            var talentGrades = await _paidDB.QueryAsync<(Guid, Guid)>("Select TalentUserID,GradeID From TalentGrade Where TalentUserID in @anwserUserIDs", new { anwserUserIDs });
            var grades = await _paidDB.QueryAsync<Grade>("Select * From [Grade];", new { });

            #region Excel初始化
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
            cell.SetCellValue("订单ID");
            cell = headerRow.CreateCell(1);
            cell.SetCellValue("用户ID");
            cell = headerRow.CreateCell(2);
            cell.SetCellValue("用户昵称");
            cell = headerRow.CreateCell(3);
            cell.SetCellValue("渠道");
            cell = headerRow.CreateCell(4);
            cell.SetCellValue("擅长领域");
            cell = headerRow.CreateCell(5);
            cell.SetCellValue("学段");
            cell = headerRow.CreateCell(6);
            cell.SetCellValue("交易金额");
            cell = headerRow.CreateCell(7);
            cell.SetCellValue("交易时间");
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
            #endregion

            foreach (var order in orders)
            {
                var row = sheet.CreateRow(rowIndex++);
                cell = row.CreateCell(0);
                cell.SetCellValue(order.ID.ToString());
                cell = row.CreateCell(1);
                cell.SetCellValue(order.CreatorID.ToString());
                cell = row.CreateCell(2);
                if (userInfos.Any(p => p.Id == order.CreatorID)) cell.SetCellValue(userInfos.FirstOrDefault(p => p.Id == order.CreatorID).NickName);
                cell = row.CreateCell(3);
                cell.SetCellValue(order.OriginType);
                cell = row.CreateCell(4);
                var talentRegionIDs = talentRegions.Where(p => p.UserID == order.AnswerID).Select(p => p.RegionTypeID);
                var row_Regions = regions.Where(p => talentRegionIDs.Contains(p.ID));
                cell.SetCellValue(string.Join("、", row_Regions.Select(p => p.Name)));
                cell = row.CreateCell(5);
                var talentGradeIDs = talentGrades.Where(p => p.Item1 == order.AnswerID).Select(p => p.Item2);
                var row_Grades = grades.Where(p => talentGradeIDs.Contains(p.ID));
                cell.SetCellValue(string.Join("、", row_Grades.Select(p => p.Name)));
                cell = row.CreateCell(6);
                if (order.PayAmount.HasValue) cell.SetCellValue(order.PayAmount.ToString());
                cell = row.CreateCell(7);
                cell.SetCellValue(order.CreateTime.ToString("yyyy-MM-dd"));
            }

            for (int i = 0; i < 8; i++)
            {
                sheet.AutoSizeColumn(i, true);
            }

            var fileName = $"订单记录表-{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";

            MemoryStream stream = new MemoryStream();
            excelFile.Write(stream, true);
            stream.Seek(0, SeekOrigin.Begin);

            var att = new Attachment(stream, fileName);
            att.ContentDisposition.FileName = fileName;
            return att;



        }


        #region 测评数据导出
        public async Task<ResponseResult> ExportAssessReport(DateTime? countingTime)
        {
            var result = ResponseResult.Failed();
            if (!countingTime.HasValue) countingTime = DateTime.Now.Date;//统计昨日
            var statistics = await _paidDB.QueryAsync<AssessStatisticsInfo>("SELECT * FROM [dbo].[AssessStatisticsInfo] WHERE CountTime = @countingTime;", new { countingTime });

            var excelFile = new XSSFWorkbook();
            var headerStyle = excelFile.CreateCellStyle();
            headerStyle.IsLocked = true;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.FillForegroundColor = HSSFColor.Yellow.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            var headerFontStyle = excelFile.CreateFont();
            headerFontStyle.IsBold = true;
            headerStyle.SetFont(headerFontStyle);

            await GenerateAssessStatistics1(excelFile.CreateSheet("业务数据统计"), headerStyle, countingTime.Value.AddDays(-7));
            await GenerateAssessStatistics2(excelFile.CreateSheet(AssessType.Type1.GetDescription().Replace("：", "_")), headerStyle, countingTime.Value.AddDays(-7));
            await GenerateAssessStatistics3(excelFile.CreateSheet(AssessType.Type2.GetDescription().Replace("：", "_")), headerStyle, countingTime.Value.AddDays(-7), AssessType.Type2);
            await GenerateAssessStatistics3(excelFile.CreateSheet(AssessType.Type3.GetDescription().Replace("：", "_")), headerStyle, countingTime.Value.AddDays(-7), AssessType.Type3);
            await GenerateAssessStatistics3(excelFile.CreateSheet(AssessType.Type4.GetDescription().Replace("：", "_")), headerStyle, countingTime.Value.AddDays(-7), AssessType.Type4);

            if (System.IO.File.Exists("r:/export.xlsx")) System.IO.File.Delete("r:/export.xlsx");
            using (var fs = System.IO.File.OpenWrite(@"r:/export.xlsx"))
            {
                excelFile.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
                Console.WriteLine("生成成功");
            }

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    excelFile.Write(stream, true);
            //    stream.Seek(0, SeekOrigin.Begin);

            //    var att = new Attachment(stream, "export.xlsx");
            //    att.ContentDisposition.FileName = "export.xlsx";
            //    await _emailClient.NotifyByMailAsync("付费问答导出", "", new string[1] { "water@sxkid.com" }, new List<object>() { att }, new List<string>() { "shenhao@sxkid.com", "jeffrey.hu@sxkid.com", "kison@sxkid.com", "yuanxinying@sxkid.com", "ken@sxkid.com" }.ToArray());
            //}
            result = ResponseResult.Success();
            return result;
        }

        async Task GenerateAssessStatistics1(ISheet sheet, ICellStyle headerStyle, DateTime startTime)
        {
            var endTime = startTime.AddDays(8).Date;
            var statistics = await _paidDB.QueryAsync<AssessStatisticsInfo>("SELECT * FROM [dbo].[AssessStatisticsInfo] WHERE CountTime >= @startTime AND CountTime < @endTime;", new { startTime, endTime });

            var headerRow = sheet.CreateRow(0);
            var cell = headerRow.CreateCell(0);
            cell.SetCellValue("测评主题");
            cell = headerRow.CreateCell(1);
            cell.SetCellValue("统计日期");
            cell = headerRow.CreateCell(2);
            cell.SetCellValue("首页访问总UV/日");
            cell = headerRow.CreateCell(3);
            cell.SetCellValue("首页访问总PV/日");
            cell = headerRow.CreateCell(4);
            cell.SetCellValue("第一题页面跳出率/日");
            cell = headerRow.CreateCell(5);
            cell.SetCellValue("完成所有题目总UV/日");
            cell = headerRow.CreateCell(6);
            cell.SetCellValue("完成所有题目总PV/日");
            cell = headerRow.CreateCell(7);
            cell.SetCellValue("服务号新增关注数/日");
            cell = headerRow.CreateCell(8);
            cell.SetCellValue("付款成功订单数/日");

            foreach (var item in headerRow.Cells)
            {
                item.CellStyle = headerStyle;
            }
            var rowIndex = 1;

            foreach (var item in statistics.OrderByDescending(p => p.CountTime).ThenBy(p => p.AssessType))
            {
                var row = sheet.CreateRow(rowIndex++);
                cell = row.CreateCell(0);
                cell.SetCellValue(item.AssessType.GetDescription());
                cell = row.CreateCell(1);
                cell.SetCellValue(item.CountTime.ToString("yyyy-MM-dd"));
                cell = row.CreateCell(2);
                cell.SetCellValue(item.HomeIndexUV);
                cell = row.CreateCell(3);
                cell.SetCellValue(item.HomeIndexPV);
                cell = row.CreateCell(4);
                cell.SetCellValue(item.FirstPagePopoutPercent.ToString());
                cell = row.CreateCell(5);
                cell.SetCellValue(item.FinishUV);
                cell = row.CreateCell(6);
                cell.SetCellValue(item.FinishPV);
                cell = row.CreateCell(7);
                cell.SetCellValue(item.FollowCount);
                cell = row.CreateCell(8);
                cell.SetCellValue(item.PayCount);
            }

            for (int i = 0; i <= 8; i++)
            {
                sheet.AutoSizeColumn(i, true);
            }
        }

        async Task GenerateAssessStatistics2(ISheet sheet, ICellStyle headerStyle, DateTime startTime)
        {
            var endTime = startTime.AddDays(8).Date;


            var assesses = await _paidDB.QueryAsync<AssessInfo>("Select * From AssessInfo Where Type = 1 AND Status = 2 AND CreateTime >= @startTime AND CreateTime < @endTime;", new { startTime, endTime });
            if (assesses == null || !assesses.Any()) return;
            var questions = (await _paidDB.QueryAsync<AssessQuestionInfo>("Select * From [AssessQuestionInfo] Where AssessType = 1 Order By [Level]", new { })).ToList();
            var userIDs = assesses.Select(p => p.UserID).Distinct();
            var userInfos = await Task.Run(() =>
            {
                return _userService.GetUserInfos(userIDs);
            });
            var optionIDs = new List<Guid>();
            foreach (var item in assesses)
            {
                try
                {
                    var obj_SelectOption = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, object>>(item.SelectedOptionShortIDs);
                    if (obj_SelectOption?.Any() == true)
                    {
                        foreach (var selected in obj_SelectOption)
                        {

                            if (selected.Value?.GetType().Name == "JArray")
                            {
                                optionIDs.AddRange(JsonConvert.DeserializeObject<IEnumerable<Guid>>(selected.Value.ToString()));
                            }
                        }
                    }
                }
                catch { }
            }
            optionIDs = optionIDs.Distinct().ToList();
            var optionInfos = await _paidDB.QueryAsync<AssessOptionInfo>("Select * From AssessOptionInfo Where ID in @optionIDs", new { optionIDs });

            var titleRow = sheet.CreateRow(0);
            var cell = titleRow.CreateCell(0);
            cell.SetCellValue($"测评主题：{assesses.First().Type.GetDescription()}（统计日期：{startTime.ToString("yyyy-MM-dd")} 至 {endTime.ToString("yyyy-MM-dd")})");
            var titleMargeRegion = new CellRangeAddress(0, 0, 0, 4);
            sheet.AddMergedRegion(titleMargeRegion);
            var headerRow = sheet.CreateRow(1);
            cell = headerRow.CreateCell(0);
            cell.SetCellValue("用户昵称");
            cell = headerRow.CreateCell(1);
            cell.SetCellValue(questions[0].Content);
            cell = headerRow.CreateCell(2);
            cell.SetCellValue(questions[1].Content);
            cell = headerRow.CreateCell(3);
            cell.SetCellValue(questions[2].Content);
            cell = headerRow.CreateCell(4);
            cell.SetCellValue(questions[3].Content);

            var rowIndex = 2;

            foreach (var assess in assesses.OrderByDescending(p => p.CreateTime))
            {
                var row = sheet.CreateRow(rowIndex++);
                cell = row.CreateCell(0);
                if (userInfos.Any(p => p.Id == assess.UserID)) cell.SetCellValue(userInfos.FirstOrDefault(p => p.Id == assess.UserID).NickName);
                var dic_Options = new Dictionary<int, IEnumerable<Guid>>();
                var dic_dynamic = new Dictionary<int, dynamic>();
                dic_dynamic = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(assess.SelectedOptionShortIDs);
                foreach (var item in dic_dynamic)
                {
                    try
                    {
                        dic_Options.Add(item.Key, JsonConvert.DeserializeObject<IEnumerable<Guid>>(item.Value.ToString()));
                    }
                    catch { }
                }
                cell = row.CreateCell(1);
                var tmp = optionInfos.Where(p => dic_Options.FirstOrDefault(x => x.Key == 1).Value.Contains(p.ID));
                cell.SetCellValue(string.Join("\\n", optionInfos.Where(p => dic_Options.FirstOrDefault(x => x.Key == 1).Value.Contains(p.ID)).Select(p => p.Content)));
                cell = row.CreateCell(2);
                cell.SetCellValue(string.Join("\\n", optionInfos.Where(p => dic_Options.FirstOrDefault(x => x.Key == 2).Value.Contains(p.ID)).Select(p => p.Content)));
                cell = row.CreateCell(3);
                cell.SetCellValue(string.Join("\\n", optionInfos.Where(p => dic_Options.FirstOrDefault(x => x.Key == 3).Value.Contains(p.ID)).Select(p => p.Content)));
                cell = row.CreateCell(4);
                cell.SetCellValue(dic_dynamic.Last().Value == null ? "" : dic_dynamic.Last().Value);
            }

            for (int i = 0; i <= 8; i++)
            {
                sheet.AutoSizeColumn(i, true);
            }
        }

        async Task GenerateAssessStatistics3(ISheet sheet, ICellStyle headerStyle, DateTime startTime, AssessType assessType)
        {
            var endTime = startTime.AddDays(8).Date;


            var assesses = await _paidDB.QueryAsync<AssessInfo>("Select * From AssessInfo Where Type = @type AND Status = 2 AND CreateTime >= @startTime AND CreateTime < @endTime;",
                new { type = (int)assessType, startTime, endTime });
            if (assesses == null || !assesses.Any()) return;
            var questions = (await _paidDB.QueryAsync<AssessQuestionInfo>("Select * From [AssessQuestionInfo] Where AssessType = @assessType Order By [Level]", new { assessType })).ToList();
            var questionIDs = questions.Select(p => p.ID);
            var optionRelates = await _paidDB.QueryAsync<AssessOptionRelationInfo>("Select * From [AssessOptionRelationInfo] Where QuestionID in @questionIDs", new { questionIDs });
            var optionShortIDs = new List<int>();
            foreach (var relate in optionRelates)
            {
                optionShortIDs.AddRange(JsonConvert.DeserializeObject<IEnumerable<int>>(relate.NextOptionShortIDs));
            }
            var shortIDs = optionShortIDs.Distinct().ToList();
            var optionInfos = await _paidDB.QueryAsync<AssessOptionInfo>("Select * From AssessOptionInfo Where ShortID in @shortIDs", new { shortIDs });
            var dic_SelectedOptions = new Dictionary<int, List<Guid>>();
            foreach (var assess in assesses)
            {
                var tmp_SelectOption = JsonConvert.DeserializeObject<Dictionary<int, IEnumerable<Guid>>>(assess.SelectedOptionShortIDs);
                foreach (var item in tmp_SelectOption)
                {
                    if (!dic_SelectedOptions.ContainsKey(item.Key)) dic_SelectedOptions[item.Key] = new List<Guid>();
                    dic_SelectedOptions[item.Key].AddRange(item.Value);
                }
            }


            var titleRow = sheet.CreateRow(0);
            var cell = titleRow.CreateCell(0);
            cell.SetCellValue($"测评主题：{assessType.GetDescription()}（统计日期：{startTime.ToString("yyyy-MM-dd")} 至 {endTime.ToString("yyyy-MM-dd")})");
            var titleMargeRegion = new CellRangeAddress(0, 0, 0, 4);
            sheet.AddMergedRegion(titleMargeRegion);
            var headerRow = sheet.CreateRow(1);
            cell = headerRow.CreateCell(0);
            cell.SetCellValue("题目");
            cell = headerRow.CreateCell(1);
            cell.SetCellValue("选项");
            cell = headerRow.CreateCell(2);
            cell.SetCellValue("选项次数");

            var rowIndex = 2;

            foreach (var item in questions.OrderBy(p => p.Level))
            {
                var row = sheet.CreateRow(rowIndex++);
                cell = row.CreateCell(0);
                cell.SetCellValue(item.Content);
                var options = optionInfos.Where(p => JsonConvert.DeserializeObject<IEnumerable<int>>(optionRelates.FirstOrDefault(x => x.QuestionID == item.ID).NextOptionShortIDs).Contains(p.ShortID))
                    .OrderBy(p => p.ShortID);
                foreach (var option in options)
                {
                    cell = row.CreateCell(1);
                    cell.SetCellValue(option.Content);
                    cell = row.CreateCell(2);
                    cell.SetCellValue(dic_SelectedOptions[item.Level].Count(p => p == option.ID));
                    if (option != options.Last())
                    {
                        row = sheet.CreateRow(rowIndex++);
                        cell = row.CreateCell(0);
                        cell.SetCellValue("");
                    }
                }
            }

            for (int i = 0; i <= 8; i++)
            {
                sheet.AutoSizeColumn(i, true);
            }
        }

        #endregion


        #region 运营渠道数据导出
        public async Task<ResponseResult> ChannelDataExportCrontab()
        {
            var result = ResponseResult.Failed();
            var tasks = await _paidDB.QueryAsync<ChannelDataExportTaskInfo>("Select * From [ChannelDataExportTaskInfo] Where [Status] < 2;", new { });
            if (tasks?.Any() == true)
            {
                foreach (var task in tasks)
                {
                    if (task.Status > 1) continue;
                    if (_paidDB.Execute("Update [ChannelDataExportTaskInfo] Set status = 2 Where [ID] = @ID And Status < 2;", new { task.ID }) > 0)
                    {
                        task.Status = 2;
                        var startTime = task.StartTime.Date;
                        var endTime = task.EndTime.AddDays(1).Date;
                        var orders = await _paidDB.QueryAsync<Order>("Select * From [Order] Where [CreateTime] >= @startTime AND [CreateTime] < @endTime AND [OriginType] = @Channel AND [Status] > 1",
                            new { startTime, endTime, task.Channel });
                        var orderIDs = orders.Select(p => p.ID).Distinct();
                        //var orderPayTimes = await _financeDB.QueryAsync<(Guid, DateTime)>(@"SELECT
                                                                                            //    po.OrderId,
                                                                                            //    wxcl.SuccessTime
                                                                                            //FROM
                                                                                            //    WxPayCallBackLog AS wxcl
                                                                                            //    LEFT JOIN PayOrder AS po ON po.OrderNo = wxcl.OutTradeNo
                                                                                            //WHERE
                                                                                            //    po.System = 0
                                                                                            //    AND po.IsDelete = 0
                                                                                            //    AND po.OrderID in @orderIDs", new { orderIDs });
                        var pv = _mongoService.ImongdDb.GetCollection<statistics>(nameof(statistics)).Aggregate().
                            Match(p => p.fw == task.Channel &&p.time >= startTime && p.time < endTime && p.url == task.Url).
                            Project(p => new statistics()
                            {
                                fw = p.fw,
                                url = p.url,
                                time = p.time
                            }).ToList();

                        #region Excel
                        var excelFile = new XSSFWorkbook();
                        var sheet = excelFile.CreateSheet("总计表");
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
                        cell.SetCellValue("渠道名称");
                        cell = headerRow.CreateCell(1);
                        cell.SetCellValue("日期（以天为单位）");
                        cell = headerRow.CreateCell(2);
                        cell.SetCellValue("扫码数（pv）");
                        cell = headerRow.CreateCell(3);
                        cell.SetCellValue("付费人数");
                        cell = headerRow.CreateCell(4);
                        cell.SetCellValue("付费订单数");
                        cell = headerRow.CreateCell(5);
                        cell.SetCellValue("付费总计金额");
                        foreach (var item in headerRow.Cells)
                        {
                            item.CellStyle = headerStyle;
                        }
                        var rowIndex = 1;
                        foreach (var date in pv.Select(p => p.time.Date).Distinct().OrderByDescending(p => p))
                        {
                            var row = sheet.CreateRow(rowIndex);
                            cell = row.CreateCell(0);
                            cell.SetCellValue(task.Channel);
                            cell = row.CreateCell(1);
                            cell.SetCellValue(date.ToShortDateString());
                            cell = row.CreateCell(2);
                            cell.SetCellValue(pv.Count(p => p.time.Date == date));
                            cell = row.CreateCell(3);
                            cell.SetCellValue(orders.Where(p => p.Status > OrderStatus.WaitingAsk && p.CreateTime.Date == date).Select(p => p.CreatorID).Distinct().Count());
                            cell = row.CreateCell(4);
                            cell.SetCellValue(orders.Count(p => p.Status > OrderStatus.WaitingAsk && p.CreateTime.Date == date));
                            cell = row.CreateCell(5);
                            cell.SetCellValue(orders.Where(p => p.Status > OrderStatus.WaitingAsk && p.CreateTime.Date == date).Sum(p => p.Amount).ToString("f2"));
                            rowIndex++;
                        }


                        //Sheet2 -> 明细表
                        sheet = excelFile.CreateSheet("明细表");
                        headerRow = sheet.CreateRow(0);
                        cell = headerRow.CreateCell(0);
                        cell.SetCellValue("付款日期");
                        cell = headerRow.CreateCell(1);
                        cell.SetCellValue("付费订单所属渠道");
                        cell = headerRow.CreateCell(2);
                        cell.SetCellValue("付费用户微信名称");
                        cell = headerRow.CreateCell(3);
                        cell.SetCellValue("达人名称");
                        cell = headerRow.CreateCell(4);
                        cell.SetCellValue("付费费用");
                        cell = headerRow.CreateCell(5);
                        cell.SetCellValue("订单状态（已结束/已退款/进行中）");
                        foreach (var item in headerRow.Cells)
                        {
                            item.CellStyle = headerStyle;
                        }
                        rowIndex = 1;

                        var userIDs = new List<Guid>();
                        userIDs.AddRange(orders.Select(p => p.CreatorID).Distinct());
                        userIDs.AddRange(orders.Select(p => p.AnswerID).Distinct());
                        userIDs = userIDs.Distinct().ToList();
                        var userinfos = await _userService.GetUserInfos(userIDs);
                        foreach (var order in orders.OrderByDescending(p => p.CreateTime))
                        {
                            var row = sheet.CreateRow(rowIndex);
                            cell = row.CreateCell(0);
                            cell.SetCellValue(order.CreateTime.ToShortDateString());
                            cell = row.CreateCell(1);
                            cell.SetCellValue(task.Channel);
                            cell = row.CreateCell(2);
                            cell.SetCellValue(userinfos.FirstOrDefault(p => p.Id == order.CreatorID).NickName);
                            cell = row.CreateCell(3);
                            cell.SetCellValue(userinfos.FirstOrDefault(p => p.Id == order.AnswerID).NickName);
                            cell = row.CreateCell(4);
                            cell.SetCellValue(order.Amount.ToString());
                            cell = row.CreateCell(5);
                            cell.SetCellValue(order.Status.GetDescription());
                            rowIndex++;
                        }

                        //Upload Cos

                        var fileName = $"上学问渠道数据统计表{task.StartTime.ToString("yyyyMMdd")}-{task.EndTime.ToString("yyyyMMdd")}.xlsx";
                        var dirPath = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName + $"\\export\\{task.ID}\\";
                        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                        var filePath = dirPath + fileName;


                        //Update task

                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                        using (var fs = System.IO.File.OpenWrite(filePath))
                        {
                            excelFile.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
                            Console.WriteLine("生成成功");
                        }

                        task.FileUrl = filePath;
                        task.Status = 3;
                        if (_paidDB.Update(task))
                        {
                            result = ResponseResult.Success();
                        }
                        #endregion
                    }
                }
            }
            return result;
        }

        public IActionResult DownloadChannelDataExport(Guid id)
        {
            var result = ResponseResult.Failed();
            if (id == Guid.Empty) result.Msg = "Param error";
            var find = _paidDB.QuerySingle<ChannelDataExportTaskInfo>("Select * From [ChannelDataExportTaskInfo] Where [ID] = @id", new { id });
            if (find?.Status == 3)
            {
                return File(System.IO.File.ReadAllBytes(find.FileUrl), "application/octet-stream",
                    $"上学问渠道数据统计表{find.StartTime.ToString("yyyyMMdd")}-{find.EndTime.ToString("yyyyMMdd")}.xlsx", false);
            }
            return Json(result);
        }
        #endregion
    }
}
