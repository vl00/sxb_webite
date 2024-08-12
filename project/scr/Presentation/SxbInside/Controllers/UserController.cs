using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using Sxb.Inside.Response;
using PMS.CommentsManage.Domain.Common;
using Sxb.Inside.ViewModels.Comment;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using ProductManagement.Framework.Foundation;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        LogDBContext _logDBContext;
        FinanceDBContext _financeDB;
        public UserController(IUserService userService, FinanceDBContext financeDB, LogDBContext logDBContext)
        {
            _logDBContext = logDBContext;
            _financeDB = financeDB;
            _userService = userService;
        }

        /// <summary>
        /// 批量获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult GetUsers([FromBody] List<Guid> userIds)
        {
            var users = _userService.ListUserInfo(userIds);
            return ResponseResult.Success(users.Select(user => new { user.Id, user.HeadImgUrl, user.Mobile, user.NickName }));
        }
        /// <summary>
        /// 批量获取用户openid
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult GetUserOpenId([FromBody] List<Guid> userIds)
        {
            var openids = _userService.ListUserOpenId(userIds);
            return ResponseResult.Success(openids);
        }
        [HttpGet]
        [Route("[controller]/SamplePhoneUserIds/{userId}")]
        public ResponseResult GetSamplePhoneUserIds(Guid userId)
        {
            var userIds = _userService.GetSamplePhoneUserIds(userId);
            return ResponseResult.Success(userIds);
        }


        [HttpPost]
        [Route("[controller]/SamplePhoneUserIds")]
        public ResponseResult GetSamplePhoneUserIds([FromBody] List<Guid> userIds)
        {
            var result = userIds.Select(userId =>
            {
                var relationUserIds = _userService.GetSamplePhoneUserIds(userId);
                return new
                {
                    UserId = userId,
                    RelationUserIds = relationUserIds
                };
            });
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 根据手机号获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/phone/{phone}")]
        public ResponseResult GetUsersByPhone(string phone)
        {
            var users = _userService.GetUserInfosByPhone(phone);
            return ResponseResult.Success(users.Select(user => new { user.Id, user.HeadImgUrl, user.Mobile, user.NickName }));
        }


        /// <summary>
        /// 根据nickname获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/name/{name}")]
        public ResponseResult GetUsersByName(string name)
        {
            var users = _userService.GetUserInfosByName(name);
            return ResponseResult.Success(users.Select(user => new { user.Id, user.HeadImgUrl, user.Mobile, user.NickName }));
        }

        /// <summary>
        /// 输出用户记录表
        /// <para>v5.3.3 付费咨询用户及业务数据统计</para>
        /// <para>最好每周1凌晨开始统计</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> ExportUserRecordWeekly()
        {
            var result = ResponseResult.Failed();
            var date_Now = DateTime.Now.Date;
            var startTime = date_Now.AddDays(-7);
            var users = await _userService.GetLastLoginUsers(startTime);
            if (users == null || !users.Any()) return result;
            var userIDs = users.Select(p => p.Id).Distinct();
            var orders = await _financeDB.QueryAsync<dynamic>(@"SELECT
	                                                                * 
                                                                FROM
	                                                                [dbo].[PayOrder] 
                                                                WHERE
	                                                                UserId IN (Select ID From iSchoolUser.dbo.UserInfo Where LoginTime >= @startTime)
	                                                                AND UpdateTime >= @startTime 
	                                                                AND OrderStatus = 6 
                                                                    AND IsDelete = 0", new { userIDs, startTime });
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
            if (users.Any(p => p.RegTime >= startTime))
            {
                minDate = users.Where(p => p.RegTime >= startTime).Min(p => p.RegTime);
            }
            if (minDate > DateTime.MinValue)
            {
                minDate = minDate.Date.AddDays(1);
                var registerUserIDs = users.Where(p => p.RegTime >= startTime).Select(p => p.Id).Distinct();

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
	                                                                AND [time] < @date_Now
	                                                                AND userId IN ('{string.Join("','", tmp_IDs)}')
                                                                GROUP BY
	                                                                userId", new { minDate, date_Now })).ToList());
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
                if (user.RegTime >= startTime)
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
                    cell.SetCellValue((date_Now - lastPayDate).Days);
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
            result = ResponseResult.Success();

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
            //    await _emailClient.NotifyByMailAsync("用户消费导出", "", new string[1] { "kison@sxkid.com" }, new List<object>() { att }, new List<string>() { "shenhao@sxkid.com", "jeffrey.hu@sxkid.com", "yuanxinying@sxkid.com", "ken@sxkid.com" }.ToArray());
            //}

            return result;
        }

    }
}
