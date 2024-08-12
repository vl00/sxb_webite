using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.School.Application.IServices;
using PMS.School.Domain.Entities;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Web.Models.Comment;
using Newtonsoft.Json;
using PMS.CommentsManage.Domain.Common;
using System.Text;
using System.Net;
using System.IO;
using Sxb.Web.Models;
using Microsoft.Extensions.Options;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models.Tag;
using Sxb.Web.ViewModels.School;
using Sxb.Web.Response;
using Microsoft.AspNetCore.Authorization;
using PMS.CommentsManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using Sxb.Web.ViewModels;
using Sxb.Web.Utils;
using ProductManagement.Tool.Amap;
using ProductManagement.Framework.Cache.Redis;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using Sxb.Web.Common;
using System.ComponentModel;
using ProductManagement.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using NLog;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Aliyun;
using static PMS.UserManage.Domain.Common.EnumSet;
using Sxb.Web.Authentication.Attribute;

namespace Sxb.Web.Controllers
{
    [Authorize]
    public class CommentWriterController : BaseController
    {
        private readonly IMessageService _message;

        //配置文件内容
        private readonly ImageSetting _setting;
        private readonly ISchoolService _schoolService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly ISchoolTagService _schoolTagService;
        private readonly ITagService _tagService;
        private readonly ISchoolImageService _imageService;
        private readonly IUserService _userService;
        private IPartTimeJobAdminRolereService _adminRolereService;

        private readonly IPartTimeJobAdminService _adminService;

        private readonly ISchoolInfoService _schoolInfo;

        private readonly IUserGrantAuthService _userGrantAuthService;

        private ISettlementAmountMoneyService _amountMoneyService;

        private readonly IEasyRedisClient _easyRedisClient;
        readonly ICityInfoService _cityService;
        Logger _log;


        private readonly IText _text;

        private readonly IMapper _mapper;

        public CommentWriterController(IEasyRedisClient easyRedisClient,
            ICityInfoService cityService, IOptions<ImageSetting> set, ISchoolInfoService schoolInfoService,
            ISchoolService schoolService, ISchoolCommentService schoolComment, ISchoolCommentScoreService commentScoreService,
            ISchoolTagService schoolTagService, ITagService tagService, ISchoolImageService imageService, IUserGrantAuthService userGrantAuthService,
            IUserService userService, IPartTimeJobAdminService adminService, IPartTimeJobAdminRolereService adminRolereService, ISettlementAmountMoneyService amountMoneyService,
            IText text, IMapper mapper, IMessageService message)
        {
            _message = message;

            _log = LogManager.GetCurrentClassLogger();

            _easyRedisClient = easyRedisClient;
            _cityService = cityService;

            _userGrantAuthService = userGrantAuthService;
            _schoolService = schoolService;
            _commentService = schoolComment;
            _commentScoreService = commentScoreService;
            _schoolTagService = schoolTagService;
            _tagService = tagService;
            _setting = set.Value;
            _imageService = imageService;
            _mapper = mapper;
            _userService = userService;
            _schoolInfo = schoolInfoService;

            _adminRolereService = adminRolereService;

            _amountMoneyService = amountMoneyService;

            _adminService = adminService;

            _text = text;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            //GetTasByBranch(Guid.Parse("11B609F0-4C97-4401-9619-007708D117A8"));

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            }

            return View();
        }

        /// <summary>
        /// 写点评页
        /// </summary>
        /// <param name="currentId">操作的分部id</param>
        /// <param name="SchoolId">学校id</param>
        /// <returns></returns>
        [Description("写点评页")]
        public IActionResult CommitCommentView(Guid currentId, Guid SchoolId, string Entrance)
        {
            var redirectUrl = $"/comment/Write?eid={currentId}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 301;
            return Json(new { });

            ////检测当前用户身份 （true：校方用户 |false：普通用户）
            //ViewBag.isSchoolRole = false;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            //}

            //Guid userId = Guid.Empty;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    userId = user.UserId;
            //}

            //var allSchoolExt = _schoolInfo.GetCurrentSchoolAllExt(SchoolId);

            //var currentSchool = allSchoolExt.Where(x=>x.SchoolSectionId == currentId).FirstOrDefault();
            //if (currentSchool == null)
            //{
            //    throw new NotFoundException();
            //}

            //ViewBag.CurrentSchool = currentSchool;

            //ViewBag.Tag = _schoolTagService.GetSchoolTagBySchoolId(currentId).Select(x => x.Tag).ToList();
            //ViewBag.AllSchoolBranch = allSchoolExt.Where(x => x.SchoolSectionId != currentId);

            //List<AreaDto> history = new List<AreaDto>();
            //if (userId != default(Guid))
            //{
            //    string Key = $"SearchCity:{userId}";
            //    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //    history.AddRange(historyCity.ToList());
            //}


            //ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            //ViewBag.History = JsonConvert.SerializeObject(history);

            //var adCodes = await _cityService.IntiAdCodes();

            ////获取当前用户所在城市
            //var localCityCode = Request.GetLocalCity();

            ////将数据保存在前台
            //ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            ////城市编码
            //ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            ////保存学校id
            //ViewBag.SchoolId = SchoolId;

            ////检测是否成功授权过
            ////ViewBag.IsWriteAuth = _commentService.UserAgreement(userId);

            //ViewBag.IsWriteAuth = _userGrantAuthService.IsGrantAuth(userId);

            //return View();            ////检测当前用户身份 （true：校方用户 |false：普通用户）
            //ViewBag.isSchoolRole = false;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            //}

            //Guid userId = Guid.Empty;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    userId = user.UserId;
            //}

            //var allSchoolExt = _schoolInfo.GetCurrentSchoolAllExt(SchoolId);

            //var currentSchool = allSchoolExt.Where(x=>x.SchoolSectionId == currentId).FirstOrDefault();
            //if (currentSchool == null)
            //{
            //    throw new NotFoundException();
            //}

            //ViewBag.CurrentSchool = currentSchool;

            //ViewBag.Tag = _schoolTagService.GetSchoolTagBySchoolId(currentId).Select(x => x.Tag).ToList();
            //ViewBag.AllSchoolBranch = allSchoolExt.Where(x => x.SchoolSectionId != currentId);

            //List<AreaDto> history = new List<AreaDto>();
            //if (userId != default(Guid))
            //{
            //    string Key = $"SearchCity:{userId}";
            //    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //    history.AddRange(historyCity.ToList());
            //}


            //ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            //ViewBag.History = JsonConvert.SerializeObject(history);

            //var adCodes = await _cityService.IntiAdCodes();

            ////获取当前用户所在城市
            //var localCityCode = Request.GetLocalCity();

            ////将数据保存在前台
            //ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            ////城市编码
            //ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            ////保存学校id
            //ViewBag.SchoolId = SchoolId;

            ////检测是否成功授权过
            ////ViewBag.IsWriteAuth = _commentService.UserAgreement(userId);

            //ViewBag.IsWriteAuth = _userGrantAuthService.IsGrantAuth(userId);

            //return View();
        }
        [Description("写点评页获取学校标签")]
        public PageResult<List<TagVo>> GetTagBySchoolId(Guid SchoolId)
        {
            try
            {
                var Tags = _schoolTagService.GetSchoolTagBySchoolId(SchoolId).Select(x => x.Tag).ToList();
                var tagvos = _mapper.Map<List<CommentTag>, List<TagVo>>(Tags);
                return new PageResult<List<TagVo>>()
                {
                    StatusCode = 200,
                    rows = tagvos
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<TagVo>>()
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }


        [Authorize]
        [BindMobile]
        [Description("提交点评")]
        public ResponseResult CommitComment(CommentWriterVo comment, Guid commentId, List<string> imgUrls, List<CommentTag> Tags)
        {
            try
            {
                Guid userId = Guid.Empty;
                UserRole userRole = new UserRole();
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                    userRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
                }

                var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=comment.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result;

                bool checkWriter = true;
                List<int> roles = new List<int>();

                var Job = _adminService.GetModelById(userId);
                //检测该用户是否为存在于兼职管理平台中的用户，平台用户写点评不允许存在相同点评
                if (Job != null)
                {
                    checkWriter = _commentService.Checkisdistinct(comment.Content);
                    roles = _adminRolereService.GetPartJobRoles(userId).Select(x => x.Role).ToList();
                }

                //检测是否为兼职领队用户，领队用户写点评，将自动新建兼职身份
                if (roles.Contains(2) && !roles.Contains(1) && Job.SettlementType == SettlementType.SettlementWeChat)
                {
                    //包含兼职领队身份，但还并未包含兼职身份时，写点评需要新增兼职用户身份，且直接归属到自身下的兼职

                    //新增兼职身份
                    _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = userId, Role = 1, ParentId = userId });

                    //新增任务周期
                    _amountMoneyService.NextSettlementData(Job, 1);
                }

                if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
                {
                    return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
                }

                if (!checkWriter)
                {
                    return ResponseResult.Failed("该评论信息不符合规则");
                }

                //获取前端post过来的点评对象
                //CommentWriterVo comment = JsonConvert.DeserializeObject<CommentWriterVo>(HttpContext.Request.Form["comment"]);

                //根据学校id得到该学校详情信息
                //var extension = _schoolService.GetSchoolExtension(comment.SchoolId);

                //首先创建点评对象，得到该点评id，进行拼接点评图片路径
                SchoolComment schoolComment = _mapper.Map<CommentWriterVo, SchoolComment>(comment);

                schoolComment.Id = commentId;
                schoolComment.SchoolId = comment.Sid;
                schoolComment.SchoolSectionId = comment.SchoolId;
                schoolComment.CommentUserId = userId;
                schoolComment.PostUserRole = userRole;

                _log.Debug($"点评调试{schoolComment.Content},{schoolComment.SchoolSectionId},{schoolComment.SchoolId}");


                List<SchoolImage> imgs = new List<SchoolImage>();
                //上传图片
                if (imgUrls.Count() > 0)
                {
                    //标记该点评有上传过图片
                    schoolComment.IsHaveImagers = true;

                    //需要上传图片
                    imgs = new List<SchoolImage>();

                    for (int i = 0; i < imgUrls.Count(); i++)
                    {
                        SchoolImage commentImg = new SchoolImage
                        {
                            DataSourcetId = schoolComment.Id,
                            ImageType = ImageType.Comment,
                            ImageUrl = imgUrls[i]
                        };

                        //图片上传成功，提交数据库
                        imgs.Add(commentImg);
                    }
                }



                //获取该用户所选，与新添加的标签
                //List<CommentTag> Tags = JsonConvert.DeserializeObject<List<CommentTag>>(HttpContext.Request.Form["Tags"]);
                if (schoolComment.SchoolId == default(Guid))
                {
                    schoolComment.SchoolId = _schoolInfo.GetSchoolName(new List<Guid>() { schoolComment.SchoolSectionId }).FirstOrDefault().SchoolId;
                }

                _commentService.AddSchoolComment(schoolComment);

                _log.Debug($"点评调试{schoolComment.Content},{schoolComment.SchoolSectionId},{schoolComment.SchoolId}");

                SchoolCommentScore commentScore = _mapper.Map<CommentWriterVo, SchoolCommentScore>(comment);
                commentScore.CommentId = schoolComment.Id;
                //已入读，录入该点评本次平均分
                if (commentScore.IsAttend)
                {
                    //总分录入平均分
                    commentScore.AggScore = Convert.ToInt32(commentScore.AggScore) / 5;
                }



                _commentScoreService.AddSchoolComment(commentScore);


                //所有邀请我点评这个学校的消息, 全部设为已读,已处理
                _message.UpdateMessageHandled(true, MessageType.InviteComment, MessageDataType.School, userId, default, new List<Guid>() { schoolComment.SchoolSectionId });

                foreach (CommentTag tag in Tags)
                {
                    if (tag.Id == default(Guid))
                    {
                        var newTag = _tagService.CheckTagIsExists(tag.Content);
                        //检测该标签是否存在
                        if (newTag == null)
                        {
                            //标签不存在
                            tag.Id = Guid.NewGuid();
                            _tagService.Add(tag);

                            SchoolTag schoolTag = new SchoolTag
                            {
                                TagId = tag.Id,
                                SchoolCommentId = schoolComment.Id,
                                UserId = userId,
                                SchoolId = schoolComment.SchoolSectionId
                            };
                            _schoolTagService.Add(schoolTag);
                        }
                        else
                        {
                            //标签已存在，添加关系
                            SchoolTag schoolTag = new SchoolTag
                            {
                                TagId = newTag.Id,
                                SchoolCommentId = schoolComment.Id,
                                UserId = userId,
                                SchoolId = schoolComment.SchoolSectionId
                            };
                            _schoolTagService.Add(schoolTag);
                        }
                    }
                    else
                    {
                        SchoolTag schoolTag = new SchoolTag
                        {
                            TagId = tag.Id,
                            SchoolCommentId = schoolComment.Id,
                            UserId = userId,
                            SchoolId = schoolComment.SchoolSectionId
                        };
                        _schoolTagService.Add(schoolTag);
                    }
                }

                if (imgs.Count != 0)
                {
                    foreach (var img in imgs)
                    {
                        _imageService.AddSchoolImage(img);
                    }
                }

                _log.Debug($"点评调试{schoolComment.Content},{schoolComment.SchoolSectionId},{schoolComment.SchoolId}");


                SuccessCommentViewModel successComment = new SuccessCommentViewModel
                {
                    DataId = schoolComment.Id,
                    SchoolId = schoolComment.SchoolSectionId
                };

                return ResponseResult.Success(successComment);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        public ModelResult<int> ScoolCommentAddreps(int code, string msg)
        {
            return new ModelResult<int>()
            {
                StatusCode = code,
                Message = msg
            };
        }

        /// <summary>
        /// 获取分部下的所有标签
        /// </summary>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        [Description("获取分部下的所有标签")]
        public PageResult<List<CommentTag>> GetTasByBranch(Guid BranchId)
        {
            try
            {
                List<CommentTag> tags = new List<CommentTag>();
                var SchoolTags = _schoolTagService.GetSchoolTagBySchoolId(BranchId);
                if (SchoolTags != null)
                {
                    SchoolTags.ForEach(x => tags.Add(x.Tag));
                }
                return new PageResult<List<CommentTag>>()
                {
                    StatusCode = 200,
                    rows = tags
                };
            }
            catch (Exception)
            {
                return new PageResult<List<CommentTag>>()
                {
                    StatusCode = 500
                };
            }
        }
        [Description("点评提交成功页")]
        public IActionResult CommentSuccess(Guid SchoolId, Guid CommentId)
        {

            string ShareLink = "/SchoolComment/CommentDetail" + ShareUrlParameterJoin.ParameterJoin(HttpContext.Request.Query, new List<string>() { "SchoolId" });

            ViewBag.ShareCommentUrl = $"/Wechat/Index?type=2&SchoolId={SchoolId}&DataSource={CommentId}";

            var comment = _commentService.QuerySchoolComment(CommentId);
            var user = _userService.ListUserInfo(new List<Guid>() { comment.CommentUserId }).FirstOrDefault();
            var school = _schoolInfo.GetSchoolName(new List<Guid>() { SchoolId }).FirstOrDefault();


            ViewBag.School = school;
            ViewBag.SchoolId = SchoolId;
            ViewBag.CommentId = CommentId;

            ViewBag.ShareLink = ShareLink;

            ViewBag.Titel = user.NickName + $"刚点评了 {school.SchoolName}，快来了解下吧!\n{school.CommentTotal}评论";
            ViewBag.Content = school.SchoolName;

            return View();
        }

        [Authorize]
        [Description("推送学校点评")]
        public PageResult<List<CommentList.SchoolInfoVo>> PushSchoolSelectedComment(Guid SchoolId, int PageIndex, int PageSize)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                }

                //var extent = _schoolService.GetSchoolExtension(SchoolId);
                //////根据写点评的学校id，获取该学校的类型
                //var school = await _schoolService.GetSchoolExtSimpleDtoAsync(0, 0, SchoolId, extent.SchoolId);

                //string key = $"IdenticalSchool:Province:{school.Province}&City:{school.City}&Area:{school.Area}&SchoolGrade:{extent.SchoolGrade}&SchoolType:{extent.SchoolType}";

                ////根据该类型得到该相同类型的学校，进行推送
                //var result = await _easyRedisClient.GetOrAddAsync(key, async () =>
                //{
                //    //return await _schoolService.GetSchoolExtListAsync(0, 0, school.Province, school.City, school.Area,new int[] { (int)extent.SchoolGrade }, 0,new int[] { (int)extent.SchoolType }, 1,new int[] { 10 });
                //    return _schoolService.GetIdenticalSchool(SchoolId, 1, 10).Result;
                //}, new TimeSpan(0, 30, 0));

                var result = _schoolService.GetIdenticalSchool(SchoolId, 1, 10).Result;

                //获取相同类型学校
                //var result = await _schoolService.GetIdenticalSchool(SchoolId, 1, 10);

                //接口调用，根据关键字模糊查询，得到学校集合id
                List<Guid> schoolIds = new List<Guid>();
                schoolIds.AddRange(result.Select(x => x.ExtId));

                //List<CommentList> comments = new List<CommentList>();
                List<CommentList.SchoolInfoVo> rez = new List<CommentList.SchoolInfoVo>();

                if (schoolIds.Count() == 0)
                {
                    schoolIds.AddRange(_commentService.GetHotSchoolSectionId());
                    //comments.AddRange(_mapper.Map<List<SchoolCommentDto>, List<CommentList>>(_commentService.PushSchoolInfo(schoolIds, userId)));
                }

                if (schoolIds.Any())
                {
                    //foreach (var x in schoolIds)
                    //{
                    // var schoolInfo = await _schoolInfo.QuerySchoolInfo(x);
                    //    rez.Add(_mapper.Map<SchoolInfoDto, CommentList.SchoolInfo>(schoolInfo));
                    //}

                    var schoolInfo = _schoolInfo.GetSchoolSectionByIds(schoolIds);
                    rez.AddRange(_mapper.Map<List<SchoolInfoDto>, List<CommentList.SchoolInfoVo>>(schoolInfo));
                }

                return new PageResult<List<CommentList.SchoolInfoVo>>()
                {
                    StatusCode = 200,
                    rows = rez
                };
            }
            catch (Exception ex)
            {
                return new PageResult<List<CommentList.SchoolInfoVo>>()
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }
    }
}