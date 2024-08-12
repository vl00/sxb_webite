using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.Search.Application.IServices;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Tool.Amap;
using ProductManagement.Tool.Amap.Result;
using Sxb.Web.Common;
using Sxb.Web.Models;
using Sxb.Web.RequestModel;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels;
using System.Text.RegularExpressions;
using Sxb.Web.ViewModels.Api;
using PMS.CommentsManage.Application.ModelDto;
using ProductManagement.Framework.WeChat;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using static PMS.UserManage.Domain.Common.EnumSet;
using Microsoft.Extensions.Configuration;
using PMS.UserManage.Application.ModelDto.Talent;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Aliyun;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly ImageSetting _setting;
        private readonly WxSetting _wx;
        private readonly ICityInfoService _cityService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISchoolService _schoolService;
        private readonly IUserServiceClient _userMessage;
        private readonly IUserService _userService;
        private readonly ITalentService _talentService;

        private readonly ISchoolCommentReplyService _schoolCommentReplyService;
        private readonly IQuestionsAnswersInfoService _questionsAnswersInfoService;
        private readonly ISchoolCommentScoreService _schoolCommentScore;

        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionInfo;

        private readonly ISearchService _searchService;

        private readonly IGiveLikeService _giveLikeService;

        private readonly ISchoolInfoService _schoolInfo;

        private readonly ISchoolScoreMQService _schoolScoreMQService;

        private readonly ISchoolQuestionTotalMQService _mQService;

        private readonly IUserGrantAuthService _userGrant;

        private readonly IMessageService _inviteStatus;

        private readonly IMessageService _message;

        private readonly IWeChatAppClient _weChatAppClient;
        private readonly IText _text;

        public CommonController(ICityInfoService cityService,
            IEasyRedisClient easyRedisClient,
            ISchoolService schoolService,
            IUserServiceClient userMessage,
            ISchoolCommentService commentService,
            IQuestionsAnswersInfoService questionsAnswersInfoService,
            IQuestionInfoService questionInfo,
            IUserService userService,
            ISearchService searchService,
            ISchoolCommentReplyService schoolCommentReplyService,
            IGiveLikeService giveLikeService,
            ISchoolCommentScoreService schoolCommentScore,
            ISchoolInfoService schoolInfo,
            ISchoolScoreMQService schoolScoreMQService,
            ISchoolQuestionTotalMQService mQService,
            IOptions<ImageSetting> set,
            IUserGrantAuthService userGrant,
            IMessageService inviteStatus,
            IMessageService message,
            IOptions<WxSetting> wx, ITalentService talentService, IWeChatAppClient weChatAppClient, IText text)
        {

            _message = message;
            _inviteStatus = inviteStatus;
            _userGrant = userGrant;
            _cityService = cityService;
            _easyRedisClient = easyRedisClient;
            _setting = set.Value;
            _wx = wx.Value;
            _userService = userService;
            _giveLikeService = giveLikeService;
            _schoolCommentReplyService = schoolCommentReplyService;
            _userMessage = userMessage;

            _questionsAnswersInfoService = questionsAnswersInfoService;
            _commentService = commentService;
            _questionInfo = questionInfo;

            _schoolService = schoolService;

            _searchService = searchService;

            _schoolCommentScore = schoolCommentScore;

            _schoolInfo = schoolInfo;

            _mQService = mQService;

            _schoolScoreMQService = schoolScoreMQService;
            _talentService = talentService;
            _weChatAppClient = weChatAppClient;
            _text = text;
        }

        [HttpGet]
        public ResponseResult ConvertNo(string no)
        {
            if (string.IsNullOrWhiteSpace(no))
            {
                return ResponseResult.Failed("no 为空");
            }

            //long 转短链 
            if (long.TryParse(no, out long noLong))
            {
                var noStr = UrlShortIdUtil.ToBase32String(no);
                return ResponseResult.Success(new
                {
                    No = noStr
                });
            }

            //短链转long
            noLong = UrlShortIdUtil.Base322Long(no);
            return ResponseResult.Success(new
            {
                No = noLong.ToString()
            });
        }

        /// <summary>
        /// 垃圾文本检测
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SimpleCheckText(string text)
        {
            var block = await _text.GarbageCheck(text);
            if (block)
            {
                return ResponseResult.Failed();
            }
            else
            {
                return ResponseResult.Success();
            }
        }

        /// <summary>
        /// 垃圾文本检测
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<ResponseResult> CheckText(string text)
        {
            var contentCheck = await _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            {
                scenes = new[] { "antispam" },
                tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                            content=text,
                            dataId = Guid.NewGuid().ToString()
                      }
                 }
            });
            return ResponseResult.Success(contentCheck.data, "参考文档: https://help.aliyun.com/document_detail/70439.html");
        }

        /// <summary>
        /// 垃圾文本检测
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<ResponseResult> PostCheckText([FromBody]string posttext)
        {
            var contentCheck = await _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            {
                scenes = new[] { "antispam" },
                tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                            content=posttext,
                            dataId = Guid.NewGuid().ToString()
                      }
                 }
            });
            return ResponseResult.Success(contentCheck.data, "参考文档: https://help.aliyun.com/document_detail/70439.html");
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            _giveLikeService.AddLike(new PMS.CommentsManage.Domain.Entities.GiveLike()
            {
                SourceId = Guid.Parse("BEECDD22-F574-472F-AD98-9167841E215E"),
                LikeType = PMS.CommentsManage.Domain.Common.LikeType.Comment,
                UserId = Guid.Parse("F975E2A3-FA89-4E2D-922B-22E42EF52231")
            }, out PMS.CommentsManage.Domain.Common.LikeStatus ob);
            return View();
        }

        [HttpPost]
        [Authorize]
        public ResponseResult AppUploadImager(List<string> imagesArr)
        {
            Guid Id = Guid.NewGuid();
            try
            {
                List<string> ImagerUrl = new List<string>();

                for (int i = 0; i < imagesArr.Count(); i++)
                {
                    string baseStr = imagesArr[i];

                    string[] sources = baseStr.Split(',');
                    //base64转字节
                    byte[] postData = Convert.FromBase64String(sources[1]);
                    //字节转image
                    Image img = GetImageExtHelper.byte2img(postData);
                    //获取图片扩展名
                    string ext = GetImageExtHelper.GetImageExt(img);
                    string commentImg = $"Comment/{Id}/{i}.{ext}";
                    //上传图片
                    int statusCode = UploadImager.UploadImagerByBase64(_setting.UploadImager + commentImg, postData);
                    if (statusCode == 200)
                    {
                        ImagerUrl.Add("/images/school_v3/" + commentImg);
                    }
                }
                return ResponseResult.Success(new { ImagerUrl, Id }, "图片上传成功");
            }
            catch (Exception)
            {
                return ResponseResult.Failed(new { Id });
            }
        }

        public async Task<ResponseResult> GetCityAreaInfo(string city = "", int cityCode = 0)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                if (string.IsNullOrEmpty(city))
                {
                    //获取当前城市信息
                    //var currentLoaction = await _amapClient.GetCurrentLocation(IPUtil.GetIpAddr(HttpContext.Request));
                    //var currentLoaction = await _amapClient.GetCurrentLocation("114.247.50.2");
                    city = "广州";
                }

                var result = await _cityService.GetAreaCode(city);

                if (userId != default(Guid))
                {
                    if (cityCode != 0)
                    {
                        var cityinfo = await _cityService.GetInfoByCityCode(cityCode);
                        string Key = $"SearchCity:{userId}";

                        await _easyRedisClient.SortedSetAddAsync(Key, cityinfo, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                    }
                }

                return ResponseResult.Success(
                    new { rows = result }
                );
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        public async Task<ResponseResult> GetUserHistorySelectCity()
        {
            try
            {
                Guid userId = default(Guid);
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                string Key = $"SearchCity:{userId}";
                var history = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 2, StackExchange.Redis.Order.Descending);
                return ResponseResult.Success(userId == default(Guid) ? null : history, "查询成功");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        public async Task<ResponseResult> GetCityInfoByPinYin(string CityPinyin)
        {
            try
            {

                List<AreaDto> areaDtos = new List<AreaDto>();
                if (CityPinyin == "" || CityPinyin.Trim() == "")
                {
                    return ResponseResult.Success(new List<AreaDto>());
                }


                //List<AreaDto> areaDtos = new List<AreaDto>()
                //{
                //    new AreaDto(){AdCode=430400,AreaName="衡阳" },
                //    new AreaDto(){AdCode=440100,AreaName="广州" }
                //};
                var rez = await _cityService.GetAllCityPinYin();
                if ((int)CityPinyin[0] > 127)
                {
                    List<CitySearchDto> citySearchDtos = new List<CitySearchDto>();
                    foreach (var item in rez)
                    {
                        citySearchDtos.AddRange(item.Value);
                    }

                    citySearchDtos.ForEach(x =>
                    {
                        if (x.Name.Contains(CityPinyin) && areaDtos.Find(y => y.AdCode == x.CityCode) == null)
                        {
                            AreaDto areaDto = new AreaDto();
                            areaDto.AdCode = x.CityCode;
                            areaDto.AreaName = x.Name;
                            areaDtos.Add(areaDto);
                        }
                    });
                }
                else
                {
                    if (!rez.ContainsKey(CityPinyin[0]))
                    {
                        return ResponseResult.Success(new List<AreaDto>());
                    }

                    rez[CityPinyin[0]].ForEach(x =>
                    {
                        if (string.Join("", x.Pinyin).Contains(CityPinyin))
                        {
                            if (areaDtos.Where(a => a.AdCode == x.CityCode).FirstOrDefault() == null)
                            {
                                AreaDto areaDto = new AreaDto();
                                areaDto.AdCode = x.CityCode;
                                areaDto.AreaName = x.Name;
                                areaDtos.Add(areaDto);
                            }
                        }
                    });
                }
                //string cityInfo = JsonConvert.SerializeObject(areaDtos);
                return ResponseResult.Success(areaDtos.Distinct());
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 添加当前用户选择的城市信息
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<ResponseResult> AddCurrentUserSelectCiyt(string city)
        {
            try
            {
                Guid userId = default(Guid);
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                var cityinfo = await _cityService.GetInfoByCityName(city);
                string Key = $"SearchCity:{userId}";

                await _easyRedisClient.SortedSetAddAsync(Key, cityinfo, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);

                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        //邀请点评
        public ResponseResult InviteComment(UserMessage message)
        {
            message.type = (int)MessageType.InviteComment;
            message.dataType = (int)MessageDataType.School;
            return PushUserMessage(message);
        }

        //邀请提问
        public ResponseResult InviteQuestion(UserMessage message)
        {
            message.type = (int)MessageType.InviteQuestion;
            message.dataType = (int)MessageDataType.School;
            return PushUserMessage(message);
        }

        //邀请回复点评
        public ResponseResult InviteReplyComment(UserMessage message)
        {
            message.type = (int)MessageType.InviteReplyComment;
            message.dataType = (int)MessageDataType.Comment;
            return PushUserMessage(message);
        }

        //邀请回复问题 = 邀请回答
        public ResponseResult InviteAnswer(UserMessage message)
        {
            message.type = (int)MessageType.InviteAnswer;
            message.dataType = (int)MessageDataType.Question;
            return PushUserMessage(message);
        }


        /// <summary>
        /// 用户中心消息推送
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ResponseResult PushUserMessage(UserMessage message)
        {
            try
            {
                Guid userId = default(Guid);
                string iSchoolAuth = "";
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;

                    iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];
                }

                //检测该用户今天对该源是否已经邀请过10人
                int invite = _userService.CheckTodayInviteSuccessTotal(userId, message.eID, message.dataID, message.type, message.dataType, DateTime.Now).Count();
                if (invite >= 10)
                {
                    return ResponseResult.Success(new { states = -1 });
                }
                else
                {
                    //检测该数据是否已经被邀请
                    bool Permissible = _userService.CheckUserisInvite(message.userID, message.type, message.dataType, userId, message.dataID, message.eID);
                    if (!Permissible)
                    {
                        //正常接口返回0
                        bool states = _message.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            Id = Guid.NewGuid(),
                            title = message.title,
                            Content = message.content,
                            DataID = message.dataID,
                            DataType = (byte)message.dataType,
                            EID = message.eID,
                            Type = (byte)message.type,
                            userID = message.userID,
                            senderID = userId,
                            IsAnony = message.IsAnony,
                            Read = false
                        });

                        //if(states == 0 && ((MessageType)message.type == MessageType.InviteComment || (MessageType)message.type == MessageType.InviteQuesion || (MessageType)message.type == MessageType.InviteAnswer))
                        //{
                        //    //调用邀请记录表 存储状态
                        //    _inviteStatus.AddInviteStatus(new List<InviteStatus>() 
                        //    {
                        //        new InviteStatus() 
                        //        { 
                        //            DataId = message.dataID,
                        //            Type = message.type,
                        //            IsRead = true,
                        //            SenderId = userId,
                        //            UserId = message.userID,
                        //            Eid = message.eID,
                        //            Content = message.content
                        //        } 
                        //    });
                        //}
                        return ResponseResult.Success(new { states });
                    }
                    else
                    {
                        return ResponseResult.Success(new { states = -2 });
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// elasticsearch学校搜索
        /// </summary>
        /// <param name="schoolName"></param>
        /// <param name="isComment">true：查询点评数据、false：</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ResponseResult GetInfoBySchoolName(string schoolName, int citycode, bool isComment, int PageIndex, int PageSize)
        {
            try
            {
                List<SchoolSectionCommentOrQuestionTotal> totals = new List<SchoolSectionCommentOrQuestionTotal>();

                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                if (citycode == 0)
                {
                    citycode = Request.GetLocalCity();
                }

                //根据学校名称模糊得到批量学校id
                List<Guid> SchoolId = new List<Guid>();
                //var result = await _searchClient.Search(new ProductManagement.API.Http.Result.GetSchoolByNameAndAdCode() { name = schoolName, curpage = PageIndex, countperpage = PageSize, citycode = citycode });
                var result = _searchService.SearchSchool(new PMS.Search.Application.ModelDto.Query.SearchSchoolQuery() { CurrentCity = citycode, CityCode = new List<int>() { citycode }, PageNo = PageIndex, PageSize = PageSize, Keyword = schoolName });


                List<SchoolNameQuery> rez = new List<SchoolNameQuery>();
                long total = 0;
                if (result != null)
                {

                    SchoolId.AddRange(result.Schools.Select(x => x.Id));

                    if (isComment)
                    {
                        totals.AddRange(_commentService.GetTotalBySchoolSectionIds(SchoolId));
                    }
                    else
                    {
                        totals.AddRange(_questionInfo.GetTotalBySchoolSectionIds(SchoolId));
                    }

                    if (result.Schools.Any())
                    {
                        //返回总学校数量
                        total = result.Total;
                        foreach (var item in result.Schools)
                        {
                            SchoolNameQuery school = new SchoolNameQuery();
                            school.SchoolId = item.Id;
                            school.School = item.SchoolId;
                            school.SchoolName = item.Name;
                            var temp = totals.FirstOrDefault(x => x.SchoolSectionId == item.Id);
                            school.CurrentTotal = temp == null ? 0 : temp.Total;
                            rez.Add(school);
                        }
                    }
                }

                return ResponseResult.Success(new
                {
                    total = total,
                    rows = rez
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        public ResponseResult UserIsAuth()
        {
            bool rez = _userGrant.IsGrantAuth(User.Identity.GetId());
            return ResponseResult.Success(rez);
        }

        //修改城市定位
        public void ChangeLocalCity(int cityCode)
        {
            Response.SetLocalCity(cityCode.ToString());
        }


        public IActionResult RedirectMiddlePage(string url)
        {

            Regex matchUrl = new Regex("^(http:|https:)?//.*$");
            if (string.IsNullOrEmpty(url) || !matchUrl.IsMatch(url))
            {
                return Redirect("/");
            }
            else
            {
                return Redirect(System.Web.HttpUtility.UrlDecode(url));
            }

        }

        //检测是否绑定电话
        [Authorize]
        public void UserIsBindPhone()
        {
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }
        }

        /// <summary>
        /// 我发布的评论+提问总数量
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetCurrentCommentAndQuestionTotal()
        {
            var user = User.Identity.GetUserInfo();
            int CommentTotal = _commentService.CommentTotal(user.UserId);
            int QuestionTotal = _questionInfo.TotalQuestion(user.UserId);
            return ResponseResult.Success(new { CommentTotal, QuestionTotal });
        }

        /// <summary>
        /// 我发布的回复+回答总数量
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetCurrentReplyAndAnswerTotal()
        {
            var user = User.Identity.GetUserInfo();
            int ReplyTotal = _schoolCommentReplyService.CommentReplyTotal(user.UserId);
            int AnswerTotal = _questionsAnswersInfoService.QuestionAnswer(user.UserId);
            return ResponseResult.Success(new { ReplyTotal, AnswerTotal });
        }

        /// <summary>
        /// 我发布的回复+回答+回复的回复+回答的回答总数量
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetCurrentPublishTotal()
        {
            UserLikeAndPublishTotal userLikeAndPublishTotal = new UserLikeAndPublishTotal();

            var user = User.Identity.GetUserInfo();
            //userLikeAndPublishTotal.ReplyTotal = _schoolCommentReplyService.CommentReplyTotal(user.UserId);
            //userLikeAndPublishTotal.AnswerTotal = _questionsAnswersInfoService.QuestionAnswer(user.UserId);
            //userLikeAndPublishTotal.ReplyReplyTotal = _schoolCommentReplyService.ReplyTotal(user.UserId);
            //userLikeAndPublishTotal.AnswerAnswerTotal = _questionsAnswersInfoService.AnswerReplyTotal(user.UserId);

            var likTotal = _giveLikeService.StatisticalSum(user.UserId);

            //userLikeAndPublishTotal.LikeCommentTotal = likTotal.LikeCommentTotal;
            //userLikeAndPublishTotal.LikeCommentReplyTotal = likTotal.LikeCommentReplyTotal;
            //userLikeAndPublishTotal.ReplyReplyTotal = likTotal.LikeReplyTotal;

            //userLikeAndPublishTotal.LikeAnswerTotal = likTotal.LikeAnswerTotal;
            //userLikeAndPublishTotal.AnswerTotal = likTotal.LikeAnswerTotal;
            userLikeAndPublishTotal.LikeTotal = likTotal.LikeTotal;
            userLikeAndPublishTotal.AnswerAndReplyTotal = likTotal.AnswerAndReplyTotal;
            userLikeAndPublishTotal.PublishTotal = likTotal.PublishTotal;


            return ResponseResult.Success(userLikeAndPublishTotal);
        }

        //[HttpPost]
        public ActionResult GetSchoolExt()
        {
            List<Guid> extId = new List<Guid>() { Guid.Parse("BE2176FC-0AE5-4A5E-BF6D-002A4AA998D5"), Guid.Parse("2BA04943-0D40-4818-8403-2BF5C8D0BF7D"), Guid.Parse("F5453059-AF33-44AB-83E9-003B3D913915") };
            var result = _schoolInfo.GetSchoolSectionByIds(extId?.ToList());
            return Json(result);
        }

        [Authorize]
        public ResponseResult CheckIsLogOut()
        {
            try
            {
                Guid userId = User.Identity.GetUserInfo().UserId;
                bool rez = _commentService.CheckLogout(userId);
                return ResponseResult.Success(rez);
            }
            catch (Exception)
            {
                return ResponseResult.Failed(false);
            }
        }

        [Authorize]
        [HttpPost]
        public ResponseResult AuthComment()
        {
            Guid userId = User.Identity.GetUserInfo().UserId;
            bool rez = _userGrant.Add(new PMS.CommentsManage.Domain.Entities.UserGrantAuth() { UserId = userId });
            return ResponseResult.Success(new { isAuthSuccess = rez });
        }

        /// <summary>
        /// 获取微信jsdk 初始化绑定参数
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GetJsdk(string url)
        {
            //string weixin_AppID = "wxeefc53a3617746e2";
            var config = Request.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            var weixin_fwh_suffix = config.GetSection("Wx").GetValue<string>("fwh_suffix");

            long timestamp = DateTime.Now.D2I();
            string nonceStr = Guid.NewGuid().ToString("N");
            var response = await _weChatAppClient.GetTicket(new WeChatGetTicketRequest() { App = "fwh" });
            string signature = WXAPIHelper.GetSign(response.ticket, nonceStr, timestamp, url);
            ShareImgLink = "https://cdn.sxkid.com/images/logo_share_v4.png";

            return ResponseResult.Success(new { weixin_AppID = response.appID, timestamp, nonceStr, signature, ShareImgLink });
        }

        /// <summary>
        /// 特定格式 微信jssdk
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<JsonResult> WeChatSDK(string url)
        {
            try
            {
                var config = Request.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

                var weixin_AppID = config.GetSection("Wx").GetValue<string>("AppId");

                long timestamp = DateTime.Now.D2I();
                string nonceStr = Guid.NewGuid().ToString("N");


                //string weixin_api_access_token = GetAccessToken_JsApi.GetValue("weixin_access_token");
                //string weixin_jsapi_ticket = GetAccessToken_JsApi.GetValue("weixin_jsapi_ticket");
                var response = await _weChatAppClient.GetTicket(new WeChatGetTicketRequest() { App = "fwh" });

                string signature = WXAPIHelper.GetSign(response.ticket, nonceStr, timestamp, url);

                ShareImgLink = "https://cdn.sxkid.com/images/logo_share_v4.png";

                return Json(new { c = 0, m = "", v = new { appid = weixin_AppID, timestamp = timestamp, noncestr = nonceStr, signature = signature } });
            }
            catch (Exception ex)
            {
                return Json(new { c = -1, m = ex.Message, v = new { appid = "", timestamp = "", noncestr = "", signature = "" } });
            }
        }

        public void Test()
        {
            //DateTime lastUpdateTime = _schoolCommentScore.GetLastUpdateTime();
            DateTime lastUpdateTime = DateTime.Parse("2019-01-01");
            DateTime nowTime = DateTime.Now;

            int pageSize = 1000;

            int total = _schoolCommentScore.SchoolCommentScoreCountByTime(lastUpdateTime, nowTime);

            int totalPage = total / pageSize + 1;

            for (int pageIndex = 1; pageIndex <= totalPage; pageIndex++)
            {
                var commentScores = _schoolCommentScore.PageSchoolCommentScoreByTime(lastUpdateTime, nowTime, pageIndex, pageSize);

                if (commentScores.Count > 0)
                {
                    var schoolSectionIds = commentScores.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToArray();

                    int i = 1;
                    foreach (var sectionId in schoolSectionIds)
                    {
                        var schoolScores = commentScores.Where(q => q.SchoolSectionId == sectionId).ToList();

                        var attendScore = schoolScores.Where(q => q.IsAttend == true).ToList();

                        SchoolScoreDto schoolScore = new SchoolScoreDto
                        {
                            SchoolId = schoolScores.FirstOrDefault().SchoolId,
                            SchoolSectionId = sectionId,
                            AggScore = schoolScores.Sum(q => q.AggScore),
                            CommentCount = schoolScores.Count,
                            AttendCommentCount = attendScore.Count,
                            EnvirScore = attendScore.Sum(q => q.EnvirScore),
                            HardScore = attendScore.Sum(q => q.HardScore),
                            LifeScore = attendScore.Sum(q => q.LifeScore),
                            ManageScore = attendScore.Sum(q => q.ManageScore),
                            TeachScore = attendScore.Sum(q => q.TeachScore),
                            LastCommentTime = schoolScores.OrderByDescending(q => q.UpdateTime).FirstOrDefault().UpdateTime
                        };
                        _schoolCommentScore.UpdateSchoolScore(schoolScore);

                        double pro = 100 * (pageIndex / Convert.ToDouble(totalPage)) * ((i++) / schoolSectionIds.Count());
                    }
                }
            }

            //把更新的所有学校分数发布到RabbitMQ
            var list = _schoolCommentScore.ListNewSchoolScores(lastUpdateTime);
            //foreach (var itemScore in list)
            //{
            //    _mqService.SendSyncSchoolScoreMessage(itemScore);
            //}

            Parallel.For(0, list.Count / 100 + 1, new ParallelOptions { MaxDegreeOfParallelism = 10 }, t =>
            {
                var result = list.Skip(t * 100).Take(100).ToList();
                _schoolScoreMQService.SendSyncSchoolScoreMessage(result);
            });
        }

        public void te()
        {
            var comment = _commentService.HottestComment(DateTime.Parse("2019-01-01"), DateTime.Parse("2019-12-31"));
            string key1 = $"Comment:Hottest";
            bool rez = _easyRedisClient.AddAsync(key1, comment, DateTime.Parse("2019-12-31")).Result;

            var CommentDtos = _easyRedisClient.GetAsync<List<SchoolCommentDto>>(key1).Result;

            string key = "Comment:HottestSchool";
            var HottestSchool = _commentService.HottestSchool(new HotCommentQuery()
            {
                StartTime = DateTime.Parse("2019-01-01"),
                EndTime = DateTime.Parse("2020-01-01")
            }, true);
            _easyRedisClient.AddAsync(key, HottestSchool);
        }


        public async Task<ResponseResult> GetProvinceInfos()
        {
            var result = await _cityService.GetProvinceInfos();
            return ResponseResult.Success(result);
        }

        public async Task<ResponseResult> GetProvinces()
        {
            var provinces = await _cityService.GetProvince();
            var result = provinces.Select(q => new
            {
                id = q.ProvinceId,
                name = q.Province
            });
            return ResponseResult.Success(result);
        }

        public ResponseResult GetCitys(int parentId)
        {
            var citys = _cityService.GetCityCodes(parentId);
            var result = citys.Select(q => new
            {
                q.Id,
                q.Name
            });
            return ResponseResult.Success(result);
        }

        [Authorize]
        public ResponseResult RecommendTalent([FromQuery] TalentRecommend recommend)
        {
            recommend.CityCode = Request.GetLocalCity();
            recommend.LoginUserId = userId.Value;
            var data = _talentService.GetRecommendTalents(recommend);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 根据城市代码获取城市名称
        /// </summary>
        /// <param name="cityCode">城市代码</param>
        /// <returns></returns>
        public async Task<ResponseResult> GetCityName(int cityCode)
        {
            var cityName = await _cityService.GetCityName(cityCode);
            if (string.IsNullOrWhiteSpace(cityName)) return ResponseResult.Failed(ResponseCode.NoFound);
            return ResponseResult.Success(cityName, "success");
        }



        /// <summary>
        /// 小程序关注公众号二维码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult FollowFwhQR()
        {
            return View();
        }
    }
}
