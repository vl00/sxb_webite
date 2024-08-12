using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using Sxb.Web.Areas.PaidQA.Models.Question;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PMS.UserManage.Repository;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Foundation;
using Microsoft.AspNetCore.SignalR;
using PMS.PaidQA.Repository;
using System.Text;
using Sxb.GenerateNo;
using ProductManagement.Infrastructure.Configs;
using WeChat;
using PMS.UserManage.Application.IServices;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Interface;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.ComponentModel;
using TencentCloud.Sms.V20190711;
using ProductManagement.API.SMS;
using PMS.MediatR.Events.PaidQA;
using Message = PMS.PaidQA.Domain.Entities.Message;
using WeChat.Interface;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.PaidQA.Controllers
{
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        IUserService userService;
        IWeChatAppClient weChatAppClient;
        CustomMsgSetting _customMsgSetting;
        WechatMessageTplSetting _wechatMessageTplSetting;
        ICouponTakeService _couponTakeService;
        IFileServiceClient fileServiceClient;
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private readonly IMediator _mediator;
        IMessageService _messageService;
        IOrderService _OrderService;
        ITalentSettingService _talentSettingService;
        ISxbGenerateNo _sxbGenerateNo;
        ICouponActivityService _couponActivityService;
        SmsClient _smsClient;
        UserDbContext _userDB;
        PaidQADBContext _paidDB;
        ITencentSmsService _tencentSmsService;
        PaidQAOption _paidQAOption;
        ICustomMsgService _customMsgService;
        public DemoController(IMediator mediator, IMessageService messageService, IOrderService orderService, ITalentSettingService talentSettingService, UserDbContext userDbContext,
            PaidQADBContext paidQADBContext, ISxbGenerateNo sxbGenerateNo
            , IOptions<PaidQAOption> options
            , IUserService userService
            , IWeChatAppClient weChatAppClient
            , IFinanceCenterServiceClient financeCenterServiceClient
            , ICouponTakeService couponTakeService
            , ICouponActivityService couponActivityService
            , IFileServiceClient fileServiceClient
            , ITencentSmsService tencentSmsService
            , ICustomMsgService customMsgService)

        {
            _sxbGenerateNo = sxbGenerateNo;
            _paidDB = paidQADBContext;
            _userDB = userDbContext;
            _talentSettingService = talentSettingService;
            _messageService = messageService;
            _OrderService = orderService;
            _mediator = mediator;
            _paidQAOption = options.Value;
            _customMsgSetting = options.Value.CustomMsgSetting;
            _wechatMessageTplSetting = options.Value.WechatMessageTplSetting;
            this.userService = userService;
            this.weChatAppClient = weChatAppClient;
            _couponTakeService = couponTakeService;
            _couponActivityService = couponActivityService;
            this.fileServiceClient = fileServiceClient;
            _tencentSmsService = tencentSmsService;
            _customMsgService = customMsgService;
        }
#if DEBUG
        public async Task<ResponseResult> TestTencentSms()
        {
            await _tencentSmsService.SendSmsAsync("+8618218943980", _paidQAOption.MobileMsgTplSetting.CreateQuestionNotify.tplid, _paidQAOption.MobileMsgTplSetting.CreateQuestionNotify.tplParams.ToArray());
            return ResponseResult.Success();
        }

        [HttpPost]
        public async Task<ResponseResult> SendTextMessage([FromBody] TextMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.Text,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }


        [HttpPost]
        public async Task<ResponseResult> SendImageMessage([FromBody] ImageMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.Image,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }

        [HttpPost]
        public async Task<ResponseResult> SendTXTMessage([FromBody] TXTMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.TXT,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }

        [HttpPost]
        public async Task<ResponseResult> SendCustomMessage([FromBody] CustomMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.Custom,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }


        [HttpPost]
        public async Task<ResponseResult> SendRandomTenlentCardMessage([FromBody] RandomTenlentCardMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.RandomTenlentCard,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }

        [HttpPost]
        public async Task<ResponseResult> SendRecommandCardMessage([FromBody] RecommandCardMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.RecommandCard,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID
            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }

        [HttpPost]
        public async Task<ResponseResult> SendSystemMessage([FromBody] SystemMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.System,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }
        [HttpPost]
        public async Task<ResponseResult> SendWechatMessage()
        {
            Guid userID = User.Identity.GetUserInfo().UserId;
            if (userService.TryGetOpenId(userID, "fwh", out string openID))
            {
                var cmd = new AskWechatTemplateSendRequest() { OrderID = default(Guid), first = "", keyword1 = "ben", keyword2 = "老师", keyword3 = DateTime.Now.ToString("yyyy-dd-mm HH:mm:ss"), remark = "我是内容", openid = openID, msgtype = WechatMessageType.专家回复问题 };
                var addResult = await _mediator.Send(cmd);
            }

            return ResponseResult.Success("");
        }
        [HttpPost]
        public async Task<ResponseResult> SendSystemStatuMessage([FromBody] SystemStatuMessage text)
        {
            var order = _OrderService.Get(Guid.Parse("D7FC4663-A188-4099-A705-B26AD51B67D9"));
            PMS.PaidQA.Domain.Entities.Message message = new PMS.PaidQA.Domain.Entities.Message()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(text, jsonSerializerSettings),
                OrderID = order.ID,
                MediaType = PMS.PaidQA.Domain.Enums.MsgMediaType.SystemStatu,
                SenderID = order.CreatorID,
                ReceiveID = order.AnswerID

            };
            var addResult = await _messageService.SendMessage(message);
            return ResponseResult.Success(addResult);
        }


        public async Task<ResponseResult> GetSimilarTalents(Guid talentID)
        {
            var result = ResponseResult.Success();
            var finds = await _talentSettingService.GetSimilarTalents(talentID);
            if (finds?.Any() == true)
            {
                result.Data = finds;
            }
            return result;
        }

        [HttpPost]
        public async Task<ResponseResult> UploadTalents()
        {
            var result = ResponseResult.Success();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var userNames = new List<string>();
            var authNames = new List<string>();
            var levelNames = new List<string>();
            var gradeNames = new List<string[]>();
            var regionNames = new List<string[]>();
            var prices = new List<decimal>();
            var introductions = new List<string>();
            var headImageNames = new List<string>();
            var emails = new List<string>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                //userNames.Add(row.Cells.First().StringCellValue?.Trim());
                var username = row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim();
                if (!string.IsNullOrWhiteSpace(username))
                {
                    userNames.Add(username);
                    authNames.Add(row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim());
                    levelNames.Add(row.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim());
                    gradeNames.Add(row.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim().Split("\n"));
                    regionNames.Add(row.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim().Split("\n"));
                    prices.Add((decimal)row.GetCell(8, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.NumericCellValue);
                    introductions.Add(row.GetCell(9, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim());
                    emails.Add(row.GetCell(12, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim());
                    headImageNames.Add(row.GetCell(13, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim());
                }
            }
            var str_SQL = $@"Select ui.* from userInfo as ui JOIN talent as t on t.user_id = ui.id WHERE ui.nickname in @userNames";
            //var str_SQL = $@"Select ui.* from userInfo as ui JOIN talent as t on t.user_id = ui.id";
            var existUserNames = new List<string>();
            var notEmptyUserName = userNames.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct();
            var existUsers = await _userDB.QueryAsync<dynamic>(str_SQL, new { userNames = notEmptyUserName });
            var grades = await _paidDB.QueryAsync<Grade>("Select * from Grade", new { });//全部学段
            var regions = await _paidDB.QueryAsync<RegionType>("Select * from RegionType", new { });
            var levels = await _paidDB.QueryAsync<LevelType>("Select * from LevelType", new { });
            var existUserIDs = new List<Guid>();
            if (existUsers?.Any() == true)
            {
                foreach (var user in existUsers)
                {
                    existUserNames.Add(user.nickname);
                    existUserIDs.Add(user.id);
                }
            }
            IEnumerable<TalentSetting> talentSettings = null;
            if (existUserIDs?.Any() == true)
            {
                talentSettings = await _paidDB.QueryAsync<TalentSetting>("Select * From [TalentSetting] Where TalentUserID in @ids", new { ids = existUserIDs });
            }
            var regMinTime = new DateTime(2019, 1, 1);
            var regMaxTime = new DateTime(2020, 12, 1);
            var headImageIndex = new Dictionary<string, int>();
            headImageIndex.Add("企业管理人员女", 1);
            headImageIndex.Add("企业管理人员男", 2);
            headImageIndex.Add("校长女", 3);
            headImageIndex.Add("校长男", 4);
            headImageIndex.Add("媒体人女", 5);
            headImageIndex.Add("媒体人男", 6);
            headImageIndex.Add("老师女", 7);
            headImageIndex.Add("老师男", 8);

            for (int i = 0; i < userNames.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(userNames[i])) continue;
                var userInfo = existUsers.FirstOrDefault(p => p.nickname == userNames[i]);

                //if (existUserNames.Contains(userNames[i]) && talentSettings.Any(p=>p.) ) continue;

                //https://cos.sxkid.com/images/talent/default_8.png 默认头像地址
                //对应序号
                //1.企业管理人员-女 | 2.企业管理人员-男 | 3.校长-女 | 4.校长-男 | 5.媒体人-女 | 6.媒体人-男 | 7.老师-女 | 8.老师-男
                var headImgIndex = headImageIndex.ContainsKey(headImageNames[i]) ? headImageIndex[headImageNames[i]] : 1;
                var headImgUrl = $"https://cos.sxkid.com/images/talent/default_{headImgIndex}.png";
                var userID = Guid.NewGuid();
                var existUserFlag = 0;
                if (userInfo != null)
                {
                    if (talentSettings.Any(p => p.TalentUserID == userInfo.id)) continue;
                    userID = userInfo.id;
                    existUserFlag = 1;
                }
                str_SQL = $"INSERT INTO userInfo (ID,nickname,regTime,loginTime,blockage,HeadImgUrl,introduction) VALUES ('{userID}','{userNames[i]}','{DateTimeHelper.GetRandomTime(regMinTime, regMaxTime)}','{DateTime.Now}',0,'{headImgUrl}','{introductions[i]}');";

                if (existUserFlag > 0 || _userDB.Execute(str_SQL, new { }) > 0)
                {
                    str_SQL = $@"INSERT INTO [dbo].[talent] (
	                        [id],[user_id],[certification_type],[certification_way],[certification_identity],[certification_title],[certification_explanation],[certification_preview],
                            [createdate],[certification_date],[isdelete],[type],[organization_name],[organization_code],[operation_name],[operation_phone],[certification_status],[status],
                            [supplementary_explanation],[organization_staff_count],[certification_identity_id],[invite_status],[IsInternal],[Email]
                        )
                        VALUES
	                        (
		                        NEWID(),
		                        @userID,
		                        0,
		                        0,
		                        @authName,
		                        @authName,
		                        0,
		                        @authName,
		                        @randomTime,
		                        @randomTime,
		                        0,
		                        0,
		                        NULL,
		                        NULL,
		                        NULL,
		                        NULL,
		                        1,
		                        1,
		                        NULL,
		                        8,
		                        'BC220654-2A9A-4BCB-B75F-E7B44054B580',
		                        0,
		                        1,
		                        @email 
	                        );";
                    var randomTime = DateTimeHelper.GetRandomTime(regMinTime, regMaxTime);
                    if (_userDB.Execute(str_SQL, new { userID, authName = authNames[i], randomTime, email = emails[i] }) > 0)
                    {
                        var level = levels.FirstOrDefault(p => p.Name == levelNames[i]);
                        if (level == null) level = levels.OrderBy(p => p.Sort).First();
                        var entity_Setting = new TalentSetting()
                        {
                            IsEnable = true,
                            Price = prices[i],
                            TalentLevelTypeID = level.ID,
                            TalentUserID = userID
                        };
                        _paidDB.Insert(entity_Setting);
                        //_paidDB.Execute("Insert Into TalentSetting", new { });
                        if (regionNames[i]?.Any() == true)
                        {
                            foreach (var item in regionNames[i])
                            {
                                var region = regions.FirstOrDefault(p => p.Name == item);
                                if (region != null)
                                {
                                    var entity_talentRegion = new TalentRegion()
                                    {
                                        ID = Guid.NewGuid(),
                                        RegionTypeID = region.ID,
                                        UserID = userID
                                    };
                                    _paidDB.Insert(entity_talentRegion);
                                }
                            }
                        }
                        //_paidDB.Execute("Insert Into TalentRegion", new { });

                        if (gradeNames[i]?.Any() == true)
                        {
                            foreach (var item in gradeNames[i])
                            {
                                var grade = grades.FirstOrDefault(p => p.Name == item);
                                if (grade != null)
                                {
                                    str_SQL = $@"INSERT INTO [dbo].[TalentGrade] ( [ID], [GradeID], [TalentUserID] )
                                    VALUES
	                                    ( newid(), @gradeID, @userID );";
                                    _paidDB.Execute(str_SQL, new { gradeID = grade.ID, userID });
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        [HttpPost]
        public async Task<ResponseResult> UpdateTalentsRegion()
        {
            var result = ResponseResult.Success();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var userNames = new List<string>();
            var regionNames = new List<string[]>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                userNames.Add(row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim());
                regionNames.Add(row.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK)?.StringCellValue.Trim().Split("\n"));
            }
            var str_SQL = $@"Select ui.* from userInfo as ui JOIN talent as t on t.user_id = ui.id WHERE ui.nickname in @userNames";
            var existUserNames = new List<string>();
            var existUsers = await _userDB.QueryAsync<dynamic>(str_SQL, new { userNames });
            var regions = await _paidDB.QueryAsync<RegionType>("Select * from RegionType", new { });
            var talentUserIDs = new List<Guid>();
            if (existUsers?.Any() == true)
            {
                foreach (var user in existUsers)
                {
                    talentUserIDs.Add(user.id);
                    existUserNames.Add(user.nickname);
                }
            }

            str_SQL = "Select * from [TalentSetting] Where TalentUserID in @talentUserIDs";
            var talentSettings = await _paidDB.QueryAsync<TalentSetting>(str_SQL, new { talentUserIDs });

            for (int i = 0; i < userNames.Count(); i++)
            {
                if (!existUserNames.Contains(userNames[i]) || string.IsNullOrWhiteSpace(userNames[i])) continue;
                var entity_UserInfo = existUsers.FirstOrDefault(p => p.nickname == userNames[i]);
                if (regionNames[i]?.Any() == true)
                {
                    var talentSetting = talentSettings.FirstOrDefault(p => p.TalentUserID == entity_UserInfo.id);
                    if (talentSetting == null) continue;
                    str_SQL = "Delete From [TalentRegion] Where UserID = @talentUserID";
                    var deleteResult = _paidDB.Execute(str_SQL, new { talentSetting.TalentUserID });
                    foreach (var item in regionNames[i])
                    {
                        var region = regions.FirstOrDefault(p => p.Name == item);
                        if (region != null)
                        {
                            var entity_talentRegion = new TalentRegion()
                            {
                                ID = Guid.NewGuid(),
                                RegionTypeID = region.ID,
                                UserID = talentSetting.TalentUserID
                            };
                            _paidDB.Insert(entity_talentRegion);
                        }
                    }
                }
            }

            return result;
        }

        [HttpPost]
        public ResponseResult UploadQuestions()
        {
            var result = ResponseResult.Success();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var answerNames = new List<string>();
            var newAnswerNames = new List<string>();
            var tagNames = new List<string>();
            var scores = new List<double>();
            var comments = new List<string>();
            var hotTypeNames = new List<string>();
            var questions = new List<string>();

            for (int i = 3; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                //userNames.Add(row.Cells.First().StringCellValue?.Trim());
                answerNames.Add(row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                newAnswerNames.Add(row.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());

                tagNames.Add(row.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                scores.Add(row.GetCell(6, MissingCellPolicy.CREATE_NULL_AS_BLANK).NumericCellValue);
                comments.Add(row.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                hotTypeNames.Add(row.GetCell(9, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                questions.Add(row.GetCell(10, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
            }

            var tmp_NickNames = new List<string>();
            tmp_NickNames.AddRange(answerNames.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());
            tmp_NickNames.AddRange(newAnswerNames.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());

            var str_SQL = $"Select ui.* from talent as t LEFT JOIN userInfo as ui on ui.id = t.user_id WHERE ui.nickname in ('{string.Join("','", tmp_NickNames)}');";

            var userInfos = _userDB.Query<PMS.UserManage.Domain.Entities.UserInfo>(str_SQL, new { });
            var tags = _paidDB.Query<EvaluateTags>("Select * from EvaluateTags WHERE Name in @names", new { names = tagNames.Distinct() })?.ToList();
            var hotTypes = _paidDB.GetAll<HotType>();
            var talentSettings = _paidDB.Query<TalentSetting>("Select * From TalentSetting Where TalentUserID in @ids", new { ids = userInfos.Select(p => p.Id) });
            var regMinTime = new DateTime(2019, 1, 1);
            var regMaxTime = new DateTime(2021, 2, 5);
            var random = new Random();
            var insertCount = 0;
            var hotQuestionNotExistCount = 0;
            for (int i = 0; i < questions.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(questions[i])) continue;

                var orderID = _paidDB.QuerySingle<Guid>($"Select Top 1 orderID from Message WHERE Content LIKE '%{questions[i].Trim()}%'", new { });
                if (orderID != Guid.Empty)
                {
                    if (_paidDB.QuerySingle<int>($"Select Count(1) From HotQuestion where orderID = @orderID", new { orderID }) < 1)
                    {
                        hotQuestionNotExistCount++;
                    }
                    continue;
                }

                var anwserNickname = string.IsNullOrWhiteSpace(newAnswerNames[i]) ? answerNames[i] : newAnswerNames[i];

                var answer = userInfos.FirstOrDefault(p => p.NickName == anwserNickname);
                var answerSetting = talentSettings.FirstOrDefault(p => p.TalentUserID == answer.Id);
                var randomTime = DateTimeHelper.GetRandomTime(regMinTime, regMaxTime);
                var finishTime = randomTime.AddSeconds(random.Next(10000, 20000));
                var entity_Order = new Order()
                {
                    Amount = answerSetting.Price,
                    AnonyName = GenerateAnonyName(),
                    AnswerID = answer.Id,
                    AskRemainCount = 8,
                    CreateTime = randomTime,
                    CreatorID = new Guid("402E2D80-B7B7-4DAA-B381-FAB2A34CD372"),
                    FinishTime = finishTime,
                    FirstReplyTimespan = random.Next(9888, 30000),
                    ID = Guid.NewGuid(),
                    IsAnonymous = true,
                    IsBlocked = false,
                    IsEvaluate = true,
                    IsPublic = true,
                    IsRefusTransiting = false,
                    IsReply = true,
                    IsSendTransitingMsg = false,
                    NO = $"PAIDQA{_sxbGenerateNo.GetNumber()}",
                    OrginAskID = null,
                    Status = OrderStatus.Finish,
                    UpdateTime = randomTime.AddSeconds(random.Next(10000, 20000))
                };
                if (_paidDB.Insert(entity_Order) > 0)
                {
                    var messageContent = new TXTMessage()
                    {
                        Content = questions[i],
                        Images = new List<string>()
                    };
                    var message = new PMS.PaidQA.Domain.Entities.Message()
                    {
                        ID = Guid.NewGuid(),
                        Content = JsonConvert.SerializeObject(messageContent),
                        CreateTime = randomTime.AddSeconds(1),
                        IsValid = true,
                        MediaType = MsgMediaType.TXT,
                        MsgType = MsgType.Question,
                        OrderID = entity_Order.ID,
                        ReadTime = randomTime.AddSeconds(random.Next(30, 100)),
                        ReceiveID = answer.Id,
                        SenderID = entity_Order.CreatorID
                    };
                    _paidDB.Insert(message);


                    var commentTag = tags.FirstOrDefault(p => p.Name == tagNames[i]);
                    if (commentTag == null)
                    {
                        commentTag = new EvaluateTags()
                        {
                            ID = Guid.NewGuid(),
                            Name = tagNames[i],
                            IsValid = true
                        };
                        if (_paidDB.Insert(commentTag) > 0)
                        {
                            tags.Add(commentTag);
                        }
                    }
                    var entity_Comment = new Evaluate()
                    {
                        Content = comments[i],
                        CreateTime = finishTime.AddSeconds(random.Next(65, 500)),
                        ID = Guid.NewGuid(),
                        IsAuto = false,
                        IsValid = true,
                        OrderID = entity_Order.ID,
                        Score = ((int)scores[i] * 2)
                    };
                    _paidDB.Insert(entity_Comment);
                    var entity_CommentRelation = new EvaluateTagRelation()
                    {
                        ID = Guid.NewGuid(),
                        EvaluateID = entity_Comment.ID,
                        TagID = commentTag.ID
                    };
                    _paidDB.Insert(entity_CommentRelation);

                    if (hotTypes.Any(p => p.Name == hotTypeNames[i]))
                    {
                        var hottype = hotTypes.FirstOrDefault(p => p.Name == hotTypeNames[i]);
                        var sort = _paidDB.QuerySingle<int>("Select isnull(max (Sort),1) from HotQuestion;", new { });
                        var entity_HotQuestion = new HotQuestion()
                        {
                            HotTypeID = hottype.ID,
                            ID = Guid.NewGuid(),
                            OrderID = entity_Order.ID,
                            Sort = ++sort
                        };
                        _paidDB.Insert(entity_HotQuestion);
                    }
                    insertCount++;
                }
            }
            result.Data = new { insertCount, questionCount = questions.Count(), hotQuestionNotExistCount };
            return result;
        }

        [HttpPost]
        [Description("上传案例")]
        public ResponseResult UploadCase()
        {
            var result = ResponseResult.Success();

            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var answerNames = new List<string>();
            var hotTypeNames = new List<string>();
            var questions = new List<string>();
            var viewCounts = new List<int>();
            var replies = new List<string>();
            var createTimes = new List<DateTime>();


            for (int i = 3; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                var anwserName = row.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim();
                if (!string.IsNullOrWhiteSpace(anwserName))
                {
                    answerNames.Add(anwserName);
                    hotTypeNames.Add(row.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                    questions.Add(row.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                    replies.Add(row.GetCell(6, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                    createTimes.Add(row.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK).DateCellValue);
                    viewCounts.Add((int)row.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).NumericCellValue);
                }
            }

            var tmp_NickNames = new List<string>();
            tmp_NickNames.AddRange(answerNames.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());

            var str_SQL = $"Select ui.* from talent as t LEFT JOIN userInfo as ui on ui.id = t.user_id WHERE ui.nickname in ('{string.Join("','", tmp_NickNames)}');";

            var userInfos = _userDB.Query<PMS.UserManage.Domain.Entities.UserInfo>(str_SQL, new { });
            var hotTypes = _paidDB.GetAll<HotType>();
            var talentSettings = _paidDB.Query<TalentSetting>("Select * From TalentSetting Where TalentUserID in @ids", new { ids = userInfos.Select(p => p.Id) });
            var regMinTime = new DateTime(2021, 1, 1);
            var regMaxTime = DateTime.Now.AddDays(-1);
            var random = new Random();
            var insertCount = 0;
            var hotQuestionNotExistCount = 0;
            for (int i = 0; i < questions.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(questions[i])) continue;

                var orderID = _paidDB.QuerySingle<Guid>($"Select Top 1 orderID from Message WHERE Content LIKE '%{questions[i].Trim()}%'", new { });
                if (orderID != Guid.Empty)
                {
                    if (_paidDB.QuerySingle<int>($"Select Count(1) From HotQuestion where orderID = @orderID", new { orderID }) < 1)
                    {
                        hotQuestionNotExistCount++;
                    }
                    continue;
                }

                var answer = userInfos.FirstOrDefault(p => p.NickName == answerNames[i]);
                if (answer == null)
                {
                    Console.WriteLine($"userInfo not found , nickname -> {answerNames[i]}");
                    continue;
                }
                var answerSetting = talentSettings.FirstOrDefault(p => p.TalentUserID == answer.Id);
                var randomTime = DateTimeHelper.GetRandomTime(regMinTime, regMaxTime);
                var finishTime = randomTime.AddSeconds(random.Next(10000, 20000));
                var entity_Order = new Order()
                {
                    Amount = answerSetting.Price,
                    AnonyName = GenerateAnonyName(),
                    AnswerID = answer.Id,
                    AskRemainCount = 8,
                    CreateTime = randomTime,
                    CreatorID = new Guid("402E2D80-B7B7-4DAA-B381-FAB2A34CD372"),
                    FinishTime = finishTime,
                    FirstReplyTimespan = random.Next(9888, 30000),
                    ID = Guid.NewGuid(),
                    IsAnonymous = true,
                    IsBlocked = false,
                    IsEvaluate = true,
                    IsPublic = true,
                    IsRefusTransiting = false,
                    IsReply = true,
                    IsSendTransitingMsg = false,
                    NO = $"PAIDQA{_sxbGenerateNo.GetNumber()}",
                    OrginAskID = null,
                    Status = OrderStatus.Finish,
                    UpdateTime = randomTime.AddSeconds(random.Next(10000, 20000))
                };
                if (createTimes[i] > DateTime.Now.AddYears(-2000))
                {
                    entity_Order.CreateTime = createTimes[i];
                }
                if (_paidDB.Insert(entity_Order) > 0)
                {
                    var messageContent = new TXTMessage()
                    {
                        Content = questions[i],
                        Images = new List<string>()
                    };
                    var message = new PMS.PaidQA.Domain.Entities.Message()
                    {
                        ID = Guid.NewGuid(),
                        Content = JsonConvert.SerializeObject(messageContent),
                        CreateTime = randomTime.AddSeconds(1),
                        IsValid = true,
                        MediaType = MsgMediaType.TXT,
                        MsgType = MsgType.Question,
                        OrderID = entity_Order.ID,
                        ReadTime = randomTime.AddSeconds(random.Next(30, 100)),
                        ReceiveID = answer.Id,
                        SenderID = entity_Order.CreatorID
                    };
                    _paidDB.Insert(message);

                    var replyContent = new TXTMessage()
                    {
                        Content = replies[i],
                        Images = new List<string>()
                    };
                    message = new PMS.PaidQA.Domain.Entities.Message()
                    {
                        ID = Guid.NewGuid(),
                        Content = JsonConvert.SerializeObject(replyContent),
                        CreateTime = randomTime.AddSeconds(5),
                        IsValid = true,
                        MediaType = MsgMediaType.TXT,
                        MsgType = MsgType.Answer,
                        OrderID = entity_Order.ID,
                        ReadTime = randomTime.AddSeconds(random.Next(30, 100)),
                        ReceiveID = answer.Id,
                        SenderID = entity_Order.CreatorID
                    };
                    _paidDB.Insert(message);

                    var viewCount = viewCounts[i];
                    if (viewCount < 1) viewCount = random.Next(1, 1000);
                    if (hotTypes.Any(p => p.Name == hotTypeNames[i]))
                    {
                        var hottype = hotTypes.FirstOrDefault(p => p.Name == hotTypeNames[i]);
                        var sort = _paidDB.QuerySingle<int>("Select isnull(max (Sort),1) from HotQuestion;", new { });
                        var entity_HotQuestion = new HotQuestion()
                        {
                            HotTypeID = hottype.ID,
                            ID = Guid.NewGuid(),
                            OrderID = entity_Order.ID,
                            Sort = ++sort,
                            ViewCount = viewCount
                        };
                        _paidDB.Insert(entity_HotQuestion);
                    }
                    insertCount++;
                }
            }
            result.Data = new { insertCount, questionCount = questions.Count(), hotQuestionNotExistCount };
            return result;
        }

        [HttpPost]
        [Description("上传案例")]
        public async Task<ResponseResult> UploadOrderTag()
        {
            var result = ResponseResult.Success();

            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var questions = new List<string>();
            var grades = new List<string>();
            var regions = new List<string>();


            for (int i = 3; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                questions.Add(row.GetCell(10, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                grades.Add(row.GetCell(12, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
                regions.Add(row.GetCell(13, MissingCellPolicy.CREATE_NULL_AS_BLANK).StringCellValue.Trim());
            }

            var allRegions = await _paidDB.QueryAsync<RegionType>("Select * From RegionType", new { });
            var allGrades = await _paidDB.QueryAsync<RegionType>("Select * From Grade", new { });
            var insertRegionTagCount = 0;
            var insertGradeTagCount = 0;
            for (int i = 0; i < questions.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(questions[i])) continue;

                var orderID = _paidDB.QuerySingle<Guid>($"Select Top 1 orderID from Message WHERE Content LIKE '%{questions[i].Trim()}%'", new { });
                if (orderID != Guid.Empty)
                {
                    _paidDB.Execute("Delete From OrderTag Where OrderID = @id", new { id = orderID });
                    if (!string.IsNullOrWhiteSpace(regions[i]))
                    {
                        var region = allRegions.FirstOrDefault(p => p.Name == regions[i].Trim());
                        if (region != null)
                        {
                            if (_paidDB.Insert(new OrderTag()
                            {
                                ID = Guid.NewGuid(),
                                OrderID = orderID,
                                TagID = region.ID,
                                TagType = OrderTagType.Region
                            }) > 0)
                            {
                                insertRegionTagCount++;
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(grades[i]))
                    {
                        var grade = allGrades.FirstOrDefault(p => p.Name == grades[i]);
                        if (grade != null)
                        {
                            if (_paidDB.Insert(new OrderTag()
                            {
                                ID = Guid.NewGuid(),
                                OrderID = orderID,
                                TagID = grade.ID,
                                TagType = OrderTagType.Grade
                            }) > 0)
                            {
                                insertGradeTagCount++;
                            }
                        }
                    }
                }
            }
            result.Data = new
            {
                QuestionCount = questions.Count(),
                insertRegionTagCount,
                insertGradeTagCount
            };
            return result;
        }

        [HttpGet]
        [Authorize]
        public async Task<ResponseResult> Refund(string token)
        {
            if (token != "002400")
            {
                return ResponseResult.Failed("No");
            }
            Guid userID = User.Identity.GetUserInfo().UserId;
            var orders = await _OrderService.GetBy(userID);
            foreach (var order in orders)
            {
                var res = await _OrderService.Refund(order, "【测试】用户自行退款。");
            }
            return ResponseResult.Success();
        }


        [HttpPost]
        public async Task<ResponseResult> SetOrderBeforeHours(Guid id, OrderStatus statu, int hour)
        {
            Order order = new Order()
            {
                ID = id,
                CreateTime = DateTime.Now.AddHours(hour),
                UpdateTime = DateTime.Now.AddHours(hour),
                Status = statu
            };
            var result = await this._OrderService.UpdateAsync(order, new[] { "CreateTime", "UpdateTime", "Status" });
            //await this._OrderService.SetToCache(id);
            await _mediator.Publish(new OrderChangeEvent(id));
            return ResponseResult.Success(result);
        }

        [HttpPost]
        public async Task<ResponseResult> SetOrderCache(Guid id)
        {
            await _mediator.Publish(new OrderChangeEvent(id));
            return ResponseResult.Success("ok");
        }

        public async Task<ResponseResult> TestCustomMsg()
        {

            Guid userID = User.Identity.GetUserInfo().UserId;
            if (userService.TryGetOpenId(userID, "fwh", out string openID))
            {
                //发送服务号消息
                TextCustomMsg msg = new TextCustomMsg()
                {
                    ToUser = openID,
                    content = _customMsgSetting.TalentUnReplyTips.Content.Replace("{orderId}", Guid.NewGuid().ToString("N"))
                };
                var accessToken = weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                var result = await _customMsgService.Send(accessToken.token, msg);
            }

            return ResponseResult.Success("OK");
        }


        string GenerateAnonyName()
        {
            StringBuilder sb = new StringBuilder();
            char[] alphabet = new char[] {
            'a','b','c','d','e','f','g','h','i','j','k','l','n','m','o','p','q','x','w','u','v'
            };
            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                sb.Append(alphabet[random.Next(0, alphabet.Length)]);
            }
            for (int i = 0; i < 5; i++)
            {
                sb.Append(random.Next(0, 9));
            }
            return sb.ToString();
        }

        public async Task<ResponseResult> BackCoupon(Guid orderId)
        {
            var res = await _couponTakeService.BackCoupon(orderId);
            return ResponseResult.Success(res);
        }




        [HttpPost]
        public async Task<ResponseResult> InitActivityData([FromForm] CouponActivity activity, [FromForm] List<Guid> spectialTalentIds, [FromForm] List<Guid> exampleTalentIds)
        {
            var effectActivity = await _couponActivityService.GetEffectActivity();
            if (effectActivity != null)
            {
                activity = effectActivity;
            }
            else
            {
                activity.Id = Guid.NewGuid();
                bool res = await _couponActivityService.AddAsync(activity);
                if (!res)
                {
                    return ResponseResult.Failed("创建活动失败");
                }
            }
            var couponExampleQAs = new List<CouponQAExample>();
            for (int i = 0; i < exampleTalentIds.Count; i++)
            {
                couponExampleQAs.Add(new CouponQAExample()
                {
                    Id = Guid.NewGuid(),
                    ActivityId = activity.Id,
                    TalentUserId = exampleTalentIds[i],
                    Sort = i + 1
                });
            }

            var insertExampleQA = await _couponActivityService.InsertExampleQA(couponExampleQAs);
            var couponSpecialTalents = new List<CouponSpecialTalent>();
            foreach (var id in spectialTalentIds)
            {
                couponSpecialTalents.Add(new CouponSpecialTalent()
                {
                    Id = Guid.NewGuid(),
                    TalentUserId = id,
                    ActivityId = activity.Id
                });
            }
            var insertSpecialTalent = await _couponActivityService.InsertSpecialTalent(couponSpecialTalents);
            return ResponseResult.Success(new
            {
                insertExampleQA,
                insertSpecialTalent
            }, null);

        }

        public async Task<ResponseResult> MockSystemSendMsg(
            Guid orderId)
        {
            Order order = await _OrderService.GetAsync(orderId);

            //结束消息
            SystemStatuMessage timOutOverMsg = new SystemStatuMessage()
            {
                Content = "咨询时间已到"
            };
            Message sendToUser = timOutOverMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
            Message sendToTalent = timOutOverMsg.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System);
            await _messageService.SendMessage(sendToUser, timOutOverMsg);
            await _messageService.SendMessage(sendToTalent, timOutOverMsg);

            TextMessage textMessage = new TextMessage()
            {
                Content = "系统测试模拟用户发送"
            };

            Message mockUserMesage = textMessage.CreateMessage(order.AnswerID, order.CreatorID, order.ID, MsgType.Answer);
            await _messageService.SendMessage(mockUserMesage, textMessage);

            return ResponseResult.Success("OK");
        }

        /// <summary>
        /// 更新内部达人资料并调整达人属性
        /// <para>v5.4.1 上学问v3.4迭代</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> ModifyTalentInfo()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();
            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var userIDs = new List<Guid>();
            var titles = new List<(Guid, string)>();
            var identities = new List<(Guid, string)>();
            var talentGrades = new List<(Guid, IEnumerable<string>)>();
            var talentRegions = new List<(Guid, IEnumerable<string>)>();
            var talentIntros = new List<(Guid, string)>();
            for (int i = 0; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (Guid.TryParse(row.GetCell(0).StringCellValue, out Guid userID))
                {
                    userIDs.Add(userID);
                    titles.Add((userID, row.GetCell(2).StringCellValue));
                    identities.Add((userID, row.GetCell(4).StringCellValue));
                    var grades = row.GetCell(5).StringCellValue.Split("\n");
                    talentGrades.Add((userID, grades));
                    var regions = row.GetCell(6).StringCellValue.Split("\n");
                    talentRegions.Add((userID, regions));
                    talentIntros.Add((userID, row.GetCell(11).StringCellValue));
                }
            }
            var talentInfos = await _userDB.QueryAsync<TalentEntity>("Select * From [Talent] Where [user_id] in @userIDs", new { userIDs });
            var userInfos = await _userDB.QueryAsync<UserInfo>("Select * from [UserInfo] Where [ID] in @userIDs", new { userIDs });
            if (talentInfos?.Count() != userInfos?.Count())
            {
                result.Msg = "达人数不等于用户数";
                return result;
            }

            var talentSQLs = new List<string>();
            var userSQLs = new List<string>();
            var talentGradeSQLs = new List<string>();
            var talentRegionSQLs = new List<string>();

            var allTalentGrades = await _paidDB.QueryAsync<dynamic>("Select * From [TalentGrade] Where [TalentUserID] In @userIDs", new { userIDs });
            var allGrades = await _paidDB.QueryAsync<Grade>("Select * From [Grade];", new { });
            var allTalentRegions = await _paidDB.QueryAsync<TalentRegion>("Select * From [TalentRegion] Where [UserID] In @userIDs", new { userIDs });
            var allRegions = await _paidDB.QueryAsync<RegionType>("Select * From [RegionType]", new { });

            foreach (var userInfo in userInfos)
            {
                var talentInfo = talentInfos.FirstOrDefault(p => p.user_id == userInfo.Id);
                if (talentInfo != null)
                {
                    var talentSQL = "Update [Talent] Set";
                    if (talentInfo.certification_identity != identities.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2)
                    {
                        talentSQL += $" [certification_identity] = '{identities.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2}'";
                    }
                    if (talentInfo.certification_title != titles.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2)
                    {
                        if (talentSQL != "Update [Talent] Set") talentSQL += " , ";
                        talentSQL += $" [certification_title] = '{titles.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2}' , [certification_preview] = '{titles.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2}'";
                    }
                    if (talentSQL != "Update [Talent] Set")
                    {
                        talentSQL += $" Where [ID] = '{talentInfo.id}';";
                        talentSQLs.Add(talentSQL);
                    }
                }
                if (userInfo.Introduction != talentIntros.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2)
                {
                    userSQLs.Add($"Update [UserInfo] Set [introduction] = '{talentIntros.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2}' Where [ID] = '{userInfo.Id}';");
                }
                if (talentRegions.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2?.Any() == true)
                {
                    var excelRegions = allRegions.Where(p => talentRegions.FirstOrDefault(x => x.Item1 == userInfo.Id).Item2.Contains(p.Name));
                    var userRegions = allTalentRegions.Where(p => p.UserID == userInfo.Id);
                    if (excelRegions.Select(p => p.ID).Intersect(userRegions.Select(p => p.RegionTypeID ?? Guid.Empty)).Count() != userRegions.Count())
                    {
                        var str_SQL = $"Delete From [TalentRegion] Where [UserID] = '{userInfo.Id}';Insert Into [TalentRegion] Values ";
                        foreach (var region in excelRegions)
                        {
                            str_SQL += $"(newid(),'{userInfo.Id}','{region.ID}'),";
                        }
                        str_SQL = str_SQL.TrimEnd(',') + ";";
                        talentRegionSQLs.Add(str_SQL);
                    }
                }
                if (talentGrades.FirstOrDefault(p => p.Item1 == userInfo.Id).Item2?.Any() == true)
                {
                    var excelGrades = allGrades.Where(p => talentGrades.FirstOrDefault(x => x.Item1 == userInfo.Id).Item2.Contains(p.Name));
                    var userGrades = allTalentGrades.Where(p => p.TalentUserID == userInfo.Id);
                    if (excelGrades.Select(p => p.ID).Intersect(userGrades.Select(p => (Guid)p.GradeID)).Count() != userGrades.Count())
                    {
                        var str_SQL = $"Delete From [TalentGrade] Where [TalentUserID] = '{userInfo.Id}';Insert Into [TalentGrade] Values ";
                        foreach (var grade in excelGrades)
                        {
                            str_SQL += $"(newid(),'{grade.ID}','{userInfo.Id}'),";
                        }
                        str_SQL = str_SQL.TrimEnd(',') + ";";
                        talentGradeSQLs.Add(str_SQL);
                    }
                }
            }

            if (talentGradeSQLs.Any()) await _paidDB.ExecuteAsync(string.Join(" ", talentGradeSQLs), new { });
            if (talentRegionSQLs.Any()) await _paidDB.ExecuteAsync(string.Join(" ", talentRegionSQLs), new { });
            if (userSQLs.Any()) await _userDB.ExecuteAsync(string.Join(" ", userSQLs), new { });
            if (talentSQLs.Any()) await _userDB.ExecuteAsync(string.Join(" ", talentSQLs), new { });
            result = ResponseResult.Success();
            return result;
        }
#endif
    }
}